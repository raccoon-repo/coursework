using System.Collections.Generic;
using BookLibrary.Core.Dao;
using BookLibrary.Core.Service;
using BookLibrary.Entities;

namespace BookLibrary.Service
{
	public class AuthorService : IAuthorService
	{
		private IAuthorDao _authorDao;

        public IAuthorDao AuthorDao
        {
            get => _authorDao;
	        set => _authorDao = value;
        }

		public void Delete(Author author)
		{
			_authorDao.Delete(author);
		}

		public IList<Author> FindAll()
		{
			return _authorDao.FindAll();
		}

		public Author FindById(int id)
		{
			return _authorDao.FindById(id);
		}

		public IList<Author> FindByName(string firstName, string lastName)
		{
			return _authorDao.FindByName(firstName, lastName);
		}

		public void Refresh(Author author)
		{
			_authorDao.Refresh(author);
		}

		public void SaveOrUpdate(Author author)
		{
			_authorDao.Save(author, SaveOption.UPDATE_IF_EXIST);
		}
	}
}
