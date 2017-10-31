using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    /// <summary>
    /// ��� ��������� � ���������� ��������� ���������
    /// </summary>
    public enum DataSourceProcessingResult
    {
        /// <summary>
        /// �������� ������� ���������
        /// </summary>
        SuccessfulProcessed,

        /// <summary>
        /// �������� ���������, �� ����� ��������� ���� ��������������
        /// </summary>
        ProcessedWithWarnings,

        /// <summary>
        /// �������� ���������, �� ����� ��������� ���� ������
        /// </summary>
        ProcessedWithErrors
    }


    /// <summary>
    /// ��������� � ������� ���������������� ��������� ���������
    /// </summary>
    public class PreviewDataResult : DisposableObject
    {

    }

    /// <summary>
    /// ��������� ��������� ��������� ������
    /// </summary>
    public class DataSourceProcessingSettings : DisposableObject
    {
        #region ����

        private string queryConstraint;

        #endregion ����


        #region �������������

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="queryConstr">����������� �������</param>
        public DataSourceProcessingSettings(string queryConstr)
        {
            this.queryConstraint = queryConstr;
        }

        #endregion �������������


        #region �������� ������

        /// <summary>
        /// ����������� ������� ������ ���������
        /// </summary>
        public string QueryConstraint
        {
            get
            {
                return queryConstraint;
            }
            set
            {
                queryConstraint = value;
            }
        }

        #endregion �������� ������
    }


    /// <summary>
    /// ���������� � ����������� ��������� ���������� ������ �������
    /// </summary>
    public sealed class DataSourcesProcessingResult : DisposableObject
    {
        #region ����

        private Dictionary<int, DataSet> previewDataSources = new Dictionary<int, DataSet>(20);
        private Dictionary<int, string> pumpedSources = new Dictionary<int, string>(20);
        private SortedList<int, string> processedSources = new SortedList<int, string>(20);
        private Dictionary<int, DataSourceProcessingSettings> dataSourcesProcessingSettings =
            new Dictionary<int, DataSourceProcessingSettings>(20);
        private IScheme scheme;

        #endregion ����


        #region �������������

        /// <summary>
        /// �����������
        /// </summary>
        public DataSourcesProcessingResult(IScheme scheme)
        {
            this.scheme = scheme;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (previewDataSources != null)
                    previewDataSources.Clear();

                if (pumpedSources != null)
                    pumpedSources.Clear();

                if (processedSources != null)
                    processedSources.Clear();

                if (dataSourcesProcessingSettings != null)
                {
                    dataSourcesProcessingSettings.Clear();
                    dataSourcesProcessingSettings = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion �������������


        #region �������� ������

        /// <summary>
        /// ������ ��������� � ������������� ������� ���������� ��� ������������� � ����������� �������
        /// </summary>
        public Dictionary<int, DataSet> PreviewDataSources
        {
            get
            {
                return previewDataSources;
            }
        }

        /// <summary>
        /// ������ ���������� ����������.
        /// ���� - �� ���������, �������� - ��������� � �����������
        /// </summary>
        public Dictionary<int, string> PumpedSources
        {
            get
            {
                return pumpedSources;
            }
        }

        /// <summary>
        /// ������ ������������ ����������.
        /// ���� - �� ���������, �������� - ��������� � ����������� ������� ���������
        /// </summary>
        public SortedList<int, string> ProcessedSources
        {
            get
            {
                return processedSources;
            }
        }

        /// <summary>
        /// ��������� ��������� ���������� ������
        /// ���� - �� ���������, �������� - ��������� ��������� ��������� ������
        /// </summary>
        public Dictionary<int, DataSourceProcessingSettings> DataSourcesProcessingSettings
        {
            get
            {
                return dataSourcesProcessingSettings;
            }
        }

        #endregion �������� ������


        #region ������ ������

        /// <summary>
        /// ��������� ������ � ������ ���������������� ��������� ������ ����������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="ds">������� � ������� ���������</param>
        public void AddToPreviewDataSources(int sourceID, DataSet ds)
        {
            if (!previewDataSources.ContainsKey(sourceID))
            {
                previewDataSources.Add(sourceID, ds);
            }
            else
            {
                previewDataSources[sourceID] = ds;
            }
        }

        /// <summary>
        /// ��������� ������ � ������ ���������� ����������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="comment">��������� � �����������</param>
        public void AddToPumpedSources(int sourceID, string comment)
        {
            if (!pumpedSources.ContainsKey(sourceID))
            {
                pumpedSources.Add(sourceID, comment);
            }
        }

        /// <summary>
        /// ��������� ������ � ������ ������������ ����������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="msg">���������</param>
        public void AddToProcessedSources(int sourceID, DataSourceProcessingResult msg)
        {
            string message = string.Empty;

            switch (msg)
            {
                case DataSourceProcessingResult.ProcessedWithErrors: message = "��������� � ��������";
                    break;

                case DataSourceProcessingResult.ProcessedWithWarnings: message = "��������� � ����������������";
                    break;

                case DataSourceProcessingResult.SuccessfulProcessed: message = "��������� �������";
                    break;
            }

            if (processedSources.ContainsKey(sourceID))
            {
                processedSources[sourceID] = message;
            }
            else
            {
                processedSources.Add(sourceID, message);
            }
        }

        /// <summary>
        /// ��������� ������ � ������ ������������ ����������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="errMsg">��������� �� ������. ������ ������ - ������� ���������</param>
        public void AddToProcessedSources(int sourceID, string errMsg)
        {
            string message = string.Empty;

            if (errMsg == string.Empty)
            {
                message = "������� ���������";
            }
            else
            {
                message = errMsg;
            }

            if (processedSources.ContainsKey(sourceID))
            {
                processedSources[sourceID] = message;
            }
            else
            {
                processedSources.Add(sourceID, message);
            }
        }

        /// <summary>
        /// ��������� ������ � ������ ��������� ��������� ���������� ������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="errMsg">��������� �� ������. ������ ������ - ������� ���������</param>
        public void AddToDataSourcesProcessingSettings(int sourceID, DataSourceProcessingSettings settings)
        {
            if (!dataSourcesProcessingSettings.ContainsKey(sourceID))
            {
                dataSourcesProcessingSettings.Add(sourceID, settings);
            }
        }

        #endregion ������ ������
    }
}