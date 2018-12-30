using System;
using System.Collections.Generic;

namespace BatchedCommands
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new Log(3);

            Console.WriteLine(".: Adding info entry 1 :.");
            log.AddEntry(new InfoLogCommand("Info entry 1"));

            Console.WriteLine("\n.: Adding warn entry 1 :.");
            log.AddEntry(new WarnLogCommand("Warn entry 1"));

            Console.WriteLine("\n.: Adding warn entry 2 :.");
            log.AddEntry(new WarnLogCommand("Warn entry 2"));

            Console.WriteLine("\n.: Adding info entry 2 :.");
            log.AddEntry(new InfoLogCommand("Info entry 2"));

            Console.WriteLine("\n.: Adding warn entry 3 :.");
            log.AddEntry(new WarnLogCommand("Warn entry 3"));

            Console.WriteLine("\n.: Flushing :.");
            log.Flush();
            
            Console.WriteLine("\n.: Running historic events :.");
            log.RunHistoricEvents();

            Console.ReadKey();
        }
    }
    interface ILogCommand
    {
        void Execute();
    }

    class Log
    {
        private readonly int _batchSize;
        private readonly Queue<ILogCommand> _queue;
        private readonly List<ILogCommand> _history;

        public Log(int batchSize)
        {
            _batchSize = batchSize;
            _queue = new Queue<ILogCommand>(batchSize);
            _history = new List<ILogCommand>();
        }

        public void AddEntry(ILogCommand logEntry)
        {
            _queue.Enqueue(logEntry);

            if (_queue.Count == _batchSize)
            {
                Flush();
            }
        }
        public void Flush()
        {
            while (_queue.Count > 0)
            {
                var entry = _queue.Dequeue();
                entry.Execute();
                _history.Add(entry);
            }
        }
        public void RunHistoricEvents()
        {
            foreach (var entry in _history)
            {
                entry.Execute();
            }
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
    class WarnLogCommand : ILogCommand
    {
        private readonly string _message;

        public WarnLogCommand(string message)
        {
            _message = message;
        }

        public void Execute()
        {
            new ConsoleLogWriter().Log(LogLevel.Warn, _message);
        }
    }

    class ConsoleLogWriter
    {
        public void Log(LogLevel level, string message)
        {
            Console.WriteLine(level.ToString().ToUpper() + " | " + message);
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
