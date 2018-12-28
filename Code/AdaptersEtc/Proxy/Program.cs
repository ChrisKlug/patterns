using System;
using System.Security.Authentication;
using System.Threading;

namespace Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            IIsItFridayService svc = new IsItFridayProxy();

            Console.WriteLine("Is it friday? {0}", svc.IsItFriday());

            Console.ReadKey();
        }
    }

    interface IIsItFridayService
    {
        bool IsItFriday();
    }

    class IsItFridayProxy : IIsItFridayService
    {
        IIsItFridayService _instance;

        public bool IsItFriday()
        {
            if (!Environment.UserName.Equals("Chris", StringComparison.OrdinalIgnoreCase))
            {
                throw new AuthenticationException("You are not allowed to do this");
            }
            return Instance.IsItFriday();
        }

        private IIsItFridayService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IsItFridayService();
                }
                return _instance;
            }
        }
    }

    class IsItFridayService : IIsItFridayService
    {
        public bool IsItFriday()
        {
            return DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday;
        }
    }
}
