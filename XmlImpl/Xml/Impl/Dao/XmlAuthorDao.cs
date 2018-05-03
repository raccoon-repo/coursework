using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using BookLibrary.Core.Dao;
using BookLibrary.Entities;
using BookLibrary.Entities.Proxies;

namespace BookLibrary.Xml.Impl.Dao
{
    public class XmlAuthorDao : IAuthorDao
    {
        private XmlBookDao _xmlBookDao;
        private DocumentHolder _documentHolder;

        private static ConcurrentDictionary<int, Author> cache = 
            new ConcurrentDictionary<int, Author>();
        
        public DocumentHolder DocumentHolder
        {
            get => _documentHolder;
            set => _documentHolder = value;
        }

        public XmlBookDao BookDao
        {
            get => _xmlBookDao;
            set => _xmlBookDao = value;
        }
        
        public XmlAuthorDao() { }

        public XmlAuthorDao(DocumentHolder documentHolder)
        {
            _documentHolder = documentHolder ?? 
                 throw new ArgumentException("Specify documentHolder");

        }

        public Author FindById(int id)
        {
            var authorNode = GetAuthorNode(id);
            if (authorNode is null)
                return null;

            var author = ParseAuthor(authorNode);
            cache.TryAdd(author.Id, author);

            return author;

        }

        public IList<Author> FindAll()
        {
            throw new System.NotImplementedException();
        }

        public IList<Author> FindByName(string firstName, string lastName)
        {
            throw new System.NotImplementedException();
        }

        public IList<Author> FindByBook(Book book)
        {
            var ids = _xmlBookDao.GetAuthorsIdByBook(book);
            var authors = new List<Author>();

            foreach (var id in ids)
            {
                authors.Add(FindById(id));
            }

            return authors;
        }

        public void Save(Author author, SaveOption option)
        {
            Save(author, option, new HashSet<int>(), new HashSet<int>());
        }

        public void Save(Author author, SaveOption option, ISet<int> savedAuthors, ISet<int> savedBooks)
        {
            if (FindById(author.Id) != null)
            {
                switch (option)
                {
                    case SaveOption.UPDATE_IF_EXIST:
                        Update(author, savedAuthors, savedBooks);
                        return;
                    case SaveOption.SAVE_ONLY:
                        return;;
                }
            }

            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;
            
            _documentHolder.IncrementLastId();
            author.Id = _documentHolder.GetLastInsertedId();

            var idAttr = xDoc.CreateAttribute("id");
            idAttr.Value = author.Id.ToString();

            var authorNode = xDoc.CreateElement("author");
            
            var firstNameNode = xDoc.CreateElement("firstName");
            firstNameNode.AppendChild(xDoc.CreateTextNode(author.FirstName));

            var lastNameNode = xDoc.CreateElement("lastName");
            lastNameNode.AppendChild(xDoc.CreateTextNode(author.LastName));

            var booksNode = xDoc.CreateElement("books");
            
            authorNode.Attributes.Append(idAttr);
            authorNode.AppendChild(firstNameNode);
            authorNode.AppendChild(lastNameNode);
            authorNode.AppendChild(booksNode);

            root.AppendChild(authorNode);
            savedAuthors.Add(author.Id);
            
            foreach (var book in author.Books)
            {
                if (!savedBooks.Contains(book.Id))
                {
                    _xmlBookDao.Save(book, SaveOption.UPDATE_IF_EXIST,
                        savedAuthors, savedBooks);
                }
                
                var idNode = xDoc.CreateElement("id");
                idNode.AppendChild(xDoc.CreateTextNode(book.Id.ToString()));
                booksNode.AppendChild(idNode);

            }

            cache.TryAdd(author.Id, new AuthorProxy(author) {
                BookDao = _xmlBookDao
            });
            
            xDoc.Save(_documentHolder.Path);
        }

        public void Update(Author author)
        {
            if (author.Id <= 0)
                return;
            
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var authorNode = root.SelectSingleNode("author[@id = '" + author.Id + "']");
            
            if (authorNode is null)
                return;
            
            //first name
            var firstNameNode = authorNode.SelectSingleNode("./firstName");
            AppendText(xDoc, firstNameNode, author.FirstName);
            
            //last name
            var lastNameNode = authorNode.SelectSingleNode("./lastName");
            AppendText(xDoc, lastNameNode, author.LastName);


            if (author is AuthorProxy a)
            {
                a.Update();
            }
            else
            {
                cache.TryAdd(author.Id, new AuthorProxy(author) {
                    BookDao = _xmlBookDao
                });
            }
            
            xDoc.Save(_documentHolder.Path);
        }

        public void Update(Author author, ISet<int> updatedAuthors, ISet<int> updatedBooks)
        {
            if (updatedAuthors.Contains(author.Id)) return;
            Update(author);
            updatedAuthors.Add(author.Id);

            var authorIsProxy = author is AuthorProxy;
            var booksAreFetched = authorIsProxy && ((AuthorProxy) author).BooksAreFetchedOrSet;

            
            if ((authorIsProxy && !booksAreFetched) || (!authorIsProxy && author.Books.Count == 0))
                return;

            var actualBooks = new HashSet<int>();
            
            foreach (var book in author.Books)
            {
                if (!updatedBooks.Contains(book.Id))
                {
                    _xmlBookDao.Save(book, SaveOption.UPDATE_IF_EXIST,
                        updatedAuthors, updatedBooks);
                }

                actualBooks.Add(book.Id);
            }
            
            var fetchedBooks = new HashSet<int>(GetBooksIdByAuthor(author));

            // no books have been deleted
            if (fetchedBooks.Count == actualBooks.Count)
                return;
            
            // some books have been added
            if (actualBooks.Count > fetchedBooks.Count)
            {
                actualBooks.ExceptWith(fetchedBooks);

                foreach (var bookId in actualBooks)
                {
                    AddBookForAuthor(bookId, author.Id);
                }
            }
            // some book have been removed
            else
            {
                fetchedBooks.ExceptWith(actualBooks);

                foreach (var bookId in fetchedBooks)
                {
                    RemoveBookFromAuthor(author.Id, bookId);
                    _xmlBookDao.RemoveAuthorFromBook(bookId, author.Id);
                }
            }

            _documentHolder.Document.Save(_documentHolder.Path);
        }

        public Author Refresh(Author author)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(Author author)
        {
            throw new System.NotImplementedException();
        }

        public IList<int> GetBooksIdByAuthor(Author a)
        {
            var authorNode = GetAuthorNode(a);
            var booksNode = authorNode.SelectSingleNode("./books");
            
            var list = new List<int>();
            string value;
            
            foreach (XmlNode node in booksNode.ChildNodes)
            {
                value = node.FirstChild.Value;
                list.Add(int.Parse(value));
            }

            return list;

        }

        public Author ParseAuthor(XmlNode authorNode)
        {
            if (authorNode is null)
                throw new ArgumentException("authorNode cannot be null");

            var id = int.Parse(authorNode.Attributes["id"].Value);
            var firstName = authorNode.SelectSingleNode("./firstName")
                ?.FirstChild.Value;

            var lastName = authorNode.SelectSingleNode("./lastName")
                ?.FirstChild.Value;

            var proxy = new AuthorProxy(id, firstName, lastName) {
                BookDao = _xmlBookDao
            };

            return proxy;
        }

        public void RemoveBookFromAuthor(Author author, Book book)
        {
            RemoveBookFromAuthor(author.Id, book.Id);
        }

        public void RemoveBookFromAuthor(int authorId, int bookId)
        {
            var xDoc = DocumentHolder.Document;
            var authorNode = GetAuthorNode(authorId);

            var booksNode = authorNode?.SelectSingleNode("./books");
            var bookNode = authorNode?.SelectSingleNode("./books/id[. ='" + bookId + "']");
            
            if (booksNode is null || bookNode is null)
                return;

            booksNode.RemoveChild(bookNode);
            
            xDoc.Save(_documentHolder.Path);    
        }

        public void AddBookForAuthor(int bookId, int authorId)
        {
            var xDoc = DocumentHolder.Document;
            var authorNode = GetAuthorNode(authorId);

            var booksNode = authorNode?.SelectSingleNode("./books");

            var idNode = xDoc.CreateElement("id");
            idNode.AppendChild(xDoc.CreateTextNode(bookId.ToString()));

            booksNode.AppendChild(idNode);
            xDoc.Save(_documentHolder.Path);

        }

        public void AddBookForAuthor(Book book, Author author)
        {
            AddBookForAuthor(book.Id, author.Id);
        }
        
        private XmlNode GetAuthorNode(Author author)
        {
            return GetAuthorNode(author.Id);
        }

        private XmlNode GetAuthorNode(int id)
        {
            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;
            
            var authorNode = root.SelectSingleNode("author[@id ='" + id + "']");

            return authorNode;
        }
        
        private void AppendText(XmlDocument xDoc, XmlNode parent, string text)
        {
            if (!parent.HasChildNodes)
                parent.AppendChild(xDoc.CreateTextNode(text));
            else
                parent.FirstChild.Value = text;
        }
    }
}