using System;

namespace Decorator
{
    class Program
    {
        static void Main(string[] args)
        {
            IIsItFridayService svc = new IsItFridayService();
            svc = new CachingDecorator(svc, TimeSpan.FromMinutes(5));
            svc = new LoggingDecorator(svc);

            Console.WriteLine("Is it friday? {0}", svc.IsItFriday());
            Console.WriteLine("Is it friday? {0}", svc.IsItFriday());

            Console.ReadKey();
        }
    }

    interface IIsItFridayService
    {
        bool IsItFriday();
    }

    class IsItFridayService : IIsItFridayService
    {
        public bool IsItFriday()
        {
            return DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday;
        }
    }

    class CachingDecorator : IIsItFridayService
    {
        private readonly IIsItFridayService _service;
        private readonly TimeSpan _cacheTimeout;
        private bool _cachedValue;
        private DateTime _lastRefresh = DateTime.MinValue;

        public CachingDecorator(IIsItFridayService service, TimeSpan cacheTimeout)
        {
            _service = service;
            _cacheTimeout = cacheTimeout;
        }

        public bool IsItFriday()
        {
            if (_lastRefresh + _cacheTimeout < DateTime.Now)
            {
                Console.WriteLine("Cache miss! Getting new value!");
                _cachedValue = _service.IsItFriday();
                _lastRefresh = DateTime.Now;
            }
            return _cachedValue;
        }
    }
    class LoggingDecorator : IIsItFridayService
    {
        private readonly IIsItFridayService _service;

        public LoggingDecorator(IIsItFridayService service)
        {
            _service = service;
        }

        public bool IsItFriday()
        {
            var val = _service.IsItFriday();
            Console.WriteLine("LoggingDecorator returning {0}", val);
            return val;
        }
    }
}
