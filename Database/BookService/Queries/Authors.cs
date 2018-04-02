using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService.Queries
{
	public static class Authors
	{
		public const string FIND_BY_ID =
			"SELECT * FROM author a WHERE a.id = @id";
		public const string FIND_ALL =
			"SELECT * FROM author";
		public const string FIND_BY_NAME =
			"SELECT * FROM author a WHERE a.fisrt_name = @first_name AND a.last_name = @last_name";
		public const string COUNT =
			"SELECT COUNT(*) FROM author a WHERE a.id = @id";

		public const string FETCH_BOOKS =
			"SELECT DISTINCT * FROM book b " +
			"LEFT JOIN book_author ba ON ba.book_id = b.id" +
			"LEFT JOIN author a ON b.author_id = a.id WHERE a.id = @id";

		public const string INSERT =
			"INSERT INTO author (first_name, last_name) VALUES (@first_name, @last_name)";
		public const string UPDATE =
			"UPDATE author SET first_name = @first_name, last_name = @last_name WHERE id = @id";
		public const string DELETE =
			"DELETE FROM author a WHERE a.id = @id";
	}
}
