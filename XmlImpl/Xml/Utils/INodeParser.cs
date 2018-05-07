using System.Collections.Generic;
using System.Xml;

namespace BookLibrary.Xml.Utils
{
    public interface INodeParser<T>
    {
        T Parse(XmlNode node);
        IList<T> ParseNodes(XmlNodeList nodes);
    }
}