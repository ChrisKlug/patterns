using System;

namespace Strategy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(".: Logging using Basic strategy :.");
            var log = new Logger(new BasicLogFormatStrategy());
            log.Log(LogLevel.Info, "Information");
            log.Log(LogLevel.Error, "Error");
            
            Console.WriteLine("\n.: Logging using Fancy strategy :.");
            log = new Logger(new FancyLogFormatStrategy());
            log.Log(LogLevel.Info, "Information");
            log.Log(LogLevel.Error, "Error");

            Console.ReadKey();
        }
    }

    interface ILogFormatStrategy
    {
        LogFormat Format(LogLevel level, string message);
    }

    class BasicLogFormatStrategy : ILogFormatStrategy
    {
        public LogFormat Format(LogLevel level, string message)
        {
            return new LogFormat(ConsoleColor.White, ConsoleColor.Black, level + " | " + message);
        }
    }
    class FancyLogFormatStrategy : ILogFormatStrategy
    {
        public LogFormat Format(LogLevel level, string message)
        {
            var logMessage = $"{level.ToString().ToUpper()} | {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | {message}";

            return new LogFormat(ToColor(level), ToBackgroundColor(level), logMessage);
        }

        private static ConsoleColor ToColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return ConsoleColor.White;
                case LogLevel.Warn:
                    return ConsoleColor.Yellow;
                case LogLevel.Error:
                    return ConsoleColor.White;
                default:
                    return ConsoleColor.DarkGray;
            }
        }
        private static ConsoleColor ToBackgroundColor(LogLevel level)
        {
            return level == LogLevel.Error ? ConsoleColor.Red : ConsoleColor.Black;
        }
    }

    class LogFormat
    {
        public LogFormat(ConsoleColor foreground, ConsoleColor background, string message)
        {
            Foreground = foreground;
            Background = background;
            Message = message;
        }

        public ConsoleColor Foreground { get; }
        public ConsoleColor Background { get; }
        public string Message { get; }
    }

    class Logger
    {
        private readonly ILogFormatStrategy _formattingStrategy;

        public Logger(ILogFormatStrategy formattingStrategy)
        {
            _formattingStrategy = formattingStrategy;
        }

        public void Log(LogLevel level, string message)
        {
            var entryFormat = _formattingStrategy.Format(level, message);

            var foreground = Console.ForegroundColor;
            var background = Console.BackgroundColor;
            Console.ForegroundColor = entryFormat.Foreground;
            Console.BackgroundColor = entryFormat.Background;
            Console.WriteLine(entryFormat.Message);
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }
    }

    enum LogLevel
    {
        Info,
        Debug,
        Warn,
        Error
    }
}
