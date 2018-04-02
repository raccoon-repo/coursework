using System;
using System.Collections.Generic;
using System.Data;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace Database.BookService.DatabaseAccess
{
	public static class DBWorker
	{
		public const string ConnectionString = 
			"server=localhost;user=books;database=book_service;port=3306;password=books";


		public static DataSet ExecuteQuery(string sqlStatement, IDictionary<string, string> args)
		{
			DataSet dataSet = new DataSet();

			using (MySqlConnection connection = new MySqlConnection(ConnectionString))
			{
				connection.Open();
				MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlStatement, connection);
				
				if (args != null) {
					foreach (KeyValuePair<string, string> arg in args) {
						dataAdapter.SelectCommand.Parameters.AddWithValue(arg.Key, arg.Value);
					}
				}
				try {
					dataAdapter.Fill(dataSet);
				} finally {
					connection.Close();
				}
				dataAdapter.Dispose();
			}

			return dataSet;
		}

		public static void ExecuteNonQuery(string sqlStatement, IDictionary<string, string> args)
		{
			using (MySqlConnection connection = new MySqlConnection(ConnectionString))
			{
				MySqlCommand cmd = new MySqlCommand(sqlStatement, connection);

				if (args != null) {
					foreach (KeyValuePair<string, string> arg in args) {
						cmd.Parameters.AddWithValue(arg.Key, arg.Value);
					}
				}
				connection.Open();
				try {
					cmd.ExecuteNonQuery();
				} finally {
					connection.Close();
				}
				cmd.Dispose();
			}
		}
	}
}
