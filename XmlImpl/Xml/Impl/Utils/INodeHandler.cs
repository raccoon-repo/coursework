using System.Xml;

namespace BookLibrary.Xml.Impl.Utils
{
    /* Responsible for handling information about
     * books inside author node and vice versa
     *
     * Also it has some utility methods
     */
    public interface INodeHandler
    {
        // find book node by id 
        // add id of the author to the node
        void AddAuthorToBook(int bookId, int authorId);
        
        // find book node by id
        // remove id of the author from the node
        void RemoveAuthorFromBook(int bookId, int authorId);

        void AddBookToAuthor(int bookId, int authorId);
        void RemoveBookFromAuthor(int bookId, int authorId);

        XmlNode GetNodeById(XmlNode root, int id);
        void AppendText(XmlDocument xDoc, XmlNode parent, string text);
    }
}