using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;
using BookLibrary.Core.Dao;
using BookLibrary.Entities;
using BookLibrary.Entities.Proxies;

namespace BookLibrary.Xml.Impl.Dao
{
    public class XmlBookDao : IBookDao
    {
        private DocumentHolder _documentHolder;
        private XmlAuthorDao _xmlAuthorDao;
        
        private static ConcurrentDictionary<int, Book> 
            cache = new ConcurrentDictionary<int, Book>();
        
        public DocumentHolder DocumentHolder
        {
            get => _documentHolder;
            set => _documentHolder = value;
        }

        public XmlAuthorDao AuthorDao
        {
            get => _xmlAuthorDao;
            set => _xmlAuthorDao = value;
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
            var ids = _xmlAuthorDao.GetBooksIdByAuthor(author);
            var result = new List<Book>();

            foreach (var id in ids)
                result.Add(FindById(id));

            return result;
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
            var fromStr = BookLibrary.Utils.FloatToString(from);
            var toStr = BookLibrary.Utils.FloatToString(to);

            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;

            var bookNodes = root.SelectNodes("book[rating >= " + fromStr + " and rating <= " + to + "]");

            return ParseBooks(bookNodes);
        }

        public IList<Book> FindByRating(float rating)
        {
            var ratingStr = BookLibrary.Utils.FloatToString(rating);
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
            Save(book, option, new HashSet<int>(), new HashSet<int>());
        }

        public void Save(Book book, SaveOption option, ISet<int> savedAuthors, ISet<int> savedBooks)
        {
            
            if (BookIsPresent(book))
            {
                switch (option)
                {
                    case SaveOption.SAVE_ONLY:
                        return;
                    case SaveOption.UPDATE_IF_EXIST:
                        Update(book, savedAuthors, savedBooks);
                        return;
                }
            }

            
           
            //Implementation of AUTO_INCREMENT
            _documentHolder.IncrementLastId();
            var id = _documentHolder.GetLastInsertedId();
            book.Id = id;
            
            var bRating = BookLibrary.Utils.FloatToString(book.Rating);
            
            var xDoc = DocumentHolder.Document;
            var root = xDoc.DocumentElement;
            
            //book
            var bookNode = xDoc.CreateElement("book");
            var idAttr = xDoc.CreateAttribute("id");
            idAttr.Value = book.Id.ToString();
            
            //title
            var titleNode = xDoc.CreateElement("title");
            titleNode.AppendChild(xDoc.CreateTextNode(book.Title));

            //description
            var descriptionNode = xDoc.CreateElement("description");
            descriptionNode.AppendChild(xDoc.CreateTextNode(book.Description));

            //rating
            var ratingNode = xDoc.CreateElement("rating");
            ratingNode.AppendChild(xDoc.CreateTextNode(bRating));

            //section
            var sectionNode = xDoc.CreateElement("section");
            sectionNode.AppendChild(xDoc.CreateTextNode(book.Section.ToString()));

            //authors
            var authorsNode = xDoc.CreateElement("authors");

            bookNode.Attributes.Append(idAttr);
            bookNode.AppendChild(sectionNode);
            bookNode.AppendChild(titleNode);
            bookNode.AppendChild(descriptionNode);
            bookNode.AppendChild(ratingNode);
            bookNode.AppendChild(authorsNode);

            root.AppendChild(bookNode);
            savedBooks.Add(book.Id);

            if (book is BookProxy p && !p.AuthorsAreFetchedOrSet)
                return;

            foreach (var author in book.Authors)
            {
                if (!savedAuthors.Contains(author.Id))
                {
                    _xmlAuthorDao.Save(author, SaveOption.UPDATE_IF_EXIST,
                        savedAuthors, savedBooks);
                    
                }
                
                var idNode = xDoc.CreateElement("id");
                idNode.AppendChild(xDoc.CreateTextNode(author.Id.ToString()));
                authorsNode.AppendChild(idNode);

            }

            cache.TryAdd(book.Id, new BookProxy(book) {
                AuthorDao = _xmlAuthorDao
            });
            
            xDoc.Save(_documentHolder.Path);
        }

        public void Update(Book book)
        {
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var bookNode = root.SelectSingleNode("book[@id = '" + book.Id + "']");
            
            if (bookNode == null)
                return;

            //title
            var titleNode = bookNode.SelectSingleNode("./title");
            AppendText(xDoc, titleNode, book.Title);
            
            //section
            var sectionNode = bookNode.SelectSingleNode("./section");
            AppendText(xDoc, sectionNode, book.Section.ToString());    
                       
            //description
            var descriptionNode = bookNode.SelectSingleNode("./description");
            AppendText(xDoc, descriptionNode, book.Description);    
            
            //rating
            var ratingNode = bookNode.SelectSingleNode("./rating");
            AppendText(xDoc, ratingNode, BookLibrary.Utils.FloatToString(book.Rating));    
            
            if (book is BookProxy proxy)
            {
                proxy.Update();
            }
            else
            {
                cache.TryAdd(book.Id, new BookProxy() {
                    AuthorDao = _xmlAuthorDao
                });
            }
            
            xDoc.Save(_documentHolder.Path);            
        }

        public void Update(Book book, ISet<int> updatedAuthors, ISet<int> updatedBooks)
        {
            if (updatedBooks.Contains(book.Id)) return;
            
            Update(book);
            updatedBooks.Add(book.Id);

            var bookIsProxy = book is BookProxy;
            var authorsAreFetched = bookIsProxy && ((BookProxy) book).AuthorsAreFetchedOrSet;

            if ((bookIsProxy && !authorsAreFetched) || (!bookIsProxy && book.Authors.Count == 0)) 
                return;
            
            var actualAuthors = new HashSet<int>();

            foreach (var author in book.Authors)
            {
                if (!updatedAuthors.Contains(author.Id))
                {
                    _xmlAuthorDao.Save(author, SaveOption.UPDATE_IF_EXIST,
                        updatedAuthors, updatedBooks);
                }

                actualAuthors.Add(author.Id);
            }

            var fetchedAuthors = new HashSet<int>(GetAuthorsIdByBook(book));

            if (actualAuthors.Count == fetchedAuthors.Count)
                return;

            if (actualAuthors.Count > fetchedAuthors.Count)
            {
                actualAuthors.ExceptWith(fetchedAuthors);

                foreach (var authorId in actualAuthors)
                {
                    AddAuthorForBook(book.Id, authorId);
                }
            }
            else
            {
                fetchedAuthors.ExceptWith(actualAuthors);

                foreach (var authorId in fetchedAuthors)
                {
                    RemoveAuthorFromBook(book.Id, authorId);
                    _xmlAuthorDao.RemoveBookFromAuthor(authorId, book.Id);
                }
            }
            
            _documentHolder.Document.Save(_documentHolder.Path);

        }

        public Book Refresh(Book book)
        {
            if (book.Id <= 0)
                return book;
            
            if (book is BookProxy proxy)
            {
                proxy.Refresh();
                return proxy;
            }

            var temp = FindById(book.Id);

            if (temp is null)
                return book;

            book.Description = temp.Description;
            book.Title = temp.Title;
            book.Rating = temp.Rating;
            book.Section = temp.Section;


            return temp;
        }

        public void Delete(Book book)
        {
            if (book.Id <= 0)
                return;


            foreach (var author in book.Authors)
            {
                _xmlAuthorDao.RemoveBookFromAuthor(author, book);
            }
            
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var bookNode = root.SelectSingleNode("book[@id ='" + book.Id + "']");
            root.RemoveChild(bookNode);

            cache.TryRemove(book.Id, out _);
            
            xDoc.Save(_documentHolder.Path);
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
            var section = BookLibrary.BookUtils.ParseSection(sectionValue);
            
            return new BookProxy(id, title, rating, section, description) {
                AuthorDao = _xmlAuthorDao
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

        public IList<int> GetAuthorsIdByBook(Book book)
        {
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var authors = root.SelectSingleNode("book[@id = '" + book.Id + "']/authors");
            var ids = new List<int>();

            foreach (XmlNode node in authors.ChildNodes)
            {
                var id = node.FirstChild.Value;
                ids.Add(int.Parse(id));
            }

            return ids;
        }

        public void RemoveAuthorFromBook(Book book, Author author)
        {
            RemoveAuthorFromBook(book.Id, author.Id);
        }

        public void RemoveAuthorFromBook(int bookId, int authorId)
        {
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var bookNode = root.SelectSingleNode("book[@id = '" + bookId + "']");
            var authorsNode = bookNode?.SelectSingleNode("./authors");
            var authorNode = bookNode?.SelectSingleNode("./authors/id[. = '" + authorId + "']");
            
            if (authorsNode is null || authorNode is null)
                return;

            authorsNode.RemoveChild(authorNode);
            
            xDoc.Save(_documentHolder.Path);
        }

        public void AddAuthorForBook(int bookId, int authorId)
        {
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var bookNode = root.SelectSingleNode("book[@id = '" + bookId + "']");
            var authorsNode = bookNode?.SelectSingleNode("./authors");

            var idNode = xDoc.CreateElement("id");
            idNode.AppendChild(xDoc.CreateTextNode(authorId.ToString()));

            authorsNode.AppendChild(idNode);
            
            xDoc.Save(_documentHolder.Path);
        }

        public void AddAuthorForBook(Book book, Author author)
        {
            AddAuthorForBook(book.Id, author.Id);
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