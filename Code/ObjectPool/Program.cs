using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectPool
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggers = new LoggerPool(3);

            Parallel.For(0, 20, x => {
                using (var logger = loggers.GetLogger())
                {
                    logger.Log("Logging number " + x);
                }
            });

            Console.ReadKey();
        }
    }

    class LoggerPool
    {
        private SemaphoreSlim _semaphore;
        private Queue<Logger> _loggers = new Queue<Logger>();
        private object _lock = new object();

        public LoggerPool(int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                var logger = new Logger("Logger " + i);
                logger.OnDisposed += OnLoggerDisposed;
                _loggers.Enqueue(logger);
            }
            _semaphore = new SemaphoreSlim(poolSize);
        }

        public Logger GetLogger()
        {
            _semaphore.Wait();

            lock (_lock)
            {
                return _loggers.Dequeue();
            }
        }

        private void OnLoggerDisposed(object sender, EventArgs e)
        {
            lock (_lock)
            {
                _loggers.Enqueue((Logger)sender);
            }
            _semaphore.Release();
        }
    }

    class Logger : IDisposable
    {
        private readonly string _name;

        public Logger(string name)
        {
            _name = name;
        }

        public void Log(string message)
        {
            Console.WriteLine("{0}: {1} ({2})", _name, message, Thread.CurrentThread.ManagedThreadId);
        }

        public void Dispose()
        {
            OnDisposed?.Invoke(this, new EventArgs());
        }

        public event EventHandler<EventArgs> OnDisposed;
    }
}
