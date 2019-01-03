using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository
{
    class Program
    {
        static void Main(string[] args)
        {
            using (IUserRepository users = new UserRepository())
            {
                Console.WriteLine(".: Getting admins :.\n");
                foreach (var user in users.WhoAreAdmins())
                {
                    Console.WriteLine("{0} {1}", user.FirstName, user.LastName);
                }

                Console.WriteLine("\n.: Getting deleted users :.\n");
                foreach (var user in users.ThatHaveBeenDeleted())
                {
                    Console.WriteLine("{0} {1}", user.FirstName, user.LastName);
                }

                Console.WriteLine("\n.: Getting users whose first name starts with C :.\n");
                foreach (var user in users.WhoseFirstNameStartsWith("C"))
                {
                    Console.WriteLine("{0} {1}", user.FirstName, user.LastName);
                }
            }

            Console.ReadKey();
        }
    }

    interface IUserRepository : IDisposable
    {
        IEnumerable<User> WhoAreAdmins();
        IEnumerable<User> ThatHaveBeenDeleted();
        IEnumerable<User> WhoseFirstNameStartsWith(string startsWith);

        void Add(User user);
        void Delete(User user);
    }

    class UserRepository : IUserRepository
    {
        private bool _disposed = false;
        private UsersContext _ctx = new UsersContext();
        
        public IEnumerable<User> WhoAreAdmins()
        {
            return _ctx.Users.Where(x => !x.IsDeleted && x.IsAdmin).ToArray();
        }
        public IEnumerable<User> ThatHaveBeenDeleted()
        {
            return _ctx.Users.Where(x => x.IsDeleted).ToArray();
        }
        public IEnumerable<User> WhoseFirstNameStartsWith(string startsWith)
        {
            return _ctx.Users.Where(x => !x.IsDeleted && x.FirstName.StartsWith(startsWith)).ToArray();
        }

        public void Add(User user)
        {
            _ctx.Users.Add(user);
        }
        public void Delete(User user)
        {
            user.IsDeleted = true;
        }

        protected void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("UserRespository");
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _ctx.Dispose();
            }
            _ctx = null;
            _disposed = true;
        }
        public void Dispose()
        {
            EnsureNotDisposed();
            Dispose(true);
        }
        ~UserRepository()
        {
            Dispose(false);
        }
    }
}
