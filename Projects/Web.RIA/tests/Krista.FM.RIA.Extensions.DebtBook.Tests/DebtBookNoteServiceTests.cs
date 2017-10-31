using System;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.DebtBook.Services.Note;
using Krista.FM.ServerLibrary;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.DebtBook.Tests
{
    [TestFixture]
    class DebtBookNoteServiceTests
    {
        [Ignore]
        [Test]
        public void GetChildRegionsNotesListTest()
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                "Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV",
                "Oracle",
                "10");

            var noteRepositoryReal = new NHibernateLinqRepository<F_S_SchBNote>();
            var territoryRepositoryReal = new NHibernateLinqRepository<D_Regions_Analysis>();
            var variantRepositoryReal = new NHibernateLinqRepository<D_Variant_Schuldbuch>();

            IDebtBookNoteService noteService = new DebtBookNoteService(noteRepositoryReal, territoryRepositoryReal, variantRepositoryReal);

            var data = noteService.GetChildRegionsNotesList(12602, 222, 6222);
            Assert.NotNull(data);
            Assert.GreaterOrEqual(data.Count, 0);
            Console.WriteLine(String.Format("Выбрано записей: {0}", data.Count));
        }
    }
}
