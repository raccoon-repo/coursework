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
		private static HashSet<int> values = new HashSet<int>();

		private static ConcurrentDictionary<uint, Book> cache = 
			new ConcurrentDictionary<uint, Book>();


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
			IList<Book> books = ParseBooks(resultSet);
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
			IList<Book> books = ParseBooks(resultSet);
			resultSet.Dispose();

			return books;
		}


		public IList<Book> FindBySection(Book.section section)
		{
			var args = new Dictionary<string, string> {
				{ "@section", section.ToString() }
			};

			DataSet resultSet = DBWorker.ExecuteQuery(Books.FIND_BY_SECTION, args);
			IList<Book> books = ParseBooks(resultSet);
			resultSet.Dispose();

			return books;
		}


		public IList<Book> FindByTitle(string title)
		{
			var args = new Dictionary<string, string> {
				{ "@title", '%' + title + '%' }
			};

			DataSet resultSet = DBWorker.ExecuteQuery(Books.FIND_BY_TITLE, args);
			IList<Book> books = ParseBooks(resultSet);
			resultSet.Dispose();

			return books;
		}

		/*
		 * Results of the execution of the following operations are cached
		 */

		public Book FindById(uint id)
		{
			Book book;
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

		public Book Refresh(Book book)
		{
			Book temp;

			if(cache.TryGetValue(book.Id, out temp)) {
				book.Title		 = temp.Title;
				book.Section	 = temp.Section;
				book.Description = temp.Description;

				if (temp.AuthorsAreFetched) {
					book.Authors = temp.Authors;
				}

				return book;
			} else {
				var args = new Dictionary<string, string> {
					{ "@id", book.Id.ToString() }
				};

				DataSet resultSet = DBWorker.ExecuteQuery(Books.FIND_BY_ID, args);
				temp = ParseBook(resultSet.Tables[0].Rows[0].ItemArray);

				book.Title = temp.Title;
				book.Section = temp.Section;
				book.Description = temp.Description;

				cache.AddOrUpdate(book.Id, book, (key, old) => book);

				return book;
			}
		}

		public Book Save(Book book)
		{
			var args = new Dictionary<string, string> {
				{ "@id", book.Id.ToString() }
			};

			DataSet resultSet = DBWorker.ExecuteQuery(Books.COUNT, args);

			int count = Int32.Parse(resultSet.Tables[0].Rows[0].ItemArray[0].ToString());

			args.Add("@title", book.Title);
			args.Add("@section", book.Section.ToString());
			args.Add("@description", book.Description);

			if (count != 0) {
				DBWorker.ExecuteNonQuery(Books.UPDATE, args);

				cache.AddOrUpdate(book.Id, book, (key, old) => { 
					old.Title = book.Title;
					old.Description = book.Description;
					old.Section = book.Section;
					if (book.AuthorsAreFetched) {
						old.Authors = book.Authors;
					}

					return old;
				});
			} else {
				DBWorker.ExecuteNonQuery(Books.INSERT, args);
				cache.TryAdd(book.Id, book);
			}

			return book;
		}

		public void Delete(Book book)
		{
			var args = new Dictionary<string, string> {
				{ "@id", book.Id.ToString() }
			};

			DBWorker.ExecuteNonQuery(Books.DELETE, args);
			DBWorker.ExecuteNonQuery(Intermediate.DELETE_BY_BOOK, args);
			cache.TryRemove(book.Id, out _);
		}

		private IList<Book> ParseBooks(DataSet dataSet)
		{
			IList<Book> books = new List<Book>();
			DataSet resultSet = DBWorker.ExecuteQuery(Books.FIND_ALL, null);
			DataTable dataTable = resultSet.Tables[0];

			foreach (DataRow row in dataTable.Rows) {
				Book book = new Book();
				object[] items = row.ItemArray;
				books.Add(ParseBook(items));
			}

			return books;
		}

		private Book ParseBook(object[] columns)
		{
			Book book = new Book {
				Id = UInt32.Parse(columns[0].ToString()),
				Title = (String)columns[1],
				Section = BookUtils.ParseSection(columns[2].ToString()),
				Description = columns[3].ToString()
			};

			return book;
		}
	}
}
