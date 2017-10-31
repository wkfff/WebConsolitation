using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Calculations.Visitors;
using Krista.FM.Common.Consolidation.Tests.Helpers;
using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests.BinaryExpresionTests
{
    [TestFixture]
    public class BinaryPriorityTests
    {
        [Test]
        public void PriorityWithoutParTest()
        {
            Check("ИСТИНА", "=", "ЛОЖЬ ИЛИ 1 + 2 * 3 = 7 И 3 + 6 / 2 < 21 / 3");
        }

        [Test]
        public void PriorityWithParTest()
        {
            Check("ИСТИНА", "=", "(ЛОЖЬ ИЛИ 1 + 2 * 3 = 7) И (3 + 6) / 2 < 21 / 3");
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
