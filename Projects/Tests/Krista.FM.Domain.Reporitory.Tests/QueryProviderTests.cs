using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Krista.FM.Domain.Reporitory.Tests.MyQueryProvider;
using NUnit.Framework;

namespace Krista.FM.Domain.Reporitory.Tests
{
    [TestFixture]
    public class QueryProviderTests
    {
        [Test]
        public void QueryProviderTest()
        {
            string constr = @"Password=dv;Persist Security Info=True;User ID=DVVer27;Data Source=fmserv\mssql2005";
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                QueryProvider provider = new DbQueryProvider(con);
/*
                IQueryable<DatabaseVersions> databaseVersions = new Query<DatabaseVersions>(provider);

                string version = "2.7.0.0";
                IQueryable<DatabaseVersions> query = databaseVersions.Where(c => c.Name == version);

                Console.WriteLine("Query:\n{0}\n", query);

                var list = query.ToList();
                foreach (var item in list)
                {
                    Console.WriteLine("Name: {0}", item.Name);
                }
                */
                Console.ReadLine();
            }            
        }
    }

    /*public class DatabaseVersions
    {
        public int ID;
        public string Name;
        public DateTime Released;
        public DateTime Updated;
        public string Comments;
    }*/
}
