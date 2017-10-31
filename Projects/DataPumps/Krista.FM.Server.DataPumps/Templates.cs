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
    // ������ �������� �������� ����������� �������

    /// <summary>
    /// ������� ����� ��� ���� �������.
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject
    {
        /// <summary>
        /// ������� ��������� ����� ������
        /// </summary>
        /// <param name="fileInfo">����</param>
        protected delegate void ProcessFileDelegate(FileInfo fileInfo);

        /// <summary>
        /// ������ ������� ��������� ������ ��������� ������
        /// </summary>
        /// <param name="dir">������� ���������</param>
        /// <param name="searchPattern">����� ������</param>
        /// <param name="emptyDirException">�������� ���������� ��� ������ �������� ��� ���</param>
        /// <param name="searchOption">�������� ������ ������: �������� ��������� �������� ��� ���</param>
        protected void ProcessFilesTemplate(DirectoryInfo dir, string searchPattern, 
            ProcessFileDelegate processFile, bool emptyDirException, SearchOption searchOption)
        {
            WriteToTrace(String.Format("������ ������ ������ �� ����� {0}", searchPattern), TraceMessageKind.Information);
            FileInfo[] files = dir.GetFiles(searchPattern, searchOption);
            if (files.GetLength(0) == 0 && emptyDirException)
            {
                throw new Exception("� �������� ��������� ��� ������ ��� �������");
            }

            string sourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int totalFiles = files.GetLength(0);

            // ������������ �����
            for (int i = 0; i < totalFiles; i++)
            {
                SetProgress(totalFiles, i + 1,
                    string.Format("��������� ����� {0}\\{1}...", sourcePath, files[i].Name),
                    string.Format("���� {0} �� {1}", i + 1, totalFiles), true);

                if (!files[i].Exists)
                    continue;

                if (files[i].Directory.Name.StartsWith("__"))
                    continue;

                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                    string.Format("����� ������� ����� {0}.", files[i].FullName));

                try
                {
                    processFile(files[i]);

                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump,
                        string.Format("������� ����� {0} ������� ���������.", files[i].FullName));
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError,
                        string.Format("������� ����� {0} ��������� � ��������.", files[i].FullName), ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// ������ ������� ��������� ������ ��������� ������
        /// </summary>
        /// <param name="dir">������� ���������</param>
        /// <param name="searchPattern">����� ������</param>
        protected void ProcessFilesTemplate(DirectoryInfo dir, string searchPattern,
            ProcessFileDelegate processFile)
        {
            ProcessFilesTemplate(dir, searchPattern, processFile, true);
        }

        /// <summary>
        /// ������ ������� ��������� ������ ��������� ������
        /// </summary>
        /// <param name="dir">������� ���������</param>
        /// <param name="searchPattern">����� ������</param>
        /// <param name="emptyDirException">�������� ���������� ��� ������ �������� ��� ���</param>
        protected void ProcessFilesTemplate(DirectoryInfo dir, string searchPattern,
            ProcessFileDelegate processFile, bool emptyDirException)
        {
            ProcessFilesTemplate(dir, searchPattern, processFile, emptyDirException, SearchOption.AllDirectories);
        }

        /// <summary>
        /// ������� ������� ��������� ���������� ������
        /// </summary>
        /// <param name="row">������ � ������� ��� ���������</param>
        protected delegate void DataPartRowProcessing(DataRow row);

        /// <summary>
        /// ������ ������� ��������� ������� ������� ������ �� ������
        /// </summary>
        /// <param name="obj">������ �������</param>
        /// <param name="constr">����������� ��� �������</param>
        /// <param name="constMaxQueryRecords">�������� ���������� ����� ��� �������</param>
        /// <param name="dataPartProcessing">������� ������� ��������� ���������� ������</param>
        protected void PartialDataProcessingTemplate(IEntity obj, string constr, int constMaxQueryRecords,
            DataPartRowProcessing dataPartProcessing, string message)
        {
            IDbDataAdapter da = null;
            DataSet ds = null;

            if (!string.IsNullOrEmpty(constr))
                constr += " and ";

            constr += string.Format("SOURCEID = {0}", this.SourceID);

            try
            {
                int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                    "select count(id) from {0} where {1}", obj.FullDBName, constr), QueryResultTypes.Scalar));
                if (totalRecs == 0)
                {
                    WriteToTrace(string.Format("��� ������ {0} ��� ��������� �� �������� ���������.", obj.FullCaption), TraceMessageKind.Warning);
                    return;
                }

                int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                    "select min(id) from {0} where {1}", obj.FullDBName, constr), QueryResultTypes.Scalar));
                int lastID = firstID + constMaxQueryRecords - 1;
                int processedRecCount = 0;

                do
                {
                    // ����������� ������� ��� ������� ������ ������
                    string idConstr = string.Format("ID >= {0} and ID <= {1} and {2}", firstID, lastID, constr);
                    firstID = lastID + 1;
                    lastID += constMaxQueryRecords;

                    InitDataSet(ref da, ref ds, obj, idConstr);
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count == 0)
                    {
                        continue;
                    }

                    int recCount = dt.Rows.Count;

                    // ������������� ������������
                    for (int i = 0; i < recCount; i++)
                    {
                        processedRecCount++;
                        SetProgress(totalRecs, processedRecCount,
                            string.Format("{0} ��� ��������� {1}...", message, this.SourceID),
                            string.Format("������ {0} �� {1}", processedRecCount, totalRecs));

                        DataRow row = dt.Rows[i];

                        dataPartProcessing(row);
                    }

                    UpdateDataSet(da, ds, obj);
                }
                while (processedRecCount < totalRecs);

                UpdateDataSet(da, ds, obj);
            }
            finally
            {
                ClearDataSet(ref ds);
            }
        }
    }
}