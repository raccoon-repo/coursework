using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
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
        public void Should_Find_Book_By_Author()
        {
            var booksPath = _projectDir + "/Resources/FindTest/book/books.xml";
            var authorsPath = _projectDir + "/Resources/FindTest/author/authors.xml";

            var dhb = new DocumentHolder(booksPath);
            var dha = new DocumentHolder(authorsPath);

            var bookDao = DaoFactory.GetBookDaoFor(dhb, dha);
            
            var author = new Author() {
                Id = 1,
                FirstName = "Jack",
                LastName = "Spenser"
            };

            var books = bookDao.FindByAuthor(author);
            
            Assert.AreEqual(2, books.Count);
        }

        [Test]
        public void Should_Update_Book()
        {
            var path = _projectDir + "/Resources/UpdateTest/books.xml";
            var bookDao = DaoFactory.GetBookDaoFor(path);

            var book1 = bookDao.FindById(1);
            var temp = book1.Title;
            
            book1.Title = "New Title";
            bookDao.Update(book1);

            var book2 = bookDao.FindById(1);
            
            Assert.AreEqual(book1.Title, book2.Title);

            book2.Title = temp;
            bookDao.Update(book2);

            book1 = bookDao.FindById(1);
            
            Assert.AreEqual(book1.Title, book2.Title);
        }

        [Test]
        public void Should_Delete_Book()
        {
            var booksPath = _projectDir + "/Resources/DeleteTest/book/books.xml";
            var bookMetaPath = _projectDir + "/Resources/DeleteTest/book/meta-inf.xml";

            var authorsPath = _projectDir + "/Resources/DeleteTest/author/authors.xml";
            var authorMetaPath = _projectDir + "/Resources/DeleteTest/author/meta-inf.xml";
            
            var dhb = new DocumentHolder(booksPath, bookMetaPath);
            var dha = new DocumentHolder(authorsPath, authorMetaPath);

            var bookDao = DaoFactory.GetBookDaoFor(dhb, dha);
            var authorDao = DaoFactory.GetAuthorDaoFor(dha, dhb);


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

            var bookDao = DaoFactory.GetBookDaoFor(dhb, dha);

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
        public void Should_Find_Book_With_Id()
        {
            var path = _projectDir + "/Resources/sample.xml";
            var bookDao = DaoFactory.GetBookDaoFor(path);

            var book = bookDao.FindById(1);
            
            Assert.NotNull(book);

            Assert.AreEqual("Test Book", book.Title);
            Assert.AreEqual("SCIENCE", book.Section.ToString());
            Assert.AreEqual(7.8f, book.Rating);
            Assert.IsNull(book.Description);

            book = bookDao.FindById(2);
            
            Assert.NotNull(book);
            
            Assert.AreEqual("Test Book 2", book.Title);
            Assert.AreEqual("PROGRAMMING", book.Section.ToString());
            Assert.AreEqual(7.5f, book.Rating);
            Assert.AreEqual("Book about programming", book.Description);
        }

        [Test]
        public void Should_Find_By_Rating()
        {
            var path = _projectDir + "/Resources/books.xml";
            var bookDao = DaoFactory.GetBookDaoFor(path);

            var books = bookDao.FindByRating(7.2f);
            
            Assert.NotNull(books);
            Assert.AreEqual(1, books.Count);
        }

        [Test]
        public void Should_Find_By_Rating_In_Range()
        {
            var path = _projectDir + "/Resources/books.xml";
            var bookDao = DaoFactory.GetBookDaoFor(path);

            var books = bookDao.FindByRating(8.0f, 9.0f);
            
            Assert.NotNull(books);
            Assert.AreEqual(5, books.Count);
        }
        
        [Test]
        public void Should_Find_All_Books()
        {
            var path = _projectDir + "/Resources/sample.xml";
            var bookDao = DaoFactory.GetBookDaoFor(path);

            var books = bookDao.FindAll();

            Assert.NotNull(books);
            Assert.AreEqual(3, books.Count);    
        }

        [Test]
        public void Check_Factory_Methods()
        {
            var path = _projectDir + "/Resources";

            var dhb = new DocumentHolder(path + "/books.xml");
            var dha = new DocumentHolder(path + "/authors.xml");
            
            var bookDao = DaoFactory.GetBookDaoFor(dhb, dha);
            var authorDao = DaoFactory.GetAuthorDaoFor(dha, dhb);

            Assert.NotNull(bookDao.AuthorDao);
            Assert.NotNull(authorDao.BookDao);
        }

        [Test]
        public void Should_Find_Books_By_Section()
        {
            var path = _projectDir + "/Resources/sample.xml";
            var bookDao = DaoFactory.GetBookDaoFor(path);

            var books = bookDao.FindBySection(BookSection.SCIENCE);
            
            Assert.NotNull(books);
            Assert.AreEqual(2, books.Count);

            var book = books[0];
            
            Assert.AreEqual("Test Book", book.Title);
            Assert.AreEqual("SCIENCE", book.Section.ToString());
            Assert.AreEqual(7.8f, book.Rating);
            Assert.IsNull(book.Description);

            book = books[1];
            
            Assert.AreEqual("Test Book 3", book.Title);
            Assert.AreEqual("SCIENCE", book.Section.ToString());
            Assert.AreEqual(7.2f, book.Rating);
            Assert.IsNull(book.Description);
        }


        [Test]
        public void Should_Save_Book_Without_Authors()
        {
            var booksXml = _projectDir + "/Resources/SaveTest/book/books.xml";
            var booksMetaInf = _projectDir + "/Resources/SaveTest/book/books-meta-inf.xml";

            var authorsXml = _projectDir + "/Resources/SaveTest/author/authors.xml";
            var authorsMetaInf = _projectDir + "/Resources/SaveTest/author/authors-meta-inf.xml";
            
            var bookDh = new DocumentHolder(booksXml, booksMetaInf);
            var authorDh = new DocumentHolder(authorsXml, authorsMetaInf);

            IBookDao bookDao = DaoFactory.GetBookDaoFor(bookDh, authorDh);

            var book = new Book() {
                Id = 22,
                Title = "Help me please",
                Description = "aaaaa",
                Rating = 10.0f
            };
            
            bookDao.Save(book, SaveOption.SAVE_ONLY);

            var book1 = bookDao.FindById(1);
            
            Assert.AreEqual(book.Title, book1.Title);
            Assert.AreEqual(book.Description, book1.Description);
            Assert.AreEqual(book.Rating, book1.Rating);
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
    }
}