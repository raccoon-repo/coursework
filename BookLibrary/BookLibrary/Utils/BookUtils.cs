using System;
using BookLibrary.Entities;

namespace BookLibrary.Utils
{
	public static class BookUtils
	{
		public static BookSection ParseSection(string section)
		{
			section = section.ToUpper().Trim();
			BookSection bs;
			bool parsingResult = Enum.TryParse(section, out bs);

			if (!parsingResult) {
				bs = BookSection.OTHER;
			}

			return bs;
		}
	}
}
