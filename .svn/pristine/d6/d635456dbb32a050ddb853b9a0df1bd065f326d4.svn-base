using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using Krista.FM.Client.Reports.Common.CommonParamForm;
using Krista.FM.Client.Reports.Common.Commands;

namespace Krista.FM.Client.Reports
{
    public enum ReportTestType
    {
        [Description("Быстрая проверка")]
        i1,
        [Description("Полная проверка")]
        i2,
        [Description("Параноидальная проверка")]
        i3
    }

    // класс с общим списком классов отчетов для автотестирования
    public class TestReports
    {
        public Collection<object> reports = new Collection<object>();

        public TestReports()
        {
            reports.Clear();
        }
    }

    // класс с общим списком таблиц БД для автотестирования структуры
    public class TestTables
    {
        public Collection<Type> tables = new Collection<Type>();

        public TestTables()
        {
            tables.Clear();
        }
    }

    class ReportAutoTesterCommand : ExcelMacrosCommand
    {
        public ReportAutoTesterCommand()
        {
            key = "ReportAutoTester";
            caption = "Модуль тестирования отчетов";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddEnumParam(ReportConsts.ParamTestType, typeof(ReportTestType));
            paramBuilder.AddListParam(ReportConsts.ParamReportList);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.TestReports(reportParams);
        }
    }

    public partial class ReportDataServer
    {
        private const string testFieldKey = "key";
        private const string testFieldMasterRef = "MasterReference";
        private const string testFieldInternalKey = "InternalKey";
        private int testCounter;

        private static bool IsServiceField(string fieldName)
        {
            return fieldName == testFieldKey || fieldName == testFieldMasterRef || fieldName == testFieldInternalKey;
        }

        private string[] TestSingleEntity(Type type, string key)
        {
            var result = new string[2];
            var entity = scheme.RootPackage.FindEntityByName(key);
            result[0] = String.Empty;
            result[1] = entity.Caption;
            var dbHelper = new ReportDBHelper(scheme);
            var dtTest = dbHelper.GetEntityData(key, "0 = 1");

            foreach (var fi in type.GetFields())
            {
                if (!dtTest.Columns.Contains(fi.Name) && !IsServiceField(fi.Name))
                {
                    result[0] = Combine(result[0], fi.Name);
                }
            }

            result[0] = result[0].Trim(',');
            return result;
        }

        private DataTable TestStructure()
        {
            var dtResult = CreateReportCaptionTable(3);
            var rto = new TestTables();

            foreach (var type in rto.tables)
            {
                var fi = type.GetField(testFieldInternalKey);
                string entityKey;

                if (fi == null)
                {
                    fi = type.GetField(testFieldMasterRef);
                    var ass = scheme.RootPackage.FindAssociationByName(fi.GetValue(null).ToString());
                    entityKey = ass.RoleData.ObjectKey;
                }
                else
                {
                    entityKey = fi.GetValue(null).ToString();
                }

                var result = TestSingleEntity(type, entityKey);
                var drError = dtResult.Rows.Add();
                drError[0] = type.Name;
                drError[1] = result[0];
                drError[2] = result[1];
            }
            return dtResult;
        }

        private static IEnumerable<string> GetTestParamValues(string paramName, ParamContainer paramBuilder,
            ReportTestType testMode)
        {
            var result = new Collection<string>();            
            if (paramName == ReportConsts.ParamPhone || paramName == ReportConsts.ParamExecutor1 || paramName == ReportConsts.ParamExecutor2)
            {
                result.Add(String.Empty);
            }
            if (paramName == ReportConsts.ParamYear)
            {
                var yearCount = 1;
                var year = DateTime.Now.Year;

                if (testMode == ReportTestType.i1)
                {
                    yearCount = 0;
                    result.Add(year.ToString());
                }

                for (var i = year - yearCount; i < year + yearCount; i++)
                {
                    result.Add(i.ToString());
                }
            }

            if (paramName == ReportConsts.ParamEndDate || paramName == ReportConsts.ParamStartDate)
            {
                var yearCount = 1;
                var loDate = DateTime.Now.AddYears(-yearCount);
                
                if (testMode == ReportTestType.i1)
                {
                    yearCount = 0;
                    result.Add(DateTime.Now.ToShortDateString());
                }

                for (var i = 0; i < yearCount * 12; i++)
                {
                    loDate = loDate.AddMonths(1);
                    result.Add(GetMonthStart(loDate));
                    result.Add(GetMonthEnd(loDate));
                    if (!result.Contains(loDate.ToShortDateString())) result.Add(loDate.ToShortDateString());
                }
            }

            if (paramBuilder[paramName].EnumObj != null)
            {
                var values = Enum.GetValues(paramBuilder[paramName].EnumObj.GetType());

                foreach (var value in values)
                {
                    result.Add(value.ToString());
                }
            }

            if (paramBuilder[paramName].BookInfo != null)
            {
                var dtItems = paramBuilder[paramName].BookInfo.CreateDataList();

                if (testMode == ReportTestType.i1)
                {
                    if (dtItems.Rows.Count > 0) result.Add(dtItems.Rows[0]["id"].ToString());
                }
                else
                {
                    foreach (DataRow dr in dtItems.Rows)
                    {
                        result.Add(dr["id"].ToString());
                    }
                }
            }

            if (paramName == ReportConsts.ParamExchangeRate)
            {
                result.Add("30");
            }
            return result;
        }

        private string TestSingleReport(CommonReportsCommand cmd, Collection<string> fixedColumns, 
            ReportTestType testType)
        {
            var paramCount = cmd.paramBuilder.lstParams.Count;
            
            if (paramCount == 0)
            {
                cmd.GetReportData(cmd.paramBuilder.GetParams());
                return String.Empty;
            }

            // строим список изменяемых колонок
            var notFixedColumns = new Collection<string>();

            foreach (var pair in cmd.paramBuilder.lstParams)
            {
                if (!fixedColumns.Contains(pair.ParentName) && notFixedColumns.Count == 0)
                {
                    notFixedColumns.Add(pair.ParentName);
                }
            }

            foreach (var paramName in notFixedColumns)
            {
                var newFixedColumns = new Collection<string>();

                foreach (var fixedParam in fixedColumns)
                {
                    newFixedColumns.Add(fixedParam);
                }

                newFixedColumns.Add(paramName);
                IEnumerable<string> paramValues = GetTestParamValues(paramName, cmd.paramBuilder, testType);
                
                foreach (var paramValue in paramValues)
                {
                    cmd.paramBuilder.GetParams()[paramName] = paramValue;

                    if (newFixedColumns.Count == paramCount)
                    {
                        try
                        {
                            testCounter++;
                            cmd.GetReportData(cmd.paramBuilder.GetParams());
                        }
                        catch (Exception e)
                        {
                            var result = cmd.paramBuilder.GetParams().Aggregate(String.Empty, (current, pair) => 
                                Combine(current, FormFilterValue(pair.Key, pair.Value)));
                            return String.Format("Тест №{0} Параметры отчета : {1} Ошибка : {2}",
                                testCounter, result.Trim(','), e.Message);
                        }
                    }
                    else
                    {
                        TestSingleReport(cmd, newFixedColumns, testType);
                    }
                }
            }
            return String.Empty;
        }

        private DataTable TestFill(Dictionary<string, string> reportParams)
        {
            var dtResult = CreateReportCaptionTable(3);
            var rto = new TestReports();
            var testList = new Collection<string>(reportParams[ReportConsts.ParamReportList].Split(','));
            
            if (testList.Count > 0)
            {
                var codeLen = testList[0].Length;
                var testType = (ReportTestType)Enum.Parse(typeof(ReportTestType), reportParams[ReportConsts.ParamTestType]);
                
                for (var i = 0; i < rto.reports.Count; i++)
                {
                    var reportCode = i.ToString().PadLeft(codeLen, '0');
                    if (testList.Contains(reportCode))
                    {
                        var reportCommand = (CommonReportsCommand)rto.reports[i];
                        
                        if (reportCommand.CheckReportTemplate())
                        {
                            testCounter = 0;
                            reportCommand.paramBuilder = new ParamContainer();
                            reportCommand.scheme = scheme;
                            reportCommand.CreateReportParams();
                            reportCommand.reportServer = new ReportDataServer { scheme = scheme };
                            var drResult = dtResult.Rows.Add();
                            drResult[0] = String.Format("{0} ({1})", reportCommand.Caption, reportCommand.Key);
                            drResult[1] = TestSingleReport(reportCommand, new Collection<string>(), testType);
                            drResult[2] = testCounter;
                        }
                        else
                        {
                            var drResult = dtResult.Rows.Add();
                            drResult[0] = reportCommand.Key;
                            drResult[1] = "Шаблон отчета утрачен";
                        }
                    }
                }
            }
            return dtResult;
        }

        public virtual DataTable[] TestReports(Dictionary<string, string> reportParams)
        {
            var dtResult = new DataTable[2];
            // 1 таблица = проверка структуры базы
            dtResult[0] = TestStructure();
            // 2 таблица - проверка отчетов
            dtResult[1] = TestFill(reportParams);
            return dtResult;
        }
    }
}
