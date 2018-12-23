using System;

namespace SingletonLazy
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
        static object _lock = new object();
        static Logger _instance;

        private Logger() {}

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Logger();
                        }
                    }
                }

                return _instance;
            }
        }
    }
}
