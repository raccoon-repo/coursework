using System.Collections.Generic;

namespace BookLibrary.Entities
{
	public class Book
	{
		private int id = 0;

		private string title;
		private ISet<Author> authors = new HashSet<Author>();
		private BookSection section;
		private string description;
		private float rating = 0.0f;

		public Book() { }
		public Book(string title)
        {
			this.title = title;
		}

		public int Id
        {
			get { return id; }
			set { id = value; }
		}

		public string Title {
			get { return title; }
			set { title = value; }
		}

		public BookSection Section
        {
			get { return section; }
			set { section = value; }
		}

		public string Description
        {
			get { return description; }
			set { description = value; }
		}

		public virtual ISet<Author> Authors
        {
			get { return authors; }
			set { authors = value; }
		}

		public virtual void AddAuthor(Author author)
		{
			authors.Add(author);
			if (!author.Books.Contains(this))
            {
				author.AddBook(this);
			}
		}

		public virtual void RemoveAuthor(Author author)
		{
			authors.Remove(author);
			if(author.Books.Contains(this))
            {
				author.RemoveBook(this);
			}
		}

		public float Rating {
			get { return rating; }
			set { rating = value; }
		}

		public override bool Equals(object obj)
		{
			if (obj is Book book)
            {
				return this.id == book.id && title.Equals(book.title);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}
	}
}
