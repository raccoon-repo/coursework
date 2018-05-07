using BookLibrary.Core.Service;
using BookLibrary.Service;
using BookLibrary.Core.Dao;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using BookLibrary.Xml.Impl.Dao;
using BookLibrary.Xml.Utils;
using BookLibrary.Xml.Utils.Impl;

namespace Wpf.Appl
{
    public class ApplicationContext
    {
        private static string _projectDir;

        private static IBookService _bookService;
        private static IAuthorService _authorService;

        private static IBookCounter _bookCounter;

        private static IBookDao _bookDao;
        private static IAuthorDao _authorDao;

        public static IBookCounter BookCounter => _bookCounter;
        public static IBookService BookService => _bookService;
        public static IAuthorService AuthorService => _authorService;


        static ApplicationContext()
        {
            _projectDir = GetProjectDirectory();

            var booksPath = _projectDir + "\\Data\\book\\books.xml";
            var booksMetaInfPath = _projectDir + "\\Data\\book\\meta-inf.xml";
            var booksCountPath = _projectDir + "\\Data\\book\\count.xml";

            var authorsPath = _projectDir + "\\Data\\author\\authors.xml";
            var authorsMetaInfPath = _projectDir + "\\Data\\author\\meta-inf.xml";

            var authorDocHolder = new DocumentHolder(authorsPath, authorsMetaInfPath);
            var bookDocHolder = new DocumentHolder(booksPath, booksMetaInfPath);

            _bookDao = DaoFactory.BookDao(bookDocHolder, authorDocHolder);
            _authorDao = DaoFactory.AuthorDao(authorDocHolder, bookDocHolder);

            _bookService = new BookService() {
                BookDao = _bookDao
            };

            _authorService = new AuthorService() {
                AuthorDao = _authorDao
            };

            _bookCounter = new BookCounter(booksCountPath);
        }

        private static string GetProjectDirectory()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Regex regex = new Regex("\\\\bin\\\\Debug$");
            return regex.Replace(dir, "");
        }

    }
}
