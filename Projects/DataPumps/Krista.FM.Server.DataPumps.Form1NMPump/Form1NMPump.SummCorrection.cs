using System;

namespace Krista.FM.Server.DataPumps.Form1NMPump
{

    // ФНС - 0003 - Форма 1-НМ (Коррекция сумм)
    public partial class Form1NMPumpModule : TextRepPumpModuleBase
    {

        #region Коррекция сумм

        /// <summary>
        /// Функция коррекции сумм фактов по данным источника
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        protected override void ProcessDataSource()
        {
            SetClsHierarchy();

            F1NMSumCorrectionConfig f1nmSumCorrectionConfig = new F1NMSumCorrectionConfig();

            // Поля сумм будем корректировать отдельно, т.к. в форме 1-НМ для показателей "Начислено" и "Поступило"
            // применяется разная классифкация доходов:
            // 1.Для показателя "Начислено" при корректировке сумм не нужно учитывать раздел 3. Т.е. коды строк
            // 3300, 3400, 3500 не должны вычитаться из 1020
            // 2.Для показателя "Поступило" корректировка сумм должна осуществлять с учетом того, что коды строк
            // 3300, 3400, 3500 подчиняются строке 1020.

            f1nmSumCorrectionConfig.EarnedField = string.Empty;
            f1nmSumCorrectionConfig.EarnedReportField = string.Empty;
            f1nmSumCorrectionConfig.InpaymentsField = "INPAYMENTS";
            f1nmSumCorrectionConfig.InpaymentsReportField = "INPAYMENTSREPORT";

            // делаем дополнительное ограничение, чтобы выбирались записи с заполненным полем "Поступило"
            // это необходимо, т.к. при закачке 2-го раздела суммы Поступило и Начислено формируются на одинаковые уровни
            // всё перемешивается и корректируется неверно
            string factConstr = string.Format(" (SOURCEID = {0}) and (INPAYMENTSREPORT <> 0) ", this.SourceID);

            CorrectFactTableSums(fctIncomesTotal, dsKd.Tables[0], clsKd, "REFKD", f1nmSumCorrectionConfig,
                BlockProcessModifier.MRStandard, null, string.Empty, "REFBUDGETLEVELS", true, factConstr);

            CorrectFactTableSums(fctIncomesRegions, dsKd.Tables[0], clsKd, "REFKD", f1nmSumCorrectionConfig,
                BlockProcessModifier.MRStandard, null, "REFREGIONS", "REFBUDGETLEVELS", true, factConstr);

            f1nmSumCorrectionConfig.EarnedField = "EARNED";
            f1nmSumCorrectionConfig.EarnedReportField = "EARNEDREPORT";
            f1nmSumCorrectionConfig.InpaymentsField = string.Empty;
            f1nmSumCorrectionConfig.InpaymentsReportField = string.Empty;

            // то же самое ограничение делаем, только для поля "Начислено"
            factConstr = string.Format(" (SOURCEID = {0}) and (EARNEDREPORT <> 0) ", this.SourceID);

            CorrectFactTableSums(fctIncomesTotal, dsKd.Tables[0], clsKd, "REFKD", f1nmSumCorrectionConfig,
                BlockProcessModifier.MRStandard, null, string.Empty, "REFBUDGETLEVELS", true, factConstr);

            CorrectFactTableSums(fctIncomesRegions, dsKd.Tables[0], clsKd, "REFKD", f1nmSumCorrectionConfig,
                BlockProcessModifier.MRStandard, null, "REFREGIONS", "REFBUDGETLEVELS", true, factConstr);

            // FMQ00005624 Борисов  - Добавление дополнительных уровней бюджета

            // добавляем дополнительные уровни по полю "Поступило"
            Calc1NMAdditionalBudgetLevels(fctIncomesTotal, dsKd.Tables[0], clsKd, "REFKD",
                BlockProcessModifier.MRStandard, null, string.Empty, "REFBUDGETLEVELS", "INPAYMENTS");
            Calc1NMAdditionalBudgetLevels(fctIncomesRegions, dsKd.Tables[0], clsKd, "REFKD",
                BlockProcessModifier.MRStandard, null, "REFREGIONS", "REFBUDGETLEVELS", "INPAYMENTS");
            // добавляем дополнительные уровни по полю "Начислено"
            Calc1NMAdditionalBudgetLevels(fctIncomesTotal, dsKd.Tables[0], clsKd, "REFKD",
                BlockProcessModifier.MRStandard, null, string.Empty, "REFBUDGETLEVELS", "EARNED");
            Calc1NMAdditionalBudgetLevels(fctIncomesRegions, dsKd.Tables[0], clsKd, "REFKD",
                BlockProcessModifier.MRStandard, null, "REFREGIONS", "REFBUDGETLEVELS", "EARNED");

            UpdateData();
        }

        #endregion Коррекция сумм

    }
}