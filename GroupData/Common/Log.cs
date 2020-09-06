using System;

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
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now}] [{logLevel}] {log}");
        }

        public static void Info(string log) =>
            Output(log, "Info", ConsoleColor.White);
        public static void Debug(string log) =>
            Output(log, "Debug", ConsoleColor.Yellow);
        public static void Error(string log) =>
            Output(log, "Error", ConsoleColor.Red);
    }
}
