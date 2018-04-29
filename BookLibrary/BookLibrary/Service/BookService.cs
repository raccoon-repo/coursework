using System.Collections.Generic;
using BookLibrary.Core.Dao;
using BookLibrary.Core.Service;
using BookLibrary.Entities;

namespace BookLibrary.Service
{
	public class BookService : IBookService
	{
		private IBookDao _bookDao;

        public IBookDao BookDao
        {
            get => _bookDao;
	        set => _bookDao = value;
        }

		public void Delete(Book book)
		{
			_bookDao.Delete(book);
		}

		public IList<Book> FindAll()
		{
			return _bookDao.FindAll();
		}

		public Book FindById(int id)
		{
			return _bookDao.FindById(id);
		}

		public IList<Book> FindByRating(float rating)
		{
			return _bookDao.FindByRating(rating);
		}

		public IList<Book> FindByRating(float from, float to)
		{
			return _bookDao.FindByRating(from, to);
		}

		public IList<Book> FindBySection(BookSection section)
		{
			return _bookDao.FindBySection(section);
		}

		public IList<Book> FindByTitle(string title)
		{
			return _bookDao.FindByTitle(title);
		}

		public void Refresh(Book book)
		{
			_bookDao.Refresh(book);
		}

		public void SaveOrUpdate(Book book)
		{
			_bookDao.Save(book, SaveOption.UPDATE_IF_EXIST);
		}
	}
}
