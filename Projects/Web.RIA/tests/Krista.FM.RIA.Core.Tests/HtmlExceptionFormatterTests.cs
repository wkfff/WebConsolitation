using System;
using System.Runtime.InteropServices;
using Krista.FM.RIA.Core.Helpers;
using NUnit.Framework;

namespace Krista.FM.Extensions.Tests
{
    [TestFixture]
    public class HtmlExceptionFormatterTests
    {
        [Test]
        public void Test()
        {
            var innerException = new ExternalException("Is inner ExternalException");
            var exception = new ApplicationException("Is ApplicationException", innerException)
            {
                Source = "Is source property",
                HelpLink = "Is HelpLink property"
            };

            exception.Data.Add("DataKey1", "DataValue1");
            exception.Data.Add("DataKey2", "DataValue2");

            var result = exception.ExpandException(new HtmlExceptionFormatter());

            Assert.IsNotNullOrEmpty(result);
            Assert.True(result.Contains("Is inner ExternalException"));
            Assert.True(result.Contains("Is ApplicationException"));
            Assert.True(result.Contains("Is source property"));
            Assert.True(result.Contains("Is HelpLink property"));
            Assert.True(result.Contains("DataKey1"));
            Assert.True(result.Contains("DataValue2"));
        }
    }
}
