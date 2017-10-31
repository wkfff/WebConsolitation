using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Calculations.Expressions;
using Krista.FM.Domain;
using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void SimpleEqualTest()
        {
            var expr = Expression.Compile(new D_Form_Relations { LeftPart = "0", RalationType = "=", RightPart = "1" });

            Assert.NotNull(expr);

            Assert.IsInstanceOf<VerifyRelation>(expr);
            var verifyRelation = (VerifyRelation)expr;
            Assert.NotNull(verifyRelation.Left);
            Assert.NotNull(verifyRelation.Right);
            Assert.AreEqual(BinaryExpressionTypes.Equal, verifyRelation.Type);
            
            Assert.IsInstanceOf<ValueExpression>(verifyRelation.Left);
            var leftValueExpression = (ValueExpression)verifyRelation.Left;
            Assert.AreEqual(ValueTypes.Integer, leftValueExpression.Type);
            Assert.AreEqual(0, leftValueExpression.Value);

            Assert.IsInstanceOf<ValueExpression>(verifyRelation.Right);
            var rightValueExpression = (ValueExpression)verifyRelation.Right;
            Assert.AreEqual(ValueTypes.Integer, rightValueExpression.Type);
            Assert.AreEqual(1, rightValueExpression.Value);
        }

        [Test]
        public void SimpleUnaryTest()
        {
            var expr = Expression.Compile(new D_Form_Relations { LeftPart = "1", RalationType = "=", RightPart = "-1" });

            Assert.NotNull(expr);

            Assert.IsInstanceOf<VerifyRelation>(expr);
            var verifyRelation = (VerifyRelation)expr;
            Assert.NotNull(verifyRelation.Left);
            Assert.NotNull(verifyRelation.Right);
            Assert.AreEqual(BinaryExpressionTypes.Equal, verifyRelation.Type);

            Assert.IsInstanceOf<ValueExpression>(verifyRelation.Left);
            var leftValueExpression = (ValueExpression)verifyRelation.Left;
            Assert.AreEqual(ValueTypes.Integer, leftValueExpression.Type);
            Assert.AreEqual(1, leftValueExpression.Value);

            Assert.IsInstanceOf<UnaryExpression>(verifyRelation.Right);
            var unaryExpression = (UnaryExpression)verifyRelation.Right;
            Assert.AreEqual(UnaryExpressionTypes.Negate, unaryExpression.Type);
            Assert.NotNull(unaryExpression.Exp);

            Assert.IsInstanceOf<ValueExpression>(unaryExpression.Exp);
            var rightValueExpression = (ValueExpression)unaryExpression.Exp;
            Assert.AreEqual(ValueTypes.Integer, rightValueExpression.Type);
            Assert.AreEqual(1, rightValueExpression.Value);
        }
    }
}
