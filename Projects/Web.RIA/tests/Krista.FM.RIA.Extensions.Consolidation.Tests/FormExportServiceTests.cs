using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using Krista.FM.Common.Consolidation.Forms.ExcelMapping;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.Metadata;
using Krista.FM.RIA.Extensions.Consolidation.Tests.Helpers;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class FormExportServiceTests
    {
        [Test]
        public void ExportImportTest()
        {
            var form = GetForm();

            var service = new FormExportService();
            var stream = service.Export(form);

            var importService = new FormImportService();
            var resultForm = importService.Import(stream);

            stream.Close();

            Assert.AreEqual(form.InternalName, resultForm.InternalName);
            
            Assert.AreEqual(form.Requisites.Count, resultForm.Requisites.Count);
            Assert.AreEqual(form.Requisites[0].InternalName, resultForm.Requisites[0].InternalName);
            
            Assert.AreEqual(form.Parts.Count, resultForm.Parts.Count);
            Assert.AreEqual(form.Parts[0].InternalName, resultForm.Parts[0].InternalName);
            
            Assert.AreEqual(form.Parts[0].Columns.Count, resultForm.Parts[0].Columns.Count);
            Assert.AreEqual(form.Parts[0].Columns[0].InternalName, resultForm.Parts[0].Columns[0].InternalName);
            
            Assert.AreEqual(form.Parts[0].Rows.Count, resultForm.Parts[0].Rows.Count);
            Assert.AreEqual(form.Parts[0].Rows[0].Name, resultForm.Parts[0].Rows[0].Name);

            Assert.AreEqual(form.Parts[0].Cells.Count, resultForm.Parts[0].Cells.Count);
            Assert.AreEqual(form.Parts[0].Cells[0].DefaultValue, resultForm.Parts[0].Cells[0].DefaultValue);
            Assert.AreEqual(form.Parts[0].Cells[1].DefaultValue, resultForm.Parts[0].Cells[1].DefaultValue);
            Assert.AreEqual(form.Parts[0].Cells[2].DefaultValue, resultForm.Parts[0].Cells[2].DefaultValue);
            Assert.AreEqual(form.Parts[0].Cells[3].DefaultValue, resultForm.Parts[0].Cells[3].DefaultValue);

            Assert.AreEqual(resultForm.Parts[0].Columns[0], resultForm.Parts[0].Cells[0].RefColumn);
            Assert.AreEqual(resultForm.Parts[0].Columns[1], resultForm.Parts[0].Cells[1].RefColumn);

            Assert.AreEqual(resultForm.Parts[0].Rows[0], resultForm.Parts[0].Cells[0].RefRow);
            Assert.AreEqual(resultForm.Parts[0].Rows[1], resultForm.Parts[0].Cells[2].RefRow);

            Assert.AreEqual(resultForm.Parts[0], resultForm.Parts[0].Cells[0].RefSection);

            Assert.AreEqual(form.TemplateFile, resultForm.TemplateFile);
            Assert.AreEqual(form.TemplateMarkup, resultForm.TemplateMarkup);
        }

        [Ignore]
        [Test]
        public void Test()
        {
            var form = GetForm();

            var service = new FormExportService();
            var stream = service.Export(form);

            using (var file = File.Open(@"d:\result.zip", FileMode.OpenOrCreate, FileAccess.Write))
            {
                stream.Position = 0;
                StreamUtils.Copy(stream, file, new byte[4096]);
                stream.Close();
            }
        }

        [Ignore]
        [Test]
        public void DbImportTest()
        {
            NHibernateHelper.SetupNHibernate("Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV");

            var form = Model.MROTTemplate.GetTemplate();
            /*var form = GetForm();*/

            var service = new FormExportService();
            var stream = service.Export(form);

            var importService = new FormImportService();
            var resultForm = importService.Import(stream);

            var repository = new NHibernateRepository<D_CD_Templates>();
            repository.Save(resultForm);
            repository.DbContext.CommitChanges();
        }

        [Ignore]
        [Test]
        public void DeserializationTest()
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(Form), null, Int32.MaxValue, true, true, null);
            using (var memoryStream = new FileStream(@"C:\Users\gbelov\Desktop\prognoz\prognoz.Markup.utf8.xml", FileMode.Open))
            {
                var formObject = (Form)ser.ReadObject(memoryStream);
                var s = formObject.Sheets[0].FooterRequisites.Region;
            }
        }

        [Ignore]
        [Test]
        public void SerializationTest()
        {
            var m = new Form
            {
                Sheets = new List<Sheet>
                {
                    new Sheet
                    {
                        Code = "Code",
                        Region = "Region",
                        Name = "Name",
                        FooterRequisites = new RequisiteMap { Region = "FooterRequisitesRegion" },
                        HeaderRequisites = new RequisiteMap { Region = "HeaderRequisitesRegion" }
                    }
                }
            };

            DataContractSerializer ser = new DataContractSerializer(typeof(Form), null, Int32.MaxValue, true, true, null);
            using (var memoryStream = new FileStream(@"C:\Users\gbelov\Desktop\prognoz\test.Markup.xml", FileMode.Create))
            {
                ser.WriteObject(memoryStream, m);
            }
        }

        private static D_CD_Templates GetForm()
        {
            var form = new D_CD_Templates { Name = "Name", ShortName = "ShortName", NameCD = "NameCD", Code = "Код", Class = "ConsForm", InternalName = "test" };
            form.TemplateFile = Encoding.UTF8.GetBytes("TemplateFile");
            form.TemplateMarkup = Encoding.UTF8.GetBytes("TemplateMarkup");

            form.Requisites.Add(new D_Form_Requisites { Name = "Name", Ord = 0, DataType = "System.Int32", InternalName = "req1", RefForm = form });

            var part = new D_Form_Part { InternalName = "s1", Name = "Name", Ord = 0, RefForm = form };
            form.Parts.Add(part);

            part.Columns.Add(new D_Form_TableColumn { InternalName = "col1", DataType = "String.Int32", Name = "Name", Ord = 0, GroupTag = "1", GroupLevel = 1, RefPart = part });
            part.Columns.Add(new D_Form_TableColumn { InternalName = "col2", DataType = "String.Int32", Name = "Name1", Ord = 1, GroupTag = "1", GroupLevel = 1, RefPart = part });
            
            part.Rows.Add(new D_Form_TableRow { Name = "Name", Multiplicity = false, Ord = 0, RefPart = part });
            part.Rows.Add(new D_Form_TableRow { Name = "Name", Multiplicity = false, Ord = 1, RefPart = part });

            part.Cells.Add(new D_Form_TableCell { DefaultValue = "1", RefColumn = part.Columns[0], RefRow = part.Rows[0], RefSection = part });
            part.Cells.Add(new D_Form_TableCell { DefaultValue = "2", RefColumn = part.Columns[1], RefRow = part.Rows[0], RefSection = part });
            part.Cells.Add(new D_Form_TableCell { DefaultValue = "3", RefColumn = part.Columns[0], RefRow = part.Rows[1], RefSection = part });
            part.Cells.Add(new D_Form_TableCell { DefaultValue = "4", RefColumn = part.Columns[1], RefRow = part.Rows[1], RefSection = part });

            return form;
        }
    }
}
