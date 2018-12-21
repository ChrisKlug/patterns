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
        static Logger()
        {
            Instance = new Logger();
        }
        private Logger() {}

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static Logger Instance { get; private set; }
    }
}
