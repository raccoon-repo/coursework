using System;
using System.Collections.Generic;
using Database.BookService.Queries;
using System.Data;

namespace Database.BookService.DatabaseAccess
{
	public class AuthorDao : IAuthorDao
	{
		private static readonly AuthorDao instance = new AuthorDao();

		public static AuthorDao Instance
		{
			get { return instance; }
			private set { }
		}

		private AuthorDao() { }

		public void Delete(Author author)
		{
			if (author.Id == 0)
				return;

			Dictionary<string, string> id = new Dictionary<string, string>();
			id.Add("@id", author.Id.ToString());

			DataSet ds = DBWorker.ExecuteQuery(Authors.COUNT, id);
			object idCount = ds.Tables[0].Rows[0].ItemArray[0];

			if (((Int32)idCount).Equals(0))
				return;

			DBWorker.ExecuteNonQuery(Authors.DELETE, id);
		}

		public IList<Author> FindAll()
		{
			DataSet resultSet = DBWorker.ExecuteQuery(Authors.FIND_ALL, null);
			DataTable dataTable = resultSet.Tables[0];
			DataTableReader dtr = new DataTableReader(dataTable);

			return null;

		}

		public Author FindById(uint id)
		{
			throw new NotImplementedException();
		}

		public IList<Author> FindByName(string firstName, string lastName)
		{
			throw new NotImplementedException();
		}

		public Author Refresh(Author author)
		{
			throw new NotImplementedException();
		}

		public Author Save(Author author)
		{
			throw new NotImplementedException();
		}
	}
}
