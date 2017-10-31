using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core.Tests.Helpers;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine;
using Krista.FM.RIA.Extensions.Consolidation.Tests.Helpers;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class DatabaseObjectNameResolverTests
    {
        private MockRepository mocks;
        private IScheme scheme;
        private ISchemeDWH dwh;
        private IDatabase db;
        private List<HashObjectsNames> data;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            scheme = mocks.DynamicMock<IScheme>();
            dwh = mocks.DynamicMock<ISchemeDWH>();
            db = mocks.DynamicMock<IDatabase>();

            data = new List<HashObjectsNames>
            {
                new HashObjectsNames { HashName = "I_F_S_CREDITISSUEDRE$W9x3gxI$1", LongName = "I_F_S_CREDITISSUEDREFSCREDITPENALTY", ObjectType = (int)ObjectTypes.ForeignKeysConstraint }
            };

            MockHttpContext context = new MockHttpContext(false);
            HttpContext.Current = context.Context;
        }

        [Ignore]
        [Test]
        public void Test()
        {
            var dt = DataTableHelper.CreateDataTable(data);
            var list = dt.Rows.Cast<DataRow>().Where(x => x["Name"].Equals("The string"));

            var serverScheme = ServerSchemeConnectionHelper.Connect("fmserv:8008", "krista2\\gbelov");

            var resolver = new DatabaseObjectHashNameResolver(serverScheme);
            var result = resolver.Get("I_F_S_CREDITISSUEDREFSCREDITPENALTY", ObjectTypes.ForeignKeysConstraint);
            result = resolver.Get("I_F_S_CREDITISSUEDREFSCREDITPENALTY", ObjectTypes.ForeignKeysConstraint);
        }

        [Test]
        public void CreateWithShortNameTest()
        {
            Expect.Call(scheme.SchemeDWH).Repeat.Never();

            mocks.ReplayAll();

            var resolver = new DatabaseObjectHashNameResolver(scheme);
            var result = resolver.Create("I_F_S_CREDITISSUEDREFSCREDITPE", ObjectTypes.ForeignKeysConstraint);

            mocks.VerifyAll();

            Assert.AreEqual("I_F_S_CREDITISSUEDREFSCREDITPE", result);
        }

        [Test]
        public void CreateWithLongNameTest()
        {
            Expect.Call(scheme.SchemeDWH).Return(dwh);
            Expect.Call(dwh.DB).Return(db);
            Expect.Call(db.Dispose);

            Expect.Call(db.ExecQuery(String.Empty, QueryResultTypes.DataTable))
                .Callback((string q, QueryResultTypes t, IDbDataParameter[] p) => q.StartsWith("select"))
                .Return(new DataTable()).Repeat.Once();

            Expect.Call(db.ExecQuery(String.Empty, QueryResultTypes.DataTable))
                .Callback((string q, QueryResultTypes t, IDbDataParameter[] p) => q.StartsWith("insert"))
                .Return(1).Repeat.Once();

            mocks.ReplayAll();

            var resolver = new DatabaseObjectHashNameResolver(scheme);
            var result = resolver.Create("I_F_S_CREDITISSUEDREFSCREDITPENALTY", ObjectTypes.ForeignKeysConstraint);

            mocks.VerifyAll();

            Assert.AreEqual("\"I_F_S_CREDITISSUEDRE$W9x3gxI$0\"", result);
        }

        [Test]
        public void CreateWithLongNameCollisionTest()
        {
            Expect.Call(scheme.SchemeDWH).Return(dwh);
            Expect.Call(dwh.DB).Return(db);
            Expect.Call(db.Dispose);

            Expect.Call(db.ExecQuery(String.Empty, QueryResultTypes.DataTable))
                .Callback((string q, QueryResultTypes t, IDbDataParameter[] p) => q.StartsWith("select"))
                .Return(new DataTable()).Repeat.Once();

            int callCount = 0;
            Func<string, QueryResultTypes, IDbDataParameter[], bool> callback2 = delegate(string query, QueryResultTypes resultType, IDbDataParameter[] parameters)
                {
                    callCount++;
                    if (callCount == 1 && query.StartsWith("insert"))
                    {
                        throw new InvalidOperationException(String.Empty, new Exception("ORA-00001"));
                    }

                    return callCount == 2 && query.StartsWith("insert");
                };
            Expect.Call(db.ExecQuery(String.Empty, QueryResultTypes.DataTable))
                .Callback(callback2).Return(1).Repeat.Once();

            mocks.ReplayAll();

            var resolver = new DatabaseObjectHashNameResolver(scheme);
            var result = resolver.Create("I_F_S_CREDITISSUEDREFSCREDITPENALTY", ObjectTypes.ForeignKeysConstraint);

            mocks.VerifyAll();

            Assert.AreEqual("\"I_F_S_CREDITISSUEDRE$W9x3gxI$1\"", result);
        }

        [Test]
        [ExpectedException(typeof(DatabaseObjectHashNameException))]
        public void CreateWithExistLongNameTest()
        {
            Expect.Call(scheme.SchemeDWH).Return(dwh);
            Expect.Call(dwh.DB).Return(db);
            Expect.Call(db.Dispose);

            Expect.Call(db.ExecQuery(String.Empty, QueryResultTypes.DataTable))
                .Callback((string q, QueryResultTypes t, IDbDataParameter[] p) => q.StartsWith("select"))
                .Return(DataTableHelper.CreateDataTable(data)).Repeat.Once();

            mocks.ReplayAll();

            var resolver = new DatabaseObjectHashNameResolver(scheme);
            resolver.Create("I_F_S_CREDITISSUEDREFSCREDITPENALTY", ObjectTypes.ForeignKeysConstraint);

            mocks.VerifyAll();
        }

        [Test]
        public void GetWithShortNameTest()
        {
            Expect.Call(scheme.SchemeDWH).Repeat.Never();

            mocks.ReplayAll();

            var resolver = new DatabaseObjectHashNameResolver(scheme);
            var result = resolver.Get("I_F_S_CREDITISSUEDREFSCREDITPE", ObjectTypes.ForeignKeysConstraint);

            mocks.VerifyAll();

            Assert.AreEqual("I_F_S_CREDITISSUEDREFSCREDITPE", result);
        }

        [Test]
        public void GetWithLongNameCollisionTest()
        {
            Expect.Call(scheme.SchemeDWH).Return(dwh);
            Expect.Call(dwh.DB).Return(db);
            Expect.Call(db.Dispose);

            Expect.Call(db.ExecQuery(String.Empty, QueryResultTypes.DataTable))
                .Callback((string q, QueryResultTypes t, IDbDataParameter[] p) => q.StartsWith("select"))
                .Return(DataTableHelper.CreateDataTable(data))
                .Repeat.Once();

            mocks.ReplayAll();

            var resolver = new DatabaseObjectHashNameResolver(scheme);
            var result = resolver.Get("I_F_S_CREDITISSUEDREFSCREDITPENALTY", ObjectTypes.ForeignKeysConstraint);

            mocks.VerifyAll();

            Assert.AreEqual("\"I_F_S_CREDITISSUEDRE$W9x3gxI$1\"", result);
        }

        [Test]
        public void GetWithNotExistLongNameCollisionTest()
        {
            Expect.Call(scheme.SchemeDWH).Return(dwh);
            Expect.Call(dwh.DB).Return(db);
            Expect.Call(db.Dispose);

            Expect.Call(db.ExecQuery(String.Empty, QueryResultTypes.DataTable))
                .Callback((string q, QueryResultTypes t, IDbDataParameter[] p) => q.StartsWith("select"))
                .Return(new DataTable())
                .Repeat.Once();

            mocks.ReplayAll();

            var resolver = new DatabaseObjectHashNameResolver(scheme);
            var result = resolver.Get("I_F_S_CREDITISSUEDREFSCREDITPENALTY", ObjectTypes.ForeignKeysConstraint);

            mocks.VerifyAll();

            Assert.IsNull(result);
        }
    }
}
