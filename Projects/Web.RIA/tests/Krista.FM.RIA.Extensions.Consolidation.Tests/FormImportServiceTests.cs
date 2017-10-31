using System.IO;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.Metadata;
using Krista.FM.RIA.Extensions.Consolidation.Tests.Helpers;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class FormImportServiceTests
    {
        [Ignore]
        [Test]
        public void Test()
        {
            using (var fileStream = File.OpenRead(@"d:\test.zip"))
            {
                var service = new FormImportService();
                service.Import(fileStream);
            }
        }
    }
}
