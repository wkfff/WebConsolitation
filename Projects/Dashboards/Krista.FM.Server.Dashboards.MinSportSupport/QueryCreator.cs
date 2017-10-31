using System;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.MinSportSupport
{
   /// <summary>
    /// класс - генератор универсальных MDX запросов
    /// </summary>
    public class QueryCreator
    {
        #region шаблоны запросов

        private const string TemplateQuery =
      @"with
            member   [Measures].[rowElement] 
            as '[#SERVICEFORMNAME#_Строки_сопоставимые].[Строки_сопоставимые].currentMember.properties(""Код"")'  
            member [Measures].[colElement] 
            as '[#SERVICEFORMNAME#_Столбцы_сопоставимые].[Столбцы_сопоставимые].currentMember.properties(""Код"")' 
            member [Measures].[Territory]  
            as '[Территории_сопоставимый].[Территории_сопоставимый].currentMember.properties(""ЕМИСС_ОКАТО"")'
            set [ТерриторииРФ] 
            as '[Территории_сопоставимый].[Территории_сопоставимый].[All].[Российская  Федерация]' 
            set [ТерриторииФО] 
            as 'Descendants 
                (
                    {
                        [Территории_сопоставимый].[Территории_сопоставимый].[All].[Российская  Федерация] 
                    },
                    [Территории_сопоставимый].[Территории_сопоставимый].[Федеральный округ],
                    SELF 
                )' 
            set [ТерриторииСуб] 
            as 'Descendants 
                (
                    {
                        [Территории_сопоставимый].[Территории_сопоставимый].[All].[Российская  Федерация] 
                    },
                    [Территории_сопоставимый].[Территории_сопоставимый].[Субъект РФ],
                    SELF 
                )' 
            #SETS#
        select
        {
            #CROSSJOIN#
        } on rows,
        { 
            [Measures].[Значение],
            [Measures].[Territory],
            [Measures].[rowElement],
            [Measures].[colElement]
        } on columns
        from [#FORMNAME#]
        where [Год].[Год].[Все].[#YEAR#]";

     private const string TemplateCrossjoin = @"NonEmptyCrossJoin({#TERRITORY#}, {#ROW_SET#}, {#COL_SET#})";

       private const string TemplateFilter =
           @"[#SERVICEFORMNAME#_#TYPE#].[#TYPE#].currentMember.properties(""Код"") = ""#CODE#""";

        private const string TemplateSet = 
            @"set [#CODE#]  
            as 'filter  
            (
                [#SERVICEFORMNAME#_#TYPE#].members,
                #FILTERS# 
            )' 
        ";

        #endregion

        #region Переменные

        private  readonly Factor factor;
        //private readonly List<string> territorySet;

        #endregion

        /// <summary>
        /// Конструктор класса 
        /// </summary>
        public QueryCreator(Factor factor)
        {
            this.factor = factor;
            //territorySet = new List<string>();
        }

        /// <summary>
        /// Обработка классификационных признаков по территориям
        /// </summary>
        /*private void ProcessingTerritoryFeature()
        {
            territorySet.Clear();
            if (factor.ClsFeatureRF)
            {
                territorySet.Add("[ТерриторииРФ]");
            }
            if (factor.ClsFeatureFO)
            {
                territorySet.Add("[ТерриторииФО]");
            }
            if (factor.ClsFeatureSubject)
            {
                territorySet.Add("[ТерриторииСуб]");
            }
        }*/

        /// <summary>
        /// Формирование описания множеств (set) в MDX для используемых справочников  
        /// </summary>
        /// <param name="handBookCode">код справочника</param>
        /// <param name="handBookLayout">раскладка справочника</param>
        /// <param name="listSets">список, в который будем выгружать настроенные описания множеств</param>
        private void FormationUseHandBookElement(string handBookCode, int handBookLayout, List<string> listSets)
        {
            var elements = XmlWorker.GetHandBookElements(handBookCode);
            var listFilters = new List<string>();
            foreach (var element in elements)
            {
                string filter = TemplateFilter
                    .Replace("#SERVICEFORMNAME#", factor.ServiceCubeName)
                    .Replace("#TYPE#", (handBookLayout==0)?"Строки_сопоставимые":"Столбцы_сопоставимые")
                    .Replace("#CODE#", element);
                listFilters.Add(filter);
            }
            string userSet = TemplateSet
                .Replace("#CODE#", handBookCode)
                .Replace("#TYPE#", (handBookLayout == 0) ? "Строки_сопоставимые" : "Столбцы_сопоставимые")
                .Replace("#SERVICEFORMNAME#", factor.ServiceCubeName)
                .Replace("#FILTERS#", String.Join(" OR \n\t", listFilters.ToArray()));
            listSets.Add(userSet);
        }


        /// <summary>
        /// Формирование описания множеств (set) в MDX элементов "всего"
        /// </summary>
        /// <param name="layout">раскладка элемента "всего"</param>
        /// <returns></returns>
        private string FormationTotalElement(int layout)
        {
            var listFilters = new List<string>();
            var totalElements = XmlWorker.GetFactorTotalElements(layout);
            foreach (var element in totalElements)
            {
                string filter = TemplateFilter
                    .Replace("#SERVICEFORMNAME#", factor.ServiceCubeName)
                    .Replace("#TYPE#", (layout == 0) ? "Строки_сопоставимые" : "Столбцы_сопоставимые")
                    .Replace("#CODE#", element);
                listFilters.Add(filter);
            }
            var userSet = TemplateSet
                .Replace("#CODE#", (layout == 0) ? "TotalElementRows":"TotalElementColumns")
                .Replace("#TYPE#", (layout == 0) ? "Строки_сопоставимые" : "Столбцы_сопоставимые")
                .Replace("#SERVICEFORMNAME#", factor.ServiceCubeName)
                .Replace("#FILTERS#", String.Join(" OR \n\t", listFilters.ToArray()));
            return userSet;
        }
        
        /// <summary>
        /// Добавление описания множеств для элементов "всего"
        /// </summary>
        /// <param name="listSet">список множеств, которым заменим #SETS#</param>
        private void FormationTotalElements(List<string> listSet)
        {
            if (XmlWorker.CheckTotalElementsOnRows())
            {
                listSet.Add(FormationTotalElement(0));     
            }
            if (XmlWorker.CheckTotalElementsOnColumns())
            {
                listSet.Add(FormationTotalElement(1));   
            }
        }

        /// <summary>
        /// Формирование CrossJoin для используемого справочника
        /// </summary>
        /// <param name="handBookCode">код справочника</param>
        /// <param name="handBookLayout">раскладка справочника</param>
        /// <param name="crossJoinSets">входящий список кроссджойнов</param>
        private void FormationCrossJoin4UseHandBooks(string handBookCode, int handBookLayout, List<string> crossJoinSets)
        {
            if ((handBookLayout == 0 && XmlWorker.CheckTotalElementsOnColumns()) || (handBookLayout == 1 && XmlWorker.CheckTotalElementsOnRows()))
            {
                var crossJoin = TemplateCrossjoin
                    .Replace("#TERRITORY#", String.Join(",", factor.GetTerritorySet().ToArray()))
                    .Replace("#ROW_SET#", (handBookLayout == 0) ? String.Format("[{0}]", handBookCode) : "[TotalElementRows]")
                    .Replace("#COL_SET#", (handBookLayout == 1) ? String.Format("[{0}]", handBookCode) : "[TotalElementColumns]");
                crossJoinSets.Add(crossJoin);   
            }
        }

        /// <summary>
        /// формирование CrossJoin для "пересекающихся" справочников
        /// </summary>
        /// <param name="crossHandBook">элемент пересекающегося справочника</param>
        /// <param name="crossJoinSets">входящий список кроссджойнов</param>
        private void FormationCrossJoin4CrossHandBooks(CrossHandBooks crossHandBook, List<string> crossJoinSets)
        {
            var crossJoin = TemplateCrossjoin
                .Replace("#TERRITORY#", String.Join(",", factor.GetTerritorySet().ToArray()))
                .Replace("#ROW_SET#", String.Format("[{0}]", crossHandBook.HandBookOnRows))
                .Replace("#COL_SET#", String.Format("[{0}]", crossHandBook.HandBookOnColumns));
            crossJoinSets.Add(crossJoin);
        }

        /// <summary>
        /// Формирование CrossJoin для кодов "всего"
        /// </summary>
        /// <param name="crossJoinSets">входящий список кроссджойнов</param>
        private void FormationCrossJoin4Totals(List<string> crossJoinSets)
        {
            if (XmlWorker.CheckTotalElementsOnColumns() && XmlWorker.CheckTotalElementsOnRows())
            {
                crossJoinSets.Add(TemplateCrossjoin
                     .Replace("#TERRITORY#", String.Join(",", factor.GetTerritorySet().ToArray()))
                     .Replace("#ROW_SET#", "[TotalElementRows]")
                     .Replace("#COL_SET#", "[TotalElementColumns]"));
            }
        }
      
        /// <summary>  
        /// Генерация общего MDX скрипта для всех элементов
        /// </summary>
        /// <returns>сгенерированный скрипт для данного показателя</returns>
        public string GenarateQuery()
        {
            // ProcessingTerritoryFeature();
            var listSets = new List<string>();
            var crossJoinSets = new List<string>();

            FormationTotalElements(listSets);

            foreach (var handBook in XmlWorker.GetFactorUseHandBooks())
            {
                FormationCrossJoin4UseHandBooks(handBook, XmlWorker.GetHandBookLayout(handBook), crossJoinSets);
                FormationUseHandBookElement(handBook, XmlWorker.GetHandBookLayout(handBook), listSets);
            }
            foreach (var crossHandBook in XmlWorker.GetFactorCrossHandBooks())
            {
                FormationCrossJoin4CrossHandBooks(crossHandBook, crossJoinSets);
            }
            FormationCrossJoin4Totals(crossJoinSets);

            var query = TemplateQuery
                   // .Replace("#TERRITORY#", String.Join(",", factor.GetTerritorySet().ToArray()))
                    .Replace("#CROSSJOIN#", String.Join(", \n ", crossJoinSets.ToArray()))
                    .Replace("#SETS#", String.Join("", listSets.ToArray()))
                    .Replace("#SERVICEFORMNAME#", factor.ServiceCubeName)
                    .Replace("#FORMNAME#", factor.CubeName)
                    .Replace("#YEAR#", "2010");

            return query;
        }

        /// <summary>
        /// Генерация MDX скрипта, для используемого справочника
        /// </summary>
        /// <param name="handBook">объект справочника</param>
        /// <returns>скрипт</returns>
        public string GenerateQueryForUseHandBook(string handBook)
        {
            //ProcessingTerritoryFeature();
            var listSets = new List<string>();
            var crossJoinSets = new List<string>();

            FormationTotalElements(listSets);
            FormationUseHandBookElement(handBook, XmlWorker.GetHandBookLayout(handBook), listSets);
            FormationCrossJoin4UseHandBooks(handBook, XmlWorker.GetHandBookLayout(handBook), crossJoinSets);

            var query = TemplateQuery
                    //.Replace("#TERRITORY#", String.Join(",", factor.GetTerritorySet().ToArray()))
                    .Replace("#CROSSJOIN#", String.Join(", \n ", crossJoinSets.ToArray()))
                    .Replace("#SETS#", String.Join("", listSets.ToArray()))
                    .Replace("#SERVICEFORMNAME#", factor.ServiceCubeName)
                    .Replace("#FORMNAME#", factor.CubeName)
                    .Replace("#YEAR#", factor.WorkYear);
            return query;
        }

        /// <summary>
        /// Генерация MDX скрипта для пересекающихся справочников
        /// </summary>
        /// <param name="crossHandBook">объект - пересекающийся справочник</param>
        /// <returns>скрипт</returns>
        public string GenerateQueryForCrossHandBook(CrossHandBooks crossHandBook)
        {
            //ProcessingTerritoryFeature();
            var listSets = new List<string>();
            var crossJoinSets = new List<string>();

            foreach (var handBook in XmlWorker.GetFactorUseHandBooks())
            {
                FormationUseHandBookElement(handBook, XmlWorker.GetHandBookLayout(handBook), listSets);
            }
            FormationCrossJoin4CrossHandBooks(crossHandBook, crossJoinSets);

            var query = TemplateQuery
                    //.Replace("#TERRITORY#", String.Join(",", factor.GetTerritorySet().ToArray()))
                    .Replace("#CROSSJOIN#", String.Join(", \n ", crossJoinSets.ToArray()))
                    .Replace("#SETS#", String.Join("", listSets.ToArray()))
                    .Replace("#SERVICEFORMNAME#", factor.ServiceCubeName)
                    .Replace("#FORMNAME#", factor.CubeName)
                    .Replace("#YEAR#", factor.WorkYear);
            return query;
        }

        /// <summary>
        /// Генерация MDX скрипта для элементов "всего"
        /// </summary>
        /// <returns>скрипт</returns>
        public string GenerateQueryForTotalElements()
        {
            //ProcessingTerritoryFeature();
            var listSets = new List<string>();
            var crossJoinSets = new List<string>();
            FormationTotalElements(listSets);
            FormationCrossJoin4Totals(crossJoinSets);
            var query = TemplateQuery
                    //.Replace("#TERRITORY#", String.Join(",", factor.GetTerritorySet().ToArray()))
                    .Replace("#CROSSJOIN#", String.Join(", \n ", crossJoinSets.ToArray()))
                    .Replace("#SETS#", String.Join("", listSets.ToArray()))
                    .Replace("#SERVICEFORMNAME#", factor.ServiceCubeName)
                    .Replace("#FORMNAME#", factor.CubeName)
                    .Replace("#YEAR#", factor.WorkYear);
            return query;
        }
    }
}
