using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning.Services.IBKKUIndicators
{
    /// <summary>
    /// ������������ ���������� ������� � ������.
    /// </summary>
    public class ResultData
    {
        public double value;
        public double assession;
    }

    /// <summary>
    /// ����������� ������� ������ ��� ����������� ���������� ����������.
    /// </summary>
    public class InputData
    {
        public Dictionary<string, double> marks = new Dictionary<string, double>();
        public double[] assessCritValues;
    }

    public class Indicator
    {
        private string name;
        private string caption;
        private string formula;
        private List<double> assessCritValues;
        private string comment;
        private Dictionary<string, Mark> marksList = new Dictionary<string, Mark>();
        private IndicatorHandler handler;
        private List<string> nonAssessReason;
        
        private string [] formulaValues;
        private ResultData [] resultData;

        /// <summary>
        /// ������������ ��������� ��� �������� ��������� �����������.
        /// </summary>
        /// <param name="row">������ � ������� ����������.</param>
        public Indicator(DataRow row)
        {
            nonAssessReason = new List<string>();
            name = row["SYMBOL"].ToString();
            caption = row["NAME"].ToString();
            formula = row["FORMULA"].ToString();
            comment = row["MEANING"].ToString();
            assessCritValues = new List<double>();
            handler = new IndicatorHandler();
            try
            {
                // �������� ��������������� ����������
                handler = (IndicatorHandler)
                          XmlDeserealizeHelper.GetHandler((byte[]) row["HANDLER"], typeof (IndicatorHandler));

                switch (handler.assessionType)
                {
                    case AssessionType.Logical:
                        assessCritValues.Add(Convert.ToDouble(row["THRESHOLD1"].ToString()));
                        break;
                    case AssessionType.Interval:
                        assessCritValues.Add(Convert.ToDouble(row["THRESHOLD1"].ToString()));
                        assessCritValues.Add(Convert.ToDouble(row["THRESHOLD2"].ToString()));
                        break;
                }
            }
            catch (Exception e)
            {
                NoteNonAssessed(IndicatorExceptionHelper.ExceptionReason(e));
            }
            formulaValues = new string[BKKUIndicatorsService.yearsCount];
        }

        #region ��������
        public string Name
        {
            get { return name; }
        }

        public string Caption
        {
            get { return caption; }
        }

        public string Formula
        {
            get { return formula; }
        }

        public string Comment
        {
            get { return comment; }
        }

        public ResultData[] ResultData
        {
            get { return resultData; }
        }

        public string[] FormulaValues
        {
            get { return formulaValues; }
        }

        public Dictionary<string, Mark> MarksList
        {
            get { return marksList; }
        }

        public IndicatorHandler Handler
        {
            get { return handler; }
        }

        public List<string> NonAssessReason
        {
            get { return nonAssessReason; }
        }
        #endregion

        /// <summary>
        /// ��������� �������� � ��������� ���������.
        /// </summary>
        public void CalculateAndAssess(MarksCasche marksCashe)
        {
            // ���� ��� ���� �����, �� �������.
            if (handler.assessionType == AssessionType.NonAssessed)
                return;
            foreach (string markName in Handler.marksNames)
            {
                // �������� ������.
                try
                {
                    MarksList.Add(markName, marksCashe.GetMark(markName));
                }
                catch(Exception e)
                {
                    NoteNonAssessed(IndicatorExceptionHelper.ExceptionReason(e, markName));
                }
            }
            // ���� ��������� �����, �� �������.
            if (handler.assessionType == AssessionType.NonAssessed)
                return;
            // ���������.
            try
            {
                resultData = Handler.CalculateIndicatorValues(marksList, assessCritValues);
            }
            catch(Exception e)
            {
                NoteNonAssessed(IndicatorExceptionHelper.ExceptionReason(e));
            }
            GetFormulaValues();
        }

        /// <summary>
        /// �������� ��������� ��� �����������.
        /// </summary>
        /// <param name="reason">������� ������.</param>
        private void NoteNonAssessed(string reason)
        {
            // �������� ��������� � ������ �������� ��� ������.
            handler.assessionType = AssessionType.NonAssessed;
            resultData = new ResultData[BKKUIndicatorsService.yearsCount];
            for (int year = 0; year < BKKUIndicatorsService.yearsCount; year++)
            {
                resultData[year] = new ResultData();
            }
            nonAssessReason.Add(reason);
        }

        /// <summary>
        /// ��������� ������������� ������� � ��������� ����������.
        /// </summary>
        private void GetFormulaValues()
        {
            for (int year = 0; year < BKKUIndicatorsService.yearsCount; year++)
            {
                string formulaValue = formula;
                foreach (Mark mark in MarksList.Values)
                {
                    string namePattern = string.Format(@"\b{0}\b", mark.Name);
                    Regex regExp = new Regex(namePattern);
                    formulaValue = regExp.Replace(formulaValue, mark.Values[year].ToString());
                } 
                string resultValue = resultData[year].value.ToString();
                formulaValues[year] = string.Format("{0} = {1}", formulaValue, resultValue);
            }
        }

        /// <summary>
        /// ��������� ������ �� ��������� ������.
        /// </summary>
        /// <returns></returns>
        public string AssessCritToString()
        {
            string assessCritString = string.Empty;
            for (int i = 0; i < assessCritValues.Count; i++)
            {
                double percentsAssess = assessCritValues[i]*100;
                assessCritString += string.Format(", {0}%", percentsAssess);
            }
            // ���� ���-�� ����������
            if (assessCritString.Length > 2)
            {   // �������� ������ ������� � ��������.
                assessCritString = assessCritString.Remove(0, 2);
            }
            return assessCritString;
        }
    }

    #region ����� ��� ��������� ����������
    /// <summary>
    /// �����, ������������ ������� ���������� ��� �������.
    /// </summary>
    internal class IndicatorExceptionHelper
    {
        /// <summary>
        /// ���������� ������� ���������� ��� �����������.
        /// </summary>
        /// <param name="e">����������.</param>
        /// <returns>�������, ���� ������� ����������. ����� ���������� ������������ ������.</returns>
        public static string ExceptionReason(Exception e)
        {
            // ���� ��� ����������� ����������.
            if (e is InvalidCastException)
            {
                return "����������� ���������� ����������.";
            }
            // ���� ����, �� ������������.
            if (e is InvalidOperationException)
            {
                return "������ � ������� ����������� ����������.";
            }
            if (e is FormatException)
            {
                return "��������� �������� ������ �� ������� ��� �����������.";
            }
            if (e.Message.Equals(RuntimeCompiledHandler.handlerErrorExceptionText))
            {
                return string.Format("{0}.", e.Message);
            }
            // ���� �� ����� �������
            throw e;
        }

        /// <summary>
        /// ���������� ������� ���������� ��� �����������.
        /// </summary>
        /// <param name="e">����������.</param>
        /// <param name="markName">��� ����������.</param>
        /// <returns>�������, ���� ������� ����������. ����� ���������� ������������ ������.</returns>
        public static string ExceptionReason(Exception e, string markName)
        {
            // ���� ��� ����������� ����������.
            if (e is InvalidCastException)
            {
                return string.Format("����������� ���������� ���������� {0}.", markName);
            }
            // ���� ����, �� ������������.
            if (e is InvalidOperationException)
            {
                return string.Format("������ � ������� ����������� ���������� {0}.", markName);
            }
            if (e is Microsoft.AnalysisServices.AdomdClient.AdomdErrorResponseException)
            {
                IndicatorExceptionHelper helper = new IndicatorExceptionHelper();
                return helper.AdomdErrorResponseExceptionReason(e, markName);
            }
            if (e.Message.Equals(RuntimeCompiledHandler.handlerErrorExceptionText))
            {
                return string.Format("{0} ���������� {1}", e.Message, markName);
            }
            // ���� �� ����� �������
            throw e;
        }

        private string AdomdErrorResponseExceptionReason(Exception e, string markName)
        {
            if (e.Message.Contains("Formula error - cannot find dimension member"))
            {
                Regex regerx = new Regex("\".*\"");
                Match math = regerx.Match(e.Message);
                if (math.Success)
                {
                    return string.Format("�� ������� ����� ���� ��������� {0} ��� ������� ���������� {1}", math.Captures[0].Value, markName);
                }
            }
            // The cube '��_��������� ��' does not exist, or it is not processed
            if (e.Message.Contains("does not exist, or it is not processed"))
            {
                Regex regerx = new Regex("'.*'");
                Match math = regerx.Match(e.Message);
                if (math.Success)
                {
                    return string.Format("��� '{0}' �� ���������� ��� �� ���������.", math.Captures[0].Value);
                }
            }
            throw e;
        }
    #endregion

    }
}
