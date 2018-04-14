using System.Collections.Generic;
using BookService.Entities;

// Interface for accessing Book entities
// stored in the database

namespace BookService.DatabaseAccess
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

		// Update a row in database that corresponds
		// to given book entity
		void Update(Book book);
		
		// Update book entity and all of its authors
		void Update(Book book, 
					ISet<int> updatedAuthors, ISet<int> updatedBooks);

		// Fetch corresponding entity from cache or database
		// then rewrite all book properties in accordance with
		// previously fetched data
		Book Refresh(Book book); 
		void Delete(Book book);
	}
}
