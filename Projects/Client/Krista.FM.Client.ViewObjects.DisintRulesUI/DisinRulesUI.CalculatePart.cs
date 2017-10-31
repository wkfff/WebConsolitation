using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Excel;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Workplace.Gui;


namespace Krista.FM.Client.ViewObjects.DisintRulesUI
{
    public partial class DisintRulesUI : BaseViewObj
    {
        #region расчеты для записей

        /// <summary>
        /// заполнение нормативов бюджетного кодекса
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool CalculateNormativesBK(UltraGridRow row)
        {
            // расчитываем все значения, которые можно расчитать
            //внебюджетные фонды
            decimal offBudget = Convert.ToDecimal(row.Cells["11" + VALUE_POSTFIX].Value) + Convert.ToDecimal(row.Cells["10" + VALUE_POSTFIX].Value) +
                Convert.ToDecimal(row.Cells["9" + VALUE_POSTFIX].Value) + Convert.ToDecimal(row.Cells["8" + VALUE_POSTFIX].Value);

            if (Convert.ToDecimal(row.Cells["7" + VALUE_POSTFIX].Value) != offBudget && offBudget != 0)
                row.Cells["7" + VALUE_POSTFIX].Value = offBudget;
            else
                if (offBudget == 0)
                    offBudget = Convert.ToDecimal(row.Cells["7" + VALUE_POSTFIX].Value);

            decimal settlementBudget = Convert.ToDecimal(row.Cells["16" + VALUE_POSTFIX].Value);
            if (settlementBudget == 0)
                settlementBudget = Convert.ToDecimal(row.Cells["17" + VALUE_POSTFIX].Value);
            if (settlementBudget != Convert.ToDecimal(row.Cells["6" + VALUE_POSTFIX].Value))
                row.Cells["6" + VALUE_POSTFIX].Value = settlementBudget;

            // Бюджет района + Бюджет поселения
            decimal consBudgetMR = Convert.ToDecimal(row.Cells["5" + VALUE_POSTFIX].Value) + Convert.ToDecimal(row.Cells["6" + VALUE_POSTFIX].Value);
            if (Convert.ToDecimal(row.Cells["4" + VALUE_POSTFIX].Value) != consBudgetMR)
                row.Cells["4" + VALUE_POSTFIX].Value = consBudgetMR;

            // любое не нулевое значение из значений «Конс. бюджет МР» ИЛИ «Бюджет ГО»
            decimal consBudgetMO = Convert.ToDecimal(row.Cells["15" + VALUE_POSTFIX].Value);
            if (consBudgetMO == 0)
                consBudgetMO = Convert.ToDecimal(row.Cells["4" + VALUE_POSTFIX].Value);
            if (Convert.ToDecimal(row.Cells["14" + VALUE_POSTFIX].Value) != consBudgetMO)
                row.Cells["14" + VALUE_POSTFIX].Value = consBudgetMO;

            // Бюджет субъекта + Конс. бюджет МО
            decimal regionBudget = Convert.ToDecimal(row.Cells["14" + VALUE_POSTFIX].Value) + Convert.ToDecimal(row.Cells["3" + VALUE_POSTFIX].Value);
            if (Convert.ToDecimal(row.Cells["2" + VALUE_POSTFIX].Value) != regionBudget)
                row.Cells["2" + VALUE_POSTFIX].Value = regionBudget;

            // 100% - Конс.бюджет субъекта - Внебюдж.фонды - УФК Смоленск - Областной бюджет Тюменской обл.
            decimal federalBudget = Math.Round(100 - Convert.ToDecimal(row.Cells["2" + VALUE_POSTFIX].Value) - offBudget -
                Convert.ToDecimal(row.Cells["12" + VALUE_POSTFIX].Value) - Convert.ToDecimal(row.Cells["13" + VALUE_POSTFIX].Value), 2);
            if (Convert.ToDecimal(row.Cells["1" + VALUE_POSTFIX].Value) != federalBudget)
                row.Cells["1" + VALUE_POSTFIX].Value = federalBudget;

            row.Update();

            if (federalBudget < 0)
                return false;
            return true;
        }

        /// <summary>
        /// проверка правильности ввода норматива по БК
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool CheckNormativesBKRow(DataRow row)
        {
            if (row.RowState == DataRowState.Deleted) return true;
            // расчитываем все значения, которые можно расчитать
            //внебюджетные фонды
            decimal offBudget = Convert.ToDecimal(row["11" + VALUE_POSTFIX]) + Convert.ToDecimal(row["10" + VALUE_POSTFIX]) +
                Convert.ToDecimal(row["9" + VALUE_POSTFIX]) + Convert.ToDecimal(row["8" + VALUE_POSTFIX]);
            if (offBudget == 0)
                offBudget = Convert.ToDecimal(row["7" + VALUE_POSTFIX]);

            decimal settlementBudget = Convert.ToDecimal(row["16" + VALUE_POSTFIX]);
            if (settlementBudget == 0)
                settlementBudget = Convert.ToDecimal(row["17" + VALUE_POSTFIX]);

            // Бюджет района + Бюджет поселения
            decimal consBudgetMR = Convert.ToDecimal(row["5" + VALUE_POSTFIX]) + Convert.ToDecimal(row["6" + VALUE_POSTFIX]);

            // любое не нулевое значение из значений «Конс. бюджет МР» ИЛИ «Бюджет ГО»
            decimal consBudgetMO = Convert.ToDecimal(row["15" + VALUE_POSTFIX]);
            if (consBudgetMO == 0)
                consBudgetMO = Convert.ToDecimal(row["4" + VALUE_POSTFIX]);

            // Бюджет субъекта + Конс. бюджет МО
            decimal regionBudget = Convert.ToDecimal(row["14" + VALUE_POSTFIX]) + Convert.ToDecimal(row["3" + VALUE_POSTFIX]);

            // 100% - Конс.бюджет субъекта - Внебюдж.фонды - УФК Смоленск - Областной бюджет Тюменской обл.
            decimal federalBudget = 100 - regionBudget - offBudget -
                Convert.ToDecimal(row["12" + VALUE_POSTFIX]) - Convert.ToDecimal(row["13" + VALUE_POSTFIX]);

            if (federalBudget < 0)
                return false;
            return true;
        }


        /// <summary>
        /// заполнение нормативов субъекта РФ
        /// </summary>
        /// <param name="row"></param>
        private bool CalculateNormativesRegionRF(UltraGridRow row)
        {
            // любое не нулевое значение из значений «Бюджет городского поселения» ИЛИ «Бюджет сельского поселения»
            decimal settlementBudget = Convert.ToDecimal(row.Cells["16" + VALUE_POSTFIX].Value);
            if (settlementBudget == 0)
                settlementBudget = Convert.ToDecimal(row.Cells["17" + VALUE_POSTFIX].Value);
            if (settlementBudget != Convert.ToDecimal(row.Cells["6" + VALUE_POSTFIX].Value))
                row.Cells["6" + VALUE_POSTFIX].Value = settlementBudget;

            // Бюджет района + Бюджет поселения
            decimal consBudgetMR = Convert.ToDecimal(row.Cells["5" + VALUE_POSTFIX].Value) + Convert.ToDecimal(row.Cells["6" + VALUE_POSTFIX].Value);
            if (Convert.ToDecimal(row.Cells["4" + VALUE_POSTFIX].Value) != consBudgetMR)
                row.Cells["4" + VALUE_POSTFIX].Value = consBudgetMR;

            // любое не нулевое значение из значений «Конс. бюджет МР» ИЛИ «Бюджет ГО»
            decimal consBudgetMO = Convert.ToDecimal(row.Cells["15" + VALUE_POSTFIX].Value);
            if (consBudgetMO == 0)
                consBudgetMO = Convert.ToDecimal(row.Cells["4" + VALUE_POSTFIX].Value);
            if (Convert.ToDecimal(row.Cells["14" + VALUE_POSTFIX].Value) != consBudgetMO)
                row.Cells["14" + VALUE_POSTFIX].Value = consBudgetMO;

            // плюс еще какой то свой уникальный расчет
            decimal regionBudget = -1;
            if (row.Cells["2" + VALUE_POSTFIX].Value != DBNull.Value)
            {
                regionBudget = Convert.ToDecimal(row.Cells["2" + VALUE_POSTFIX].Value) - Convert.ToDecimal(row.Cells["14" + VALUE_POSTFIX].Value);
                if (Convert.ToDecimal(row.Cells["3" + VALUE_POSTFIX].Value) != regionBudget)
                    row.Cells["3" + VALUE_POSTFIX].Value = regionBudget;
            }

            row.Update();

            if (regionBudget < 0)
                return false;
            return true;
        }


        /// <summary>
        /// проверка нормативов субъекта РФ
        /// </summary>
        /// <param name="row"></param>
        private bool CheckNormativesRegionRFRow(DataRow row)
        {
            if (row.RowState == DataRowState.Deleted) return true;
            // любое не нулевое значение из значений «Бюджет городского поселения» ИЛИ «Бюджет сельского поселения»
            decimal settlementBudget = Convert.ToDecimal(row["16" + VALUE_POSTFIX]);
            if (settlementBudget == 0)
                settlementBudget = Convert.ToDecimal(row["17" + VALUE_POSTFIX]);

            // Бюджет района + Бюджет поселения
            decimal consBudgetMR = Convert.ToDecimal(row["5" + VALUE_POSTFIX]) + Convert.ToDecimal(row["6" + VALUE_POSTFIX]);

            // любое не нулевое значение из значений «Конс. бюджет МР» ИЛИ «Бюджет ГО»
            decimal consBudgetMO = Convert.ToDecimal(row["15" + VALUE_POSTFIX]);
            if (consBudgetMO == 0)
                consBudgetMO = Convert.ToDecimal(row["4" + VALUE_POSTFIX]);

            // плюс еще какой то свой уникальный расчет
            decimal regionBudget = -1;
            if (row["2" + VALUE_POSTFIX] != DBNull.Value)
                regionBudget = Convert.ToDecimal(row["2" + VALUE_POSTFIX]) - consBudgetMO;

            if (regionBudget < 0)
                return false;
            return true;
        }


        /// <summary>
        /// заполнение нормативов муниципального района
        /// </summary>
        /// <param name="row"></param>
        private bool CalculateNormativesMR(UltraGridRow row)
        {
            // любое не нулевое значение из значений «Бюджет городского поселения» ИЛИ «Бюджет сельского поселения»
            decimal settlementBudget = Convert.ToDecimal(row.Cells["16" + VALUE_POSTFIX].Value);
            if (settlementBudget == 0)
                settlementBudget = Convert.ToDecimal(row.Cells["17" + VALUE_POSTFIX].Value);
            if (settlementBudget != Convert.ToDecimal(row.Cells["6" + VALUE_POSTFIX].Value))
                row.Cells["6" + VALUE_POSTFIX].Value = settlementBudget;

            // плюс еще какой то свой уникальный расчет
            decimal areaBudget = -1;
            if (row.Cells["4" + VALUE_POSTFIX].Value != DBNull.Value)
            {
                areaBudget = Convert.ToDecimal(row.Cells["4" + VALUE_POSTFIX].Value) - Convert.ToDecimal(row.Cells["6" + VALUE_POSTFIX].Value);
                if (Convert.ToDecimal(row.Cells["5" + VALUE_POSTFIX].Value) != areaBudget)
                    row.Cells["5" + VALUE_POSTFIX].Value = areaBudget;
            }

            row.Update();

            if (areaBudget < 0)
                return false;
            return true;
        }


        /// <summary>
        /// проверка норматива 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool CheckNormativesMRRow(DataRow row)
        {
            if (row.RowState == DataRowState.Deleted) return true;
            // любое не нулевое значение из значений «Бюджет городского поселения» ИЛИ «Бюджет сельского поселения»
            decimal settlementBudget = Convert.ToDecimal(row["16" + VALUE_POSTFIX]);
            if (settlementBudget == 0)
                settlementBudget = Convert.ToDecimal(row["17" + VALUE_POSTFIX]);

            // плюс еще какой то свой уникальный расчет
            decimal areaBudget = -1;
            if (row["4" + VALUE_POSTFIX] != DBNull.Value)
                areaBudget = Convert.ToDecimal(row["4" + VALUE_POSTFIX]) - settlementBudget;

            if (areaBudget < 0)
                return false;
            return true;
        }


        private bool CalculateVarNormativesRegionRF(UltraGridRow row)
        {
            // некоторые значения будут считаться из нескольких...

            // бюджет городского поселения
            decimal townBubget = Convert.ToDecimal(row.Cells["16" + VALUE_POSTFIX].Value);
            decimal townBubgetAdd = 0;
            if (row.Cells["16" + REF_VALUE_POSTFIX].Value != DBNull.Value)
                townBubgetAdd = Convert.ToDecimal(row.Cells["16" + REF_VALUE_POSTFIX].Value);
            decimal resultTownBudget = townBubget + townBubgetAdd;
            if (Convert.ToDecimal(row.Cells["16" + RESULT_VALUE_POSTFIX].Value) != resultTownBudget)
                row.Cells["16" + RESULT_VALUE_POSTFIX].Value = resultTownBudget;
            // бюджет сельского поселения
            decimal ruralBudget = Convert.ToDecimal(row.Cells["17" + VALUE_POSTFIX].Value);
            decimal ruralBudgetAdd = 0;
            if (row.Cells["17" + REF_VALUE_POSTFIX].Value != DBNull.Value)
                ruralBudgetAdd = Convert.ToDecimal(row.Cells["17" + REF_VALUE_POSTFIX].Value);
            decimal resultRuralBudget = ruralBudget + ruralBudgetAdd;
            if (Convert.ToDecimal(row.Cells["17" + RESULT_VALUE_POSTFIX].Value) != resultRuralBudget)
                row.Cells["17" + RESULT_VALUE_POSTFIX].Value = resultRuralBudget;
            // бюджет поселения
            decimal settlementBudget = townBubget;
            if (settlementBudget == 0)
                settlementBudget = ruralBudget;
            if (Convert.ToDecimal(row.Cells["6" + VALUE_POSTFIX].Value) != settlementBudget)
                row.Cells["6" + VALUE_POSTFIX].Value = settlementBudget;
            decimal settlementBudgetAdd = 0;
            if (row.Cells["6" + REF_VALUE_POSTFIX].Value != DBNull.Value)
                settlementBudgetAdd = Convert.ToDecimal(row.Cells["6" + REF_VALUE_POSTFIX].Value);
            decimal resultSettlementBudget = settlementBudgetAdd + settlementBudget;
            if (Convert.ToDecimal(row.Cells["6" + RESULT_VALUE_POSTFIX].Value) != resultSettlementBudget)
                row.Cells["6" + RESULT_VALUE_POSTFIX].Value = resultSettlementBudget;
            // бюджет района
            decimal areaBudget = Convert.ToDecimal(row.Cells["5" + VALUE_POSTFIX].Value);
            decimal areaBudgetAdd = 0;
            if (row.Cells["5" + REF_VALUE_POSTFIX].Value != DBNull.Value)
                areaBudgetAdd = Convert.ToDecimal(row.Cells["5" + REF_VALUE_POSTFIX].Value);
            decimal resultAreaBudget = areaBudget + areaBudgetAdd;
            if (Convert.ToDecimal(row.Cells["5" + RESULT_VALUE_POSTFIX].Value) != resultAreaBudget)
                row.Cells["5" + RESULT_VALUE_POSTFIX].Value = resultAreaBudget;
            // Конс. бюджет МР
            decimal consBudgetMR = resultAreaBudget + resultSettlementBudget;
            if (Convert.ToDecimal(row.Cells["4" + VALUE_POSTFIX].Value) != consBudgetMR)
                row.Cells["4" + VALUE_POSTFIX].Value = consBudgetMR;
            // бюджет ГО
            decimal goBubget = Convert.ToDecimal(row.Cells["15" + VALUE_POSTFIX].Value);
            decimal goBubgetAdd = 0;
            if (row.Cells["15" + REF_VALUE_POSTFIX].Value != DBNull.Value)
                goBubgetAdd = Convert.ToDecimal(row.Cells["15" + REF_VALUE_POSTFIX].Value);
            decimal resultGOBudget = goBubget + goBubgetAdd;
            if (Convert.ToDecimal(row.Cells["15" + RESULT_VALUE_POSTFIX].Value) != resultGOBudget)
                row.Cells["15" + RESULT_VALUE_POSTFIX].Value = resultGOBudget;
            // любое не нулевое значение из значений «Конс. бюджет МР» ИЛИ «Бюджет ГО»
            decimal consBudgetMO = consBudgetMR;
            if (consBudgetMO < resultGOBudget)
                consBudgetMO = resultGOBudget;
            if (Convert.ToDecimal(row.Cells["14" + VALUE_POSTFIX].Value) != consBudgetMO)
                row.Cells["14" + VALUE_POSTFIX].Value = consBudgetMO;
            // плюс еще какой то свой уникальный расчет
            decimal regionBudget = -1;
            if (row.Cells["2" + VALUE_POSTFIX].Value != DBNull.Value)
            {
                regionBudget = Convert.ToDecimal(row.Cells["2" + VALUE_POSTFIX].Value) - consBudgetMO;
                if (Convert.ToDecimal(row.Cells["3" + VALUE_POSTFIX].Value) != regionBudget)
                    row.Cells["3" + VALUE_POSTFIX].Value = regionBudget;
            }

            row.Update();

            if (regionBudget < 0)
                return false;
            return true;
        }


        private bool CheckVarNormativesRegionRFRow(DataRow row)
        {
            if (row.RowState == DataRowState.Deleted) return true;
            // некоторые значения будут считаться из нескольких...
            // бюджет городского поселения
            decimal townBubget = Convert.ToDecimal(row["16" + VALUE_POSTFIX]);
            decimal townBubgetAdd = 0;
            if (row["16" + REF_VALUE_POSTFIX] != DBNull.Value)
                townBubgetAdd = Convert.ToDecimal(row["16" + REF_VALUE_POSTFIX]);
            decimal resultTownBudget = townBubget + townBubgetAdd;
            // бюджет сельского поселения
            decimal ruralBudget = Convert.ToDecimal(row["17" + VALUE_POSTFIX]);
            decimal ruralBudgetAdd = 0;
            if (row["17" + REF_VALUE_POSTFIX] != DBNull.Value)
                ruralBudgetAdd = Convert.ToDecimal(row["17" + REF_VALUE_POSTFIX]);
            decimal resultRuralBudget = ruralBudget + ruralBudgetAdd;
            // бюджет поселения
            decimal settlementBudget = townBubget;
            if (settlementBudget == 0)
                settlementBudget = ruralBudget;
            decimal settlementBudgetAdd = 0;
            if (row["6" + REF_VALUE_POSTFIX] != DBNull.Value)
                settlementBudgetAdd = Convert.ToDecimal(row["6" + REF_VALUE_POSTFIX]);
            decimal resultSettlementBudget = settlementBudgetAdd + settlementBudget;
            // бюджет района
            decimal areaBudget = Convert.ToDecimal(row["5" + VALUE_POSTFIX]);
            decimal areaBudgetAdd = 0;
            if (row["5" + REF_VALUE_POSTFIX] != DBNull.Value)
                areaBudgetAdd = Convert.ToDecimal(row["5" + REF_VALUE_POSTFIX]);
            decimal resultAreaBudget = areaBudget + areaBudgetAdd;
            // Конс. бюджет МР
            decimal consBudgetMR = resultAreaBudget + resultSettlementBudget;
            // бюджет ГО
            decimal goBubget = Convert.ToDecimal(row["15" + VALUE_POSTFIX]);
            decimal goBubgetAdd = 0;
            if (row["15" + REF_VALUE_POSTFIX] != DBNull.Value)
                goBubgetAdd = Convert.ToDecimal(row["15" + REF_VALUE_POSTFIX]);
            decimal resultGOBudget = goBubget + goBubgetAdd;
            // любое не нулевое значение из значений «Конс. бюджет МР» ИЛИ «Бюджет ГО»
            decimal consBudgetMO = consBudgetMR;
            if (consBudgetMO < resultGOBudget)
                consBudgetMO = resultGOBudget;
            // плюс еще какой то свой уникальный расчет
            decimal regionBudget = -1;
            if (row["2" + VALUE_POSTFIX] != DBNull.Value)
                regionBudget = Convert.ToDecimal(row["2" + VALUE_POSTFIX]) - consBudgetMO;

            if (regionBudget < 0)
                return false;
            return true;
        }


        private bool CalculateVarNormativesMR(UltraGridRow row)
        {
            // бюджет городского поселения
            decimal townBubget = Convert.ToDecimal(row.Cells["16" + VALUE_POSTFIX].Value);
            decimal townBubgetAdd = 0;
            if (row.Cells["16" + REF_VALUE_POSTFIX].Value != DBNull.Value)
                townBubgetAdd = Convert.ToDecimal(row.Cells["16" + REF_VALUE_POSTFIX].Value);
            decimal resultTownBudget = townBubget + townBubgetAdd;
            if (Convert.ToDecimal(row.Cells["16" + RESULT_VALUE_POSTFIX].Value) != resultTownBudget)
                row.Cells["16" + RESULT_VALUE_POSTFIX].Value = resultTownBudget;
            // бюджет сельского поселения
            decimal ruralBudget = Convert.ToDecimal(row.Cells["17" + VALUE_POSTFIX].Value);
            decimal ruralBudgetAdd = 0;
            if (row.Cells["17" + REF_VALUE_POSTFIX].Value != DBNull.Value)
                ruralBudgetAdd = Convert.ToDecimal(row.Cells["17" + REF_VALUE_POSTFIX].Value);
            decimal resultRuralBudget = ruralBudget + ruralBudgetAdd;
            if (Convert.ToDecimal(row.Cells["17" + RESULT_VALUE_POSTFIX].Value) != resultRuralBudget)
                row.Cells["17" + RESULT_VALUE_POSTFIX].Value = resultRuralBudget;
            // бюджет поселения
            decimal settlementBudget = townBubget;
            if (settlementBudget == 0)
                settlementBudget = ruralBudget;
            if (Convert.ToDecimal(row.Cells["6" + VALUE_POSTFIX].Value) != settlementBudget)
                row.Cells["6" + VALUE_POSTFIX].Value = settlementBudget;
            decimal settlementBudgetAdd = 0;
            if (row.Cells["6" + REF_VALUE_POSTFIX].Value != DBNull.Value)
                settlementBudgetAdd = Convert.ToDecimal(row.Cells["6" + REF_VALUE_POSTFIX].Value);
            decimal resultSettlementBudget = settlementBudgetAdd + settlementBudget;
            if (Convert.ToDecimal(row.Cells["6" + RESULT_VALUE_POSTFIX].Value) != resultSettlementBudget)
                row.Cells["6" + RESULT_VALUE_POSTFIX].Value = resultSettlementBudget;
            // бюджет района
            decimal areaBudget = -1;
            if (row.Cells["4" + VALUE_POSTFIX].Value != DBNull.Value)
            {
                areaBudget = Convert.ToDecimal(row.Cells["4" + VALUE_POSTFIX].Value) - resultSettlementBudget;
                if (Convert.ToDecimal(row.Cells["5" + VALUE_POSTFIX].Value) != areaBudget)
                    row.Cells["5" + VALUE_POSTFIX].Value = areaBudget;
            }

            if (areaBudget < 0)
                return false;
            return true;
        }


        private bool CheckVarNormativesMRRow(DataRow row)
        {
            if (row.RowState == DataRowState.Deleted) return true;
            // бюджет городского поселения
            decimal townBubget = Convert.ToDecimal(row["16" + VALUE_POSTFIX]);
            decimal townBubgetAdd = 0;
            if (row["16" + REF_VALUE_POSTFIX] != DBNull.Value)
                townBubgetAdd = Convert.ToDecimal(row["16" + REF_VALUE_POSTFIX]);
            decimal resultTownBudget = townBubget + townBubgetAdd;
            // бюджет сельского поселения
            decimal ruralBudget = Convert.ToDecimal(row["17" + VALUE_POSTFIX]);
            decimal ruralBudgetAdd = 0;
            if (row["17" + REF_VALUE_POSTFIX] != DBNull.Value)
                ruralBudgetAdd = Convert.ToDecimal(row["17" + REF_VALUE_POSTFIX]);
            decimal resultRuralBudget = ruralBudget + ruralBudgetAdd;
            // бюджет поселения
            decimal settlementBudget = townBubget;
            if (settlementBudget == 0)
                settlementBudget = ruralBudget;
            decimal settlementBudgetAdd = 0;
            if (row["6" + REF_VALUE_POSTFIX] != DBNull.Value)
                settlementBudgetAdd = Convert.ToDecimal(row["6" + REF_VALUE_POSTFIX]);
            decimal resultSettlementBudget = settlementBudgetAdd + settlementBudget;
            // бюджет района
            decimal areaBudget = -1;
            if (row["4" + VALUE_POSTFIX] != DBNull.Value)
                areaBudget = Convert.ToDecimal(row["4" + VALUE_POSTFIX]) - resultSettlementBudget;

            if (areaBudget < 0)
                return false;
            return true;
        }


        #endregion

        #region проверки на корректность данных

        bool CheckNormativesBK(List<string> errorList, DataTable normatiesTable, bool checkAllNormatives)
        {
            bool returnValue = true;
            foreach (DataRow row in normatiesTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;
                if (!checkAllNormatives)
                    if (row.RowState == DataRowState.Unchanged) continue;
                // проверка на уникальность КД в пределах одного года
                int refKD = Convert.ToInt32(row["RefKD"]);
                int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                if (normatiesTable.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1}", refKD, refYearDayUNV)).Length > 1)
                {
                    errorList.Add(string.Format("{0}. Код дохода должен быть уникальным в пределах года", GetRowInfo(row)));
                    returnValue = false;
                }
                // проверка того, что бы все нормативы соответствовали правилам их расчета
                if (!CheckNormativesBKRow(row))
                    errorList.Add(string.Format("{0}. Неверный норматив доходов. Сумма процентов отчислений в бюджеты разных уровней не должна превышать 100%. Значения нормативов не могут быть отрицательными", GetRowInfo(row)));

                decimal offBudget = Convert.ToDecimal(row["11" + VALUE_POSTFIX]) + Convert.ToDecimal(row["10" + VALUE_POSTFIX]) +
                Convert.ToDecimal(row["9" + VALUE_POSTFIX]) + Convert.ToDecimal(row["8" + VALUE_POSTFIX]);
                if (Convert.ToDecimal(row["7" + VALUE_POSTFIX]) != offBudget && offBudget != 0)
                    errorList.Add(string.Format("{0}. Неверный норматив доходов. Сумма процентов отчислений в различные внебюджетные фонды должна быть равна значению отчислений во внебюджетные фонды или равна нулю", GetRowInfo(row)));

                CheckBubgetMO(row, errorList);

                CheckSettlementBudget(row, errorList);
            }
            return returnValue;
        }


        bool CheckNormativesRegionRF(List<string> errorList, DataTable normatiesTable, bool checkAllNormatives)
        {
            bool returnValue = true;
            foreach (DataRow row in normatiesTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;
                if (!checkAllNormatives)
                    if (row.RowState == DataRowState.Unchanged) continue;
                // проверка на уникальность КД в пределах одного года
                int refKD = Convert.ToInt32(row["RefKD"]);
                int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);

                if (!PresentParentNormativeForRow(row, NormativesKind.NormativesRegionRF))
                    errorList.Add(string.Format("{0}. Для записи не найдена родительская запись в нормативах", GetRowInfo(row)));

                if (normatiesTable.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1}", refKD, refYearDayUNV)).Length > 1)
                {
                    errorList.Add(string.Format("{0}. Код дохода должен быть уникальным в пределах года", GetRowInfo(row)));
                    returnValue = false;
                }
                // проверка того, что бы все нормативы соответствовали правилам их расчета
                if (!CheckNormativesRegionRFRow(row))
                    errorList.Add(string.Format("{0}. Неверный норматив доходов. Сумма процентов отчислений в бюджеты разных уровней не должна превышать 100%. Значения нормативов не могут быть отрицательными", GetRowInfo(row)));

                CheckBubgetMO(row, errorList);

                CheckSettlementBudget(row, errorList);
                List<string> errors = new List<string>();
                if (!CheckCorrectNormative(errors, row, NormativesKind.NormativesRegionRF))
                {
                    foreach (string error in errors)
                    {
                        errorList.Add(string.Format("{0}. {1}", GetRowInfo(row), error));
                    }
                }
            }
            return returnValue;
        }


        private bool CheckNormativesMR(List<string> errorList, DataTable normatiesTable, bool checkAllNormatives)
        {
            bool returnValue = true;
            foreach (DataRow row in normatiesTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;
                if (!checkAllNormatives)
                    if (row.RowState == DataRowState.Unchanged) continue;
                // проверка на уникальность КД в пределах одного года
                int refKD = Convert.ToInt32(row["RefKD"]);
                int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);

                if (!PresentParentNormativeForRow(row, NormativesKind.NormativesMR))
                    errorList.Add(string.Format("{0}. Для записи не найдена родительская запись в нормативах", GetRowInfo(row)));

                if (normatiesTable.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1}", refKD, refYearDayUNV)).Length > 1)
                {
                    errorList.Add(string.Format("{0}. Код дохода должен быть уникальным в пределах года", GetRowInfo(row)));
                    returnValue = false;
                }
                // проверка того, что бы все нормативы соответствовали правилам их расчета
                if (!CheckNormativesMRRow(row))
                    errorList.Add(string.Format("{0}. Неверный норматив доходов. Сумма процентов отчислений в бюджеты разных уровней не должна превышать 100%", GetRowInfo(row)));

                CheckSettlementBudget(row, errorList);
                List<string> errors = new List<string>();
                if (!CheckCorrectNormative(errors, row, NormativesKind.NormativesMR))
                {
                    foreach (string error in errors)
                    {
                        errorList.Add(string.Format("{0}. {1}", GetRowInfo(row), error));
                    }
                }
            }
            return returnValue;
        }


        private bool CheckVarNormativesRegionRF(List<string> errorList, DataTable normatiesTable, bool checkAllNormatives)
        {
            bool returnValue = true;
            foreach (DataRow row in normatiesTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;
                if (!checkAllNormatives)
                    if (row.RowState == DataRowState.Unchanged) continue;
                // проверка на уникальность КД в пределах одного года
                int refKD = Convert.ToInt32(row["RefKD"]);
                int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                int refRegions = Convert.ToInt32(row["RefRegions"]);

                if (!PresentParentNormativeForRow(row, NormativesKind.VarNormativesRegionRF))
                    errorList.Add(string.Format("{0}. Для записи не найдена родительская запись в нормативах", GetRowInfo(row)));

                if (normatiesTable.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1} and RefRegions = {2}", refKD, refYearDayUNV, refRegions)).Length > 1)
                {
                    errorList.Add(string.Format("{0}. Код дохода должен быть уникальным в пределах года по району", GetRowInfo(row)));
                    returnValue = false;
                }
                // проверим, нету ли такого норматива в д
                if (!CheckDiffNormatives(NormativesKind.VarNormativesRegionRF, refKD, refYearDayUNV, refRegions))
                    errorList.Add(string.Format("{0}. Код дохода должен быть уникальным в пределах года по району, найдены дубликаты в других нормативах", GetRowInfo(row)));
                // проверка того, что бы все нормативы соответствовали правилам их расчета
                if (!CheckVarNormativesRegionRFRow(row))
                    errorList.Add(string.Format("{0}. Неверный норматив доходов. Сумма процентов отчислений в бюджеты разных уровней не должна превышать 100%. Значения нормативов не могут быть отрицательными", GetRowInfo(row)));   
                // проверка ввода вычисляемых и вводимых значений
                CheckSettlementBudget(row, errorList);
            }
            return returnValue;
        }


        private bool CheckVarNormativesMR(List<string> errorList, DataTable normatiesTable, bool checkAllNormatives)
        {
            bool returnValue = true;
            foreach (DataRow row in normatiesTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;
                if (!checkAllNormatives)
                    if (row.RowState == DataRowState.Unchanged) continue;
                // проверка на уникальность КД в пределах одного года
                int refKD = Convert.ToInt32(row["RefKD"]);
                int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                int refRegions = Convert.ToInt32(row["RefRegions"]);

                if (!PresentParentNormativeForRow(row, NormativesKind.VarNormativesMR))
                    errorList.Add(string.Format("{0}. Для записи не найдена родительская запись в нормативах", GetRowInfo(row)));

                if (normatiesTable.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1} and RefRegions = {2}", refKD, refYearDayUNV, refRegions)).Length > 1)
                {
                    errorList.Add(string.Format("{0}. Код дохода должен быть уникальным в пределах года по району", GetRowInfo(row)));
                    returnValue = false;
                }
                // проверка того, есть ли такой же норматив в другом дифференцированном нормативе
                if (!CheckDiffNormatives(NormativesKind.VarNormativesMR, refKD, refYearDayUNV, refRegions))
                    errorList.Add(string.Format("{0}. Код дохода должен быть уникальным в пределах года по району, найдены дубликаты в других нормативах", GetRowInfo(row)));
                // проверка того, что бы все нормативы соответствовали правилам их расчета
                if (!CheckVarNormativesMRRow(row))
                    errorList.Add(string.Format("{0}. Неверный норматив доходов. Сумма процентов отчислений в бюджеты разных уровней не должна превышать 100%. Значения нормативов не могут быть отрицательными", GetRowInfo(row)));
                CheckSettlementBudget(row, errorList);
            }
            return returnValue;
        }


        private bool CheckSettlementBudget(DataRow row, List<string> errorList)
        {
            decimal townBubget = Convert.ToDecimal(row["16" + VALUE_POSTFIX]);
            decimal ruralBudget = Convert.ToDecimal(row["17" + VALUE_POSTFIX]);
            decimal settlementBudget = Convert.ToDecimal(row["6" + VALUE_POSTFIX]);
            if (townBubget != 0 && ruralBudget != 0 && townBubget != ruralBudget)
            {
                switch (currentNormatives)
                {
                    case NormativesKind.VarNormativesMR:
                    case NormativesKind.VarNormativesRegionRF:
                        errorList.Add(string.Format("{0}. Значение поля '% бюджет городского поселения доп.' должно быть равно значению поля '% бюджет сельского поселения доп.' или нулю", GetRowInfo(row)));
                        break;
                    default:
                        errorList.Add(string.Format("{0}. Значение поля '% бюджет городского поселения' должно быть равно значению поля '% бюджет сельского поселения' или нулю", GetRowInfo(row)));
                        break;
                }
                return false;
            }
            return true;
        }


        private bool CheckSettlementBudget(DataRow row)
        {
            if (row["16" + VALUE_POSTFIX] == DBNull.Value || row["17" + VALUE_POSTFIX] == DBNull.Value ||
                row["6" + VALUE_POSTFIX] == DBNull.Value)
                return false;
            decimal townBubget = Convert.ToDecimal(row["16" + VALUE_POSTFIX]);
            decimal ruralBudget = Convert.ToDecimal(row["17" + VALUE_POSTFIX]);
            decimal settlementBudget = Convert.ToDecimal(row["6" + VALUE_POSTFIX]);
            if (townBubget != 0 && ruralBudget != 0 && townBubget != ruralBudget)
                return false;
            return true;
        }


        private bool CheckBubgetMO(DataRow row, List<string> errorList)
        {
            decimal consBudgetMO = Convert.ToDecimal(row["14" + VALUE_POSTFIX]);
            decimal budgetGO = Convert.ToDecimal(row["15" + VALUE_POSTFIX]);
            decimal consBudgetMR = Convert.ToDecimal(row["4" + VALUE_POSTFIX]);
            if (budgetGO != 0 && consBudgetMR != 0 && budgetGO != consBudgetMR)
            {
                errorList.Add(string.Format("Запись с ID = {0} (КД {1}, год {2}). Значение поля '% бюжет ГО' должно быть равно значению поля '% конс. бюджет МР' или нулю", row["ID"], row["RefKD"], row["RefYearDayUNV"]));
                return false;
            }
            return true;
        }


        private bool CheckBubgetMO(DataRow row)
        {
            if (row["14" + VALUE_POSTFIX] == DBNull.Value || row["15" + VALUE_POSTFIX] == DBNull.Value || row["4" + VALUE_POSTFIX] == DBNull.Value)
                return false;
            decimal consBudgetMO = Convert.ToDecimal(row["14" + VALUE_POSTFIX]);
            decimal budgetGO = Convert.ToDecimal(row["15" + VALUE_POSTFIX]);
            decimal consBudgetMR = Convert.ToDecimal(row["4" + VALUE_POSTFIX]);
            if (budgetGO != 0 && consBudgetMR != 0 && budgetGO != consBudgetMR)
                return false;
            return true;
        }


        private bool CheckNormativesData(DataTable normatiesTable)
        {
            Dictionary<string, string> visibleColumns = GetVisibleColumnsNames();
            foreach (DataRow row in normatiesTable.Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                {
                    foreach (KeyValuePair<string, string> column in visibleColumns)
                    {
                        if (row[UltraGridEx.GetSourceColumnName(column.Key)] == DBNull.Value)
                        {
                            UltraGridRow gridRow = UltraGridHelper.FindGridRow(drv.newGrid.ugData, "ID", row["ID"]);
                            if (gridRow != null)
                                gridRow.Activate();
                            MessageBox.Show(string.Format("Запись с ID = {0}. Поле '{1}' не заполнено", row["ID"],
                                column.Value), "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        private bool CheckNormatives(List<string> errorList, DataTable normatiesTable,
            NormativesKind normative, bool checkAllNormatives)
        {
            bool successCheck = true;
            switch (normative)
            {
                case NormativesKind.NormativesBK:
                    successCheck = CheckNormativesBK(errorList, normatiesTable, checkAllNormatives);
                    break;
                case NormativesKind.NormativesMR:
                    successCheck = CheckNormativesMR(errorList, normatiesTable, checkAllNormatives);
                    break;
                case NormativesKind.NormativesRegionRF:
                    successCheck = CheckNormativesRegionRF(errorList, normatiesTable, checkAllNormatives);
                    break;
                case NormativesKind.VarNormativesMR:
                    successCheck = CheckVarNormativesMR(errorList, normatiesTable, checkAllNormatives);
                    break;
                case NormativesKind.VarNormativesRegionRF:
                    successCheck = CheckVarNormativesRegionRF(errorList, normatiesTable, checkAllNormatives);
                    break;
            }
            return successCheck;
        }


        private bool CheckCorrectNormative(List<string> errorList, DataRow normativeRow, NormativesKind normative)
        {
            int refKD = Convert.ToInt32(normativeRow["RefKD"]);
            int refYearDayUNV = Convert.ToInt32(normativeRow["RefYearDayUNV"]);

            int parentKD = 0;
            int parentYear = 0;
            NormativesKind parentNormative = NormativesKind.Unknown;
            if (!disintRulesModule.GetParentRowParams(refKD, refYearDayUNV, normative, ref parentKD, ref parentYear, ref parentNormative))
                return true;

            string KD = GetKDFromRef(parentKD);

            decimal valueGO = 0;
            decimal valueGOParent = 0;
            decimal valueR = 0;
            decimal valueRParent = 0;
            decimal valueP = 0;
            decimal valuePParent = 0;
            decimal valueGP = 0;
            decimal valueGPParent = 0;
            decimal valueSP = 0;
            decimal valueSPParent = 0;

            switch (normative)
            {
                case NormativesKind.NormativesRegionRF:
                    valueGO = GetNormativeValue(normativeRow, string.Format("{0}{1}", 15, VALUE_POSTFIX));
                    valueGOParent = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(normative, refKD, refYearDayUNV, 15)) * 100;
                    valueR = GetNormativeValue(normativeRow, string.Format("{0}{1}", 5, VALUE_POSTFIX));
                    valueRParent = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(normative, refKD, refYearDayUNV, 5)) * 100;
                    valueP = GetNormativeValue(normativeRow, string.Format("{0}{1}", 6, VALUE_POSTFIX));
                    valuePParent = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(normative, refKD, refYearDayUNV, 6)) * 100;
                    valueGP = GetNormativeValue(normativeRow, string.Format("{0}{1}", 16, VALUE_POSTFIX));
                    valueGPParent = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(normative, refKD, refYearDayUNV, 16)) * 100;
                    valueSP = GetNormativeValue(normativeRow, string.Format("{0}{1}", 17, VALUE_POSTFIX));
                    valueSPParent = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(normative, refKD, refYearDayUNV, 17)) * 100;
                    break;
                case NormativesKind.NormativesMR:
                    valueP = GetNormativeValue(normativeRow, string.Format("{0}{1}", 6, VALUE_POSTFIX));
                    valuePParent = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(normative, refKD, refYearDayUNV, 6)) * 100;
                    valueGP = GetNormativeValue(normativeRow, string.Format("{0}{1}", 16, VALUE_POSTFIX));
                    valueGPParent = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(normative, refKD, refYearDayUNV, 16)) * 100;
                    valueSP = GetNormativeValue(normativeRow, string.Format("{0}{1}", 17, VALUE_POSTFIX));
                    valueSPParent = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(normative, refKD, refYearDayUNV, 17)) * 100;
                    break;
            }

            string parentNormativeName = GetNormativeRusName(parentNormative);

            if (valueGO < valueGOParent)
            {  
                errorList.Add(string.Format("Значение норматива '% бюджет ГО' меньше, чем в родительском ('{0}', KD = {1}, год = {2}, значение = {3})", parentNormativeName, KD, parentYear, valueGOParent));
            }
            if (valueP < valuePParent)
            {
                errorList.Add(string.Format("Значение норматива '% бюджет поселения' меньше, чем в родительском ('{0}', KD = {1}, год = {2}, значение = {3})", parentNormativeName, KD, parentYear, valuePParent));
            }
            if (valueR < valueRParent)
            {
                errorList.Add(string.Format("Значение норматива '% бюджет района' меньше, чем в родительском ('{0}', KD = {1}, год = {2}, значение = {3})", parentNormativeName, KD, parentYear, valueRParent));
            }
            if (valueGP < valueGPParent)
            {
                errorList.Add(string.Format("Значение норматива '% бюджет городского поселения' меньше, чем в родительском ('{0}', KD = {1}, год = {2}, значение = {3})", parentNormativeName, KD, parentYear, valueGPParent));
            }
            if (valueSP < valueSPParent)
            {
                errorList.Add(string.Format("Значение норматива '% бюджет сельского поселения' меньше, чем в родительском ('{0}', KD = {1}, год = {2}, значение = {3})", parentNormativeName, KD, parentYear, valueSPParent));
            }
            return errorList.Count == 0;
        }

        private decimal GetNormativeValue(DataRow row, string columnName)
        {
            decimal value = 0;
            if (row[columnName] != DBNull.Value)
                value = Convert.ToDecimal(row[columnName]);
            return value;
        }

        #endregion

        #region вспомогательные методы

        private bool CheckDiffNormatives(NormativesKind normativeKind, int refKD, int refYearDayUNV, int refRegions)
        {
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string query = "select ID from {0} where RefKD = ? and RefYearDayUNV = ? and RefRegions = ?";
                IDbDataParameter[] queryParams = new IDbDataParameter[3];
                queryParams[0] = new System.Data.OleDb.OleDbParameter("RefKD", refKD);//db.CreateParameter("RefKD", refKD);
                queryParams[1] = new System.Data.OleDb.OleDbParameter("RefYearDayUNV", refYearDayUNV);//db.CreateParameter("RefYear", refYear);
                queryParams[2] = new System.Data.OleDb.OleDbParameter("RefRegions", refRegions);//db.CreateParameter("RefRegions", refRegions);
                switch (normativeKind)
                {
                    case NormativesKind.VarNormativesMR:
                        query = string.Format(query, "F_NORM_VARIEDREGION");
                        break;
                    case NormativesKind.VarNormativesRegionRF:
                        query = string.Format(query, "F_NORM_VARIEDMR");
                        break;
                }
                DataTable queryResult = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams);
                if (queryResult.Rows.Count > 0)
                    return false;
                return true;
            }
        }


        /// <summary>
        /// получение протокола ошибок для текущего норматива
        /// </summary>
        /// <param name="listError"></param>
        private void CreateErrorsProtocol(List<string> listError)
        {
            // сохраняем протокол в файлик
            string protocolName = string.Format("{0}. Протокол проверки", ((BaseViewObj)WorkplaceSingleton.Workplace.ActiveContent).Caption);
            string fileName = string.Empty;
            if (ExportImportHelper.GetFileName(protocolName + ".xls", ExportImportHelper.fileExtensions.xls, true, ref fileName))
            {
                Workbook wb = new Workbook();
                if (string.Compare(Path.GetExtension(fileName), ".xlsx", true) == 0)
                {
                    wb.SetCurrentFormat(WorkbookFormat.Excel2007);
                }
                Worksheet ws = wb.Worksheets.Add("Protokol");
                ws.Rows[0].Cells[0].Value = protocolName;
                int index = 3;
                foreach (string strErr in listError)
                {
                    ws.Rows[index].Cells[0].Value = strErr;
                    index++;
                }
                wb.Save(fileName);
            }
        }

        /// <summary>
        /// список видимых колонок норматива
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetVisibleColumnsNames()
        {
            Dictionary<string, string> columns = new Dictionary<string,string>();
            foreach (UltraGridColumn column in drv.newGrid.ugData.DisplayLayout.Bands[0].Columns)
            {
                if (!column.Hidden && column.Header.Caption != string.Empty)
                    columns.Add(column.Key, column.Header.Caption);
            }
            return columns;
        }

        /// <summary>
        /// расчет текущего норматива без сохранения в базу
        /// </summary>
        private void CalculateCurrentNormative()
        {
            foreach (UltraGridRow row in drv.newGrid.ugData.Rows)
            {
                UltraGridRow activeRow = UltraGridHelper.GetRowCells(row);
                CalculateRow(activeRow);
            }
            BurnNormative(currentNormatives, true);

        }

        /// <summary>
        /// расчет одного норматива из числа нескольких
        /// </summary>
        /// <param name="errorList"></param>
        /// <param name="normative"></param>
        private void CalculateSingleNormative(NormativesKind normative, List<string> errorList)
        {
            DataTable normativeTable = null;
            if (normative == currentNormatives)
            {
                normativeTable = dtNormatives;
            }
            else
                normativeTable = this.disintRulesModule.GetNormatives(normative);
            CalculateNormative(normativeTable, normative);
            CheckNormatives(errorList, normativeTable, normative, true);
            disintRulesModule.ApplyChanges(normative, normativeTable.GetChanges());
        }

        /// <summary>
        /// запись результата проверки одного норматива в лист екселя
        /// </summary>
        /// <param name="errorList"></param>
        /// <param name="wb"></param>
        /// <param name="normative"></param>
        private void WriteErrorsToExcel(List<string> errorList, Infragistics.Excel.Workbook wb, NormativesKind normative)
        {
            string protocolName = string.Format("{0}. Протокол проверки", GetNormativeRusName(normative));

            Infragistics.Excel.Worksheet ws = wb.Worksheets.Add(string.Format("Protokol {0}", wb.Worksheets.Count));

            ws.Rows[0].Cells[0].Value = protocolName;
            int index = 3;
            foreach (string strErr in errorList)
            {
                ws.Rows[index].Cells[0].Value = strErr;
                index++;
            }
        }

        /// <summary>
        /// расчет всех нормативов с получением списка несоответствий и записью результатов в базу
        /// </summary>
        private void CalculateAllNormatives()
        {
            if (!CheckNormativesData(dtNormatives))
                return;

            Infragistics.Excel.Workbook wb = new Workbook();
            List<string> errorList = new List<string>();
            DataTable normative = new DataTable();
            bool isHaveErrors = false;

            if (currentNormatives == NormativesKind.NormativesBK)
            {
                CalculateSingleNormative(NormativesKind.NormativesBK, errorList);
                WriteErrorsToExcel(errorList, wb, NormativesKind.NormativesBK);
                isHaveErrors = errorList.Count > 0;
                errorList.Clear();
            }

            if (currentNormatives == NormativesKind.NormativesBK || currentNormatives == NormativesKind.NormativesRegionRF)
            {
                CalculateSingleNormative(NormativesKind.NormativesRegionRF, errorList);
                WriteErrorsToExcel(errorList, wb, NormativesKind.NormativesRegionRF);
                if (!isHaveErrors)
                    isHaveErrors = errorList.Count > 0;
                errorList.Clear();
            }

            if (currentNormatives != NormativesKind.VarNormativesMR && currentNormatives != NormativesKind.VarNormativesRegionRF)
            {
                CalculateSingleNormative(NormativesKind.NormativesMR, errorList);
                WriteErrorsToExcel(errorList, wb, NormativesKind.NormativesMR);
                if (!isHaveErrors)
                    isHaveErrors = errorList.Count > 0;
                errorList.Clear();
            }

            if (currentNormatives != NormativesKind.VarNormativesMR)
            {
                CalculateSingleNormative(NormativesKind.VarNormativesRegionRF, errorList);
                WriteErrorsToExcel(errorList, wb, NormativesKind.VarNormativesRegionRF);
                if (!isHaveErrors)
                    isHaveErrors = errorList.Count > 0;
                errorList.Clear();
            }

            if (currentNormatives != NormativesKind.VarNormativesRegionRF)
            {
                CalculateSingleNormative(NormativesKind.VarNormativesMR, errorList);
                WriteErrorsToExcel(errorList, wb, NormativesKind.VarNormativesMR);
                if (!isHaveErrors)
                    isHaveErrors = errorList.Count > 0;
                errorList.Clear();
            }

            dtNormatives.AcceptChanges();
            drv.newGrid.ClearAllStateImages();
            drv.newGrid.BurnChangesDataButtons(false);

            if (isHaveErrors)
            {
                if (MessageBox.Show("В результате проверки данных были обнаружены ошибки. Сохранить протокол проверки?", "Проверка данных",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    string fileName = string.Empty;
                    if (ExportImportHelper.GetFileName("Протокол проверки.xls", ExportImportHelper.fileExtensions.xls, true, ref fileName))
                    {
                        if (string.Compare(Path.GetExtension(fileName), ".xlsx", true) == 0)
                        {
                            wb.SetCurrentFormat(WorkbookFormat.Excel2007);
                        }
                        wb.Save(fileName);
                    }
                        
                }
            }
        }

        /// <summary>
        /// расчитывает одну запись норматива
        /// </summary>
        /// <param name="row"></param>
        private void CalculateRow(UltraGridRow row)
        {
            switch (currentNormatives)
            {
                case NormativesKind.VarNormativesMR:
                    CalculateVarNormativesMR(row);
                    break;
                case NormativesKind.VarNormativesRegionRF:
                    CalculateVarNormativesRegionRF(row);
                    break;
                case NormativesKind.NormativesMR:
                    CalculateNormativesMR(row);
                    break;
                case NormativesKind.NormativesRegionRF:
                    CalculateNormativesRegionRF(row);
                    break;
                case NormativesKind.NormativesBK:
                    CalculateNormativesBK(row);
                    break;
            }
        }

        /// <summary>
        /// проверяет одну запись норматива
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        private bool CheckRow(DataRow dataRow)
        {
            switch (currentNormatives)
            {
                case NormativesKind.NormativesBK:
                    return CheckNormativesBKRow(dataRow);
                case NormativesKind.NormativesMR:
                    return CheckNormativesMRRow(dataRow);
                case NormativesKind.NormativesRegionRF:
                    return CheckNormativesRegionRFRow(dataRow);
                case NormativesKind.VarNormativesMR:
                    return CheckVarNormativesMRRow(dataRow);
                case NormativesKind.VarNormativesRegionRF:
                    return CheckVarNormativesRegionRFRow(dataRow);
            }
            return true;
        }

        /// <summary>
        /// проверяет запись норматива без предварительного расчета этой записи
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool CheckRowWithoutCalculate(DataRow row)
        {
            int refKD = 0;
            int refYearDayUNV = 0;
            int refRegions = 0;

            switch (currentNormatives)
            {
                case NormativesKind.NormativesBK:
                    if (row["1" + VALUE_POSTFIX] == DBNull.Value)
                        return false;
                    if (Convert.ToDecimal(row["1" + VALUE_POSTFIX]) < 0)
                        return false;

                    if (!CheckBubgetMO(row))
                        return false;

                    break;
                case NormativesKind.NormativesRegionRF:
                    if (row["3" + VALUE_POSTFIX] == DBNull.Value)
                        return false;
                    if (Convert.ToDecimal(row["3" + VALUE_POSTFIX]) < 0)
                        return false;
                    if (!CheckBubgetMO(row))
                        return false;
                    break;
                case NormativesKind.NormativesMR:
                    if (row["5" + VALUE_POSTFIX] == DBNull.Value)
                        return false;
                    if (Convert.ToDecimal(row["5" + VALUE_POSTFIX]) < 0)
                        return false;
                    break;
                case NormativesKind.VarNormativesMR:
                    if (row["5" + VALUE_POSTFIX] == DBNull.Value)
                        return false;
                    if (Convert.ToDecimal(row["5" + VALUE_POSTFIX]) < 0)
                        return false;
                    refKD = Convert.ToInt32(row["RefKD"]);
                    refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                    refRegions = Convert.ToInt32(row["RefRegions"]);
                    if (!CheckDiffNormatives(currentNormatives, refKD, refYearDayUNV, refRegions))
                        return false;
                    break;
                case NormativesKind.VarNormativesRegionRF:
                    if (row["3" + VALUE_POSTFIX] == DBNull.Value)
                        return false;
                    if (Convert.ToDecimal(row["3" + VALUE_POSTFIX]) < 0)
                        return false;
                    refKD = Convert.ToInt32(row["RefKD"]);
                    refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                    refRegions = Convert.ToInt32(row["RefRegions"]);
                    if (!CheckDiffNormatives(currentNormatives, refKD, refYearDayUNV, refRegions))
                        return false;
                    break;
            }

            if (currentNormatives != NormativesKind.NormativesBK)
                if (!PresentParentNormativeForRow(row, currentNormatives))
                    return false;

            if (!CheckSettlementBudget(row))
                return false;

            if (FinddecimalsNormativesForRow(row, currentNormatives, row.Table))
                return false;

            return true;
        }

        /// <summary>
        /// проверяет одну запись и выдает список ошибок
        /// </summary>
        /// <param name="row"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private bool CheckRowWithoutCalculate(DataRow row, ref StringBuilder errors)
        {
            //StringBuilder errors = new StringBuilder();
            int refKD = 0;
            int refYearDayUNV = 0;
            int refRegions = 0;

            switch (currentNormatives)
            {
                case NormativesKind.NormativesBK:
                    if (row["1" + VALUE_POSTFIX] == DBNull.Value)
                        return false;
                    if (Convert.ToDecimal(row["1" + VALUE_POSTFIX]) < 0)
                        errors.AppendLine("Неверный норматив доходов. Значения нормативов не могут быть отрицательными");

                    if (!CheckBubgetMO(row))
                        errors.AppendLine("Значение поля '% бюжет ГО' должно быть равно значению поля '% конс. бюджет МР' или нулю");

                    break;
                case NormativesKind.NormativesRegionRF:
                    if (row["3" + VALUE_POSTFIX] == DBNull.Value)
                        return false;
                    if (Convert.ToDecimal(row["3" + VALUE_POSTFIX]) < 0)
                        errors.AppendLine("Неверный норматив доходов. Значения нормативов не могут быть отрицательными");
                    if (!CheckBubgetMO(row))
                        errors.AppendLine("Значение поля '% бюжет ГО' должно быть равно значению поля '% конс. бюджет МР' или нулю");
                    break;
                case NormativesKind.NormativesMR:
                    if (row["5" + VALUE_POSTFIX] == DBNull.Value)
                        return false;
                    if (Convert.ToDecimal(row["5" + VALUE_POSTFIX]) < 0)
                        errors.AppendLine("Неверный норматив доходов. Значения нормативов не могут быть отрицательными");
                    break;
                case NormativesKind.VarNormativesMR:
                    if (row["5" + VALUE_POSTFIX] == DBNull.Value)
                        return false;
                    if (Convert.ToDecimal(row["5" + VALUE_POSTFIX]) < 0)
                        errors.AppendLine("Неверный норматив доходов. Значения нормативов не могут быть отрицательными");
                    refKD = Convert.ToInt32(row["RefKD"]);
                    refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                    refRegions = Convert.ToInt32(row["RefRegions"]);
                    if (!CheckDiffNormatives(currentNormatives, refKD, refYearDayUNV, refRegions))
                        errors.AppendLine("Код дохода должен быть уникальным в пределах года по району, найдены дубликаты в других нормативах");
                    break;
                case NormativesKind.VarNormativesRegionRF:
                    if (row["3" + VALUE_POSTFIX] == DBNull.Value)
                        return false;
                    if (Convert.ToDecimal(row["3" + VALUE_POSTFIX]) < 0)
                        errors.AppendLine("Неверный норматив доходов. Значения нормативов не могут быть отрицательными");
                    refKD = Convert.ToInt32(row["RefKD"]);
                    refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                    refRegions = Convert.ToInt32(row["RefRegions"]);
                    if (!CheckDiffNormatives(currentNormatives, refKD, refYearDayUNV, refRegions))
                        errors.AppendLine("Код дохода должен быть уникальным в пределах года по району, найдены дубликаты в других нормативах");
                    break;
            }

            if (currentNormatives != NormativesKind.NormativesBK)
                if (!PresentParentNormativeForRow(row, currentNormatives))
                    errors.AppendLine("Для записи не найдена родительская запись в нормативах");

            if (!CheckSettlementBudget(row))
                switch (currentNormatives)
                {
                    case NormativesKind.VarNormativesMR:
                    case NormativesKind.VarNormativesRegionRF:
                        errors.AppendLine("Значение поля '% бюджет городского поселения доп.' должно быть равно значению поля '% бюджет сельского поселения доп.' или нулю");
                        break;
                    default:
                        errors.AppendLine("Значение поля '% бюджет городского поселения' должно быть равно значению поля '% бюджет сельского поселения' или нулю");
                        break;
                }

            if (FinddecimalsNormativesForRow(row, currentNormatives, row.Table))
                switch (currentNormatives)
                {
                    case NormativesKind.VarNormativesMR:
                    case NormativesKind.VarNormativesRegionRF:
                        errors.AppendLine("Код дохода должен быть уникальным в пределах года по району");
                        break;
                    default:
                        errors.AppendLine("Код дохода должен быть уникальным в пределах года");
                        break;
                }
            List<string> errorsList = new List<string>();
            if (currentNormatives != NormativesKind.NormativesBK)
                if (!CheckCorrectNormative(errorsList, row, currentNormatives))
                {
                    foreach (string strError in errorsList)
                    {
                        errors.AppendLine(strError);
                    }
                }

            if (errors.Length > 0)
                return false;
            else
                return true;
        }


        private bool FinddecimalsNormativesForRow(DataRow row, NormativesKind normative, DataTable normatiesTable)
        {
            if (row["RefKD"] == DBNull.Value)
                return false;
            if (row["RefYearDayUNV"] == DBNull.Value)
                return false;
            int refKD = Convert.ToInt32(row["RefKD"]);
            int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
            switch (normative)
            {
                case NormativesKind.NormativesBK:
                case NormativesKind.NormativesMR:
                case NormativesKind.NormativesRegionRF:
                    if (normatiesTable.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1}", refKD, refYearDayUNV)).Length > 1)
                        return true;
                    break;
                case NormativesKind.VarNormativesMR:
                case NormativesKind.VarNormativesRegionRF:
                    if (row["RefRegions"] == DBNull.Value)
                        return false;
                    int refRegions = Convert.ToInt32(row["RefRegions"]);
                    if (normatiesTable.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1} and RefRegions = {2}", refKD, refYearDayUNV, refRegions)).Length > 1)
                        return true;
                    break;
            }
            return false;
        }


        /// <summary>
        /// проверяет, есть ли запись, на которую ссылается текущая запись норматива
        /// </summary>
        /// <param name="row"></param>
        /// <param name="normative"></param>
        /// <returns></returns>
        private bool PresentParentNormativeForRow(DataRow row, NormativesKind normative)
        {
            if (row["RefKD"] == DBNull.Value || row["RefYearDayUNV"] == DBNull.Value)
                return false;

            int refKD = Convert.ToInt32(row["RefKD"]);
            int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
            decimal value = 0;
            switch (normative)
            {
                case NormativesKind.NormativesMR:
                    value = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(normative, refKD, refYearDayUNV, 4));
                    break;
                case NormativesKind.NormativesRegionRF:
                    value = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(normative, refKD, refYearDayUNV, 2));
                    break;
                case NormativesKind.VarNormativesMR:
                    value = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(normative, refKD, refYearDayUNV, 4));
                    break;
                case NormativesKind.VarNormativesRegionRF:
                    value = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(normative, refKD, refYearDayUNV, 2));
                    break;
            }
            if (value < 0)
                return false;
            return true;
        }


        #region вычисление нормативов по таблице данных


        private bool CalculateNormative(DataTable normatives, NormativesKind normative)
        {
            foreach (DataRow row in normatives.Rows)
            {
                switch (normative)
                {
                    case NormativesKind.NormativesMR:
                        CalculateNormativesMR(row);
                        break;
                    case NormativesKind.NormativesRegionRF:
                        CalculateNormativesRegionRF(row);
                        break;
                    case NormativesKind.VarNormativesMR:
                        CalculateVarNormativesMR(row);
                        break;
                    case NormativesKind.VarNormativesRegionRF:
                        CalculateVarNormativesRegionRF(row);
                        break;
                }
            }
            return true;
        }


        private bool CalculateNormativesRegionRF(DataRow row)
        {
            // любое не нулевое значение из значений «Бюджет городского поселения» ИЛИ «Бюджет сельского поселения»
            decimal settlementBudget = Convert.ToDecimal(row["16" + VALUE_POSTFIX]);
            if (settlementBudget == 0)
                settlementBudget = Convert.ToDecimal(row["17" + VALUE_POSTFIX]);
            if (settlementBudget != Convert.ToDecimal(row["6" + VALUE_POSTFIX]))
                row["6" + VALUE_POSTFIX] = settlementBudget;

            // Бюджет района + Бюджет поселения
            decimal consBudgetMR = Convert.ToDecimal(row["5" + VALUE_POSTFIX]) + Convert.ToDecimal(row["6" + VALUE_POSTFIX]);
            if (Convert.ToDecimal(row["4" + VALUE_POSTFIX]) != consBudgetMR)
                row["4" + VALUE_POSTFIX] = consBudgetMR;

            // любое не нулевое значение из значений «Конс. бюджет МР» ИЛИ «Бюджет ГО»
            decimal consBudgetMO = Convert.ToDecimal(row["15" + VALUE_POSTFIX]);
            if (consBudgetMO == 0)
                consBudgetMO = Convert.ToDecimal(row["4" + VALUE_POSTFIX]);
            if (Convert.ToDecimal(row["14" + VALUE_POSTFIX]) != consBudgetMO)
                row["14" + VALUE_POSTFIX] = consBudgetMO;

            // плюс еще какой то свой уникальный расчет
            decimal regionBudget = -1;
            if (row["2" + VALUE_POSTFIX] != DBNull.Value)
            {
                regionBudget = Convert.ToDecimal(row["2" + VALUE_POSTFIX]) - Convert.ToDecimal(row["14" + VALUE_POSTFIX]);
                if (Convert.ToDecimal(row["3" + VALUE_POSTFIX]) != regionBudget)
                    row["3" + VALUE_POSTFIX] = regionBudget;
            }

            if (regionBudget < 0)
                return false;
            return true;
        }


        private bool CalculateNormativesMR(DataRow row)
        {
            // любое не нулевое значение из значений «Бюджет городского поселения» ИЛИ «Бюджет сельского поселения»
            decimal settlementBudget = Convert.ToDecimal(row["16" + VALUE_POSTFIX]);
            if (settlementBudget == 0)
                settlementBudget = Convert.ToDecimal(row["17" + VALUE_POSTFIX]);
            if (settlementBudget != Convert.ToDecimal(row["6" + VALUE_POSTFIX]))
                row["6" + VALUE_POSTFIX] = settlementBudget;

            // плюс еще какой то свой уникальный расчет
            decimal areaBudget = -1;
            if (row["4" + VALUE_POSTFIX] != DBNull.Value)
            {
                areaBudget = Convert.ToDecimal(row["4" + VALUE_POSTFIX]) - Convert.ToDecimal(row["6" + VALUE_POSTFIX]);
                if (Convert.ToDecimal(row["5" + VALUE_POSTFIX]) != areaBudget)
                    row["5" + VALUE_POSTFIX] = areaBudget;
            }

            if (areaBudget < 0)
                return false;
            return true;
        }


        private bool CalculateVarNormativesRegionRF(DataRow row)
        {
            // некоторые значения будут считаться из нескольких...

            // бюджет городского поселения
            decimal townBubget = Convert.ToDecimal(row["16" + VALUE_POSTFIX]);
            decimal townBubgetAdd = 0;
            if (row["16" + REF_VALUE_POSTFIX] == DBNull.Value)
                townBubgetAdd = Convert.ToDecimal(row["16" + REF_VALUE_POSTFIX]);
            decimal resultTownBudget = townBubget + townBubgetAdd;
            if (Convert.ToDecimal(row["16" + RESULT_VALUE_POSTFIX]) != resultTownBudget)
                row["16" + RESULT_VALUE_POSTFIX] = resultTownBudget;
            // бюджет сельского поселения
            decimal ruralBudget = Convert.ToDecimal(row["17" + VALUE_POSTFIX]);
            decimal ruralBudgetAdd = 0;
            if (row["17" + REF_VALUE_POSTFIX] != DBNull.Value)
                ruralBudgetAdd = Convert.ToDecimal(row["17" + REF_VALUE_POSTFIX]);
            decimal resultRuralBudget = ruralBudget + ruralBudgetAdd;
            if (Convert.ToDecimal(row["17" + RESULT_VALUE_POSTFIX]) != resultRuralBudget)
                row["17" + RESULT_VALUE_POSTFIX] = resultRuralBudget;
            // бюджет поселения
            decimal settlementBudget = townBubget;
            if (settlementBudget == 0)
                settlementBudget = ruralBudget;
            row["6" + VALUE_POSTFIX] = settlementBudget;
            decimal settlementBudgetAdd = 0;
            if (row["6" + REF_VALUE_POSTFIX] != DBNull.Value)
                settlementBudgetAdd = Convert.ToDecimal(row["6" + REF_VALUE_POSTFIX]);
            decimal resultSettlementBudget = settlementBudgetAdd + settlementBudget;
            if (Convert.ToDecimal(row["6" + RESULT_VALUE_POSTFIX]) != resultSettlementBudget)
                row["6" + RESULT_VALUE_POSTFIX] = resultSettlementBudget;
            // бюджет района
            decimal areaBudget = Convert.ToDecimal(row["5" + VALUE_POSTFIX]);
            decimal areaBudgetAdd = 0;
            if (row["5" + REF_VALUE_POSTFIX] != DBNull.Value)
                areaBudgetAdd = Convert.ToDecimal(row["5" + REF_VALUE_POSTFIX]);
            decimal resultAreaBudget = areaBudget + areaBudgetAdd;
            if (Convert.ToDecimal(row["5" + RESULT_VALUE_POSTFIX]) != resultAreaBudget)
                row["5" + RESULT_VALUE_POSTFIX] = resultAreaBudget;
            // Конс. бюджет МР
            decimal consBudgetMR = resultAreaBudget + resultSettlementBudget;
            if (Convert.ToDecimal(row["4" + VALUE_POSTFIX]) != consBudgetMR)
                row["4" + VALUE_POSTFIX] = consBudgetMR;
            // бюджет ГО
            decimal goBubget = Convert.ToDecimal(row["15" + VALUE_POSTFIX]);
            decimal goBubgetAdd = 0;
            if (row["15" + REF_VALUE_POSTFIX] != DBNull.Value)
                goBubgetAdd = Convert.ToDecimal(row["15" + REF_VALUE_POSTFIX]);
            decimal resultGOBudget = goBubget + goBubgetAdd;
            if (Convert.ToDecimal(row["15" + RESULT_VALUE_POSTFIX]) != resultGOBudget)
                row["15" + RESULT_VALUE_POSTFIX] = resultGOBudget;
            // любое не нулевое значение из значений «Конс. бюджет МР» ИЛИ «Бюджет ГО»
            decimal consBudgetMO = consBudgetMR;
            if (consBudgetMO < resultGOBudget)
                consBudgetMO = resultGOBudget;
            if (Convert.ToDecimal(row["14" + VALUE_POSTFIX]) != consBudgetMO)
                row["14" + VALUE_POSTFIX] = consBudgetMO;
            // плюс еще какой то свой уникальный расчет
            decimal regionBudget = -1;
            if (row["2" + VALUE_POSTFIX] != DBNull.Value)
            {
                regionBudget = Convert.ToDecimal(row["2" + VALUE_POSTFIX]) - consBudgetMO;
                if (Convert.ToDecimal(row["3" + VALUE_POSTFIX]) != regionBudget)
                    row["3" + VALUE_POSTFIX] = regionBudget;
            }

            if (regionBudget < 0)
                return false;
            return true;
        }


        private bool CalculateVarNormativesMR(DataRow row)
        {
            // бюджет городского поселения
            decimal townBubget = Convert.ToDecimal(row["16" + VALUE_POSTFIX]);
            decimal townBubgetAdd = 0;
            if (row["16" + REF_VALUE_POSTFIX] != DBNull.Value)
                townBubgetAdd = Convert.ToDecimal(row["16" + REF_VALUE_POSTFIX]);
            decimal resultTownBudget = townBubget + townBubgetAdd;
            if (Convert.ToDecimal(row["16" + RESULT_VALUE_POSTFIX]) != resultTownBudget)
                row["16" + RESULT_VALUE_POSTFIX] = resultTownBudget;
            // бюджет сельского поселения
            decimal ruralBudget = Convert.ToDecimal(row["17" + VALUE_POSTFIX]);
            decimal ruralBudgetAdd = 0;
            if (row["17" + REF_VALUE_POSTFIX] != DBNull.Value)
                ruralBudgetAdd = Convert.ToDecimal(row["17" + REF_VALUE_POSTFIX]);
            decimal resultRuralBudget = ruralBudget + ruralBudgetAdd;
            if (Convert.ToDecimal(row["17" + RESULT_VALUE_POSTFIX]) != resultRuralBudget)
                row["17" + RESULT_VALUE_POSTFIX] = resultRuralBudget;
            // бюджет поселения
            decimal settlementBudget = townBubget;
            if (settlementBudget == 0)
                settlementBudget = ruralBudget;
            row["6" + VALUE_POSTFIX] = settlementBudget;
            decimal settlementBudgetAdd = 0;
            if (row["6" + REF_VALUE_POSTFIX] != DBNull.Value)
                settlementBudgetAdd = Convert.ToDecimal(row["6" + REF_VALUE_POSTFIX]);
            decimal resultSettlementBudget = settlementBudgetAdd + settlementBudget;
            if (Convert.ToDecimal(row["6" + RESULT_VALUE_POSTFIX]) != resultSettlementBudget)
                row["6" + RESULT_VALUE_POSTFIX] = resultSettlementBudget;
            // бюджет района
            decimal areaBudget = -1;
            if (row["4" + VALUE_POSTFIX] != DBNull.Value)
            {
                areaBudget = Convert.ToDecimal(row["4" + VALUE_POSTFIX]) - resultSettlementBudget;
                if (Convert.ToDecimal(row["5" + VALUE_POSTFIX]) != areaBudget)
                    row["5" + VALUE_POSTFIX] = areaBudget;
            }

            if (areaBudget < 0)
                return false;
            return true;
        }


        #endregion


        #endregion
    }
}
