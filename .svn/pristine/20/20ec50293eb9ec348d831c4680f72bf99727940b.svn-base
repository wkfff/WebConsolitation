using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.MarksOMSU.Services;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Tests
{
    [TestFixture]
    public class CalculationsTests
    {
        private MockRepository mocks;
        private IMarksRepository marksRepository;
        private IMarksOmsuRepository repository;
        private D_Regions_Analysis region;
        private List<D_OMSU_MarksOMSU> marks;
        private List<F_OMSU_Reg16> facts;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            marksRepository = mocks.DynamicMock<IMarksRepository>();
            repository = mocks.DynamicMock<IMarksOmsuRepository>();

            region = new D_Regions_Analysis { ID = 1 };

            marks = new List<D_OMSU_MarksOMSU>
            {
                /*00*/ new D_OMSU_MarksOMSU { ID = 1, Symbol = "A", Formula = String.Empty },
                /*01*/ new D_OMSU_MarksOMSU { ID = 2, Symbol = "B", Formula = String.Empty },
                /*02*/ new D_OMSU_MarksOMSU { ID = 3, Symbol = "C", Formula = "A+B" },
                /*03*/ new D_OMSU_MarksOMSU { ID = 4, Symbol = "Z", Formula = String.Empty },
                /*04*/ new D_OMSU_MarksOMSU { ID = 5, Symbol = "DivZero", Formula = "B/Z" },
                /*05*/ new D_OMSU_MarksOMSU { ID = 6, Symbol = "D", Formula = String.Empty },
                /*06*/ new D_OMSU_MarksOMSU { ID = 7, Symbol = "Null", Formula = String.Empty },
                /*07*/ new D_OMSU_MarksOMSU { ID = 8, Symbol = "CalcWithNull", Formula = "D + Null" },
                /*08*/ new D_OMSU_MarksOMSU { ID = 9, Symbol = "ManualPrognoz", Formula = "A+B", PrognMO = true },
                /*09*/ new D_OMSU_MarksOMSU { ID = 10, Symbol = "ProtectAcceptedOrApproved", Formula = "A+B" }
            };

            var omsuStatusEdit = new FX_OMSU_StatusData { ID = (int)OMSUStatus.OnEdit, Code = (int)OMSUStatus.OnEdit, Name = "Edit", RowType = 0 };
            var omsuStatusAccepted = new FX_OMSU_StatusData { ID = (int)OMSUStatus.Accepted, Code = (int)OMSUStatus.Accepted, Name = "Accepted", RowType = 0 };
            var omsuStatusApproved = new FX_OMSU_StatusData { ID = (int)OMSUStatus.Approved, Code = (int)OMSUStatus.Approved, Name = "Approved", RowType = 0 };

            facts = new List<F_OMSU_Reg16>
            {
                /*00*/ new F_OMSU_Reg16 { RefMarksOMSU = marks[0], RefRegions = region, CurrentValue = 2, PriorValue = 2, Prognoz1 = 2, Prognoz2 = 2, Prognoz3 = 2, RefStatusData = omsuStatusEdit },
                /*01*/ new F_OMSU_Reg16 { RefMarksOMSU = marks[1], RefRegions = region, CurrentValue = 5, PriorValue = 5, Prognoz1 = 5, Prognoz2 = 5, Prognoz3 = 5, RefStatusData = omsuStatusEdit },
                /*02*/ new F_OMSU_Reg16 { RefMarksOMSU = marks[2], RefRegions = region, CurrentValue = -1, RefStatusData = omsuStatusEdit },
                /*03*/ new F_OMSU_Reg16 { RefMarksOMSU = marks[3], RefRegions = region, CurrentValue = 0, RefStatusData = omsuStatusEdit },
                /*04*/ new F_OMSU_Reg16 { RefMarksOMSU = marks[4], RefRegions = region, CurrentValue = -1, RefStatusData = omsuStatusEdit },
                /*05*/ new F_OMSU_Reg16 { RefMarksOMSU = marks[5], RefRegions = region, CurrentValue = 3, RefStatusData = omsuStatusEdit },
                /*06*/ new F_OMSU_Reg16 { RefMarksOMSU = marks[6], RefRegions = region, CurrentValue = null, RefStatusData = omsuStatusEdit },
                /*07*/ new F_OMSU_Reg16 { RefMarksOMSU = marks[7], RefRegions = region, CurrentValue = 1, RefStatusData = omsuStatusEdit },
                /*08*/ new F_OMSU_Reg16 { RefMarksOMSU = marks[8], RefRegions = region, CurrentValue = -1, PriorValue = -1, Prognoz1 = -1, Prognoz2 = -1, Prognoz3 = -1, RefStatusData = omsuStatusEdit },
                /*09*/ new F_OMSU_Reg16 { RefMarksOMSU = marks[9], RefRegions = region, CurrentValue = -1, PriorValue = -1, Prognoz1 = -1, Prognoz2 = -1, Prognoz3 = -1, RefStatusData = omsuStatusAccepted },
                /*10*/ new F_OMSU_Reg16 { RefMarksOMSU = marks[9], RefRegions = region, CurrentValue = -1, PriorValue = -1, Prognoz1 = -1, Prognoz2 = -1, Prognoz3 = -1, RefStatusData = omsuStatusApproved },
            };

            Expect.Call(marksRepository.FindAll()).Return(marks.AsQueryable()).Repeat.Any();
        }

        [Test]
        public void SimpleTest()
        {
            Expect.Call(repository.GetFactForMarkRegion(1, 1)).Return(facts[0]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(2, 1)).Return(facts[1]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(3, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(9, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(10, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(() => repository.Save(null)).Repeat.Any();

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, repository);
            
            calc.Calc(new List<F_OMSU_Reg16> { facts[0] }, 1, false);

            mocks.VerifyAll();

            Assert.AreEqual(7, facts[2].CurrentValue);
        }

        [Test]
        public void TestCalc()
        {
            Expect.Call(repository.GetFactForMarkRegion(1, 1)).Return(facts[0]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(2, 1)).Return(facts[1]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(3, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(9, 1)).Return(facts[8]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(10, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(() => repository.Save(facts[0])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[1])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[2])).Repeat.Once();

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, repository);
            
            calc.Calc(new List<F_OMSU_Reg16> { facts[0] }, 1, false);

            mocks.VerifyAll();

            Assert.AreEqual(7, facts[2].CurrentValue);
        }

        [Test]
        public void TestCalcForceProtected()
        {
            Expect.Call(repository.GetFactForMarkRegion(1, 1)).Return(facts[0]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(2, 1)).Return(facts[1]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(3, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(9, 1)).Return(facts[8]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(10, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(() => repository.Save(facts[0])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[1])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[2])).Repeat.Once();

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, repository);
            
            calc.CalcForceProtected(new List<F_OMSU_Reg16> { facts[0] }, 1, false);

            mocks.VerifyAll();

            Assert.AreEqual(7, facts[2].CurrentValue);
        }

        [Test]
        public void TestCalcDoNotSave()
        {
            Expect.Call(repository.GetFactForMarkRegion(1, 1)).Return(facts[0]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(2, 1)).Return(facts[1]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(3, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(9, 1)).Return(facts[8]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(10, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(() => repository.Save(facts[0])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[1])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[2])).Repeat.Never(); /* В БД не пишем... */

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, repository);
            
            calc.CalcForceProtectedDoNotSave(new List<F_OMSU_Reg16> { facts[0] }, 1, false);

            mocks.VerifyAll();

            Assert.AreEqual(7, facts[2].CurrentValue);  /* ...но значение изменяется. */
        }

        [Test]
        public void DivideTest()
        {
            Expect.Call(repository.GetFactForMarkRegion(1, 1)).Return(facts[0]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(2, 1)).Return(facts[1]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(3, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(4, 1)).Return(facts[3]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(5, 1)).Return(facts[4]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(9, 1)).Return(facts[8]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(10, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(() => repository.Save(null)).Repeat.Any();

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, repository);

            calc.Calc(new List<F_OMSU_Reg16> { facts[1] }, 1, false);

            mocks.VerifyAll();

            Assert.AreEqual(0, facts[4].CurrentValue);
        }

        [Test]
        public void CalcWithNullTest()
        {
            Expect.Call(repository.GetFactForMarkRegion(6, 1)).Return(facts[5]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(7, 1)).Return(facts[6]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(8, 1)).Return(facts[7]).Repeat.Once();
            Expect.Call(() => repository.Save(null)).Repeat.Any();

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, repository);

            calc.Calc(new List<F_OMSU_Reg16> { facts[5] }, 1, false);

            mocks.VerifyAll();

            Assert.AreEqual(null, facts[6].CurrentValue);
        }

        [Test]
        public void CalculatePrognoz()
        {
            Expect.Call(repository.GetFactForMarkRegion(1, 1)).Return(facts[0]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(2, 1)).Return(facts[1]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(3, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(() => repository.Save(facts[0])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[1])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[2])).Repeat.Once();

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, repository);
            calc.Calc(new List<F_OMSU_Reg16> { facts[2] }, 1, true);

            mocks.VerifyAll();

            Assert.AreEqual(7, facts[2].PriorValue);
            Assert.AreEqual(7, facts[2].CurrentValue);
            Assert.AreEqual(7, facts[2].Prognoz1);
            Assert.AreEqual(7, facts[2].Prognoz2);
            Assert.AreEqual(7, facts[2].Prognoz3);
        }

        [Test]
        public void PreserveManualEnteredPrognoz()
        {
            Expect.Call(repository.GetFactForMarkRegion(1, 1)).Return(facts[0]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(2, 1)).Return(facts[1]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(9, 1)).Return(facts[8]).Repeat.Once();
            Expect.Call(() => repository.Save(facts[0])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[1])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[8])).Repeat.Once();

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, repository);
            calc.Calc(new List<F_OMSU_Reg16> { facts[8] }, 1, true);

            mocks.VerifyAll();

            Assert.AreEqual(7, facts[8].PriorValue);
            Assert.AreEqual(7, facts[8].CurrentValue);
            Assert.AreEqual(-1, facts[8].Prognoz1);
            Assert.AreEqual(-1, facts[8].Prognoz2);
            Assert.AreEqual(-1, facts[8].Prognoz3);
        }

        [Test]
        public void PreserveAcceptedValues()
        {
            Expect.Call(repository.GetFactForMarkRegion(1, 1)).Return(facts[0]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(2, 1)).Return(facts[1]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(10, 1)).Return(facts[9]).Repeat.Once();
            Expect.Call(() => repository.Save(facts[0])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[1])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[9])).Repeat.Once(); /* Результаты сохранять можно... */

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, repository);
            calc.Calc(new List<F_OMSU_Reg16> { facts[9] }, 1, true);

            mocks.VerifyAll();

            /* ...но исходные знаения не должны изменится */
            Assert.AreEqual(-1, facts[9].PriorValue);
            Assert.AreEqual(-1, facts[9].CurrentValue);
            Assert.AreEqual(-1, facts[9].Prognoz1);
            Assert.AreEqual(-1, facts[9].Prognoz2);
            Assert.AreEqual(-1, facts[9].Prognoz3);
        }

        [Test]
        public void PreserveApprovedValues()
        {
            Expect.Call(repository.GetFactForMarkRegion(1, 1)).Return(facts[0]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(2, 1)).Return(facts[1]).Repeat.Once();
            Expect.Call(repository.GetFactForMarkRegion(10, 1)).Return(facts[10]).Repeat.Once();
            Expect.Call(() => repository.Save(facts[0])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[1])).Repeat.Never();
            Expect.Call(() => repository.Save(facts[10])).Repeat.Once(); /* Результаты в БД сохранять можно... */

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, repository);
            calc.Calc(new List<F_OMSU_Reg16> { facts[10] }, 1, true);

            mocks.VerifyAll();

            /* ...но исходные знаения не должны изменится */
            Assert.AreEqual(-1, facts[10].PriorValue);
            Assert.AreEqual(-1, facts[10].CurrentValue);
            Assert.AreEqual(-1, facts[10].Prognoz1);
            Assert.AreEqual(-1, facts[10].Prognoz2);
            Assert.AreEqual(-1, facts[10].Prognoz3);
        }

        [Ignore]
        [Test]
        public void WithDbTest()
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            var clientSession = ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                "Password=dv;Persist Security Info=True;User ID=HMAOACR;Initial Catalog=HMAOACR;Data Source=mzserv\\mssql2008", 
                "SQL", 
                "10");

            var extensionService = mocks.DynamicMock<IMarksOmsuExtension>();
            Expect.Call(extensionService.DataSourceOmsu).Return(new DataSources { ID = 8 });
            mocks.ReplayAll();

            IMarksRepository repository = new MarksRepository(new NHibernateLinqRepository<D_OMSU_MarksOMSU>(), extensionService);

            NHibernateLinqRepository<F_OMSU_Reg16> factRepository = new NHibernateLinqRepository<F_OMSU_Reg16>();

            var calc = new MarksCalculator(repository, new MarksOmsuRepository(factRepository, null, null, null, null));

            var facts = factRepository.FindAll()
                .Where(x => x.RefRegions.ID == 8227 && (x.RefMarksOMSU.Formula == null || x.RefMarksOMSU.Formula == String.Empty))
                .ToList();

            calc.Calc(facts, 8227, true);
        }
    }
}
