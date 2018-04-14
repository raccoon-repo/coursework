using System.Collections.Generic;
using BookService.Entities;

namespace BookService.Service
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
