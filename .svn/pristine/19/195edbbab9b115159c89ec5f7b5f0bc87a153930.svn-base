using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Calculations.Visitors;
using Krista.FM.Common.Consolidation.Tests.Helpers;
using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests
{
    [TestFixture]
    public class SimpleVerifyRelationTests
    {
        #region Success Int

        [Test, Category("Сравнение целых чисел")]
        public void EqualIntTest()
        {
            Check("1", "=", "1");
        }

        [Test, Category("Сравнение целых чисел")]
        public void NotEqualIntTest()
        {
            Check("1", "!=", "2");
        }

        [Test, Category("Сравнение целых чисел")]
        public void LesserOrEqualIntTest()
        {
            Check("1", "<=", "2");
        }

        [Test, Category("Сравнение целых чисел")]
        public void LesserAndEqualIntTest()
        {
            Check("2", "<=", "2");
        }

        [Test, Category("Сравнение целых чисел")]
        public void GreaterOrEqualIntTest()
        {
            Check("2", ">=", "1");
        }

        [Test, Category("Сравнение целых чисел")]
        public void GreaterAndEqualIntTest()
        {
            Check("2", ">=", "2");
        }

        [Test, Category("Сравнение целых чисел")]
        public void LesserIntTest()
        {
            Check("1", "<", "2");
        }

        [Test, Category("Сравнение целых чисел")]
        public void GreaterIntTest()
        {
            Check("2", ">", "1");
        }

        #endregion Success Int

        #region Fail Int

        [Test, Category("Сравнение целых чисел")]
        public void FailEqualIntTest()
        {
            CheckFail("2", "=", "1");
        }

        [Test, Category("Сравнение целых чисел")]
        public void FailNotEqualIntTest()
        {
            CheckFail("2", "!=", "2");
        }

        [Test, Category("Сравнение целых чисел")]
        public void FailLesserOrEqualIntTest()
        {
            CheckFail("3", "<=", "2");
        }

        [Test, Category("Сравнение целых чисел")]
        public void FailLesserAndEqualIntTest()
        {
            CheckFail("3", "<=", "2");
        }

        [Test, Category("Сравнение целых чисел")]
        public void FailGreaterOrEqualIntTest()
        {
            CheckFail("0", ">=", "1");
        }

        [Test, Category("Сравнение целых чисел")]
        public void FailGreaterAndEqualIntTest()
        {
            CheckFail("0", ">=", "2");
        }

        [Test, Category("Сравнение целых чисел")]
        public void FailLesserIntTest()
        {
            CheckFail("2", "<", "2");
        }

        [Test, Category("Сравнение целых чисел")]
        public void FailGreaterIntTest()
        {
            CheckFail("1", ">", "1");
        }

        #endregion Fail Int

        #region Success Decimal

        [Test, Category("Сравнение дробных чисел")]
        public void EqualDecimalTest()
        {
            Check("1.11", "=", "1.11");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void EqualEpsDecimalTest()
        {
            Check("1.0001", "=", "1.0002");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void NotEqualEpsDecimalTest()
        {
            Check("1.001", "!=", "1.002");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void LesserOrEqualDecimalTest()
        {
            Check("1.11", "<=", "2.11");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void LesserOrEqualEpsDecimalTest()
        {
            Check("1.001", "<=", "1.002");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void LesserAndEqualDecimalTest()
        {
            Check("1.001", "<=", "1.001");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void GreaterOrEqualDecimalTest()
        {
            Check("1.002", ">=", "1.001");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void GreaterAndEqualDecimalTest()
        {
            Check("1.001", ">=", "1.001");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void LesserDecimalTest()
        {
            Check("1.001", "<", "1.002");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void GreaterDecimalTest()
        {
            Check("1.002", ">", "1.001");
        }

        #endregion Success Decimal

        #region Fail Decimal

        [Test, Category("Сравнение дробных чисел")]
        public void FailEqualDecimalTest()
        {
            CheckFail("1.001", "=", "1.002");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void FailNotEqualDecimalTest()
        {
            CheckFail("1.001", "!=", "1.001");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void FailLesserOrEqualDecimalTest()
        {
            CheckFail("1.002", "<=", "1.001");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void FailGreaterOrEqualDecimalTest()
        {
            CheckFail("1.001", ">=", "1.002");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void FailLesserDecimalTest()
        {
            CheckFail("1.002", "<", "1.001");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void FailLesser2DecimalTest()
        {
            CheckFail("1.001", "<", "1.001");
        }

        [Test, Category("Сравнение дробных чисел")]
        public void FailGreaterDecimalTest()
        {
            CheckFail("1.001", ">", "1.001");
        }

        [Test, Category("Сравнение дробных чисел")]
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
