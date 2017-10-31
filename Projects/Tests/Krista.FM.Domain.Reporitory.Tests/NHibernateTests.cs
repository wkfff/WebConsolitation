using System;
using System.Collections.Generic;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using Krista.FM.Domain.Reporitory.NHibernate;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace Krista.FM.Domain.Reporitory.Tests
{
    [TestFixture]
    public class NHibernateTests
    {
        [Test]
        [Ignore]
        public void NHibernateTest()
        {
            // create our NHibernate session factory
            var sessionFactory = CreateSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    DatabaseVersions dbv = session.Get<DatabaseVersions>(100);
                    dbv.Released = DateTime.Now;
                    dbv.NeedUpdate = false;

                    transaction.Commit();

                    IList<DatabaseVersions> databaseVersions = session.CreateCriteria<DatabaseVersions>().List<DatabaseVersions>();
                    IList<D_S_TitleReport> list = session.CreateCriteria<D_S_TitleReport>().List<D_S_TitleReport>();
                }
            }
        }

        /// <summary>
        /// Configure NHibernate. This method returns an ISessionFactory instance that is
        /// populated with mappings created by Fluent NHibernate.
        /// 
        /// Line 1:   Begin configuration
        ///      2+3: Configure the database being used (SQLite file db)
        ///      4+5: Specify what mappings are going to be used (Automappings from the CreateAutomappings method)
        ///      6:   Expose the underlying configuration instance to the BuildSchema method,
        ///           this creates the database.
        ///      7:   Finally, build the session factory.
        /// </summary>
        /// <returns></returns>
        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(OracleClientConfiguration
                    .Oracle10
                    .ConnectionString(c => c.Is(
                        "DATA SOURCE=DV;PERSIST SECURITY INFO=True;USER ID=DV;Password=DV"))
                    .ShowSql())
                .Mappings(m => m.AutoMappings.Add(CreateAutomappings))
                .BuildSessionFactory();
        }

        static AutoPersistenceModel CreateAutomappings()
        {
            // This is the actual automapping - use AutoMap to start automapping,
            // then pick one of the static methods to specify what to map (in this case
            // all the classes in the assembly that contains Employee), and then either
            // use the Setup and Where methods to restrict that behaviour, or (preferably)
            // supply a configuration instance of your definition to control the automapper.
            return AutoMap.AssemblyOf<DatabaseVersions>(new ExampleAutomappingConfiguration())
                .Conventions.Add<CascadeConvention>()
                .Conventions.Add<TableConvention>();
        }

        private static void BuildSchema(Configuration config)
        {
            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
                .Create(false, true);
        }

    }

    class TableConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            instance.Table(instance.EntityType.Name.ToUpper());
        }
    }

    /// <summary>
    /// This is a convention that will be applied to all entities in your application. What this particular
    /// convention does is to specify that many-to-one, one-to-many, and many-to-many relationships will all
    /// have their Cascade option set to All.
    /// </summary>
    class CascadeConvention : IReferenceConvention, IHasManyConvention, IHasManyToManyConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.Cascade.All();
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Cascade.All();
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Cascade.All();
        }
    }

    /// <summary>
    /// This is an example automapping configuration. You should create your own that either
    /// implements IAutomappingConfiguration directly, or inherits from DefaultAutomappingConfiguration.
    /// Overriding methods in this class will alter how the automapper behaves.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    class ExampleAutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            // specify the criteria that types must meet in order to be mapped
            // any type for which this method returns false will not be mapped.
            return type.Namespace == "Krista.FM.Domain" && type.BaseType.Name == "DomainObject";
        }

        public override bool IsComponent(Type type)
        {
            // override this method to specify which types should be treated as components
            // if you have a large list of types, you should consider maintaining a list of them
            // somewhere or using some form of conventional and/or attribute design
            return type == typeof(D_S_TestObject);
        }
    }
}
