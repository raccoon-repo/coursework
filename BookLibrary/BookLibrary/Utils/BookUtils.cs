using System;
using BookLibrary.Entities;

namespace BookLibrary
{
	public static class BookUtils
	{
		public static BookSection ParseSection(string section)
		{
			section = section.ToUpper().Trim();
			bool parsingResult = Enum.TryParse(section, out BookSection bs);

			if (!parsingResult) {
				bs = BookSection.OTHER;
			}

			return bs;
		}
	}
}
