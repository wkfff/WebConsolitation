using System;
using System.IO;
using System.Runtime.Serialization;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.Metadata
{
    public class FormImportService : IFormImportService
    {
        public D_CD_Templates Import(Stream stream)
        {
            byte[] templateFile = null;
            byte[] templateMarkup = null;
            object formObject = null;

            stream.Position = 0;
            var zf = new ZipFile(stream);
            foreach (ZipEntry zipEntry in zf)
            {
                if (!zipEntry.IsFile)
                {
                    // Игнорируем каталоги
                    continue;
                }

                string entryFileName = zipEntry.Name;

                Stream zipStream = zf.GetInputStream(zipEntry);

                byte[] buffer = new byte[4096];
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    StreamUtils.Copy(zipStream, memoryStream, buffer);
                    buffer = new byte[memoryStream.Length];
                    memoryStream.Position = 0;
                    memoryStream.Read(buffer, 0, (int)memoryStream.Length);
                }

                if (entryFileName.EndsWith(".xls"))
                {
                    templateFile = buffer;
                }

                if (entryFileName.EndsWith(".Markup.xml"))
                {
                    templateMarkup = buffer;
                }

                if (entryFileName.EndsWith(".Metadata.xml"))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(D_CD_Templates), null, Int32.MaxValue, true, true, null);
                    var memoryStream = new MemoryStream(buffer);
                    formObject = ser.ReadObject(memoryStream);
                    memoryStream.Close();
                }
            }

            var form = formObject as D_CD_Templates;

            if (form == null)
            {
                // TODO: Кинуть конкретное исключение.
                throw new Exception();
            }

            form.TemplateFile = templateFile;
            form.TemplateMarkup = templateMarkup;

            return form;
        }
    }
}
