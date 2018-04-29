using System.Collections.Generic;
using BookLibrary.Entities;

namespace BookLibrary.Core.Service
{
	public interface IAuthorService
	{
		Author FindById(int id);

		IList<Author> FindAll();
		IList<Author> FindByName(string firstName, string lastName);

		void SaveOrUpdate(Author author);
		void Refresh(Author author);
		void Delete(Author author);
	}
}
