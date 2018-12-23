using System;

namespace FactoryMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            DateHelper dateHelper;
#if DEBUG
            dateHelper = new DateHelper();
#else
            dateHelper = new FancyDateHelper();
#endif

            dateHelper.IsItFriday();

            Console.ReadKey();
        }
    }

    interface ILogger
    {
        void Log(string message);
    }

    class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
    class FancyLogger : ILogger
    {
        public void Log(string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }
    }

    class DateHelper {
        public bool IsItFriday()
        {
            var day = DateTime.Now.DayOfWeek;
            GetLogger().Log($"Is { day.ToString() } friday? { day == DayOfWeek.Friday }");
            return day == DayOfWeek.Friday;
        }

        protected virtual ILogger GetLogger()
        {
            return new ConsoleLogger();
        }
    }
    class FancyDateHelper : DateHelper
    {
        protected override ILogger GetLogger()
        {
            return new FancyLogger();
        }
    }
}
