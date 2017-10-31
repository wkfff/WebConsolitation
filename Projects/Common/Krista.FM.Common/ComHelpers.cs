using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Krista.FM.Common
{
    #region работа с TypeLibrary
    internal class ConversionEventHandler : ITypeLibExporterNotifySink
    {
        public void ReportEvent(ExporterEventKind eventKind, int eventCode, string eventMsg)
        {
            // Handle the warning event here.
        }

        public Object ResolveRef(Assembly asm)
        {
            // Resolve the reference here and return a correct type library.
            return null;
        }
    }

    /// <summary>
    /// Хэлпер для работы с ТЛБ
    /// </summary>
    public struct TypeLibHelper
    {
        [ComImport, Guid("00020406-0000-0000-C000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(false)]
        internal interface ITypeLibInterface
        {
            void CreateTypeInfo();
            void SetName();
            void SetVersion();
            void SetGuid();
            void SetDocString();
            void SetHelpFileName();
            void SetHelpContext();
            void SetLcid();
            void SetLibFlags();
            void SaveAllChanges();
        }

        internal enum REGKIND
        {
            REGKIND_DEFAULT,
            REGKIND_REGISTER,
            REGKIND_NONE
        }

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void RegisterTypeLib(ITypeLib TypeLib, string szFullPath, string szHelpDirs);

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void LoadTypeLibEx(string strTypeLibName, REGKIND regKind, out ITypeLib TypeLib);

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void UnRegisterTypeLib(ref Guid libID, short wVerMajor, short wVerMinor, int lcid, System.Runtime.InteropServices.ComTypes.SYSKIND syskind);

        /// <summary>
        /// Проверить, существует ли TLB для сборки, если нет - создать
        /// Если существующая старее - обновить.
        /// </summary>
        /// <param name="assemblyName">Название сборки</param>
        public static void CheckTypeLib(Assembly assembly)
        {
            string curPath = AppDomain.CurrentDomain.BaseDirectory;
            FileInfo servLibFileInfo = new FileInfo(curPath + assembly.ManifestModule.Name);
            string tlbName = curPath + Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name) + ".TLB";
            // если тлб-файл есть - проверяем чтобы он был не старее тайплабрари
            if (File.Exists(tlbName))
            {
                FileInfo typeLibFileInfo = new FileInfo(tlbName);
                if (servLibFileInfo.LastWriteTime > typeLibFileInfo.LastWriteTime)
                {
                    // если старее - пытаемся удалить
                    try
                    {
                        typeLibFileInfo.Delete();
                    }
                    catch { }
                }
            }

            if (!File.Exists(tlbName))
            {
                ConversionEventHandler eventHandler = new ConversionEventHandler();
                TypeLibConverter cnv = new TypeLibConverter();
                ITypeLibInterface tlb = (ITypeLibInterface)cnv.ConvertAssemblyToTypeLib(
                    assembly,
                    tlbName, TypeLibExporterFlags.None, eventHandler
                );
                tlb.SaveAllChanges();

                ITypeLib lib = null;
                LoadTypeLibEx(tlbName, REGKIND.REGKIND_NONE, out lib);
                if ((lib != null) && (Marshal.IsComObject(lib)))
                {
                    RegisterTypeLib(lib, tlbName, curPath);
                    Marshal.ReleaseComObject(lib);
                }
            }

            //LoadTypeLibEx(tlbName, REGKIND.REGKIND_NONE, out lib);

            //IntPtr ptr1 = IntPtr.Zero;
            //lib.GetLibAttr(out ptr1);
            //System.Runtime.InteropServices.ComTypes.TYPELIBATTR typelibattr1 = (System.Runtime.InteropServices.ComTypes.TYPELIBATTR)Marshal.PtrToStructure(ptr1, typeof(System.Runtime.InteropServices.ComTypes.TYPELIBATTR));
            //UnRegisterTypeLib(ref typelibattr1.guid, typelibattr1.wMajorVerNum, typelibattr1.wMinorVerNum, typelibattr1.lcid, typelibattr1.syskind);

        }

    }
    #endregion

    public static class ComHelper
    {
        /// <summary>
        /// Освобождение COM-ссылки на объект MS Ofiice
        /// </summary>
        /// <param name="obj">объект MS Ofiice</param>
        public static void ReleaseComReference(ref object obj)
        {
            try
            {
                // пытаемся уничтожить COM-объект
                if (Marshal.IsComObject(obj))
                    Marshal.ReleaseComObject(obj);
                // Всякие Excel'ы не всегда сразу удаляют свои объект
                // Поэтому хотя бы уничтожаем все ссылки в памяти .NET Framework
                // Это тоже не всегда помогает...
            }
            catch
            {

            }
            obj = null;
            GC.GetTotalMemory(true);
        }
    }
}