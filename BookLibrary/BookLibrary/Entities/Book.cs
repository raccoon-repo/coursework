using System.Collections.Generic;

namespace BookLibrary.Entities
{
	public class Book
	{
		private int _id = 0;

		private string _title;
		private ISet<Author> _authors = new HashSet<Author>();
		private BookSection _section;
		private string _description;
		private float _rating = 0.0f;

		public Book() { }
		public Book(string title)
        {
			this._title = title;
		}

		public int Id
        {
			get => _id;
			set => _id = value;
		}

		public string Title {
			get => _title;
			set => _title = value;
		}

		public BookSection Section
        {
			get => _section;
			set => _section = value;
		}

		public string Description
        {
			get => _description;
			set => _description = value;
		}

		public float Rating 
		{
			get => _rating;
			set => _rating = value;
		}

		public virtual ISet<Author> Authors
        {
			get => _authors;
			set => _authors = value;
		}

		public virtual void AddAuthor(Author author)
		{
			_authors.Add(author);
			if (!author.Books.Contains(this))
            {
				author.AddBook(this);
			}
		}

		public virtual void RemoveAuthor(Author author)
		{
			_authors.Remove(author);
			if(author.Books.Contains(this))
            {
				author.RemoveBook(this);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Book book)
            {
				return this._id == book._id && _title.Equals(book._title);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}
	}
}
