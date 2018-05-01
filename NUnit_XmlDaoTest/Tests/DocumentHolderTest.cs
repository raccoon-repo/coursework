using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using BookLibrary.Xml.Impl.Dao;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NUnit_XmlDaoTest.Tests
{
    [TestFixture]
    public class DocumentHolderTest
    {
        private static string _projectDir = GetProjectDirectory();
        
        [Test]
        public void Should_Retrieve_Last_Inserted_Id_And_Increment_It()
        {
            var dataPath = _projectDir + "/Resources/DHTest/data.xml";
            var metaPath = _projectDir + "/Resources/DHTest/.meta-inf.xml";
            
            var dh = new DocumentHolder(dataPath, metaPath);
            var lastInsertedId = dh.GetLastInsertedId();
            
            Assert.AreEqual(1, lastInsertedId);
            
            dh.IncrementLastId();
            
            Assert.AreEqual(lastInsertedId + 1, dh.GetLastInsertedId());            
        }

        
        private static string GetProjectDirectory()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Regex regex = new Regex("/bin/Debug$");
            return regex.Replace(dir, "");
        }
    }
}