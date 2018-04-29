namespace BookLibrary.Database.Queries
{
	public static class JoinTable
	{
		public const string INSERT =
			"INSERT INTO book_author (book_id, author_id) VALUES (@book_id, @author_id)";
		public const string DELETE =
			"DELETE FROM book_author WHERE author_id = @author_id AND book_id = @book_id";

		public const string DELETE_BY_BOOK =
			"DELETE FROM book_author WHERE book_id = @id";
		public const string DELETE_BY_AUTHOR =
			"DELETE FROM book_author WHERE author_id = @id";
		public const string COUNT =
			"SELECT COUNT(*) FROM book_author ba WHERE ba.book_id = @book_id AND ba.author_id = @author_id";

		public const string AUTHORS_ID =
			"SELECT author_id FROM book_author ba WHERE ba.book_id = @book_id";
		public const string BOOKS_ID =
			"SELECT book_id FROM book_author ba WHERE ba.author_id = @author_id";
	}
}
