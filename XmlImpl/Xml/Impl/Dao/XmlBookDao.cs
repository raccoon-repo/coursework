using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Xml;
using BookLibrary.Core.Dao;
using BookLibrary.Entities;
using BookLibrary.Entities.Proxies;

namespace BookLibrary.Xml.Impl.Dao
{
    public class XmlBookDao : IBookDao
    {
        private DocumentHolder _documentHolder;
        private XmlAuthorDao _authorDao;
        
        private static ConcurrentDictionary<int, Book> 
            cache = new ConcurrentDictionary<int, Book>();
        
        public DocumentHolder DocumentHolder
        {
            get => _documentHolder;
            set => _documentHolder = value;
        }

        public XmlAuthorDao AuthorDao
        {
            get => _authorDao;
            set => _authorDao = value;
        }
        
        public XmlBookDao() { }

        public XmlBookDao(DocumentHolder documentHolder)
        {
            _documentHolder = documentHolder ?? 
                  throw new ArgumentException("Specify documentHolder");
        }
        
        
        public Book FindById(int id)
        {
            Book book;

            if (cache.TryGetValue(id, out book))
                return book;
            
            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;

            var bookNode = root.SelectSingleNode("book[@id='" + id + "']");
            if (bookNode is null)
                return null;

            book = ParseBook(bookNode);
            cache.TryAdd(id, book);
            return book;
        }

        public IList<Book> FindAll()
        {
            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;

            return ParseBooks(root);
        }

        public IList<Book> FindBySection(BookSection section)
        {
            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;
            var bookNodes = root.SelectNodes("book[section='" + section + "']");
            return ParseBooks(bookNodes);

        }

        public IList<Book> FindByAuthor(Author author)
        {
            throw new System.NotImplementedException();
        }

        public IList<Book> FindByTitle(string title)
        {
            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;

            var bookNodes = root.SelectNodes("book[title='" + title + "']");
            return ParseBooks(bookNodes);
        }

        public IList<Book> FindByRating(float from, float to)
        {
            var fromStr = Utils.Utils.FloatToString(from);
            var toStr = Utils.Utils.FloatToString(to);

            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;

            var bookNodes = root.SelectNodes("book[rating >= " + fromStr + " and rating <= " + to + "]");

            return ParseBooks(bookNodes);
        }

        public IList<Book> FindByRating(float rating)
        {
            var ratingStr = Utils.Utils.FloatToString(rating);
            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;

            var bookNodes = root.SelectNodes("book[rating='" + rating + "']");
            return ParseBooks(bookNodes);
        }

        public bool BookIsPresent(Book book)
        {
            return FindById(book.Id) != null;
        }

        public void Save(Book book, SaveOption option)
        {
            throw new System.NotImplementedException();
        }

        public void Save(Book book, SaveOption option, ISet<int> savedAuthors, ISet<int> savedBooks)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Book book)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Book book, ISet<int> updatedAuthors, ISet<int> updatedBooks)
        {
            throw new System.NotImplementedException();
        }

        public Book Refresh(Book book)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(Book book)
        {
            throw new System.NotImplementedException();
        }

        private Book ParseBook(XmlNode bookNode)
        {
            var title = bookNode.SelectSingleNode("./title")?.FirstChild?.Value;
            var description = bookNode.SelectSingleNode("./description")?.FirstChild?.Value;
            var sectionValue = bookNode.SelectSingleNode("./section")?.FirstChild?.Value;
            var idValue = bookNode.Attributes?["id"].Value;
            var ratingValue = bookNode.SelectSingleNode("./rating")?.FirstChild?.Value;

            if (idValue == null)
            {
                return null;
            }


            var id = int.Parse(idValue);
            var rating = ratingValue == null ? 1.0f : float.Parse(ratingValue);
            var section = Utils.BookUtils.ParseSection(sectionValue);
            
            return new BookProxy() {
                Id = id, Title = title,
                Section = section,
                Rating = rating,
                Description = description,
                AuthorDao = _authorDao
            };
        }

        private IList<Book> ParseBooks(XmlNode rootNode)
        {
            IList<Book> books = new List<Book>();
            XmlNodeList nodes = rootNode.SelectNodes("./book");
            foreach (XmlNode bookNode in nodes)
                books.Add(ParseBook(bookNode));
            
            return books;
        }

        private IList<Book> ParseBooks(XmlNodeList list)
        {
            IList<Book> books = new List<Book>();
            foreach (XmlNode bookNode in list)
                books.Add(ParseBook(bookNode));

            return books;
        }
        
        
    }
}