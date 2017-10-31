using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning.Services.IndicatorsService
{
    /// <summary>
    /// Представляет результаты расчета и оценки.
    /// </summary>
    public class ResultData
    {
        public double value;
        public double assession;
    }

    /// <summary>
    /// Преставляет входные данные для обработчика вычислений индикатора.
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
        /// Инстанцирует индикатор без значений требуемых показателей.
        /// </summary>
        /// <param name="row">Строка с данными индикатора.</param>
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
                // пытаемся десериализовать обработчик
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
            formulaValues = new string[IndicatorsService.yearsCount];
        }

        #region Свойства
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
        /// Вычисляет значение и оценивает индикатор.
        /// </summary>
        public void CalculateAndAssess(MarksCasche marksCashe)
        {
            // Если уже были глюки, то выходим.
            if (handler.assessionType == AssessionType.NonAssessed)
                return;
            foreach (string markName in Handler.marksNames)
            {
                // Собираем ошибки.
                try
                {
                    MarksList.Add(markName, marksCashe.GetMark(markName));
                }
                catch(Exception e)
                {
                    NoteNonAssessed(IndicatorExceptionHelper.ExceptionReason(e, markName));
                }
            }
            // Если появились глюки, то выходим.
            if (handler.assessionType == AssessionType.NonAssessed)
                return;
            // Вычисляем.
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
        /// Помечает индикатор как неоцененный.
        /// </summary>
        /// <param name="reason">Причина ошибки.</param>
        private void NoteNonAssessed(string reason)
        {
            // Помечаем неоценным и делаем заглушку для данных.
            handler.assessionType = AssessionType.NonAssessed;
            resultData = new ResultData[IndicatorsService.yearsCount];
            for (int year = 0; year < IndicatorsService.yearsCount; year++)
            {
                resultData[year] = new ResultData();
            }
            nonAssessReason.Add(reason);
        }

        /// <summary>
        /// Формирует представление формулы с числовыми значениями.
        /// </summary>
        private void GetFormulaValues()
        {
            for (int year = 0; year < IndicatorsService.yearsCount; year++)
            {
                string formulaValue = formula;
                foreach (Mark mark in MarksList.Values)
                {
                    string namePattern = string.Format(@"\b{0}\b", mark.Name);
                    Regex regExp = new Regex(namePattern);
                    string markValue = string.Format("{0:N2}", mark.Values[year]);
                    formulaValue = regExp.Replace(formulaValue, markValue);
                } 
                string resultValue = string.Format("{0:N6}", resultData[year].value);
                formulaValues[year] = string.Format("{0} = {1}", formulaValue, resultValue);
            }
        }

        /// <summary>
        /// Формирует строку из критериев оценки.
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
            // Если что-то получилось
            if (assessCritString.Length > 2)
            {   // отрезаем первую запятую с пробелом.
                assessCritString = assessCritString.Remove(0, 2);
            }
            return assessCritString;
        }
    }
   
}
