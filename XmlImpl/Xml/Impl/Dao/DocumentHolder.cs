using System;
using System.Xml;
using System.Xml.Linq;

namespace BookLibrary.Xml.Impl.Dao
{
    public class DocumentHolder
    {
        private XmlDocument _document;
        private string _path;

        public string Path => _path;

        public XmlDocument Document => _document;

        public DocumentHolder(string path)
        {
            _path = path ?? throw new ArgumentException("Null is an inaccessible value. Specify path");
            _document = new XmlDocument();
            _document.Load(_path);
        }
    }
}