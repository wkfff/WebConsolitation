using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;


namespace Krista.FM.Server.DataPumps.DataAccess
{
    /// <summary>
    /// �������� ������� 1�.
    /// ��������� ��������:
    /// ���� - ��� �����, �������� - ������ �����.
    /// ��������� ������ �����:
    /// ���� - ��� ����, �������� - ����� ������� ����.
    /// </summary>
    public sealed class Format1nDescription : Dictionary<string, Dictionary<string, int>> { }


    /// <summary>
    /// ������ ������ ������� 1� � ������������� ���� - ������ ������ ����� ������� � ���� ������.
    /// ��������� ��������:
    /// ���� - ��� �����, �������� - ������ ����� (������ ���� ����� �� ���������� �����).
    /// ��������� ������ �����:
    /// ���� - ��� ����, �������� - �������� ����.
    /// </summary>
    public sealed class Format1nHierarchicalData : Dictionary<string, List<Dictionary<string, string>>> { }


    /// <summary>
    /// ������ ����� ����� ������� 1�
    /// </summary>
    public struct Format1nBlock
    {
        /// <summary>
        /// ��� �����
        /// </summary>
        public string BlockName;

        /// <summary>
        /// ������ �����.
        /// ��������� ������ �����:
        /// ���� - ��� ����, �������� - �������� ����.
        /// </summary>
        public Dictionary<string, string> BlockData;
    }


    /// <summary>
    /// ������ ������ ������� 1� � �������� ���� - � ��� ������������������, ����� ��������� � �����.
    /// ��������� ��������:
    /// ���� - ��� �����, �������� - ������ ����� (������ ���� ����� �� ���������� �����).
    /// ��������� ������ �����:
    /// ���� - ��� ����, �������� - �������� ����.
    /// </summary>
    public sealed class Format1nLinearData : List<Format1nBlock> { }


    /// <summary>
    /// ������ 1�
    /// </summary>
    public enum Format1n
    {
        /// <summary>
        /// ��������� �������� �����������
        /// </summary>
        VKP,

        /// <summary>
        /// ������ ������������� ����������� 
        /// </summary>
        DPR
    }


    /// <summary>
    /// ����� ������� �������� 1�
    /// </summary>
    public sealed class Formats1nParser : DisposableObject
    {
        #region ����

        private Format1nDescription vkpDescription;
        private Format1nDescription dprDescription;
        
        #endregion ����


        #region �������������

        /// <summary>
        /// ������������ ��������
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (vkpDescription != null) vkpDescription.Clear();
                if (dprDescription != null) dprDescription.Clear();
            }

            base.Dispose(disposing);
        }

        #endregion �������������


        #region �������� ������

        /// <summary>
        /// �������� ������� "��������� �������� ����������� � ������ (����������/�����������)"
        /// </summary>
        public Format1nDescription VkpDescription
        {
            get 
            {
                // ���� �������� ��� �� ��������� - ���������
                if (vkpDescription == null)
                {
                    vkpDescription = new Format1nDescription();

                    // �����
                    vkpDescription.Add("VKP", new Dictionary<string, int>(100));
                    vkpDescription.Add("VKPSTBK", new Dictionary<string, int>(100));

                    // �������� ��������� ������
                    vkpDescription["VKP"].Add("DATE_VED", 3);

                    vkpDescription["VKPSTBK"].Add("DEB_SUM", 11);
                    vkpDescription["VKPSTBK"].Add("CRED_SUM", 12);
                    vkpDescription["VKPSTBK"].Add("KOD_DOH", 8);
                }

                return vkpDescription; 
            }
        }

        /// <summary>
        /// �������� ������� "������ ������������� �����������"
        /// </summary>
        public Format1nDescription DprDescription
        {
            get
            {
                // ���� �������� ��� �� ��������� - ���������
                if (dprDescription == null)
                {
                    dprDescription = new Format1nDescription();

                    // �����
                    dprDescription.Add("FK", new Dictionary<string, int>(10));
                    dprDescription.Add("DP", new Dictionary<string, int>(100));
                    dprDescription.Add("DPR", new Dictionary<string, int>(100));

                    // �������� ��������� ������
                    dprDescription["FK"].Add("NUM_VER", 1);

                    dprDescription["DP"].Add("NAME_FO", 1);
                    dprDescription["DP"].Add("DP_DATE", 5);
                    dprDescription["DP"].Add("POL_ACC", 11);

                    dprDescription["DPR"].Add("STR_SUM", 1);
                    dprDescription["DPR"].Add("KOD_DOH", 2);
                }

                return dprDescription;
            }
        }

        #endregion �������� ������


        #region ������� ������

        /// <summary>
        /// ���������� ������ ����� ������� 1�.
        /// ������ ������ ������� 1� � ������������� ���� - ������ ������ ����� ������� � ���� ������.
        /// </summary>
        /// <param name="file">����</param>
        /// <param name="format1n">������ �����</param>
        /// <returns>������ �����</returns>
        public Format1nHierarchicalData ParseHierarchicalFile(FileInfo file, Format1n format1n)
        {
            if (!file.Exists) return null;

            // ������� �������� ������� ����� � ��������� ���
            return GetHierarchicalDataFromFile(file, GetFormatDescription(format1n));
        }

        /// <summary>
        /// ���������� ������ ����� ������� 1�.
        /// ������ ������ ������� 1� � �������� ���� - � ��� ������������������, ����� ��������� � �����.
        /// </summary>
        /// <param name="file">����</param>
        /// <param name="format1n">������ �����</param>
        /// <returns>������ �����</returns>
        public Format1nLinearData ParseLinearFile(FileInfo file, Format1n format1n)
        {
            if (!file.Exists) return null;

            // ������� �������� ������� ����� � ��������� ���
            return GetLinearDataFromFile(file, GetFormatDescription(format1n));
        }

        /// <summary>
        /// ���������� �������� ������� �� ��� �����
        /// </summary>
        /// <param name="format1n">������������ �������</param>
        /// <returns>��������</returns>
        private Format1nDescription GetFormatDescription(Format1n format1n)
        {
            switch (format1n)
            {
                case Format1n.VKP: return this.VkpDescription;

                case Format1n.DPR: return this.DprDescription;
            }

            return null;
        }

        /// <summary>
        /// ������������� ��������� ������ ����� ���������� �������
        /// </summary>
        /// <param name="format1nData">������ �����</param>
        /// <param name="formatDescription">�������� �������</param>
        private void InitFormat1nData(ref Format1nHierarchicalData format1nData, Format1nDescription formatDescription)
        {
            // ��������� ��������� format1nData ������� �������
            if (format1nData != null) format1nData.Clear();
            format1nData = new Format1nHierarchicalData();

            foreach (KeyValuePair<string, Dictionary<string, int>> kvp in formatDescription)
            {
                List<Dictionary<string, string>> list = new List<Dictionary<string,string>>(1000);
                //list.Add(new Dictionary<string, string>(blockDescription.Count));

                format1nData.Add(kvp.Key, list);

                //Dictionary<string, int> fields = kvp.Value;
                //foreach (KeyValuePair<string, int> fieldsKvp in fields)
                //{
                //    list[0].Add(fieldsKvp.Key);
                //}
            }
        }

        /// <summary>
        /// ������ ���� � ���������� ��������� � ��� �������
        /// </summary>
        /// <param name="file">����</param>
        /// <param name="formatDescription">�������� �������</param>
        /// <returns>������ �����</returns>
        private Format1nHierarchicalData GetHierarchicalDataFromFile(FileInfo file, Format1nDescription formatDescription)
        {
            Format1nHierarchicalData result = null;
            InitFormat1nData(ref result, formatDescription);

            // ��������� ���� � ������
            string[] fileContent = CommonRoutines.GetFileContent(file, Encoding.GetEncoding(1251));

            // ������� ������ � ������� ����� � ��������� �� �������� �������� �������
            int count = fileContent.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                // ������ �����
                string[] fields = fileContent[i].Split('|');
                if (formatDescription.ContainsKey(fields[0]))
                {
                    // �������� ������ �����
                    Dictionary<string, int> blockDescription = formatDescription[fields[0]];

                    // ��������� � ������ ������ ����� ��� ���� ������
                    List<Dictionary<string, string>> blockData = result[fields[0]];
                    Dictionary<string, string> blockRow = new Dictionary<string, string>(blockDescription.Count);
                    blockData.Add(blockRow);

                    foreach (KeyValuePair<string, int> kvp in blockDescription)
                    {
                        blockRow.Add(kvp.Key, fields[kvp.Value]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// ������ ���� � ���������� ��������� � ��� �������
        /// </summary>
        /// <param name="file">����</param>
        /// <param name="formatDescription">�������� �������</param>
        /// <returns>������ �����</returns>
        private Format1nLinearData GetLinearDataFromFile(FileInfo file, Format1nDescription formatDescription)
        {
            Format1nLinearData result = new Format1nLinearData();
            result.Capacity = 2000;

            // ��������� ���� � ������
            string[] fileContent = CommonRoutines.GetFileContent(file, Encoding.GetEncoding(1251));

            // ������� ������ � ������� ����� � ��������� �� �������� �������� �������
            int count = fileContent.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                // ������ �����
                string[] fields = fileContent[i].Split('|');
                if (formatDescription.ContainsKey(fields[0]))
                {
                    // �������� ������ �����
                    Dictionary<string, int> blockDescription = formatDescription[fields[0]];

                    // ��������� � ������ ������ ��� ���� ����
                    Format1nBlock block = new Format1nBlock();
                    block.BlockName = fields[0];
                    Dictionary<string, string> blockRow = new Dictionary<string, string>(blockDescription.Count);
                    block.BlockData = blockRow;
                    result.Add(block);

                    foreach (KeyValuePair<string, int> kvp in blockDescription)
                    {
                        blockRow.Add(kvp.Key, fields[kvp.Value]);
                    }
                }
            }

            return result;
        }

        #endregion ������� ������
    }
}