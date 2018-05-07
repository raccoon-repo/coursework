using System.Xml;
using BookLibrary.Xml.Impl.Dao;

namespace BookLibrary.Xml.Utils.Impl
{
    public class NodeHandler : INodeHandler
    {
        private DocumentHolder _authors;
        
        private DocumentHolder _books;
        
        public static INodeHandler GetNodeHandlerFor(DocumentHolder books, DocumentHolder authors)
        {
            return new NodeHandler(books, authors);
        }

        private NodeHandler(DocumentHolder books, DocumentHolder authors)
        {
            _authors = authors;
            _books = books;
        }
        
        public void AddAuthorToBook(int bookId, int authorId)
        {
            var xDoc = _books.Document;
            var bookNode = GetBookNodeById(bookId);

            var authorNodes = bookNode?.SelectSingleNode("./authors");

            var idNode = xDoc.CreateElement("id");
            idNode.AppendChild(xDoc.CreateTextNode(authorId.ToString()));
            authorNodes.AppendChild(idNode);
            
            xDoc.Save(_books.Path);
        }

        public void RemoveAuthorFromBook(int bookId, int authorId)
        {
            var xDoc = _books.Document;
            var root = xDoc.DocumentElement;
            
            var bookNode = root.SelectSingleNode("book[@id = '" + bookId + "']");
            var authorsNode = bookNode?.SelectSingleNode("./authors");
            var authorNode = bookNode?.SelectSingleNode("./authors/id[. = '" + authorId + "']");
            
            if (authorsNode is null || authorNode is null)
                return;

            authorsNode.RemoveChild(authorNode);
            
            xDoc.Save(_books.Path);
        }

        public void AddBookToAuthor(int bookId, int authorId)
        {
            var xDoc = _authors.Document;
            var authorNode = GetAuthorNodeById(authorId);

            var booksNode = authorNode?.SelectSingleNode("./books");

            var idNode = _authors.Document.CreateElement("id");
            idNode.AppendChild(_authors.Document.CreateTextNode(bookId.ToString()));
            booksNode.AppendChild(idNode);
            
            xDoc.Save(_authors.Path);
        }

        public void RemoveBookFromAuthor(int bookId, int authorId)
        {
            var xDoc = _authors.Document;
            var root = xDoc.DocumentElement;
            var authorNode = GetNodeById(root, authorId);

            var booksNode = authorNode?.SelectSingleNode("./books");
            var bookNode = authorNode?.SelectSingleNode("./books/id[. ='" + bookId + "']");
            
            if (booksNode is null || bookNode is null)
                return;

            booksNode.RemoveChild(bookNode);
            
            xDoc.Save(_authors.Path); 
        }

        public void AppendText(XmlDocument xDoc, XmlNode parent, string text)
        {
            if (!parent.HasChildNodes)
                parent.AppendChild(xDoc.CreateTextNode(text));
            else
                parent.FirstChild.Value = text;
        }

        public XmlNode GetNodeById(XmlNode root, int id)
        {
            var node = root.SelectSingleNode("./*[@id = '" + id + "']");

            return node;
        }

        private XmlNode GetAuthorNodeById(int id)
        {
            var root = _authors.Document.DocumentElement;

            return GetNodeById(root, id);
        }

        private XmlNode GetBookNodeById(int id)
        {
            var root = _books.Document.DocumentElement;

            return GetNodeById(root, id);
        }
    }
}