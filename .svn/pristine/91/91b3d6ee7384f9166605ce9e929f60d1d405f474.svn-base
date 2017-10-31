using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.ServerLibrary;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Server.FinSourcePlanning.Services.IndicatorsService
{
    /// <summary>
    /// Типы запросов для получения значений показателя.
    /// </summary>
    public enum QueryTypes
    {
        MDX,
        SQL,
        Computable
    }

    /// <summary>
    /// Представляет обработчик показателя.
    /// </summary>
    public class MarkHandler
    {
        public QueryTypes queryType;
        public string queryTemplate;
        public string name;
        public List<string> markNames;

        /// <summary>
        /// Вычисление значений показателя.
        /// </summary>
        /// <returns>Массив со значениями по годам.</returns>
        public double[] CalculateMarkValues(MarksCasche marksCashe)
        {
            QueryEngineAbstraction engine = QueryEngineAbstraction.FactoryMethod(queryType, markNames, marksCashe);
            return engine.MakeAndRunQuery(queryTemplate);
        }
    }

    /// <summary>
    /// Абстрактный класс для получения значений показателей.
    /// </summary>
    internal abstract class QueryEngineAbstraction
    {
        protected string variantIF;
        protected string variantIncome;
        protected string variantIncomeYear;
        protected string variantOutcome;

        /// <summary>
        /// Формирует и выполняет запрос.
        /// </summary>
        /// <param name="queryTemplate">Шаблон запроса.</param>
        /// <returns>Массив со значениями по годам.</returns>
        public abstract double[] MakeAndRunQuery(string queryTemplate);

        /// <summary>
        /// Фабричный метод для скриптовых движков.
        /// </summary>
        /// <param name="type">Тип нужного движка.</param>
        /// <param name="markNames">Список показателей используемых в вычислимом.</param>
        /// <param name="marksCasche">Кэш показателей.</param>
        /// <returns>Экземпляр движка нужного типа.</returns>
        public static QueryEngineAbstraction FactoryMethod(QueryTypes type, List<string> markNames, MarksCasche marksCasche)
        {
            switch(type)
            {
                case QueryTypes.MDX:
                    return new MdxEngine(marksCasche.VariantIF, marksCasche.VariantIncome, marksCasche.VariantIncomeYear,
                        marksCasche.VariantOutcome, marksCasche.AdomdConnection);
                case QueryTypes.SQL:
                    return  new SqlEngine(marksCasche.VariantBorrowID);
                case QueryTypes.Computable:
                    return new ComputableEngine(markNames, marksCasche);
            }
            throw new Exception("В обработчике исходных данных указан некорректный тип запроса.");
        }
    }

    #region Реализация для MDX
    internal class MdxEngine : QueryEngineAbstraction
    {
        private AdomdConnection adomdConnection;

        public MdxEngine(string variantIF, string variantIncome, string variantIncomeYear, string variantOutcome, AdomdConnection adomdConnection)
        {
            this.variantIF = GetVariantName(variantIF);
            this.variantIncome = GetVariantName(variantIncome);
            this.variantIncomeYear = variantIncomeYear;
            this.variantOutcome = GetVariantName(variantOutcome);
            this.adomdConnection = adomdConnection;
        }

        private string GetVariantName(string variant)
        {
            string[] variantParts = variant.Split('.');
            return variantParts[variantParts.Length - 1];
        }

        private string[] MakeQuery(string queryTemplate)
        {
            queryTemplate = queryTemplate.Replace("/*#Вариант ИФ#*/", variantIF);
            queryTemplate = queryTemplate.Replace("/*#Вариант доходов#*/", variantIncome);
            queryTemplate = queryTemplate.Replace("/*#Вариант доходов год#*/", variantIncomeYear);
            queryTemplate = queryTemplate.Replace("/*#Вариант расходов#*/", variantOutcome);
            string[] query = new string[IndicatorsService.yearsCount];
            for (int year = 0; year < IndicatorsService.yearsCount; year++)
            {
                int queryYear = FinSourcePlanningFace.baseYear + year;
                query[year] = queryTemplate.Replace("/*#год#*/", queryYear.ToString());
            }
            return query;
        }

        public override double[] MakeAndRunQuery(string queryTemplate)
        {
            string[] query = MakeQuery(queryTemplate);
            
            AdomdCommand command = new AdomdCommand();
            command.Connection = adomdConnection;
            double[] result = new double[IndicatorsService.yearsCount];
            for (int year = 0; year < IndicatorsService.yearsCount; year++)
            {
                command.CommandText = query[year];
                CellSet cellSet = command.ExecuteCellSet();
                result[year] = Convert.ToDouble(cellSet.Cells[0].Value);
            }
            return result;
        }
    }
    #endregion

    #region Реализация для Sql
    internal class SqlEngine : QueryEngineAbstraction
    {
        private int variantBorrowID;
        private int creditPeriod;
        private List<IDbDataParameter> parametrs;

        public SqlEngine(int variantBorrowID)
        {
            this.variantBorrowID = variantBorrowID;
        }

        private string MakeQuery(string queryTemplate, IDatabase db, int year)
        {
            int queryYear = FinSourcePlanningFace.baseYear + year;
            parametrs = new List<IDbDataParameter>();
            string query = queryTemplate;

            // Надо четкое сопоставление порядка следования параметров.
            if (query.Contains("/*#год#*/"))
            {
                query = query.Replace("/*#год#*/", queryYear.ToString());
              //  parametrs.Add(db.CreateParameter("year", queryYear));
            }

            if (query.Contains("/*#ID варианта заимствований#*/"))
            {
                query = query.Replace("/*#ID варианта заимствований#*/", variantBorrowID.ToString());
                //parametrs.Add(db.CreateParameter("variantBorrowID", variantBorrowID));
            }

            if (query.Contains("/*#Начало периода#*/"))
            {
                string queryDate = string.Format("To_Date('01.01.{0}', 'dd.mm.yyyy')", queryYear);
                query = query.Replace("/*#Начало периода#*/", queryDate);
              //  parametrs.Add(db.CreateParameter("periodStart", date));
            }
            if (query.Contains("/*#Окончание периода#*/"))
            {
                string queryDate = string.Format("To_Date('01.01.{0}', 'dd.mm.yyyy')", queryYear);
                query = query.Replace("/*#Окончание периода#*/", queryDate);
              //  parametrs.Add(db.CreateParameter("periodEnd", date));
            }
            
            return query;
        }

        public override double[] MakeAndRunQuery(string queryTemplate)
        {
            double[] result = new double[IndicatorsService.yearsCount];
            IDatabase db =  FinSourcePlanningFace.Instance.Scheme.SchemeDWH.DB;
            try
            {
                for (int year = 0; year < IndicatorsService.yearsCount; year++)
                {
                    string query = MakeQuery(queryTemplate, db, year);
                    DataTable queryResult = (DataTable)(db.ExecQuery(query, QueryResultTypes.DataTable));
                    result[year] = queryResult.Rows.Count == 0 || queryResult.Rows[0][0] == DBNull.Value 
                            ? 0 
                            : Convert.ToDouble(queryResult.Rows[0][0]);
                }
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return result;
        }
    }
    #endregion

    #region Реализация вычислимых

    internal class ComputableEngine : QueryEngineAbstraction
    {
        private List<string> markNames;
        private MarksCasche marksCasche;
        private string calculateMethodName = "Calculate";

        public ComputableEngine(List<string> markNames, MarksCasche marksCasche)
        {
            this.markNames = markNames;
            this.marksCasche = marksCasche;
        }

        /// <summary>
        /// Составляет текст для компиляции.
        /// </summary>
        /// <param name="queryTemplate">Формула вычислений.</param>
        /// <returns>Сформированный текст сборки</returns>
        private string CompositionHandlerText(string queryTemplate)
        {
            // Декларируем сигнатуру и сам метод
            string calculateMethodDeclaration = string.Format(
                "public double {0}(System.Collections.Generic.Dictionary<string, double> input)", calculateMethodName);
            string calculateMethodContent = string.Format("{0}", queryTemplate);
            return string.Format("{0}{{{1}}}", calculateMethodDeclaration, calculateMethodContent);
        } 

        public override double[] MakeAndRunQuery(string queryTemplate)
        {
            Dictionary<string, Mark> marksList = new Dictionary<string, Mark>();
            // Собираем необходимые показатели
            foreach (string markName in markNames)
            {
                marksList.Add(markName, marksCasche.GetMark(markName));
            }
            double[] resultData = new double[IndicatorsService.yearsCount];
            for (int year = 0; year < IndicatorsService.yearsCount; year++)
            {
                // Формируем параметры.
                object[] parametrs = new object[1];
                Dictionary<string, double> input = new Dictionary<string, double>();
                foreach (string markName in markNames)
                {
                   input.Add(markName, marksList[markName].Values[year]);
                }
                parametrs[0] = input;
                // Собираем текст метода и вызываем обработчик.
                string calculatingMethod = CompositionHandlerText(queryTemplate);
                RuntimeCompiledHandler handler = new RuntimeCompiledHandler(calculatingMethod);
                resultData[year] = (double) handler.ExecuteHandler(parametrs, calculateMethodName);
            }
            return resultData;
        }
    }

    #endregion
}
