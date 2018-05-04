using System;
using System.Collections.Generic;
 using BookLibrary.Entities;
using BookLibrary.Xml.Impl.Utils.Impl;

namespace BookLibrary.Xml.Impl.Dao
{
    public class DaoFactory
    {

        private static readonly Dictionary<DocumentHolder, XmlBookDao>
            BookDaoCache = new Dictionary<DocumentHolder, XmlBookDao>();
        
        private static readonly Dictionary<DocumentHolder, XmlAuthorDao>
            AuthorDaoCache = new Dictionary<DocumentHolder, XmlAuthorDao>();
        

        private static XmlBookDao BookDao(DocumentHolder holder)
        {
            if (holder == null)
                throw new ArgumentException("Specify document holder");

            if (BookDaoCache.TryGetValue(holder, out XmlBookDao bookDao))
                return bookDao;
            
            bookDao = new XmlBookDao(holder);
            BookDaoCache.Add(holder, bookDao);

            return bookDao;
        }

        private static XmlAuthorDao AuthorDao(DocumentHolder holder)
        {
            if (holder == null)
                throw new ArgumentException("Specify document holder");

            if (AuthorDaoCache.TryGetValue(holder, out var authorDao))
                return authorDao;
            
            authorDao = new XmlAuthorDao(holder);
            AuthorDaoCache.Add(holder, authorDao);

            return authorDao;
        }

        
        /*
         * DocumentHolder holds such information as path to
         * document with data, meta information for that document etc.
         * 
         * forBooks is a document holder for books
         * forAuthors is a document holder for authors
         *
         * returns configured BookDao instance that contains AuthorDao
         * for the specified path of the document
         */
        public static XmlBookDao BookDao(DocumentHolder forBooks, DocumentHolder forAuthors)
        {
            var bookDao = BookDao(forBooks);
            var authorDao = AuthorDao(forAuthors);

            bookDao.AuthorDao = authorDao;
            authorDao.BookDao = bookDao;

            bookDao.Parser = new BookProxyNodeParser<Book>() {
                AuthorDao = authorDao
            };

            authorDao.Parser = new AuthorProxyNodeParser<Author>() {
                BookDao = bookDao
            };

            var nodeHandler = NodeHandler.GetNodeHandlerFor(forBooks, forAuthors);

            authorDao.NodeHandler = nodeHandler;
            bookDao.NodeHandler = nodeHandler;
            
            return bookDao;
        }

        public static XmlAuthorDao AuthorDao(DocumentHolder forAuthors, DocumentHolder forBooks)
        {
            var authorDao = AuthorDao(forAuthors);
            var bookDao = BookDao(forBooks);

            authorDao.BookDao = bookDao;
            bookDao.AuthorDao = authorDao;

            authorDao.Parser = new AuthorProxyNodeParser<Author>() {
                BookDao = bookDao
            };

            bookDao.Parser = new BookProxyNodeParser<Book>() {
                AuthorDao = authorDao
            };
            
            var nodeHandler = NodeHandler.GetNodeHandlerFor(forBooks, forAuthors);

            authorDao.NodeHandler = nodeHandler;
            bookDao.NodeHandler = nodeHandler;

            return authorDao;
        }
        
        
    }
}