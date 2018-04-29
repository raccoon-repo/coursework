using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookLibrary.Entities;
using BookLibrary.Database.Impl.Dao;
using BookLibrary.Core.Dao;
using BookLibrary.Entities.Proxies;
using System.Collections.Generic;
using BookLibrary.Database;

namespace DaoTest
{
    [TestClass]
    public class DaoTest
    {
        public static DBWorker dBWorker = new DBWorker(DBWorker.TEST_CON_STRING);
        public static IBookDao bookDao = new BookDao(dBWorker);
        public static IAuthorDao authorDao = new AuthorDao(dBWorker);

		[TestMethod]
		public void Test_BookDao ()
		{
			Init_Data();
			Stopwatch nonCachedInstanceFetch = new Stopwatch();
			Stopwatch cachedInstanceFetch = new Stopwatch();

			// Test cache efficiency
			nonCachedInstanceFetch.Start();
			Book nonCachedBookInstance = bookDao.FindById(1);
			nonCachedInstanceFetch.Stop();

			cachedInstanceFetch.Start();
			Book cachedBookInstance = bookDao.FindById(1);
			cachedInstanceFetch.Stop();

			Assert.IsTrue(cachedInstanceFetch.ElapsedTicks < nonCachedInstanceFetch.ElapsedTicks);
			/* ***************************** */


			// Check if returned by findById() method instances
			// are BookProxy' instance
			Assert.IsTrue(cachedBookInstance is BookProxy);
			Assert.IsTrue(nonCachedBookInstance is BookProxy);

			// *find* methods tests
			IList<Book> allBooks = bookDao.FindAll();
			IList<Book> foundByTitle = bookDao.FindByTitle("Fast Recipes");
			IList<Book> foundByTitle2 = bookDao.FindByTitle("Fake Your Death");
			IList<Book> foundByTitle3 = bookDao.FindByTitle("True");
			IList<Book> foundByRating = bookDao.FindByRating(7.8f);
			IList<Book> foundBySection = bookDao.FindBySection(BookSection.FICTION);

			Assert.AreEqual(1, foundByTitle.Count);
			Assert.AreEqual(1, foundByTitle2.Count);
			Assert.AreEqual(2, foundByTitle3.Count);
			Assert.AreEqual(3, foundByRating.Count);
			Assert.AreEqual(5, foundBySection.Count);

			Book temp = bookDao.FindById(1);
			string tempTitle = temp.Title;

			// test refresh method

			temp.Title = "New Title";
			bookDao.Refresh(temp);
			Assert.AreEqual(tempTitle, temp.Title);

			// test update method

			temp.Title = "New Title";
			bookDao.Update(temp);

			Book temp1 = bookDao.FindById(temp.Id);
			Assert.AreEqual(temp1.Title, temp.Title);

			// test save method again

			temp.Title = "Title*";
			bookDao.Save(temp, SaveOption.UPDATE_IF_EXIST);

			temp1 = bookDao.FindById(temp.Id);
			Assert.AreEqual(temp1.Title, temp.Title);
		}

		public void Init_Data()
		{
            ( (BookDao)bookDao ).AuthorDao = authorDao;
            ( (AuthorDao)authorDao ).BookDao = bookDao;


			Book b1 = new Book() {
				Title = "Fast Recipes",
				Rating = 7.8f,
				Section = BookSection.COOKERY,
			};

			Book b2 = new Book() {
				Title = "Ten Ways To Fake Your Death",
				Rating = 8.8f,
				Section = BookSection.FICTION,
			};

			Book b3 = new Book() {
				Title = "Need Help?",
				Rating = 7.8f,
				Section = BookSection.FICTION,
			};

			Book b4 = new Book() {
				Title = "Billy Milligan",
				Rating = 9.1f,
				Section = BookSection.DOCUMENTARY,
			};

			Book b5 = new Book() {
				Title = "True Detective",
				Rating = 10.0f,
				Section = BookSection.FICTION,
			};

			Book b6 = new Book() {
				Title = "Hold On",
				Rating = 7.8f,
				Section = BookSection.ART,
			};

			Book b7 = new Book() {
				Title = "The Art Of Programming",
				Rating = 9.0f,
				Section = BookSection.PROGRAMMING,
			};

			Book b8 = new Book() {
				Title = "English",
				Rating = 8.5f,
				Section = BookSection.FOREIGN_LANGUAGES,
			};

			Book b9 = new Book() {
				Title = "Harry Potter",
				Rating = 9.2f,
				Section = BookSection.FICTION,
			};

			Book b10 = new Book() {
				Title = "Shine",
				Rating = 9.1f,
				Section = BookSection.FICTION,
			};

			Book b11 = new Book() {
				Title = "Not True Detective",
				Rating = 9.5f,
				Section = BookSection.DOCUMENTARY
			};

			Author a1 = new Author() {
				FirstName = "Nicolas",
				LastName = "Claus"
			};

			Author a2 = new Author() {
				FirstName = "Stephen",
				LastName = "Nye"
			};

			Author a3 = new Author() {
				FirstName = "Valerian",
				LastName = "Hollo"
			};

			Author a4 = new Author() {
				FirstName = "Roman",
				LastName = "Sonniy"
			};

			Author a5 = new Author() {
				FirstName = "Eugene",
				LastName = "Colbert"
			};

			Author a6 = new Author() {
				FirstName = "Haruko",
				LastName = "Kodjima"
			};

			b1.AddAuthor(a1);
			b1.AddAuthor(a2);

			b2.AddAuthor(a3);

			b3.AddAuthor(a4);

			b5.AddAuthor(a5);
			b5.AddAuthor(a1);

			b6.AddAuthor(a1);

			b7.AddAuthor(a2);
			b7.AddAuthor(a6);

			b8.AddAuthor(a4);

			b9.AddAuthor(a2);

			b10.AddAuthor(a3);
			b10.AddAuthor(a4);
			b10.AddAuthor(a5);

			b11.AddAuthor(a5);
			b11.AddAuthor(a1);

			bookDao.Save(b1, SaveOption.UPDATE_IF_EXIST);
			bookDao.Save(b2, SaveOption.UPDATE_IF_EXIST);
			bookDao.Save(b3, SaveOption.UPDATE_IF_EXIST);
			bookDao.Save(b4, SaveOption.UPDATE_IF_EXIST);
			bookDao.Save(b5, SaveOption.UPDATE_IF_EXIST);
			bookDao.Save(b6, SaveOption.UPDATE_IF_EXIST);
			bookDao.Save(b7, SaveOption.UPDATE_IF_EXIST);
			bookDao.Save(b8, SaveOption.UPDATE_IF_EXIST);
			bookDao.Save(b9, SaveOption.UPDATE_IF_EXIST);
			bookDao.Save(b10, SaveOption.UPDATE_IF_EXIST);
			bookDao.Save(b11, SaveOption.UPDATE_IF_EXIST);
		}
	}
}
