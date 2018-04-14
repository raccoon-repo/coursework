using BookService.DatabaseAccess.Impl;
using System.Collections.Generic;

namespace BookService.Entities.Proxies
{
	public class BookProxy : Book
	{
		private bool authorsAreFetchedOrSet = false;

		public BookProxy(Book book)
		{
			Id			= book.Id;
			Title		= book.Title;
			Rating		= book.Rating;
			Section		= book.Section;
			Description = book.Description;

			if(book is BookProxy proxy && proxy.authorsAreFetchedOrSet) {
				base.Authors = proxy.Authors;
				authorsAreFetchedOrSet = true;
			} else if (!(book is BookProxy) && book.Authors.Count != 0) {
				base.Authors = book.Authors;
				authorsAreFetchedOrSet = true;
			}
		}

		public BookProxy() { }

		public override ISet<Author> Authors
		{
			get {
				if(!authorsAreFetchedOrSet) {
					FetchAuthors();
				}

				return base.Authors;
			}
			set { base.Authors = value; }
			
		}

		public void FetchAuthors()
		{
			authorsAreFetchedOrSet = true;
			base.Authors = new HashSet<Author>(
				AuthorDao.Instance.FindByBook(this));

		}

		public bool AuthorsAreFetchedOrSet {
			get { return authorsAreFetchedOrSet; }
			private set { }
		}

		public override bool Equals(object obj)
		{
			if (obj is Book book) {
				return Id == book.Id;
			}

			return false;
		}

		public override void AddAuthor(Author author)
		{
			if(!AuthorsAreFetchedOrSet) {
				FetchAuthors();
			}
			base.AddAuthor(author);
		}
		
		public override void RemoveAuthor(Author author)
		{
			if(!AuthorsAreFetchedOrSet) {
				FetchAuthors();
			}
			base.RemoveAuthor(author);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

	}
}
