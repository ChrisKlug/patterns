using System;

namespace StaticFactoryMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            var dateHelper = DateHelper.Create(false);
            var isItFriday = dateHelper.IsItFriday();
            
            dateHelper = DateHelper.Create(true);
            isItFriday = dateHelper.IsItFriday();

            Console.ReadKey();
        }
    }

    class DateHelper
    {
        protected DateHelper()
        {

        }

        public static DateHelper Create(bool addLogging = false)
        {
            return addLogging ? new DateHelper() : new NonLoggingDateHelper();
        }

        public bool IsItFriday()
        {
            var day = DateTime.Now.DayOfWeek;
            Log($"Is { day.ToString() } friday? { day == DayOfWeek.Friday }");
            return day == DayOfWeek.Friday;
        }

        protected virtual void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
    class NonLoggingDateHelper : DateHelper
    {
        protected override void Log(string message) {}
    }
}
