using System;
using System.Collections.Generic;
using Database.BookService.Queries;
using System.Data;
using System.Collections.Concurrent;
using Database.BookService.Proxies;

namespace Database.BookService.DatabaseAccess
{
	public class AuthorDao : IAuthorDao
	{
		private static readonly AuthorDao instance = new AuthorDao();
		private static ConcurrentDictionary<int, Author> cache =
			new ConcurrentDictionary<int, Author>();

		public static AuthorDao Instance
		{
			get { return instance; }
			private set { }
		}

		private AuthorDao() { }

		
		public IList<Author> FindAll()
		{
			DataSet resultSet = DBWorker.ExecuteQuery(Authors.FIND_ALL, null);
			DataTable dataTable = resultSet.Tables[0];

			IList<Author> authors = ParseAuthors(dataTable);
			resultSet.Dispose();

			return authors;

		}

		public IList<Author> FindByBook(Book book)
		{
			if (book is BookProxy) {
				Dictionary<string, string> args = new Dictionary<string, string>() {
					{"@id", book.Id.ToString()}
				};
				IList<Author> authors;

				DataSet dataSet = DBWorker.ExecuteQuery(Books.FETCH_AUTHORS, args);
				authors = ParseAuthors(dataSet.Tables[0]);
				dataSet.Dispose();

				return authors;
			}

			return null;
		}

		public IList<Author> FindByName(string firstName, string lastName)
		{
			if (firstName == null || lastName == null)
				return null;

			Dictionary<string, string> args = new Dictionary<string, string>() {
				{ "@first_name", firstName }, { "@last_name", lastName }
			};
			IList<Author> authors;

			DataSet dataSet = DBWorker.ExecuteQuery(Authors.FIND_BY_NAME, args);
			authors = ParseAuthors(dataSet.Tables[0]);
			dataSet.Dispose();

			return authors;
		}

		public Author FindById(int id)
		{
			Author temp;		
			if(cache.TryGetValue(id, out temp)) {
				return temp;
			}

			var args = new Dictionary<string, string>() {
				{ "@id", id.ToString() }
			};
			DataSet dataSet = DBWorker.ExecuteQuery(Authors.FIND_BY_ID, args);
			temp = ParseAuthor(dataSet.Tables[0].Rows[0].ItemArray);
			dataSet.Dispose();

			cache.TryAdd(id, temp);

			return temp;
		}

		public void Delete(Author author)
		{
			if (author.Id == 0)
				return;

			Dictionary<string, string> args = new Dictionary<string, string> {
				{ "@id", author.Id.ToString() }
			};

			uint count = GetCount(Authors.COUNT_BY_ID, args);

			if (count == 0)
				return;

			DBWorker.ExecuteNonQuery(Authors.DELETE, args);
			DBWorker.ExecuteNonQuery(JoinTable.DELETE_BY_AUTHOR, args);
			cache.TryRemove(author.Id, out _);
		}

		public Author Refresh(Author author)
		{
			if (author.Id == 0)
				return null;
			Author temp;

			if (cache.TryGetValue(author.Id, out temp)) {
				author.FirstName = temp.FirstName;
				author.LastName  = temp.LastName;
				
				return author;
			} else {

				var args = new Dictionary<string, string>() {
					{ "@id", author.Id.ToString() }
				};

				DataSet dataSet = DBWorker.ExecuteQuery(Authors.FIND_BY_ID, args);
				temp = ParseAuthor(dataSet.Tables[0].Rows[0].ItemArray);

				author.FirstName = temp.FirstName;
				author.LastName = temp.LastName;

				return author;
			}
		}

		public void Save(Author author, SaveOption option) 
		{
			Save(author, option, new HashSet<int>(), new HashSet<int>());
		}

		public void Save(Author author, SaveOption option, ISet<int> savedAuthors, ISet<int> savedBooks)
		{
			var args = new Dictionary<string, string> {
				{ "@id", author.Id.ToString() }
			};

			uint count = GetCount(Authors.COUNT_BY_ID, args);

			if (count != 0) {
				if (option == SaveOption.SAVE_ONLY) {
					return;
				} else if (option == SaveOption.UPDATE_IF_EXIST) {
					Update(author, savedAuthors, savedBooks);
					return;
				}
			}

			args.Add("@first_name", author.FirstName);
			args.Add("@last_name", author.LastName);

			author.Id = DBWorker.InsertAndReturnId(Authors.INSERT, args);
			savedAuthors.Add(author.Id);

			foreach (var book in author.Books) {
				if (!savedBooks.Contains(book.Id)) {
					BookDao.Instance.Save(book, SaveOption.UPDATE_IF_EXIST, savedAuthors, savedBooks);

				}

				args.Clear();
				args.Add("@book_id", book.Id.ToString());
				args.Add("@author_id", author.Id.ToString());

				count = GetCount(JoinTable.COUNT, args);
				if (count == 0) { 
					DBWorker.ExecuteNonQuery(JoinTable.INSERT, args);
				}
			}

			AuthorProxy proxy = new AuthorProxy(author);
			cache.TryAdd(proxy.Id, proxy);
		}

		public void Update(Author author)
		{
			var args = new Dictionary<string, string>() {
				{ "@id", author.Id.ToString() },
				{ "@first_name", author.FirstName },
				{ "@last_name", author.LastName }
			};

			DBWorker.ExecuteNonQuery(Authors.UPDATE, args);

			if (author is AuthorProxy) 
			{
				cache.AddOrUpdate(author.Id, author, (a, b) => author);
			} 
			else 
			{
				AuthorProxy proxy = new AuthorProxy(author);
				cache.AddOrUpdate(author.Id, proxy, (a, b) => proxy);
			}

		}

		public void Update(Author author, ISet<int> updatedAuthors, ISet<int> updatedBooks)
		{
			if (!updatedAuthors.Contains(author.Id)) {
				Update(author);
				updatedAuthors.Add(author.Id);
				var actualBooks = new HashSet<int>();

				// check if reference's type is AuthorProxy and its books 
				// are loaded in order not to fetch authors from database
				// or if it is new object that might not be in the database
				if ((author is AuthorProxy proxy && proxy.BooksAreFetched) || 
					(!(author is AuthorProxy) && author.Books.Count != 0)) 
				{
					foreach (var book in author.Books) {
						if (!updatedBooks.Contains(book.Id)) 
						{
							BookDao.Instance.Save(book, SaveOption.UPDATE_IF_EXIST,
												  updatedAuthors, updatedBooks);
						}
							actualBooks.Add(book.Id);
					}

					var fetchedBooks = GetBooksIdByAuthor(author);
					var args = new Dictionary<string, string>();

					foreach (int id in actualBooks) {
						if (!fetchedBooks.Contains(id)) {
							args.Clear();
							args.Add("@book_id", id.ToString());
							args.Add("@author_id", author.Id.ToString());
							DBWorker.ExecuteNonQuery(JoinTable.DELETE, args);
						}
					}
				}
			}
		}


		private IList<Author> ParseAuthors(DataTable dataTable)
		{
			IList<Author> authors = new List<Author>();

			foreach(DataRow row in dataTable.Rows) {
				Author author = ParseAuthor(row.ItemArray);
				authors.Add(author);
			}

			return authors;
		}

		private Author ParseAuthor(object[] row)
		{
			AuthorProxy author = new AuthorProxy() {
				Id = Int32.Parse(row[0].ToString()),
				FirstName = row[1].ToString(),
				LastName = row[2].ToString()
			};

			return author;
		}

		private uint GetCount(string sqlStatement, Dictionary<string, string> args)
		{
			object o_count = DBWorker.ExecuteScalar(sqlStatement, args);
			return UInt32.Parse(o_count.ToString());
		}

		private ISet<int> GetBooksIdByAuthor(Author author)
		{
			var args = new Dictionary<string, string> {
				{ "@author_id", author.Id.ToString() }
			};

			DataSet ds = DBWorker.ExecuteQuery(JoinTable.BOOKS_ID, args);
			var set = new HashSet<int>();

			foreach (DataRow row in ds.Tables[0].Rows) {
				int id = Int32.Parse(row.ItemArray[0].ToString());
				set.Add(id);
			}

			return set;
		}
	}
}
