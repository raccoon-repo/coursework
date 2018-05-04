using System.Collections.Generic;
using System.Xml;

namespace BookLibrary.Xml.Impl.Utils
{
    public interface INodeParser<T>
    {
        T Parse(XmlNode node);
        IList<T> ParseNodes(XmlNodeList nodes);
    }
}