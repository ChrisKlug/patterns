using System;
using System.Diagnostics;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]

namespace Adapter
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger log = new Log4NetLogger<Program>();

            log.Log(LogLevel.Info, "Here is some info");
            log.Log(LogLevel.Debug, "Here is some debug information");
            log.Log(LogLevel.Warn, "Here is a warning");
            log.Log(LogLevel.Error, "Here is an error");

            Console.ReadKey();
        }
    }

    interface ILogger
    {
        void Log(LogLevel level, string message);
    }

    enum LogLevel
    {
        Info,
        Debug,
        Warn,
        Error
    }

    class Log4NetLogger<T> : ILogger
    {
        private log4net.ILog _log;

        public Log4NetLogger()
        {
            _log = log4net.LogManager.GetLogger(typeof(T));
        }

        public void Log(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Info:
                    _log.Info(message);
                    break;
                case LogLevel.Debug:
                    _log.Debug(message);
                    break;
                case LogLevel.Warn:
                    _log.Warn(message);
                    break;
                case LogLevel.Error:
                    _log.Error(message);
                    break;
            }
        }
    }
}
