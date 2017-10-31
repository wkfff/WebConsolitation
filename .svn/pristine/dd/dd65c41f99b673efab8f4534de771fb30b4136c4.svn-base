using System;
using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Calculations.Visitors;
using Krista.FM.Common.Consolidation.Tests.Helpers;
using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests
{
    [TestFixture]
    public class UnaryExpresionTests
    {
        [Test]
        public void NotTest()
        {
            Check("ИСТИНА", "=", "НЕ ЛОЖЬ");
        }

        [Test]
        public void Not2Test()
        {
            Check("ЛОЖЬ", "=", "НЕ ИСТИНА");
        }

        [Test]
        public void NotPriorityTest()
        {
            Check("ЛОЖЬ", "=", "НЕ ИСТИНА И ЛОЖЬ");
        }

        [Test]
        public void NegateTest()
        {
            Check("0", ">", "-1");
        }

        [Test]
        public void Negate2Test()
        {
            Check("1", "=", "-(-1)");
        }

        [Test]
        public void NegatePriorityTest()
        {
            Check("0", "=", "1 + -1");
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void NotErrorPriorityTest()
        {
            Check("ИСТИНА", "=", "НЕ 'ЛОЖЬ'");
        }

        [Test]
        public void NotForNumbersTrueTest()
        {
            Check("ИСТИНА", "=", "НЕ 0");
        }

        [Test]
        public void NotForNumbersFalseTest()
        {
            Check("ЛОЖЬ", "=", "НЕ 1");
        }

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

        #endregion Utils
    }
}
