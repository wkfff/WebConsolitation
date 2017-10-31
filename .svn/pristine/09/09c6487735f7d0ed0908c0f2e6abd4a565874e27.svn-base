using System;
using System.IO;
using System.Reflection;

namespace Krista.FM.Common.TaskDocuments.OfficeCustomPropertiesAdapters.CalumMcLellan.StructuredStorage
{
    public class PropertySetsAdapter : IDisposable
    {
        private readonly object instance;

        public PropertySetsAdapter(string file)
        {
            string platform = PlatformDetect.Is64BitProcess && !PlatformDetect.Is32BitProcessOn64BitProcessor() 
                ? "x64" : "x32";

            var assemblyPath = Path.GetDirectoryName(GetType().Assembly.Location);

            // Если мы работаем в контексте IIS, то некорректро брать путь из свойства Location.
            // Путь доставать нужно из свойства CodeBase
            if (assemblyPath.Contains("\\assembly\\d"))
            {
                assemblyPath = Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).AbsolutePath);
            }

            var handle = Activator.CreateInstanceFrom(
                Path.Combine(assemblyPath, String.Format("Platform\\{0}\\CalumMcLellan.StructuredStorage.dll", platform)),
                "CalumMcLellan.StructuredStorage.PropertySets",
                true, BindingFlags.CreateInstance, null,
                new object[] { file, false },
                null, null, null);

            if (handle == null)
            {
                throw new ApplicationException("Невозможно загрузить сборку CalumMcLellan.StructuredStorage.dll.");
            }

            instance = handle.Unwrap();
        }

        public bool Contains(Guid formatIdentifier)
        {
            return Convert.ToBoolean(
                ReflectionHelper.CallMethod(instance, "Contains", formatIdentifier));
        }

        public PropertySetAdapter Add(Guid formatIdentifier, bool unicode)
        {
            return new PropertySetAdapter(
                ReflectionHelper.CallMethod(instance, "Add", formatIdentifier, unicode));
        }

        public PropertySetAdapter this[Guid formatIdentifier]
        {
            get
            {
                return new PropertySetAdapter(
                    ReflectionHelper.CallMethod(instance, "get_Item", formatIdentifier));
            }
        }

        public void Dispose()
        {
            ReflectionHelper.CallMethod(instance, "Dispose");
        }
    }
}
