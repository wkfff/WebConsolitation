using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Server.DisintRules
{
    internal static class NormativesObjectKeys
    {
        #region общие константы и переменные
        /// <summary>
        /// колонка со значением, которое используется для расщеспления
        /// </summary>
        internal const string VALUE_POSTFIX = "_Value";
        // временная колонка, будет удалена
        internal const string RESULT_VALUE_POSTFIX = "_ResultValue";
        /// <summary>
        /// колонка со значением, подтянутым из другого норматива
        /// </summary>
        internal const string REF_VALUE_POSTFIX = "_RefValue";
        /// <summary>
        /// колонка, используемое в дифференцированых нормативах для хранения собственного значения
        /// </summary>
        internal const string SELF_VALUE_POSTFIX = "_Self_Value";

        #endregion

        internal const string f_Norm_BK = "9573725c-76e4-421f-9ec9-ffdc0950e571";
        internal const string f_Norm_Region = "5c717c31-e716-4cc3-bab1-450bc8357b3a";
        internal const string f_Norm_MR = "d076de1c-9891-40d8-930a-ee43475cc9ed";
        internal const string f_Norm_VariedRegion = "98e6d985-a163-47ca-bf3f-a0071cff9ff4";
        internal const string f_Norm_VariedMR = "24b11318-466c-454d-ac01-c50e3a6c75a2";
        internal const string fx_FX_BudgetLevels = "bd6afa07-4c81-498d-8a50-8d667179af07";
        internal const string d_KD_Analysis = "2553274b-4cee-4d20-a9a6-eef173465d8b";
        internal const string d_Regions_Analysis = "383f887a-3ebb-4dba-8abb-560b5777436f";
        internal const string d_Variant_PlanIncomes = "1525f07f-8a60-47af-9b80-7200e74956bc";
    }
}
