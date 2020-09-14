using DataCatch.Common;
using DataCatch.Douban.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using System.Text.Json;
using Microsoft.International.Converters.PinYinConverter;
using System.Text;
using Newtonsoft.Json;

namespace DataCatch.Emoji
{
    public class EmojiAll : IProcess
    {
        public static bool IsRunning { get; set; }
        /// <summary>
        /// EmojiAll
        /// </summary>
        private static readonly string ApiUrl = Config.Jconfig.Apis.EmojiAll;
        private readonly string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Emoji");
        /// <summary>
        /// GET
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public async Task<string> ApiGet(string api) =>
            await new HttpClient().GetAsync(api).Result.Content.ReadAsStringAsync();
        /// <summary>
        /// GET API
        /// </summary>
        /// <param name="paramters"></param>
        /// <returns></returns>
        public string GetApiUrl(Dictionary<string, string> paramters) =>
            $"{ApiUrl}?{string.Join("&", paramters.Select(p => $"{p.Key}={p.Value}"))}";
        public async Task Start()
        {
            if (IsRunning)
            {
                Log.Info("Process Emoji is still on running");
                return;
            }
            IsRunning = true;
            Log.Info("Process Emoji is starting");

            var emojis = new List<Emoji>();
            for (int i = 1; i < 15; i++)
            {
                var url = GetApiUrl(new Dictionary<string, string> { { "page", $"{i}" } });
                Log.Debug($"Emoji current api url: {url}");
                await Catch(url, emojis);
            }
            SaveEmoji(emojis);
            SaveEmojiText(emojis);
            IsRunning = false;
            Log.Info("Process Emoji is end");
        }

        public async Task Catch(string url, List<Emoji> emos)
        {
            var data = await ApiGet(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(data);

            var htmlNodes = htmlDoc.DocumentNode;
            var subNodes = htmlNodes
                .SelectNodes("/html/body/section/div[2]/div[2]/div/div/div[1]/div[2]/div/div/div/div[2]/table/tr[position()>1]");

            Log.Debug($"Current emoji get nodes count: {subNodes.Count}");
            foreach (var node in subNodes)
            {
                var sub = node
                    .SelectNodes("td")
                    .Select(a => a.InnerText.Trim())
                    .ToList();

                Log.Debug(string.Format("Get emojis: {0}, {1}, {2}", sub.ToArray()));

                var pinyin = new List<string>();
                foreach (var ch in sub[2]
                            .Replace("“", "")
                            .Replace("”", ""))
                {
                    // check if chinese charcter
                    if (ch >= 0x4e00 && ch <= 0x9fbb)
                    {
                        var pys = new ChineseChar(ch).Pinyins;
                        pinyin.Add(pys[0]
                            .ToLower()
                            .Substring(0, pys[0].Length - 1));
                    }
                    else
                        pinyin.Add(ch.ToString());
                }

                emos.Add(new Emoji()
                {
                    Emotion = sub[0],
                    Unicode = sub[1],
                    Descript = sub[2],
                    Pinyin = string.Join("'", pinyin)
                });

                Log.Debug($"Get emoji description pinyin: {sub[0]}, {sub[1]}, {sub[2]}, {string.Join("'", pinyin)}");
            }
        }

        public void SaveEmoji(List<Emoji> emos)
        {
            var data = JsonConvert.SerializeObject(emos, Formatting.Indented);

            Directory.CreateDirectory(BasePath);

            var tasks = new List<Task>
            {
                new Task(async () =>
                    await File.WriteAllTextAsync(
                            Path.Combine(BasePath, "data.json"),
                            data
                    )
            )
            };

            tasks.Add(new Task(async () =>
                await Task.WhenAll(tasks)
                    .ContinueWith(t =>
                        Log.Info($"emoji file saved")
                    )
            ));

            tasks.ForEach(t => t.Start());
        }

        public void SaveEmojiText(List<Emoji> emos)
        {
            var data = new List<string>();
            foreach (var e in emos)
            {
                data.Add($"{e.Pinyin.Replace("'", "")} {e.Emotion} 5");
            }

            Directory.CreateDirectory(BasePath);

            var tasks = new List<Task>
            {
                new Task(async () =>
                    await File.WriteAllTextAsync(
                            Path.Combine(BasePath, "data.txt"),
                            string.Join("\n", data)
                    )
            )
            };

            tasks.Add(new Task(async () =>
                await Task.WhenAll(tasks)
                    .ContinueWith(t =>
                        Log.Info($"emoji file saved")
                    )
            ));

            tasks.ForEach(t => t.Start());
        }
    }

    public class Emoji
    {
        public string Emotion { get; set; }
        public string Unicode { get; set; }
        public string Descript { get; set; }
        public string Pinyin { get; set; }
    }
}
