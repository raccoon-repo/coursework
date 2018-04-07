using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService.DatabaseAccess
{
	public interface IAuthorDao
	{
		Author FindById(uint id);

		IList<Author> FindAll();
		IList<Author> FindByName(string firstName, string lastName);
		IList<Author> FindByBook(Book book);

		Author Save(Author author);
		Author Refresh(Author author);
		void Delete(Author author);
	}
}
