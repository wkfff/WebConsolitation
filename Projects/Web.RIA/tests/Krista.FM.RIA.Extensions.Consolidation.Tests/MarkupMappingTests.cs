using System.Text;
using Krista.FM.Common.Consolidation.Forms.ExcelMapping;
using Krista.FM.Common.Consolidation.Forms.Layout;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.Consolidation.Tests.Model;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class MarkupMappingTests
    {
        [Test]
        public void EmptyImportExportTest()
        {
            D_CD_Templates template = new D_CD_Templates();
            template.SetFormMarkupMappings(new Form());

            var mapping = template.GetFormMarkupMappings();

            Assert.IsEmpty(mapping.Sheets);
        }

        [Test]
        public void GenerateTest()
        {
            var str = Encoding.UTF8.GetString(MROTTemplate.GetTemplate().TemplateMarkup);
        }
    }
}
