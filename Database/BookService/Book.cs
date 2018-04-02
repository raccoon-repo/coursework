using Database.BookService.DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService
{
	public class Book
	{
		private uint id;
		private bool idIsSet = false;
		private bool authorsAreLoaded = false;

		private string title;
		private IList<Author> authors = new List<Author>();
		private section _section;
		private string description;

		public uint Id {
			get {
				return id;
			}
			set {
				if (idIsSet)
					throw new InvalidOperationException("Id of the book can be set only once");
				else {
					idIsSet = true;
					id = value;
				}
			}
		}

		public string Title {
			get {
				return title;
			}
			set {
				title = value;
			}
		}

		public section Section {
			get {
				return _section;
			}
			set {
				_section = value;
			}
		}

		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}

		public IList<Author> Authors
		{
			get {
				if (!authorsAreLoaded) {
					authorsAreLoaded = true;
				}

				return authors;
			}
			set {
				authors = value;
			}
		}

		public bool AuthorsAreFetched
		{
			get {
				return authorsAreLoaded;
			}
			private set { }
		}

		public enum section
		{
			FICTION, HOBBY, SELF_DEVELOPMENT, ECONOMY,
			SCIENCE, ART, FOREIGN_LANGUAGES, PROGRAMMING,
			TECHNOLOGIES, COOKERY, TRAVELS, DOCUMENTARY,
			HISTORY, OTHER
		}
	}
}
