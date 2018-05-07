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
    }
}
