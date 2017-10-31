using System.Linq;
using Krista.FM.Domain;
using NUnit.Framework;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class JavaScriptDomainConverterTests
    {
        [Test]
        public void CanConvertTest()
        {
            var result = JavaScriptDomainConverter<F_OMSU_Reg16>.Deserialize(Resources.Resources.OivStoreSaveJSON);

            Assert.NotNull(result.Updated);
            Assert.AreEqual(1, result.Updated.Count());
            Assert.AreEqual(540, result.Updated[0].ID);
            Assert.AreEqual(1, result.Updated[0].PriorValue);
            Assert.AreEqual(16, result.Updated[0].RefMarksOMSU.ID);
        }
    }
}
