using System;

namespace AbstractFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            LoggerFactory loggerFactory;
#if DEBUG
            loggerFactory = new ConsoleLoggerFactory();
#else
            loggerFactory = new FancyLoggerFactory();
#endif

            loggerFactory.GetLogger().Log("Hello World!!!");
            loggerFactory.GetExceptionLogger().Log(new Exception("Demoing logging an exception..."));

            Console.ReadKey();
        }
    }

    interface ILogger
    {
        void Log(string message);
    }
    interface IExceptionLogger
    {
        void Log(Exception exception);
    }

    abstract class LoggerFactory
    {
        public abstract ILogger GetLogger();
        public abstract IExceptionLogger GetExceptionLogger();
    }

    class ConsoleLoggerFactory : LoggerFactory
    {
        public override IExceptionLogger GetExceptionLogger()
        {
            return new ConsoleExceptionLogger();
        }

        public override ILogger GetLogger()
        {
            return new ConsoleLogger();
        }
    }
    class FancyLoggerFactory : LoggerFactory
    {
        public override IExceptionLogger GetExceptionLogger()
        {
            return new FancyConsoleExceptionLogger();
        }

        public override ILogger GetLogger()
        {
            return new FancyConsoleLogger();
        }
    }

    class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
    class ConsoleExceptionLogger : IExceptionLogger
    {
        public void Log(Exception exception)
        {
            Console.WriteLine("ERROR: {0}\r\nStacktrace: {1}", exception.GetBaseException().Message, exception.StackTrace);
        }
    }

    class FancyConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }
    }
    class FancyConsoleExceptionLogger : IExceptionLogger
    {
        public void Log(Exception exception)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: {0}\r\nStacktrace: {1}", exception.GetBaseException().Message, exception.StackTrace);
            Console.ForegroundColor = color;
        }
    }
}
