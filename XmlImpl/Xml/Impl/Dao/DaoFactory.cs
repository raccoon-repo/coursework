using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace BookLibrary.Xml.Impl.Dao
{
    public class DaoFactory
    {

        private static readonly Dictionary<DocumentHolder, XmlBookDao>
            BookDaoCache = new Dictionary<DocumentHolder, XmlBookDao>();
        
        private static readonly Dictionary<DocumentHolder, XmlAuthorDao>
            AuthorDaoCache = new Dictionary<DocumentHolder, XmlAuthorDao>();
        
        /*
         * Retrieve cached dao that corresponds to specified path
         * path is the path to xml document with data
         */
        public static XmlBookDao GetBookDaoFor(string path)
        {
            if (path is null)
                throw new ArgumentException("Specify the path to a document");

            var documentHolder = new DocumentHolder(path);
            
            if (BookDaoCache.TryGetValue(documentHolder, out var bookDao))
                return bookDao;

            bookDao = new XmlBookDao(documentHolder);
            BookDaoCache.Add(documentHolder, bookDao);
            
            return bookDao;
        }

        public static XmlAuthorDao GetAuthorDaoFor(string path)
        {
            if (path is null)
                throw new ArgumentException("Specify the path to a document");
            var documentHolder = new DocumentHolder(path);

            if (AuthorDaoCache.TryGetValue(documentHolder, out var authorDao))
                return authorDao;

            authorDao = new XmlAuthorDao(documentHolder);
            AuthorDaoCache.Add(documentHolder, authorDao);

            return authorDao;
        }

        public static XmlBookDao GetBookDaoFor(DocumentHolder holder)
        {
            if (holder == null)
                throw new ArgumentException("Specify document holder");

            if (BookDaoCache.TryGetValue(holder, out XmlBookDao bookDao))
                return bookDao;
            
            bookDao = new XmlBookDao(holder);
            BookDaoCache.Add(holder, bookDao);

            return bookDao;
        }

        public static XmlAuthorDao GetAuthorDaoFor(DocumentHolder holder)
        {
            if (holder == null)
                throw new ArgumentException("Specify document holder");

            if (AuthorDaoCache.TryGetValue(holder, out XmlAuthorDao authorDao))
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
        public static XmlBookDao GetBookDaoFor(DocumentHolder forBooks, DocumentHolder forAuthors)
        {
            var bookDao = GetBookDaoFor(forBooks);
            var authorDao = GetAuthorDaoFor(forAuthors);

            bookDao.AuthorDao = authorDao;
            authorDao.BookDao = bookDao;

            return bookDao;
        }

        public static XmlAuthorDao GetAuthorDaoFor(DocumentHolder forAuthors, DocumentHolder forBooks)
        {
            var authorDao = GetAuthorDaoFor(forAuthors);
            var bookDao = GetBookDaoFor(forBooks);

            authorDao.BookDao = bookDao;
            bookDao.AuthorDao = authorDao;

            return authorDao;
        }
        
        
    }
}