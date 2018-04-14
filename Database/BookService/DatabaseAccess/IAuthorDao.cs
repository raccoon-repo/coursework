﻿using System.Collections.Generic;
using BookService.Entities;

// Interface for accessing Author entities
// stored in the database

namespace BookService.DatabaseAccess
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