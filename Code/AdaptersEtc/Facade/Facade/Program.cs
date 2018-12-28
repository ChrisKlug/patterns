using Microsoft.Extensions.Configuration;
using System;

namespace Facade
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger log = new LoggingFacade<Program>();

            log.Log(LogLevel.Info, "Here is some information");
            log.Log(LogLevel.Error, "Here is an error!!!");

            Console.ReadKey();
        }
    }
    interface ILogger
    {
        void Log(LogLevel level, string message);
    }

    enum LogLevel
    {
        Info,
        Debug,
        Warn,
        Error
    }

    class LoggingFacade<T> : ILogger
    {
        static ConsoleLogger _logger;
        IConfiguration _config;

        public void Log(LogLevel level, string message)
        {
            var color = GetConsoleColor(level);
            var format = _config["logFormat"];

            Logger.Log(typeof(T).Name, color, format, level.ToString(), message);
        }

        private ConsoleColor GetConsoleColor(LogLevel level)
        {
            var color = Config["colors:" + level];
            if (Enum.TryParse(color, true, out ConsoleColor consoleColor))
            {
                return consoleColor;
            }
            return ConsoleColor.White;
        }
        private IConfiguration Config
        {
            get
            {
                if (_config == null)
                {
                    _config = new ConfigurationBuilder()
                            .SetBasePath(Environment.CurrentDirectory)
                            .AddJsonFile("appSettings.json", false, true)
                            .Build();
                }
                return _config;
            }
        }
        private static ConsoleLogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new ConsoleLogger();
                }
                return _logger;
            }
        }
    }

    class ConsoleLogger
    {
        public void Log(string type, ConsoleColor color, string format, string logLevel, string message)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            var log = format.Replace("%d", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                            .Replace("%t", type)
                            .Replace("%l", logLevel)
                            .Replace("%m", message);
            Console.WriteLine(log);
            Console.ForegroundColor = currentColor;
        }
    }
}
