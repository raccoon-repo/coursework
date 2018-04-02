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
		private IList<Book> books = new List<Book>();

		private bool booksAreSet = false;
		private bool idIsSet = false;

		public uint Id {
			get {
				return id;
			} set {
				if (idIsSet) {
					throw new InvalidOperationException("Id can be set only once");
				} else {
					idIsSet = true;
					id = value;
				}
			}
		}
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public IList<Book> Books
		{
			get {
				return books;
			}
			private set { }
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
