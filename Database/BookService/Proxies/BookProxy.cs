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
		private bool idIsSet = false;
		private bool authorsAreFetched = false;
		private bool authorsAreSet = false;

		private uint id;

		public BookProxy(Book book)
		{
			Id			= book.Id;
			Title		= book.Title;
			Rating		= book.Rating;
			Description = book.Description;
		}

		public BookProxy() { }

		// Make id persistent after loading entity from database
		// It means that proxy's id is immutable during using it
		// but we can change id of underlying book entity and it will
		// not affect the proxy anyway. So we are able to correctly save
		// and update entity without anomalies and side effects within 
		// our database
		public override uint Id
		{
			get {
				return id;
			}
			set {
				if (!idIsSet) {
					idIsSet = true;
					base.Id = value;
					id = value;
				}
			}
		}

		public override ISet<Author> Authors
		{
			get {
				if(!authorsAreFetched || authorsAreSet) {
					authorsAreFetched = true;
					authorsAreSet = false;
					base.Authors = new HashSet<Author>(AuthorDao.Instance.FindByBook(this));
				}

				return base.Authors;
			} set {
				authorsAreSet = true;
				base.Authors = Authors;
			}
			
		}

		public bool IdIsSet { get; private set; }

		public bool AuthorsAreFetched { get; private set; }

		public override bool Equals(object obj)
		{
			if (obj is BookProxy) {
				BookProxy t = (BookProxy)obj;
				return Id == t.Id;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

	}
}
