using DataCatch.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DataCatch.Douban.Base
{
    /// <summary>
    /// 数据抓取基础实现
    /// </summary>
    public abstract class DataCatchBase
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
                var debugCount = 0;
                while (true)
                {
                    Log.Info("Douban process start datacatching");
                    await Catch();
                    Log.Debug($"Douban process has run for {++debugCount} times");
                    Thread.Sleep(1000 * 60 * 5);
                }
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
        /// <param name="data"></param>
        /// <returns></returns>
        public void FileSave(string data, string folder)
        {
            var subPath = Path.Combine(
                    BasePath,
                    folder,
                    DateTime.Now.ToString("yyyy-MM-dd"));

            Directory.CreateDirectory(subPath);

            var task = new Task(async () =>
                await File.WriteAllTextAsync(
                    Path.Combine(
                        subPath,
                        $"{DateTime.Now:yyyyMMdd_HHmmss}.json"),
                    data)
            );

            task.Start();
        }
    }
}
