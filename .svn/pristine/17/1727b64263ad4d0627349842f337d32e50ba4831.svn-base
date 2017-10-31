using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.RIA.Core.Extensions;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class ExpressionTests
    {
        [Test]
        public void CanEvaluteParameters()
        {
            MockRepository mocks = new MockRepository();
            IParametersService service = mocks.DynamicMock<IParametersService>();
            Expect.Call(service.GetParameterValue("p1")).Return("A");
            Expect.Call(service.GetParameterValue("p2")).Return("Z");

            mocks.ReplayAll();

            Expression exp = new Expression(service);
            string result = exp.Eval("$(p1) + $(p2)");

            mocks.VerifyAll();

            Assert.AreEqual("AZ", result);
        }
    }
}
