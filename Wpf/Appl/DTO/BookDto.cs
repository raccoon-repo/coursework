using BookLibrary.Entities;

namespace Wpf.Appl.DTO
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public float Rating { get; set; }
        public string Section { get; set; }
        public int Quantity { get; set; }
        public int Shelf { get; set; }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is BookDto dto)
                return Id == dto.Id && Title.Equals(dto.Title) &&
                       Rating == dto.Rating && Section.Equals(dto.Section) &&
                       Quantity == dto.Quantity;

            return false;
        }
    }
}
