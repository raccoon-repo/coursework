using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace BookLibrary.Database
{
	public class DBWorker
	{
        private string connectionString;

        public const string DEFAULT_CON_STRING =
            "server=localhost;user=books;database=book_service;port=3306;password=books";
        public const string TEST_CON_STRING =
            "server=localhost;user=books;database=test_book_service;port=3306;password=books";


        public DBWorker(string connectionString)
        {
            if (connectionString != null)
            {
                this.connectionString = connectionString;
            }
            else
            {
                connectionString = DEFAULT_CON_STRING;
            }
        }

		public DataSet ExecuteQuery(string sqlStatement, IDictionary<string, string> args)
		{
			DataSet dataSet = new DataSet();

			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				connection.Open();
				MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlStatement, connection);

				SetParameters(dataAdapter.SelectCommand, args);

				try {
					dataAdapter.Fill(dataSet);
				} finally {
					connection.Close();
				}
				dataAdapter.Dispose();
			}

			return dataSet;
		}

		public void ExecuteNonQuery(string sqlStatement, IDictionary<string, string> args)
		{
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				MySqlCommand cmd = new MySqlCommand(sqlStatement, connection);

				SetParameters(cmd, args);

				connection.Open();
				try {
					cmd.ExecuteNonQuery();
				} finally {
					connection.Close();
				}
				cmd.Dispose();
			}
		}


		//return id of the inserted row
		public int InsertAndReturnId(string sqlStatement, IDictionary<string, string> args)
		{
			int lastId;

			using (MySqlConnection connection = new MySqlConnection(connectionString)) 
			{
				MySqlCommand cmd = new MySqlCommand(sqlStatement, connection);
				MySqlCommand cmd1 = new MySqlCommand("SELECT LAST_INSERT_ID()", connection);

				SetParameters(cmd, args);
				

				connection.Open();
				try {
					cmd.ExecuteNonQuery();
					lastId = Int32.Parse(cmd1.ExecuteScalar().ToString());
				} finally {
					connection.Close();
				}

				cmd.Dispose();
				cmd1.Dispose();
			}

			return lastId;
		}

		public object ExecuteScalar(string sqlStatement, IDictionary<string, string> args)
		{
			object scalarValue = null;

			using (MySqlConnection connection = new MySqlConnection(connectionString)) 
			{
				MySqlCommand cmd = new MySqlCommand(sqlStatement, connection);
				SetParameters(cmd, args);

				connection.Open();
				try {
					scalarValue = cmd.ExecuteScalar();
				} finally {
					connection.Close();
				}

				cmd.Dispose();
			}

			return scalarValue;
		}

		private void SetParameters(MySqlCommand command, IDictionary<string, string> args)
		{
			if (args != null && args.Count != 0) {
				foreach (KeyValuePair<string, string> arg in args) {
					command.Parameters.AddWithValue(arg.Key, arg.Value);
				}
			}
		}
	}
}
