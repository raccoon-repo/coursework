using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace BookLibrary.Xml.Impl.Dao
{
    public class DocumentHolder
    {
        private static string _argExcMessage =
            "Null is an inaccessible value. Specify the path";

        private static string _metaInfArgExc =
            "Null is an inaccessible value. Specify the path of meta information";
        
        private XmlDocument _document;
        private XmlDocument _metaInf;
        private string _path;
        private string _metaInfPath;

        public string Path => _path;

        public XmlDocument Document => _document;

        public DocumentHolder(string path)
        {
            _path = path ?? throw new ArgumentException(_argExcMessage);
            _document = new XmlDocument();
            _document.Load(_path);            
        }

        /*
         * metaInfPath is a path to an xml file that contains
         * meta information such as last inserted identity into
         * the document with data
         */
        public DocumentHolder(string path, string metaInfPath)
        {
            _path = path ?? throw new ArgumentException(_argExcMessage);
            _metaInfPath = metaInfPath ?? throw new ArgumentException(_metaInfArgExc);
            
            _document = new XmlDocument();
            _metaInf = new XmlDocument();
            
            _document.Load(_path);
            _metaInf.Load(_metaInfPath);
        }

        public int GetLastInsertedId()
        {
            if (_metaInf is null)
                throw new InvalidOperationException("meta-information file " +
                                          "is not specified. Can't get last id");

            var lastIdStr = _metaInf.SelectSingleNode("//lastInsertedId")?.FirstChild?.Value;
            
            return int.Parse(lastIdStr);
        }

        public void IncrementLastId()
        {
            if (_metaInf is null)
                throw new InvalidOperationException("meta-information file is not " +
                                          "specified. Can't increment last inserted id");

            int lastId = GetLastInsertedId() + 1;
            _metaInf.SelectSingleNode("//lastInsertedId").FirstChild.Value = lastId.ToString();
            _metaInf.Save(_metaInfPath);
        }

        public override bool Equals(object obj)
        {
            if (obj == null | !(obj is DocumentHolder))
                return false;

            var dh = (DocumentHolder) obj;

            return dh._metaInfPath == _metaInfPath && dh._path == _path;
        }

        public override int GetHashCode()
        {
            return (_path + _metaInf).GetHashCode();
        }
    }
}