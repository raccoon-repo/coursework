using Database.BookService.DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService.Proxies
{
	public class AuthorProxy : Author
	{
		private bool booksAreFetched = false;

		public AuthorProxy(Author author)
		{
			FirstName = author.FirstName;
			LastName = author.LastName;
		}

		public AuthorProxy() { }

		public bool BooksAreFetched {
			get { return BooksAreFetched; }
			private set { }
		}

		public override ISet<Book> Books {
			get {
				if(!booksAreFetched) {
					booksAreFetched = true;
					var books = new HashSet<Book>(
						BookDao.Instance.FindByAuthor(this));
					base.Books = new ProxiedBookSet<Book>(books, this);
				}

				return base.Books;
			}
			set {
				base.Books = Books;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Author author) { 
				return this.Id == author.Id;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
