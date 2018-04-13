﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService
{
	public class Author
	{
		private int id = 0;
		private string firstName = "";
		private string lastName = "";
		private ISet<Book> books = new HashSet<Book>();

		public Author() { }
		public Author(string firstName) {
			this.firstName = firstName;
		}

		public int Id {
			get { return id; }
			set { id = value; }
		}

		public string FirstName {
			get { return firstName; }
			set { firstName = value; }
		}
		public string LastName {
			get { return lastName; }
			set { lastName = value;  }
		}
		public virtual ISet<Book> Books
		{
			get {
				return books;
			}
			set {
				books = value;
			}
		}


		public void AddBook(Book book)
		{
			books.Add(book);

			if (!book.Authors.Contains(this)) {
				book.AddAuthor(this);
			}
		}

		public void RemoveBook(Book book)
		{
			books.Remove(book);

			if(book.Authors.Contains(this)) {
				book.AddAuthor(this);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Author author) {
				return id == author.id && firstName.Equals(author.firstName) && lastName.Equals(author.LastName);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}
	}
}
