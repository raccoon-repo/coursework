using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlImpl.Xml.Utils
{
    /**
     * Responsible for arranging books 
     * on shelves
     */
    public interface IBookArranger
    {
        bool IsPresent(int bookId);
        void Set(int shelf, int bookId);
        int GetShelf(int bookId);

        void AddEntry(int bookId, int shelf);
        void DeleteEntry(int bookId);
    }
}
