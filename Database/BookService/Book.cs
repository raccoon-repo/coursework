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
		private uint id = 0;

		private string title;
		private ISet<Author> authors;
		private section _section;
		private string description;
		private float rating;

		public virtual uint Id {
			get {
				return id;
			}
			set {
				id = value;
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

		public virtual ISet<Author> Authors
		{
			get {
				return authors;
			}
			set {
				authors = value;
			}
		}

		public float Rating { get; set; }

		public enum section
		{
			FICTION, HOBBY, SELF_DEVELOPMENT, ECONOMY,
			SCIENCE, ART, FOREIGN_LANGUAGES, PROGRAMMING,
			TECHNOLOGIES, COOKERY, TRAVELS, DOCUMENTARY,
			HISTORY, OTHER
		}
	}
}
