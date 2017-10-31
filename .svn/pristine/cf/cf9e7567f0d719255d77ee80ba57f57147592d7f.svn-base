using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Calculations.Visitors;
using Krista.FM.Common.Consolidation.Tests.Helpers;
using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests
{
    [TestFixture]
    public class SimpleArithmeticPriorityVerifyRelationTests
    {
        [Test, Category("Приоритет арифметических вычислений")]
        public void PriorityWithoutParTest()
        {
            Check("20", "=", "2 + 3 * 6");
        }

        [Test, Category("Приоритет арифметических вычислений")]
        public void PriorityWithoutPar2Test()
        {
            Check("5", "=", "2 + 6 / 2");
        }

        [Test, Category("Приоритет арифметических вычислений")]
        public void PriorityWithParTest()
        {
            Check("30", "=", "(2 + 3) * 6");
        }

        [Test, Category("Приоритет арифметических вычислений")]
        public void PriorityWithPar2Test()
        {
            Check("2", "=", "(2 + 6) / 4");
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
