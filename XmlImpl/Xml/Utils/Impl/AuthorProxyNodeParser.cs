using System;
using System.Collections.Generic;
using System.Xml;
using BookLibrary.Core.Dao;
using BookLibrary.Entities;
using BookLibrary.Entities.Proxies;

namespace BookLibrary.Xml.Utils.Impl
{
    
    /* Responsible for parsing XmlNode into
     * AuthorProxy instance
     */
    public class AuthorProxyNodeParser<T> : INodeParser<Author>
    {

        public IBookDao BookDao { get; set; }

        public Author Parse(XmlNode authorNode)
        {
            
            if (authorNode is null)
                throw new ArgumentException("authorNode cannot be null");

            var id = int.Parse(authorNode.Attributes["id"].Value);
            var firstName = authorNode.SelectSingleNode("./firstName")
                ?.FirstChild.Value;

            var lastName = authorNode.SelectSingleNode("./lastName")
                ?.FirstChild.Value;

            var proxy = new AuthorProxy(id, firstName, lastName) {
                BookDao = this.BookDao
            };

            return proxy;
        }

        public IList<Author> ParseNodes(XmlNodeList nodes)
        {
            if (nodes is null)
                throw new ArgumentException("nodes cannot be null");
            
            IList<Author> authors = new List<Author>();

            foreach (XmlNode authorNode in nodes)
                authors.Add(Parse(authorNode));

            return authors;
        }
    }
}