using System.Collections.Generic;
using BookLibrary.Core.Dao;

namespace BookLibrary.Entities.Proxies
{
	public class AuthorProxy : Author
	{
		private bool _booksAreFetchedOrSet = false;
        private IBookDao _bookDao;

		private string _oldFirstName;
		private string _oldLastName;

		public AuthorProxy(Author author)
		{
			FirstName = author.FirstName;
			LastName = author.LastName;

			_oldFirstName = FirstName;
			_oldLastName = LastName;

			if(author is AuthorProxy proxy && proxy._booksAreFetchedOrSet)
            {
				base.Books = proxy.Books;
				_booksAreFetchedOrSet = true;
			}
            else if (!(author is AuthorProxy) && author.Books.Count != 0)
            {
				base.Books = author.Books;
				_booksAreFetchedOrSet = true;
			}
		}

		public AuthorProxy(int id, string firstName, string lastname)
			: base(id, firstName, lastname)
		{
			_oldFirstName = firstName;
			_oldLastName = lastname;
		}

        public AuthorProxy() { }

        public IBookDao BookDao
        {
            get => _bookDao;
	        set => _bookDao = value;
        }

		public bool BooksAreFetchedOrSet => _booksAreFetchedOrSet;

		public override ISet<Book> Books {
			get {
				if(!_booksAreFetchedOrSet) 
				{
					FetchBooks();
				}

				return base.Books;
			}
			set => base.Books = value;
		}

		private void FetchBooks()
		{
			_booksAreFetchedOrSet = true;
			base.Books = new HashSet<Book>(
				_bookDao.FindByAuthor(this));
		}

		public override void AddBook(Book book)
		{
			if(!_booksAreFetchedOrSet)
            {
				FetchBooks();
			}
			base.AddBook(book);
		}

		public override void RemoveBook(Book book)
		{
			if(BooksAreFetchedOrSet)
            {
				FetchBooks();
			}
			base.RemoveBook(book);
		}

		public override bool Equals(object obj)
		{
			if (obj is Author author)
            { 
				return this.Id == author.Id;
			}

			return false;
		}

		public AuthorProxy Refresh()
		{
			FirstName = _oldFirstName;
			LastName = _oldLastName;

			return this;
		}

		public AuthorProxy Update()
		{
			_oldFirstName = FirstName;
			_oldLastName = LastName;

			return this;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
