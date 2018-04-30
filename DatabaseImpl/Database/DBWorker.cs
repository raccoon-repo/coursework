using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using BookLibrary.Core.Dao;
using MySql.Data.MySqlClient;

namespace BookLibrary.Database
{
	public class DBWorker
	{
        private readonly string _connectionString;

        public const string DEFAULT_CON_STRING =
            "server=localhost;user=books;database=book_service;port=3306;password=books;SslMode=none";
        public const string TEST_CON_STRING =
            "server=localhost;user=books;database=test_book_service;port=3306;password=books;SslMode=none";

		public string ConnectionString => _connectionString;

		public DBWorker(string connectionString)
        {
            if (connectionString != null)
            {
                this._connectionString = connectionString;
            }
            else
            {
                connectionString = DEFAULT_CON_STRING;
            }
        }

		public DataSet ExecuteQuery(string sqlStatement, IDictionary<string, string> args)
		{
			var dataSet = new DataSet();

			using (var connection = new MySqlConnection(_connectionString))
			{
				connection.Open();
				var dataAdapter = new MySqlDataAdapter(sqlStatement, connection);

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
			using (MySqlConnection connection = new MySqlConnection(_connectionString))
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

			using (var connection = new MySqlConnection(_connectionString)) 
			{
				var cmd = new MySqlCommand(sqlStatement, connection);
				var cmd1 = new MySqlCommand("SELECT LAST_INSERT_ID()", connection);

				SetParameters(cmd, args);
				

				connection.Open();
				try {
					cmd.ExecuteNonQuery();
					lastId = int.Parse(cmd1.ExecuteScalar().ToString());
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

			using (var connection = new MySqlConnection(_connectionString)) 
			{
				var cmd = new MySqlCommand(sqlStatement, connection);
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
				foreach (var arg in args) {
					command.Parameters.AddWithValue(arg.Key, arg.Value);
				}
			}
		}
	}
}
