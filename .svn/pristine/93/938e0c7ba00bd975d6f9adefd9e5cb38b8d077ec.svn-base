using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain.Reporitory.NHibernate;
using NUnit.Framework;

namespace Krista.FM.Domain.Reporitory.Tests
{
    [TestFixture]
    public class NHibernateRepositoryTests
    {
        [SetUp]
        public void Setup()
        {
            // create our NHibernate session factory
            //NHibernateSession.SessionFactory = CreateSessionFactory();
        }

        [Test]
        [Ignore]
        public void NHibernateTest()
        {
            NHibernateRepository<DatabaseVersions> repository = new NHibernateRepository<DatabaseVersions>();
            var db = repository.GetAll();
            DatabaseVersions dbv = new DatabaseVersions()
                                       {
                                           Name = "2.6.0.0", 
                                           Comments = "Test", 
                                           NeedUpdate = false,
                                           Released = DateTime.Now,
                                           Updated = DateTime.Now
                                       };
            repository.Save(dbv);
            repository.DbContext.CommitChanges();
            db = repository.GetAll();
        }
    }
}
