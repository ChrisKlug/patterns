using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Borg
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Using Configuration ---");
            Console.WriteLine("appName = {0}", new Configuration().GetSetting("appName"));
            Console.WriteLine("version = {0}", new Configuration().GetSetting("version"));

            Console.WriteLine("\n\n--- Using ExtendedConfiguration ---");
            Console.WriteLine("appName = {0}", new ExtendedConfiguration().GetSetting("appName"));
            Console.WriteLine("version = {0}", new ExtendedConfiguration().GetSetting("version"));
            Console.WriteLine("ENV:USERNAME = {0}", new ExtendedConfiguration().GetEnvironmentVariable("USERNAME"));
            Console.WriteLine("ENV:OS = {0}", new ExtendedConfiguration().GetEnvironmentVariable("OS"));

            Console.ReadKey();
        }
    }

    class Configuration
    {
        private static IDictionary<string, string> _values = new Dictionary<string, string>();

        static Configuration()
        {
            Console.WriteLine("\n> Initializing Configuration State <\n");
            var config = JObject.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "config.json")));
            foreach (var item in config.Properties())
            {
                _values.Add(item.Name, item.Value.Value<string>());
            }
        }

        public string GetSetting(string key) {
            return _values.ContainsKey(key) ? _values[key] : null;
        }
    }

    class ExtendedConfiguration : Configuration
    {
        private static IDictionary<string, string> _values = new Dictionary<string, string>();

        static ExtendedConfiguration()
        {
            Console.WriteLine("\n> Initializing ExtendedConfiguration State <\n");
            foreach (DictionaryEntry item in Environment.GetEnvironmentVariables())
            {
                _values.Add((string)item.Key, (string)item.Value);
            }
        }

        public string GetEnvironmentVariable(string key)
        {
            return _values.ContainsKey(key) ? _values[key] : null;
        }
    }
}
