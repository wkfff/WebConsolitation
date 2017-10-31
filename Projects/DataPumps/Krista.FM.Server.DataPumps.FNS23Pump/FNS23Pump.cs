using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS23Pump
{
    // ФНС - 0023 - Форма 4-НОМ
    public class FNS23PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // ОКВЭД.ФНС (d_OKVED_FNS)
        private IDbDataAdapter daOkved;
        private DataSet dsOkved;
        private IClassifier clsOkved;
        private Dictionary<string, int> cacheOkved = null;
        private int nullOkved;
        // Доходы.Группы ФНС (d_D_GroupFNS)
        private IDbDataAdapter daIncomes;
        private DataSet dsIncomes;
        private IClassifier clsIncomes;
        private Dictionary<string, int> cacheIncomes = null;
        private int nullIncomes;
        // Задолженность.ФНС (d_Arrears_FNS)
        private IDbDataAdapter daArrears;
        private DataSet dsArrears;
        private IClassifier clsArrears;
        private Dictionary<string, int> cacheArrears = null;
        private int nullArrears;
        // Районы.ФНС (d_Regions_FNS)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;
        private int nullRegions;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_4 НОМ_Сводный (f_D_FNS4NOMTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_4 НОМ_Районы (f_D_FNS4NOMRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private List<string> strCodeList;
        private ReportType reportType;
        private decimal[] totalSums = null;
        private List<int[]> incomesRefsList = new List<int[]>(5);
        private List<int[]> arrearsRefsList = new List<int[]>(5);
        // номер столбца, в котором находится код строки
        private int strCodeColumnIndex = -1;
        // номер раздела (начинается с 0)
        private int sectionIndex = -1;
        // код строки (для отчётов по строкам)
        private string strCode = string.Empty;

        #endregion Поля

        #region Структуры, перечисления

        private enum ReportType
        {
            Svod,
            Str,
            Region
        }

        #endregion Структуры, перечисления

        #region Константы

        private string[] strCodes2005 = new string[] { "1010", "1015", "1020", "1025", "1030", "1035", "1040", "1045", "1050", "1055",
            "1060", "1065", "1070", "1075", "1080", "1085", "1090", "1095", "1100", "1105", "1110", "1115", "1120", "1125", "1130", "1135",
            "1140", "1145", "1150", "1155", "1160", "1165", "1170", "1175", "1180", "1190", "1195", "1200", "1205", "1210", "1220", "1230",
            "1240", "1250", "1251", "1260", "1270", "1280", "1290", "1300", "1310", "1320", "1330", "1340", "1350", "1360", "1370", "1380",
            "1400", "2010", "2015", "2020", "2025", "2030", "2035", "2040", "2045", "2050", "2055", "2060", "2065", "2070", "2075", "2080",
            "2085", "2090", "2095", "2100", "2105", "2110", "2115", "2120", "2125", "2130", "2135", "2140", "2145", "2150", "2155", "2160",
            "2165", "2170", "2175", "2180", "2190", "2195", "2200", "2205", "2210", "2220", "2230", "2240", "2250", "2251", "2260", "2270",
            "2280", "2290", "2300", "2310", "2320", "2330", "2340", "2350", "2360", "2370", "2380", "2400", "3010", "3015", "3020", "3025",
            "3030", "3035", "3040", "3045", "3050", "3055", "3060", "3065", "3070", "3075", "3080", "3085", "3090", "3095", "3100", "3105",
            "3110", "3115", "3120", "3125", "3130", "3135", "3140", "3145", "3150", "3155", "3160", "3165", "3170", "3175", "3180", "3190",
            "3195", "3200", "3205", "3210", "3220", "3230", "3240", "3250", "3251", "3260", "3270", "3280", "3290", "3300", "3310", "3320",
            "3330", "3340", "3350", "3360", "3370", "3380", "3400", "4010", "4015", "4025", "4030", "4035", "4040", "4045", "4050", "4055",
            "4060", "4065", "4070", "4075", "4080", "4085", "4090", "4095", "4100", "4105", "4110", "4115", "4120", "4125", "4130", "4135",
            "4140", "4145", "4150", "4155", "4160", "4165", "4170", "4175", "4180", "4190", "4195", "4200", "4205", "4210", "4220", "4230",
            "4240", "4250", "4251", "4260", "4270", "4280", "4290", "4300", "4310", "4320", "4330", "4340", "4350", "4360", "4370", "4380",
            "4400", "5010", "5015", "5020", "5025", "5030", "5035", "5040", "5045", "5050", "5055", "5060", "5065", "5070", "5075", "5080",
            "5085", "5090", "5095", "5100", "5105", "5110", "5115", "5120", "5125", "5130", "5135", "5140", "5145", "5150", "5155", "5160",
            "5165", "5170", "5175", "5180", "5190", "5195", "5200", "5205", "5210", "5220", "5230", "5240", "5250", "5251", "5260", "5270",
            "5280", "5290", "5300", "5310", "5320", "5330", "5340", "5350", "5360", "5370", "5380", "5400" };
        private string[] strCodes2007 = new string[] { "1010", "1015", "1020", "1025", "1030", "1035", "1040", "1045", "1050", "1055",
            "1060", "1065", "1070", "1075", "1080", "1085", "1090", "1095", "1100", "1105", "1110", "1115", "1120", "1125", "1130", "1135",
            "1140", "1145", "1150", "1155", "1160", "1165", "1170", "1175", "1180", "1185", "1190", "1195", "1200", "1205", "1210", "1215",
            "1220", "1225", "1240", "1245", "1250", "1255", "1270", "1280", "1285", "1290", "1300", "1305", "1315", "1320", "1325", "1330",
            "1340", "1345", "1350", "1355", "1370", "1375", "1380", "1390", "1400", "1410", "1420", "1430", "1440", "1450", "1500", "1510",
            "2010", "2015", "2020", "2025", "2030", "2035", "2040", "2045", "2050", "2055", "2060", "2065", "2070", "2075", "2080", "2085",
            "2090", "2095", "2100", "2105", "2110", "2115", "2120", "2125", "2130", "2135", "2140", "2145", "2150", "2155", "2160", "2165",
            "2170", "2175", "2180", "2185", "2190", "2195", "2200", "2205", "2210", "2215", "2220", "2225", "2240", "2245", "2250", "2255",
            "2270", "2280", "2285", "2290", "2300", "2305", "2315", "2320", "2325", "2330", "2340", "2345", "2350", "2355", "2370", "2375",
            "2380", "2390", "2400", "2410", "2420", "2430", "2440", "2450", "2500", "2510", "3010", "3015", "3020", "3025", "3030", "3035",
            "3040", "3045", "3050", "3055", "3060", "3065", "3070", "3075", "3080", "3085", "3090", "3095", "3100", "3105", "3110", "3115",
            "3120", "3125", "3130", "3135", "3140", "3145", "3150", "3155", "3160", "3165", "3170", "3175", "3180", "3185", "3190", "3195",
            "3200", "3205", "3210", "3215", "3220", "3225", "3240", "3245", "3250", "3255", "3270", "3280", "3285", "3290", "3300", "3305",
            "3315", "3320", "3325", "3330", "3340", "3345", "3350", "3355", "3370", "3375", "3380", "3390", "3400", "3410", "3420", "3430",
            "3440", "3450", "3500", "3510", "4010", "4015", "4020", "4025", "4030", "4035", "4040", "4045", "4050", "4055", "4060", "4065",
            "4070", "4075", "4080", "4085", "4090", "4095", "4100", "4105", "4110", "4115", "4120", "4125", "4130", "4135", "4140", "4145",
            "4150", "4155", "4160", "4165", "4170", "4175", "4180", "4185", "4190", "4195", "4200", "4205", "4210", "4215", "4220", "4225",
            "4240", "4245", "4250", "4255", "4270", "4280", "4285", "4290", "4300", "4305", "4315", "4320", "4325", "4330", "4340", "4345",
            "4350", "4355", "4370", "4375", "4380", "4390", "4400", "4410", "4420", "4430", "4440", "4450", "4500", "4510", "5010", "5015",
            "5020", "5025", "5030", "5035", "5040", "5045", "5050", "5055", "5060", "5065", "5070", "5075", "5080", "5085", "5090", "5095",
            "5100", "5105", "5110", "5115", "5120", "5125", "5130", "5135", "5140", "5145", "5150", "5155", "5160", "5165", "5170", "5175",
            "5180", "5185", "5190", "5195", "5200", "5205", "5210", "5215", "5220", "5225", "5240", "5245", "5250", "5255", "5270", "5280",
            "5285", "5290", "5300", "5305", "5315", "5320", "5325", "5330", "5340", "5345", "5350", "5355", "5370", "5375", "5380", "5390",
            "5400", "5410", "5420", "5430", "5440", "5450", "5500", "5510" };
        // наименования служебных каталогов
        private const string constSvodDirName = "Сводный";
        private const string constStrDirName = "Строки";
        private const string constRegDirName = "Районы";
        // сслылки на доходы.группы ФНС и задолженности: индекс столбца в отчете (начиная со столбца "1", так как кол во столбцов не фиксировано);
        // код доходов фнс (задолженность), на которое закачиваем
        // секция 1
        private int[] incomesRefsSection1year2005Const = new int[] {
            0, 0, 0, 0, 0, 0, 100000000, 101000000, 101010000, 102000000, 108000000, 103000000,
            103010000, 104000000, 200000000, 300000000, 400000000, 0, 0, 0, 0, 0, 0, 0 };
        private int[] incomesRefsSection1year2006Const = new int[] {
            0, 0, 0, 0, 0, 0, 100000000, 101000000, 101010000, 102000000, 108000000, 103000000,
            103010000, 104000000, 200000000, 300000000, 400000000, 0, 0, 0, 0, 0 };
        private int[] incomesRefsSection1year2009Const = new int[] {
            0, 0, 0, 0, 0, 0, 0, 0, 100000000, 101000000, 101010000, 102000000, 104010000, 103000000,
            103010000, 104000000, 200000000, 300000000, 400000000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private int[] arrearsRefsSection1year2005Const = new int[] {
            1010, 1020, 1037, 1038, 1039, 1040, 1020, 1020, 1020, 1020, 1020, 1020,
            1020, 1020, 1020, 1020, 1020, 1060, 1050, 1220, 1221, 1222, 1223, 1224 };
        private int[] arrearsRefsSection1year2006Const = new int[] {
            1010, 1020, 1037, 1038, 1039, 1040, 1020, 1020, 1020, 1020, 1020,
            1020, 1020, 1020, 1020, 1020, 1020, 1060, 1050, 1190, 1140, 1220 };
        private int[] arrearsRefsSection1year2009Const = new int[] {
            1010, 1015, 1020, 1025, 1037, 1038, 1039, 1040, 1020, 1020, 1020,
            1020, 1020, 1020, 1020, 1020, 1020, 1020, 1020, 1045, 1060, 1050,
            1190, 1140, 1220, 1250, 1260, 1270, 1280, 1290, 1300 };
        // секция 2
        private int[] incomesRefsSection2year2005Const = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private int[] incomesRefsSection2year2006Const = new int[] { 0, 0, 0, 0, 0, 0 };
        private int[] incomesRefsSection2year2009Const = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private int[] arrearsRefsSection2year2005Const = new int[] { 2010, 2011, 2012, 2013, 2014, 2030, 2020, 2190, 2191, 2192, 2193, 2194 };
        private int[] arrearsRefsSection2year2006Const = new int[] { 2010, 2030, 2020, 2160, 2110, 2190 };
        private int[] arrearsRefsSection2year2009Const = new int[] { 2010, 2015, 2030, 2020, 2160, 2110, 2190, 2215, 2216, 2217, 2218, 2219, 2230 };
        // секция 3
        private int[] incomesRefsSection3Const = new int[] {
            105000000, 105000000, 105010000, 105010000, 105020000, 105020000, 105030000, 105030000, 105040000, 105040000 };
        private int[] arrearsRefsSection3Const = new int[] {
            4000, 4005, 4010, 4210, 4010, 4210, 4010, 4210, 4010, 4210 };

        private int[] incomesRefsSection3Const_2011 = new int[] {
            105000000, 106000000, 107010000};
        private int[] arrearsRefsSection3Const_2011 = new int[] {
            4000, 5000, 6000};
        // секция 4
        private int[] incomesRefsSection4Const = new int[] {
            106000000, 106000000, 106010000, 106010000, 106010000, 106010000, 106020000, 106020000, 106020000,
            106020000, 106030000, 106030000, 106030000, 106030000, 106040000, 106040000, 106040000, 106040000 };
        private int[] arrearsRefsSection4Const = new int[] {
            5000, 5005, 5010, 5050, 5200, 5210, 5010, 5050, 5200,
            5210, 5010, 5050, 5200, 5210, 5010, 5050, 5200, 5210 };
        // секция 5
        private int[] incomesRefsSection5Const = new int[] { 107010000, 107010000, 107010100, 107010100, 107010200, 107010200 };
        private int[] arrearsRefsSection5Const = new int[] { 6000, 6005, 6010, 6190, 6010, 6190 };

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        private int[] GetClsRefs(int[] clsRefs, Dictionary<string, int> clsCache, int nullCls)
        {
            int count = clsRefs.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                clsRefs[i] = FindCachedRow(clsCache, clsRefs[i].ToString(), nullCls);
            }
            return clsRefs;
        }

        private void FillClsRefs()
        {
            if (this.DataSource.Year >= 2009)
            {
                incomesRefsList.Add(GetClsRefs((int[])incomesRefsSection1year2009Const.Clone(), cacheIncomes, nullIncomes));
                incomesRefsList.Add(GetClsRefs((int[])incomesRefsSection2year2009Const.Clone(), cacheIncomes, nullIncomes));
                arrearsRefsList.Add(GetClsRefs((int[])arrearsRefsSection1year2009Const.Clone(), cacheArrears, nullArrears));
                arrearsRefsList.Add(GetClsRefs((int[])arrearsRefsSection2year2009Const.Clone(), cacheArrears, nullArrears));
            }
            else if (this.DataSource.Year >= 2006)
            {
                incomesRefsList.Add(GetClsRefs((int[])incomesRefsSection1year2006Const.Clone(), cacheIncomes, nullIncomes));
                incomesRefsList.Add(GetClsRefs((int[])incomesRefsSection2year2006Const.Clone(), cacheIncomes, nullIncomes));
                arrearsRefsList.Add(GetClsRefs((int[])arrearsRefsSection1year2006Const.Clone(), cacheArrears, nullArrears));
                arrearsRefsList.Add(GetClsRefs((int[])arrearsRefsSection2year2006Const.Clone(), cacheArrears, nullArrears));
            }
            else
            {
                incomesRefsList.Add(GetClsRefs((int[])incomesRefsSection1year2005Const.Clone(), cacheIncomes, nullIncomes));
                incomesRefsList.Add(GetClsRefs((int[])incomesRefsSection2year2005Const.Clone(), cacheIncomes, nullIncomes));
                arrearsRefsList.Add(GetClsRefs((int[])arrearsRefsSection1year2005Const.Clone(), cacheArrears, nullArrears));
                arrearsRefsList.Add(GetClsRefs((int[])arrearsRefsSection2year2005Const.Clone(), cacheArrears, nullArrears));
            }
            if (this.DataSource.Year>=2011)
                incomesRefsList.Add(GetClsRefs((int[])incomesRefsSection3Const_2011.Clone(), cacheIncomes, nullIncomes));
            else incomesRefsList.Add(GetClsRefs((int[])incomesRefsSection3Const.Clone(), cacheIncomes, nullIncomes));

            incomesRefsList.Add(GetClsRefs((int[])incomesRefsSection4Const.Clone(), cacheIncomes, nullIncomes));
            incomesRefsList.Add(GetClsRefs((int[])incomesRefsSection5Const.Clone(), cacheIncomes, nullIncomes));
            if (this.DataSource.Year >= 2011)
                arrearsRefsList.Add(GetClsRefs((int[])arrearsRefsSection3Const_2011.Clone(), cacheArrears, nullArrears));
            else arrearsRefsList.Add(GetClsRefs((int[])arrearsRefsSection3Const.Clone(), cacheArrears, nullArrears));
            arrearsRefsList.Add(GetClsRefs((int[])arrearsRefsSection4Const.Clone(), cacheArrears, nullArrears));
            arrearsRefsList.Add(GetClsRefs((int[])arrearsRefsSection5Const.Clone(), cacheArrears, nullArrears));
        }

        protected override void QueryData()
        {
            InitClsDataSet(ref daOkved, ref dsOkved, clsOkved);
            nullOkved = clsOkved.UpdateFixedRows(this.DB, this.SourceID);
            int incomesSourceId = AddDataSource("ФНС", "0023", ParamKindTypes.YearMonth, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitDataSet(ref daIncomes, ref dsIncomes, clsIncomes, true, string.Format("SOURCEID = {0}", incomesSourceId), string.Empty);
            nullIncomes = clsIncomes.UpdateFixedRows(this.DB, this.SourceID);
            int arrearsSourceId = AddDataSource("ФНС", "0023", ParamKindTypes.YearMonth, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitDataSet(ref daArrears, ref dsArrears, clsArrears, true, string.Format("SOURCEID = {0}", arrearsSourceId), string.Empty);
            nullArrears = clsArrears.UpdateFixedRows(this.DB, this.SourceID);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
            InitFactDataSet(ref daIncomesTotal, ref dsIncomesTotal, fctIncomesTotal);
            InitFactDataSet(ref daIncomesRegion, ref dsIncomesRegion, fctIncomesRegion);
            FillCaches();
            FillClsRefs();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheOkved, dsOkved.Tables[0], "RowCode", "ID");
            FillRowsCache(ref cacheIncomes, dsIncomes.Tables[0], "Code", "ID");
            FillRowsCache(ref cacheArrears, dsArrears.Tables[0], "Code", "ID");
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], new string[] { "Code", "Name" }, "|", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOkved, dsOkved, clsOkved);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        private const string D_D_GROUP_FNS_GUID = "b9169eb6-de81-420b-8a2b-05ffa2fd35c1";
        private const string D_ARREARS_FNS_GUID = "516ec293-bf4c-4ff8-a2c5-bc04acb70a81";
        private const string D_OKVED_FNS_GUID = "9f549d45-9e27-4c0a-948e-b99294de79bf";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string F_D_FNS_4NOM_TOTAL_GUID = "63eb10c5-1626-4d46-a377-8ac2f2c24902";
        private const string F_D_FNS_4NOM_REGIONS_GUID = "e62bcbc0-3f8d-4e0c-93bd-da6ae7bf5753";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsOkved = this.Scheme.Classifiers[D_OKVED_FNS_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };

            // нужно сопоставлять доходы.фнс, задолженность.фнс - источник год, месяц = 0
            this.AssociateClassifiersEx = new IClassifier[] {
                clsIncomes = this.Scheme.Classifiers[D_D_GROUP_FNS_GUID],
                clsArrears = this.Scheme.Classifiers[D_ARREARS_FNS_GUID] };

            this.UsedFacts = new IFactTable[] {
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_4NOM_TOTAL_GUID],
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_4NOM_REGIONS_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesTotal);
            ClearDataSet(ref dsIncomesRegion);
            ClearDataSet(ref dsOkved);
            ClearDataSet(ref dsIncomes);
            ClearDataSet(ref dsArrears);
            ClearDataSet(ref dsRegions);
            incomesRefsList.Clear();
            arrearsRefsList.Clear();
        }

        #endregion Работа с базой и кэшами

        #region Общие функции

        private void CheckOkved()
        {
            // для отчетов в разрезе районов и строк классификатор ОКВЭД.фнс должен быть заполнен
            if ((reportType != ReportType.Svod) && (cacheOkved.Count <= 1))
                throw new Exception("Не заполнен классификатор ОКВЭД.ФНС - закачайте сводные отчеты");
        }

        private void CheckIncomes()
        {
            // должен быть заполнен классификатор Доходы.группы ФНС
            if (cacheIncomes.Count <= 1)
                throw new Exception("Не заполнен классификатор 'Доходы.Группы ФНС'. Данные по этому источнику закачаны не будут.");
        }

        private void CheckArrears()
        {
            // должен быть заполнен классификатор задолженность.ФНС
            if (cacheArrears.Count <= 1)
                throw new Exception("Не заполнен классификатор 'Задолженность.ФНС'. Данные по этому источнику закачаны не будут.");
        }

        private void CheckDirectories(DirectoryInfo dir)
        {
            DirectoryInfo[] svod = dir.GetDirectories(constSvodDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] str = dir.GetDirectories(constStrDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] reg = dir.GetDirectories(constRegDirName, SearchOption.TopDirectoryOnly);
            // Каталог "Сводный" должен присутствовать
            if (svod.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(constSvodDirName);
                throw new Exception(string.Format("Отсутствует каталог \"{0}\"", constSvodDirName));
            }
            if (str.GetLength(0) == 0)
                dir.CreateSubdirectory(constStrDirName);
            if (reg.GetLength(0) == 0)
                dir.CreateSubdirectory(constRegDirName);
            // Каталоги Строки и Районы для одного месяца не могут быть заполнены одновременно
            if ((str.GetLength(0) > 0 && str[0].GetFiles().GetLength(0) > 0) &&
                (reg.GetLength(0) > 0 && reg[0].GetFiles().GetLength(0) > 0))
                throw new Exception("Каталоги \"Строки\" и \"Районы\" для одного месяца не могут быть заполнены одновременно");
        }

        private decimal CleanFactValue(string factValue)
        {
            factValue = factValue.Trim().ToUpper().Trim('X').Trim('Х').PadLeft(1, '0');
            return Convert.ToDecimal(factValue);
        }

        // обнуление итоговой суммы
        private void SetNullTotalSum()
        {
            int sumsCount = totalSums.GetLength(0);
            for (int i = 0; i < sumsCount; i++)
            {
                totalSums[i] = 0;
            }
        }

        // проверка контрольной суммы
        private void CheckTotalSum(decimal totalSum, decimal controlSum, string comment)
        {
            if (totalSum != controlSum)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Контрольная сумма {0:F} не сходится с итоговой {1:F} {2}",
                    controlSum, totalSum, comment));
            }
        }

        private void CheckXlsTotalSum(ExcelHelper excelDoc, int curRow)
        {
            int[] incomesRefs = null;
            int[] arrearsRefs = null;
            GetClsRefsByStrCode(ref incomesRefs, ref arrearsRefs);
            int columnsCount = incomesRefs.GetLength(0);
            int columnOffset = GetStrCodeColumn() + 1;
            for (int sumIndex = 0; sumIndex < columnsCount; sumIndex++)
            {
                string strValue = excelDoc.GetValue(curRow, sumIndex + columnOffset).Trim();
                decimal controlSum = CleanFactValue(strValue);
                CheckTotalSum(totalSums[sumIndex], controlSum, string.Format("(столбец {0})", sumIndex + columnOffset));
            }
        }

        private void SetClsHierarchy()
        {
            string d_OKVED_FNS23_HierarchyFileName = string.Empty;
            if (this.DataSource.Year >= 2009)
                d_OKVED_FNS23_HierarchyFileName = const_d_OKVED_FNS23_HierarchyFile2009;
            else if (this.DataSource.Year >= 2007)
                d_OKVED_FNS23_HierarchyFileName = const_d_OKVED_FNS23_HierarchyFile2007;
            else
                d_OKVED_FNS23_HierarchyFileName = const_d_OKVED_FNS23_HierarchyFile2005;
            SetClsHierarchy(clsOkved, ref dsOkved, "RowCode", d_OKVED_FNS23_HierarchyFileName, ClsHierarchyMode.Special);
        }

        private void SetStrCodeList()
        {
            strCodeList = new List<string>();
            if (this.DataSource.Year >= 2007)
                strCodeList.AddRange(strCodes2007);
            else
                strCodeList.AddRange(strCodes2005);
        }

        private void ShowAbsentStrCodes()
        {
            if (strCodeList.Count == 0)
                return;
            string codes = string.Empty;
            foreach (string code in strCodeList)
                codes += code + ", ";
            codes = codes.Remove(codes.Length - 2);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
              string.Format("Отсутствует файл в разрезе районов по кодам строк ({0})", codes));
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            if (reportType == ReportType.Str)
            {
                try
                {
                    // должны быть представлены все коды по строкам разделов,
                    // заполняем список кодов и проверяем на их присутствие в файлах
                    SetStrCodeList();
                    if (dir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0) == 0)
                        strCodeList.Clear();
                    ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                    // оставшиеся коды в списке отсутствовали в файлах, предупреждаем
                    ShowAbsentStrCodes();
                }
                finally
                {
                    strCodeList.Clear();
                }
            }
            else
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            }
        }

        private void PumpFiles(DirectoryInfo dir)
        {
            reportType = ReportType.Svod;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных сводных отчетов.");
            ProcessAllFiles(dir.GetDirectories(constSvodDirName)[0]);
            reportType = ReportType.Region;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе районов.");
            ProcessAllFiles(dir.GetDirectories(constRegDirName)[0]);
            reportType = ReportType.Str;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе строк.");
            ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);
        }

        #endregion Общие функции

        #region Работа с Excel

        private int GetStrCodeColumn()
        {
            if (reportType == ReportType.Svod)
                return strCodeColumnIndex;
            return 2;
        }

        private int GetSectionIndexByStrCode(string strCode)
        {
            return (Convert.ToInt32(strCode.Trim().PadLeft(1, '0')) / 1000 - 1);
        }

        private int PumpOkvedRow(ExcelHelper excelDoc, int curRow)
        {
            string okvedName = excelDoc.GetValue(curRow, 1).Trim();
            string okvedCode = excelDoc.GetValue(curRow, 2).Trim();
            string rowCode = excelDoc.GetValue(curRow, GetStrCodeColumn()).Trim();
            object[] mapping = new object[] { "NAME", okvedName, "CodeStr", okvedCode, "RowCode", rowCode };
            return PumpCachedRow(cacheOkved, dsOkved.Tables[0], clsOkved, mapping, rowCode, "ID");
        }

        private int PumpRegionsRow(ExcelHelper excelDoc, int curRow)
        {
            string regionName = excelDoc.GetValue(curRow, 1).Trim();
            string regionCode = excelDoc.GetValue(curRow, 2).Trim();
            // есть такие хуевые районы со строковым кодом - меняем код на ноль
            if (CommonRoutines.TrimNumbers(regionCode) != string.Empty)
                regionCode = "0";
            object[] mapping = new object[] { "NAME", regionName, "CODE", regionCode };
            string regKey = string.Format("{0}|{1}", regionCode, regionName);
            return PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, mapping, regKey, "ID");
        }

        private void GetClsRefsByStrCode(ref int[] incomesRefs, ref int[] arrearsRefs)
        {
            incomesRefs = incomesRefsList[sectionIndex];
            arrearsRefs = arrearsRefsList[sectionIndex];
        }

        private void PumpFactRow(decimal valueReport, int refDate, int refOkved, int refRegions,
            int refIncomes, int refArrears, int sumIndex)
        {
            if (valueReport == 0)
                return;

            totalSums[sumIndex] += valueReport;
            valueReport *= 1000;

            if (reportType == ReportType.Svod)
            {
                object[] mapping = new object[] {
                    "ValueReport", valueReport, "RefOKVED", refOkved, "RefD", refIncomes,
                    "RefArrears", refArrears, "RefYearDayUNV", refDate };
                PumpRow(dsIncomesTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] {
                    "ValueReport", valueReport, "RefOKVED", refOkved, "RefD", refIncomes,
                    "RefArrears", refArrears, "RefYearDayUNV", refDate, "RefRegions", refRegions };
                PumpRow(dsIncomesRegion.Tables[0], mapping);
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                }
            }
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate)
        {
            string cellValue = excelDoc.GetValue(curRow, 3).Trim();
            if (cellValue == string.Empty)
                return;

            int refOkved = nullOkved;
            if (reportType == ReportType.Svod)
            {
                refOkved = PumpOkvedRow(excelDoc, curRow);
            }

            int refRegions = nullRegions;
            if (reportType == ReportType.Str)
            {
                refRegions = PumpRegionsRow(excelDoc, curRow);
                refOkved = FindCachedRow(cacheOkved, strCode, nullOkved);
            }

            int[] incomesRefs = null;
            int[] arrearsRefs = null;
            GetClsRefsByStrCode(ref incomesRefs, ref arrearsRefs);
            int columnsCount = incomesRefs.GetLength(0);
            int columnOffset = GetStrCodeColumn() + 1;
            for (int sumIndex = 0; sumIndex < columnsCount; sumIndex++)
            {
                string strValue = excelDoc.GetValue(curRow, sumIndex + columnOffset).Trim();
                decimal valueReport = CleanFactValue(strValue);
                PumpFactRow(valueReport, refDate, refOkved, refRegions, incomesRefs[sumIndex], arrearsRefs[sumIndex], sumIndex);
            }
        }

        #region Отчеты в разрезе строк

        private string GetStrCode(ExcelHelper excelDoc, int curRow)
        {
            if (this.DataSource.Year >= 2006)
            {
                string cellValue = excelDoc.GetValue(curRow + 1, 1).Trim();
                if (cellValue.ToUpper().Contains(CONTROL_SUM_ROW))
                    return string.Empty;
                return cellValue.Split('-')[0].Trim();
            }
            else
            {
                string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                if (cellValue.ToUpper().Contains(CONTROL_SUM_ROW))
                    return string.Empty;
                return cellValue.Split('-')[1].Trim();
            }
        }

        private const string STR_CUT_ROW = "РАЗРЕЗ ПО СТРОКЕ";
        private bool IsStrCode(string cellValue)
        {
            if (reportType == ReportType.Str)
                return (cellValue.StartsWith(STR_CUT_ROW));
            return false;
        }

        #endregion Отчеты в разрезе строк

        private const string CONTROL_SUM_ROW = "КОНТРОЛЬНАЯ СУММА";
        private const string TOTAL_ROW = "ВСЕГО";
        private bool IsSectionEnd(string cellValue)
        {
            if (sectionIndex == -1)
                return false;
            if (reportType == ReportType.Svod)
                return (cellValue.StartsWith(CONTROL_SUM_ROW));
            return (cellValue.StartsWith(TOTAL_ROW));
        }

        private const string TABLE_TITLE_TEXT_RUS = "А";
        private const string TABLE_TITLE_TEXT_LAT = "A";
        private bool IsSectionStart(string cellValue)
        {
            return ((string.Compare(cellValue, TABLE_TITLE_TEXT_RUS, true) == 0) ||
                (string.Compare(cellValue, TABLE_TITLE_TEXT_LAT, true) == 0));
        }

        private const string STRCODE_TITLE = "В";
        // так как в сводных отчетах иногда может два столбца Б (пиздец, а нахуя?), ищем индекс колонки кода строки
        private void SetStrCodeColumnIndex(ExcelHelper excelDoc, int curRow)
        {
            if (reportType != ReportType.Svod)
                return;
            for (strCodeColumnIndex = 1; ; strCodeColumnIndex++)
                if (excelDoc.GetValue(curRow, strCodeColumnIndex).Trim().ToUpper() == STRCODE_TITLE)
                    return;
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate)
        {
            bool toPumpRow = false;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
                    if (cellValue == string.Empty)
                        continue;

                    if (IsStrCode(cellValue))
                    {
                        sectionIndex = -1;
                        strCode = GetStrCode(excelDoc, curRow);
                        if ((strCode == string.Empty) || !strCodeList.Contains(strCode))
                            continue;
                        strCodeList.Remove(strCode);
                        sectionIndex = GetSectionIndexByStrCode(strCode);
                    }

                    if (IsSectionEnd(cellValue))
                    {
                        toPumpRow = false;
                        CheckXlsTotalSum(excelDoc, curRow);
                        strCode = string.Empty;
                    }

                    if (toPumpRow && (sectionIndex != -1))
                    {
                        PumpXlsRow(excelDoc, curRow, refDate);
                        continue;
                    }

                    if (IsSectionStart(cellValue))
                    {
                        toPumpRow = true;
                        totalSums = new decimal[32];
                        SetNullTotalSum();
                        SetStrCodeColumnIndex(excelDoc, curRow);
                        if (reportType != ReportType.Str)
                        {
                            string strCode = excelDoc.GetValue(curRow + 1, GetStrCodeColumn());
                            sectionIndex = GetSectionIndexByStrCode(strCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа {1} отчета {2} возникла ошибка ({3})",
                        curRow, excelDoc.GetWorksheetName(), fileName, ex.Message), ex);
                }
        }

        private const string DATE_CELL_TEXT = "по состоянию на";
        private int GetXlsDateRef(ExcelHelper excelDoc)
        {
            int dateRef = -1;
            // пытаемся найти дату в диапазоне ячеек A4..A14
            int curCol = 1;
            for (int curRow = 4; curRow <= 14; curRow++)
            {
                string cellText = excelDoc.GetValue(curRow, curCol).Trim();
                if (cellText.ToUpper().Contains(DATE_CELL_TEXT.ToUpper()))
                {
                    dateRef = CommonRoutines.ShortDateToNewDate(CommonRoutines.TrimLetters(cellText));
                    dateRef = CommonRoutines.DecrementDate(dateRef);
                    break;
                }
            }
            if (dateRef == -1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Concat(
                    "Не удалось найти дату отчета в диапазоне ячеек А4..А14 или ",
                    "неправильно задана подстрока для поиска даты 'по состоянию на', ",
                    "дата будет определена параметрами источника."));
                dateRef = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            }
            CheckDataSourceByDate(dateRef, true);
            return dateRef;
        }

        private void PumpXlsFile(FileInfo file)
        {
            CheckOkved();
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.OpenDocument(file.FullName);
                int refDate = GetXlsDateRef(excelDoc);
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    PumpXlsSheetData(file.Name, excelDoc, refDate);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            CheckIncomes();
            CheckArrears();
            CheckDirectories(dir);
            if (dir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0) == 0)
                return;
            PumpFiles(dir);
            UpdateData();
            SetClsHierarchy();
            UpdateData();
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        protected override void ProcessDataSource()
        {
            SetClsHierarchy();
            UpdateData();
            // добавляем родительские записи по иерархии задолженности (иначе корректировка будет неправильная)
            AddParentRecords(fctIncomesTotal, dsArrears.Tables[0], "RefArrears", dsIncomes.Tables[0], "RefD",
                dsOkved.Tables[0], "RefOKVED", "ValueReport");
            // добавляем родительские записи по иерархии доходов (иначе корректировка будет неправильная)
            AddParentRecords(fctIncomesTotal, dsIncomes.Tables[0], "RefD", dsArrears.Tables[0], "RefArrears",
                dsOkved.Tables[0], "RefOKVED", "ValueReport");
            // корректировка сумм
            F4NMSumCorrectionConfig f4nomSumCorrectionConfig = new F4NMSumCorrectionConfig();
            f4nomSumCorrectionConfig.ValueField = "Value";
            f4nomSumCorrectionConfig.ValueReportField = "ValueReport";

            // корректировка сводной таблицы
            CorrectFactTableSums(fctIncomesTotal, dsArrears.Tables[0], clsArrears, "RefArrears",
                f4nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefOKVED", "RefD", "RefYearDayUNV" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctIncomesTotal, dsIncomes.Tables[0], clsIncomes, "RefD",
                f4nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefOKVED", "RefArrears", "RefYearDayUNV" }, string.Empty, string.Empty, false);
            CorrectFactTableSums(fctIncomesTotal, dsOkved.Tables[0], clsOkved, "RefOKVED",
                f4nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefD", "RefArrears", "RefYearDayUNV" }, string.Empty, string.Empty, false);
            // корректировка таблицы районов
            CorrectFactTableSums(fctIncomesRegion, dsArrears.Tables[0], clsArrears, "RefArrears",
                f4nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefOKVED", "RefD", "RefYearDayUNV" }, "RefRegions", string.Empty, true);
            CorrectFactTableSums(fctIncomesRegion, dsIncomes.Tables[0], clsIncomes, "RefD",
                f4nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefOKVED", "RefArrears", "RefYearDayUNV" }, "RefRegions", string.Empty, false);
            CorrectFactTableSums(fctIncomesRegion, dsOkved.Tables[0], clsOkved, "RefOKVED",
                f4nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefD", "RefArrears", "RefYearDayUNV" }, "RefRegions", string.Empty, false);
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            int year = -1;
            int month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Коррекции сумм фактов по данным источника");
        }

        #endregion Обработка данных

        #region Сопоставление

        protected override int GetClsSourceID(int sourceID)
        {
            if (sourceID <= 0)
                return -1;
            IDataSource ds = this.Scheme.DataSourceManager.DataSources[sourceID];
            IDataSource clsDs = FindDataSource(ParamKindTypes.YearMonth, ds.SupplierCode, ds.DataCode, string.Empty, ds.Year, 0, string.Empty, 0, string.Empty);
            if (clsDs == null)
                return -1;
            return clsDs.ID;
        }

        #endregion Сопоставление

    }
}
