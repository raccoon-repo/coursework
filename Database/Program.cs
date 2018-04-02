using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using Database.BookService.DatabaseAccess;
using Database.BookService.Queries;
using Database.BookService;

namespace Database
{
	class Program
	{
		static void Main(string[] args)
		{
			IBookDao bookDao = BookDao.Instance;
			IList<Book> books = bookDao.FindAll();

			Book book = books[0];
			Console.WriteLine(book.Id);

			book.Section = Book.section.ART;

			bookDao.Save(book);

			Console.ReadLine();
		}
	}
}
