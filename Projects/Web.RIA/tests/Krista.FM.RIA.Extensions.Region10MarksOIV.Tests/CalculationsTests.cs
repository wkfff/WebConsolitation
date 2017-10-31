using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.Region10MarksOIV.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Tests
{
    [TestFixture]
    public class CalculationsTests
    {
        private MockRepository mocks;
        private IMarksRepository marksRepository;
        private IFactRepository factRepository;
        private D_Territory_RF territory;
        private List<D_OIV_Mark> marks;
        private List<F_OIV_REG10Qual> facts;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            marksRepository = mocks.DynamicMock<IMarksRepository>();
            factRepository = mocks.DynamicMock<IFactRepository>();

            territory = new D_Territory_RF { ID = 1 };

            marks = new List<D_OIV_Mark>
            {
                new D_OIV_Mark { ID = 1, Symbol = "A", Formula = String.Empty },
                new D_OIV_Mark { ID = 2, Symbol = "B", Formula = String.Empty },
                new D_OIV_Mark { ID = 3, Symbol = "C", Formula = "A+B" },
                new D_OIV_Mark { ID = 4, Symbol = "Z", Formula = String.Empty },
                new D_OIV_Mark { ID = 5, Symbol = "DivZero", Formula = "B/Z" },
                new D_OIV_Mark { ID = 6, Symbol = "D", Formula = String.Empty },
                new D_OIV_Mark { ID = 7, Symbol = "Null", Formula = String.Empty },
                new D_OIV_Mark { ID = 8, Symbol = "CalcWithNull", Formula = "D + Null" },
            };

            facts = new List<F_OIV_REG10Qual>
            {
                new F_OIV_REG10Qual { RefOIV = marks[0], RefTerritory = territory, Fact = 2 },
                new F_OIV_REG10Qual { RefOIV = marks[1], RefTerritory = territory, Fact = 5 },
                new F_OIV_REG10Qual { RefOIV = marks[2], RefTerritory = territory, Fact = -1 },
                new F_OIV_REG10Qual { RefOIV = marks[3], RefTerritory = territory, Fact = 0 },
                new F_OIV_REG10Qual { RefOIV = marks[4], RefTerritory = territory, Fact = -1 },
                new F_OIV_REG10Qual { RefOIV = marks[5], RefTerritory = territory, Fact = 3 },
                new F_OIV_REG10Qual { RefOIV = marks[6], RefTerritory = territory, Fact = null },
                new F_OIV_REG10Qual { RefOIV = marks[7], RefTerritory = territory, Fact = 1 },
            };

            Expect.Call(marksRepository.FindAll()).Return(marks.AsQueryable()).Repeat.Any();
            Expect.Call(() => factRepository.Save(null)).Repeat.Any();
        }

        [Test]
        public void SimpleTest()
        {
            Expect.Call(factRepository.GetFactForMarkTerritory(1, 1)).Return(facts[0]).Repeat.Once();
            Expect.Call(factRepository.GetFactForMarkTerritory(2, 1)).Return(facts[1]).Repeat.Once();
            Expect.Call(factRepository.GetFactForMarkTerritory(3, 1)).Return(facts[2]).Repeat.Once();

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, factRepository);
            
            calc.Calc(new List<F_OIV_REG10Qual> { facts[0] }, 1, false);

            mocks.VerifyAll();

            Assert.AreEqual(7, facts[2].Fact);
        }

        [Test]
        public void DivideTest()
        {
            Expect.Call(factRepository.GetFactForMarkTerritory(1, 1)).Return(facts[0]).Repeat.Once();
            Expect.Call(factRepository.GetFactForMarkTerritory(2, 1)).Return(facts[1]).Repeat.Once();
            Expect.Call(factRepository.GetFactForMarkTerritory(3, 1)).Return(facts[2]).Repeat.Once();
            Expect.Call(factRepository.GetFactForMarkTerritory(4, 1)).Return(facts[3]).Repeat.Once();
            Expect.Call(factRepository.GetFactForMarkTerritory(5, 1)).Return(facts[4]).Repeat.Once();

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, factRepository);

            calc.Calc(new List<F_OIV_REG10Qual> { facts[1] }, 1, false);

            mocks.VerifyAll();

            Assert.AreEqual(0, facts[4].Fact);
        }

        [Test]
        public void CalcWithNullTest()
        {
            Expect.Call(factRepository.GetFactForMarkTerritory(6, 1)).Return(facts[5]).Repeat.Once();
            Expect.Call(factRepository.GetFactForMarkTerritory(7, 1)).Return(facts[6]).Repeat.Once();
            Expect.Call(factRepository.GetFactForMarkTerritory(8, 1)).Return(facts[7]).Repeat.Once();

            mocks.ReplayAll();

            var calc = new MarksCalculator(marksRepository, factRepository);

            calc.Calc(new List<F_OIV_REG10Qual> { facts[5] }, 1, false);

            mocks.VerifyAll();

            Assert.AreEqual(null, facts[6].Fact);
        }
    }
}
