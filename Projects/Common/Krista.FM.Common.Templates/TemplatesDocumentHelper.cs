using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Common.Templates
{
    public class TemplatesDocumentsHelper
    {
        private static string TemplatesDocumentsPath = string.Empty;
		private static List<string> filters = new List<string>();
		private const string DocsFolderName = "TemplatesDocuments";
        
        private ITemplatesRepository _repository;

		static TemplatesDocumentsHelper()
		{
			filters.Add("Документ MS Word (*.doc)|*.doc");
			filters.Add("Документ MS Excel (*.xls)|*.xls");
			filters.Add("Документ MDX Эксперт (*.exd)|*.exd");
			filters.Add("Документ MDX Эксперт 3(*.exd3)|*.exd3");
			filters.Add("Веб-отчет (*.xml)|*.xml");
			filters.Add("Все файлы (*.*)|*.*");
		}

        public TemplatesDocumentsHelper(ITemplatesRepository repository)
        {
            _repository = repository;
        }


		public static string GetFilters()
		{
			return String.Join("|", filters.ToArray());
		}

		public static int GetFilterIndexForType(TemplateDocumentTypes documentTypes)
		{
			switch (documentTypes)
			{
				case TemplateDocumentTypes.MSWord:
				case TemplateDocumentTypes.MSWordPlaning:
				case TemplateDocumentTypes.MSWordTemplate:
					return 0;
				case TemplateDocumentTypes.MSExcel:
				case TemplateDocumentTypes.MSExcelPlaning:
				case TemplateDocumentTypes.MSExcelTemplate:
					return 1;
				case TemplateDocumentTypes.MDXExpert:
					return 2;
				case TemplateDocumentTypes.MDXExpert3:
					return 3;
				case TemplateDocumentTypes.WebReport:
					return 4;
				default:
					return 5;
			}
		}

        public static string GetDocsFolder()
        {
            if (!string.IsNullOrEmpty(TemplatesDocumentsPath))
                return TemplatesDocumentsPath;
            // проверяем наличие папки локальных документов (если нет - создаем)
            TemplatesDocumentsPath = Assembly.GetExecutingAssembly().Location;
            TemplatesDocumentsPath = Path.GetDirectoryName(TemplatesDocumentsPath);
            TemplatesDocumentsPath = TemplatesDocumentsPath + Path.DirectorySeparatorChar + DocsFolderName;
            // если не существует - создаем
            if (!Directory.Exists(TemplatesDocumentsPath))
                Directory.CreateDirectory(TemplatesDocumentsPath);
            return TemplatesDocumentsPath;
        }

        public static string GetFullFileName(int templateDocumentRowID, string documentColumnName)
        {
            string documentsDirectory = AppDomain.CurrentDomain.BaseDirectory + DocsFolderName;
            string fileName = documentColumnName;
            if (!Directory.Exists(documentsDirectory))
                Directory.CreateDirectory(documentsDirectory);
            // к названию файла добавим ID шаблона

            if (!fileName.Contains(templateDocumentRowID.ToString()))
                fileName = string.Format("{0}_{1}", templateDocumentRowID, fileName);
            
            return documentsDirectory + "\\" + fileName;
        }


        public static void SaveDocument(string fileName, byte[] document)
        {
            FileStream fs = null;
            try
            {
                // если файл существует, переписываем его
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Write);
                fs.Write(document, 0, document.Length);
                document = null;
            }
            finally
            {
                if (fs != null) fs.Close();
                GC.GetTotalMemory(true);
            }
        }

        /// <summary>
        /// сохраняет во временные документы
        /// </summary>
        /// <returns></returns>
        public string SaveDocument(int templateId, string templateName, string fullName)
        {
            return SaveDocument(GetDocsFolder(), templateId, templateName, fullName);
        }

        /// <summary>
        /// сохраняет документ по указанному пути
        /// </summary>
        /// <returns></returns>
        public string SaveDocument(string reportPath, int templateId, string templateName, string fullName)
        {
            string templatesFolder = reportPath;
            byte[] document = DocumentsHelper.DecompressFile(_repository.GetDocument(templateId));
            string fileName = templatesFolder + Path.DirectorySeparatorChar + templateName + Path.GetExtension(fullName);
            try
            {
                // сохраняем файл, если существует - перезаписываем
                SaveDocument(fileName, document);
            }
            catch
            {
                // пока что глушим исключение
            }
            return fileName;
        }
    }
}
