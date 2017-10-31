using Krista.FM.Common.OfficePluginServices.FMOfficeAddin;

namespace Krista.FM.Common.OfficePluginServices
{
    public class WordPluginService : PluginService
    {
        public WordPluginService()
            : base(FMOfficeAddinConsts.wordAddinProgID, FMOfficeAddinConsts.wordAddinRegPath)
        {
        }

        private static bool? isPluginInstalled;

        public static bool PluginInstalled
        {
            get
            {
                if (isPluginInstalled == null)
                {
                    isPluginInstalled = CheckPluginInstalled(
                        FMOfficeAddinConsts.wordAddinProgID,
                        FMOfficeAddinConsts.wordAddinRegPath);
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

            return GetPlanningExtensionInterface(appObj, FMOfficeAddinConsts.wordAddinProgID);
        }
    }
}
