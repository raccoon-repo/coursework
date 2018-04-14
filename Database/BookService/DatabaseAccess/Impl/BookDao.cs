using BookService.Entities;
using BookService.Entities.Proxies;
using BookService.DatabaseAccess.Queries;
using BookService.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace BookService.DatabaseAccess.Impl
{
	public class BookDao : IBookDao
	{
		private static readonly BookDao instance = new BookDao();
		
		// Cached instances are stored here
		private static ConcurrentDictionary<int, Book> cache = 
			new ConcurrentDictionary<int, Book>();

		public static BookDao Instance
		{
			get { return instance; }
			private set { }
		}

		public bool BookIsPresent(Book book)
		{
			throw new NotImplementedException();
		}

		public IList<Book> FindAll()
		{
			DataSet resultSet = DBWorker.ExecuteQuery(Books.FIND_ALL, null);
			IList<Book> books = ParseBooks(resultSet.Tables[0]);
			resultSet.Dispose();

			return books;
		}

		public IList<Book> FindByAuthor(Author author)
		{
			if (author.Id == 0)
				return null;

			var args = new Dictionary<string, string> {
				{ "@id", author.Id.ToString() }
			};

			DataSet resultSet = DBWorker.ExecuteQuery(Books.FETCH_BOOKS, args);
			IList<Book> books = ParseBooks(resultSet.Tables[0]);
			resultSet.Dispose();

			return books;
		}

		public IList<Book> FindBySection(BookSection section)
		{
			var args = new Dictionary<string, string> {
				{ "@section", section.ToString() }
			};

			DataSet resultSet = DBWorker.ExecuteQuery(Books.FIND_BY_SECTION, args);
			IList<Book> books = ParseBooks(resultSet.Tables[0]);
			resultSet.Dispose();

			return books;
		}


		public IList<Book> FindByTitle(string title)
		{
			var args = new Dictionary<string, string> {
				{ "@title", '%' + title + '%' }
			};

			DataSet resultSet = DBWorker.ExecuteQuery(Books.FIND_BY_TITLE, args);
			IList<Book> books = ParseBooks(resultSet.Tables[0]);
			resultSet.Dispose();

			return books;
		}

		/* ----- Results of the execution of the following operations are cached -----*/

		public Book FindById(int id)
		{
			Book book;

			if (cache.TryGetValue(id, out book)) {
				return new BookProxy(book);
			}

			var args = new Dictionary<string, string> {
				{ "@id", id.ToString() }
			};

			DataSet resultSet = DBWorker.ExecuteQuery(Books.FIND_BY_ID, args);
			object[] row = resultSet.Tables[0].Rows[0].ItemArray;

			book = ParseBook(row);
			cache.TryAdd(book.Id, book);
			resultSet.Dispose();

			return book;
		}

		public Book Refresh(Book book)
		{
			Book temp;

			// check first if book is already cached
			if (cache.TryGetValue(book.Id, out temp)) 
			{
				book.Title		 = temp.Title;
				book.Section	 = temp.Section;
				book.Description = temp.Description;

				return book;
			} 
			else // if not - query data from database
			{
				var args = new Dictionary<string, string> {
					{ "@id", book.Id.ToString() }
				};

				DataSet resultSet = DBWorker.ExecuteQuery(Books.FIND_BY_ID, args);
				temp = ParseBook(resultSet.Tables[0].Rows[0].ItemArray);

				book.Title = temp.Title;
				book.Section = temp.Section;
				book.Description = temp.Description;

				cache.AddOrUpdate(book.Id, book, (a, b) => book);

				return book;
			}
		}

		public void Save(Book book, SaveOption option)
		{
			Save(book, option, new HashSet<int>(), new HashSet<int>());
		}

		public void Save(Book book, SaveOption option, ISet<int> savedAuthors, ISet<int> savedBooks)
		{
			var args = new Dictionary<string, string> {
				{ "@id", book.Id.ToString() }
			};

			uint count = GetCount(Books.COUNT_BY_ID, args);

			if (count != 0) 
			{
				switch (option) {
					case SaveOption.SAVE_ONLY:
						return;
					case SaveOption.UPDATE_IF_EXIST:
						Update(book, savedAuthors, savedBooks);
						return;
				}
			}

			args.Remove("@id");
			args.Add("@title", book.Title);
			args.Add("@rating", Utils.Utils.FloatToString(book.Rating));
			args.Add("@section", book.Section.ToString());
			args.Add("@description", book.Description);

			book.Id = DBWorker.InsertAndReturnId(Books.INSERT, args);
			savedBooks.Add(book.Id);

			foreach (var author in book.Authors) {
				if (!savedAuthors.Contains(author.Id)) {
					AuthorDao.Instance.Save(author, SaveOption.UPDATE_IF_EXIST, 
											savedAuthors, savedBooks);
				}

				args.Clear();
				args.Add("@book_id", book.Id.ToString());
				args.Add("@author_id", author.Id.ToString());

				count = GetCount(JoinTable.COUNT, args);
				if (count == 0) {
					DBWorker.ExecuteNonQuery(JoinTable.INSERT, args);
				}
			}

			BookProxy proxy = new BookProxy(book);
			cache.TryAdd(proxy.Id, proxy);
		}

		public void Update(Book book)
		{
			var args = new Dictionary<string, string> {
				{ "@id", book.Id.ToString() }, { "@title", book.Title },
				{ "@description", book.Description},
				{ "@rating", Utils.Utils.FloatToString(book.Rating) },
				{ "@section", book.Section.ToString() }
			};

			DBWorker.ExecuteNonQuery(Books.UPDATE, args);

			if (book is BookProxy) 
			{
				cache.AddOrUpdate(book.Id, book, (a, b) => book);
			} 
			else 
			{
				BookProxy proxy = new BookProxy(book);
				cache.AddOrUpdate(book.Id, proxy, (a, b) => proxy);
			}
		}

		public void Update(Book book, ISet<int> updatedAuthors, ISet<int> updatedBooks)
		{
			if(!updatedBooks.Contains(book.Id)) {
				Update(book);
				updatedBooks.Add(book.Id);
				var actualAuthors = new HashSet<int>();

				if ((book is BookProxy proxy && proxy.AuthorsAreFetchedOrSet) || 
					(!(book is BookProxy) && book.Authors.Count != 0)) 
				{
					foreach (var author in book.Authors) {
						if (!updatedAuthors.Contains(author.Id)) {
							AuthorDao.Instance.Save(author, SaveOption.UPDATE_IF_EXIST,
													updatedAuthors, updatedBooks);
						}

						actualAuthors.Add(author.Id);
					}

					var fetchedAuthors = GetAuthorsId(book);
					var args = new Dictionary<string, string>();

					foreach (var id in actualAuthors) {
						if (!fetchedAuthors.Contains(id)) {
							args.Add("@author_id", id.ToString());
							args.Add("@book_id", book.Id.ToString());
							DBWorker.ExecuteNonQuery(JoinTable.DELETE, args);
						}
					}
				}


			}
		}

		public IList<Book> FindByRating(float from, float to)
		{

			if (from >= 0.0f && from <= 10.0f && from <= to && to <= 10.0f) {
				var args = new Dictionary<string, string>() {
					{ "@from", Utils.Utils.FloatToString(from) },
					{ "@to", Utils.Utils.FloatToString(to) }
				};

				DataSet dataSet = DBWorker.ExecuteQuery(Books.FIND_BY_RATING_IN_RANGE, args);
				IList<Book> books = ParseBooks(dataSet.Tables[0]);
				dataSet.Dispose();

				return books;
			}

			return null;
		}

		public IList<Book> FindByRating(float rating)
		{
			if (rating >= 0.0f && rating <= 10.0f) {
				var args = new Dictionary<string, string>() {
					{ "@rating", Utils.Utils.FloatToString(rating) }
				};

				DataSet dataSet = DBWorker.ExecuteQuery(Books.FIND_BY_RATING, args);
				IList<Book> books = ParseBooks(dataSet.Tables[0]);
				dataSet.Dispose();

				return books;
			}

			return null;
		}



		public void Delete(Book book)
		{

			if (book is BookProxy) {

				var args = new Dictionary<string, string> {
					{ "@id", book.Id.ToString() }
				};

				// Delete from book table
				DBWorker.ExecuteNonQuery(Books.DELETE, args);

				// Delete all rows in the intermediate table
				// that have book_id column value equals to @id
				DBWorker.ExecuteNonQuery(JoinTable.DELETE_BY_BOOK, args);

				// Remove it from cache
				cache.TryRemove(book.Id, out _);
			}
		}

		/* ----- ---------------------------------------------------------------------- ----- */


		private IList<Book> ParseBooks(DataTable table)
		{
			IList<Book> books = new List<Book>();
		
			foreach (DataRow row in table.Rows) {
				BookProxy book = ParseBook(row.ItemArray);
				books.Add(book);
			}

			return books;
		}

		private BookProxy ParseBook(object[] row)
		{
			BookProxy book = new BookProxy {
				Id = Int32.Parse(row[0].ToString()),
				Title = (String)row[1],
				Section = BookUtils.ParseSection(row[2].ToString()),
				Description = row[3].ToString(),
				Rating = float.Parse(row[4].ToString())
			};


			return book;
		}

		private uint GetCount(string sqlStatement, Dictionary<string, string> args)
		{
			object o_count = DBWorker.ExecuteScalar(sqlStatement, args);
			return UInt32.Parse(o_count.ToString());
		}

		private ISet<int> GetAuthorsId(Book book)
		{
			var args = new Dictionary<string, string>() {
				{ "@book_id", book.Id.ToString() }
			};

			DataSet ds = DBWorker.ExecuteQuery(JoinTable.AUTHORS_ID, args);
			var set = new HashSet<int>();
			foreach (DataRow row in ds.Tables[0].Rows) {
				int id = Int32.Parse(row.ItemArray[0].ToString());
				set.Add(id);
			}

			return set;
		}
	}
}
