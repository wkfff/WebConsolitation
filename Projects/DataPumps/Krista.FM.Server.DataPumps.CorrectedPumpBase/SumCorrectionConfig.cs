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
    /// ��������� ��� ��������� ����
    /// </summary>
    public class SumCorrectionConfig
    {
        /// <summary>
        /// ������ ����� � ��������� ������� ��� ��������� � ������� ������
        /// </summary>
        public virtual string[] SumFieldForCorrect()
        {
            return null;
        }

        /// <summary>
        /// ������ ����� ��� ������������������ ����
        /// </summary>
        public virtual string[] Fields4CorrectedSums()
        {
            return null;
        }
    }


    /// <summary>
    /// ��������� � ����������� ��� ��������� ���� (������)
    /// </summary>
    public class MRSumCorrectionConfig : SumCorrectionConfig
    {
        #region ����

        private string yearPlanReportField;
        private string quarterPlanReportField;
        private string monthPlanReportField;
        private string factReportField;
        private string assignedReportField;
        private string yearPlanField;
        private string quarterPlanField;
        private string monthPlanField;
        private string factField;
        private string assignedField;

        private string excSumP;
        private string excSumPRep;
        private string excSumF;
        private string excSumFRep;

        private string[] sumFieldForCorrect = null;
        private string[] fields4CorrectedSums = null;

        #endregion ����


        /// <summary>
        /// �������� ���� "������� ����������" ������
        /// </summary>
        public string YearPlanReportField
        {
            get
            {
                return yearPlanReportField;
            }
            set
            {
                yearPlanReportField = value;
                sumFieldForCorrect = null;
            }
        }

        /// <summary>
        /// �������� ���� "����������� ����������" ������
        /// </summary>
        public string QuarterPlanReportField
        {
            get
            {
                return quarterPlanReportField;
            }
            set
            {
                quarterPlanReportField = value;
                sumFieldForCorrect = null;
            }
        }

        /// <summary>
        /// �������� ���� "�������� ����������" ������
        /// </summary>
        public string MonthPlanReportField
        {
            get
            {
                return monthPlanReportField;
            }
            set
            {
                monthPlanReportField = value;
                sumFieldForCorrect = null;
            }
        }

        /// <summary>
        /// �������� ���� "����" ������
        /// </summary>
        public string FactReportField
        {
            get
            {
                return factReportField;
            }
            set
            {
                factReportField = value;
                sumFieldForCorrect = null;
            }
        }

        /// <summary>
        /// �������� ���� "���������" ������
        /// </summary>
        public string AssignedReportField
        {
            get
            {
                return assignedReportField;
            }
            set
            {
                assignedReportField = value;
                sumFieldForCorrect = null;
            }
        }

        /// <summary>
        /// �������� ���� "������� ����������" ��� ����������������� ����
        /// </summary>
        public string YearPlanField
        {
            get
            {
                return yearPlanField;
            }
            set
            {
                yearPlanField = value;
                fields4CorrectedSums = null;
            }
        }

        /// <summary>
        /// �������� ���� "����������� ����������" ��� ����������������� ����
        /// </summary>
        public string QuarterPlanField
        {
            get
            {
                return quarterPlanField;
            }
            set
            {
                quarterPlanField = value;
                fields4CorrectedSums = null;
            }
        }

        /// <summary>
        /// �������� ���� "�������� ����������" ��� ����������������� ����
        /// </summary>
        public string MonthPlanField
        {
            get
            {
                return monthPlanField;
            }
            set
            {
                monthPlanField = value;
                fields4CorrectedSums = null;
            }
        }

        /// <summary>
        /// �������� ���� "����" ��� ����������������� ����
        /// </summary>
        public string FactField
        {
            get
            {
                return factField;
            }
            set
            {
                factField = value;
                fields4CorrectedSums = null;
            }
        }

        /// <summary>
        /// �������� ���� "���������" ��� ����������������� ����
        /// </summary>
        public string AssignedField
        {
            get
            {
                return assignedField;
            }
            set
            {
                assignedField = value;
                fields4CorrectedSums = null;
            }
        }

        public string ExcSumP
        {
            get
            {
                return excSumP;
            }
            set
            {
                excSumP = value;
                fields4CorrectedSums = null;
            }
        }

        public string ExcSumPRep
        {
            get
            {
                return excSumPRep;
            }
            set
            {
                excSumPRep = value;
                sumFieldForCorrect = null;
            }
        }

        public string ExcSumF
        {
            get
            {
                return excSumF;
            }
            set
            {
                excSumF = value;
                fields4CorrectedSums = null;
            }
        }

        public string ExcSumFRep
        {
            get
            {
                return excSumFRep;
            }
            set
            {
                excSumFRep = value;
                sumFieldForCorrect = null;
            }
        }

        /// <summary>
        /// �������� ���� "���������� �� ������� ����������", ������������ �� ������ ������
        /// </summary>
        public string SpreadYearPlanReportField;

        /// <summary>
        /// �������� ���� "���������� �� �������� ����������", ������������ �� ������ ������
        /// </summary>
        public string SpreadMonthPlanReportField;

        /// <summary>
        /// �������� ���� "���������� �� ������� ����������", ��������������� �� ����� ���������
        /// </summary>
        public string SpreadYearPlanField;

        /// <summary>
        /// �������� ���� "���������� �� �������� ����������", ��������������� �� ����� ���������
        /// </summary>
        public string SpreadMonthPlanField;

        /// <summary>
        /// ������ ����� � ��������� ������� ��� ��������� � ������� ������
        /// </summary>
        public override string[] SumFieldForCorrect()
        {
            if (sumFieldForCorrect == null)
            {
                sumFieldForCorrect = new string[] { 
                    this.YearPlanReportField, this.QuarterPlanReportField, this.MonthPlanReportField, this.FactReportField, this.AssignedReportField, 
                    this.ExcSumPRep, this.ExcSumFRep };
            }

            return CommonRoutines.RemoveArrayElements(sumFieldForCorrect, string.Empty) as string[];
        }

        /// <summary>
        /// ������ ����� ��� ������������������ ����
        /// </summary>
        public override string[] Fields4CorrectedSums()
        {
            if (fields4CorrectedSums == null)
            {
                fields4CorrectedSums = new string[] { 
                    this.YearPlanField, this.QuarterPlanField, this.MonthPlanField, this.FactField, this.AssignedField, 
                    this.ExcSumP, this.ExcSumF };
            }

            return CommonRoutines.RemoveArrayElements(fields4CorrectedSums, string.Empty) as string[];
        }
    }

    /// <summary>
    /// ��������� � ����������� ��� ��������� ���� (������)
    /// </summary>
    public class YRSumCorrectionConfig : SumCorrectionConfig
    {
        #region ����

        private string assignedReportField;
        private string performedReportField;
        private string assignedField;
        private string performedField;
        private string[] sumFieldForCorrect = null;
        private string[] fields4CorrectedSums = null;

        #endregion ����


        /// <summary>
        /// �������� ���� "���������" ������
        /// </summary>
        public string AssignedReportField
        {
            get
            {
                return assignedReportField;
            }
            set
            {
                assignedReportField = value;
                sumFieldForCorrect = null;
            }
        }

        /// <summary>
        /// �������� ���� "���������" ������
        /// </summary>
        public string PerformedReportField
        {
            get
            {
                return performedReportField;
            }
            set
            {
                performedReportField = value;
                sumFieldForCorrect = null;
            }
        }

        /// <summary>
        /// �������� ���� "���������" ��� ����������������� ����
        /// </summary>
        public string AssignedField
        {
            get
            {
                return assignedField;
            }
            set
            {
                assignedField = value;
                fields4CorrectedSums = null;
            }
        }

        /// <summary>
        /// �������� ���� "���������" ��� ����������������� ����
        /// </summary>
        public string PerformedField
        {
            get
            {
                return performedField;
            }
            set
            {
                performedField = value;
                fields4CorrectedSums = null;
            }
        }

        /// <summary>
        /// ������ ����� � ��������� ������� ��� ��������� � ������� ������
        /// </summary>
        public override string[] SumFieldForCorrect()
        {
            if (sumFieldForCorrect == null)
            {
                sumFieldForCorrect = new string[] { this.AssignedReportField, this.PerformedReportField };
            }

            return CommonRoutines.RemoveArrayElements(sumFieldForCorrect, string.Empty) as string[];
        }

        /// <summary>
        /// ������ ����� ��� ������������������ ����
        /// </summary>
        public override string[] Fields4CorrectedSums()
        {
            if (fields4CorrectedSums == null)
            {
                fields4CorrectedSums = new string[] { this.AssignedField, this.PerformedField };
            }

            return CommonRoutines.RemoveArrayElements(fields4CorrectedSums, string.Empty) as string[];
        }
    }

    /// <summary>
    /// ��������� � ����������� ��� ��������� ���� (�� ������)
    /// </summary>
    public class FKMRSumCorrectionConfig : SumCorrectionConfig
    {
        #region ����

        private string assignedReportField;
        private string factReportField;
        private string assignedField;
        private string factField;
        private string[] sumFieldForCorrect = null;
        private string[] fields4CorrectedSums = null;

        #endregion ����


        /// <summary>
        /// �������� ���� "���������" ������
        /// </summary>
        public string AssignedReportField
        {
            get
            {
                return assignedReportField;
            }
            set
            {
                assignedReportField = value;
                sumFieldForCorrect = null;
            }
        }

        /// <summary>
        /// �������� ���� "���������" ������
        /// </summary>
        public string FactReportField
        {
            get
            {
                return factReportField;
            }
            set
            {
                factReportField = value;
                sumFieldForCorrect = null;
            }
        }

        /// <summary>
        /// �������� ���� "���������" ��� ����������������� ����
        /// </summary>
        public string AssignedField
        {
            get
            {
                return assignedField;
            }
            set
            {
                assignedField = value;
                fields4CorrectedSums = null;
            }
        }

        /// <summary>
        /// �������� ���� "���������" ��� ����������������� ����
        /// </summary>
        public string FactField
        {
            get
            {
                return factField;
            }
            set
            {
                factField = value;
                fields4CorrectedSums = null;
            }
        }

        /// <summary>
        /// ������ ����� � ��������� ������� ��� ��������� � ������� ������
        /// </summary>
        public override string[] SumFieldForCorrect()
        {
            if (sumFieldForCorrect == null)
            {
                sumFieldForCorrect = new string[] { this.AssignedReportField, this.FactReportField };
            }

            return CommonRoutines.RemoveArrayElements(sumFieldForCorrect, string.Empty) as string[];
        }

        /// <summary>
        /// ������ ����� ��� ������������������ ����
        /// </summary>
        public override string[] Fields4CorrectedSums()
        {
            if (fields4CorrectedSums == null)
            {
                fields4CorrectedSums = new string[] { this.AssignedField, this.FactField };
            }

            return CommonRoutines.RemoveArrayElements(fields4CorrectedSums, string.Empty) as string[];
        }
    }

    /// <summary>
    /// ��������� � ����������� ��� ��������� ���� (����� 1��)
    /// </summary>
    public class F1NMSumCorrectionConfig : SumCorrectionConfig
    {
        #region ����

        private string earnedReportField;
        private string inpaymentsReportField;
        private string earnedField;
        private string inpaymentsField;
        private string[] sumFieldForCorrect = null;
        private string[] fields4CorrectedSums = null;

        #endregion ����


        /// <summary>
        /// �������� ���� "���������" ������
        /// </summary>
        public string EarnedReportField
        {
            get
            {
                return earnedReportField;
            }
            set
            {
                earnedReportField = value;
                sumFieldForCorrect = null;
            }
        }

        /// <summary>
        /// �������� ���� "���������" ������
        /// </summary>
        public string InpaymentsReportField
        {
            get
            {
                return inpaymentsReportField;
            }
            set
            {
                inpaymentsReportField = value;
                sumFieldForCorrect = null;
            }
        }

        /// <summary>
        /// �������� ���� "���������" ��� ����������������� ����
        /// </summary>
        public string EarnedField
        {
            get
            {
                return earnedField;
            }
            set
            {
                earnedField = value;
                fields4CorrectedSums = null;
            }
        }

        /// <summary>
        /// �������� ���� "���������" ��� ����������������� ����
        /// </summary>
        public string InpaymentsField
        {
            get
            {
                return inpaymentsField;
            }
            set
            {
                inpaymentsField = value;
                fields4CorrectedSums = null;
            }
        }

        /// <summary>
        /// ������ ����� � ��������� ������� ��� ��������� � ������� ������
        /// </summary>
        public override string[] SumFieldForCorrect()
        {
            if (sumFieldForCorrect == null)
            {
                sumFieldForCorrect = new string[] { this.EarnedReportField, this.InpaymentsReportField };
            }

            return CommonRoutines.RemoveArrayElements(sumFieldForCorrect, string.Empty) as string[];
        }

        /// <summary>
        /// ������ ����� ��� ������������������ ����
        /// </summary>
        public override string[] Fields4CorrectedSums()
        {
            if (fields4CorrectedSums == null)
            {
                fields4CorrectedSums = new string[] { this.EarnedField, this.InpaymentsField };
            }

            return CommonRoutines.RemoveArrayElements(fields4CorrectedSums, string.Empty) as string[];
        }
    }

    /// <summary>
    /// ��������� � ����������� ��� ��������� ���� (����� 4��)
    /// </summary>
    public class F4NMSumCorrectionConfig : SumCorrectionConfig
    {
        #region ����

        private string valueReportField;
        private string valueField;
        private string[] sumFieldForCorrect = null;
        private string[] fields4CorrectedSums = null;

        #endregion ����

        public string ValueReportField
        {
            get
            {
                return valueReportField;
            }
            set
            {
                valueReportField = value;
                sumFieldForCorrect = null;
            }
        }

        public string ValueField
        {
            get
            {
                return valueField;
            }
            set
            {
                valueField = value;
                fields4CorrectedSums = null;
            }
        }

        public override string[] SumFieldForCorrect()
        {
            if (sumFieldForCorrect == null)
                sumFieldForCorrect = new string[] { this.ValueReportField };
            return CommonRoutines.RemoveArrayElements(sumFieldForCorrect, string.Empty) as string[];
        }

        public override string[] Fields4CorrectedSums()
        {
            if (fields4CorrectedSums == null)
                fields4CorrectedSums = new string[] { this.ValueField };
            return CommonRoutines.RemoveArrayElements(fields4CorrectedSums, string.Empty) as string[];
        }
    }

    public class CommonSumCorrectionConfig : SumCorrectionConfig
    {
        #region ����

        private string sum1Report = string.Empty;
        private string sum1 = string.Empty;
        private string sum2Report = string.Empty;
        private string sum2 = string.Empty;
        private string sum3Report = string.Empty;
        private string sum3 = string.Empty;
        private string sum4Report = string.Empty;
        private string sum4 = string.Empty;
        private string sum5Report = string.Empty;
        private string sum5 = string.Empty;
        private string[] sumFieldForCorrect = null;
        private string[] fields4CorrectedSums = null;

        #endregion ����

        public string Sum1Report
        {
            get
            {
                return sum1Report;
            }
            set
            {
                sum1Report = value;
                sumFieldForCorrect = null;
            }
        }

        public string Sum1
        {
            get
            {
                return sum1;
            }
            set
            {
                sum1 = value;
                fields4CorrectedSums = null;
            }
        }

        public string Sum2Report
        {
            get
            {
                return sum2Report;
            }
            set
            {
                sum2Report = value;
                sumFieldForCorrect = null;
            }
        }

        public string Sum2
        {
            get
            {
                return sum2;
            }
            set
            {
                sum2 = value;
                fields4CorrectedSums = null;
            }
        }

        public string Sum3Report
        {
            get
            {
                return sum3Report;
            }
            set
            {
                sum3Report = value;
                sumFieldForCorrect = null;
            }
        }

        public string Sum3
        {
            get
            {
                return sum3;
            }
            set
            {
                sum3 = value;
                fields4CorrectedSums = null;
            }
        }

        public string Sum4Report
        {
            get
            {
                return sum4Report;
            }
            set
            {
                sum4Report = value;
                sumFieldForCorrect = null;
            }
        }

        public string Sum4
        {
            get
            {
                return sum4;
            }
            set
            {
                sum4 = value;
                fields4CorrectedSums = null;
            }
        }

        public string Sum5Report
        {
            get
            {
                return sum5Report;
            }
            set
            {
                sum5Report = value;
                sumFieldForCorrect = null;
            }
        }

        public string Sum5
        {
            get
            {
                return sum5;
            }
            set
            {
                sum5 = value;
                fields4CorrectedSums = null;
            }
        }

        public override string[] SumFieldForCorrect()
        {
            if (sumFieldForCorrect == null)
                sumFieldForCorrect = new string[] { this.Sum1Report, this.Sum2Report, this.Sum3Report, this.Sum4Report, this.Sum5Report };
            return CommonRoutines.RemoveArrayElements(sumFieldForCorrect, string.Empty) as string[];
        }

        public override string[] Fields4CorrectedSums()
        {
            if (fields4CorrectedSums == null)
                fields4CorrectedSums = new string[] { this.Sum1, this.Sum2, this.Sum3, this.Sum4, this.Sum5 };
            return CommonRoutines.RemoveArrayElements(fields4CorrectedSums, string.Empty) as string[];
        }
    }

    public class CommonLiteSumCorrectionConfig : SumCorrectionConfig
    {
        public string[] sumFieldForCorrect = null;
        public string[] fields4CorrectedSums = null;

        public override string[] SumFieldForCorrect()
        {
            return sumFieldForCorrect;
        }

        public override string[] Fields4CorrectedSums()
        {
            return fields4CorrectedSums;
        }
    }

}