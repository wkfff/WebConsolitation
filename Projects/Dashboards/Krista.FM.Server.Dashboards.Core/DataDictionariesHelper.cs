using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.Core
{
    /// <summary>
    /// ���������������� �����, ��������������� ������ ������ �� ��������� ������
    /// </summary>
    public static class DataDictionariesHelper
    {
        #region ������ ��������

        /// <summary>
        /// ������ ��������
        /// </summary>
        private static Dictionary<string, string> budgetLevelsTypes;

        /// <summary>
        /// ���������� �������� ������� ��������
        /// </summary>
        public static Dictionary<string, string> BudgetLevelTypes
        {
            get
            {
                // ���� ������� ������
                if (budgetLevelsTypes == null || budgetLevelsTypes.Count == 0)
                {
                    // ��������� ���
                    FillBudgetLevelsTypes();
                }
                return budgetLevelsTypes;
            }
        }

        private static void FillBudgetLevelsTypes()
        {
            budgetLevelsTypes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("budgetLevels", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������ �������", dt);

            // ���� �� ����� ���� � ������������, �� �������� ������ �������
            foreach (DataRow row in dt.Rows)
            {
                budgetLevelsTypes.Add(row[0].ToString(), row[1].ToString());
            }
        }

        #endregion 

        #region ������� ���

        /// <summary>
        /// ������� ���
        /// </summary>
        private static Dictionary<string, string> outcomesFKRTypes;
        private static Dictionary<string, string> outcomesFKRLevels;

        /// <summary>
        /// ���������� ������� ������������ �������� ���
        /// </summary>
        public static Dictionary<string, string> OutcomesFKRTypes
        {
            get
            {
                // ���� ������� ������
                if (outcomesFKRTypes == null || outcomesFKRTypes.Count == 0)
                {
                    // ��������� ���
                    FillOutcomesFKR();
                }
                return outcomesFKRTypes;
            }
        }

        /// <summary>
        /// ���������� ������� ������� �������� ���
        /// </summary>
        public static Dictionary<string, string> OutcomesFKRLevels
        {
            get
            {
                // ���� ������� ������
                if (outcomesFKRLevels == null || outcomesFKRLevels.Count == 0)
                {
                    // ��������� ���
                    FillOutcomesFKR();
                }
                return outcomesFKRLevels;
            }
        }

        private static void FillOutcomesFKR()
        {
            outcomesFKRTypes = new Dictionary<string, string>();
            outcomesFKRLevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("outcomesFKR", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "������� ����", dt);

            foreach (DataRow row in dt.Rows)
            {
                outcomesFKRTypes.Add(row[0].ToString(), row[1].ToString());
                outcomesFKRLevels.Add(row[0].ToString(), row[2].ToString());
            }
        }

        #endregion 

        #region ������� ��� ��� ��

        /// <summary>
        /// ������� ���
        /// </summary>
        private static Dictionary<string, string> outcomesFOFKRTypes;
        private static Dictionary<string, string> outcomesFOFKRLevels;

        /// <summary>
        /// ���������� ������� ������������ �������� ��� ��� ��
        /// </summary>
        public static Dictionary<string, string> OutcomesFOFKRTypes
        {
            get
            {
                // ���� ������� ������
                if (outcomesFOFKRTypes == null || outcomesFOFKRTypes.Count == 0)
                {
                    // ��������� ���
                    FillOutcomesFOFKR();
                }
                return outcomesFOFKRTypes;
            }
        }

        /// <summary>
        /// ���������� ������� ������� �������� ���
        /// </summary>
        public static Dictionary<string, string> OutcomesFOFKRLevels
        {
            get
            {
                // ���� ������� ������
                if (outcomesFOFKRLevels == null || outcomesFOFKRLevels.Count == 0)
                {
                    // ��������� ���
                    FillOutcomesFOFKR();
                }
                return outcomesFOFKRLevels;
            }
        }

        private static void FillOutcomesFOFKR()
        {
            outcomesFOFKRTypes = new Dictionary<string, string>();
            outcomesFOFKRLevels = new Dictionary<string, string>();

            CustomParam.CustomParamFactory("fkr_dimension").Value = RegionSettingsHelper.Instance.FKRDimension;
            CustomParam.CustomParamFactory("fkr_all_level").Value = RegionSettingsHelper.Instance.FKRAllLevel;

            CustomParam.CustomParamFactory("rzpr_internal_circulation_extruding").Value = RegionSettingsHelper.Instance.RzPrInternalCircualtionExtruding;

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("outcomesFOFKR", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������� ����", dt);

            int multiplyValueCount = 4;
            foreach (DataRow row in dt.Rows)
            {
                string type = row[1].ToString();
                string level = row[2].ToString();

                type = RegionsNamingHelper.CheckMultiplyValue(type, multiplyValueCount);
                level = RegionsNamingHelper.CheckMultiplyValue(level, multiplyValueCount);

                outcomesFOFKRTypes.Add(GetDictionaryUniqueKey(outcomesFOFKRTypes, row[0].ToString()), type);
                outcomesFOFKRLevels.Add(GetDictionaryUniqueKey(outcomesFOFKRLevels, row[0].ToString()), level);
            }
        }

        #endregion 

        #region ������� �����

        /// <summary>
        /// ������� �����
        /// </summary>
        private static Dictionary<string, string> outcomesKOSGUTypes;
        private static Dictionary<string, string> outcomesKOSGULevels;

        /// <summary>
        /// ���������� ������� ������������ �������� �����
        /// </summary>
        public static Dictionary<string, string> OutcomesKOSGUTypes
        {
            get
            {
                // ���� ������� ������
                if (outcomesKOSGUTypes == null || outcomesKOSGUTypes.Count == 0)
                {
                    // ��������� ���
                    FillOutcomesKOSGU();
                }
                return outcomesKOSGUTypes;
            }
        }

        /// <summary>
        /// ���������� ������� ������� �������� �����
        /// </summary>
        public static Dictionary<string, string> OutcomesKOSGULevels
        {
            get
            {
                // ���� ������� ������
                if (outcomesKOSGULevels == null || outcomesFKRLevels.Count == 0)
                {
                    // ��������� ���
                    FillOutcomesKOSGU();
                }
                return outcomesKOSGULevels;
            }
        }

        private static void FillOutcomesKOSGU()
        {
            outcomesKOSGUTypes = new Dictionary<string, string>();
            outcomesKOSGULevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("outcomesKOSGU", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "������� �����", dt);

            foreach (DataRow row in dt.Rows)
            {
                outcomesKOSGUTypes.Add(row[0].ToString(), row[1].ToString());
                outcomesKOSGULevels.Add(row[0].ToString(), row[2].ToString());
            }
        }

        #endregion 

        #region ������� ������������ ����

        /// <summary>
        /// ������� ������������ ����
        /// </summary>
        private static Dictionary<string, string> shortGRBSNames;
        /// <summary>
        /// ���� ����
        /// </summary>
        private static Dictionary<string, string> shortGRBSCodes;

        /// <summary>
        /// ���������� ������� ������� ������������ ����
        /// </summary>
        public static Dictionary<string, string> ShortGRBSNames
        {
            get
            {
                // ���� ������� ������
                if (shortGRBSNames == null || shortGRBSNames.Count == 0)
                {
                    // ��������� ���
                    FillShortGRBSNames();
                }
                return shortGRBSNames;
            }
        }

        /// <summary>
        /// ���������� ������� ������� ������������ ����
        /// </summary>
        public static Dictionary<string, string> ShortGRBSCodes
        {
            get
            {
                // ���� ������� ������
                if (shortGRBSCodes == null || shortGRBSCodes.Count == 0)
                {
                    // ��������� ���
                    FillShortGRBSNames();
                }
                return shortGRBSCodes;
            }
        }

        private static void FillShortGRBSNames()
        {
            shortGRBSNames = new Dictionary<string, string>();
            shortGRBSCodes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("shortGRBSNames", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "������� ������������ ����", dt);

            foreach (DataRow row in dt.Rows)
            {
                shortGRBSNames.Add(row[0].ToString(), row[1].ToString());
                shortGRBSCodes.Add(row[1].ToString(), row[2].ToString());
            }
        }

        public static string GetGRBSCodeByShortName(string shortName)
        {
            if (ShortGRBSCodes.ContainsKey(shortName))
            {
                return ShortGRBSCodes[shortName];
            }
            return shortName;
        }

        public static string GetGRBSCodeByFullName(string fullName)
        {
            string shortName = GetShortGRBSName(fullName);
            return GetGRBSCodeByShortName(shortName);
        }

        /// <summary>
        /// ��������� �������� ����� ���� �� �������
        /// </summary>
        /// <param name="fullName">������ ���</param>
        /// <returns>������� ���</returns>
        public static string GetShortGRBSName(string fullName)
        {
            if (ShortGRBSNames.ContainsKey(fullName))
            {
                return ShortGRBSNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// ��������� ������� ����� ���� �� ��������
        /// </summary>
        /// <param name="shortName">������� ���</param>
        /// <returns>������ ���</returns>
        public static string GetFullGRBSName(string shortName)
        {
            if (ShortGRBSNames.ContainsValue(shortName))
            {
                foreach (string key in ShortGRBSNames.Keys)
                {
                    if (ShortGRBSNames[key] == shortName)
                    {
                        return key;
                    }
                }
            }
            return shortName;
        }

        #endregion 

        #region �������� ��� ��������� �����

        /// <summary>
        /// �������� ��� ��������� �����
        /// </summary>
        private static Dictionary<string, string> cashPlanNonEmptyDays;

        /// <summary>
        /// ���������� ������� �������� ���� ��������� �����
        /// </summary>
        public static Dictionary<string, string> CashPlanNonEmptyDays
        {
            get
            {
                // ���� ������� ������
                if (cashPlanNonEmptyDays == null || cashPlanNonEmptyDays.Count == 0)
                {
                    // ��������� ���
                    FillCashPlanNonEmptyDays();
                }
                return cashPlanNonEmptyDays;
            }
        }

        private static void FillCashPlanNonEmptyDays()
        {
            cashPlanNonEmptyDays = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("cashPlanNonEmptyDays", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    cashPlanNonEmptyDays.Add(GetDictionaryUniqueKey(cashPlanNonEmptyDays, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    cashPlanNonEmptyDays.Add(GetDictionaryUniqueKey(cashPlanNonEmptyDays, curMonth), "Month");
                    month = curMonth;
                }

                string day = GetDictionaryUniqueKey(cashPlanNonEmptyDays, row[4].ToString());
                cashPlanNonEmptyDays.Add(day, "Day");
            }
        }

        private static string GetDictionaryUniqueKey(Dictionary<string, string> dictionary, string key)
        {
            string newKey = key;
            while (dictionary.ContainsKey(newKey))
            {
                newKey += " ";
            }
            return newKey;
        }

        #endregion 

        #region ������� ������������ �����

        /// <summary>
        /// ������� ������������ ����
        /// </summary>
        private static Dictionary<string, string> shortKOSGUNames;

        /// <summary>
        /// ���������� ������� ������� ������������ �����
        /// </summary>
        public static Dictionary<string, string> ShortKOSGUNames
        {
            get
            {
                // ���� ������� ������
                if (shortKOSGUNames == null || shortKOSGUNames.Count == 0)
                {
                    // ��������� ���
                    FillShortKOSGUNames();
                }
                return shortKOSGUNames;
            }
        }

        private static void FillShortKOSGUNames()
        {
            shortKOSGUNames = new Dictionary<string, string>();
            shortKOSGUNames.Add("���������� �����", "���������� �����");
            shortKOSGUNames.Add("������ ������� �� ���������� �����", "������ �������");
            shortKOSGUNames.Add("���������� �� ������� �� ������ �����", "���������� �� ��������");
            shortKOSGUNames.Add("������ �����", "������ �����");
            shortKOSGUNames.Add("������������ ������", "������������ ������");
            shortKOSGUNames.Add("�������� ����� �� ����������� ����������", "������ �� ���������");
            shortKOSGUNames.Add("������, ������ �� ���������� ���������", "������, ������ �� ������. ���������");
            shortKOSGUNames.Add("������ ������, ������", "������ ������, ������");
            shortKOSGUNames.Add("������������ ���������������� (��������������) �����", "������������ ���. (���) �����");
            shortKOSGUNames.Add("������������� ������������ ������������", "������������� ������������ ���-��");
            shortKOSGUNames.Add("���������� �����������", "���������� �����������");
            shortKOSGUNames.Add("������ �������", "������ �������");
            shortKOSGUNames.Add("���������� ��������� �������� �������", "���������� ��-�� ���. ��-�");
            shortKOSGUNames.Add("���������� ��������� �������������� �������", "���������� ��-�� �����. �������");
            shortKOSGUNames.Add("���������� ��������� ������������ �������", "���������� ��-�� ���. �������");
            shortKOSGUNames.Add("���������� ��������� ����� � ���� ���� ������� � ��������", "���������� ��-�� �����");
            shortKOSGUNames.Add("���������� ��������� ��������������� �������", "���������� ��-�� ��������. �������");
        }

        /// <summary>
        /// ��������� �������� ����� ����� �� �������
        /// </summary>
        /// <param name="fullName">������ ���</param>
        /// <returns>������� ���</returns>
        public static string GetShortKOSGUName(string fullName)
        {
            if (ShortKOSGUNames.ContainsKey(fullName))
            {
                return ShortKOSGUNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// ��������� ������� ����� ����� �� ��������
        /// </summary>
        /// <param name="shortName">������� ���</param>
        /// <returns>������ ���</returns>
        public static string GetFullKOSGUName(string shortName)
        {
            if (ShortKOSGUNames.ContainsValue(shortName))
            {
                foreach (string key in ShortKOSGUNames.Keys)
                {
                    if (ShortKOSGUNames[key] == shortName)
                    {
                        return key;
                    }
                }
            }
            return shortName;
        }

        #endregion 

        #region ������� ������������ �������� � ������� �������

        /// <summary>
        /// ������� ������������ �������� � ������� �������
        /// </summary>
        private static Dictionary<string, string> shortBugetNames;

        /// <summary>
        /// ���������� ������� ������� ������������ �������� � ������� �������
        /// </summary>
        public static Dictionary<string, string> ShortBugetNames
        {
            get
            {
                // ���� ������� ������
                if (shortBugetNames == null || shortBugetNames.Count == 0)
                {
                    // ��������� ���
                    FillShortBugetNames();
                }
                return shortBugetNames;
            }
        }

        private static void FillShortBugetNames()
        {
            shortBugetNames = new Dictionary<string, string>();
            shortBugetNames.Add("��������� � ����������� ������", "��������� � ����������� ������");
            shortBugetNames.Add("������ �� �������, ������", "������ �� �������, ������");
            shortBugetNames.Add("������ �� ������ (������, ������), ����������� �� ���������� ���������� ���������", "������ �� ������ (������, ������)");
            shortBugetNames.Add("������ �� ���������� �����", "������ �� ���������");
            shortBugetNames.Add("������ �� ���������", "������ �� ���������");
            shortBugetNames.Add("������, ����� � ���������� ������� �� ����������� ���������� ���������", "������ � ����� �� �����. ���������� ���������");
            shortBugetNames.Add("��������������� �������", "�����. �������");
            shortBugetNames.Add("������������� � ����������� �� ���������� �������, ������ � ���� ������������ ��������", "������������� �� ���������� �������");
            shortBugetNames.Add("������ �� ������������� ���������, ������������ � ��������������� � ������������� �������������", "������ �� ������������� �����. � �������. ���������");
            shortBugetNames.Add("������� ��� ����������� ���������� ���������", "������� ��� �����. ���������� ���������");
            shortBugetNames.Add("������ �� �������� ������� ����� � ����������� ������ �����������", "������ �� �������� ������� �����");
            shortBugetNames.Add("������ �� ������� ������������ � �������������� �������", "������ �� ������� �������");
            shortBugetNames.Add("���������������� ������� � �����", "���������������� ������� � �����");
            shortBugetNames.Add("������, �������, ���������� ������", "������, �������, ���������� ������");
            shortBugetNames.Add("������ ����������� ������", "������ ����������� ������");
            shortBugetNames.Add("������ �������� ��������� ������� ���������� ��������� �� �������� �������� �������� � ��������� ������� ���", "������ �� �������� �������� ��������, ��������� ������� ���");
            shortBugetNames.Add("������� �������� �������� � ��������� ������� ���", "������� �������� �������� � ��������� ������� ���");

            shortBugetNames.Add("������������������� �������", "��������������. �������");
            shortBugetNames.Add("������������ �������", "������������ �������");
            shortBugetNames.Add("������������ ������������ � ������������������ ������������", "������������ ������������ � ������������. ����.");
            shortBugetNames.Add("������������ ���������", "������������ ���������");
            shortBugetNames.Add("�������-������������ ���������", "���");
            shortBugetNames.Add("������ ���������� �����", "������ ���������� �����");
            shortBugetNames.Add("�����������", "�����������");
            shortBugetNames.Add("��������, ��������������, �������� �������� ����������", "��������, ��������������, ���");
            shortBugetNames.Add("���������������, ���������� �������� � �����", "���������������, ����������� � �����");
            shortBugetNames.Add("���������� ��������", "���������� ��������");
            shortBugetNames.Add("������������ ����������", "������������ ����������");
        }

        /// <summary>
        /// ��������� �������� ����� �������� � ������� ������� �� �������
        /// </summary>
        /// <param name="fullName">������ ���</param>
        /// <returns>������� ���</returns>
        public static string GetShortBugetName(string fullName)
        {
            if (ShortBugetNames.ContainsKey(fullName))
            {
                return ShortBugetNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// ��������� ������� ����� �������� � ������� ������� �� ��������
        /// </summary>
        /// <param name="shortName">������� ���</param>
        /// <returns>������ ���</returns>
        public static string GetFullBugetName(string shortName)
        {
            if (ShortBugetNames.ContainsValue(shortName))
            {
                foreach (string key in ShortBugetNames.Keys)
                {
                    if (ShortBugetNames[key] == shortName)
                    {
                        return key;
                    }
                }
            }
            return shortName;
        }

        #endregion 

        #region ������� ������������ ��

        /// <summary>
        /// ������� ������������ ��
        /// </summary>
        private static Dictionary<string, string> shortKDNames;

        /// <summary>
        /// ���������� ������� ������� ������������ ��
        /// </summary>
        public static Dictionary<string, string> ShortKDNames
        {
            get
            {
                // ���� ������� ������
                if (shortKDNames == null || shortKDNames.Count == 0)
                {
                    // ��������� ���
                    FillShortKDNames();
                }
                return shortKDNames;
            }
        }

        private static void FillShortKDNames()
        {
            shortKDNames = new Dictionary<string, string>();
            shortKDNames.Add("����� �� ������� �����������", "����� �� �������");
            shortKDNames.Add("����� �� ������ ���������� ���", "����");
            shortKDNames.Add("����� �� ��������� ���������� ���", "����");
            shortKDNames.Add("�����, ��������� � ����� � ���������� ��������", "���");
            shortKDNames.Add("�����, ��������� � ����� � ����������� ���������� ������� ���������������", "���");
            shortKDNames.Add("������ ����� �� ��������� �����", "����");
            shortKDNames.Add("������ ����� �� ��������� ����� ��� ��������� ����� ������������", "����");
            shortKDNames.Add("����� �� ��������� �����������", "���");
            shortKDNames.Add("������ �� ������������� ���������", "������ �� ���. ���������");
            shortKDNames.Add("����� �� ������������� �����", "����� �� ���. �����");
            shortKDNames.Add("������ �� ������� ���������", "������ �� ����. ���������");
            shortKDNames.Add("����� �� ������ �������� ����������", "����");
            shortKDNames.Add("������ �������������������� �����", "����");
            shortKDNames.Add("��������������� �������", "���.�������");
            shortKDNames.Add("������������� � ����������� �� ���������� �������, ������ � ���� ������������ ��������", "������������� �� ���������� �������");
        }

        /// <summary>
        /// ��������� �������� ����� �� �� �������
        /// </summary>
        /// <param name="fullName">������ ���</param>
        /// <returns>������� ���</returns>
        public static string GetShortKDName(string fullName)
        {
            if (ShortKDNames.ContainsKey(fullName))
            {
                return ShortKDNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// ��������� ������� ����� �� �� ��������
        /// </summary>
        /// <param name="shortName">������� ���</param>
        /// <returns>������ ���</returns>
        public static string GetFullKDName(string shortName)
        {
            if (ShortKDNames.ContainsValue(shortName))
            {
                foreach (string key in ShortKDNames.Keys)
                {
                    if (ShortKDNames[key] == shortName)
                    {
                        return key;
                    }
                }
            }
            return shortName;
        }

        #endregion 

        #region �������� ��� ����������� ���������� �� ���������� ������� ��������

        /// <summary>
        /// �������� ��� ����������� ����������
        /// </summary>
        private static Dictionary<string, string> hotInfoNonEmptyDays;

        /// <summary>
        /// ���������� ������� �������� ���� ����������� ����������
        /// </summary>
        public static Dictionary<string, string> HotInfoNonEmptyDays
        {
            get
            {
                // ���� ������� ������
                if (hotInfoNonEmptyDays == null || hotInfoNonEmptyDays.Count == 0)
                {
                    // ��������� ���
                    FillHotInfoNonEmptyDays();
                }
                return hotInfoNonEmptyDays;
            }
        }

        private static void FillHotInfoNonEmptyDays()
        {
            hotInfoNonEmptyDays = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("hotInfoNonEmptyDays", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    hotInfoNonEmptyDays.Add(GetDictionaryUniqueKey(hotInfoNonEmptyDays, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    hotInfoNonEmptyDays.Add(GetDictionaryUniqueKey(hotInfoNonEmptyDays, curMonth), "Month");
                    month = curMonth;
                }

                string day = GetDictionaryUniqueKey(hotInfoNonEmptyDays, row[4].ToString());
                hotInfoNonEmptyDays.Add(day, "Day");
            }
        }

        #endregion 

        #region �������� ������ ����������� ���������� �� ���������� ������� ��������

        /// <summary>
        /// �������� ������ ����������� ����������
        /// </summary>
        private static Dictionary<string, string> hotInfoNonEmptyMonths;

        /// <summary>
        /// ���������� ������� �������� ������� ����������� ����������
        /// </summary>
        public static Dictionary<string, string> HotInfoNonEmptyMonths
        {
            get
            {
                // ���� ������� ������
                if (hotInfoNonEmptyMonths == null || hotInfoNonEmptyMonths.Count == 0)
                {
                    // ��������� ���
                    FillHotInfoNonEmptyMonths();
                }
                return hotInfoNonEmptyMonths;
            }
        }

        private static void FillHotInfoNonEmptyMonths()
        {
            hotInfoNonEmptyMonths = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("hotInfoNonEmptyMonths", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    hotInfoNonEmptyMonths.Add(GetDictionaryUniqueKey(hotInfoNonEmptyMonths, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    hotInfoNonEmptyMonths.Add(GetDictionaryUniqueKey(hotInfoNonEmptyMonths, curMonth), "Month");
                    month = curMonth;
                }
            }
        }

        #endregion 

        #region �������� ��� ����������� ����� �����

        /// <summary>
        /// �������� ��� ����������� ����� �����
        /// </summary>
        private static Dictionary<string, string> labourMarketNonEmptyDays;

        /// <summary>
        /// ���������� ������� �������� ���� ����������� ����� �����
        /// </summary>
        public static Dictionary<string, string> LabourMarketNonEmptyDays
        {
            get
            {
                // ���� ������� ������
                if (labourMarketNonEmptyDays == null || labourMarketNonEmptyDays.Count == 0)
                {
                    // ��������� ���
                    FillLabourMarketNonEmptyDays();
                }
                return labourMarketNonEmptyDays;
            }
        }

        private static void FillLabourMarketNonEmptyDays()
        {
            labourMarketNonEmptyDays = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("labourMarketNonEmptyDays", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    labourMarketNonEmptyDays.Add(GetDictionaryUniqueKey(labourMarketNonEmptyDays, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    labourMarketNonEmptyDays.Add(GetDictionaryUniqueKey(labourMarketNonEmptyDays, curMonth), "Month");
                    month = curMonth;
                }

                string day = GetDictionaryUniqueKey(labourMarketNonEmptyDays, row[4].ToString());
                labourMarketNonEmptyDays.Add(day, "Day");
            }
        }

        #endregion 
        private static Dictionary<string, string> creditsNonEmptyDays;
        public static Dictionary<string, string> CreditsNonEmptyDays
        {
            get
            {
                // ���� ������� ������
                if (creditsNonEmptyDays == null || creditsNonEmptyDays.Count == 0)
                {
                    // ��������� ���
                    FillCreditsNonEmptyDays();
                }
                return creditsNonEmptyDays;
            }
        }

        private static void FillCreditsNonEmptyDays()
        {
            creditsNonEmptyDays = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("creditsNonEmptyDays", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    creditsNonEmptyDays.Add(GetDictionaryUniqueKey(creditsNonEmptyDays, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    creditsNonEmptyDays.Add(GetDictionaryUniqueKey(creditsNonEmptyDays, curMonth), "Month");
                    month = curMonth;
                }

                string day = GetDictionaryUniqueKey(creditsNonEmptyDays, row[4].ToString());
                creditsNonEmptyDays.Add(day, "Day");
            }
        }


        #region ������ ����������� ����������� ����� �����

       

        private static Dictionary<string, string> indicatorsTypes;
        
        public static Dictionary<string, string> IndicatorsTypes
        {
        get
            {
                // ���� ������� ������
                if (indicatorsTypes == null || indicatorsTypes.Count == 0)
                {
                    // ��������� ���
                    FillIndicatorsTypes();
                }
                return indicatorsTypes;
            }
        }

        private static Dictionary<string, string> okvedTypes;
        public static Dictionary<string, string> OKVEDTypes
        {
            get
            {
                // ���� ������� ������
                if (okvedTypes == null || okvedTypes.Count == 0)
                {
                    // ��������� ���
                    FillOKVEDTypes();
                }
                return okvedTypes;
            }
        }
        private static Dictionary<string, string> kindsTypes;

        public static Dictionary<string, string> KindsTypes
        {
            get
            {
                // ���� ������� ������
                if (kindsTypes == null || KindsTypes.Count == 0)
                {
                    // ��������� ���
                    FillKindsTypes();
                }
                return kindsTypes;
            }
        }

        /// <summary>
        /// ������ ����������� ����������� ����� �����
        /// </summary>
        private static Dictionary<string, string> labourMarketIndicatorNumbers;
        /// <summary>
        /// ���������� ������� ������� ����������� ����������� ����� �����
        /// </summary>
        public static Dictionary<string, string> LabourMarketIndicatorNumbers
        {
            get
            {
                // ���� ������� ������
                if (labourMarketIndicatorNumbers == null || labourMarketIndicatorNumbers.Count == 0)
                {
                    // ��������� ���
                    FillLabourMarketIndicatorNumbers();
                }
                return labourMarketIndicatorNumbers;
            }
        }

        private static void FillLabourMarketIndicatorNumbers()
        {
            labourMarketIndicatorNumbers = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("labourMarketIndicatorNumbers", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(row[1].ToString(), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out value))
                    {
                        labourMarketIndicatorNumbers.Add(row[0].ToString(), string.Format("{0:N0}", value));
                    }
                    else
                    {
                        labourMarketIndicatorNumbers.Add(row[0].ToString(),row[1].ToString());
                    }
                }
            }
        }

        public static string GetLabourMarketIndicatorNumber(string indicatorName)
        {
            if (LabourMarketIndicatorNumbers.ContainsKey(indicatorName))
            {
                return LabourMarketIndicatorNumbers[indicatorName];
            }
            else if (indicatorName.Split('_').Length > 0)
            {
                string indName = indicatorName.Split('_')[0];
                if (LabourMarketIndicatorNumbers.ContainsKey(indName))
                {
                    return LabourMarketIndicatorNumbers[indName];
                }
            }

            return "0";
        }

        #endregion 

        #region ���������� ����������� ����� �����

        /// <summary>
        /// ������ ��������
        /// </summary>
        private static Dictionary<string, string> labourMarketIndicatorsTypes;

        /// <summary>
        /// ���������� �������� ������� ��������
        /// </summary>
        public static Dictionary<string, string> LabourMarketIndicatorsTypes
        {
            get
            {
                // ���� ������� ������
                if (labourMarketIndicatorsTypes == null || labourMarketIndicatorsTypes.Count == 0)
                {
                    // ��������� ���
                    FillLabourMarketIndicatorsTypes();
                }
                return labourMarketIndicatorsTypes;
            }
        }

        private static void FillLabourMarketIndicatorsTypes()
        {
            labourMarketIndicatorsTypes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("labourMarketIndicators", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������", dt);

            // ���� �� ����� ���� � ������������, �� �������� ������ �������
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() != "���" &&
                    row[0].ToString() != "����������� ������������ ��������� ���������")
                {
                    labourMarketIndicatorsTypes.Add(row[0].ToString(), row[1].ToString());
                }
            }
        }

        private static void FillIndicatorsTypes()
        {
            indicatorsTypes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("Indicators", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������", dt);

            // ���� �� ����� ���� � ������������, �� �������� ������ �������
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() != "���" &&
                    row[0].ToString() != "����������� ������������ ��������� ���������")
                {
                    indicatorsTypes.Add(String.Format("{0} {1}", row[1], row[0]), row[2].ToString());
                }
            }
        }

        private static void FillOKVEDTypes()
        {
            okvedTypes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("OKVEDList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "����������", dt);

            // ���� �� ����� ���� � ������������, �� �������� ������ �������
            foreach (DataRow row in dt.Rows)
            {
                
                    okvedTypes.Add(String.Format("{0}", row[0]), row[1].ToString());
            
            }
        }

        public static string GetIndicator(string Indicator)
        { 
            if (IndicatorsTypes.ContainsValue(Indicator))
            {
                foreach (string key in IndicatorsTypes.Keys)
                {
                    if (IndicatorsTypes[key].Contains(Indicator))
                    {
                        return key;
                    }
                }
            }
            return "";
        }

        private static void FillKindsTypes()
        {
            kindsTypes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("Kinds", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����", dt);

            // ���� �� ����� ���� � ������������, �� �������� ������ �������
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() != "���" &&
                    row[0].ToString() != "����������� ������������ ��������� ���������")
                {
                    kindsTypes.Add(row[0].ToString(), row[1].ToString());
                }
            }
        }

        #endregion 

        #region ������ ��������� ������������ ������

        /// <summary>
        ///  ������ ��������� ������������ ������
        /// </summary>
        private static Dictionary<string, string> foSubjectList;

        /// <summary>
        /// ���������� �������� ��������� ������������ ������
        /// </summary>
        public static Dictionary<string, string> FOSubjectList
        {
            get
            {
                // ���� ������� ������
                if (foSubjectList == null || foSubjectList.Count == 0)
                {
                    // ��������� ���
                    FillFOSubjectList();
                }
                return foSubjectList;
            }
        }

        private static void FillFOSubjectList()
        {
            foSubjectList = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FOSubjectList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "��������", dt);

            foreach (DataRow row in dt.Rows)
            {
                foSubjectList.Add(row[0].ToString(), row[1].ToString());
            }
        }

        #endregion 

        #region �������������� ��� � ����������

        /// <summary>
        /// �������������� ���
        /// </summary>
        private static Dictionary<string, string> mbtAdministratorsDetailUniqNames;
        private static Dictionary<string, string> mbtAdministratorsDetailLevels;

        /// <summary>
        /// ���������� ������� ������������ ��������������� ���
        /// </summary>
        public static Dictionary<string, string> MBTAdministratorsDetailUniqNames
        {
            get
            {
                // ���� ������� ������
                if (mbtAdministratorsDetailUniqNames == null || mbtAdministratorsDetailUniqNames.Count == 0)
                {
                    // ��������� ���
                    FillMBTAdministratorsDetail();
                }
                return mbtAdministratorsDetailUniqNames;
            }
        }

        /// <summary>
        /// ���������� ������� ������� ��������������� ���
        /// </summary>
        public static Dictionary<string, string> MBTAdministratorsDetailLevels
        {
            get
            {
                // ���� ������� ������
                if (mbtAdministratorsDetailLevels == null || mbtAdministratorsDetailLevels.Count == 0)
                {
                    // ��������� ���
                    FillMBTAdministratorsDetail();
                }
                return mbtAdministratorsDetailLevels;
            }
        }

        private static void FillMBTAdministratorsDetail()
        {
            mbtAdministratorsDetailUniqNames = new Dictionary<string, string>();
            mbtAdministratorsDetailLevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("MBTAdministratorDetailList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "�������������� ���", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    string key = row[0].ToString();
                    if (key.ToLower().Contains("��������"))
                    {
                        key = "��������";
                    }
                    else if (key.ToLower().Contains("�������"))
                    {
                        key = "�������";
                    }
                    else if (key.ToLower().Contains("���������"))
                    {
                        key = "���������";
                    }
                    else if (key.ToLower().Contains("����"))
                    {
                        key = "����";
                    }
                    key = GetDictionaryUniqueKey(mbtAdministratorsDetailUniqNames, key);
                    mbtAdministratorsDetailUniqNames.Add(key, row[1].ToString());
                    mbtAdministratorsDetailLevels.Add(key, row[2].ToString());
                }
            }
        }

        #endregion 

        #region �������������� ��� ��� ��������

        /// <summary>
        /// �������������� ���
        /// </summary>
        private static Dictionary<string, string> mbtAdministratorsUniqNames;
        private static Dictionary<string, string> mbtAdministratorsLevels;

        /// <summary>
        /// ���������� ������� ������������ ��������������� ���
        /// </summary>
        public static Dictionary<string, string> MBTAdministratorsUniqNames
        {
            get
            {
                // ���� ������� ������
                if (mbtAdministratorsUniqNames == null || mbtAdministratorsUniqNames.Count == 0)
                {
                    // ��������� ���
                    FillMBTAdministrators();
                }
                return mbtAdministratorsUniqNames;
            }
        }

        /// <summary>
        /// ���������� ������� ������� ��������������� ���
        /// </summary>
        public static Dictionary<string, string> MBTAdministratorsLevels
        {
            get
            {
                // ���� ������� ������
                if (mbtAdministratorsLevels == null || mbtAdministratorsLevels.Count == 0)
                {
                    // ��������� ���
                    FillMBTAdministrators();
                }
                return mbtAdministratorsLevels;
            }
        }

        private static void FillMBTAdministrators()
        {
            mbtAdministratorsUniqNames = new Dictionary<string, string>();
            mbtAdministratorsLevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("MBTAdministratorList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "�������������� ���", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    string key = row[0].ToString();
                    key = GetDictionaryUniqueKey(mbtAdministratorsUniqNames, key);
                    mbtAdministratorsUniqNames.Add(key, row[1].ToString());
                    mbtAdministratorsLevels.Add(key, row[2].ToString());
                }
            }
        }

        #endregion 

        #region ������ ������ ������� ��������

        /// <summary>
        /// ������ ������� �������
        /// </summary>
        private static Dictionary<string, string> fullBudgetLevelNumbers;
        /// <summary>
        /// ���������� ����� ������� �������
        /// </summary>
        private static Dictionary<string, string> fullBudgetLevelUniqNames;
        /// <summary>
        /// ���������� ����� �������
        /// </summary>
        private static Dictionary<string, string> fullBudgetRegionUniqNames;

        /// <summary>
        /// ���������� ������� ������� ������� ��������
        /// </summary>
        public static Dictionary<string, string> FullBudgetLevelNumbers
        {
            get
            {
                // ���� ������� ������
                if (fullBudgetLevelNumbers == null || fullBudgetLevelNumbers.Count == 0)
                {
                    // ��������� ���
                    FillFullBudgetLevels();
                }
                return fullBudgetLevelNumbers;
            }
        }

        /// <summary>
        /// ���������� ������� ���������� ���� ������� ��������
        /// </summary>
        public static Dictionary<string, string> FullBudgetLevelUniqNames
        {
            get
            {
                // ���� ������� ������
                if (fullBudgetLevelUniqNames == null || fullBudgetLevelUniqNames.Count == 0)
                {
                    // ��������� ���
                    FillFullBudgetLevels();
                }
                return fullBudgetLevelUniqNames;
            }
        }

        /// <summary>
        /// ���������� ������� ���������� ���� �������
        /// </summary>
        public static Dictionary<string, string> FullBudgetRegionUniqNames
        {
            get
            {
                // ���� ������� ������
                if (fullBudgetRegionUniqNames == null || fullBudgetRegionUniqNames.Count == 0)
                {
                    // ��������� ���
                    FillFullBudgetLevels();
                }
                return fullBudgetRegionUniqNames;
            }
        }

        private static void FillFullBudgetLevels()
        {
            fullBudgetLevelNumbers = new Dictionary<string, string>();
            fullBudgetLevelUniqNames = new Dictionary<string, string>();
            fullBudgetRegionUniqNames = new Dictionary<string, string>();

            CustomParam.CustomParamFactory("localSettlementLevelName").Value = RegionSettingsHelper.Instance.SettlementLevel;
            CustomParam.CustomParamFactory("localLevelName").Value = RegionSettingsHelper.Instance.RegionsLevel;
            CustomParam.CustomParamFactory("fns_district_budget_level").Value = RegionSettingsHelper.Instance.FNSDistrictBudgetLevel;

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("fullBudgetLevels", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������ �������", dt);

            // ���� �� ����� ���� � ������������, �� �������� ������ �������
            foreach (DataRow row in dt.Rows)
            {
                if (row[1] != DBNull.Value)
                {
                    string uniqKey = GetDictionaryUniqueKey(fullBudgetLevelNumbers, row[0].ToString());
                    fullBudgetLevelNumbers.Add(uniqKey, row[1].ToString());
                }
                if (row[2] != DBNull.Value)
                {
                    string uniqKey = GetDictionaryUniqueKey(fullBudgetLevelUniqNames, row[0].ToString());
                    fullBudgetLevelUniqNames.Add(uniqKey, row[2].ToString());
                }
                if (row[3] != DBNull.Value)
                {
                    string uniqKey = GetDictionaryUniqueKey(fullBudgetRegionUniqNames, row[0].ToString());
                    fullBudgetRegionUniqNames.Add(uniqKey, row[3].ToString());
                }
            }
        }

        #endregion 

        #region ������� ������������ �����

        /// <summary>
        /// ������� ������������ �����
        /// </summary>
        private static Dictionary<string, string> shortOKVDNames;

        /// <summary>
        /// ���������� ������� ������� ������������ �����
        /// </summary>
        public static Dictionary<string, string> ShortOKVDNames
        {
            get
            {
                // ���� ������� ������
                if (shortOKVDNames == null || shortOKVDNames.Count == 0)
                {
                    // ��������� ���
                    FillShortOKVDNames();
                }
                return shortOKVDNames;
            }
        }

        private static void FillShortOKVDNames()
        {
            shortOKVDNames = new Dictionary<string, string>();
            shortOKVDNames.Add("�������� ���������, ����� � ������ ���������", "�/�, �����, �/�");
            shortOKVDNames.Add("������ �������� ����������", "������ ��");
            shortOKVDNames.Add("������������ � ������������� ��������������, ���� � ����", "�\\�, ���, ����");
            shortOKVDNames.Add("������� � ��������� ��������; ������ ���������������� �������, ����������, ������� ������� � ��������� ������� �����������", "��������, ������");
            shortOKVDNames.Add("�������� � ���������� ����������, ������ � �������������� �����", "������������");
            shortOKVDNames.Add("��������������� ���������� � ����������� ������� ������������; ������������ ���������� �����������", "���. ����������, ���-���; ���. �����."); 
            shortOKVDNames.Add("��������������� ���������� � ����������� ������� ������������; ���������� �����������", "���. ����������, ���-���; ���. �����.");
            shortOKVDNames.Add("��������������� � �������������� ���������� �����", "�����. � ���.������");
            shortOKVDNames.Add("�������������� ������ ������������, ���������� � ������������ �����, ������������ �������� ��������, ������������ ������������������ �����������", "������ �������");
            shortOKVDNames.Add("�������������� ������ ������������, ���������� � ������������ �����", "������ ����., ���. � ����.������");
        }

        /// <summary>
        /// ��������� �������� ����� ����� �� �������
        /// </summary>
        /// <param name="fullName">������ ���</param>
        /// <returns>������� ���</returns>
        public static string GetShortOKVDName(string fullName)
        {
            if (ShortOKVDNames.ContainsKey(fullName))
            {
                return ShortOKVDNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// ��������� ������� ����� ����� �� ��������
        /// </summary>
        /// <param name="shortName">������� ���</param>
        /// <returns>������ ���</returns>
        public static string GetFullOKVDName(string shortName)
        {
            if (ShortOKVDNames.ContainsValue(shortName))
            {
                foreach (string key in ShortOKVDNames.Keys)
                {
                    if (ShortOKVDNames[key] == shortName)
                    {
                        return key;
                    }
                }
            }
            return shortName;
        }

        #endregion 

        #region �������� ��� ��� ������������� ���

        /// <summary>
        /// �������� ��� ��� ������������� ���
        /// </summary>
        private static Dictionary<string, string> mbtNonEmptyDays;

        /// <summary>
        /// ���������� ������� �������� ���� ��� ������������� ���
        /// </summary>
        public static Dictionary<string, string> MBTNonEmptyDays
        {
            get
            {
                // ���� ������� ������
                if (mbtNonEmptyDays == null || mbtNonEmptyDays.Count == 0)
                {
                    // ��������� ���
                    FillMBTNonEmptyDays();
                }
                return mbtNonEmptyDays;
            }
        }

        private static void FillMBTNonEmptyDays()
        {
            mbtNonEmptyDays = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("mbtNonEmptyDays", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    mbtNonEmptyDays.Add(GetDictionaryUniqueKey(mbtNonEmptyDays, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    mbtNonEmptyDays.Add(GetDictionaryUniqueKey(mbtNonEmptyDays, curMonth), "Month");
                    month = curMonth;
                }

                string day = GetDictionaryUniqueKey(mbtNonEmptyDays, row[4].ToString());
                mbtNonEmptyDays.Add(day, "Day");
            }
        }

        #endregion 

        #region ������ ��������� ������ ��

        /// <summary>
        /// ������ ��������� ������ ��
        /// </summary>
        private static Dictionary<string, string> foFondVariantList;

        /// <summary>
        /// ���������� ������� �� �������� ��������� ����
        /// </summary>
        public static Dictionary<string, string> FOFondVariantList
        {
            get
            {
                // ���� ������� ������
                if (foFondVariantList == null || foFondVariantList.Count == 0)
                {
                    // ��������� ���
                    FillFOFondVariantList();
                }
                return foFondVariantList;
            }
        }

        private static void FillFOFondVariantList()
        {
            foFondVariantList = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FOFondVariantList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    foFondVariantList.Add(row[0].ToString(), row[1].ToString());
                }
            }
        }

        #endregion 

        #region ������ �������� �����������

        /// <summary>
        /// ���������� ����� �������� �����������
        /// </summary>
        private static Dictionary<string, string> mainActivityUniqNames;
        /// <summary>
        /// ������ �������������� �����������
        /// </summary>
        private static Dictionary<string, string> mainActivityLevels;

        /// <summary>
        /// ���������� ������� ����.���� �������� �����������
        /// </summary>
        public static Dictionary<string, string> MainActivityUniqNames
        {
            get
            {
                // ���� ������� ������
                if (mainActivityUniqNames == null || mainActivityUniqNames.Count == 0)
                {
                    // ��������� ���
                    FillMainActivityUniqNames();
                }
                return mainActivityUniqNames;
            }
        }

        /// <summary>
        /// ���������� ������� ������� �������� �����������
        /// </summary>
        public static Dictionary<string, string> MainActivityLevels
        {
            get
            {
                // ���� ������� ������
                if (mainActivityLevels == null || mainActivityLevels.Count == 0)
                {
                    // ��������� ���
                    FillMainActivityUniqNames();
                }
                return mainActivityLevels;
            }
        }

        private static void FillMainActivityUniqNames()
        {
            mainActivityUniqNames = new Dictionary<string, string>();
            mainActivityLevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("MainActivityList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "�������� �����������", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty &&
                    row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                {
                    string key = GetDictionaryUniqueKey(mainActivityUniqNames, row[0].ToString());
                    mainActivityUniqNames.Add(key, row[1].ToString());
                    mainActivityLevels.Add(key, row[2].ToString());
                }
            }
        }

        #endregion 

        #region ������ �������������� �����������
        
        /// <summary>
        /// ���������� ����� �������������� �����������
        /// </summary>
        private static Dictionary<string, string> additionalActivityUniqNames;
        /// <summary>
        /// ������ �������������� �����������
        /// </summary>
        private static Dictionary<string, string> additionalActivityLevels;

        /// <summary>
        /// ���������� ������� ����.���� �������������� �����������
        /// </summary>
        public static Dictionary<string, string> AdditionalActivityUniqNames
        {
            get
            {
                // ���� ������� ������
                if (additionalActivityUniqNames == null || additionalActivityUniqNames.Count == 0)
                {
                    // ��������� ���
                    FillAdditionalActivityUniqNames();
                }
                return additionalActivityUniqNames;
            }
        }

        /// <summary>
        /// ���������� ������� ������� �������������� �����������
        /// </summary>
        public static Dictionary<string, string> AdditionalActivityLevels
        {
            get
            {
                // ���� ������� ������
                if (additionalActivityLevels == null || additionalActivityLevels.Count == 0)
                {
                    // ��������� ���
                    FillAdditionalActivityUniqNames();
                }
                return additionalActivityLevels;
            }
        }

        private static void FillAdditionalActivityUniqNames()
        {
            additionalActivityUniqNames = new Dictionary<string, string>();
            additionalActivityLevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("AdditionalActivityList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "�������������� �����������", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty &&
                    row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                {
                    string key = row[0].ToString();
                    if (!additionalActivityUniqNames.ContainsKey(key))
                    {
                        additionalActivityUniqNames.Add(key, row[1].ToString());
                        additionalActivityLevels.Add(key, row[2].ToString());
                    }
                }
            }
        }

        #endregion 

        #region ������ ������������� ������� ��� ������� �� ����

        private static Dictionary<string, string> mofoRegionsTypes;
        private static Dictionary<string, string> mofoRegionsUniqueNames;

        /// <summary>
        /// ���������� �������� ����� ������������� ������� ��� ������� �� ����
        /// </summary>
        public static Dictionary<string, string> MOFORegionsTypes
        {
            get
            {
                // ���� ������� ������
                if (mofoRegionsTypes == null || mofoRegionsTypes.Count == 0)
                {
                    // ��������� ���
                    FillMOFORegions();
                }
                return mofoRegionsTypes;
            }
        }

        /// <summary>
        /// ���������� �������� ���������� ���� ������������� ������� ��� ������� �� ����
        /// </summary>
        public static Dictionary<string, string> MOFORegionsUniqueNames
        {
            get
            {
                // ���� ������� ������
                if (mofoRegionsUniqueNames == null || mofoRegionsUniqueNames.Count == 0)
                {
                    // ��������� ���
                    FillMOFORegions();
                }
                return mofoRegionsUniqueNames;
            }
        }

        private static void FillMOFORegions()
        {
            mofoRegionsTypes = new Dictionary<string, string>();
            mofoRegionsUniqueNames = new Dictionary<string, string>();

            CustomParam.CustomParamFactory("settlement_level").Value = RegionSettingsHelper.Instance.SettlementLevel;
            CustomParam.CustomParamFactory("region_level").Value = RegionSettingsHelper.Instance.RegionsLevel;

            DataTable regionsDT = new DataTable();
            string query = DataProvider.GetQueryText("MOFORegionsList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "��", regionsDT);

            for (int i = 0; i < regionsDT.Rows.Count; i++)
            {
                DataRow row = regionsDT.Rows[i];
                if (row[1] != DBNull.Value && row[2] != DBNull.Value && row[3] != DBNull.Value)
                {
                    string name = row[1].ToString();
                    string type = row[2].ToString();
                    string uniqName = row[3].ToString();

                    name = name.Replace("������������� �����", "��");
                    name = name.Replace("�������� ���������", "��");
                    name = name.Replace("��������� ���������", "��");
                    name = name.Replace("������������� �����������", "��");

                    string key = GetDictionaryUniqueKey(mofoRegionsUniqueNames, name);
                    mofoRegionsTypes.Add(key, type);
                    mofoRegionsUniqueNames.Add(key, uniqName);
                }

            }
        }

        #endregion

        #region ��������� ���� ��� ����������� ����������� ���������

        public static string GetFederalPopulationDate()
        {
            string query = DataProvider.GetQueryText("FederalPopulationDate", HttpContext.Current.Request.PhysicalApplicationPath);
            DataTable dtPopulationDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "�������", dtPopulationDate);

            string date = string.Empty;

            if (dtPopulationDate.Rows.Count > 0 && dtPopulationDate.Rows[0][1] != DBNull.Value &&
                    dtPopulationDate.Rows[0][1].ToString() != string.Empty)
            {
                date = string.Format("�� 01.01.{0} �.", dtPopulationDate.Rows[0][1]);
            }

            return date;
        }

        #endregion

        #region ��������� ���� ��� ����������� ����������� ��������� �������� �������

        public static string GetRegionPopulationDate(int currentYear)
        {
            CustomParam.CustomParamFactory("current_year").Value = currentYear.ToString();
            CustomParam.CustomParamFactory("consolidate_region").Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            CustomParam.CustomParamFactory("population_cube").Value = RegionSettingsHelper.Instance.PopulationCube;
            CustomParam.CustomParamFactory("population_filter").Value = RegionSettingsHelper.Instance.PopulationFilter;
            CustomParam.CustomParamFactory("population_period_dimension").Value = RegionSettingsHelper.Instance.PopulationPeriodDimension;

            string query = DataProvider.GetQueryText("RegionPopulationDate", HttpContext.Current.Request.PhysicalApplicationPath);
            DataTable dtPopulationDate = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "�������", dtPopulationDate);

            string date = String.Empty;

            if (dtPopulationDate.Rows.Count > 0 && dtPopulationDate.Rows[0][1] != DBNull.Value &&
                    dtPopulationDate.Rows[0][1].ToString() != String.Empty)
            {
                date = String.Format("�� 01.01.{0} �.", dtPopulationDate.Rows[0][1]);
            }

            return date;
        }

        public static string GetOmskRegionPopulationDate(int currentYear)
        {
            CustomParam.CustomParamFactory("current_year").Value = currentYear.ToString();

            string query = DataProvider.GetQueryText("OmskRegionPopulationDate", HttpContext.Current.Request.PhysicalApplicationPath);
            DataTable dtPopulationDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "�������", dtPopulationDate);

            string date = String.Empty;

            if (dtPopulationDate.Rows.Count > 0 && dtPopulationDate.Rows[0][1] != DBNull.Value &&
                    dtPopulationDate.Rows[0][1].ToString() != String.Empty)
            {
                date = String.Format("�� 01.01.{0} �.", dtPopulationDate.Rows[0][1]);
            }

            return date;
        }

        #endregion

        #region ������ ����������� ��� ������ ��������

        /// <summary>
        /// ������ ����������� ��� ������ ��������
        /// </summary>
        private static Dictionary<string, string> qualityEvaluationIndicatorList;

        /// <summary>
        /// ���������� ������� �� �������� ����������� ��� ������ ��������
        /// </summary>
        public static Dictionary<string, string> QualityEvaluationIndicatorList
        {
            get
            {
                // ���� ������� ������
                if (qualityEvaluationIndicatorList == null || qualityEvaluationIndicatorList.Count == 0)
                {
                    // ��������� ���
                    FillQualityEvaluationIndicatorList();
                }
                return qualityEvaluationIndicatorList;
            }
        }

        private static void FillQualityEvaluationIndicatorList()
        {
            qualityEvaluationIndicatorList = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("QualityEvaluationIndicatorList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    qualityEvaluationIndicatorList.Add(row[0].ToString(), row[1].ToString());
                }
            }
        }

        #endregion 

        #region ������ ����������� ��� �������� ��������

        /// <summary>
        /// ������ ����������� ��� ������ ��������
        /// </summary>
        private static Dictionary<string, string> qualityValueIndicatorList;

        /// <summary>
        /// ���������� ������� �� �������� ����������� ��� �������� ��������
        /// </summary>
        public static Dictionary<string, string> QualityValueIndicatorList
        {
            get
            {
                // ���� ������� ������
                if (qualityValueIndicatorList == null || qualityValueIndicatorList.Count == 0)
                {
                    // ��������� ���
                    FillQualityValueIndicatorList();
                }
                return qualityValueIndicatorList;
            }
        }

        private static void FillQualityValueIndicatorList()
        {
            qualityValueIndicatorList = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("QualityValueIndicatorList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    qualityValueIndicatorList.Add(row[0].ToString(), row[1].ToString());
                }
            }
        }

        #endregion 

        #region ������� ������������ ���� ��� ����������� ��

        /// <summary>
        /// ������� ������������ ���� ��� ����������� ��
        /// </summary>
        private static Dictionary<string, string> shortFMGRBSNames;

        /// <summary>
        /// ���������� ������� ������� ������������ ���� ��� ����������� ��
        /// </summary>
        public static Dictionary<string, string> ShortFMGRBSNames
        {
            get
            {
                // ���� ������� ������
                if (shortFMGRBSNames == null || shortFMGRBSNames.Count == 0)
                {
                    // ��������� ���
                    FillShortFMGRBSNames();
                }
                return shortFMGRBSNames;
            }
        }

        private static void FillShortFMGRBSNames()
        {
            shortFMGRBSNames = new Dictionary<string, string>();
            shortFMGRBSNames.Add("��������������� �������� ������ �������. ���������� �����������", "�����������");
            shortFMGRBSNames.Add("��������������� �������� ������ �������", "�����������");
            shortFMGRBSNames.Add("���������� ������ ������������� ������ �������", "�����������");
            shortFMGRBSNames.Add("����������������� ������������� ������ ������� ��� ������������� ���������� ���������", "�����������������");
            shortFMGRBSNames.Add("����������-������� ������ ������ �������", "���");
            shortFMGRBSNames.Add("������������ ��������������-��������� �������� ������ �������", "������");
            shortFMGRBSNames.Add("������������ ��������������� ������ �������", "��������");
            shortFMGRBSNames.Add("������������ ������������� ��������� ������ �������", "������������");
            shortFMGRBSNames.Add("������������ �������� ������ �������", "�����������");
            shortFMGRBSNames.Add("������������ ����������� ������ �������", "������");
            shortFMGRBSNames.Add("������������ �� ����� ��������, ���������� �������� � ������ ������ �������", "�����������");
            shortFMGRBSNames.Add("������������ ������������ ��������, ���������� � ����� ������ �������", "�������");
            shortFMGRBSNames.Add("������������ ��������� ��������� � �������������� ������ �������", "��������������");
            shortFMGRBSNames.Add("������������ ������������� � �������-������������� ��������� ������ �������", "��������");
            shortFMGRBSNames.Add("������������ ����� � ����������� �������� ������ �������", "�������");
            shortFMGRBSNames.Add("������������ ��������� ������ �������", "���������");
            shortFMGRBSNames.Add("������� ��������������-�������� ���������� ������ �������", "����");
            shortFMGRBSNames.Add("������� ���������� �� ����� ������, ���������������� � ������� �������� ������������ ������ �������", "�� ������");
            shortFMGRBSNames.Add("������� ���������� �� ����� ����������� ������� � ������������ ��������� ������ �������", "�� �� ����� �� � ��");
            shortFMGRBSNames.Add("��������������� �������� ��������� ������ �������", "����������");
            shortFMGRBSNames.Add("������� ���������� ����������� ������ �������", "�� �����������");
            shortFMGRBSNames.Add("���������� ���������� ��� �� ������ �������", "���");
            shortFMGRBSNames.Add("������������� �������� ������ �������", "��������");
            shortFMGRBSNames.Add("������� ���������� ���������������� ������������� ������� � ��������������� ���������� ������ �������", "�� ������.������");
            shortFMGRBSNames.Add("������������ �������������� �������� ������ �������", "���");
            shortFMGRBSNames.Add("��������������� ��������� �� ������� �� ����������� ���������� ���������� ����� � ������ ����� ������� ��� ������������ ��������� ��������� � �������������� ������ �������", "������������");
            shortFMGRBSNames.Add("������� ���������� ��������������� ������ ��������� ��������� ������ �������", "�� ���������");
            shortFMGRBSNames.Add("������� ���������� �� ��������� �������� ������ �������", "�� �� �����������");
            shortFMGRBSNames.Add("�������������� ������ ������� �� ������ ��������", "��������������");
            shortFMGRBSNames.Add("������� ���������� �������������� ���������� � ���������������� ������ �������", "������");

            shortFMGRBSNames.Add("������������ �������� ������ �������", "������");
            shortFMGRBSNames.Add("������� ���������� ������� ��������� ������ �������", "�� ������");
            shortFMGRBSNames.Add("������� ���������� ��������� ��������, ���������������� ������������� ������� ������ �������", "�����������������");

            shortFMGRBSNames.Add("���� �����-����������� ����������� ������ - ����", "���� ����");
            shortFMGRBSNames.Add("������������� �����-����������� ����������� ������ � ����", "������������� ����");
            shortFMGRBSNames.Add("����������������� ����� - ����������� ����������� ������ - ���� ��� ������������� ���������� ��������� � � ��������� ���������� ���������", "����������������� ����");
            shortFMGRBSNames.Add("����������������� �����-����������� ����������� ������ � ���� � �. ������", "����������������� ���� � �. ������");
            shortFMGRBSNames.Add("������������ ������ �� ������� �����-����������� ����������� ������ - ����", "��� ����");
            shortFMGRBSNames.Add("������ �� �������� � ������� � ����� ��������������� �����-����������� ����������� ������ - ����", "����������� ����");
            shortFMGRBSNames.Add("���������� ����������� �����-����������� ����������� ������ � ����", "������������� ����");
            shortFMGRBSNames.Add("������ ���������������� ������� �� ����������� ���������� ���������� ����� � ������ ����� ������� �����-����������� ����������� ������ - ����", "������������ ����");
            shortFMGRBSNames.Add("������� ���������� ������������ ���������� ��������� �� ����� ����������� �������, ������������ ��������� � ���������� ����������� ��������� �������� �� �����-����������� ����������� ������ � ����", "�� �� ����� �� � �� ����");
            shortFMGRBSNames.Add("����������� ��������� ��������� � ���������� �����-����������� ����������� ������ - ����", "��������� � ���������� ����");
            shortFMGRBSNames.Add("���������� ���������� ��� �� �����-����������� ����������� ������ - ����", "��� ����");
            shortFMGRBSNames.Add("����������� ����������, ����� � ���������� �����-����������� ����������� ������ � ����", "��������� � ���������� ����");
            shortFMGRBSNames.Add("����������� ����������� � ���������� �������� �����- ����������� ����������� ������ - ����", "�������������� � �������� ����");
            shortFMGRBSNames.Add("����������� �������� �����-����������� ����������� ������ - ����", "����������� ����");
            shortFMGRBSNames.Add("����������� ������������ ������ �����-����������� ����������� ������ - ����", "����������� ������������ ������ ����");
            shortFMGRBSNames.Add("����������� ��������������� �����-����������� ����������� ������ � ����", "�������� ����");
            shortFMGRBSNames.Add("����������� ���������� �������� � ������ �����- ����������� ����������� ������ - ����", "�������� ����");
            shortFMGRBSNames.Add("������� �� ���������� �������� �����-����������� ����������� ������ � ����", "������� �� ���������� �������� ����");
            shortFMGRBSNames.Add("����������� ����������� �������� �����-����������� ����������� ������ - ����", "�������������� ����");
            shortFMGRBSNames.Add("����������� �������� �����- ����������� ����������� ������ - ����", "����������� ����");
            shortFMGRBSNames.Add("����������� ����� � ��������� ��������� �����-����������� ����������� ������ - ����", "�������� � ��������� ����");
            shortFMGRBSNames.Add("�������� ����������� �����-����������� ����������� ������ � ����", "�������� ����������� ����");
            shortFMGRBSNames.Add("����������� ����������� ������ ��������� �����- ����������� ����������� ������ - ����", "����������� ����������� ������ ��������� ����");
            shortFMGRBSNames.Add("����������� �� �������� ������� �����-����������� ����������� ������ � ����", "���������� ����");
            shortFMGRBSNames.Add("����������������� �����-����������� ����������� ������ � ���� � �. �����-����������", "����������������� ���� � �. �����-����������");
            shortFMGRBSNames.Add("����������� ��������� �������� � ����������� ������� ��������� �����- ����������� ����������� ������ - ����", "��������������� � ����������� ������� ��������� ����");
            shortFMGRBSNames.Add("������ �� �������� � ������� � ����� ����������� �����- ����������� ����������� ������ - ����", "��������� ����");
            shortFMGRBSNames.Add("������ ��������� �������� � ������������� ������� �����-����������� ����������� ������ - ����", "�������������� ����");
            shortFMGRBSNames.Add("����������� �� ���������� ��������������� ���������� �����-����������� ����������� ������ - ����", "������������ ����");
            shortFMGRBSNames.Add("������������� �������� �����-����������� ����������� ������ - ����", "������������ ����");
            shortFMGRBSNames.Add("������� ��������������� ����������� �����-����������� ����������� ������ � ����", "������� �������������� ����");
            shortFMGRBSNames.Add("����������� ������������� �����-����������� ����������� ������ � ����", "�������� ����");
            shortFMGRBSNames.Add("���������� ����������������� ��������� �����-����������� ����������� ������ � ����", "���������� ��� ����");
            shortFMGRBSNames.Add("����������� �������������, ���������� � �������-  ������������� ��������� �����-����������� ����������� ������ - ����", "������������������ � ��� ����");
            shortFMGRBSNames.Add("����������� �� �������� ������������� ������� ������ �����-����������� ����������� ������ � ����", "������������������ ������ ����");
            shortFMGRBSNames.Add("����������� �������� �����-����������� ����������� ������ - ����", "������ ����");
            shortFMGRBSNames.Add("����������� �� ���������������� �����- ����������� ����������� ������ - ����", "�������� ����");
            shortFMGRBSNames.Add("������ ��������������� ������ �������� ����������� �������� �����-����������� ����������� ������ - ����", "�������������� ����");
            shortFMGRBSNames.Add("������ �� �������� � ������� � ����� ������ ����������  �����, �������� ��������� ���� � ������ ��������� �����-����������� ����������� ������ - ����", "������������ ����");
            shortFMGRBSNames.Add("����������������� �����-����������� ����������� ������ � ���� � �. �������������", "����������������� ���� � �. �������������");
            shortFMGRBSNames.Add("������ �� ����� ������� �����-����������� ����������� ������ - ����", "�������� ������ ����");
            shortFMGRBSNames.Add("����������� ���������������� ������ �����-����������� ����������� ������ - ����", "������������ ����");
            shortFMGRBSNames.Add("����������� �������������� ���������� �����-����������� ����������� ������ - ����", "������������������� ����");
            shortFMGRBSNames.Add("����������� ���������� �������� �����-����������� ����������� ������ - ����", "����������� ����");
            shortFMGRBSNames.Add("����������� ���������� ������ ����������� �����-����������� ����������� ������ - ����", "����������� ����������� ����");
            shortFMGRBSNames.Add("����������� �������������� �������� �����-����������� ����������� ������ - ����", "������������ ����");
            shortFMGRBSNames.Add("����������� �������� �������� �����-����������� ����������� ������ - ����", "�������������� ����");
            shortFMGRBSNames.Add("����������� ��������������� �����-����������� ����������� ������ - ����", "�������� ����");
            shortFMGRBSNames.Add("������������ ������ �����-����������� ����������� ������ - ����", "��������� ����");
            shortFMGRBSNames.Add("������������� �����-����������� ����������� ������ - ����", "������������� ����");
        }

        /// <summary>
        /// ��������� �������� ����� �� �� �������
        /// </summary>
        /// <param name="fullName">������ ���</param>
        /// <returns>������� ���</returns>
        public static string GetShortFMGRBSNames(string fullName)
        {
            if (ShortFMGRBSNames.ContainsKey(fullName))
            {
                return ShortFMGRBSNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// ��������� ������� ����� �� �� ��������
        /// </summary>
        /// <param name="shortName">������� ���</param>
        /// <returns>������ ���</returns>
        public static string GetFullFMGRBSNames(string shortName)
        {
            if (ShortFMGRBSNames.ContainsValue(shortName))
            {
                foreach (string key in ShortFMGRBSNames.Keys)
                {
                    if (ShortFMGRBSNames[key] == shortName)
                    {
                        return key;
                    }
                }
            }
            return shortName;
        }

        #endregion 

        #region ������ ���������� ��� ����

        private static Dictionary<string, string> urfoRegionUniqueNames;
        private static Dictionary<string, string> urfoRegionLevels;

        /// <summary>
        /// ���������� ������� ����.������ ���������� ��� ����
        /// </summary>
        public static Dictionary<string, string> UrfoRegionUniqueName
        {
            get
            {
                // ���� ������� ������
                if (urfoRegionUniqueNames == null || urfoRegionUniqueNames.Count == 0)
                {
                    // ��������� ���
                    FillUrfoRegionList();
                }
                return urfoRegionUniqueNames;
            }
        }

        /// <summary>
        /// ���������� ������� ������� ���������� ��� ����
        /// </summary>
        public static Dictionary<string, string> UrfoRegionLevels
        {
            get
            {
                // ���� ������� ������
                if (urfoRegionLevels == null || urfoRegionLevels.Count == 0)
                {
                    // ��������� ���
                    FillUrfoRegionList();
                }
                return urfoRegionLevels;
            }
        }

        private static void FillUrfoRegionList()
        {
            urfoRegionUniqueNames = new Dictionary<string, string>();
            urfoRegionLevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("UrFORegionList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "���������� ����", dt);

            foreach (DataRow row in dt.Rows)
            {
                urfoRegionUniqueNames.Add(GetDictionaryUniqueKey(urfoRegionUniqueNames, row[0].ToString()), row[1].ToString());
                urfoRegionLevels.Add(GetDictionaryUniqueKey(urfoRegionLevels, row[0].ToString()), row[2].ToString());
            }
        }

        #endregion 

        public static string GetShortRzPrName(string fullName)
        {
            string shortName = fullName;

            switch (fullName)
            {
                case "������������������� �������":
                    {
                        return "���������.�������";
                    }
                case "������������ �������":
                    {
                        return "������������ �������";
                    }
                case "������������ ������������ � ������������������ ������������":
                    {
                        return "���.������������ � ������������.����.";
                    }
                case "������������ ���������":
                    {
                        return "������������ ���������";
                    }
                case "�������-������������ ���������":
                    {
                        return "���";
                    }
                case "������ ���������� �����":
                    {
                        return "������ �����.�����";
                    }
                case "�����������":
                    {
                        return "�����������";
                    }
                case "��������, ��������������":
                    {
                        return "�������� � ��������������";
                    }
                case "��������, ��������������, �������� �������� ����������":
                    {
                        return "��������,  ��������������, ���";
                    }
                case "�������� �������� ����������":
                    {
                        return "���";
                    }
                case "���������������":
                    {
                        return "���������������";
                    }
                case "���������������, ���������� �������� � �����":
                    {
                        return "�����., ���.�������� � �����";
                    }
                case "���������� �������� � �����":
                    {
                        return "���������� �������� � �����";
                    }
                case "���������� ��������":
                    {
                        return "���������� ��������";
                    }
                case "������������ ����������":
                    {
                        return "������������ ����������";
                    }
                case "������������ ���������� ������ ��������� �������� ��������� ���������� ��������� � ������������� �����������":
                    {
                        return "��� �������� ���.�� � ��";
                    }
                case "������������ ���������������� � �������������� �����":
                    {
                        return "������.���.� ���.�����";
                    }
            }
            return shortName;
        }
    }
}
