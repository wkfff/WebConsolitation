using System;
using System.Collections.Generic;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning.Services.IBKKUIndicators
{
    public class IndicatorHandler
    {
        /// <summary>
        /// Тип оценки индикатора.
        /// </summary>
        public AssessionType assessionType;

        /// <summary>
        /// Текст обработчика вычислений.
        /// </summary>
        public string handlerFormulaText;

        // Текст обработчика оценки.
        public string handlerAssessMethodText;

        /// <summary>
        /// Имя обработчика.
        /// </summary>
        public string name;

        /// <summary>
        /// Список имен показателей.
        /// </summary>
        public List<string> marksNames;

        /// <summary>
        /// Считает величины индикатора.
        /// </summary>
        /// <param name="marks">Список показателей.</param>
        /// <param name="assessCritValues">Значения критериев оценки.</param>
        /// <returns>Результаты расчета и оценки по годам.</returns>
        public ResultData[] CalculateIndicatorValues(Dictionary<string, Mark> marks, List<double> assessCritValues)
        {
            ResultData[] resultData = new ResultData[BKKUIndicatorsService.yearsCount];
            for (int year = 0; year < BKKUIndicatorsService.yearsCount; year++)
            {
                object[] parametrs = new object[1];
                InputData inputData = new InputData();
                foreach (string markName in marksNames)
                {
                    inputData.marks.Add(markName, marks[markName].Values[year]);
                }
                inputData.assessCritValues = new double[assessCritValues.Count];
                assessCritValues.CopyTo(inputData.assessCritValues);
                parametrs[0] = inputData;
                // Вызываем обработчик.
                string handlerContentText = CompositionHandlerText();
                RuntimeCompiledHandler handler = new RuntimeCompiledHandler(handlerContentText);
                resultData[year] = (ResultData) handler.ExecuteHandler(parametrs, handlerCalculateMethodName);
            }
            return resultData;
        }

        private string handlerAssessMethodName = "Assess";
        private string handlerCalculateMethodName = "Calculate";

        private string HandlerAssessMethodContent()
        {
            switch (assessionType)
            {
                case AssessionType.Logical:
                    return "if (assessingValue <= assessCritValues[0]) return 0; else return 1;";
                case AssessionType.Interval:
                    return "if (assessingValue < assessCritValues[0]) return 0;" +
                           " if (assessingValue > assessCritValues[1]) return 1;" +
                           " return (assessingValue - assessCritValues[0]) / (assessCritValues[1] - assessCritValues[0]);";
                default:
                    return string.Empty;
            }
        }

        private string CompositionHandlerText()
        {
            string assessMethodDeclaration = string.Format("private double {0}(double assessingValue, double[] assessCritValues)", handlerAssessMethodName);
            if (string.IsNullOrEmpty(handlerAssessMethodText))
            {
                handlerAssessMethodText = HandlerAssessMethodContent();
            }
            string calculateMethodDeclaration = string.Format("public ResultData {0}(InputData input)", handlerCalculateMethodName);
            string calculateMethodContent = string.Format("ResultData result = new ResultData();{0}result.assession = Assess(result.value, input.assessCritValues);return result;", handlerFormulaText);
            return 
                string.Format("{0}{{{1}}}{2}{{{3}}}", assessMethodDeclaration, handlerAssessMethodText, calculateMethodDeclaration, calculateMethodContent);
        } 
    }
}