using System;
using System.Collections.Concurrent;

namespace BookLibrary.Xml.Impl.Dao
{
    public class DaoFactory
    {
        private static readonly ConcurrentDictionary<string, XmlBookDao>
            BookDaoCache = new ConcurrentDictionary<string, XmlBookDao>();
        
        private static readonly ConcurrentDictionary<string, XmlAuthorDao>
            AuthorDaoCache = new ConcurrentDictionary<string, XmlAuthorDao>();

        
        /*
         * Retrieve cached dao that corresponds to specified path
         * path is the path to xml document with data
         */
        public static XmlBookDao GetBookDaoFor(string path)
        {
            if (path is null)
                throw new ArgumentException("Specify the path to a document");

            if (BookDaoCache.TryGetValue(path, out var bookDao))
                return bookDao;

            var documentHolder = new DocumentHolder(path);
            bookDao = new XmlBookDao(documentHolder);
            BookDaoCache.TryAdd(path, bookDao);
            
            return bookDao;
        }

        public static XmlAuthorDao GetAuthorDaoFor(string path)
        {
            if (path is null)
                throw new ArgumentException("Specify the path to a document");

            if (AuthorDaoCache.TryGetValue(path, out var authorDao))
                return authorDao;
            
            var documentHolder = new DocumentHolder(path);
            authorDao = new XmlAuthorDao(documentHolder);
            AuthorDaoCache.TryAdd(path, authorDao);

            return authorDao;
        }

        
        /*
         * pathForBooks is a path to xml document with books
         * pathForAuthors is a path to xml document with authors
         *
         * returns configured BookDao instance that contains AuthorDao
         * for the specified path of the document
         */
        public static XmlBookDao GetBookDaoFor(string pathForBooks, string pathForAuthors)
        {
            var bookDao = GetBookDaoFor(pathForAuthors);
            var authorDao = GetAuthorDaoFor(pathForAuthors);

            bookDao.AuthorDao = authorDao;

            return bookDao;
        }

        public static XmlAuthorDao GetAuthorDaoFor(string pathForAuthors, string pathForBooks)
        {
            var authorDao = GetAuthorDaoFor(pathForAuthors);
            var bookDao = GetBookDaoFor(pathForBooks);

            authorDao.BookDao = bookDao;

            return authorDao;
        }
    }
}