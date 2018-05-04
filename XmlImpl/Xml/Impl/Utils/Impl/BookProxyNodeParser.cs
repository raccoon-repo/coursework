using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using BookLibrary.Core.Dao;
using BookLibrary.Entities;
using BookLibrary.Entities.Proxies;

namespace BookLibrary.Xml.Impl.Utils.Impl
{
    
    /* Responsible for parsing XmlNode
     * into BookPoxy instance
     */
    public class BookProxyNodeParser<T> : INodeParser<Book>
    {

        public IAuthorDao AuthorDao { get; set; } 

        public Book Parse(XmlNode bookNode)
        {
            var title = bookNode.SelectSingleNode("./title")?.FirstChild?.Value;
            var description = bookNode.SelectSingleNode("./description")?.FirstChild?.Value;
            var sectionValue = bookNode.SelectSingleNode("./section")?.FirstChild?.Value;
            var idValue = bookNode.Attributes?["id"].Value;
            var ratingValue = bookNode.SelectSingleNode("./rating")?.FirstChild?.Value;

            if (idValue == null)
            {
                return null;
            }


            var id = int.Parse(idValue);
            var rating = ratingValue == null ? 1.0f : float.Parse(ratingValue);
            var section = BookLibrary.BookUtils.ParseSection(sectionValue);

            return new BookProxy(id, title, rating, section, description)
            {
                AuthorDao = this.AuthorDao
            };
        }

        public IList<Book> ParseNodes(XmlNodeList nodes)
        {
            IList<Book> books = new List<Book>();
            foreach (XmlNode bookNode in nodes)
                books.Add(Parse(bookNode));

            return books;
        }
    }
}