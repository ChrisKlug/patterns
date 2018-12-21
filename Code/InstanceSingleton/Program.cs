using System;

namespace SingletonInstance
{
    class Program
    {
        static void Main(string[] args)
        {
            // Default
            Logger.Instance.Log("Test");

            // With configuration
            Logger.Initialize("Prefix");
            Logger.Instance.Log("Test2");

            // With custom implementation
            Logger.Initialize(new FancyConsoleLogger("Red", ConsoleColor.Red));
            Logger.Instance.Log("Test");
        }
    }

    public interface ILogger
    {
        void Log(string message);
    }

    static class Logger
    {
        static ILogger _instance;

        public static void Initialize(string prefix)
        {
            EnsureNotInitialized();
            _instance = new ConsoleLogger(prefix);
        }
        public static void Initialize(ILogger logger)
        {
            EnsureNotInitialized();
            _instance = logger;
        }

        private static void EnsureNotInitialized()
        {
            //if (_instance != null)
            //{
            //    throw new InvalidOperationException("Can only initialize class once");
            //}
        }

        public static ILogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConsoleLogger();
                }
                return _instance;
            }
        }
    }

    public class ConsoleLogger : ILogger
    {
        private readonly string _prefix;

        public ConsoleLogger() {}
        public ConsoleLogger(string prefix)
        {
            _prefix = prefix;
        }

        public void Log(string message)
        {
            if (_prefix != null) { 
                Console.WriteLine("{0}: {1}", _prefix, message);
            } else
            {
                Console.WriteLine(message);
            }
        }
    }

    public class FancyConsoleLogger : ILogger
    {
        private readonly string _prefix;
        private readonly ConsoleColor _color;

        public FancyConsoleLogger(string prefix, ConsoleColor color)
        {
            _prefix = prefix;
            _color = color;
        }

        public void Log(string message)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = _color;
            Console.WriteLine("{0}: {1}", _prefix, message);
            Console.ForegroundColor = c;
        }
    }
}
