using System.Configuration;

using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.E86N.Services.DocService
{
    public class DocHelpers
    {
        public static string GetLocalFilePath(F_Doc_Docum doc)
        {
            return ConfigurationManager.AppSettings["DocFilesSavePath"] + "\\" + doc.RefParametr.RefPartDoc.Name + "\\" + doc.RefParametr.ID + "\\";
        }

        /// <summary>
        /// полный путь к прикрепленному файлу
        /// </summary>
        public static string GetFullFilePath(F_Doc_Docum doc)
        {
            // Новые файлы хранятся по иерархическому принципу.
            var fileNameHierarchy = GetLocalFilePath(doc) + doc.Url;
            if (System.IO.File.Exists(fileNameHierarchy))
            {
                return fileNameHierarchy;
            }

            // Файл в иерархии не нашли, значит он может лежать просто по пути хранения без иерархии.
            var fileNameFlat = ConfigurationManager.AppSettings["DocFilesSavePath"] + "\\" + doc.Url;
            if (System.IO.File.Exists(fileNameFlat))
            {
                return fileNameFlat;
            }

            // Файла нет нигде...
            return string.Empty;
        }
    }
}
