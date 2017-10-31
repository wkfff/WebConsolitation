using System;
using System.IO;
using System.Runtime.Serialization;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.Metadata
{
    public class FormExportService : IFormExportService
    {
        public Stream Export(D_CD_Templates form)
        {
            var templateFileStream = new MemoryStream(form.TemplateFile);
            var templateMarkupStream = new MemoryStream(form.TemplateMarkup);
            var metadataStream = SerializeFormMetadata(form);

            return CreateZipStream(form.InternalName, templateFileStream, templateMarkupStream, metadataStream);
        }

        private static MemoryStream SerializeFormMetadata(D_CD_Templates form)
        {
            var metadataStream = new MemoryStream();

            DataContractSerializer ser = new DataContractSerializer(form.GetType(), null, Int32.MaxValue, true, true, new HibernateDataContractSurrogate());
            ser.WriteObject(metadataStream, form);
            metadataStream.Position = 0;
            
            return metadataStream;
        }

        private static MemoryStream CreateZipStream(string internalName, MemoryStream templateFileStream, MemoryStream templateMarkupStream, MemoryStream metadataStream)
        {
            MemoryStream outputMemStream = new MemoryStream();
            ZipOutputStream zipOutStream = new ZipOutputStream(outputMemStream);

            zipOutStream.SetLevel(6);

            AddZipFile(internalName + ".xls", templateFileStream, zipOutStream);
            AddZipFile(internalName + ".Markup.xml", templateMarkupStream, zipOutStream);
            AddZipFile(internalName + ".Metadata.xml", metadataStream, zipOutStream);

            zipOutStream.IsStreamOwner = false;
            zipOutStream.Close();

            outputMemStream.Position = 0;
            return outputMemStream;
        }

        private static void AddZipFile(string name, MemoryStream templateFileStream, ZipOutputStream zipOutStream)
        {
            ZipEntry newEntry = new ZipEntry(name);
            newEntry.DateTime = DateTime.Now;

            zipOutStream.PutNextEntry(newEntry);

            StreamUtils.Copy(templateFileStream, zipOutStream, new byte[4096]);
            templateFileStream.Close();

            zipOutStream.CloseEntry();
        }
    }
}
