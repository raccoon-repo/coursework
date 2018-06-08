using BookLibrary.Xml.Impl.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlImpl.Xml.Utils.Impl
{
    public class BookArranger : IBookArranger
    {
        private DocumentHolder _documentHolder;


        /**
         * path - path to the document that
         * stores records with position of books
         * on shelves.
         * 
         * The document has specified format:
         * 
         * <books>
         *     <entry>
         *         <bookId></bookId>
         *         <shelf></shelf>
         *     </entry>
         * </books>
         */
        public BookArranger(string path)
        {
            _documentHolder = new DocumentHolder(path);
        }

        public int GetShelf(int bookId)
        {
            if (!IsPresent(bookId))
                return -1;

            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var countNode = root.SelectSingleNode("entry[bookId='" + bookId + "']/shelf");
            if (countNode == null || countNode.FirstChild == null)
                return -1;

            var count = int.Parse(countNode.FirstChild.Value);
            return count;
        }

        public void Set(int shelf, int bookId)
        {
            if (!IsPresent(bookId) || shelf < 0 || bookId < 0)
                return;

            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var res = root.SelectSingleNode("entry[bookId='" + bookId + "']/shelf");
            res.FirstChild.Value = shelf.ToString();

            xDoc.Save(_documentHolder.Path);

        }

        public bool IsPresent(int bookId)
        {
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var res = root.SelectSingleNode("entry[bookId='" + bookId + "']");

            return root.SelectSingleNode("entry[bookId='" + bookId + "']") != null;
        }

        private void CreateEntry(int bookId, int shelf)
        {
            if (IsPresent(bookId))
                return;

            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var entryElement = xDoc.CreateElement("entry");
            var bookIdNode = xDoc.CreateElement("bookId");
            bookIdNode.AppendChild(xDoc.CreateTextNode(bookId.ToString()));

            var shelfNode = xDoc.CreateElement("shelf");
            shelfNode.AppendChild(xDoc.CreateTextNode(shelf.ToString()));

            entryElement.AppendChild(bookIdNode);
            entryElement.AppendChild(shelfNode);
            root.AppendChild(entryElement);

            xDoc.Save(_documentHolder.Path);

        }

        public void AddEntry(int bookId, int shelf)
        {
            if (IsPresent(bookId) || shelf < 0 || bookId < 0)
                return;

            CreateEntry(bookId, shelf);

        }

        public void DeleteEntry(int bookId)
        {
            if (!IsPresent(bookId))
                return;

            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var res = root.SelectSingleNode("entry[bookId='" + bookId + "']");
            root.RemoveChild(res);

            xDoc.Save(_documentHolder.Path);
        }
    }
}
