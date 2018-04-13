using Database.BookService.DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService.Proxies
{
	public class BookProxy : Book
	{
		private bool authorsAreFetched = false;

		public BookProxy(Book book)
		{
			Id			= book.Id;
			Title		= book.Title;
			Rating		= book.Rating;
			Section		= book.Section;
			Description = book.Description;
		}

		public BookProxy() { }


		public override ISet<Author> Authors
		{
			get {
				if(!authorsAreFetched) {
					authorsAreFetched = true;

					var authors = new HashSet<Author>(
						AuthorDao.Instance.FindByBook(this));
	
					base.Authors = new ProxiedAuthorSet<Author>(authors, this);
				}

				return base.Authors;
			}
			set { base.Authors = value; }
			
		}


		public bool AuthorsAreFetched { get; private set; }

		public override bool Equals(object obj)
		{
			if (obj is Book book) {
				return Id == book.Id;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

	}
}
