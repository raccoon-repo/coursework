namespace BookLibrary.Database.Queries
{
	public static class Authors
	{
		public const string FIND_BY_ID =
			"SELECT * FROM author a WHERE a.id = @id";
		public const string FIND_ALL =
			"SELECT * FROM author";
		public const string FIND_BY_NAME =
			"SELECT * FROM author a WHERE a.fisrt_name = @first_name AND a.last_name = @last_name";
		public const string COUNT_BY_ID =
			"SELECT COUNT(*) FROM author a WHERE a.id = @id";

		public const string FETCH_AUTHORS =
			"SELECT DISTINCT a.id, a.first_name, a.last_name FROM author a " +
			"LEFT JOIN book_author ba ON ba.author_id = a.id " +
			"LEFT JOIN book b ON ba.book_id = b.id WHERE b.id = @id";

		public const string INSERT =
			"INSERT INTO author (first_name, last_name) VALUES (@first_name, @last_name)";
		public const string UPDATE =
			"UPDATE author SET first_name = @first_name, last_name = @last_name WHERE id = @id";
		public const string DELETE =
			"DELETE FROM author a WHERE a.id = @id";
	}
}
