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
		private static ConcurrentDictionary<uint, Author> cache =
			new ConcurrentDictionary<uint, Author>();

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
			DataTableReader dtr = new DataTableReader(dataTable);

			return null;

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

		public Author FindById(uint id)
		{
			if (id == 0)
				return null;
			Author temp;
			
			if(cache.TryGetValue(id, out temp)) {
				return temp;
			}

			var args = new Dictionary<string, string>() {
				{ "@id", id.ToString() }
			};
			DataSet dataSet = DBWorker.ExecuteQuery(Authors.FIND_BY_ID, args);
			temp = ParseAuthor(dataSet.Tables[0].Rows[0].ItemArray);

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

			object o_count = DBWorker.ExecuteScalar(Authors.COUNT_BY_ID, args);
			uint count = UInt32.Parse(o_count.ToString());

			if (count == 0)
				return;

			DBWorker.ExecuteNonQuery(Authors.DELETE, args);
			DBWorker.ExecuteNonQuery(Intermediate.DELETE_BY_AUTHOR, args);
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
				
				if (temp.BooksAreFetched) {
					author.Books = temp.Books;
				}

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

		public Author Save(Author author)
		{
			
			var args = new Dictionary<string, string>() {
				{ "@id", author.Id.ToString() }
			};

			// check if the author with the same id does exist
			object o_count = DBWorker.ExecuteScalar(Authors.COUNT_BY_ID, args);
			uint count = UInt32.Parse(o_count.ToString());

			args.Add("@first_name", author.FirstName);
			args.Add("@last_name", author.LastName);

			// if yes, update
			// otherwise, save it
			if (count != 0) {
				DBWorker.ExecuteNonQuery(Authors.UPDATE, args);
				cache.AddOrUpdate(author.Id, author, (key, old) => author);
				return author;
			} else {
				args.Remove("@id");
				//insert it and get id for this author
				author.Id = DBWorker.InsertAndReturnId(Authors.INSERT, args);
				cache.TryAdd(author.Id, author);

				return author;
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
			Author author = new Author() {
				Id = UInt32.Parse(row[0].ToString()),
				FirstName = row[1].ToString(),
				LastName = row[2].ToString()
			};

			return author;
		}
	}
}
