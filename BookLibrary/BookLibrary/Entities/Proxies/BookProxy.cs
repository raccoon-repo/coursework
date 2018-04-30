using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Security;
using System.Security.Cryptography;
using BookLibrary.Core.Dao;

namespace BookLibrary.Entities.Proxies
{
	public class BookProxy : Book
	{
		private bool _authorsAreFetchedOrSet = false;
        
		private IAuthorDao _authorDao;

		private string _oldTitle;
		private float _oldRating;
		private BookSection _oldSection;
		private string _oldDescription;
		
        public BookProxy(Book book)
		{
			Id			= book.Id;
			Title		= book.Title;
			Rating		= book.Rating;
			Section		= book.Section;
			Description = book.Description;

			_oldTitle 		= Title;
			_oldRating 		= Rating;
			_oldDescription = Description;
			_oldSection 	= Section;
			
			if(book is BookProxy proxy && proxy._authorsAreFetchedOrSet)
            {
				base.Authors = proxy.Authors;
				_authorsAreFetchedOrSet = true;
			}
            else if (!(book is BookProxy) && book.Authors.Count != 0)
            {
				base.Authors = book.Authors;
				_authorsAreFetchedOrSet = true;
			}
		}

		public BookProxy(int id, string title, float rating, BookSection section, string description)
		: base(id, title, rating, section, description)
		{
			_oldTitle = title;
			_oldRating = rating;
			_oldDescription = description;
			_oldSection = section;
		}

        public BookProxy() { }

        public IAuthorDao AuthorDao
        {
            get => _authorDao;
	        set => _authorDao = value;
        }		

		public override ISet<Author> Authors
		{
			get 
			{
				if(!_authorsAreFetchedOrSet)
                {
					FetchAuthors();
				}

				return base.Authors;
			}
			set => base.Authors = value;
		}

		public BookProxy Refresh()
		{
			Title 		= _oldTitle;
			Rating 		= _oldRating;
			Section 	= _oldSection;
			Description = _oldDescription;

			return this;
		}

		public BookProxy Update()
		{
			_oldTitle 		= Title;
			_oldRating 		= Rating;
			_oldSection 	= Section;
			_oldDescription = Description;

			return this;
		}

		private void FetchAuthors()
		{
			_authorsAreFetchedOrSet = true;
			base.Authors = new HashSet<Author>(
				_authorDao.FindByBook(this));
		}

		public bool AuthorsAreFetchedOrSet 
		{
			get => _authorsAreFetchedOrSet;
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
			if(!AuthorsAreFetchedOrSet)
            {
				FetchAuthors();
			}
			base.AddAuthor(author);
		}
		
		public override void RemoveAuthor(Author author)
		{
			if(!AuthorsAreFetchedOrSet)
            {
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
