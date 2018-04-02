using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService.DatabaseAccess
{
	public interface IBookDao
	{
		Book FindById(uint id);

		IList<Book> FindAll();
		IList<Book> FindBySection(Book.section section);
		IList<Book> FindByAuthor(Author author);
		IList<Book> FindByTitle(string title);
		bool BookIsPresent(Book book);

		Book Save(Book book);
		Book Refresh(Book book);
		void Delete(Book book);
	}
}
