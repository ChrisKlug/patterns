using System;

namespace NullObject
{
    class Program
    {
        static void Main(string[] args)
        {
            var svc1 = new IsItFridayService();
            var svc2 = new IsItFridayService(new ConsoleLogger());

            Console.WriteLine("Is it friday? " + svc1.IsItFriday());
            Console.WriteLine("Is it friday? " + svc2.IsItFriday());
        }
    }

    class IsItFridayService
    {
        private readonly ILogger _logger;

        public IsItFridayService() : this(new NullLogger())
        {
        }
        public IsItFridayService(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsItFriday()
        {
            _logger.Log($"Checking if it's Friday - Returning: {DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday}");

            return DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday;
        }
    }

    interface ILogger
    {
        void Log(string message);
    }

    class NullLogger : ILogger
    {
        public void Log(string message) {}
    }

    class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
