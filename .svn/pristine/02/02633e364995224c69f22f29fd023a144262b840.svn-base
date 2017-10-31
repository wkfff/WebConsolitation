using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Krista.FM.Common.OfficePluginServices.FMOfficeAddin;
using Krista.FM.Common.RegistryUtils;

namespace Krista.FM.Common.OfficePluginServices
{
    /// <summary>
    /// Класс для работы с офисной надстройкой.
    /// </summary>
    public abstract class PluginService : DisposableObject
    {
        private readonly string fmAddinProgId;
        private static bool? isPluginInstalled;

        internal PluginService(string progId, string regPath)
        {
            fmAddinProgId = progId;
        }

        protected static bool CheckPluginInstalled(string progId, string regPath)
        {
            isPluginInstalled = false;
            if (!Utils.CheckLibByProgID(progId, true))
            {
                return false;
            }
            // проверяем зарегистрированность и доступность в качестве плагина Excel
            // ..в HKLM
            if (CheckPluginInApplication(regPath, true))
                return true;
            if (!(bool)isPluginInstalled && CheckPluginInApplication(regPath, false))
                return true;
            return false;
        }

        /// <summary>
        /// Проверка зарегистрированности плагина в MS Office
        /// </summary>
        /// <param name="regPath"></param>
        /// <param name="forAllUsers">где проверять в HKLM или в HKCU</param>
        /// <returns>true/false</returns>
        private static bool CheckPluginInApplication(string regPath, bool forAllUsers)
        {
            RegistryKey key = forAllUsers ? Registry.LocalMachine.OpenSubKey(regPath, false) : Registry.CurrentUser.OpenSubKey(regPath, false);

            if (key == null)
                return false;

            int loadBehavior = -1;
            try
            {
                loadBehavior = Convert.ToInt32(key.GetValue("LoadBehavior"));
            }
            catch
            {

            }
            key.Close();
            return (loadBehavior == 3);
        }

        public IFMPlanningExtension GetPlanningExtensionInterface(object appObj)
        {
            return GetPlanningExtensionInterface(appObj, fmAddinProgId);
        }


        /// <summary>
        /// Получить обменный интерфейс нашего плагина
        /// </summary>
        protected static IFMPlanningExtension GetPlanningExtensionInterface(object appObj, string progId)
        {
            // в случае криво установленного офиса могут возникать исключения доступа 
            IFMPlanningExtension extension = null;
            // глушим их безжалостно
            try
            {
                // получаем коллекцию  аддинов
                object comAddIns = ReflectionHelper.GetProperty(appObj, "COMAddIns");
                // получаем количество установленных аддинов
                int comAddInsCount = Convert.ToInt32(ReflectionHelper.GetProperty(comAddIns, "Count"));
                // ищем наш плагин
                object comAddIn = null;
                for (int i = 1; i <= comAddInsCount; i++)
                {
                    comAddIn = ReflectionHelper.CallMethod(comAddIns, "Item", i);
                    string objName = Convert.ToString(ReflectionHelper.GetProperty(comAddIn, "ProgID"));
                    if (string.Compare(progId, objName, true) == 0)
                        break;
                    comAddIn = null;
                }
                // если нашли - получаем наш обменный интерфейс
                if (comAddIn != null)
                {
                    object comAddInCoClass = ReflectionHelper.GetProperty(comAddIn, "Object");
                    extension = comAddInCoClass as IFMPlanningExtension;
                }
            }
            catch
            {

            }
            return extension;
        }

        internal static void ResetPlanningSetings(IFMPlanningExtension extension)
        {
            SetPlanningSettings(extension, String.Empty, String.Empty,
                String.Empty, String.Empty, String.Empty, -1, String.Empty);
        }

        internal void SetPlanningSettings(object obj, string taskName, string taskId, string documentName,
            string documentId, string doerName, int sheetType, string originalFileName)
        {
            // загружаем ексел, открываем файл
            object appObj = obj;
            IFMPlanningExtension extension = null;
            try
            {
                extension = GetPlanningExtensionInterface(appObj, fmAddinProgId);
                SetPlanningSettings(extension, taskName, taskId, documentName,
                    documentId, doerName, sheetType, originalFileName);
            }
            finally
            {
                if (extension != null) Marshal.ReleaseComObject(extension);
            }
        }

        public static void SetPlanningSettings(IFMPlanningExtension extension, string taskName, string taskId, string documentName,
            string documentId, string doerName, int sheetType, string originalFileName)
        {
            if (extension == null)
                return;

            extension.SetPropValueByName(FMOfficeAddinConsts.pspDocumentName, documentName);
            extension.SetPropValueByName(FMOfficeAddinConsts.pspDocumentId, documentId);
            extension.SetPropValueByName(FMOfficeAddinConsts.pspTaskName, taskName);
            extension.SetPropValueByName(FMOfficeAddinConsts.pspTaskId, taskId);
            extension.SetPropValueByName(FMOfficeAddinConsts.pspOwner, doerName);
            extension.SetPropValueByName(FMOfficeAddinConsts.pspDocPath, originalFileName);
            if (sheetType != -1)
                extension.SetPropValueByName(FMOfficeAddinConsts.pspSheetType, sheetType.ToString());
        }

        internal void SetPlaginParams(IFMPlanningExtension extension, bool showDocument)
        {
            if (extension != null)
            {
                extension.IsLoadingFromTask = true;
                // 
                extension.IsSilentMode = !showDocument;
            }
        }

        public IFMPlanningExtension SetPlaginParams(object obj, bool showDocument)
        {
            IFMPlanningExtension extension = GetPlanningExtensionInterface(obj, fmAddinProgId);
            SetPlaginParams(extension, showDocument);
            return extension;
        }

        public static void RestoreParams(IFMPlanningExtension extension)
        {
            if (extension != null)
            {
                extension.IsLoadingFromTask = false;
                extension.IsSilentMode = false;
            }
        }
    }
}
