using System;

namespace BadSingleton
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Instance.Log("Hello World");
        }
    }

    class Logger
    {
        private Logger() {}

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static Logger Instance { get; } = new Logger();
    }
}
