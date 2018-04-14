using System.Collections.Generic;
using BookService.DatabaseAccess.Impl;
namespace BookService.Entities.Proxies
{
	public class AuthorProxy : Author
	{
		private bool booksAreFetchedOrSet = false;

		public AuthorProxy(Author author)
		{
			FirstName = author.FirstName;
			LastName = author.LastName;

			if(author is AuthorProxy proxy && proxy.booksAreFetchedOrSet) {
				base.Books = proxy.Books;
				booksAreFetchedOrSet = true;
			} else if (!(author is AuthorProxy) && author.Books.Count != 0) {
				base.Books = author.Books;
				booksAreFetchedOrSet = true;
			}
		}

		public AuthorProxy() { }

		public bool BooksAreFetchedOrSet {
			get { return booksAreFetchedOrSet; }
			private set { }
		}

		public override ISet<Book> Books {
			get {
				if(!booksAreFetchedOrSet) {
					FetchBooks();
				}

				return base.Books;
			}
			set {
				base.Books = Books;
			}
		}

		private void FetchBooks()
		{
			booksAreFetchedOrSet = true;
			base.Books = new HashSet<Book>(
				BookDao.Instance.FindByAuthor(this));
		}

		public override void AddBook(Book book)
		{
			if(!booksAreFetchedOrSet) {
				FetchBooks();
			}
			base.AddBook(book);
		}

		public override void RemoveBook(Book book)
		{
			if(BooksAreFetchedOrSet) {
				FetchBooks();
			}
			base.RemoveBook(book);
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
