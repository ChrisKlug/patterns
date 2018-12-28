using System;
using System.Threading;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]

namespace Bridge
{
    class Program
    {
        static void Main(string[] args)
        {
            var consoleWriter = new ConsoleLogWriter();
            var log4NetWriter = new Log4NetLogWriter<Program>();

            var simpleConsoleLogger = new SimpleLogger(consoleWriter);
            simpleConsoleLogger.Log<Program>(LogLevel.Warn, "Here is a warning from the SimpleLogger through the ConsoleLogWriter");
            Console.WriteLine();

            var simpleLog4NetLogger = new SimpleLogger(log4NetWriter);
            simpleLog4NetLogger.Log<Program>(LogLevel.Warn, "Here is a warning from the SimpleLogger through the Log4NetLogWriter<T>");
            Console.WriteLine();

            var advancedConsoleLogger = new AdvancedLogger(consoleWriter);
            advancedConsoleLogger.Log<Program>(LogLevel.Warn, "Here is a warning from the AdvancedLogger through the ConsoleLogWriter");
            Console.WriteLine();

            var advancedLog4NetLogger = new AdvancedLogger(log4NetWriter);
            advancedLog4NetLogger.Log<Program>(LogLevel.Warn, "Here is a warning from the AdvancedLogger through the Log4NetLogWriter<T>");
            Console.WriteLine();

            var configurableConsoleLogger = new ConfigurableLogger(consoleWriter, "CONFIGURABLE > %l | %t | Message: %m");
            configurableConsoleLogger.Log<Program>(LogLevel.Warn, "Here is a warning from the ConfigurableLogger through the ConsoleLogWriter");
            Console.WriteLine();

            var configurableLog4NetLogger = new ConfigurableLogger(log4NetWriter, "CONFIGURABLE > %l | %t | Message: %m");
            configurableLog4NetLogger.Log<Program>(LogLevel.Warn, "Here is a warning from the ConfigurableLogger through the Log4NetLogWriter<T>");
            Console.WriteLine();

            Console.ReadKey();
        }
    }

    enum LogLevel
    {
        Info,
        Debug,
        Warn,
        Error
    }

    interface ILogger
    {
        void Log<T>(LogLevel level, string message);
    }
    interface ILogWriter
    {
        void Write(LogLevel level, string message);
    }

    class SimpleLogger : ILogger
    {
        private readonly ILogWriter _logWriter;

        public SimpleLogger(ILogWriter logWriter)
        {
            _logWriter = logWriter;
        }

        public void Log<T>(LogLevel level, string message)
        {
            var logEntry = $"SIMPLE > {level} | {message}";
            _logWriter.Write(level, logEntry);
        }
    }
    class AdvancedLogger : ILogger
    {
        public AdvancedLogger(ILogWriter logWriter)
        {
            LogWriter = logWriter;
        }

        public void Log<T>(LogLevel level, string message)
        {
            var logEntry = $"ADVANCED > {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | {typeof(T).Name} | {level} | {Thread.CurrentThread.ManagedThreadId} | {message}";
            LogWriter.Write(level, logEntry);
        }

        ILogWriter LogWriter { get; }
    }
    class ConfigurableLogger : ILogger
    {
        private readonly string _format;

        public ConfigurableLogger(ILogWriter logWriter, string format)
        {
            LogWriter = logWriter;
            _format = format;
        }

        public void Log<T>(LogLevel level, string message)
        {
            var logEntry = _format.Replace("%d", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                            .Replace("%t", typeof(T).Name)
                            .Replace("%l", level.ToString())
                            .Replace("%tid", Thread.CurrentThread.ManagedThreadId.ToString())
                            .Replace("%m", message);

            LogWriter.Write(level, logEntry);
        }

        protected ILogWriter LogWriter { get; }
    }

    class ConsoleLogWriter : ILogWriter
    {
        public void Write(LogLevel level, string message)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ToColor(level);
            Console.WriteLine(message);
            Console.ForegroundColor = currentColor;
        }

        protected ConsoleColor ToColor(LogLevel level)
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
    class Log4NetLogWriter<T> : ILogWriter
    {
        private log4net.ILog _log;

        public Log4NetLogWriter()
        {
            _log = log4net.LogManager.GetLogger(typeof(T));
        }

        public void Write(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Info:
                    _log.Info(message);
                    break;
                case LogLevel.Debug:
                    _log.Debug(message);
                    break;
                case LogLevel.Warn:
                    _log.Warn(message);
                    break;
                case LogLevel.Error:
                    _log.Error(message);
                    break;
            }
        }
    }
}
