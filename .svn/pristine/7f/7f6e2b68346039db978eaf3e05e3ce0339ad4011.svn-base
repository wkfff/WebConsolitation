using System;
using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Calculations.Visitors;
using Krista.FM.Common.Consolidation.Tests.Helpers;
using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests
{
    [TestFixture]
    public class SimpleArithmeticVerifyRelationTests
    {
        [Test, Category("Арифметика над целыми числами")]
        public void StringConcatTest()
        {
            Check("'Hello'", "=", "'Hel' + 'lo'");
        }

        #region Success Int

        [Test, Category("Арифметика над целыми числами")]
        public void SummIntTest()
        {
            Check("2", "=", "1 + 1");
        }

        [Test, Category("Арифметика над целыми числами")]
        public void SubstractIntTest()
        {
            Check("0", "=", "1 - 1");
        }

        [Test, Category("Арифметика над целыми числами")]
        public void TimesIntTest()
        {
            Check("4", "=", "2 * 2");
        }

        [Test, Category("Арифметика над целыми числами")]
        public void DivIntTest()
        {
            Check("2", "=", "4 / 2");
        }

        [Test, Category("Арифметика над целыми числами")]
        public void DivDecimalResultIntTest()
        {
            Check("0.25", "=", "1 / 4");
        }

        [Test, Category("Арифметика над целыми числами")]
        public void DivDecimalResult2IntTest()
        {
            Check("0.125", "=", "1 / 8");
        }

        [Test, Category("Арифметика над целыми числами")]
        public void DivDecimalResultEpsIntTest()
        {
            // 0.0625 = 1 / 16
            Check("0.0629", "=", "1 / 16");
        }

        [Test, Category("Арифметика над целыми числами")]
        public void DivZeroIntTest()
        {
            Check("0", "=", "0 / 2");
        }

        [Test, Category("Арифметика над целыми числами")]
        [ExpectedException(typeof(DivideByZeroException))]
        public void DivZeroErrorIntTest()
        {
            Check("2", "=", "4 / 0");
        }

        #endregion Success Int

        #region Success Decimal

        [Test, Category("Арифметика над дробными числами")]
        public void SummDecimalTest()
        {
            Check("2.002", "=", "1.001 + 1.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void SummDecimalIntTest()
        {
            Check("2.001", "=", "1.001 + 1");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void SummDecimalInt2Test()
        {
            Check("2.001", "=", "1 + 1.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void SubstractDecimalTest()
        {
            Check("1.001", "=", "2.002 - 1.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void SubstractDecimalIntTest()
        {
            Check("0.001", "=", "1.001 - 1");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void SubstractDecimalInt2Test()
        {
            Check("0.999", "=", "1 - 0.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void TimesDecimalTest()
        {
            Check("1.002", "=", "1.001 * 1.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void DivDecimalTest()
        {
            Check("0.99", "=", "1.01 / 1.02");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void DivDecimal2Test()
        {
            Check("2", "=", "2.002 / 1.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void DivDecimalZeroTest()
        {
            Check("0", "=", "0.000 / 1.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        [ExpectedException(typeof(DivideByZeroException))]
        public void DivDecimalZeroErrorTest()
        {
            Check("0", "=", "1.000 / 0.000");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void ModDecimalTest()
        {
            Check("1.4", "=", "4.5 % 3.1");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void ModDecimalZeroTest()
        {
            Check("0", "=", "0.000 % 1.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        [ExpectedException(typeof(DivideByZeroException))]
        public void ModDecimalZeroErrorTest()
        {
            Check("0", "=", "1.000 % 0.000");
        }

        #endregion Success Decimal

        #region Fail Decimal

        [Test, Category("Арифметика над дробными числами")]
        public void FailEqualDecimalTest()
        {
            CheckFail("1.001", "=", "1.002");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void FailNotEqualDecimalTest()
        {
            CheckFail("1.001", "!=", "1.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void FailLesserOrEqualDecimalTest()
        {
            CheckFail("1.002", "<=", "1.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void FailGreaterOrEqualDecimalTest()
        {
            CheckFail("1.001", ">=", "1.002");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void FailLesserDecimalTest()
        {
            CheckFail("1.002", "<", "1.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void FailLesser2DecimalTest()
        {
            CheckFail("1.001", "<", "1.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void FailGreaterDecimalTest()
        {
            CheckFail("1.001", ">", "1.001");
        }

        [Test, Category("Арифметика над дробными числами")]
        public void FailGreater2DecimalTest()
        {
            CheckFail("1.001", ">", "1.002");
        }

        #endregion Fail Decimal

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
