using BookLibrary.Entities;

namespace BookLibrary.Xml.Utils
{
    public interface IBookCounter
    {
        int Count(int bookId);
        void SetCount(int bookId, int count);

        void AddNew(int bookId, int count);
        void Remove(int bookId);

        void Increment(int bookId);
        void Decrement(int bookId);
    }
}
