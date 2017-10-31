using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Calculations.Visitors;
using Krista.FM.Common.Consolidation.Tests.Helpers;
using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests.BinaryExpresionTests
{
    [TestFixture]
    public class BinaryLogicTests
    {
        #region Success

        [Test]
        public void AndTrueTest()
        {
            Check("ИСТИНА", "=", "ИСТИНА И ИСТИНА");
        }

        [Test]
        public void AndFalseTest()
        {
            Check("ЛОЖЬ", "=", "ИСТИНА И ЛОЖЬ");
        }

        [Test]
        public void OrTrueTest()
        {
            Check("ИСТИНА", "=", "ИСТИНА ИЛИ ИСТИНА");
        }

        [Test]
        public void OrTrue2Test()
        {
            Check("ИСТИНА", "=", "ИСТИНА ИЛИ ЛОЖЬ");
        }

        [Test]
        public void OrFalseTest()
        {
            Check("ЛОЖЬ", "=", "ЛОЖЬ И ЛОЖЬ");
        }

        #endregion Success

        #region Fail

        [Test]
        public void AndTrueFailTest()
        {
            CheckFail("ЛОЖЬ", "=", "ИСТИНА И ИСТИНА");
        }

        [Test]
        public void AndFalseFailTest()
        {
            CheckFail("ИСТИНА", "=", "ИСТИНА И ЛОЖЬ");
        }

        [Test]
        public void OrTrueFailTest()
        {
            CheckFail("ЛОЖЬ", "=", "ИСТИНА ИЛИ ИСТИНА");
        }

        [Test]
        public void OrTrue2FailTest()
        {
            CheckFail("ЛОЖЬ", "=", "ИСТИНА ИЛИ ЛОЖЬ");
        }

        [Test]
        public void OrFalseFailTest()
        {
            CheckFail("ИСТИНА", "=", "ЛОЖЬ И ЛОЖЬ");
        }

        #endregion Fail

        #region Priority

        [Test]
        public void PriorityWithoutParTest()
        {
            Check("ИСТИНА", "=", "ЛОЖЬ ИЛИ ИСТИНА И ИСТИНА");
        }

        [Test]
        public void PriorityWithoutPar2Test()
        {
            Check("ИСТИНА", "=", "ИСТИНА ИЛИ ЛОЖЬ И ИСТИНА");
        }

        [Test]
        public void PriorityWithoutPar3Test()
        {
            Check("ЛОЖЬ", "=", "ЛОЖЬ ИЛИ ИСТИНА И ЛОЖЬ");
        }

        [Test]
        public void PriorityWithParTest()
        {
            Check("ИСТИНА", "=", "ИСТИНА И (ЛОЖЬ ИЛИ ИСТИНА)");
        }

        [Test]
        public void PriorityWithPar2Test()
        {
            Check("ЛОЖЬ", "=", "(ЛОЖЬ ИЛИ ЛОЖЬ) И ИСТИНА");
        }

        [Test]
        public void PriorityWithPar3Test()
        {
            Check("ЛОЖЬ", "=", "(ИСТИНА ИЛИ ИСТИНА) И ЛОЖЬ");
        }

        #endregion Priority

        #region Utils

        private static EvaluationVisitor Calculate(string left, string op, string right)
        {
            var expr = Expression.Compile(RelationsFactory.Create(left, op, right));
            var visitor = new EvaluationVisitor();
            expr.Accept(visitor);
            return visitor;
        }

        private static void Check(string left, string op, string right)
        {
            var visitor = Calculate(left, op, right);

            Assert.IsInstanceOf<bool>(visitor.Result);
            Assert.IsTrue((bool)visitor.Result);
        }

        private static void CheckFail(string left, string op, string right)
        {
            var visitor = Calculate(left, op, right);

            Assert.IsInstanceOf<bool>(visitor.Result);
            Assert.IsFalse((bool)visitor.Result);
        }

        #endregion Utils
    }
}
