using System.Collections.Generic;
using BookLibrary.Entities;

/*
 * Dao stands for Data Access Object
 * 
 * It is used for accessing Author instances 
 * from any datasource
 */

namespace BookLibrary.Core.Dao
{
	public interface IAuthorDao
	{
		Author FindById(int id);

		IList<Author> FindAll();
		IList<Author> FindByName(string firstName, string lastName);
		IList<Author> FindByBook(Book book);

		void Save(Author author, SaveOption option);
		void Save(Author author, SaveOption option, 
				  ISet<int> savedAuthor, ISet<int> savedBooks);
		void Update(Author author);
		void Update(Author author, ISet<int> updatedAuthors, 
					ISet<int> updatedBooks);

		Author Refresh(Author author);
		void Delete(Author author);
	}
}
