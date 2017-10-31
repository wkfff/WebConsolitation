using System;
using System.IO;
using System.Runtime.Serialization;
using Krista.FM.Common.Consolidation.Forms.ExcelMapping;
using Krista.FM.Domain;

namespace Krista.FM.Common.Consolidation.Forms.Layout
{
    [CLSCompliant(false)]
    public static class MarkupMappingExtensions
    {
        private static readonly DataContractSerializer serializer;

        static MarkupMappingExtensions()
        {
            serializer = new DataContractSerializer(typeof(Form), null, Int32.MaxValue, true, false, null);
        }

        public static Form GetFormMarkupMappings(this D_CD_Templates template)
        {
            if (template.TemplateMarkup == null)
            {
                throw new LayoutException("У формы незадано сопоставление структуры с шаблоном отображения.");
            }

            using (var memoryStream = new MemoryStream(template.TemplateMarkup))
            {
                var mappingObject = serializer.ReadObject(memoryStream);
                return (Form)mappingObject;
            }
        }

        public static void SetFormMarkupMappings(this D_CD_Templates template, Form markupMapping)
        {
            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, markupMapping);
                memoryStream.Position = 0;
                template.TemplateMarkup = new byte[memoryStream.Length];
                memoryStream.Read(template.TemplateMarkup, 0, (int)memoryStream.Length);
            }
        }
    }
}
