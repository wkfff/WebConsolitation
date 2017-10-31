using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.ServerLibrary;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Server.FinSourcePlanning.Services.IBKKUIndicators
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
        public double[] CalculateMarkValues(string variantIF, string variantIncome, string variantOutcome, int variantBorrowID, MarksCasche marksCashe)
        {
            QueryEngineAbstraction engine = QueryEngineAbstraction.FactoryMethod(queryType, variantIF, variantIncome, variantOutcome, variantBorrowID, markNames, marksCashe);
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
        /// <param name="variantIF">Вариант ИФ.</param>
        /// <param name="variantIncome">Вариант доходов.</param>
        /// <param name="variantOutcome">Вариант расходов.</param>
        /// <param name="markNames">Список показателей используемых в вычислимом.</param>
        /// <param name="marksCasche">Кэш показателей.</param>
        /// <returns>Экземпляр движка нужного типа.</returns>
        public static QueryEngineAbstraction FactoryMethod(QueryTypes type, string variantIF, string variantIncome, string variantOutcome, int variantBorrowID, List<string> markNames, MarksCasche marksCasche)
        {
            switch(type)
            {
                case QueryTypes.MDX:
                    return new MdxEngine(variantIF, variantIncome, variantOutcome, marksCasche.AdomdConnection);
                case QueryTypes.SQL:
                    return  new SqlEngine(variantBorrowID);
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

        public MdxEngine(string variantIF, string variantIncome, string variantOutcome, AdomdConnection adomdConnection)
        {
            this.variantIF = VariantToMdxForm(variantIF);
            this.variantIncome = VariantToMdxForm(variantIncome);
            this.variantOutcome = VariantToMdxForm(variantOutcome);
            this.adomdConnection = adomdConnection;
        }
        
        /// <summary>
        /// Преобразовывает имя варианта для использования в MDX запросе.
        /// </summary>
        /// <param name="variant"></param>
        /// <returns></returns>
        private string VariantToMdxForm(string variant)
        {
            string result = string.Empty;
            string[] variantParts = variant.Split('.');
            for (int i = 0; i < variantParts.Length; i++)
            {
                result += string.Format("[{0}].", variantParts[i]);
            }
            return result.Trim('.');
        }

        private string[] MakeQuery(string queryTemplate)
        {
            queryTemplate = queryTemplate.Replace("/*#Вариант ИФ#*/", variantIF);
            queryTemplate = queryTemplate.Replace("/*#Вариант доходов#*/", variantIncome);
            queryTemplate = queryTemplate.Replace("/*#Вариант расходов#*/", variantOutcome);
            string[] query = new string[BKKUIndicatorsService.yearsCount];
            for (int year = 0; year < BKKUIndicatorsService.yearsCount; year++)
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
            double[] result = new double[BKKUIndicatorsService.yearsCount];
            for (int year = 0; year < BKKUIndicatorsService.yearsCount; year++)
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

        private string MakeQuery(string queryTemplate, IDatabase db, int year, out List<IDbDataParameter> parametrs)
        {
            int queryYear = FinSourcePlanningFace.baseYear + year;
            parametrs = new List<IDbDataParameter>();
            string query = queryTemplate;
            if (query.Contains("/*#ID варианта заимствований#*/"))
            {
                query = query.Replace("/*#ID варианта заимствований#*/", "?");
                parametrs.Add(db.CreateParameter("variantBorrowID", variantBorrowID));
            }

            if (query.Contains("/*#Начало периода#*/"))
            {
                query = query.Replace("/*#Начало периода#*/", "?");
                DateTime date = new DateTime(queryYear - 1, 1, 1);
                parametrs.Add(db.CreateParameter("periodStart", date));
            }
            if (query.Contains("/*#Окончание периода#*/"))
            {
                query = query.Replace("/*#Окончание периода#*/", "?");
                DateTime date = new DateTime(queryYear, 1, 1);
                parametrs.Add(db.CreateParameter("periodEnd", date));
            }
            
            return query;
        }

        public override double[] MakeAndRunQuery(string queryTemplate)
        {
            double[] result = new double[BKKUIndicatorsService.yearsCount];
            IDatabase db =  FinSourcePlanningFace.Instance.Scheme.SchemeDWH.DB;
            try
            {
                // для РОД срочность кредита разве важна?

                //select sum(t.Summa) from f_s_СreditIncome f join t_s_PlanServiceCI t on (f.ID = t.RefCreditInc)
                //where f.RefVariant = /*#ID варианта заимствований#*/
                // and f.RefsCreditPeriod = /*#Срочность кредита#*/
                //and t.datestart >= /*#Начало периода#*/ 
                //and t.datestart <  /*#Окончание периода#*/

//select sum(t.Summa) from f_s_СreditIncome f join t_s_PlanServiceCI t on (f.ID = t.RefCreditInc)
//where f.RefVariant = /*#*/43/*#*/  
 // and f.RefsCreditPeriod = /*#*/1/*#*/
  //and t.datestart >= /*#*/To_Date('01.01.2008', 'dd.mm.yyyy')/*#*/ 
  //and t.datestart <  /*#*/To_Date('01.01.2009', 'dd.mm.yyyy')/*#*/


                for (int year = 0; year < BKKUIndicatorsService.yearsCount; year++)
                {
                    string query = MakeQuery(queryTemplate, db, year, out parametrs);
                    DataTable queryResult = (DataTable)(db.ExecQuery(query, QueryResultTypes.DataTable, parametrs.ToArray()));
                    result[year] = queryResult.Rows[0][0] == DBNull.Value ? 0 : Convert.ToDouble(queryResult.Rows[0][0]);
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
            double[] resultData = new double[BKKUIndicatorsService.yearsCount];
            for (int year = 0; year < BKKUIndicatorsService.yearsCount; year++)
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
