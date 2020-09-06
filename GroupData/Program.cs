using DataCatch.Common;
using DataCatch.Douban.Group;
using System;
using System.Threading.Tasks;

namespace DataCatch
{
    static class Program
    {
        static void Main()
        {
            try
            {
                Log.Info("Init");
                // 初始化配置信息
                Config.InitConfig();

                var t = new Task(
                    async () =>
                    await new GroupDataCatch().Start()
                );

                t.Start();
                t.Wait();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}
