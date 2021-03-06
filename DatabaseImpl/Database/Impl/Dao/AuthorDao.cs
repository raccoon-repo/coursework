﻿using System;
using System.Collections.Generic;
using BookLibrary.Database.Queries;
using BookLibrary.Core.Dao;
using System.Data;
using System.Collections.Concurrent;
using BookLibrary.Entities;
using BookLibrary.Entities.Proxies;

namespace BookLibrary.Database.Impl.Dao
{
	public class AuthorDao : IAuthorDao
	{
		private static ConcurrentDictionary<int, Author> cache =
			new ConcurrentDictionary<int, Author>();

        private DBWorker _dBWorker;
        private IBookDao _bookDao;

		public AuthorDao(DBWorker dBWorker)
		{
			if (dBWorker == null)
				throw new ArgumentException("dBWorker cannot be null");

			_dBWorker = dBWorker;
			_bookDao = DaoCache.GetBookDao(dBWorker.ConnectionString);
		}
		
		public AuthorDao() {}
		
		public IBookDao BookDao
        {
            get => _bookDao;
			set => _bookDao = value;
		}

		public DBWorker DBWorker
		{
			get => _dBWorker;
			set => _dBWorker = value;
		}

		public IList<Author> FindAll()
		{
			DataSet resultSet = _dBWorker.ExecuteQuery(Authors.FIND_ALL, null);
			DataTable dataTable = resultSet.Tables[0];

			IList<Author> authors = ParseAuthors(dataTable);
			resultSet.Dispose();

			return authors;

		}

		public IList<Author> FindByBook(Book book)
		{
            if (book.Id == 0)
                return null;

			Dictionary<string, string> args = new Dictionary<string, string>() {
				{"@id", book.Id.ToString()}
			};
			IList<Author> authors;
        
			DataSet dataSet = _dBWorker.ExecuteQuery(Authors.FETCH_AUTHORS, args);
			authors = ParseAuthors(dataSet.Tables[0]);
			dataSet.Dispose();


			return authors;
		}

		public IList<Author> FindByName(string firstName, string lastName)
		{
			if (firstName == null || lastName == null)
				return null;

			Dictionary<string, string> args = new Dictionary<string, string>() {
				{ "@first_name", firstName }, { "@last_name", lastName }
			};
			IList<Author> authors;

			DataSet dataSet = _dBWorker.ExecuteQuery(Authors.FIND_BY_NAME, args);
			authors = ParseAuthors(dataSet.Tables[0]);
			dataSet.Dispose();

			return authors;
		}

		public Author FindById(int id)
		{
            if (cache.TryGetValue(id, out Author temp))
            {
                return temp;
            }

            var args = new Dictionary<string, string>() {
				{ "@id", id.ToString() }
			};

			DataSet dataSet = _dBWorker.ExecuteQuery(Authors.FIND_BY_ID, args);
			temp = ParseAuthor(dataSet.Tables[0].Rows[0].ItemArray);
			dataSet.Dispose();

			cache.TryAdd(id, temp);

			return temp;
		}

		public void Delete(Author author)
		{
			if (author.Id == 0)
				return;

			var args = new Dictionary<string, string> {
				{ "@id", author.Id.ToString() }
			};

			int count = GetCount(Authors.COUNT_BY_ID, args);

			if (count == 0)
				return;

			_dBWorker.ExecuteNonQuery(Authors.DELETE, args);
			_dBWorker.ExecuteNonQuery(JoinTable.DELETE_BY_AUTHOR, args);
			cache.TryRemove(author.Id, out _);
		}

		public Author Refresh(Author author)
		{
			if (author.Id == 0)
				return null;

			Author temp;

			if (cache.TryGetValue(author.Id, out temp))
			{
				var proxy = (AuthorProxy) temp;

				proxy.Refresh();
				
				// intended reference comparison
				if (author == proxy)
					return proxy;
				
				author.FirstName = proxy.FirstName;
				author.LastName  = proxy.LastName;
				
				return author;
			} else {

				var args = new Dictionary<string, string>() {
					{ "@id", author.Id.ToString() }
				};

				DataSet dataSet = _dBWorker.ExecuteQuery(Authors.FIND_BY_ID, args);
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

            int count = GetCount(Authors.COUNT_BY_ID, args);

            if (count != 0)
            {
	            switch (option)
	            {
		            case SaveOption.SAVE_ONLY:
			            return;
		            case SaveOption.UPDATE_IF_EXIST:
			            Update(author, savedAuthors, savedBooks);
			            return;
	            }
            }

            args.Add("@first_name", author.FirstName);
            args.Add("@last_name", author.LastName);

            author.Id = _dBWorker.InsertAndReturnId(Authors.INSERT, args);
            savedAuthors.Add(author.Id);

            foreach (var book in author.Books) {
                if (!savedBooks.Contains(book.Id)) 
                {
                    _bookDao.Save(book, SaveOption.UPDATE_IF_EXIST, savedAuthors, savedBooks);

                }

                args.Clear();
                args.Add("@book_id", book.Id.ToString());
                args.Add("@author_id", author.Id.ToString());

                count = GetCount(JoinTable.COUNT, args);
                if (count == 0) 
                {
                    _dBWorker.ExecuteNonQuery(JoinTable.INSERT, args);
                }
            }

            AuthorProxy proxy = new AuthorProxy(author) {
                BookDao = _bookDao
            };

			cache.TryAdd(proxy.Id, proxy);
		}

		public void Update(Author author)
		{
			var args = new Dictionary<string, string>() {
				{ "@id", author.Id.ToString() },
				{ "@first_name", author.FirstName },
				{ "@last_name", author.LastName }
			};

			_dBWorker.ExecuteNonQuery(Authors.UPDATE, args);

			if (author is AuthorProxy proxy)
			{
				proxy.Update();
			} 
			else 
			{
                proxy = new AuthorProxy(author) {
                    BookDao = _bookDao
                };

				cache.AddOrUpdate(author.Id, proxy, (a, b) => proxy);
			}

		}

		public void Update(Author author, ISet<int> updatedAuthors, ISet<int> updatedBooks)
		{
			if (!updatedAuthors.Contains(author.Id)) 
			{
				Update(author);
				updatedAuthors.Add(author.Id);
				var actualBooks = new HashSet<int>();

				// check if reference's type is AuthorProxy and its books 
				// are loaded in order not to fetch authors from database
				// or if it is new object that might not be in the database
				if ((author is AuthorProxy proxy && proxy.BooksAreFetchedOrSet) || 
					(!(author is AuthorProxy) && author.Books.Count != 0)) 
				{
					foreach (var book in author.Books)
                    {
						if (!updatedBooks.Contains(book.Id)) 
						{
							_bookDao.Save(book, SaveOption.UPDATE_IF_EXIST,
                                         updatedAuthors, updatedBooks
                            );
						}

                        actualBooks.Add(book.Id);
					}

					var fetchedBooks = GetBooksIdByAuthor(author);
					var args = new Dictionary<string, string>();

					foreach (int id in actualBooks) 
					{
						if (!fetchedBooks.Contains(id)) 
						{
							args.Clear();
							args.Add("@book_id", id.ToString());
							args.Add("@author_id", author.Id.ToString());
							_dBWorker.ExecuteNonQuery(JoinTable.DELETE, args);
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
			var id = int.Parse(row[0].ToString());
			var firstName = row[1].ToString();
			var lastName = row[2].ToString();
			
            var author = new AuthorProxy(id, firstName, lastName) {
                BookDao = _bookDao
			};

			return author;
		}

		private int GetCount(string sqlStatement, Dictionary<string, string> args)
		{
			var count = _dBWorker.ExecuteScalar(sqlStatement, args);
			return int.Parse(count.ToString());
		}

		private ISet<int> GetBooksIdByAuthor(Author author)
		{
			var args = new Dictionary<string, string> {
				{ "@author_id", author.Id.ToString() }
			};

			DataSet ds = _dBWorker.ExecuteQuery(JoinTable.BOOKS_ID, args);
			var set = new HashSet<int>();

			foreach (DataRow row in ds.Tables[0].Rows) {
				int id = Int32.Parse(row.ItemArray[0].ToString());
				set.Add(id);
			}

			return set;
		}
	}
}
