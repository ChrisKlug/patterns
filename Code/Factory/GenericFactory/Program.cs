using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GenericFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileFactory = GetFileFactory();

            fileFactory.Create("c:\\test\\test.txt").Open();
            fileFactory.Create("http://www.google.com/test.pdf").Open();

            Console.ReadKey();
        }

        static IFileFactory GetFileFactory()
        {
            var fileFactory = new FileFactory();

            fileFactory.RegisterCreator(x => x.StartsWith("http://") || x.StartsWith("https://"), url => new HttpFile(url));
            fileFactory.RegisterCreator(x => true, url => new File(url));

            return fileFactory;
        }
    }

    class Factory<T, U>
    {
        private IList<(Predicate<U> Predicate, Func<U, T> Creator)> _creators = new List<(Predicate<U> Predicate, Func<U, T> Creator)>();

        public void RegisterCreator(Predicate<U> predicate, Func<U, T> creator)
        {
            _creators.Add((Predicate: predicate, Creator: creator));
        }

        protected T Get(U obj)
        {
            if (!_creators.Any(y => y.Predicate(obj)))
            {
                return default(T);
            }
            return _creators.FirstOrDefault(y => y.Predicate(obj)).Creator(obj);
        }
    }

    interface IFile
    {
        Stream Open();
        string Name { get; }
    }

    class File : IFile
    {
        private readonly string _path;

        public File(string path)
        {
            Name = Path.GetFileName(path);
            this._path = path;
        }

        public string Name { get; }

        public Stream Open()
        {
            Console.WriteLine("Opening file from " + _path);
            // TODO: Implement
            return null;
        }
    }
    class HttpFile : IFile
    {
        private readonly string _url;

        public HttpFile(string path)
        {
            Name = Path.GetFileName(path);
            _url = path;
        }

        public string Name { get; }

        public Stream Open()
        {
            Console.WriteLine("Fetching file over HTTP from " + _url);
            // TODO: Implement
            return null;
        }
    }

    interface IFileFactory
    {
        IFile Create(string path);
    }

    class FileFactory : Factory<IFile, string>, IFileFactory
    {
        public IFile Create(string path)
        {
            return Get(path);
        }
    }
}
