using Database.BookService.Proxies;
using Database.BookService.Queries;
using Database.BookService.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService.DatabaseAccess
{
	public class BookDao : IBookDao
	{
		private static readonly BookDao instance = new BookDao();
		private static ConcurrentDictionary<uint, BookProxy> cache = 
			new ConcurrentDictionary<uint, BookProxy>();


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
				{ "@author_id", author.Id.ToString() }
			};

			DataSet resultSet = DBWorker.ExecuteQuery(Books.FIND_BY_AUTHOR, args);
			IList<Book> books = ParseBooks(resultSet.Tables[0]);
			resultSet.Dispose();

			return books;
		}

		public IList<Book> FindBySection(Book.section section)
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

		public Book FindById(uint id)
		{
			BookProxy book;

			if (cache.TryGetValue(id, out book)) {
				return book;
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

		/* Query book from database and update properties
		 * of passed book in accordance with the result
		 * of the query
		 */

		public Book Refresh(Book book)
		{
			BookProxy temp;

			if(cache.TryGetValue(book.Id, out temp) && book is BookProxy) {
				book.Title		 = temp.Title;
				book.Section	 = temp.Section;
				book.Description = temp.Description;

				return book;
			} else if (book is BookProxy) {
				var args = new Dictionary<string, string> {
					{ "@id", book.Id.ToString() }
				};

				DataSet resultSet = DBWorker.ExecuteQuery(Books.FIND_BY_ID, args);
				temp = ParseBook(resultSet.Tables[0].Rows[0].ItemArray);

				book.Title = temp.Title;
				book.Section = temp.Section;
				book.Description = temp.Description;

				cache.AddOrUpdate(book.Id, (BookProxy) book, 
					(key, old) => (BookProxy) book
				);

				return book;
			}

			return book;
		}

		public Book Save(Book book)
		{
			var args = new Dictionary<string, string> {
				{ "@id", book.Id.ToString() }, { "@title", book.Title },
				{ "@rating", book.Rating.ToString() },
				{ "@section", book.Section.ToString() },
				{ "@description", book.Description}
			};

			// update
			if (book is BookProxy) {
				BookProxy proxy = (BookProxy)book;

				DBWorker.ExecuteNonQuery(Books.UPDATE, args);
				cache.AddOrUpdate(book.Id, proxy, (key, oldValue) => proxy);

				if (proxy.AuthorsAreFetched) {

				}
				
			} else {
				args.Remove("@id");
				BookProxy proxy = new BookProxy(book);
				uint id = DBWorker.InsertAndReturnId(Books.INSERT, args);
				proxy.Id = id;

				return proxy;
			}

			return book;
		}

		public IList<Book> FindByRating(float from, float to)
		{

			if (from >= 0.0f && from <= 10.0f && from <= to && to <= 10.0f) {
				var args = new Dictionary<string, string>() {
					{ "@from", from.ToString() },
					{ "@to", from.ToString() }
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
					{ "@rating", rating.ToString() }
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
				DBWorker.ExecuteNonQuery(Intermediate.DELETE_BY_BOOK, args);

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
				Id = UInt32.Parse(row[0].ToString()),
				Title = (String)row[1],
				Section = BookUtils.ParseSection(row[2].ToString()),
				Description = row[3].ToString(),
				Rating = float.Parse(row[4].ToString())
			};

			return book;
		}
	}
}
