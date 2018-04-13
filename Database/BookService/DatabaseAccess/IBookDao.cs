using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService.DatabaseAccess
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
	}
}
