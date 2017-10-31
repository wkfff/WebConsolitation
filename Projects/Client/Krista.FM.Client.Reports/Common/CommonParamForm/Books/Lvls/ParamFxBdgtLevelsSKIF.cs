using System;
using Krista.FM.Client.Reports.Database.ClsFx;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Lvls
{
    class ParamFxBdgtLevelsSKIF : ParamInfo
    {
        public ParamFxBdgtLevelsSKIF()
        {
            Description = "Уровни бюджета.СКИФ";
            BookInfo = new ParamBookInfo
            {
                WriteFullText = true,
                MultiSelect = true,
                FullScreen = false,
                DefaultSort = fx_BdgtLevels_SKIF.ID,
                ItemTemplate = fx_BdgtLevels_SKIF.Name,
                EntityKey = fx_BdgtLevels_SKIF.InternalKey,
                ItemFilter = String.Format("{0} > 0", fx_BdgtLevels_SKIF.ID)
            };
        }
    }
}
