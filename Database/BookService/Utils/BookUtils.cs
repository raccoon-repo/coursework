using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService.Utils
{
	public static class BookUtils
	{
		public static Book.section ParseSection(string section)
		{
			section = section.ToUpper().Trim();
			Book.section bs;
			bool parsingResult = Enum.TryParse(section, out bs);

			if (!parsingResult) {
				bs = Book.section.OTHER;
			}

			return bs;
		}
	}
}
