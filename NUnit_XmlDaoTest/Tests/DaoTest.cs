using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading;
using BookLibrary.Core.Dao;
using BookLibrary.Entities;
using NUnit.Framework;

using BookLibrary.Xml.Impl.Dao;
using NUnit.Framework.Constraints;

namespace NUnit_XmlDaoTest.Tests
{
    [TestFixture]
    public class DaoTest
    {
        private string _projectDir;

        [Test]
        public void Should_Delete_Book()
        {
            var booksPath = _projectDir + "/Resources/DeleteTest/book/books.xml";
            var bookMetaPath = _projectDir + "/Resources/DeleteTest/book/meta-inf.xml";

            var authorsPath = _projectDir + "/Resources/DeleteTest/author/authors.xml";
            var authorMetaPath = _projectDir + "/Resources/DeleteTest/author/meta-inf.xml";
            
            var dhb = new DocumentHolder(booksPath, bookMetaPath);
            var dha = new DocumentHolder(authorsPath, authorMetaPath);

            var bookDao = DaoFactory.BookDao(dhb, dha);
            var authorDao = DaoFactory.AuthorDao(dha, dhb);

            var book1 = bookDao.FindById(1);
            bookDao.Delete(book1);

            var book1_1 = bookDao.FindById(1);
            Assert.IsNull(book1_1);

            var a1 = authorDao.FindById(1);
            var a3 = authorDao.FindById(3);
            
            Assert.AreEqual(1, a1.Books.Count);
            Assert.AreEqual(1, a3.Books.Count);
            
        }

        [Test]
        public void Should_Save_Book()
        {
            var booksPath = _projectDir + "/Resources/SaveTest/Full/book/books.xml";
            var bookMetaPath = _projectDir + "/Resources/SaveTest/Full/book/meta-inf.xml";

            var authorsPath = _projectDir + "/Resources/SaveTest/Full/author/authors.xml";
            var authorMetaPath = _projectDir + "/Resources/SaveTest/Full/author/meta-inf.xml";
            
            var dhb = new DocumentHolder(booksPath, bookMetaPath);
            var dha = new DocumentHolder(authorsPath, authorMetaPath);

            var bookDao = DaoFactory.BookDao(dhb, dha);

            var b1 = new Book() {
                Title = "b1", 
                Description = "Test"
            };

            var b2 = new Book() {
                Title = "b2",
                Rating = 7.9f,
                Description = "Test Book"
            };

            var b3 = new Book() {
                Title = "b3",
                Rating = 9.5f,
                Description = "Book for test purpose"
            };

            var b4 = new Book() {
                Title = "b4"
            };

            var b5 = new Book() {
                Title = "b5"
            };

            var a1 = new Author() {
                FirstName = "a1",
                LastName = "Smith"
            };

            var a2 = new Author() {
                FirstName = "a2",
                LastName = "Miller"
            };

            var a3 = new Author() {
                FirstName = "a3",
                LastName = "Cat"
            };
            
            b1.AddAuthor(a1);
            
            b2.AddAuthor(a1);
            b2.AddAuthor(a2);
            
            b3.AddAuthor(a2);
            
            b4.AddAuthor(a1);
            
            b5.AddAuthor(a2);
            b5.AddAuthor(a3);
            
            bookDao.Save(b1, SaveOption.UPDATE_IF_EXIST);

        }

        [Test]
        public void Should_Remove_Author_Then_Add_Author()
        {
            var booksPath = _projectDir + "/Resources/SaveTest/SaveUpdate/book/books.xml";
            var bookMetaPath = _projectDir + "/Resources/SaveTest/SaveUpdate/book/meta-inf.xml";

            var authorsPath = _projectDir + "/Resources/SaveTest/SaveUpdate/author/authors.xml";
            var authorMetaPath = _projectDir + "/Resources/SaveTest/SaveUpdate/author/meta-inf.xml";
            
            var dhb = new DocumentHolder(booksPath, bookMetaPath);
            var dha = new DocumentHolder(authorsPath, authorMetaPath);

            var bookDao = DaoFactory.BookDao(dhb, dha);
            var authorDao = DaoFactory.AuthorDao(dha, dhb);
            
            var book = bookDao.FindById(1);
            var author = authorDao.FindById(1);
            
            // TEST
            book.RemoveAuthor(author);
            
            bookDao.Save(book, SaveOption.UPDATE_IF_EXIST);

            author = authorDao.FindById(1);
            
            Assert.AreEqual(2, author.Books.Count);
            book = bookDao.FindById(1);
            Assert.AreEqual(0, book.Authors.Count);
            
            book.AddAuthor(author);
            bookDao.Save(book, SaveOption.UPDATE_IF_EXIST);

            book = bookDao.FindById(1);
            
            Assert.AreEqual(1, book.Authors.Count);

            author = authorDao.FindById(1);
            
            Assert.AreEqual(3, author.Books.Count);

        }

        [Test]
        public void Should_Find_Book_With_Id()
        {
            var bookDao = BookDaoForSearch();

            var book = bookDao.FindById(1);
            
            Assert.AreEqual("Test 1", book.Title);
            Assert.AreEqual("SCIENCE", book.Section.ToString());
            Assert.AreEqual("TEST 1", book.Description);
            Assert.AreEqual(7.2f, book.Rating);
            Assert.AreEqual(2, book.Authors.Count);
        }

        [Test]
        public void Should_Find_All_Books()
        {
            var bookDao = BookDaoForSearch();

            var books = bookDao.FindAll();
            
            Assert.AreEqual(7, books.Count);
        }
        
        [Test]
        public void Should_Find_Book_By_Author()
        {
            var bookDao = BookDaoForSearch();
            
            var author = new Author() {
                Id = 1
            };

            var books = bookDao.FindByAuthor(author);
            Assert.AreEqual(2, books.Count);

        }

        [Test]
        public void Should_Find_By_Rating()
        {
            var bookDao = BookDaoForSearch();

            var books = bookDao.FindByRating(7.2f);
            
            Assert.AreEqual(1, books.Count);

            books = bookDao.FindByRating(8.0f, 9.0f);
            
            Assert.AreEqual(5, books.Count);
        }
        

        [Test]
        public void Should_Find_Books_By_Section()
        {
            var bookDao = BookDaoForSearch();

            var books = bookDao.FindBySection(BookSection.SCIENCE);
            
            Assert.AreEqual(2, books.Count);

            books = bookDao.FindBySection(BookSection.HOBBY);
            
            Assert.AreEqual(1, books.Count);
        }

        [SetUp]
        public void Init()
        {
            _projectDir = GetProjectDirectory();
        }

        private static string GetProjectDirectory()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Regex regex = new Regex("/bin/Debug$");
            return regex.Replace(dir, "");
        }

        private XmlBookDao BookDaoForSearch()
        {
            var booksPath = _projectDir + "/Resources/SearchTest/book/books.xml";
            var authorsPath = _projectDir + "/Resources/SearchTest/author/authors.xml";

            var dhb = new DocumentHolder(booksPath);
            var dha = new DocumentHolder(authorsPath);

            var bookDao = DaoFactory.BookDao(dhb, dha);

            return bookDao;
        }
    }
}