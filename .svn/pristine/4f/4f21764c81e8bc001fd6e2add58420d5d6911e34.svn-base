using System;
using Krista.FM.Common.OfficeHelpers;
using Krista.FM.Common.OfficePluginServices;

namespace Krista.FM.Common.TaskDocuments
{
    public class OfficePluginFactory
    {
        public static PluginService Create(string fileName)
        {
            if (ExcelApplication.IsApplicableFile(fileName))
                return new ExcelPluginService();
            if (WordApplication.IsApplicableFile(fileName))
                return new WordPluginService();
            throw new OfficePluginFactoryException(String.Format("Невозможно создать PluginService для файла \"{0}\".", fileName));
        }
    }
}
