using System.Collections.Generic;

namespace BookLibrary.Entities
{
	public class Author
	{
		private int _id = 0;
		private string _firstName = "";
		private string _lastName = "";
		private IList<Book> _books = new List<Book>();

		public Author() { }
		public Author(string firstName, string lastName)
        {
			_firstName = firstName;
            _lastName = lastName;
		}

		public Author(int id, string firstName, string lastName)
		{
			_id = id;
			_lastName = lastName;
			_firstName = firstName;
		}

		public int Id
        {
			get => _id;
			set => _id = value;
		}

		public string FirstName
        {
			get => _firstName;
			set => _firstName = value;
		}

		public string LastName
        {
			get => _lastName;
			set => _lastName = value;
		}
		public virtual IList<Book> Books
		{
			get => _books;
			set => _books = value;
		}


		public virtual void AddBook(Book book)
		{
			_books.Add(book);

			if (!book.Authors.Contains(this))
            {
				book.AddAuthor(this);
			}
		}

		public virtual void RemoveBook(Book book)
		{
			_books.Remove(book);

			if(book.Authors.Contains(this)) 
			{
				book.AddAuthor(this);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Author author)
            {
				return _id == author._id && _firstName.Equals(author._firstName) && _lastName.Equals(author.LastName);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}
	}
}
