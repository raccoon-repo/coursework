namespace BookService.DatabaseAccess.Queries
{
	public static class Books
	{
		public const string FIND_BY_ID =
				"SELECT * FROM book AS b WHERE b.id = @id";
		public const string FIND_ALL =
				"SELECT * FROM book";
		public const string COUNT_BY_ID =
				"SELECT COUNT(*) FROM book AS b WHERE b.id = @id";

		public const string FIND_BY_TITLE =
				"SELECT * FROM book AS b WHERE b.title LIKE @title";

		public const string FIND_BY_SECTION =
				"SELECT DISTINCT * FROM book b WHERE b.section = @section";

		public const string FIND_BY_RATING_IN_RANGE =
				"SELECT DISTINCT * FROM book b WHERE b.rating >= @from AND b.rating <= @to";
		public const string FIND_BY_RATING =
				"SELECT DISTINCT * FROM book b WHERE b.rating LIKE @rating";

		public const string FETCH_BOOKS =
			"SELECT DISTINCT b.id, b.title, b.section, b.description, b.rating FROM book b " +
			"LEFT JOIN book_author ba ON ba.book_id = b.id " +
			"LEFT JOIN author a ON ba.author_id = a.id WHERE a.id = @id";

		public const string INSERT =
				"INSERT INTO book (title, section, description, rating) VALUES (@title, @section, @description, @rating)";
		public const string UPDATE =
				"UPDATE book b SET " +
					"b.title = @title, b.section = @section, " +
					"b.description = @description, b.rating = @rating " +
				"WHERE b.id = @id";

		public const string INSERT_QUANTITY =
				"INSERT INTO book_quantity (book_id, quantity) VALUES (@id, @quantity)";
		public const string UPDATE_QUANTITY =
				"UPDATE book_quantity SET quantity = @quantity WHERE book_id = @id";

		public const string DELETE =
				"DELETE FROM book WHERE id = @id";

	}
}
