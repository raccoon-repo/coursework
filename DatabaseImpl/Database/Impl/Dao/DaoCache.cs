using System.Collections.Concurrent;
using BookLibrary.Core.Dao;

namespace BookLibrary.Database.Impl.Dao
{
    public class DaoCache
    {
        private static ConcurrentDictionary<string, IAuthorDao> _authorDaoCache = 
            new ConcurrentDictionary<string, IAuthorDao>();

        private static ConcurrentDictionary<string, IBookDao> _bookDaoCache = 
            new ConcurrentDictionary<string, IBookDao>();

        public static IAuthorDao GetAuthorDao(string connectionString)
        {
            
            if (_authorDaoCache.TryGetValue(connectionString, out var authorDao))
            {
                return authorDao;
            }
            
            DBWorker dbWorker = new DBWorker(connectionString);
            
            var bookDao = new BookDao() {
                DBWorker = dbWorker
            };
            
            authorDao = new AuthorDao() {
                DBWorker = dbWorker,
                BookDao = bookDao
            };

            bookDao.AuthorDao = authorDao;
            
            _authorDaoCache.TryAdd(connectionString, authorDao);
            _bookDaoCache.TryAdd(connectionString, bookDao);
            return authorDao;
        }

        public static IBookDao GetBookDao(string connectionString)
        {
            IBookDao bookDao;

            if (_bookDaoCache.TryGetValue(connectionString, out bookDao))
            {
                return bookDao;
            }
            
            DBWorker dbWorker = new DBWorker(connectionString);
            var authorDao = new AuthorDao() {
                DBWorker = dbWorker,
            };

            bookDao = new BookDao() {
                DBWorker = dbWorker,
                AuthorDao = authorDao
            };

            authorDao.BookDao = bookDao;
            
            _bookDaoCache.TryAdd(connectionString, bookDao);
            _authorDaoCache.TryAdd(connectionString, authorDao);
            
            return bookDao;
        }
    }
}