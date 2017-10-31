using System.IO;

namespace Krista.FM.Common.TaskDocuments.OfficeCustomPropertiesAdapters
{
    public static class OfficeCustomPropertiesFactory
    {
        public static OfficeCustomPropertiesAdapter Create(string file)
        {
            using (new LogicalCallWithoutContext())
            {
                if (Path.GetExtension(file) == ".xlsx")
                {
                    return new OfficeOpenXmlAdapter(file);
                }
                return new StructuredStorageAdapter(file);
            }
        }
    }
}
