using BookLibrary.Entities;
using BookLibrary.Entities.Proxies;
using BookLibrary.Database.Queries;
using BookLibrary;
using BookLibrary.Core.Dao;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace BookLibrary.Database.Impl.Dao
{
	public class BookDao : IBookDao
	{	
		private static ConcurrentDictionary<int, Book> cache = 
			new ConcurrentDictionary<int, Book>();

        private DBWorker _dBWorker;
        private IAuthorDao _authorDao;

		public BookDao(DBWorker dBWorker)
		{
			if (dBWorker == null)
				throw new ArgumentException("dBWorker cannot be null");

			_dBWorker = dBWorker;
			_authorDao = DaoCache.GetAuthorDao(_dBWorker.ConnectionString);
		}
		
		public BookDao() { }

        public IAuthorDao AuthorDao
        {
            get => _authorDao;
	        set => _authorDao = value;
        }

		public DBWorker DBWorker
		{
			get => _dBWorker;
			set => _dBWorker = value;
		}

		public bool BookIsPresent(Book book)
		{
			throw new NotImplementedException();
		}

		public IList<Book> FindAll()
		{
			DataSet resultSet = _dBWorker.ExecuteQuery(Books.FIND_ALL, null);
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

			DataSet resultSet = _dBWorker.ExecuteQuery(Books.FETCH_BOOKS, args);
			IList<Book> books = ParseBooks(resultSet.Tables[0]);
			resultSet.Dispose();

			return books;
		}

		public IList<Book> FindBySection(BookSection section)
		{
			var args = new Dictionary<string, string> {
				{ "@section", section.ToString() }
			};

			DataSet resultSet = _dBWorker.ExecuteQuery(Books.FIND_BY_SECTION, args);
			IList<Book> books = ParseBooks(resultSet.Tables[0]);
			resultSet.Dispose();

			return books;
		}


		public IList<Book> FindByTitle(string title)
		{
			var args = new Dictionary<string, string> {
				{ "@title", '%' + title + '%' }
			};

			DataSet resultSet = _dBWorker.ExecuteQuery(Books.FIND_BY_TITLE, args);
			IList<Book> books = ParseBooks(resultSet.Tables[0]);
			resultSet.Dispose();

			return books;
		}

		/* ----- Results of the execution of the following operations are cached -----*/

		public Book FindById(int id)
		{

            if (cache.TryGetValue(id, out Book book))
            {
                return book;
            }

            var args = new Dictionary<string, string> {
				{ "@id", id.ToString() }
			};

			DataSet resultSet = _dBWorker.ExecuteQuery(Books.FIND_BY_ID, args);
			object[] row = resultSet.Tables[0].Rows[0].ItemArray;

			book = ParseBook(row);
			cache.TryAdd(book.Id, book);
			resultSet.Dispose();

			return book;
		}

		public Book Refresh(Book book)
		{	
			// check first if book is already cached
			if (cache.TryGetValue(book.Id, out var temp))
			{
				var proxy = (BookProxy) temp;

				// refresh the state of proxy to its initial state
				// because it might have been changed
				proxy.Refresh();
				
				// intended comparison by reference
				// if the book is a reference to the proxy instance
				// then return the proxy
				if (proxy == book)
					return proxy;
				
			
				// else update the book instance and return its proxy
			
				book.Title		 = proxy.Title;
				book.Section	 = proxy.Section;
				book.Description = proxy.Description;

				return proxy;
			} 
			else // if not - query data from database
			{
				var args = new Dictionary<string, string> {
					{ "@id", book.Id.ToString() }
				};
				
				DataSet resultSet = _dBWorker.ExecuteQuery(Books.FIND_BY_ID, args);
				
				temp = ParseBook(resultSet.Tables[0].Rows[0].ItemArray);

				book.Title = temp.Title;
				book.Section = temp.Section;
				book.Description = temp.Description;

				cache.AddOrUpdate(book.Id, temp, (a, b) => temp);

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

			int count = GetCount(Books.COUNT_BY_ID, args);

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
			args.Add("@rating", BookLibrary.Utils.FloatToString(book.Rating));
			args.Add("@section", book.Section.ToString());
			args.Add("@description", book.Description);

			book.Id = _dBWorker.InsertAndReturnId(Books.INSERT, args);
			savedBooks.Add(book.Id);

			foreach (var author in book.Authors) {
				if (!savedAuthors.Contains(author.Id)) {
					_authorDao.Save(author, SaveOption.UPDATE_IF_EXIST, 
											savedAuthors, savedBooks);
				}

				args.Clear();
				args.Add("@book_id", book.Id.ToString());
				args.Add("@author_id", author.Id.ToString());

				count = GetCount(JoinTable.COUNT, args);
				if (count == 0) {
					_dBWorker.ExecuteNonQuery(JoinTable.INSERT, args);
				}
			}

            BookProxy proxy = new BookProxy(book) {
                AuthorDao = _authorDao
            };
            cache.TryAdd(proxy.Id, proxy);
		}

		public void Update(Book book)
		{
			var args = new Dictionary<string, string> {
				{ "@id", book.Id.ToString() }, { "@title", book.Title },
				{ "@description", book.Description},
				{ "@rating", BookLibrary.Utils.FloatToString(book.Rating) },
				{ "@section", book.Section.ToString() }
			};

			_dBWorker.ExecuteNonQuery(Books.UPDATE, args);

			if (book is BookProxy proxy)
			{
				// no need to update cache
				// because we may have only one proxy
				// with the same identity at a time 
				proxy.Update();
			} 
			else 
			{
                proxy = new BookProxy(book) {
                    AuthorDao = _authorDao
                };
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
							_authorDao.Save(author, SaveOption.UPDATE_IF_EXIST,
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
							_dBWorker.ExecuteNonQuery(JoinTable.DELETE, args);
						}
					}
				}


			}
		}

        public void Delete(int id)
        {
            var args = new Dictionary<string, string> {
                    { "@id", id.ToString() }
                };

            // Delete from book table
            _dBWorker.ExecuteNonQuery(Books.DELETE, args);

            // Delete all rows in the intermediate table
            // that have book_id column value equals to @id
            _dBWorker.ExecuteNonQuery(JoinTable.DELETE_BY_BOOK, args);

            // Remove it from cache
            cache.TryRemove(id, out _);
        }

		public void Delete(Book book)
		{
            Delete(book.Id);
		}

		/* ----- ---------------------------------------------------------------------- ----- */

		public IList<Book> FindByRating(float from, float to)
		{

			if (from >= 0.0f && from <= 10.0f && from <= to && to <= 10.0f) {
				var args = new Dictionary<string, string>() {
					{ "@from", BookLibrary.Utils.FloatToString(from) },
					{ "@to", BookLibrary.Utils.FloatToString(to) }
				};

				DataSet dataSet = _dBWorker.ExecuteQuery(Books.FIND_BY_RATING_IN_RANGE, args);
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
					{ "@rating", BookLibrary.Utils.FloatToString(rating) }
				};

				DataSet dataSet = _dBWorker.ExecuteQuery(Books.FIND_BY_RATING, args);
				IList<Book> books = ParseBooks(dataSet.Tables[0]);
				dataSet.Dispose();

				return books;
			}

			return null;
		}

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

			var id = int.Parse(row[0].ToString());
			var title = (string) row[1];
			var section = BookUtils.ParseSection(row[2].ToString());
			var description = row[3].ToString();
			var rating = float.Parse(row[4].ToString());

			BookProxy book = new BookProxy (id, section: section,
					title: title, description: description, 
				 	rating: rating) 
			{
                AuthorDao = _authorDao
			};


			return book;
		}

		private int GetCount(string sqlStatement, IDictionary<string, string> args)
		{
			var count = _dBWorker.ExecuteScalar(sqlStatement, args);
			return int.Parse(count.ToString());
		}

		private ISet<int> GetAuthorsId(Book book)
		{
			var args = new Dictionary<string, string>() {
				{ "@book_id", book.Id.ToString() }
			};

			DataSet ds = _dBWorker.ExecuteQuery(JoinTable.AUTHORS_ID, args);
			var set = new HashSet<int>();
			foreach (DataRow row in ds.Tables[0].Rows) {
				int id = int.Parse(row.ItemArray[0].ToString());
				set.Add(id);
			}

			return set;
		}
	}
}
