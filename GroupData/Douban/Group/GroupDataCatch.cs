using DataCatch.Common;
using DataCatch.Douban.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataCatch.Douban.Group
{
    public class GroupDataCatch : DataCatchBase
    {
        /// <summary>
        /// 豆瓣小组api
        /// </summary>
        private readonly Dictionary<ApiType, string> GroupApis = Config.Jconfig.Apis.DoubanGroup;

        private readonly List<long> GroupIds = Config.Jconfig.Params.GroupIdList;
        public override async Task Catch()
        {
            Log.Debug($"Total groups count: {GroupIds.Count}");
            Log.Debug($"Total group id list: {string.Join(",", GroupIds)}");

            var currentTime = DateTime.Now;
            // <folder, data>
            var dataToSave = new Dictionary<string, string>();
            foreach (var groupId in GroupIds)
            {
                Log.Info($"Start GET, current groupid: {groupId}");
                var pathToday = $"{groupId}\\{currentTime:yyyy-MM-dd}";
                var paramters = new Dictionary<string, string>
                {
                    ["groupId"] = groupId.ToString()
                };

                // Group Info API
                var groupInfoApi = GetApiUrl(ApiType.Info, paramters);
                Log.Debug($"Get current group info api url: {groupInfoApi}");

                var groupData = await ApiGet(groupInfoApi);

                dataToSave[$"{pathToday}\\group"] = groupData;

                // Group Topics Info API
                var groupTopicInfoApi = $"{GetApiUrl(ApiType.Topics, paramters)}?count=1";
                Log.Debug($"Get current group topic info api url: {groupInfoApi}");

                var topicInfoData = await ApiGet(groupTopicInfoApi);

                dataToSave[$"{pathToday}\\topic"] = topicInfoData;
            }

            if (dataToSave.Any()) FileSave(dataToSave, $"{currentTime:yyyyMMdd_HHmmss}.json");
        }

        public override string GetApiUrl(
            ApiType apiType,
            Dictionary<string, string> paramters) =>
            new Regex(@":(\w+)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled)
                .Replace(GroupApis[apiType],
                    m => paramters[m.Groups[1].Value]);
    }
}
