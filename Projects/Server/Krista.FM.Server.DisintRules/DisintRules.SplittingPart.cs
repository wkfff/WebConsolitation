using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DisintRules
{
    internal enum VariantType
    {
        /// <summary>
        /// по уровням бюджета
        /// </summary>
        BudgetLevels = 1,
        /// <summary>
        /// в контингенте во все уровни бюджета
        /// </summary>
        QuotaToAllLevels = 2,
        /// <summary>
        /// в контингенте в консолидированный бюджет субъекта
        /// </summary>
        QuotaToConsRegion = 3,
        /// <summary>
        /// контингенте в консолидированный бюджет муниципального района
        /// </summary>
        QuotaToConsMR = 4,
        /// <summary>
        /// х.з.
        /// </summary>
        Unknown = -1
    }

    internal enum TerritoryType
    {
        /// <summary>
        /// бюджет субъекта
        /// </summary>
        SB,
        /// <summary>
        /// бюджет муниципального района
        /// </summary>
        MR,
        /// <summary>
        /// бюжет городского поселения
        /// </summary>
        GP,
        /// <summary>
        /// бюджет сельского поселения
        /// </summary>
        SP,
        /// <summary>
        /// бюджет городского округа
        /// </summary>
        GO,
        /// <summary>
        /// межселенные территории
        /// </summary>
        MT,
        /// <summary>
        /// бюджет районного центра
        /// </summary>
        RC,
        /// <summary>
        /// х.з.
        /// </summary>
        Unknown
    }

    public partial class DisintRules : DisposableObject, IDisintRules
    {
        private IEntity planIncDivide;

        private static string f_D_FOPlanIncDivide = "3f71b13b-3e87-45ad-8f72-1d023da07d10";

        private NormativesSplitService splitService;

        private int newVariantId = -1;

        public string SplitData(int variantId, int refVariantType, bool checkNormativeType, int splitVariant, int splitYear)
        {
            newVariantId = splitVariant;
            return InnerSplitData(variantId, refVariantType, checkNormativeType, ref splitVariant, splitYear);
        }

        public string SplitData(int variantId, int refVariantType, bool checkNormativeType, ref int newVariantIdId, int splitYear)
        {
            newVariantId = -1;
            return InnerSplitData(variantId, refVariantType, checkNormativeType, ref newVariantIdId, splitYear);
        }

        private string InnerSplitData(int variantId, int refVariantType, bool checkNormativeType, ref int newVariantIdId, int splitYear)
        {
            IClassifiersProtocol protocol = null;
            try
            {
                using (IDatabase db = activeScheme.SchemeDWH.DB)
                {
                    // начинаем расщепление
                    protocol =
                        (IClassifiersProtocol)
                        activeScheme.GetProtocol(Assembly.GetExecutingAssembly().ManifestModule.Name);
                    NormativesUtils.SendMessageToProtocol(protocol,
                        string.Format("Запуск операции расщепления данных по варианту с id = {0}", variantId),
                            ClassifiersEventKind.ceInformation, variantId);

                    splitService = new NormativesSplitService(activeScheme);
                    planIncDivide = activeScheme.RootPackage.FindEntityByName(f_D_FOPlanIncDivide);
                    // если сохраняем данные расщепления на заранее указанный вариант, удалим данные по этому варианту
                    if (newVariantId != -1)
                    {
                        DeleteSplitData(db, newVariantId, splitYear, planIncDivide.FullDBName);
                    }

                    string yearFilter = splitYear > 0
                        ? string.Format(" and RefYearDayUNV like '{0}____' ", splitYear)
                        : string.Empty;
                    
                    // запрашиваем данные, которые будем расщеплять
                    DataTable dtIds =
                        (DataTable)
                        db.ExecQuery(
                            string.Format("select id from f_D_FOPlanInc where RefVariant = {0}{1} order by id asc", variantId, yearFilter),
                            QueryResultTypes.DataTable);

                    if (dtIds.Rows.Count == 0)
                    {
                        NormativesUtils.SendMessageToProtocol(protocol,
                                                              string.Format(
                                                                  "Данные для расщепления по варианту c id = {0} не найдены",
                                                                  variantId), ClassifiersEventKind.ceWarning, variantId);
                        return "Данные для расщепления не найдены";
                    }
                    // расщепляем по 5000 записей
                    int firstId = Convert.ToInt32(dtIds.Rows[0][0]);
                    int lastId = Convert.ToInt32(dtIds.Rows[dtIds.Rows.Count - 1][0]);
                    int tmpId = firstId + 5000;
                    if (tmpId > lastId)
                        tmpId = lastId;

                    // количество записей, данные по которым были обработаны
                    int splittedRowsCount = 0;
                    // количество записей, которые были записаны на новый вариант с результатами расщепления
                    int resultRowsCount = 0;

                    while (firstId <= lastId)
                    {
                        string selectDataQuery =
                            string.Format(
                                @"select inc.ID, inc.SourceID, inc.Fact, inc.Design, inc.PlanPer, inc.Estimate, inc.Forecast, inc.Total, inc.TaxResource, inc.Restructuring, inc.Arrears,
                                inc.Priorcharge, inc.RefVariant, inc.RefKD, inc.RefRegions, inc.RefKVSR, inc.RefFODepartments, inc.RefYearDayUNV, inc.RefTaxObjects, inc.RefOrganizations, kd.CodeStr as Code, reg.Name
                                from f_D_FOPlanInc inc, d_KD_PlanIncomes kd, d_Regions_Plan reg where inc.RefVariant = {0}{1} and (inc.id between ? and ?) and inc.RefKD = kd.ID and inc.RefRegions = reg.ID", variantId, yearFilter);
                        DataTable dtDataForSplit = (DataTable) db.ExecQuery(selectDataQuery, QueryResultTypes.DataTable,
                                                                            new DbParameterDescriptor("p0", firstId),
                                                                            new DbParameterDescriptor("p1", tmpId));
                        
                        using (IDataUpdater du = planIncDivide.GetDataUpdater("1 = 2", null))
                        {
                            DataTable dtSplittedData = new DataTable();
                            du.Fill(ref dtSplittedData);
                            // производим расщепление по десять тыщ записей)
                            foreach (DataRow rowForSplit in dtDataForSplit.Rows)
                            {
                                int year = Convert.ToInt32(rowForSplit["RefYearDayUNV"].ToString().Substring(0, 4));
                                // расщепляем по одной записи
                                DataTable dtClone = dtSplittedData.Clone();
                                if (splitService.SplitData(rowForSplit, dtClone, checkNormativeType, year,
                                                       (VariantType)refVariantType, db, protocol, variantId))
                                    splittedRowsCount++;
                                resultRowsCount += dtClone.Rows.Count;
                                // копируем данные в базу
                                foreach (DataRow row in dtClone.Rows)
                                {
                                    // создаем новый вариант если есть какие нибудь расщепленные данные
                                    row["RefVariant"] = CreateNewVariant(variantId);
                                    dtSplittedData.Rows.Add(row.ItemArray);
                                }
                            }
                            du.Update(ref dtSplittedData);
                        }

                        firstId = tmpId + 1;
                        tmpId = firstId + 5000;
                        if (tmpId > lastId)
                            tmpId = lastId;
                    }
                    newVariantIdId = newVariantId;

                    NormativesUtils.SendMessageToProtocol(protocol,
                        string.Format("По варианту с  id = {0} было расщеплено {1} записей",
                            variantId, dtIds.Rows.Count), ClassifiersEventKind.ceInformation, variantId);
                    // если вариант был создан и какие то данные были расщеплены, выведем сообщение об успешном завершении
                    if (newVariantId != -1)
                    {
                        NormativesUtils.SendMessageToProtocol(protocol,
                            string.Format(
                                "Расщепление данных по варианту с  id = {0} успешно завершено",
                                variantId), ClassifiersEventKind.ceSuccefullFinished, variantId);
                        return string.Format("Расщепление данных успешно завершено. В результате создан вариант c id '{0}' {1} Результат расщепления : обработано {2} записей. Расщеплено {3} записей. Сохранено {4} записей. {5}Подробности смотри в протоколе расщепления",
                            newVariantId, Environment.NewLine, dtIds.Rows.Count, splittedRowsCount, resultRowsCount, Environment.NewLine);
                    }
                    else
                    {
                        NormativesUtils.SendMessageToProtocol(protocol,
                            string.Format(
                                "Данные по варианту с id = {0} расщеплены не были. Вариант с результатами расщепления не был создан",
                                variantId), ClassifiersEventKind.ceWarning, variantId);
                        return string.Empty;
                    }
                }
            }
            finally
            {
                if (protocol != null)
                    protocol.Dispose();
            }
        }

        #region вспомогательные методы

        /// <summary>
        /// создание нового варианта
        /// </summary>
        /// <param name="oldVariantId"></param>
        /// <returns></returns>
        private int CreateNewVariant(int oldVariantId)
        {
            if (newVariantId != -1)
                return newVariantId;
            IEntity variant = activeScheme.RootPackage.FindEntityByName(NormativesObjectKeys.d_Variant_PlanIncomes);
            using (IDataUpdater du = variant.GetDataUpdater())
            {
                DataTable dtVariant = new DataTable();
                du.Fill(ref dtVariant);
                DataRow oldVariant = dtVariant.Select(string.Format("ID = {0}", oldVariantId))[0];
                string variantName = dtVariant.Select(string.Format("ID = {0}", oldVariantId))[0]["Name"].ToString();
                string newVariantName = string.Empty;
                DataRow[] rows = dtVariant.Select(string.Format("Name = '{0}'", variantName));
                int num = 1;
                while (rows.Length != 0)
                {
                    newVariantName = string.Concat("Результаты расщепления: ", variantName, string.Format("({0})", num));
                    rows = dtVariant.Select(string.Format("Name like '{0}'", newVariantName));
                    num++;
                }
                DataRow newRow = dtVariant.Rows.Add(oldVariant.ItemArray);
                int newId = variant.GetGeneratorNextValue;
                newRow["ID"] = newId;
                newRow["Name"] = newVariantName;
                du.Update(ref dtVariant);
                newVariantId = newId;
                return newId;
            }
        }

        private void DeleteSplitData(IDatabase db, int variant, int year, string tableName)
        {
            string yearFilter = year > 0
                        ? string.Format(" and RefYearDayUNV like '{0}____' ", year)
                        : string.Empty;
            db.ExecQuery(string.Format("delete from {0} where RefVariant = ?{1}", tableName, yearFilter), QueryResultTypes.NonQuery,
                         new DbParameterDescriptor("p0", variant));
        }

        #endregion

    }
}
