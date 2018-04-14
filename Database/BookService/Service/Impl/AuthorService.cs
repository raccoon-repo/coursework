using System.Collections.Generic;
using BookService.DatabaseAccess;
using BookService.DatabaseAccess.Impl;
using BookService.Entities;

namespace BookService.Service.Impl
{
	public class AuthorService : IAuthorService
	{
		private IAuthorDao authorDao = AuthorDao.Instance;

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
