using System.Collections.Generic;
using BookLibrary.Core.Service;
using BookLibrary.Core.Dao;
using BookLibrary.Entities;

namespace BookLibrary.Database.Impl.Service
{
	public class AuthorService : IAuthorService
	{
		private IAuthorDao authorDao;

        public IAuthorDao AuthorDao
        {
            get { return authorDao; }
            set { authorDao = value; }
        }

		public void Delete(Author author)
		{
			authorDao.Delete(author);
		}

		public IList<Author> FindAll()
		{
			return authorDao.FindAll();
		}

		public Author FindById(int id)
		{
			return authorDao.FindById(id);
		}

		public IList<Author> FindByName(string firstName, string lastName)
		{
			return authorDao.FindByName(firstName, lastName);
		}

		public void Refresh(Author author)
		{
			authorDao.Refresh(author);
		}

		public void SaveOrUpdate(Author author)
		{
			authorDao.Save(author, SaveOption.UPDATE_IF_EXIST);
		}
	}
}
