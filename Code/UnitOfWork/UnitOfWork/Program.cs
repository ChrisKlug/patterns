using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnitOfWork
{
    class Program
    {
        static void Main(string[] args)
        {
            var users = new InMemoryStore();
            Console.WriteLine("Starting up...\nNumber or users in store: " + users.Count);

            Console.WriteLine("\nAdding some users");
            var uowUsers = new InMemoryStore();
            var uow = new UnitOfWork(uowUsers);
            for (int i = 0; i < 5; i++)
            {
                uowUsers.Add(new User(i, "User " + i));
            }
            Console.WriteLine("Number or users in store: " + users.Count);
            Console.WriteLine("Completing unit of work...");
            uow.Complete();
            Console.WriteLine("Number or users in store: " + users.Count);

            Console.WriteLine("\nAdding and removing some users");
            uowUsers = new InMemoryStore();
            uow = new UnitOfWork(uowUsers);
            for (int i = 6; i < 11; i++)
            {
                uowUsers.Add(new User(i, "User " + i));
            }
            for (int i = 6; i < 11; i=i+3)
            {
                uowUsers.Remove(uowUsers.WhereIdIs(i));
            }
            Console.WriteLine("Number or users in store: " + users.Count);
            Console.WriteLine("Completing unit of work...");
            uow.Complete();
            Console.WriteLine("Users in store: ");
            foreach (var user in users.All())
            {
                Console.WriteLine(" - " + user.Name);
            }

            Console.ReadKey();
        }
    }

    class User
    {
        public User(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }
    }

    class InMemoryStore : ISupportUnitOfWork
    {
        static Hashtable _users = new Hashtable();
        readonly Hashtable _addedUsers = new Hashtable();
        readonly Hashtable _removedUsers = new Hashtable();

        public User[] All()
        {
            return _users.Values.Cast<User>().OrderBy(x => x.Id).ToArray();
        }
        public User WhereIdIs(int id)
        {
            if (_addedUsers.ContainsKey(id))
            {
                return (User)_addedUsers[id];
            }
            else if (_users.ContainsKey(id))
            {
                return (User)_users[id];
            }
            return null;
        }
        public void Add(User user)
        {
            if (_users.ContainsKey(user.Id))
            {
                throw new InvalidOperationException("Cannot insert User with duplicate key");
            }
            _addedUsers.Add(user.Id, user);
        }
        public void Remove(User user)
        {
            if (_addedUsers.ContainsKey(user.Id))
            {
                _addedUsers.Remove(user.Id);
            }
            else if (_users.ContainsKey(user.Id))
            {
                _users.Remove(user.Id);
            }
        }

        void ISupportUnitOfWork.Complete()
        {
            foreach (User user in _addedUsers.Values)
            {
                _users.Add(user.Id, user);
            }
            foreach (User user in _removedUsers.Values)
            {
                _users.Remove(user.Id);
            }
            _addedUsers.Clear();
            _removedUsers.Clear();
        }

        public int Count
        {
            get
            {
                return _users.Count;
            }
        }
    }

    interface ISupportUnitOfWork
    {
        void Complete();
    }

    class UnitOfWork
    {
        private readonly ISupportUnitOfWork _item;
        bool _complete = false;

        public UnitOfWork(ISupportUnitOfWork item)
        {
            _item = item;
        }

        public void Complete()
        {
            if (_complete)
            {
                throw new InvalidOperationException("Cannot complete an already completed UnitOfWOrk");
            }
            _item.Complete();
        }
    }
}
