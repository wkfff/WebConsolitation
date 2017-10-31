using System.IO;
using System.Text;
using Krista.FM.Common.Consolidation.Forms.Layout;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Models;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class FormMarkupServiceTests
    {
        private D_CD_Templates form;

        [SetUp]
        public void SetUp()
        {
            form = new D_CD_Templates();
            form.Requisites.Add(new D_Form_Requisites { ID = 1, InternalName = "Region", DataType = "System.String", IsHeader = true, Name = "по муниципальному образованию", Ord = 1, ReadOnly = true, RefForm = form });
            form.Requisites.Add(new D_Form_Requisites { ID = 2, InternalName = "Period", DataType = "System.DateTime", IsHeader = true, Name = "За период", Ord = 2, ReadOnly = true, RefForm = form });
            form.Requisites.Add(new D_Form_Requisites { ID = 3, InternalName = "Duer", DataType = "System.String", IsHeader = false, Name = "ФИО Исполнителя", Ord = 1, RefForm = form });
            form.Requisites.Add(new D_Form_Requisites { ID = 4, InternalName = "Phone", DataType = "System.String", IsHeader = false, Name = "Телефон", Ord = 2, RefForm = form });
            form.Requisites.Add(new D_Form_Requisites { ID = 5, InternalName = "FillDate", DataType = "System.DateTime", IsHeader = false, Name = "Дата заполнения отчета", Ord = 3, RefForm = form });

            D_Form_Part part = new D_Form_Part { ID = 1, InternalName = "s1", Name = "Информация о выполнении обязательств территориального трехстороннего соглашения", Ord = 0, RefForm = form };
            part.Columns.Add(new D_Form_TableColumn { ID = 1, InternalName = "Code", DataType = "System.String", Name = "№ п.п.", IsPersistent = true, Ord = 0, ReadOnly = true, RefPart = part });
            part.Columns.Add(new D_Form_TableColumn { ID = 2, InternalName = "Name", DataType = "System.String", Name = "Наименование", IsPersistent = true, Ord = 1, ReadOnly = true, RefPart = part });
            part.Columns.Add(new D_Form_TableColumn { ID = 3, InternalName = "Total", DataType = "System.Double", Name = "Всего по муниципальному образованию", IsPersistent = true, Ord = 2, ReadOnly = false, RefPart = part });
            part.Columns.Add(new D_Form_TableColumn { ID = 4, InternalName = "Org", DataType = "System.Double", Name = "Крупные и средние организации", IsPersistent = true, Ord = 3, ReadOnly = false, RefPart = part });
            part.Columns.Add(new D_Form_TableColumn { ID = 5, InternalName = "TinyOrg", DataType = "System.Double", Name = "Субъекты малого предпринима-тельства", IsPersistent = true, Ord = 4, ReadOnly = false, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 1, Ord = 0, Name = "1.0", ReadOnly = true, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 2, Ord = 1, Name = "1.1", ReadOnly = false, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 3, Ord = 2, Name = "1.2", ReadOnly = false, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 4, Ord = 3, Name = "2.0", ReadOnly = true, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 5, Ord = 4, Name = "2.1", ReadOnly = false, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 6, Ord = 5, Name = "2.2", ReadOnly = false, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 7, Ord = 6, Name = "3", ReadOnly = true, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 8, Ord = 7, Name = "3.0", ReadOnly = true, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 9, Ord = 8, Name = "3.1", ReadOnly = false, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 10, Ord = 9, Name = "3.2", ReadOnly = false, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 11, Ord = 10, Name = "4.0", ReadOnly = true, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 12, Ord = 11, Name = "4.1", ReadOnly = false, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 13, Ord = 12, Name = "4.2", ReadOnly = false, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 14, Ord = 13, Name = "5.0", ReadOnly = true, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 15, Ord = 14, Name = "5.1", ReadOnly = false, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { ID = 16, Ord = 15, Name = "5.2", ReadOnly = false, RefPart = part });
            form.Parts.Add(part);
        }

        [Ignore]
        [Test]
        public void GetSectionTableTest()
        {
            while (true)
            {
                var service = new FormLayoutMarkupService();
                form.TemplateFile = Resources.Resource.MROTTemplate;
                form.TemplateMarkup = Encoding.UTF8.GetBytes(Resources.Resource.MROTMarkup);
                var sheetMapping = form.GetFormMarkupMappings().Sheets[0];
                var viewModel = service.GetSectionTable(form, sheetMapping, "s1");
                LayoutMarkupViewModel markup = viewModel.Layout;

                var stringBuilder = new StringBuilder();
                var jsonSerializerSettings = new JsonSerializerSettings();
                JsonSerializer.Create(jsonSerializerSettings).Serialize(new JsonTextWriter(new StringWriter(stringBuilder)), markup);
                var str = stringBuilder.ToString();

                StringBuilder columns = new StringBuilder()
                    .AppendLine("Ext.ns('app.grid');")
                    .AppendLine("app.grid.columnModel = [");
                foreach (var column in viewModel.Columns)
                {
                    columns
                        .Append(column.Serialize())
                        .Append(",");
                }

                columns.RemoveLastChar();
                columns.Append("];");
                File.WriteAllLines(@"C:\Users\gbelov\My Documents\Aptana Studio 3 Workspace\TestJS\ExcelGridView.Columns.js", new[] { columns.ToString() });

                string[] lines = { "Ext.ns('app.grid');", "app.grid.layoutMarkupData = ", str, ";" };
                File.WriteAllLines(@"C:\Users\gbelov\My Documents\Aptana Studio 3 Workspace\TestJS\ExcelGridView.Data.js", lines);
            }
        }

        [Ignore]
        [Test]
        public void GetLayoutMarkupTest()
        {
            while (true)
            {
                var service = new FormLayoutMarkupService();
                var template = new D_CD_Templates
                    {
                        TemplateFile = Resources.Resource.MROTTemplate,
                        TemplateMarkup = Encoding.UTF8.GetBytes(Resources.Resource.MROTMarkup)
                    };
                LayoutMarkupViewModel markup = service.GetLayoutMarkup(form, template.GetFormMarkupMappings());

                var stringBuilder = new StringBuilder();
                var jsonSerializerSettings = new JsonSerializerSettings();
                JsonSerializer.Create(jsonSerializerSettings).Serialize(new JsonTextWriter(new StringWriter(stringBuilder)), markup);
                var str = stringBuilder.ToString();

                string[] lines = { "Ext.ns('app.grid');", "app.grid.layoutMarkupData = ", str, ";" };
                File.WriteAllLines(@"D:\Projects\ext-3.4.0\examples\grid\ExcelGridView.Data.js", lines);
            }
        }
    }
}
