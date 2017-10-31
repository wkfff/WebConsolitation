using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Server
{
    public class ReportsService
    {
        private IScheme scheme;

        public ReportsService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        /// <summary>
        /// получение отчетов из каталога 'системные отчеты' 
        /// </summary>
        /// <returns></returns>
        public DataTable GetSystemReportsStructure()
        {
            // составляем список отчетов, которые подчинены главному
            return GetReports("SystemReports");
        }

        /// <summary>
        /// получение отчетов из любого каталога по его коду
        /// </summary>
        /// <param name="topDirectoryCode"></param>
        /// <returns></returns>
        public DataTable GetReports(string topDirectoryCode)
        {
            DataTable dtAllReports = scheme.TemplatesService.Repository.GetTemplatesInfo(
                TemplateTypes.System);

            DataRow[] rows = dtAllReports.Select(string.Format("Code = '{0}'", topDirectoryCode));
            // нет отчетов системы, выходим, возвращая пустой список
            if (rows == null || rows.Length == 0)
                return dtAllReports;

            rows[0]["ParentID"] = DBNull.Value;
            DataTable dt = dtAllReports.Clone();
            FillReports(dtAllReports, rows[0], ref dt);
            return dt;
        }

        private void FillReports(DataTable dtAllReports, DataRow parentRow, ref DataTable dtReports)
        {
            dtReports.Rows.Add(parentRow.ItemArray);
            foreach (DataRow childRow in dtAllReports.Rows.Cast<DataRow>().
                Where(w => (!w.IsNull("ParentID") && Convert.ToInt32(w["ParentID"]) == Convert.ToInt32(parentRow["ID"]))))
            {
                //dtReports.Rows.Add(childRow.ItemArray);
                FillReports(dtAllReports, childRow, ref dtReports);
            }
        }

        /// <summary>
        /// получение документа отчета в виде бинарных данных
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public byte[] GetReportData(int reportId)
        {
            byte[] reportData = scheme.TemplatesService.Repository.GetDocument(reportId);
            return DecompressFile(reportData);
        }

        private static byte[] DecompressFile(byte[] compressedFileBufer)
        {
            MemoryStream ms = null;
            DeflateStream decompressedzipStream = null;
            try
            {
                ms = new MemoryStream(compressedFileBufer);
                ms.Write(compressedFileBufer, 0, compressedFileBufer.Length);
                decompressedzipStream = new DeflateStream(ms, CompressionMode.Decompress, true);
                ms.Position = 0;
                List<byte> list = new List<byte>();
                ReadAllBytesFromStream(decompressedzipStream, list);
                return list.ToArray();
            }
            finally
            {
                decompressedzipStream.Close();
            }
        }

        private static int ReadAllBytesFromStream(Stream stream, List<byte> list)
        {
            try
            {
                int offset = 0;
                int totalCount = 0;
                while (true)
                {
                    int byteRead = stream.ReadByte();

                    if (byteRead == -1)
                        break;
                    list.Add((byte)byteRead);
                    offset++;
                    totalCount++;
                }
                return totalCount;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
