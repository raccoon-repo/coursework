using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService.Queries
{
	public static class Books
	{
		public const string FIND_BY_ID =
				"SELECT * FROM book AS b WHERE b.id = @id";
		public const string FIND_ALL =
				"SELECT * FROM book";
		public const string COUNT =
				"SELECT COUNT(*) FROM book AS b WHERE b.id = @id";

		public const string FIND_BY_TITLE =
				"SELECT * FROM book AS b WHERE b.title LIKE @title";
		public const string FIND_BY_AUTHOR =
				"SELECT DISTINCT * FROM book AS b " +
				"LEFT JOIN book_author ba ON b.id = ba.book_id " +
				"LEFT JOIN author a ON a.id = ba.author_id WHERE a.id = @author_id";

		public const string FIND_BY_SECTION =
				"SELECT DISTINCT * FROM book b WHERE b.section = @section";

		public const string FETCH_AUTHORS = 
			"SELECT DISTINCT * FROM author a " +
			"LEFT JOIN book_author ba ON ba.author_id = a.id " +
			"LEFT JOIN book b ON ba.book_id = b.id WHERE b.id = @id";

		public const string INSERT =
				"INSERT INTO book (title, section, description) VALUES (@title, @section, @description)";
		public const string UPDATE =
				"UPDATE book b SET b.title = @title, b.section = @section, b.description = @description WHERE b.id = @id";

		public const string INSERT_QUANTITY =
				"INSERT INTO book_quantity (book_id, quantity) VALUES (@id, @quantity)";
		public const string UPDATE_QUANTITY =
				"UPDATE book_quantity SET quantity = @quantity WHERE book_id = @id";

		public const string DELETE =
				"DELETE FROM book WHERE id = @id";

	}
}
