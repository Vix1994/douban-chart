using DataCatch.Common;
using DataCatch.Douban.Base;
using Hangfire;
using Hangfire.MemoryStorage;
using System;
using System.Reflection;

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

                GlobalConfiguration.Configuration.UseColouredConsoleLogProvider().UseMemoryStorage();

                using (var server = new BackgroundJobServer())
                {
                    Log.Info("Loading process");
                    foreach (var processConfig in Config.Jconfig.Process)
                    {
                        var name = processConfig[0];
                        var fullName = processConfig[1];
                        var cron = processConfig[2];

                        var process = (IProcess)Assembly.Load(name).CreateInstance(fullName);

                        RecurringJob.AddOrUpdate(
                            name,
                            () => process.Start(),
                            cron,
                            TimeZoneInfo.Local);

                        Log.Info($"Process {fullName} load up");
                    }
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}
