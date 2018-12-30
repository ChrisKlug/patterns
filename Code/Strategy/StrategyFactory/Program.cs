using System;
using System.Collections.Generic;
using System.Linq;

namespace StrategyFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            var strategyFactory = new LoggingStrategyFactory();
            strategyFactory.RegisterStrategy(level => level == LogLevel.Error, level => new ErrorLoggingStrategy());

            var logger = new Logger(strategyFactory);

            logger.Log(LogLevel.Info, "Information");
            Console.WriteLine();

            logger.Log(LogLevel.Warn, "Warning!");
            Console.WriteLine();

            logger.Log(LogLevel.Error, "Error!");
            Console.WriteLine();

            logger.Log("Exception!", new Exception("Oops"));
            Console.WriteLine();

            Console.ReadKey();
        }
    }

    interface ILoggingStrategy
    {
        void Log(string message);
        void Log(string message, Exception error);
    }

    class DefaultLoggingStrategy : ILoggingStrategy
    {
        private readonly LogLevel _level;

        public DefaultLoggingStrategy(LogLevel level)
        {
            this._level = level;
        }

        public void Log(string message)
        {
            WriteEntry(ToColor(_level), $"ERROR | {message} | {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        public void Log(string message, Exception error)
        {
            Log(message);
        }

        private static void WriteEntry(ConsoleColor color, string message)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ForegroundColor = foregroundColor;
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
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.DarkGray;
            }
        }
    }
    class ErrorLoggingStrategy : ILoggingStrategy
    {
        public void Log(string message)
        {
            WriteEntry($"ERROR | {message} | {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        public void Log(string message, Exception error)
        {
            WriteEntry($"ERROR | {message} | {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\nStacktrace:\n{error.GetBaseException().StackTrace}");
        }

        private static void WriteEntry(string message)
        {
            var foregroundColor = Console.ForegroundColor;
            var backgroundColor = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
        }
    }

    class LoggingStrategyFactory
    {
        private IList<(Predicate<LogLevel> Predicate, Func<LogLevel, ILoggingStrategy> Creator)> _creators = new List<(Predicate<LogLevel> Predicate, Func<LogLevel, ILoggingStrategy> Creator)>();

        public void RegisterStrategy(Predicate<LogLevel> predicate, Func<LogLevel, ILoggingStrategy> creator)
        {
            _creators.Add((Predicate: predicate, Creator: creator));
        }

        public ILoggingStrategy Get(LogLevel level)
        {
            if (_creators.Any(y => y.Predicate(level)))
            {
                return _creators.First(y => y.Predicate(level)).Creator(level);
            }
            return new DefaultLoggingStrategy(level);
        }
    }

    class Logger
    {
        private readonly LoggingStrategyFactory _loggingStrategyFactory;

        public Logger(LoggingStrategyFactory loggingStrategyFactory)
        {
            this._loggingStrategyFactory = loggingStrategyFactory;
        }

        public void Log(LogLevel level, string message)
        {
            _loggingStrategyFactory.Get(level).Log(message);
        }
        public void Log(string message, Exception error)
        {
            _loggingStrategyFactory.Get(LogLevel.Error).Log(message, error);
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
