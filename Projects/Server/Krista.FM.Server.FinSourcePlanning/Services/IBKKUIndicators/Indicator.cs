using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning.Services.IBKKUIndicators
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
            formulaValues = new string[BKKUIndicatorsService.yearsCount];
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
            resultData = new ResultData[BKKUIndicatorsService.yearsCount];
            for (int year = 0; year < BKKUIndicatorsService.yearsCount; year++)
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

    #region Класс для обработки исключений
    /// <summary>
    /// Класс, определяющий причину исключения при расчете.
    /// </summary>
    internal class IndicatorExceptionHelper
    {
        /// <summary>
        /// Определяет причину исключения для индикаторов.
        /// </summary>
        /// <param name="e">Исключение.</param>
        /// <returns>Причина, если удалось определить. Иначе исключение отправляется дальше.</returns>
        public static string ExceptionReason(Exception e)
        {
            // Если нет обработчика индикатора.
            if (e is InvalidCastException)
            {
                return "Отсутствует обработчик индикатора.";
            }
            // Если есть, но некорректный.
            if (e is InvalidOperationException)
            {
                return "Ошибка в формате обработчика индикатора.";
            }
            if (e is FormatException)
            {
                return "Граничные значения оценки не указаны или некорректны.";
            }
            if (e.Message.Equals(RuntimeCompiledHandler.handlerErrorExceptionText))
            {
                return string.Format("{0}.", e.Message);
            }
            // Если не нашли причину
            throw e;
        }

        /// <summary>
        /// Определяет причину исключения для показателей.
        /// </summary>
        /// <param name="e">Исключение.</param>
        /// <param name="markName">Имя показателя.</param>
        /// <returns>Причина, если удалось определить. Иначе исключение отправляется дальше.</returns>
        public static string ExceptionReason(Exception e, string markName)
        {
            // Если нет обработчика показателя.
            if (e is InvalidCastException)
            {
                return string.Format("Отсутствует обработчик показателя {0}.", markName);
            }
            // Если есть, но некорректный.
            if (e is InvalidOperationException)
            {
                return string.Format("Ошибка в формате обработчика показателя {0}.", markName);
            }
            if (e is Microsoft.AnalysisServices.AdomdClient.AdomdErrorResponseException)
            {
                IndicatorExceptionHelper helper = new IndicatorExceptionHelper();
                return helper.AdomdErrorResponseExceptionReason(e, markName);
            }
            if (e.Message.Equals(RuntimeCompiledHandler.handlerErrorExceptionText))
            {
                return string.Format("{0} показателя {1}", e.Message, markName);
            }
            // Если не нашли причину
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
                    return string.Format("Не удалось найти член измерения {0} при расчете показателя {1}", math.Captures[0].Value, markName);
                }
            }
            // The cube 'ФО_Результат ИФ' does not exist, or it is not processed
            if (e.Message.Contains("does not exist, or it is not processed"))
            {
                Regex regerx = new Regex("'.*'");
                Match math = regerx.Match(e.Message);
                if (math.Success)
                {
                    return string.Format("Куб '{0}' не существует или не рассчитан.", math.Captures[0].Value);
                }
            }
            throw e;
        }
    #endregion

    }
}
