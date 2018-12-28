using System;

namespace FluentBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new LoggerBuilder()
                                .AddColorSelector(new ColorSelector())
                                .AddPrefix("CUSTOM LOGGER")
                                .IncludeDateTime()
                                .Build();
            logger.Log(LogLevel.Warn, "Hello from my logger!");

            var fancyLogger = new LoggerBuilder().CreateFancyLogger();
            fancyLogger.Log(LogLevel.Warn, "Hello from fancy logger!");

            var magentaLogger = new LoggerBuilder()
                                    .SetMagentaConfig("Meganta")
                                    .Build();
            magentaLogger.Log(LogLevel.Warn, "Hello from magenta logger!");

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

    class ColorSelector : IColorSelector
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

    class LoggerBuilder
    {
        private string _prefix;
        private bool _includeDateTime;
        private Func<LogLevel, ConsoleColor> _colorSelector = x => ConsoleColor.White;

        public LoggerBuilder AddPrefix(string prefix)
        {
            _prefix = prefix;
            return this;
        }
        public LoggerBuilder AddColorSelector(Func<LogLevel, ConsoleColor> selector)
        {
            _colorSelector = selector;
            return this;
        }
        public LoggerBuilder AddColorSelector(IColorSelector selector)
        {
            _colorSelector = x => selector.GetColor(x);
            return this;
        }
        public LoggerBuilder IncludeDateTime()
        {
            _includeDateTime = true;
            return this;
        }

        public ILogger Build()
        {
            return new ConsoleLogger
            {
                Prefix = _prefix,
                IncludeDateTime = _includeDateTime,
                ColorSelector = _colorSelector
            };
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
            Console.WriteLine($"{Prefix}{(Prefix == null ? "" : " | ")}{(IncludeDateTime ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss |") : "")} {message}");
            Console.ForegroundColor = foregroundColor;
        }

        public string Prefix { get; set; }
        public Func<LogLevel, ConsoleColor> ColorSelector { get; set; }
        public bool IncludeDateTime { get; set; }
    }

    static class LoggerBuilderExtensions
    {
        public static ILogger CreateFancyLogger(this LoggerBuilder builder)
        {
            return builder
                    .AddColorSelector(x => x == LogLevel.Warn ? ConsoleColor.Cyan : ConsoleColor.White)
                    .AddPrefix("FANCY")
                    .Build();
        }
        public static LoggerBuilder SetMagentaConfig(this LoggerBuilder builder, string prefix)
        {
            return builder
                    .AddColorSelector(x => ConsoleColor.Magenta)
                    .IncludeDateTime()
                    .AddPrefix(prefix);
        }
    }
}
