using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using BookLibrary.Core.Dao;
using BookLibrary.Database;
using BookLibrary.Database.Impl.Dao;
using BookLibrary.Entities;
using BookLibrary.Entities.Proxies;
using NUnit.Framework;

namespace NUnit_TestDao.Tests
{
    [TestFixture]
    public class DaoTest
    {
	    private static readonly DBWorker DbWorker = new DBWorker(DBWorker.TEST_CON_STRING);
	    private static readonly IBookDao BookDao = new BookDao(DbWorker);
	    private static readonly IAuthorDao AuthorDao = new AuthorDao(DbWorker);

		[Test]
		public void Test_BookDao()
		{
			var nonCachedInstanceFetch = new Stopwatch();
			var cachedInstanceFetch = new Stopwatch();

			// Test cache efficiency
			nonCachedInstanceFetch.Start();
			Book nonCachedBookInstance = BookDao.FindById(1);
			nonCachedInstanceFetch.Stop();

			cachedInstanceFetch.Start();
			Book cachedBookInstance = BookDao.FindById(1);
			cachedInstanceFetch.Stop();

			Assert.IsTrue(cachedInstanceFetch.ElapsedTicks < nonCachedInstanceFetch.ElapsedTicks);
			/* ***************************** */


			// Check if returned by findById() method instances
			// are BookProxy' instance
			Assert.IsTrue(cachedBookInstance is BookProxy);
			Assert.IsTrue(nonCachedBookInstance is BookProxy);

			// *find* methods tests
			IList<Book> allBooks = BookDao.FindAll();
			IList<Book> foundByTitle = BookDao.FindByTitle("Fast Recipes");
			IList<Book> foundByTitle2 = BookDao.FindByTitle("Fake Your Death");
			IList<Book> foundByTitle3 = BookDao.FindByTitle("True");
			IList<Book> foundByRating = BookDao.FindByRating(7.8f);
			IList<Book> foundBySection = BookDao.FindBySection(BookSection.FICTION);

			Assert.AreEqual(1, foundByTitle.Count);
			Assert.AreEqual(1, foundByTitle2.Count);
			Assert.AreEqual(2, foundByTitle3.Count);
			Assert.AreEqual(3, foundByRating.Count);
			Assert.AreEqual(5, foundBySection.Count);

			Book temp = BookDao.FindById(1);
			string tempTitle = temp.Title;

			// test refresh method

			temp.Title = "New Title";
			BookDao.Refresh(temp);
			Assert.AreEqual(tempTitle, temp.Title);

			// test update method

			temp.Title = "New Title";
			BookDao.Update(temp);

			Book temp1 = BookDao.FindById(temp.Id);
			Assert.AreEqual(temp1.Title, temp.Title);

			// test save method again

			temp.Title = "Title*";
			BookDao.Save(temp, SaveOption.UPDATE_IF_EXIST);

			temp1 = BookDao.FindById(temp.Id);
			Assert.AreEqual(temp1.Title, temp.Title);
		}

	    [SetUp]
		public void Init_Data()
		{
			
			var b1 = new Book() {
				Title = "Fast Recipes",
				Rating = 7.8f,
				Section = BookSection.COOKERY,
			};

			var b2 = new Book() {
				Title = "Ten Ways To Fake Your Death",
				Rating = 8.8f,
				Section = BookSection.FICTION,
			};

			var b3 = new Book() {
				Title = "Need Help?",
				Rating = 7.8f,
				Section = BookSection.FICTION,
			};

			var b4 = new Book() {
				Title = "Billy Milligan",
				Rating = 9.1f,
				Section = BookSection.DOCUMENTARY,
			};

			var b5 = new Book() {
				Title = "True Detective",
				Rating = 10.0f,
				Section = BookSection.FICTION,
			};

			var b6 = new Book() {
				Title = "Hold On",
				Rating = 7.8f,
				Section = BookSection.ART,
			};

			var b7 = new Book() {
				Title = "The Art Of Programming",
				Rating = 9.0f,
				Section = BookSection.PROGRAMMING,
			};

			var b8 = new Book() {
				Title = "English",
				Rating = 8.5f,
				Section = BookSection.FOREIGN_LANGUAGES,
			};

			var b9 = new Book() {
				Title = "Harry Potter",
				Rating = 9.2f,
				Section = BookSection.FICTION,
			};

			var b10 = new Book() {
				Title = "Shine",
				Rating = 9.1f,
				Section = BookSection.FICTION,
			};

			var b11 = new Book() {
				Title = "Not True Detective",
				Rating = 9.5f,
				Section = BookSection.DOCUMENTARY
			};

			var a1 = new Author() {
				FirstName = "Nicolas",
				LastName = "Claus"
			};

			var a2 = new Author() {
				FirstName = "Stephen",
				LastName = "Nye"
			};

			var a3 = new Author() {
				FirstName = "Valerian",
				LastName = "Hollo"
			};

			var a4 = new Author() {
				FirstName = "Roman",
				LastName = "Sonniy"
			};

			var a5 = new Author() {
				FirstName = "Eugene",
				LastName = "Colbert"
			};

			var a6 = new Author() {
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

			BookDao.Save(b1, SaveOption.UPDATE_IF_EXIST);
			BookDao.Save(b2, SaveOption.UPDATE_IF_EXIST);
			BookDao.Save(b3, SaveOption.UPDATE_IF_EXIST);
			BookDao.Save(b4, SaveOption.UPDATE_IF_EXIST);
			BookDao.Save(b5, SaveOption.UPDATE_IF_EXIST);
			BookDao.Save(b6, SaveOption.UPDATE_IF_EXIST);
			BookDao.Save(b7, SaveOption.UPDATE_IF_EXIST);
			BookDao.Save(b8, SaveOption.UPDATE_IF_EXIST);
			BookDao.Save(b9, SaveOption.UPDATE_IF_EXIST);
			BookDao.Save(b10, SaveOption.UPDATE_IF_EXIST);
			BookDao.Save(b11, SaveOption.UPDATE_IF_EXIST);
		}
	}
}
