using System;
using System.Collections.Generic;
using BookLibrary.Core.Dao;
using BookLibrary.Entities;

namespace BookLibrary.Xml.Impl.Dao
{
    public class XmlAuthorDao : IAuthorDao
    {
        private XmlBookDao _xmlBookDao;
        private DocumentHolder _documentHolder;

        public DocumentHolder DocumentHolder
        {
            get => _documentHolder;
            set => _documentHolder = value;
        }

        public XmlBookDao BookDao
        {
            get => _xmlBookDao;
            set => _xmlBookDao = value;
        }
        
        public XmlAuthorDao() { }

        public XmlAuthorDao(DocumentHolder documentHolder)
        {
            _documentHolder = documentHolder ?? 
                 throw new ArgumentException("Specify documentHolder");


        }

        public Author FindById(int id)
        {
            throw new System.NotImplementedException();
        }

        public IList<Author> FindAll()
        {
            throw new System.NotImplementedException();
        }

        public IList<Author> FindByName(string firstName, string lastName)
        {
            throw new System.NotImplementedException();
        }

        public IList<Author> FindByBook(Book book)
        {
            throw new System.NotImplementedException();
        }

        public void Save(Author author, SaveOption option)
        {
            throw new System.NotImplementedException();
        }

        public void Save(Author author, SaveOption option, ISet<int> savedAuthor, ISet<int> savedBooks)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Author author)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Author author, ISet<int> updatedAuthors, ISet<int> updatedBooks)
        {
            throw new System.NotImplementedException();
        }

        public Author Refresh(Author author)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(Author author)
        {
            throw new System.NotImplementedException();
        }
    }
}