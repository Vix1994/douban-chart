using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataCatch.Common
{
    /// <summary>
    /// 配置
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// 配置文件
        /// </summary>
        public static JConfig Jconfig { get; set; }
        /// <summary>
        /// 初始化配置
        /// </summary>
        public static void InitConfig()
        {
            var config = LoadJson("config.json");
            Jconfig = JsonSerializer.Deserialize<JConfig>(
                    config,
                    new JsonSerializerOptions() {
                        PropertyNameCaseInsensitive = true
                    }
                );
        }

        /// <summary>
        /// 读取json
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        private static string LoadJson(string fileName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            using var stream = new StreamReader(path);
            return stream.ReadToEnd();
        }
    }

    /// <summary>
    /// jsonconfig
    /// </summary>
    public class JConfig
    {
        /// <summary>
        /// api参数
        /// </summary>
        public Params Params { get; set; }
        /// <summary>
        /// API
        /// </summary>
        [JsonPropertyName("api")]
        public Api Apis { get; set; }
        /// <summary>
        /// 进程
        /// </summary>
        public string[][] Process { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
    }

    /// <summary>
    /// API
    /// </summary>
    public class Api
    {
        /// <summary>
        /// 豆瓣小组
        /// </summary>
        [JsonPropertyName("douban-group")]
        [JsonConverter(typeof(JsonSpecificConverter))]
        public Dictionary<ApiType, String> DoubanGroup { get; set; }
        /// <summary>
        /// EmojiAll
        /// </summary>
        public string EmojiAll { get; set; }
    }

    /// <summary>
    /// API参数
    /// </summary>
    public class Params
    {
        /// <summary>
        /// 小组id
        /// </summary>
        [JsonPropertyName("douban-group-list")]
        public List<long> GroupIdList { get; set; }
    }

    public enum ApiType
    {
        Info,
        Members,
        Topics,
        TopicInfo,
        Comments
    }
}
