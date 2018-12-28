using System;

namespace Builder
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new LoggerBuilder();

            var defaultLog = builder.CreateDefaultLogger();
            defaultLog.Log(LogLevel.Warn, "Warning!");

            var fancyLog = builder.CreateFancyLogger();
            fancyLog.Log(LogLevel.Warn, "Fancy warning!");

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

    interface IColorSelector
    {
        ConsoleColor GetColor(LogLevel level);
    }

    class LoggerBuilder
    {
        public ILogger CreateDefaultLogger()
        {
            return new ConsoleLogger
            {
                Prefix = "DEFAULT LOGGER"
            };
        }
        public ILogger CreateFancyLogger()
        {
            return new ConsoleLogger
            {
                ColorSelector = new ColorSelector().GetColor,
                IncludeDateTime = true,
                Prefix = ".: FANCY LOGGER :."
            };
        }

        private class ColorSelector
        {
            public ConsoleColor GetColor(LogLevel level)
            {
                switch (level)
                {
                    case LogLevel.Debug:
                        return ConsoleColor.White;
                    case LogLevel.Warn:
                        return ConsoleColor.Yellow;
                    case LogLevel.Error:
                        return ConsoleColor.Red;
                    default:
                        return ConsoleColor.DarkGray;
                }
            }
        }
    }

    class ConsoleLogger : ILogger
    {
        public ConsoleLogger()
        {
            ColorSelector = x => ConsoleColor.White;
        }

        public void Log(LogLevel level, string message)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ColorSelector(level);
            Console.WriteLine($"{Prefix}{(Prefix == null ? "": " | ")}{(IncludeDateTime ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss |") : "")} {message}");
            Console.ForegroundColor = foregroundColor;
        }

        public string Prefix { get; set; }
        public Func<LogLevel, ConsoleColor> ColorSelector { get; set; }
        public bool IncludeDateTime { get; set; }
    }
}
