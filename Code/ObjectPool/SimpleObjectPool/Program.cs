using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleObjectPool
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggers = new LoggerPool(3);

            Parallel.For(0, 20, x => {
                var logger = loggers.GetLogger();
                logger.Log("Logging number " + x);
            });

            Console.ReadKey();
        }
    }

    class LoggerPool
    {
        private List<Logger> _loggers = new List<Logger>();
        private object _lock = new object();
        private int _index = -1;

        public LoggerPool(int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                var logger = new Logger("Logger " + i);
                _loggers.Add(logger);
            }
        }

        public Logger GetLogger()
        {
            lock (_lock)
            {
                _index++;
                if (_index >= _loggers.Count)
                    _index = 0;
                return _loggers[_index];
            }
        }
    }

    class Logger
    {
        private readonly string _name;

        public Logger(string name)
        {
            _name = name;
        }

        public void Log(string message)
        {
            Thread.Sleep(1000);
            Console.WriteLine("{0}: {1} ({2})", _name, message, Thread.CurrentThread.ManagedThreadId);
        }
    }
}
