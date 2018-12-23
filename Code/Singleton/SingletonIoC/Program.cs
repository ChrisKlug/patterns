using Autofac;
using System;

namespace IocSingleton
{
    class Program
    {
        static void Main(string[] args)
        {
            // Leave it all default
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLogger>()
                .As<ILogger>()
                .SingleInstance();
            var container = builder.Build();

            for (int i = 0; i < 10; i++)
            {
                container.Resolve<ILogger>().Log("Hello number " + i);
            }
        }
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleLogger : ILogger
    {
        private readonly string _prefix = DateTime.UtcNow.Millisecond.ToString();

        public void Log(string message)
        {
            Console.WriteLine("{0}: {1}", _prefix, message);
        }
    }
}
