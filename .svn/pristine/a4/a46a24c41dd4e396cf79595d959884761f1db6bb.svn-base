using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations
{
    public class CalculationServerBase
    {
        internal readonly IScheme scheme;
        internal readonly FinSourcesRererencesUtils finSourcesRererencesUtils;
        internal IEntity entity;

        public CalculationServerBase(IScheme scheme)
        {
            this.scheme = scheme;
            finSourcesRererencesUtils = new FinSourcesRererencesUtils(scheme);
        }

        /// <summary>
        /// список уникальных кодов для набора констант (коды у разных констант могут быть одинаковыми)
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="constNames"></param>
        /// <returns></returns>
        internal List<string> GetConstUniqueCodes(ref List<string> errors, params string[] constNames)
        {
            List<string> constCodes = new List<string>();
            foreach (string constName in constNames)
            {
                string code = GetConstCode(constName, ref errors);
                if (!constCodes.Contains(code))
                    constCodes.Add(code);
            }
            return constCodes;
        }

        /// <summary>
        /// сумма значений для набора кодов из констант
        /// </summary>
        /// <returns></returns>
        internal decimal GetConstsIfSum(int borrowVariant, int sourceId, int year, int budgetLevel, IDatabase db, ref List<string> errors, params string[] constNames)
        {
            List<string> codes = GetConstUniqueCodes(ref errors, constNames);
            decimal sum = 0;

            foreach (string code in codes)
            {
                sum += GetResultIF(borrowVariant, sourceId, year, budgetLevel, code, db);
            }
            return sum;
        }

        internal int GetRegionType()
        {
            int regionType = -1;
            if (scheme.GlobalConstsManager.Consts.ContainsKey("TerrPartType"))
                regionType = Utils.GetBudgetLevel(Convert.ToInt32(scheme.GlobalConstsManager.Consts["TerrPartType"].Value));
            return regionType;
        }

        internal string GetConstCode(string constName, ref List<string> errors)
        {
            object[] constValues = finSourcesRererencesUtils.GetConstDataByName(constName);
            if (constValues == null)
            {
                errors.Add(string.Format("Константа с идентификатором '{0}' не найдена в справочнике констант для ИФ", constName));
                return string.Empty;
            }
            return finSourcesRererencesUtils.GetConstDataByName(constName)[0].ToString();
        }

        internal string GetConstCode(string constName, ref List<string> errors, int codePartIndex, int codePartLength)
        {
            string constCode = GetConstCode(constName, ref errors);
            if (!string.IsNullOrEmpty(constCode))
                return constCode.Substring(codePartIndex, codePartLength);
            return null;
        }

        /// <summary>
        /// доходы с учетом кода доходов
        /// </summary>
        internal Decimal GetIncomes(int sourceID, int variant, int budgetLevel, int year, string kdCodesFilter, IDatabase db)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_D_FOPlanIncDivide);
            IEntity kdEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_KD_Plan_Key);
            decimal result = 0;

            string planKDQuery = string.Format("Select ID from {0} where ({1})", kdEntity.FullDBName, kdCodesFilter);

            string query = string.Format("Select Sum({0}) from {1} where SourceID = {2} and RefVariant = {3} and RefYearDayUNV like '{4}____' and RefKD IN ({5}) and RefBudLevel = {6}",
                year == DateTime.Today.Year ? "Estimate" : "Forecast",
                entity.FullDBName, sourceID, variant, year, planKDQuery, budgetLevel);

            object objResult = db.ExecQuery(query, QueryResultTypes.Scalar);
            if (!(objResult is DBNull))
                result = Convert.ToDecimal(objResult);

            return result;
        }

        /// <summary>
        /// доходы
        /// </summary>
        internal Decimal GetIncomes(int sourceID, int variant, int budgetLevel, int year, IDatabase db)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_D_FOPlanIncDivide);
            decimal result = 0;

            string query = string.Format("Select Sum({0}) from {1} where SourceID = {2} and RefVariant = {3} and RefBudLevel = {4} and RefYearDayUNV like '{5}____'",
                year == DateTime.Today.Year ? "Estimate" : "Forecast",
                entity.FullDBName, sourceID, variant, budgetLevel, year);

            object objResult = db.ExecQuery(query, QueryResultTypes.Scalar);
            if (!(objResult is DBNull))
                result = Convert.ToDecimal(objResult);
            
            return result;
        }

        /// <summary>
        /// расходы
        /// </summary>
        /// <param name="variant"></param>
        /// <param name="sourceID"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        internal Decimal GetOutcomes(int variant, int sourceID, int year, int budgetLevel, IDatabase db)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_R_FO26R);
            decimal result = 0;

            string query = string.Format(
                    "select Sum(Summa) from {0} where SourceID = {1} and RefVariant = {2} and RefBdgtLvls = {3} and RefYearDayUNV like '{4}____'",
                    entity.FullDBName, sourceID, variant, budgetLevel, year);

            object objResult = db.ExecQuery(query, QueryResultTypes.Scalar);
            if (!(objResult is DBNull))
                result = Convert.ToDecimal(objResult);

            return result;
        }

        /// <summary>
        /// получение результата ИФ по разным кодам КИФ
        /// </summary>
        internal Decimal GetResultIF(int variant, int sourceID, int year, int budgetLevel, string kifCode, IDatabase db)
        {
            if (string.IsNullOrEmpty(kifCode))
                return 0;
            decimal result = 0;
            IEntity kifEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_KIF_Plan_Key);
            IEntity planIFEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Plan_Key);

            string kifSelect = string.Format("select id from {0} where CodeStr = '{1}' and SourceID = {2}",
                kifEntity.FullDBName, kifCode, sourceID);
            object kif = db.ExecQuery(kifSelect, QueryResultTypes.Scalar);
            string query = string.Format("select Sum({0}) from {1} where SourceID = {2} and RefSVariant = {3} and RefBudgetLevels = {4} and RefYearDayUNV like '{5}____' and RefKIF in ({6})",
                year == DateTime.Today.Year ? "COALESCE(Estimate, 0) + COALESCE(Fact, 0)" : "Forecast",
                planIFEntity.FullDBName, sourceID, variant, budgetLevel, year, kifSelect);
            object objResult = db.ExecQuery(query, QueryResultTypes.Scalar);
            if (!(objResult is DBNull))
                result = Convert.ToDecimal(objResult);
            
            return result;
        }

        internal decimal GetResultIFDirection(int variant, int sourceID, int year, int budgetLevel, string kifCode, IDatabase db)
        {
            if (string.IsNullOrEmpty(kifCode))
                return 0;
            decimal result = 0;
            IEntity kifEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_KIF_Plan_Key);
            IEntity planIFEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Plan_Key);

            string query = string.Format(
                @"select Sum(ifResult.{0}*kifPlan.RefKIF) from {1} ifResult, {2} kifPlan 
                where kifPlan.ID in (select ID from {2} where CodeStr like '___{6}%') 
                and ifResult.RefSVariant = {3} and ifResult.SourceID = {4} and ifResult.RefYearDayUNV like '{5}____' 
                and ifResult.RefKIF = kifPlan.ID 
                and ifResult.REFBUDGETLEVELS = {7}",
                year == DateTime.Today.Year ? "Estimate" : "Forecast",
                planIFEntity.FullDBName, kifEntity.FullDBName, variant, sourceID, year, kifCode, budgetLevel);

            object objResult = db.ExecQuery(query, QueryResultTypes.Scalar);
            if (!(objResult is DBNull))
                result = Convert.ToDecimal(objResult);
            
            return result;
        }

        internal object GetNewId(IEntity entity)
        {
            if (string.Compare(scheme.SchemeDWH.FactoryName, "System.Data.SqlClient", true) == 0)
                return DBNull.Value;
            return entity.GetGeneratorNextValue;
        }
    }
}
