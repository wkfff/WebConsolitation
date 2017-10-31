using Krista.FM.Common.OfficePluginServices.FMOfficeAddin;

namespace Krista.FM.Common.OfficePluginServices
{
    public class ExcelPluginService : PluginService
    {
        public ExcelPluginService()
            : base(FMOfficeAddinConsts.excelAddinProgID, FMOfficeAddinConsts.excelAddinRegPath)
        {
        }

        private static bool? isPluginInstalled;

        public static bool PluginInstalled
        {
            get
            {
                if (isPluginInstalled == null)
                {
                    isPluginInstalled = CheckPluginInstalled(FMOfficeAddinConsts.excelAddinProgID,
                        PlatformDetect.Is64BitProcess ? FMOfficeAddinConsts.excelAddinRegPath64 : FMOfficeAddinConsts.excelAddinRegPath);
                }
                return (bool)isPluginInstalled;
            }
        }

        /// <summary>
        /// Получить обменный интерфейс нашего плагина.
        /// </summary>
        public new static IFMPlanningExtension GetPlanningExtensionInterface(object appObj)
        {
            if (!PluginInstalled)
            {
                return null;
            }

            return GetPlanningExtensionInterface(appObj, FMOfficeAddinConsts.excelAddinProgID);
        }
    }
}
