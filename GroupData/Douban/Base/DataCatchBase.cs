using DataCatch.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataCatch.Douban.Base
{
    /// <summary>
    /// 数据抓取基础实现
    /// </summary>
    public abstract class DataCatchBase : IProcess
    {
        /// <summary>
        /// 日志路径
        /// </summary>
        private static readonly string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "douban");
        public async Task Start()
        {
            Log.Info("Douban process start");
            try
            {
                Log.Info("Douban process start datacatching");
                await Catch();
                Log.Info("Finished");
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        /// <summary>
        /// 数据抓取
        /// </summary>
        /// <returns></returns>
        public abstract Task Catch();

        /// <summary>
        /// api地址
        /// </summary>
        /// <param name="apiType">api类型</param>
        /// <param name="groupId">小组id</param>
        /// <param name="topicId">话题id</param>
        /// <returns></returns>
        public abstract string GetApiUrl(
            ApiType apiType,
            Dictionary<string, string> paramters);

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public async Task<string> ApiGet(string api) =>
            await new HttpClient().GetAsync(api).Result.Content.ReadAsStringAsync();

        /// <summary>
        /// file save
        /// </summary>
        /// <param name="data">path, data</param>
        /// <param name="fileName"></param>
        public void FileSave(Dictionary<string, string> data, string fileName)
        {
            var tasks = new List<Task>();
            foreach (var pairs in data)
            {
                Directory.CreateDirectory(
                    Path.Combine(BasePath, pairs.Key));

                var filePath = Path.Combine(BasePath, pairs.Key, fileName);
                tasks.Add(new Task(async () =>
                    await File.WriteAllTextAsync(
                            filePath,
                            pairs.Value
                    )
                ));
            }

            tasks.Add(new Task(async () =>
                await Task.WhenAll(tasks)
                    .ContinueWith(t =>
                        Log.Info($"{data.Count} files saved")
                    )
            ));

            tasks.ForEach(t => t.Start());
        }
    }
}
