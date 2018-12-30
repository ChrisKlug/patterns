using Autofac;
using System;

namespace ServiceLocator
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = CreateContainer();

            ServiceLocator.Initialize(container);

            var logger = ServiceLocator.Instance.Get<ILogger>();
            logger.Log("Hello World\n");

            var isItFriday = ServiceLocator.Instance.Get<IIsItFridayService>().IsItFriday();
            logger.Log("Is it friday? " + (isItFriday ? "Yes!!!" : "No..."));

            Console.ReadKey();
        }

        static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConsoleLogger>().As<ILogger>();
            builder.RegisterType<IsItFridayService>().As<IIsItFridayService>();

            return builder.Build();
        }
    }

    class ServiceLocator
    {
        static ServiceLocator _instance;
        private readonly IContainer _container;

        private ServiceLocator(IContainer container) {
            _container = container;
        }

        public T Get<T>()
        {
            try
            {
                return _container.Resolve<T>();
            }
            catch
            {
                return default(T);
            }
        }

        public static void Initialize(IContainer container)
        {
            Instance = new ServiceLocator(container);
        }

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("Cannot use service locator before it has ben initialized");
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
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

    interface IIsItFridayService
    {
        bool IsItFriday();
    }

    class IsItFridayService : IIsItFridayService
    {
        public bool IsItFriday()
        {
            ServiceLocator.Instance.Get<ILogger>().Log("...checking to see if it is friday...");

            return DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday;
        }
    }
}
