using System.Collections.Generic;
using BookLibrary.Core.Dao;
using BookLibrary.Core.Service;
using BookLibrary.Entities;

namespace BookLibrary.Database.Impl
{
	public class BookService : IBookService
	{
		private IBookDao bookDao;

        public IBookDao BookDao
        {
            get { return bookDao; }
            set { bookDao = value; }
        }

		public void Delete(Book book)
		{
			bookDao.Delete(book);
		}

		public IList<Book> FindAll()
		{
			return bookDao.FindAll();
		}

		public Book FindById(int id)
		{
			return bookDao.FindById(id);
		}

		public IList<Book> FindByRating(float rating)
		{
			return bookDao.FindByRating(rating);
		}

		public IList<Book> FindByRating(float from, float to)
		{
			return bookDao.FindByRating(from, to);
		}

		public IList<Book> FindBySection(BookSection section)
		{
			return bookDao.FindBySection(section);
		}

		public IList<Book> FindByTitle(string title)
		{
			return bookDao.FindByTitle(title);
		}

		public void Refresh(Book book)
		{
			bookDao.Refresh(book);
		}

		public void SaveOrUpdate(Book book)
		{
			bookDao.Save(book, SaveOption.UPDATE_IF_EXIST);
		}
	}
}
