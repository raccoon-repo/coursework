using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml;
using BookLibrary.Core.Dao;
using BookLibrary.Entities;
using BookLibrary.Entities.Proxies;
using BookLibrary.Xml.Utils;

namespace BookLibrary.Xml.Impl.Dao
{
    public class XmlAuthorDao : IAuthorDao
    {
        private static ConcurrentDictionary<int, Author> cache = 
            new ConcurrentDictionary<int, Author>();
        
        public DocumentHolder DocumentHolder { get; set; }

        public INodeHandler NodeHandler { get; set; }
        
        public INodeParser<Author> Parser { get; set; }

        public XmlBookDao BookDao { get; set; }

        public XmlAuthorDao() { }

        public XmlAuthorDao(DocumentHolder documentHolder)
        {
            DocumentHolder = documentHolder ?? 
                 throw new ArgumentException("Specify documentHolder");

        }

        public Author FindById(int id)
        {
            var root = DocumentHolder.Document.DocumentElement;
            var authorNode = NodeHandler.GetNodeById(root, id);
            
            if (authorNode is null)
                return null;

            var author = Parser.Parse(authorNode);
            cache.TryAdd(author.Id, author);

            return author;

        }

        public IList<Author> FindAll()
        {
            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;

            return Parser.ParseNodes(root.ChildNodes);
        }

        public IList<Author> FindByName(string firstName, string lastName)
        {
            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;

            var authorNodes =
                root.SelectNodes("./author[firstName ='" + firstName + "' and lastName = '" + lastName + "']");

            return Parser.ParseNodes(authorNodes);
        }

        public IList<Author> FindByBook(Book book)
        {
            var ids = BookDao.GetAuthorsIdByBook(book);
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

            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;
            
            DocumentHolder.IncrementLastId();
            author.Id = DocumentHolder.GetLastInsertedId();

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
                    BookDao.Save(book, SaveOption.UPDATE_IF_EXIST,
                        savedAuthors, savedBooks);
                }
                
                var idNode = xDoc.CreateElement("id");
                idNode.AppendChild(xDoc.CreateTextNode(book.Id.ToString()));
                booksNode.AppendChild(idNode);

            }

            cache.TryAdd(author.Id, new AuthorProxy(author) {
                BookDao = BookDao
            });
            
            xDoc.Save(DocumentHolder.Path);
        }

        public void Update(Author author)
        {
            if (author.Id <= 0)
                return;
            
            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;

            var authorNode = root?.SelectSingleNode("author[@id = '" + author.Id + "']");
            
            if (authorNode is null)
                return;
            
            //first name
            var firstNameNode = authorNode.SelectSingleNode("./firstName");
            NodeHandler.AppendText(xDoc, firstNameNode, author.FirstName);
            
            //last name
            var lastNameNode = authorNode.SelectSingleNode("./lastName");
            NodeHandler.AppendText(xDoc, lastNameNode, author.LastName);


            if (author is AuthorProxy a)
            {
                a.Update();
            }
            else
            {
                cache.TryAdd(author.Id, new AuthorProxy(author) {
                    BookDao = BookDao
                });
            }
            
            xDoc.Save(DocumentHolder.Path);
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
                    BookDao.Save(book, SaveOption.UPDATE_IF_EXIST,
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
                    NodeHandler.AddBookToAuthor(bookId, author.Id);
                }
            }
            // some book have been removed
            else
            {
                fetchedBooks.ExceptWith(actualBooks);

                foreach (var bookId in fetchedBooks)
                {
                    NodeHandler.RemoveBookFromAuthor(bookId, author.Id);
                    NodeHandler.RemoveAuthorFromBook(bookId, author.Id);
                }
            }

            DocumentHolder.Document.Save(DocumentHolder.Path);
        }

        public Author Refresh(Author author)
        {
            if (author.Id <= 0)
                return author;

            if (author is AuthorProxy proxy)
            {
                proxy.Refresh();
                return proxy;
            }

            var temp = FindById(author.Id);

            author.FirstName = temp.FirstName;
            author.LastName = temp.LastName;
            author.Books = temp.Books;

            return temp;

        }

        public void Delete(Author author)
        {
            if (author.Id <= 0)
                return;

            foreach (var book in author.Books)
            {
                NodeHandler.RemoveAuthorFromBook(book.Id, author.Id);
            }

            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;

            var authorNode = NodeHandler.GetNodeById(root, author.Id);

            root.RemoveChild(authorNode);
            cache.TryRemove(author.Id, out _);
            
            xDoc.Save(DocumentHolder.Path);

        }

        public IList<int> GetBooksIdByAuthor(Author a)
        {
            var root = DocumentHolder.Document.DocumentElement;
            var authorNode = NodeHandler.GetNodeById(root, a.Id);
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
    }
}
