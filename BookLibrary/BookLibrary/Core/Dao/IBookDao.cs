using System.Collections.Generic;
using BookLibrary.Entities;

/*
 * Dao stands for Data Access Object
 * 
 * It is used for accessing Book instances
 * from any datasource
 */

namespace BookLibrary.Core.Dao
{
	public interface IBookDao
	{

		Book FindById(int id);

		IList<Book> FindAll();
		IList<Book> FindBySection(BookSection section);
		IList<Book> FindByAuthor(Author author);
		IList<Book> FindByTitle(string title);
		IList<Book> FindByRating(float from, float to);
		IList<Book> FindByRating(float rating);
		bool BookIsPresent(Book book);

		void Save(Book book, SaveOption option);
		void Save(Book book, SaveOption option, 
				  ISet<int> savedAuthors, ISet<int> savedBooks);

		void Update(Book book);
		
		void Update(Book book, 
					ISet<int> updatedAuthors, ISet<int> updatedBooks);

		Book Refresh(Book book); 
		void Delete(Book book);
        void Delete(int id);
	}
}
