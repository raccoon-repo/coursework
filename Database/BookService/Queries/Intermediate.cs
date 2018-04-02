using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService.Queries
{
	public static class Intermediate
	{
		public const string INSERT =
			"INSERT INTO book_author (book_id, author_id) VALUES (@book_id, @author_id)";
		public const string DELETE_BY_BOOK =
			"DELETE FROM book_author WHERE book_id = @id";
		public const string DELETE_BY_AUTHOR =
			"DELETE FROM book_author WHERE author_id = @id";
		public const string COUNT =
			"SELECT COUNT(*) FROM book_author ba WHERE ba.book_id = @book_id AND ba.author_id = @author_id";
	}
}
