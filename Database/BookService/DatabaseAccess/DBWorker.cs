using System;
using System.Collections.Generic;
using System.Data;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace Database.BookService.DatabaseAccess
{
	public static class DBWorker
	{
		private static string ConnectionString = 
			"server=localhost;user=books;database=book_service;port=3306;password=books";


		public static void SetConfigurationFor(String conf)
		{
			if(conf.Equals("test")) {
				ConnectionString = "server=localhost;user=books;database=book_service_test;port=3306;password=books";
			} else { 
				ConnectionString = "server=localhost;user=books;database=book_service;port=3306;password=books";
			}
		}

		public static DataSet ExecuteQuery(string sqlStatement, IDictionary<string, string> args)
		{
			DataSet dataSet = new DataSet();

			using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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

		public static void ExecuteNonQuery(string sqlStatement, IDictionary<string, string> args)
		{
			using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
		public static uint InsertAndReturnId(string sqlStatement, IDictionary<string, string> args)
		{
			uint lastId;

			using (MySqlConnection connection = new MySqlConnection(ConnectionString)) 
			{
				MySqlCommand cmd = new MySqlCommand(sqlStatement, connection);
				MySqlCommand cmd1 = new MySqlCommand("SELECT LAST_INSERT_ID()", connection);

				SetParameters(cmd, args);
				

				connection.Open();
				try {
					cmd.ExecuteNonQuery();
					lastId = UInt32.Parse(cmd1.ExecuteScalar().ToString());
				} finally {
					connection.Close();
				}

				cmd.Dispose();
				cmd1.Dispose();
			}

			return lastId;
		}

		public static object ExecuteScalar(string sqlStatement, IDictionary<string, string> args)
		{
			object scalarValue = null;

			using (MySqlConnection connection = new MySqlConnection(ConnectionString)) 
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

		private static void SetParameters(MySqlCommand command, IDictionary<string, string> args)
		{
			if (args != null && args.Count != 0) {
				foreach (KeyValuePair<string, string> arg in args) {
					command.Parameters.AddWithValue(arg.Key, arg.Value);
				}
			}
		}
	}
}
