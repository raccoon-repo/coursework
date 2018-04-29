using System.Collections.Generic;
using BookLibrary.Entities;

namespace BookLibrary.Core.Service
{
	public interface IBookService
	{
		Book FindById(int id);

		IList<Book> FindAll();
		IList<Book> FindByTitle(string title);
		IList<Book> FindBySection(BookSection section);
		IList<Book> FindByRating(float rating);
		IList<Book> FindByRating(float from, float to);

		void SaveOrUpdate(Book book);
		void Delete(Book book);
		void Refresh(Book book);
	}
}
