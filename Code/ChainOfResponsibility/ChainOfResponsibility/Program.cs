using System;
using System.Collections.Generic;
using System.Linq;

namespace ChainOfResponsibility
{
    class Program
    {
        static void Main(string[] args)
        {
            var memberGreetingHandler = new GenericGreetingHandler(x => x.IsMember, x => $"Hello {x.Username}! Welcome back!");
            var chrisGreetingHandler = new GenericGreetingHandler(x => x.Username.Equals("Chris", StringComparison.OrdinalIgnoreCase), x => $"Welcome back you awesome you!");

            var greetingService = new GreetingService();
            greetingService.AddHandler(chrisGreetingHandler);
            greetingService.AddHandler(memberGreetingHandler);

            var nobody = new User("Nobody", false);
            var joe = new User("Joe", true);
            var chris = new User("Chris", true);

            Console.WriteLine("Welcoming nobody: {0}", greetingService.GetGreetingFor(nobody));
            Console.WriteLine("Welcoming Joe: {0}", greetingService.GetGreetingFor(joe));
            Console.WriteLine("Welcoming Chris: {0}", greetingService.GetGreetingFor(chris));

            Console.ReadKey();
        }
    }

    class User
    {
        public User(string username, bool isMember)
        {
            Username = username;
            IsMember = isMember;
        }

        public string Username { get; }
        public bool IsMember { get; }
    }

    class GreetingService
    {
        IList<IGreetingHandler> _handlers = new List<IGreetingHandler> { new DefaultGreetingHandler() };

        public void AddHandler(IGreetingHandler handler)
        {
            handler.SetNextHandler(_handlers.First());
            _handlers.Insert(_handlers.Count - 1, handler);
        }
        public string GetGreetingFor(User user)
        {
            return _handlers[0].GetGreetingFor(user);
        }

        private class DefaultGreetingHandler : IGreetingHandler
        {
            public string GetGreetingFor(User user)
            {
                return "Welcome!";
            }

            public void SetNextHandler(IGreetingHandler handler)
            {
            }
        }
    }

    class GenericGreetingHandler : IGreetingHandler
    {
        private readonly Predicate<User> _predicate;
        private readonly Func<User, string> _getGreeting;
        IGreetingHandler _next;

        public GenericGreetingHandler(Predicate<User> predicate, Func<User, string> getGreeting)
        {
            _predicate = predicate;
            _getGreeting = getGreeting;
        }

        public string GetGreetingFor(User user)
        {
            if (_predicate(user))
            {
                return _getGreeting(user);
            }

            return _next.GetGreetingFor(user);
        }

        public void SetNextHandler(IGreetingHandler handler)
        {
            _next = handler;
        }
    }

    interface IGreetingHandler
    {
        void SetNextHandler(IGreetingHandler handler);
        string GetGreetingFor(User user);
    }
}
