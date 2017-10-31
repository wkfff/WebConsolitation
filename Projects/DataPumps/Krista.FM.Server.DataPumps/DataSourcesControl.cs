using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Lifetime;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    // ������ � ��������� ���������� ����������� ������

    /// <summary>
    /// ������� ����� ��� ���� �������
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject
    {
        #region ����� � ���������� ����������

        /// <summary>
        /// ��������� �������� ������
        /// </summary>
        /// <param name="supplierCode">��� ����������</param>
        /// <param name="dataCode">���������� ����� ����������</param>
        /// <param name="dsType">��� ���������</param>
        /// <param name="budgetName">������������ �������</param>
        /// <param name="year">���</param>
        /// <param name="month">�����</param>
        /// <param name="variant">�������</param>
        /// <param name="quarter">�������</param>
        /// <param name="isNewDataSource">��� ����� ��������, ���� ����!</param>
        /// <returns>�� ���������</returns>
        public IDataSource AddDataSource(IPumpHistoryElement phe, string supplierCode, string dataCode,
            ParamKindTypes parametersType, string budgetName, int year, int month, string variant,
            int quarter, string territory, out bool isNewDataSource)
        {
            string str;
            isNewDataSource = true;

            IDataSource dataSource = null;

            try
            {
                // ���� ����� ��������
                dataSource = FindDataSource(parametersType, supplierCode, dataCode, budgetName, year, month, variant,
                    quarter, territory);

                // ���� ����� �������� ��� ����, �� ��� � ����������. ���� ��� - ������� �����
                if (dataSource != null)
                {
                    isNewDataSource = false;
                    this.Scheme.DataSourceManager.DataSources.Add(dataSource, phe);
                }
                else
                {
                    dataSource = this.Scheme.DataSourceManager.DataSources.CreateElement();
                    dataSource.SupplierCode = supplierCode;
                    dataSource.DataCode = dataCode;
                    dataSource.DataName = constDefaultClsName;
                    str = dataCode.PadLeft(4, '0');

                    if (Scheme.DataSourceManager.DataSuppliers.ContainsKey(supplierCode))
                    {
                        if (Scheme.DataSourceManager.DataSuppliers[supplierCode].DataKinds.ContainsKey(str))
                        {
                            dataSource.DataName = Scheme.DataSourceManager.DataSuppliers[supplierCode].DataKinds[str].Name;
                        }
                    }

                    dataSource.ParametersType = parametersType;
                    dataSource.BudgetName = budgetName;
                    dataSource.Year = year;
                    dataSource.Month = month;
                    dataSource.Variant = variant;
                    dataSource.Quarter = quarter;
                    dataSource.Territory = territory;

                    this.Scheme.DataSourceManager.DataSources.Add(dataSource);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("������ ��� ���������� ��������� ������", ex);
            }

            return dataSource;
        }

        /// <summary>
        /// ��������� �������� ������
        /// </summary>
        /// <param name="supplierCode">��� ����������</param>
        /// <param name="dataCode">���������� ����� ����������</param>
        /// <param name="dsType">��� ���������</param>
        /// <param name="budgetName">������������ �������</param>
        /// <param name="year">���</param>
        /// <param name="month">�����</param>
        /// <param name="variant">�������</param>
        /// <param name="quarter">�������</param>
        /// <returns>�� ���������</returns>
        public IDataSource AddDataSource(string supplierCode, string dataCode, ParamKindTypes parametersType,
            string budgetName, int year, int month, string variant, int quarter, string territory)
        {
            bool isNewDataSource = false;

            return AddDataSource(this.PumpRegistryElement.PumpHistoryCollection[this.PumpID], supplierCode,
                dataCode, parametersType, budgetName, year, month, variant, quarter, territory, out isNewDataSource);
        }

        /// <summary>
        /// ��������� �������� ������ � ������������� �������� DataSource
        /// </summary>
        /// <param name="dsType">��� ���������</param>
        /// <param name="budgetName">������������ �������</param>
        /// <param name="year">���</param>
        /// <param name="month">�����</param>
        /// <param name="variant">�������</param>
        /// <param name="quarter">�������</param>
        protected void SetDataSource(ParamKindTypes parametersType, string budgetName, int year, int month,
            string variant, int quarter, string territory)
        {
            bool isNewDataSource;

            this.DataSource = AddDataSource(this.PumpRegistryElement.PumpHistoryCollection[this.PumpID],
                this.PumpRegistryElement.SupplierCode, this.PumpRegistryElement.DataCode, parametersType,
                budgetName, year, month, variant, quarter, territory, out isNewDataSource);

            WriteToTrace(string.Format(
                "������� SOURCEID: {0} ({1}), PUMPID: {2}.",
                this.SourceID, GetDataSourceDescription(this.SourceID), this.PumpID), TraceMessageKind.Warning);
        }

        /// <summary>
        /// ��������� �������� ������ � ������������� �������� DataSource
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        protected void SetDataSource(int sourceID)
        {
            IDataSource ds = GetDataSourceBySourceID(sourceID);

            SetDataSource(ds.ParametersType, ds.BudgetName, ds.Year, ds.Month,
                ds.Variant, ds.Quarter, ds.Territory);
        }

        /// <summary>
        /// ���� �������� ������ �� ��� ����������
        /// </summary>
        /// <param name="parametersType">��� ���������</param>
        /// <param name="supplierCode">��� ����������</param>
        /// <param name="dataCode">��� ������</param>
        /// <param name="budgetName">������������ �������</param>
        /// <param name="year">���</param>
        /// <param name="month">�����</param>
        /// <param name="variant">�������</param>
        /// <param name="quarter">�������</param>
        /// <returns>��������</returns>
        protected IDataSource FindDataSource(ParamKindTypes parametersType, string supplierCode, string dataCode,
            string budgetName, int year, int month, string variant, int quarter, string territory)
        {
            IDataSource ds = this.Scheme.DataSourceManager.DataSources.CreateElement();
            ds.BudgetName = budgetName;
            ds.DataCode = dataCode;
            ds.Month = month;
            ds.ParametersType = parametersType;
            ds.Quarter = quarter;
            ds.SupplierCode = supplierCode;
            ds.Territory = territory;
            ds.Variant = variant;
            ds.Year = year;

            int? result = this.Scheme.DataSourceManager.DataSources.FindDataSource(ds);

            if (result == null) return null;

            return this.Scheme.DataSourceManager.DataSources[(int)result];
        }

        /// <summary>
        /// �������� � ������, ���� �������� �� ������
        /// </summary>
        protected enum DataSourceNotFoundAction
        {
            /// <summary>
            /// ������� ��������
            /// </summary>
            CreateDataSource,

            /// <summary>
            /// ������� null ��� -1 � ����������� �� ����
            /// </summary>
            ReturnNull,

            /// <summary>
            /// ������������ ����������
            /// </summary>
            ThrowException
        }

        /// <summary>
        /// ���� �������� ������ �� ��� ����������
        /// </summary>
        /// <param name="parametersType">��� ���������</param>
        /// <param name="supplierCode">��� ����������</param>
        /// <param name="dataCode">��� ������</param>
        /// <param name="budgetName">������������ �������</param>
        /// <param name="year">���</param>
        /// <param name="month">�����</param>
        /// <param name="variant">�������</param>
        /// <param name="quarter">�������</param>
        /// <returns>�� ���������</returns>
        protected int FindDataSourceID(ParamKindTypes parametersType, string supplierCode, string dataCode,
            string budgetName, int year, int month, string variant, int quarter, string territory,
            DataSourceNotFoundAction dataSourceNotFoundAction)
        {
            IDataSource ds = FindDataSource(parametersType, supplierCode, dataCode, budgetName, year, month, variant,
                quarter, territory);

            if (ds == null)
            {
                switch (dataSourceNotFoundAction)
                {
                    case DataSourceNotFoundAction.ReturnNull: return -1;

                    case DataSourceNotFoundAction.CreateDataSource:
                    case DataSourceNotFoundAction.ThrowException:
                        throw new Exception("�� ������ �������� " +
                            GetDataSourceDescription(parametersType, supplierCode, dataCode, budgetName, year, month, variant,
                                quarter, territory));
                }
            }

            return ds.ID;
        }

        /// <summary>
        /// ��������� ���� ��� ��������� �� ���������� ���������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <returns>����</returns>
        private string GetCacheKeyByDataSource(int sourceID)
        {
            IDataSource ds = this.scheme.DataSourceManager.DataSources[sourceID];
            if (ds == null)
                return string.Empty;

            int totalWidthForKey = 100;
            string result = ds.Year.ToString().PadLeft(totalWidthForKey, '0');

            switch (ds.ParametersType)
            {
                case ParamKindTypes.Budget:
                    result += ds.BudgetName.PadLeft(totalWidthForKey, ' ');
                    break;

                case ParamKindTypes.YearMonth:
                    result += ds.Month.ToString().PadLeft(totalWidthForKey, '0');
                    break;

                case ParamKindTypes.YearMonthVariant:
                    result += ds.Month.ToString().PadLeft(totalWidthForKey, '0') +
                        ds.Variant.PadLeft(totalWidthForKey, ' ');
                    break;

                case ParamKindTypes.YearQuarter:
                    result += ds.Quarter.ToString().PadLeft(totalWidthForKey, '0');
                    break;

                case ParamKindTypes.YearQuarterMonth:
                    result += ds.Quarter.ToString().PadLeft(totalWidthForKey, '0') +
                        ds.Month.ToString().PadLeft(totalWidthForKey, '0');
                    break;

                case ParamKindTypes.YearTerritory:
                    result += ds.Territory.PadLeft(totalWidthForKey, ' ');
                    break;

                case ParamKindTypes.YearVariant:
                    result += ds.Variant.PadLeft(totalWidthForKey, ' ');
                    break;

                case ParamKindTypes.Variant:
                    result = ds.Variant.PadLeft(totalWidthForKey, ' ');
                    break;

                case ParamKindTypes.YearVariantMonthTerritory:
                    result += ds.Variant.PadLeft(totalWidthForKey, ' ') +
                        ds.Month.ToString().PadLeft(totalWidthForKey, '0') +
                        ds.Territory.PadLeft(totalWidthForKey, ' ');
                    break;
            }

            return result;
        }

        /// <summary>
        /// ��������� ������ ���������� ���������� �� ����
        /// </summary>
        protected void SortDataSources(ref Dictionary<int, string> dataSources)
        {
            if (dataSources.Count == 0)
                return;

            SortedList<string, int> st = new SortedList<string, int>(dataSources.Count);

            foreach (KeyValuePair<int, string> kvp in dataSources)
            {
                string key = GetCacheKeyByDataSource(kvp.Key);
                if (!st.ContainsKey(key))
                {
                    st.Add(key, kvp.Key);
                }
                else
                {
              //      throw new Exception(string.Format(
               //         "�������� � �����������, ������������ ��������� {0}, ��� ������������ � ���������",
                //        kvp.Key));
                }
            }

            Dictionary<int, string> tmp = new Dictionary<int, string>(dataSources.Count);
            foreach (KeyValuePair<string, int> kvp in st)
            {
                int value = kvp.Value;
                if (!tmp.ContainsKey(value))
                {
                    tmp.Add(value, dataSources[kvp.Value]);
                }
                else
                {
                 //   throw new Exception(string.Format("�������� � ID {0} ��� ������������ � ���������", value));
                }
            }

            dataSources.Clear();
            dataSources = tmp;
        }

        #endregion ����� � ���������� ����������


        #region ��������� ���������� �� ����������

        /// <summary>
        /// ������ ��� �������������� ������ ���� ��������� ������ � ��������
        /// </summary>
        protected string[] KindsOfParamsByNumber = new string[] { 
            "���������� �����, ���", "���", "���, �����", "���, �����, �������", "���, �������", 
            "���, �������", "���, ����������", "��� ������� �����", "��� ����������", "�������",
            "���, �����, ����������", "���, �������, ����������", "���, �������, �����, ����������" };

        /// <summary>
        /// ���������� �������� ��������� �� ��� ����������
        /// </summary>
        /// <param name="parametersType">��� ���������</param>
        /// <param name="supplierCode">��� ����������</param>
        /// <param name="dataCode">��� ������</param>
        /// <param name="budgetName">������������ �������</param>
        /// <param name="year">���</param>
        /// <param name="month">�����</param>
        /// <param name="variant">�������</param>
        /// <param name="quarter">�������</param>
        /// <returns>��������</returns>
        protected string GetDataSourceDescription(ParamKindTypes parametersType, string supplierCode, string dataCode,
            string budgetName, int year, int month, string variant, int quarter, string territory)
        {
            string result = string.Format(
                "��� ����������: {0}; ��� ������: {1}; ��� ���������: {2}",
                supplierCode, dataCode, KindsOfParamsByNumber[(int)parametersType]);

            switch (parametersType)
            {
                case ParamKindTypes.Budget:
                    result += string.Format("; ���������� �����: {0}; ���: {1}", budgetName, year);
                    break;

                case ParamKindTypes.Year:
                    result += string.Format("; ���: {0}", year);
                    break;

                case ParamKindTypes.YearMonth:
                    result += string.Format("; ���: {0}; �����: {1}", year, month);
                    break;

                case ParamKindTypes.YearMonthVariant:
                    result += string.Format("; ���: {0}; �����: {1}; �������: {2}", year, month, variant);
                    break;

                case ParamKindTypes.YearQuarter:
                    result += string.Format("; ���: {0}; �������: {1}", year, quarter);
                    break;

                case ParamKindTypes.YearQuarterMonth:
                    result += string.Format("; ���: {0}; �������: {1}; �����: {2}", year, quarter, month);
                    break;

                case ParamKindTypes.YearTerritory:
                    result += string.Format("; ���: {0}; ����������: {1}", year, territory);
                    break;

                case ParamKindTypes.YearVariant:
                    result += string.Format("; ���: {0}; �������: {1}", year, variant);
                    break;

                case ParamKindTypes.Variant:
                    result += string.Format("; �������: {0}", variant);
                    break;

                case ParamKindTypes.YearMonthTerritory:
                    result += string.Format("; ���: {0};  �����: {1}; ����������: {2}", year, month, territory);
                    break;

                case ParamKindTypes.YearQuarterTerritory:
                    result += string.Format("; ���: {0};  �������: {1}; ����������: {2}", year, quarter, territory);
                    break;

                case ParamKindTypes.YearVariantMonthTerritory:
                    result += string.Format("; ���: {0}; �������: {1}; �����: {2}; ����������: {3}", year, variant, month, territory);
                    break;
            }

            return result;
        }

        /// <summary>
        /// ���������� �������� ��������� �� ��� ����������
        /// </summary>
        /// <param name="sourceID">ID ���������</param>
        /// <returns>��������</returns>
        protected string GetDataSourceDescription(int sourceID)
        {
            IDataSource ds = GetDataSourceBySourceID(sourceID);

            return GetDataSourceDescription(ds.ParametersType, ds.SupplierCode, ds.DataCode,
                ds.BudgetName, ds.Year, ds.Month, ds.Variant, ds.Quarter, ds.Territory);
        }

        /// <summary>
        /// ��������� ���� � ��������� �� ������ �� ��������� ���������� ������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <returns>����</returns>
        protected string GetSourcePathBySourceID(int sourceID)
        {
            return string.Format(
                "{0}{1}",
                Scheme.DataSourceManager.BaseDirectory,
                GetShortSourcePathBySourceID(sourceID));
        }

        /// <summary>
        /// ��������� ���� � ��������� �� ������ �� ��������� ���������� ������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <returns>����</returns>
        protected DirectoryInfo GetSourceDirBySourceID(int sourceID)
        {
            return new DirectoryInfo(GetSourcePathBySourceID(sourceID));
        }

        /// <summary>
        /// ��������� ����������� ���� � ��������� �� ������ �� ��������� ���������� ������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <returns>����</returns>
        protected string GetShortSourcePathBySourceID(int sourceID)
        {
            IDataSource ds = Scheme.DataSourceManager.DataSources[sourceID];
            if (ds == null) return string.Empty;

            string result = string.Format(
                "\\{0}\\{1}_{2}",
                ds.SupplierCode, ds.DataCode.PadLeft(4, '0'), ds.DataName);

            if (ds.BudgetName != string.Empty)
                result += string.Format("\\{0}", ds.BudgetName);
            if (ds.Year != 0)
                result += string.Format("\\{0}", ds.Year);
            if (ds.Month != 0)
                result += string.Format("\\{0}", ds.Month);
            if (ds.Variant != string.Empty)
                result += string.Format("\\{0}", ds.Variant);
            if (ds.Quarter != 0)
                result += string.Format("\\{0}", ds.Quarter);
            if (ds.Territory != string.Empty)
                result += string.Format("\\{0}", ds.Territory);

            return result;
        }

        /// <summary>
        /// ��������� ����������� ���� � ��������� �� ������ �� ��������� ���������� ������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <returns>����</returns>
        protected DirectoryInfo GetShortSourceDirBySourceID(int sourceID)
        {
            return new DirectoryInfo(GetShortSourcePathBySourceID(sourceID));
        }

        /// <summary>
        /// ���������� ���� ��������� ������� �� ������� ���������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <returns>����</returns>
        protected string GetLastPumpDateBySourceID(int sourceID)
        {
            if (sourceID < 0) return string.Empty;

            DataTable dt = (DataTable)this.DB.ExecQuery(string.Format(
                "select EVENTDATETIME from DATAPUMPPROTOCOL where DATASOURCEID = {0} and KINDSOFEVENTS = 104 " +
                "order by PUMPHISTORYID desc, EVENTDATETIME asc", sourceID), QueryResultTypes.DataTable);
            if (dt.Rows.Count == 0)
            {
                return string.Empty;
            }

            return Convert.ToString(dt.Rows[0]["EVENTDATETIME"]);
        }

        /// <summary>
        /// ���������� ������ ���� �����-���� ���������� ������ �������� ����������
        /// </summary>
        /// <param name="pumpID">�� �������. ���� -1, �� �� �����������. ���� >0, �� ���������� ��������� �� �����
        /// �� �������</param>
        /// <returns>������ ����������</returns>
        protected Dictionary<int, string> GetAllPumpedDataSources(int pumpID)
        {
            Dictionary<int, string> result = new Dictionary<int, string>(100);
            DataTable dt;

            if (pumpID < 0)
            {
                dt = this.PumpRegistryElement.DataSources;
            }
            else
            {
                dt = this.DB.ExecQuery(
                    "select REFDATASOURCES as ID from DATASOURCES2PUMPHISTORY where REFPUMPHISTORY = ?",
                    QueryResultTypes.DataTable,
                    this.DB.CreateParameter("REFPUMPHISTORY", pumpID)) as DataTable;
            }

            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (Convert.ToInt32(dt.Rows[i]["Deleted"]) != 0)
                    continue;
                result.Add(Convert.ToInt32(row["ID"]), string.Empty);
            }

            return result;
        }

        /// <summary>
        /// ���������� ������ ���� �����-���� ���������� ������ �������� ����������
        /// </summary>
        /// <returns>������ ����������</returns>
        protected Dictionary<int, string> GetAllPumpedDataSources()
        {
            return GetAllPumpedDataSources(-1);
        }

        /// <summary>
        /// ���������� ��������� ��������� �� ��� ��
        /// </summary>
        /// <param name="id">��</param>
        /// <returns>��������</returns>
        protected IDataSource GetDataSourceBySourceID(int id)
        {
            return this.Scheme.DataSourceManager.DataSources[id];
        }

        /// <summary>
        /// ������� ��������� �� ������� ����������� �� ���������� � ����������� ����������.
        /// </summary>
        /// <returns>���������</returns>
        private string MakeDataSourcesVault()
        {
            string result = string.Empty;

            for (int i = 0; i < this.ProcessedSources.Keys.Count; i++)
            {
                result += string.Format(
                    "�������� {0}: {1} \n",
                    this.ProcessedSources.Keys[i],
                    this.ProcessedSources[this.ProcessedSources.Keys[i]]);
            }
            if (result != string.Empty)
            {
                result = "���������� ��������� ����������: \n" + result;
            }

            return result;
        }

        /// <summary>
        /// ��������� ��������� ��������� �� ������������ ��������� ����
        /// </summary>
        /// <param name="date">���� YYYYMMDD</param>
        /// <param name="generateException">������������ ���������� ��� �������������� ��� ���</param>
        protected bool CheckDataSourceByDate(int date, bool generateException)
        {
            int year = -1;
            int month = -1;
            int day = -1;
            CommonRoutines.DecodeNewDate(date, out year, out month, out day);

            if (this.DataSource.Year != year && (this.DataSource.ParametersType != ParamKindTypes.NoDivide))
            {
                if (generateException)
                    throw new Exception(string.Format("���� {0} �� ������������� ���������� ���������", date));
                else
                    return false;
            }

            if (this.DataSource.Month != month && (this.DataSource.ParametersType == ParamKindTypes.YearMonth ||
                this.DataSource.ParametersType == ParamKindTypes.YearMonthVariant))
            {
                if (generateException)
                {
                    throw new Exception(string.Format("���� {0} �� ������������� ���������� ���������", date));
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        #endregion ��������� ���������� �� ����������
    }
}