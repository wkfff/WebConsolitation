using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.FactTables;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// Соблюдение требований БК РФ Саратов
        /// </summary>
        public DataTable[] GetBKSaratovData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[1];
            dtTables[0] = CreateReportCaptionTable(55);
            var drData = dtTables[0].Rows.Add();
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var fact1Entity = ConvertorSchemeLink.GetEntity(f_S_Plan.InternalKey);
            var fact2Entity = ConvertorSchemeLink.GetEntity(f_D_FOPlanIncDivide.InternalKey);
            var fact3Entity = ConvertorSchemeLink.GetEntity(f_R_FO26R.InternalKey);
            var kifEntity = ConvertorSchemeLink.GetEntity(d_KIF_Plan.InternalKey);
            var kdPlanEntity = ConvertorSchemeLink.GetEntity(d_KD_PlanIncomes.InternalKey);
            var rplanEntity = ConvertorSchemeLink.GetEntity(d_R_Plan.InternalKey);
            var ekrEntity = ConvertorSchemeLink.GetEntity(d_EKR_PlanOutcomes.InternalKey);
            var variant1 = reportParams[ReportConsts.ParamVariantID];
            var variant2 = reportParams[ReportConsts.ParamVariantDID];
            var variant3 = reportParams[ReportConsts.ParamVariantRID];
            var refParams1 = new Dictionary<string, string> {{kifEntity.FullDBName, "RefKif"}};
            var refParams2 = new Dictionary<string, string> {{kdPlanEntity.FullDBName, "RefKD"}};
            var refParams3 = new Dictionary<string, string>
                                 {
                                     {ekrEntity.FullDBName, "RefEKR"},
                                     {rplanEntity.FullDBName, "RefR"}
                                 };

            var sourceQuery = String.Format("select k.id from HUB_DATASOURCES k where k.SUPPLIERCODE = 'ФО' and k.DATACODE = 29 and k.YEAR = {0}", year);
            var dbHelper = new ReportDBHelper(scheme);
            var dtSource = dbHelper.GetTableData(sourceQuery);
            
            if (dtSource.Rows.Count > 0)
            {
                var sourceID = dtSource.Rows[0][0].ToString();
                // Общие парметры запросов
                var queryParams1 = new Dictionary<string, string>
                                       {
                                           {
                                               "RefYearDayUNV",
                                               String.Format(">={0}0000 and c.RefYearDayUNV <{1}0000", year, year + 1)
                                               },
                                           {"RefBudgetLevels", "3"},
                                           {"RefSVariant", variant1},
                                           {"SourceID", sourceID}
                                       };

                var queryParams2 = new Dictionary<string, string>
                                       {
                                           {"RefYearDayUNV", queryParams1["RefYearDayUNV"]},
                                           {"RefBudLevel", "3"},
                                           {"RefVariant", variant2},
                                           {"SourceID", sourceID}
                                       };

                var queryParams3 = new Dictionary<string, string>
                                       {
                                           {"RefYearDayUNV", queryParams1["RefYearDayUNV"]},
                                           {"RefBdgtLvls", "3"},
                                           {"RefVariant", variant3},
                                           {"SourceID", sourceID}
                                       };

                // Пункт 1 
                // Доходы - всего
                SetCodeParams(queryParams2, "Code12=1,2,3");
                var totalSum1 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // Налоговые и неналоговые доходы
                SetCodeParams(queryParams2, "Code12=1,3");
                var detailSum11 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // Налоговые, неналоговые
                SetCodeParams(queryParams2, "Code12=1");
                var detailSum111 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // предпринимательская деятельность
                SetCodeParams(queryParams2, "Code12=3");
                var detailSum112 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // возврат в федеральный бюджет
                SetCodeParams(queryParams2, "Code12=1;Code13=18;Code14=1");
                var detailSum113 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // возврат в областной бюджет
                SetCodeParams(queryParams2, "Code12=1;Code13=18;Code14=2");
                var detailSum114 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // безвозмездные поступления
                SetCodeParams(queryParams2, "Code12=2");
                var detailSum12 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // безвозмездные поступления от др.бюджетов
                SetCodeParams(queryParams2, "Code12=2");
                var detailSum121 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // дотации на выравнивание
                SetCodeParams(queryParams2, "Code12=2;Code13=2;Code14=1");
                var detailSum122 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // дотации на меры сбалансирования
                SetCodeParams(queryParams2, "Code12=2;Code13=2;Code14=1;Code16=2,3,4,5");
                var detailSum123 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // дотации для ЗАТО
                SetCodeParams(queryParams2, "Code12=2;Code13=2;Code14=1;Code16=6,7,8,9");
                var detailSum124 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // субсидии
                SetCodeParams(queryParams2, "Code12=2;Code13=2;Code14=2");
                var detailSum125 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // субвенции
                SetCodeParams(queryParams2, "Code12=2;Code13=2;Code14=3");
                var detailSum126 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // иные межбюджетные трансферты
                SetCodeParams(queryParams2, "Code12=2;Code13=2;Code14=4,5");
                var detailSum127 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // прочие безвозмездные поступления от гос корпораций
                SetCodeParams(queryParams2, "Code12=2;Code13=2;Code14=9");
                var detailSum128 = GetBKSumValue(fact2Entity, queryParams2, refParams2);
                // Пункт 2
                // Расходы - всего
                var totalSum2 = GetBKSumValue(fact3Entity, queryParams3, refParams3);
                // Пункт 3
                // Дефицит
                var detailSum31 = totalSum1 - totalSum2;
                // Пункт 4
                // Кредиты от кредитных организаций
                SetCodeParams(queryParams1, "Code12=1;Code13=2");
                var detailSum41 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // получение
                SetCodeParams(queryParams1, "Code12=1;Code13=2;Code18=7");
                var detailSum411 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // погашение
                SetCodeParams(queryParams1, "Code12=1;Code13=2;Code18=8");
                var detailSum412 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // Бюджетные кредиты от федер.бюджета
                SetCodeParams(queryParams1, "Code12=1;Code13=3");
                var detailSum42 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // получение
                SetCodeParams(queryParams1, "Code12=1;Code13=3;Code18=7");
                var detailSum421 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // погашение
                SetCodeParams(queryParams1, "Code12=1;Code13=3;Code18=8");
                var detailSum422 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // Остатки средств
                SetCodeParams(queryParams1, "Code12=1;Code13=5");
                var detailSum43 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // увеличение
                SetCodeParams(queryParams1, "Code12=1;Code13=3;Code18=5");
                var detailSum431 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // уменьшение
                SetCodeParams(queryParams1, "Code12=1;Code13=3;Code18=6");
                var detailSum432 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // Поступление от продажи акций
                SetCodeParams(queryParams1, "Code12=1;Code13=6;Code14=1");
                var detailSum44 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // возврат
                SetCodeParams(queryParams1, "Code12=1;Code13=6;Code14=5;Code15=1;Code18=6");
                var detailSum451 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // предоставление
                SetCodeParams(queryParams1, "Code12=1;Code13=6;Code14=5;Code15=1;Code18=6");
                var detailSum452 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // Бюджетные кредиты юридическим лицам
                var detailSum45 = detailSum451 + detailSum452;
                // возврат
                SetCodeParams(queryParams1, "Code12=1;Code13=6;Code14=5;Code15=2;Code18=6");
                var detailSum461 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // за счет целевых федеральных средств
                SetCodeParams(queryParams1, "Code12=1;Code13=6;Code14=7;Code18=5");
                var detailSum4621 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // за счет областных средств
                SetCodeParams(queryParams1, "Code12=1;Code13=6;Code14=5;Code15=2;Code18=5");
                var detailSum4622 = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                // предоставление
                var detailSum462 = detailSum4621 + detailSum4622;
                // Бюджетные кредиты местным бюджетам
                var detailSum46 = detailSum461 + detailSum462;
                // Дефицит поступления от продажи акций
                var detailSum32 = detailSum42 + detailSum43 + detailSum44;
                // Дефицит за искл.поступлений от продажи акций
                var detailSum33 = detailSum31 + detailSum32;
                // Источники - всего
                var totalSum4 = detailSum41 + detailSum42 + detailSum43 + detailSum44 + detailSum45 + detailSum46;
                SetCodeParams(queryParams3, "Code21=2;Code22=3");
                var rod = GetBKSumValue(fact3Entity, queryParams3, refParams3);
                SetCodeParams(queryParams3, String.Empty);
                var r = GetBKSumValue(fact3Entity, queryParams3, refParams3);
                SetCodeParams(queryParams3, "FKR=1103");
                var rsub = GetBKSumValue(fact3Entity, queryParams3, refParams3);
                // расходов на обслуж.гос.долга в расходах без субвенций из фед.б-та  
                decimal detailSum65 = 0;
                
                if (Math.Abs(r - rsub) > 0)
                {
                    detailSum65 = rod / (r - rsub);
                }

                decimal detailSum61 = 0;
                decimal detailSum62 = 0;
                
                if (Math.Abs(detailSum11) > 0)
                {
                    // дефицита в доходах без учета безвозмездных поступлений 
                    detailSum61 = detailSum31 * 100 / detailSum11;
                    // дефицита за исключением поступлений от акций 
                    detailSum62 = detailSum33 * 100 / detailSum11;
                }
                
                // Предельный размер дефицита (15%) 
                var detailSum66 = detailSum11 * (decimal)0.15;
                var procParams = new CommonQueryParam
                                     {
                                         sourceID = sourceID,
                                         variantID = variant1,
                                         yearStart = GetYearStart(year),
                                         yearEnd = GetYearEnd(year)
                                     };

                var cb = GetCapitalSum(procParams);
                var kp = GetCreditSum(procParams);
                var g = GetGarantSum(procParams);
                // Объем государственного долга
                var totalSum5 = cb[3] + kp[4] + g[5];
                decimal detailSum63 = 0;
                decimal detailSum64 = 0;
                
                if (Math.Abs(detailSum11) > 0)
                {
                    // гос.долга в доходах без учета утвержд.объема безвозм.поступ 
                    detailSum63 = totalSum5 * 100 / detailSum11;
                    // гос.долга с учетом бюдж.кредитов в доходах без учета утвержд.объема безвозм.поступ 
                    detailSum64 = (totalSum5 + detailSum421) * 100 / detailSum11;
                }
                
                // столбик результатов(как в шаблоне для простоты)
                drData[00] = year;
                drData[01] = year;
                drData[05] = totalSum1;
                drData[06] = detailSum11;
                drData[07] = detailSum111;
                drData[08] = detailSum112;
                drData[09] = detailSum113;
                drData[10] = detailSum114;
                drData[11] = detailSum12;
                drData[12] = detailSum121;
                drData[13] = detailSum122;
                drData[14] = detailSum123;
                drData[15] = detailSum124;
                drData[16] = detailSum125;
                drData[17] = detailSum126;
                drData[18] = detailSum127;
                drData[19] = detailSum128;
                drData[20] = totalSum2;
                drData[21] = detailSum31;
                drData[22] = detailSum32;
                drData[23] = detailSum33;
                drData[24] = totalSum4;
                drData[25] = detailSum41;
                drData[26] = detailSum411;
                drData[27] = detailSum412;
                drData[28] = detailSum42;
                drData[29] = detailSum421;
                drData[30] = detailSum422;
                drData[31] = detailSum43;
                drData[32] = detailSum431;
                drData[33] = detailSum432;
                drData[34] = detailSum44;
                drData[35] = detailSum45;
                drData[36] = detailSum451;
                drData[37] = detailSum452;
                drData[38] = detailSum46;
                drData[39] = detailSum461;
                drData[40] = detailSum462;
                drData[41] = detailSum4621;
                drData[42] = detailSum4621;
                drData[43] = totalSum5;
                drData[45] = detailSum61;
                drData[46] = detailSum62;
                drData[47] = detailSum63;
                drData[48] = detailSum64;
                drData[49] = detailSum65;
                drData[50] = detailSum66;
            }

            return dtTables;
        }

        public DataTable[] GetAdminBudgetSourceDeficitData(Dictionary<string, string> reportParams)
        {
            var kifEntity = ConvertorSchemeLink.GetEntity(d_KIF_Plan.InternalKey);
            var kvsrEntity = ConvertorSchemeLink.GetEntity(d_KVSR_Plan.InternalKey);
            var dtTables = new DataTable[2];
            dtTables[0] = CreateReportTableBudgetSourceDeficit();
            dtTables[1] = CreateReportTableBudgetSourceDeficit();
            dtTables[1].Rows.Add()[0] = reportParams[ReportConsts.ParamYear];

            var sourceQuery =
                String.Format("select k.id from HUB_DATASOURCES k " +
                    " where k.SUPPLIERCODE = 'ФО' and k.DATACODE = 29 and k.YEAR = {0} ",
                    reportParams[ReportConsts.ParamYear]);

            var dbHelper = new ReportDBHelper(scheme);
            var dtSource = dbHelper.GetTableData(sourceQuery);

            if (dtSource.Rows.Count > 0)
            {
                var sourceID = Convert.ToInt32(dtSource.Rows[0][0]);
                var kifQuery = String.Format("select k.CodeStr, k.Name from {0} k where k.SourceID = {1} and k.CodeStr not like '000%'",
                        kifEntity.FullDBName, sourceID);
                var kvsrQuery = String.Format("select k.Code, k.Name from {0} k where k.SourceID = {1} and k.Code > 0  group by code, name order by code",
                        kvsrEntity.FullDBName, sourceID);
                var dtKIF = dbHelper.GetTableData(kifQuery);
                var dtKVSR = dbHelper.GetTableData(kvsrQuery);

                foreach (DataRow kvsrRow in dtKVSR.Rows)
                {
                    var value = kvsrRow["Code"].ToString();
                    var part1 = value.PadLeft(3, '0');
                    var drKIF = dtKIF.Select(String.Format("CodeStr like '{0}%'", part1));

                    if (drKIF.Length <= 0)
                    {
                        continue;
                    }

                    var drResult = dtTables[0].Rows.Add();
                    drResult[0] = part1;
                    drResult[8] = kvsrRow["Name"];
                    drResult[9] = 1;

                    foreach (var kifRow in drKIF)
                    {
                        drResult = dtTables[0].Rows.Add();
                        var codeValue = kifRow["CodeStr"].ToString();
                        part1 = codeValue.Substring(00, 3);
                        var part2 = codeValue.Substring(03, 2);
                        var part3 = codeValue.Substring(05, 2);
                        var part4 = codeValue.Substring(07, 2);
                        var part5 = codeValue.Substring(09, 2);
                        var part6 = codeValue.Substring(11, 2);
                        var part7 = codeValue.Substring(13, 4);
                        var part8 = codeValue.Substring(17, 3);
                        drResult[0] = part1;
                        drResult[1] = part2;
                        drResult[2] = part3;
                        drResult[3] = part4;
                        drResult[4] = part5;
                        drResult[5] = part6;
                        drResult[6] = part7;
                        drResult[7] = part8;
                        drResult[8] = kifRow["Name"];
                        drResult[9] = 2;
                    }
                }
            }

            return dtTables;
        }

        /// <summary>
        /// Заполнитель для отчета "Источники финансирования дефицита бюджета"
        /// </summary>
        public DataTable[] GetBudgetSourceDeficitData(Dictionary<string, string> reportParams, int yearCount, bool invertSign)
        {
            decimal totalSum = 0;
            var kifEntity = ConvertorSchemeLink.GetEntity(d_KIF_Plan.InternalKey);
            var planEntity = ConvertorSchemeLink.GetEntity(f_S_Plan.InternalKey);
            var dtTables = new DataTable[2];
            dtTables[0] = CreateReportTableBudgetSourceDeficit();
            dtTables[1] = CreateReportTableBudgetSourceDeficit();
            var yearLim1 = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            
            if (yearCount > 1)
            {
                yearLim1++;
            }

            var kifQuery =
                String.Format("select k.id, k.ParentID, k.CodeStr, k.Name, k.RefKIF " +
                " from {0} k ", kifEntity.FullDBName);
            var planIFQuery = String.Format(
                "select k.id, p.Forecast, k.CodeStr, k.RefKIF, k.ParentID, k.Name, p.RefSVariant, p.SourceID, k.CodeStr as SortStr, p.RefYearDayUnv " +
                " from {0} p, {1} k " +
                "where " +
                " k.id = p.RefKIF " +
                " and p.RefSVariant in ({2}) " +
                " and p.RefYearDayUnv >= {3} and p.RefYearDayUnv < {4} " +
                " order by k.CodeStr",
                planEntity.FullDBName, kifEntity.FullDBName,
                reportParams[ReportConsts.ParamVariantID],
                10000 * yearLim1,
                10000 * (yearLim1 + yearCount));

            var dbHelper = new ReportDBHelper(scheme);
            var dtKIF = dbHelper.GetTableData(kifQuery);
            var dtPlan = dbHelper.GetTableData(planIFQuery);

            foreach (DataRow planRow in dtPlan.Rows)
            {
                if (planRow["SortStr"] != DBNull.Value && planRow["SortStr"].ToString().Length > 2)
                {
                    planRow["SortStr"] = planRow["SortStr"].ToString().Remove(0, 3);
                }
            }
            
            dtPlan = DataTableUtils.SortDataSet(dtPlan, "SortStr asc");

            foreach (DataRow planRow in dtPlan.Rows)
            {
                var drFind = FindDataRow(dtTables[0], planRow["CodeStr"].ToString(), "F13");
                var checkYear = CheckPeriod(planRow["RefYearDayUnv"], yearLim1);

                if (planRow["CodeStr"].ToString().Length <= 10)
                {
                    continue;
                }

                decimal sum = 0;
                var direction = Convert.ToInt32(planRow["RefKIF"]);
                    
                if (planRow["Forecast"] != DBNull.Value)
                {
                    sum = ConvertTo1000(direction * Convert.ToDecimal(planRow["Forecast"]));
                }

                if (checkYear)
                {
                    totalSum += sum;
                }

                if (drFind == null)
                {
                    var drResult = dtTables[0].Rows.Add();
                    AddDeficitRecord(drResult, planRow, sum, checkYear);
                }
                else
                {
                    if (checkYear)
                    {
                        drFind[9] = Convert.ToDecimal(drFind[9]) + sum;
                    }
                }

                var parentRow = FindDataRow(dtTables[0], planRow["ParentID"].ToString(), "F11");

                if (parentRow != null)
                {
                    // Если иерархия уже была построена
                    while (parentRow != null)
                    {
                        if (checkYear)
                        {
                            parentRow[9] = Convert.ToDecimal(parentRow[9]) + sum;
                        }

                        parentRow = parentRow["F12"] == DBNull.Value
                                        ? null
                                        : FindDataRow(dtTables[0], parentRow["F12"].ToString(), "F11");
                    }
                }
                else
                {
                    // Если иерархия еще не была построена
                    if (planRow["ParentID"] != DBNull.Value)
                    {
                        var drKIF = dtKIF.Select(String.Format("id = {0}", planRow["ParentID"]));
                        var counter = 0;

                        while (drKIF.Length > 0)
                        {
                            var drParent = dtTables[0].NewRow();
                            AddDeficitRecord(drParent, drKIF[0], sum, checkYear);
                            dtTables[0].Rows.InsertAt(drParent, dtTables[0].Rows.Count - 1 - counter);
                            counter++;
                            parentRow = FindDataRow(dtTables[0], drKIF[0]["ParentID"].ToString(), "F11");

                            if (parentRow != null)
                            {
                                while (parentRow != null)
                                {
                                    if (checkYear)
                                    {
                                        parentRow[9] = Convert.ToDecimal(parentRow[9]) + sum;
                                    }

                                    parentRow = parentRow["F12"] == DBNull.Value
                                                    ? null
                                                    : FindDataRow(dtTables[0], parentRow["F12"].ToString(), "F11");
                                }

                                drKIF = dtKIF.Select("0 = 1");
                            }
                            else
                            {
                                drKIF = drKIF[0]["ParentID"] != DBNull.Value
                                            ? dtKIF.Select(String.Format("id = {0}", drKIF[0]["ParentID"]))
                                            : dtKIF.Select("0 = 1");
                            }
                        }
                    }
                }
            }
            
            // Омские заморочки с кодами
            var unsignedCode1 = GetConstCode(scheme, "KIFGrnt");
            var unsignedCode2 = unsignedCode1;
            
            if (unsignedCode1 != String.Empty)
            {
                unsignedCode1 = unsignedCode1.Substring(3, 6);
            }
            
            if (unsignedCode1.Length > 4)
            {
                unsignedCode2 = unsignedCode1.Substring(0, 4);
            }

            var regionCode = GetOKTMOCode(scheme);
            
            if (invertSign)
            {
                foreach (DataRow dr in dtTables[0].Rows)
                {
                    if (dr[7].ToString() != "000"
                        || String.Format("{0}{1}{2}", dr[1], dr[2], dr[3]) == unsignedCode1
                        || regionCode == "52 701 000" && String.Format("{0}{1}", dr[1], dr[2]) == unsignedCode2)
                    {
                        dr[9] = Math.Abs(Convert.ToDecimal(dr[9]));
                    }
                }
            }
            if (regionCode == "52 701 000")
            {
                DataRow drDelete = null;
                
                foreach (DataRow dr in dtTables[0].Rows)
                {
                    var fullCode = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                        dr[0], dr[1], dr[2], dr[3], dr[4], dr[5], dr[6], dr[7]);

                    if (fullCode == "00001000000000000000")
                    {
                        drDelete = dr;
                    }
                }
                if (drDelete != null) dtTables[0].Rows.Remove(drDelete);
            }

            var drParams = CreateReportParamsRow(dtTables);
            drParams[0] = yearLim1;
            drParams[9] = totalSum;
            return dtTables;
        }

        public DataTable[] GetBudgetSourceDeficitPeriodData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            dtTables[0] = CreateReportCaptionTable(15);
            dtTables[1] = CreateReportCaptionTable(15);
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]) + 1;
            var drCaption = dtTables[1].Rows.Add();
            drCaption[00] = year;
            var dtResultYear1 = GetBudgetSourceDeficitData(reportParams, 2, true);
            reportParams[ReportConsts.ParamYear] = Convert.ToString(year + 1);
            var dtResultYear2 = GetBudgetSourceDeficitData(reportParams, 1, true);
            drCaption[01] = reportParams[ReportConsts.ParamYear];
            drCaption[09] = GetLastRow(dtResultYear1[1])[9];
            drCaption[10] = GetLastRow(dtResultYear2[1])[9];
            var notFoundRows1 = new Collection<string>();

            foreach (DataRow dr1 in dtResultYear1[0].Rows)
            {
                var drResult = dtTables[0].Rows.Add();
                notFoundRows1.Add(dr1[10].ToString());
                foreach (DataRow dr2 in dtResultYear2[0].Rows)
                {
                    if (dr1[10].ToString() != dr2[10].ToString())
                    {
                        continue;
                    }

                    drResult[10] = dr2[9];
                    notFoundRows1.Remove(dr1[10].ToString());
                }

                for (var i = 0; i < 10; i++)
                {
                    drResult[i] = dr1[i];
                }
            }
            return dtTables;
        }

        /// <summary>
        /// Отчета "Соответствие требованиям Бюджетного кодекса"
        /// </summary>
        public DataTable[] GetBKIndicatorsData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[5];
            dtTables[0] = CreateReportBKIndicatorsStructure();
            var baseYear = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var fact1Entity = ConvertorSchemeLink.GetEntity(f_S_Plan.InternalKey);
            var fact2Entity = ConvertorSchemeLink.GetEntity(f_D_FOPlanIncDivide.InternalKey);
            var fact3Entity = ConvertorSchemeLink.GetEntity(f_R_FO26R.InternalKey);
            var kifEntity = ConvertorSchemeLink.GetEntity(d_KIF_Plan.InternalKey);
            var kdPlanEntity = ConvertorSchemeLink.GetEntity(d_KD_PlanIncomes.InternalKey);
            var rplanEntity = ConvertorSchemeLink.GetEntity(d_R_Plan.InternalKey);
            var ekrEntity = ConvertorSchemeLink.GetEntity(d_EKR_PlanOutcomes.InternalKey);
            var variant1 = reportParams[ReportConsts.ParamVariantID];
            var variant2 = reportParams[ReportConsts.ParamVariantDID];
            var variant3 = reportParams[ReportConsts.ParamVariantRID];
            var refParams1 = new Dictionary<string, string> {{kifEntity.FullDBName, "RefKif"}};
            var refParams2 = new Dictionary<string, string> {{kdPlanEntity.FullDBName, "RefKD"}};
            var refParams3 = new Dictionary<string, string>
                                 {
                                     {ekrEntity.FullDBName, "RefEKR"},
                                     {rplanEntity.FullDBName, "RefR"}
                                 };

            // Структура для проврочных полей
            for (var i = 1; i < 5; i++)
            {
                dtTables[i] = CreateReportCaptionTable(3);
                
                for (var j = 0; j < 20; j++)
                {
                    dtTables[i].Rows.Add();
                }
            }

            var sourceQuery = String.Format("select k.id from HUB_DATASOURCES k where k.SUPPLIERCODE = 'ФО' and k.DATACODE = 29 and k.YEAR = {0}", baseYear);
            var dbHelper = new ReportDBHelper(scheme);
            var dtSource = dbHelper.GetTableData(sourceQuery);
            var sourceID = ReportConsts.UndefinedKey;
            
            if (dtSource.Rows.Count > 0)
            {
                sourceID = dtSource.Rows[0][0].ToString();
            }

            for (var i = 0; i < 3; i++)
            {
                var year = baseYear + i;
                var drBK = dtTables[0].Rows.Add();
                // Общие парметры запросов
                var queryParams1 = new Dictionary<string, string>
                                       {
                                           {
                                               "RefYearDayUNV",
                                               String.Format(">={0}0000 and c.RefYearDayUNV <{1}0000", year, year + 1)
                                               },
                                           {"RefBudgetLevels", "3"},
                                           {"RefSVariant", variant1},
                                           {"SourceID", sourceID}
                                       };

                var queryParams2 = new Dictionary<string, string>
                                       {
                                           {"RefYearDayUNV", queryParams1["RefYearDayUNV"]},
                                           {"RefBudLevel", "3"},
                                           {"RefVariant", variant2},
                                           {"SourceID", sourceID}
                                       };

                var queryParams3 = new Dictionary<string, string>
                                       {
                                           {"RefYearDayUNV", queryParams1["RefYearDayUNV"]},
                                           {"RefBdgtLvls", "3"},
                                           {"RefVariant", variant3},
                                           {"SourceID", sourceID}
                                       };

                // Поехали считать показатели
                SetCodeParams(queryParams1, "Code12=1;Code13=6;Code14=1");
                var pa = GetBKSumValue(fact1Entity, queryParams1, refParams1);

                SetCodeParams(queryParams1, "Code12=1;Code13=5");
                var sos = GetBKSumValue(fact1Entity, queryParams1, refParams1);
                
                if (sos < 0)
                {
                    sos = 0;
                }

                SetCodeParams(queryParams2, "Code12=1,2,3");
                var d = GetBKSumValue(fact2Entity, queryParams2, refParams2);

                SetCodeParams(queryParams2, "Code12=2");
                var bp = GetBKSumValue(fact2Entity, queryParams2, refParams2);

                SetCodeParams(queryParams1, "Code18=8");
                SetCodeParams(queryParams3, "Code10=92001140920311013;Code21=2;Code22=9");
                var pdo = GetBKSumValue(fact1Entity, queryParams1, refParams1) + GetBKSumValue(fact3Entity, queryParams3, refParams3);

                SetCodeParams(queryParams3, "Code21=2;Code22=3");
                var rod = GetBKSumValue(fact3Entity, queryParams3, refParams3);

                SetCodeParams(queryParams3, String.Empty);
                var p = GetBKSumValue(fact3Entity, queryParams3, refParams3);

                SetCodeParams(queryParams3, "FKR=1103");
                var psub = GetBKSumValue(fact3Entity, queryParams3, refParams3);

                // Поехали считать суммы по ИФ

                var procParams = new CommonQueryParam
                                     {
                                         sourceID = sourceID,
                                         variantID = variant1,
                                         yearStart = GetYearStart(year),
                                         yearEnd = GetYearEnd(year)
                                     };

                var cb = GetCapitalSum(procParams);
                var kp = GetCreditSum(procParams);
                var g = GetGarantSum(procParams);

                var gd = g[5] + kp[4] + cb[3];
                var oz = kp[4] + cb[3];
                var df = d - p;

                drBK[0] = year;
                
                if ((d - bp) != 0)
                {
                    drBK[1] = (df - pa - sos)/(d - bp);
                }

                if ((df + pdo) != 0)
                {
                    drBK[2] = oz/(df + pdo);
                }

                if ((d - bp) != 0)
                {
                    drBK[3] = gd/(d - bp);
                }

                if ((p - psub) != 0)
                {
                    drBK[4] = rod/(p - psub);
                }

                // ст.92.1.БК РФ 2 лист
                dtTables[1].Rows[0][i] = d;
                dtTables[1].Rows[1][i] = p;
                dtTables[1].Rows[2][i] = df;
                dtTables[1].Rows[3][i] = sos;
                dtTables[1].Rows[4][i] = bp;
                
                if (drBK[1] != DBNull.Value)
                {
                    dtTables[1].Rows[5][i] = Convert.ToDecimal(drBK[1]);
                }

                // ст.106.БК РФ 3 лист
                for (var kk = 0; kk < cb.Length; kk++)
                {
                    dtTables[2].Rows[kk][i] = cb[kk];
                }

                var startIndex = cb.Length;

                for (var kk = 0; kk < kp.Length; kk++)
                {
                    dtTables[2].Rows[startIndex + kk][i] = kp[kk];
                }
                
                startIndex += kp.Length;
                dtTables[2].Rows[startIndex + 0][i] = d;
                dtTables[2].Rows[startIndex + 1][i] = p;
                dtTables[2].Rows[startIndex + 2][i] = pdo;
                
                if (drBK[2] != DBNull.Value)
                {
                    dtTables[2].Rows[startIndex + 3][i] = Convert.ToDecimal(drBK[2]);
                }

                // ст.107.БК РФ 4 лист
                startIndex = 0;
                
                for (var kk = 0; kk < cb.Length; kk++)
                {
                    dtTables[3].Rows[kk][i] = cb[kk];
                }
                
                startIndex += cb.Length;
                
                for (var kk = 0; kk < kp.Length; kk++)
                {
                    dtTables[3].Rows[startIndex + kk][i] = kp[kk];
                }
                
                startIndex += kp.Length;
                
                for (var kk = 0; kk < g.Length; kk++)
                {
                    dtTables[3].Rows[startIndex + kk][i] = g[kk];
                }
                
                startIndex += g.Length;
                dtTables[3].Rows[startIndex + 0][i] = d;
                dtTables[3].Rows[startIndex + 1][i] = bp;
                
                if (drBK[3] != DBNull.Value)
                {
                    dtTables[3].Rows[startIndex + 2][i] = Convert.ToDecimal(drBK[3]);
                }

                // ст.111.БК РФ 5 лист
                dtTables[4].Rows[0][i] = rod;
                dtTables[4].Rows[1][i] = p;
                dtTables[4].Rows[2][i] = psub;
                
                if (drBK[4] != DBNull.Value)
                {
                    dtTables[4].Rows[3][i] = Convert.ToDecimal(drBK[4]);
                }
            }

            return dtTables;
        }
    }
}