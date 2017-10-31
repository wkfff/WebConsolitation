using System.Text;
using Krista.FM.Common.Consolidation.Forms.Layout;
using Krista.FM.Domain;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class FormLayoutMarkupValidatorTests
    {
        [Test]
        public void ValidatorTest()
        {
            var form = new D_CD_Templates();
            form.TemplateFile = Resources.Resource.MROTTemplate;
            form.TemplateMarkup = Encoding.UTF8.GetBytes(Resources.Resource.MROTMarkup);
            var sheetMapping = form.GetFormMarkupMappings();

            var validator = new FormLayoutMarkupValidator();
            var errors = validator.Validate(form, sheetMapping);

            Assert.AreEqual(0, errors.Count);
        }
    }
}
