using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService
{
	public class Author
	{
		private uint id = 0;
		private string firstName;
		private string lastName;
		private ISet<Book> books;

		private bool booksAreFetched = false;
		private bool idIsSet = false;

		public uint Id {
			get {
				return id;
			}
			set {
				id = value;
			}
		}

		public bool BooksAreFetched { get; private set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public ISet<Book> Books
		{
			get {
				return books;
			}
			set {
				books = value;
			}
		}

		public void AddBook(Book b)
		{
			books.Add(b);
		}

		public void RemoveBook(Book b)
		{
			books.Remove(b);
		}
	}
}
