using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    /// <summary>
    /// ������� ����� ��� �������, ���������� �������������� ��������� �������� ��������������� � 
    /// ��������� ���� �� �������� ��������������� �� ����� ��������� ������.
    /// </summary>
    public abstract partial class CorrectedPumpModuleBase : DataPumpModuleBase
    {
        #region ���������, ������������

        /// <summary>
        /// ����������� ��������� ��������������. ����� ��� ��������� ������� ��������� ����� ������������� 
        /// �������������� � ��� � ��� ������
        /// </summary>
        protected enum ClsProcessModifier
        {
            /// <summary>
            /// ���.�������
            /// </summary>
            EKR,

            /// <summary>
            /// ���.�������
            /// </summary>
            FKR,

            /// <summary>
            /// ���.������������
            /// </summary>
            EKRBook,

            /// <summary>
            /// ���.������������
            /// </summary>
            FKRBook,

            /// <summary>
            /// ����������.�������
            /// </summary>
            MarksOutcomes,

            MarksInDebt,
            MarksOutDebt,

            /// <summary>
            /// ��� ��������������
            /// </summary>
            AllClassifiers,

            /// <summary>
            /// �������������� ��������� ��������������
            /// </summary>
            Special,

            /// <summary>
            /// ����������� ��������� ��������������
            /// </summary>
            Standard,

            /// <summary>
            /// ��������� ����������� ��������������
            /// </summary>
            SrcInFin,

            /// <summary>
            /// ��������� �������� ��������������
            /// </summary>
            SrcOutFin,

            /// <summary>
            /// �������������
            /// </summary>
            Arrears,

            // ����� �������
            Excess,
            // ���� �������
            Account, 

            /// <summary>
            /// ���������� �� ��� �������� ��������, � ������ �� �����, ������� �������� ����� ��������������
            /// </summary>
            CacheSubCode
        }

        #endregion ���������, ������������


        #region �������������

        /// <summary>
		/// �����������
		/// </summary>
		/// <param name="scheme">������ �� ��������� ������� �����</param>
        public CorrectedPumpModuleBase()
		{

		}

		/// <summary>
		/// ����������
		/// </summary>
        protected override void Dispose(bool disposing)
		{
            if (disposing)
            {
                if (corrFK2FO != null) corrFK2FO.Clear();

                if (corrFO2FK != null) corrFO2FK.Clear();
            }
        }

        #endregion �������������


        #region ����� �������

        /// <summary>
        /// ���������� ������ �������� ������� ����� ���������������, ���������������� ���������� ���������
        /// </summary>
        /// <param name="clsYears">������ ����� ���������������</param>
        /// <returns>������</returns>
        protected int GetYearIndexByYear(int[] clsYears)
        {
            if (clsYears == null || clsYears.GetLength(0) == 0) return 0;

            Array.Sort(clsYears);
            for (int i = clsYears.GetLength(0) - 1; i >= 0; i--)
            {
                if (this.DataSource.Year >= clsYears[i]) return i;
            }

            return 0;
        }

        #endregion ����� �������


        #region ��������� ������

        /// <summary>
        /// ������ ������ ��� ���������
        /// </summary>
        protected override void QueryDataForProcess()
        {
            QueryData();
        }

        /// <summary>
        /// ������� ��������� ���� ������ �� ������ ���������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        protected override void ProcessDataSource()
        {

        }

        /// <summary>
        /// ���� ��������� ������
        /// </summary>
        protected override void DirectProcessData()
        {
            ProcessDataSourcesTemplate("��������� ���� ������");
        }

        #endregion ��������� ������
    }
}