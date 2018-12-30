using System;

namespace Command
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new Log();

            var infoEntry = new InfoLogCommand("Here is some info");
            log.AddEntry(infoEntry);

            var errorEntry = new ErrorLogCommand("Something went wrong!", new Exception("Oops!"));
            log.AddEntry(infoEntry);

            var fancyEntry = new FancyLogCommand(LogLevel.Warn, "Fancy warning!");
            log.AddEntry(infoEntry);

            var fancyError = new FancyLogCommand("Fancy error!", new Exception("Oops!"));
            log.AddEntry(fancyError);

            Console.ReadKey();
        }
    }

    interface ILogCommand
    {
        void Execute();
    }

    class Log
    {
        public void AddEntry(ILogCommand logEntry)
        {
            logEntry.Execute();
        }
    }

    class InfoLogCommand : ILogCommand
    {
        private readonly string _message;

        public InfoLogCommand(string message)
        {
            _message = message;
        }

        public void Execute()
        {
            new ConsoleLogWriter().Log(LogLevel.Info, _message);
        }
    }
    class ErrorLogCommand : ILogCommand
    {
        private readonly string _message;
        private readonly Exception _exception;

        public ErrorLogCommand(string message, Exception exception)
        {
            _message = message;
            this._exception = exception;
        }

        public void Execute()
        {
            var logMessage = _message + "\nStacktrace:\n" + _exception.GetBaseException().StackTrace;
            new ConsoleLogWriter().Log(LogLevel.Info, logMessage);
        }
    }
    class FancyLogCommand : ILogCommand
    {
        public FancyLogCommand(LogLevel level, string message)
        {
            Level = level;
            Message = message;
        }
        public FancyLogCommand(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
            Level = LogLevel.Error;
        }

        public void Execute()
        {
            if (Level == LogLevel.Error && Exception != null)
            {
                var logMessage = Message + "\nStacktrace:\n" + Exception.GetBaseException().StackTrace;
                new FancyLogWriter().Log(Level, logMessage, ConsoleColor.Red);
            }
            else
            {
                new FancyLogWriter().Log(Level, Message, ToColor(Level));
            }
        }

        public static ConsoleColor ToColor(LogLevel level)
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

        public LogLevel Level { get; }
        public string Message { get; }
        public Exception Exception { get; }
    }

    class ConsoleLogWriter
    {
        public void Log(LogLevel level, string message)
        {
            Console.WriteLine(level.ToString().ToUpper() + " | " + message);
        }
    }
    class FancyLogWriter
    {
        public void Log(LogLevel level, string message, ConsoleColor color)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine("{0} | {1} | {2}", level.ToString().ToUpper(), message, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.ForegroundColor = foregroundColor;
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
