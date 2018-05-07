using BookLibrary.Entities;
using BookLibrary.Xml.Utils;
using System.Collections.Generic;


namespace Wpf.Appl.DTO
{
    public class BookDtoConverter
    {
        public IBookCounter BookCounter { get; set; }

        public BookDto ConvertBook(Book book)
        {
            return new BookDto() {
                Id = book.Id,
                Title = book.Title,
                Rating = book.Rating,
                Quantity = BookCounter.Count(book.Id)
            };
        }

        public IList<BookDto> ConvertBooks(IList<Book> books)
        {
            IList<BookDto> bookDtos = new List<BookDto>();

            foreach (var book in books)
                bookDtos.Add(ConvertBook(book));

            return bookDtos;
        }
    }
}
