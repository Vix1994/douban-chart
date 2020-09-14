using DataCatch.Douban.Base;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DataCatch.Common
{
    public static class Log
    {
        /// <summary>
        /// output
        /// </summary>
        /// <param name="log"></param>
        private static void Output(string log, string logLevel, ConsoleColor color)
        {
            var output = $"[{DateTime.Now}] [{logLevel}] {log}";
            Console.ForegroundColor = color;
            Console.WriteLine(output);
        }

        public static void Info(string log) =>
            Output(log, "Info", ConsoleColor.Green);
        public static void Debug(string log) =>
            Output(log, "Debug", ConsoleColor.Yellow);
        public static void Error(string log) =>
            Output(log, "Error", ConsoleColor.Red);
    }

    public class WriteLog : IProcess
    {
        public static bool IsRunning { get; set; }
        /// <summary>
        /// 输出日志到目录
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Log",
                DateTime.Now.ToString("yyyy-MM-dd"));

            Directory.CreateDirectory(path);

            // write current console logs
        }
    }
}
