using DataCatch.Common;
using DataCatch.Douban.Base;
using System.Collections.Generic;
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
            foreach (var groupId in GroupIds)
            {
                Log.Info($"Start GET, current groupid: {groupId}");
                var paramters = new Dictionary<string, string>
                {
                    ["groupId"] = groupId.ToString()
                };

                var apiUrl = GetApiUrl(ApiType.Info, paramters);
                Log.Debug($"Get current api url: {apiUrl}");

                var data = await ApiGet(apiUrl);

                FileSave(data, groupId.ToString());
                Log.Info("File saved.");
            }
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
