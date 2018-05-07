using BookLibrary.Xml.Impl.Dao;
using System;

namespace BookLibrary.Xml.Utils.Impl
{
    public class BookCounter : IBookCounter
    {
        private DocumentHolder _documentHolder;

        public BookCounter(string documentPath)
        {
            _documentHolder = new DocumentHolder(documentPath);
        }

        public BookCounter(DocumentHolder document)
        {
            _documentHolder = document;
        }

        public void Increment(int bookId)
        {
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var countNode = root.SelectSingleNode("entry[bookId = '" + bookId + "']/count");
            int value = int.Parse(countNode.FirstChild.Value) + 1;
            countNode.FirstChild.Value = value.ToString();
            xDoc.Save(_documentHolder.Path);

        }

        public void Decrement(int bookId)
        {
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var countNode = root.SelectSingleNode("entry[bookId ='" + bookId + "']/count");
            int value = int.Parse(countNode.FirstChild.Value);

            if (value == 0)
                return;

            countNode.FirstChild.Value = (value - 1).ToString();

            xDoc.Save(_documentHolder.Path);
        }

        public int Count(int bookId)
        {
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var countNode = root.SelectSingleNode("entry[bookId ='" + bookId + "']/count");

            return int.Parse(countNode.FirstChild.Value);
        }


        public void SetCount(int bookId, int count)
        {
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var countNode = root.SelectSingleNode("entry[bookId = '" + bookId + "']/count");

            countNode.FirstChild.Value = count.ToString();

            xDoc.Save(_documentHolder.Path);

        }

        public void AddNew(int bookId, int count)
        {
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var entryNode = xDoc.CreateElement("entry");
            var bookIdNode = xDoc.CreateElement("bookId");
            bookIdNode.AppendChild(xDoc.CreateTextNode(bookId.ToString()));

            var countNode = xDoc.CreateElement("count");
            countNode.AppendChild(xDoc.CreateTextNode(count.ToString()));

            entryNode.AppendChild(bookIdNode);
            entryNode.AppendChild(countNode);
            root.AppendChild(entryNode);

            xDoc.Save(_documentHolder.Path);
        }

        public void Remove(int bookId)
        {
            var xDoc = _documentHolder.Document;
            var root = xDoc.DocumentElement;

            var entryNode = root.SelectSingleNode("entry[bookId = '" + bookId + "']");
            if (entryNode is null)
                return;

            root.RemoveChild(entryNode);

            xDoc.Save(_documentHolder.Path);

        }
    }
}
