using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using BookService.DatabaseAccess;
using BookService.DatabaseAccess.Impl;
using BookService.Entities;
using BookService.Entities.Proxies;

namespace DaoTest
{
	[TestClass]
	public class ProxyTest
	{

		private static IBookDao bookDao = BookDao.Instance;
		private static IAuthorDao authorDao = AuthorDao.Instance;

		[TestMethod]
		public void Test_Lazy_Initialization()
		{
			DBWorker.SetConfigurationFor("test");

			Book book = bookDao.FindById(1);
			Author author = authorDao.FindById(1);

			// check if both obtained instances are proxies

			Assert.IsTrue(book is BookProxy);
			Assert.IsTrue(author is AuthorProxy);

			// check if both proxies have uninitialized collections

			BookProxy bookProxy = (BookProxy)book;
			AuthorProxy authorProxy = (AuthorProxy)author;

			Assert.IsFalse(bookProxy.AuthorsAreFetchedOrSet);
			Assert.IsFalse(authorProxy.BooksAreFetchedOrSet);

			// initialize collection of authors

			Console.WriteLine();

			foreach (Author a in book.Authors) {
				Console.WriteLine($"First Name: {a.FirstName}, LastName: {a.LastName}");
			}

			Console.WriteLine();

			foreach(Book b in author.Books) {
				Console.WriteLine($"Id: {b.Id}, Title: {b.Title}");
			}

			Console.WriteLine();

			// check if object inside collection is also proxy

			var authors = new List<Author>(book.Authors);
			var books = new List<Book>(author.Books);

			Author a1 = authors[1];
			Book b1 = books[0];

			Assert.IsTrue(a1 is AuthorProxy);
			Assert.IsTrue(b1 is BookProxy);

			authorProxy = (AuthorProxy)a1;
			bookProxy = (BookProxy)b1;

			Assert.IsFalse(authorProxy.BooksAreFetchedOrSet);
			Assert.IsFalse(bookProxy.AuthorsAreFetchedOrSet);
		}

		[TestMethod]
		public void Test_Proxys_Modifications()
		{

		}
	}
}
