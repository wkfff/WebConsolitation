using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.RIA.Core.Controllers.Filters;
using Krista.FM.RIA.Core.Controllers.Helpers;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Controllers
{
    public class EntityController : SchemeBoundController
    {
        private readonly IEntityDataService dataService;

        ////private const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Extensions.Entity.dll/Krista.FM.RIA.Extensions.Entity/Presentation/Views/Entity/";

        public EntityController(IEntityDataService dataService)
        {
            this.dataService = dataService;
        }

        private enum DataSourceType
        {
            /// <summary>
            /// Источник - год
            /// </summary>
            Year, 

            /// <summary>
            /// Источник - месяц
            /// </summary>
            Month, 

            /// <summary>
            /// Источник - квартал
            /// </summary>
            Quarter
        }

        /// <summary>
        /// Служит для отображения сущности
        /// </summary>
        /// <param name="objectKey">Идентификатор сущности</param>
        /// <param name="filter">набор фильтров</param>
        /// <returns> Путь к странице для отображения</returns>        
        [ViewEntityAuthorizationFilter]
        public ActionResult Show(string objectKey, string filter)
        {
            // получаем объект - сущность
            IEntity entity = Scheme.RootPackage.FindEntityByName(objectKey);

            if (entity == null)
            {
                throw new Exception("Объект не найден.");
            }

            var gridView = new Core.Gui.GridView
                               {
                                   Entity = entity,
                                   Readonly = false,
                                   Id = "gridView{0}".FormatWith(entity.FullDBName),
                                   Title = entity.FullCaption
                               };

            // приводим к типу "классификатор"
            IClassifier classifier = entity as IClassifier;

            // если приведение успешно
            if (classifier != null)
            {
                if (((IDataSourceDividedClass)entity).IsDivided)
                {
                    Dictionary<int, string> sources = ((IClassifier)entity).GetDataSourcesNames();
                    gridView.SourceId = NearestSource(entity, sources);
                }

                // если классификатор иерархичный
                if (classifier.Levels.HierarchyType == HierarchyType.ParentChild)
                {
                    gridView.ParentId = "PARENTID";
                    return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", gridView);
                }
            }

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", gridView);
        }

        /// <summary>
        /// Служит для отображения справочника
        /// </summary>
        /// <param name="objectKey">Идентификатор сущности</param>
        /// <param name="sourceId">Идентификатор внешнего источника</param>
        /// <param name="filter">Набор фильтров</param>
        /// <param name="showMode">
        /// WithoutHierarchy - отображать иерархические классификаторы без иерархии.
        /// Normal - отображать в обычном режиме.
        /// </param>
        /// <param name="isReadonly">Отображать ли справочник в режиме только для чтения (по умолчанию true)</param>
        /// <param name="sortField">Поле, по которому производить сортировку (ID - по-умолчанию)</param>
        /// <param name="sortDir">Направление сортировки (ASC/DESC) (ASC - по-умолчанию)</param>
        [ViewEntityAuthorizationFilter]
        public ActionResult Book(string objectKey, string sourceId, string filter, EntityBookViewModel.ShowModeType? showMode, bool? isReadonly, string sortField, string sortDir)
        {
            if (showMode == null)
            {
                showMode = EntityBookViewModel.ShowModeType.Normal;
            }

            // получаем объект - сущность
            IEntity entity = Scheme.RootPackage.FindEntityByName(objectKey);

            if (entity == null)
            {
                throw new Exception("Объект не найден.");
            }

            // приводим к типу "классификатор"
            IClassifier classifier = entity as IClassifier;

            int sourceIdValue;
            if (!Int32.TryParse(sourceId, out sourceIdValue))
            {
                sourceIdValue = -1;
            }

            int bookSourceId = (classifier == null) ? -1 : GetDataSourceId((IClassifier)entity, sourceIdValue);

            var gridView = new Core.Gui.GridView
                               {
                                   Entity = entity,
                                   Id = "gridView{0}".FormatWith(entity.FullDBName),
                                   Title = entity.FullCaption,
                                   SourceId = bookSourceId,
                                   Readonly = isReadonly ?? true,
                                   IsBook = true,
                                   ShowMode = (EntityBookViewModel.ShowModeType)showMode,
                                   ViewService = new DefaultViewService(filter)
                               };
            
            if (sortField != null)
            {
                gridView.SortField = sortField;
            }

            if (sortDir != null)
            {
                gridView.SortDir = sortDir;
            }

            // если приведение успешно
            if (classifier != null)
            {
                // если классификатор иерархичный
                if (classifier.Levels.HierarchyType == HierarchyType.ParentChild)
                {
                    gridView.ParentId = "PARENTID";
                    return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", gridView);
                }
            }

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", gridView);
        }

        /// <summary>
        /// Получение данных для грида с учетом фильтров.
        /// </summary>
        [ViewEntityAuthorizationFilter]
        public AjaxStoreResult Data(string objectKey, int limit, int start, string dir, string sort, [FiltersBinder]FilterConditions filters)
        {
            return DataWithServerFilter(objectKey, limit, start, dir, sort, null, String.Empty, filters);
        }

        /// <summary>
        /// Получение иерархических данных для грида с учетом фильтров.
        /// </summary>
        [ViewEntityAuthorizationFilter]
        public AjaxStoreResult DataH(
                                    string objectKey, 
                                    int limit, 
                                    int start, 
                                    string dir, 
                                    string sort, 
                                    int? anode, 
                                    int? source, 
                                    string serverFilter, 
                                    [FiltersBinder]FilterConditions filters, 
                                    EntityBookViewModel.ShowModeType? showMode)
        {
            List<string> queryFilterConditions = new List<string>();

            if (serverFilter.IsNotNullOrEmpty())
            {
                queryFilterConditions.Add(serverFilter);
            }

            // справочники для долговой книги
            if (showMode == EntityBookViewModel.ShowModeType.WithoutHierarchy)
            {
                if (anode != null)
                {
                    return new AjaxStoreResult(new DataTable(), 0);
                }
            }
            else 
            {
                if (anode == null)
                {
                    if (serverFilter.IsNullOrEmpty() || !(serverFilter.ToUpper().StartsWith("(ID = ") ||
                        serverFilter.ToUpper().StartsWith("(PARENTID IN ") ||
                        serverFilter.ToUpper().StartsWith("(REFTERR =")))
                    {
                        queryFilterConditions.Add("(PARENTID is null)");
                    }
                }
                else
                {
                    queryFilterConditions.Add("(PARENTID = {0})".FormatWith(anode));
                }
            }

            return DataWithServerFilter(objectKey, limit, start, dir, sort, source, String.Join("AND", queryFilterConditions.ToArray()), filters);
        }

        /* для обработки запроса для автозаполения из списка*/
        public AjaxStoreResult DataWithCustomSearch(
                                                    string objectKey, 
                                                    int limit, 
                                                    int start, 
                                                    string dir, 
                                                    string sort, 
                                                    int? source, 
                                                    string serverFilter,
                                                    [FiltersBinder]FilterConditions filters, 
                                                    string query, 
                                                    string[] fields)
        {
            if (query.IsNotNullOrEmpty() && fields != null && fields.Length > 0)
            {
                var customSearchFilter = String.Join(
                    " or ", 
                    fields.Select(field => "(UPPER({0}) LIKE '%{1}%')".FormatWith(field, query.ToUpper())).ToArray());

                serverFilter = serverFilter.IsNotNullOrEmpty()
                                   ? serverFilter + " AND ({0}) ".FormatWith(customSearchFilter)
                                   : " ({0}) ".FormatWith(customSearchFilter);
            } 
            
            return DataWithServerFilter(objectKey, limit, start, dir, sort, source, serverFilter, filters);
        }

        /// <summary>
        /// Получение данных для грида с учетом фильтров.
        /// </summary>
        [ViewEntityAuthorizationFilter]
        public AjaxStoreResult DataWithServerFilter(
                                                    string objectKey, 
                                                    int limit, 
                                                    int start, 
                                                    string dir, 
                                                    string sort, 
                                                    int? source, 
                                                    string serverFilter,
                                                    [FiltersBinder]FilterConditions filters)
        {
            if (source != null)
            {
                serverFilter = serverFilter.IsNotNullOrEmpty()
                                   ? serverFilter + " AND " + "(SOURCEID = {0})".FormatWith(source)
                                   : "(SOURCEID = {0})".FormatWith(source);
            }

            IEntity entity = Scheme.RootPackage.FindEntityByName(objectKey);

            if (entity == null)
            {
                return new AjaxStoreResult(null, 0);
            }

            IDbDataParameter[] prms = new IDbDataParameter[filters.Conditions.Count];
            List<string> selectFilter = new List<string>();
            int i = 0;

            // Накладываем фильтры грида
            foreach (FilterCondition filter in filters.Conditions)
            {
                if (filter.FilterType == FilterType.Date)
                {
                    prms[i] = new DbParameterDescriptor(filter.Name + i, filter.Value, DbType.Date);
                }
                else
                {
                    prms[i] = new DbParameterDescriptor(filter.Name + i, filter.Value, DbType.String);    
                }
                
                string filterComparison = filter.Comparison == Comparison.Gt ? ">" : filter.Comparison == Comparison.Lt ? "<" : "=";
                if (filter.FilterType == FilterType.String)
                {
                    filterComparison = "like";
                    prms[i].Value = "%{0}%".FormatWith(filter.Value.ToUpper());
                }

                string filterStr = "(UPPER({0}) {1} ?)".FormatWith(filter.Name, filterComparison);
                selectFilter.Add(filterStr);
                i++;
            }

            // Накладываем явно указанный фильтр
            if (!serverFilter.IsNullOrEmpty())
            {
                selectFilter.Add(serverFilter);
            }

            string queryFilter = String.Join(" and ", selectFilter.ToArray());

            return dataService.GetData(entity, start, limit, dir, sort, queryFilter, prms);
        }

        [ViewEntityAuthorizationFilter]
        public AjaxStoreResult DataRecord(string objectKey, int start, string filter)
        {
            IEntity entity = Scheme.RootPackage.FindEntityByName(objectKey);
            if (entity == null)
            {
                return new AjaxStoreResult(null, 0);
            }

            IDbDataParameter[] prms = new IDbDataParameter[1];
            List<string> selectFilter = new List<string>();

            prms[0] = new DbParameterDescriptor("ID", filter, DbType.String);
            selectFilter.Add("(ID = ?)");
            string queryFilter = String.Join(" and ", selectFilter.ToArray());

            return dataService.GetData(entity, start, 1, String.Empty, String.Empty, queryFilter, prms);
        }

        [ViewEntityAuthorizationFilter]
        public AjaxStoreResult DataPaging(
                                            string objectKey, 
                                            string filter, 
                                            string filterFields, 
                                            int start, 
                                            int limit, 
                                            string filterId, 
                                            string dir, 
                                            string sort)
        {
            IEntity entity = Scheme.RootPackage.FindEntityByName(objectKey);
            if (entity == null)
            {
                return new AjaxStoreResult(null, 0);
            }

            string queryFilter = String.Empty;
            IDbDataParameter[] prms = null;

            if (!filterId.IsNullOrEmpty())
            {
                // Принудительная выборка по ID
                prms = new IDbDataParameter[1];
                prms[0] = new DbParameterDescriptor("ID", filterId, DbType.String);
                queryFilter = "(ID = ?)";
            }
            else if (!filter.IsNullOrEmpty())
            {
                // Выборка с учетом фильтра по указанным полям
                string[] parts = filterFields.Split(new[] { ',' });

                List<string> selectFilter = new List<string>();
                prms = new IDbDataParameter[parts.Count()];
                int i = 0;
                foreach (string filterField in parts)
                {
                    prms[i++] = new DbParameterDescriptor(filterField, "%{0}%".FormatWith(filter), DbType.String);
                    selectFilter.Add("({0} like ?)".FormatWith(filterField));
                }

                queryFilter = String.Join(" or ", selectFilter.ToArray());
            }

            return dataService.GetData(entity, start, limit, dir, sort, queryFilter, prms);
        }

        public ActionResult Save(string objectKey)
        {
            try
            {
                IEntity entity = Scheme.RootPackage.FindEntityByName(objectKey);
                if (entity == null)
                {
                    return new AjaxStoreResult(null, 0);
                }

                StoreDataHandler dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);

                if (dataSet.ContainsKey("Updated"))
                {
                    entity.CanEditRecord(Scheme.UsersManager, true);

                    var table = dataSet["Updated"];
                    dataService.Update(entity, table);
                }

                List<long> newIdList = null;
                if (dataSet.ContainsKey("Created"))
                {
                    entity.CanAddRecord(Scheme.UsersManager, true);

                    var table = dataSet["Created"];

                    /* Используется только для иерархических данных */
                    Dictionary<long, long> ids = new Dictionary<long, long>();
                    /* признак иерархических данных */
                    if (dataSet.ContainsKey("hierarchy"))
                    {
                        foreach (var item in table)
                        {
                            ids.Add((long)item["ID"], (long)item["ID"]);
                        }
                    }

                    newIdList = dataService.Create(entity, table, ids);
                }

                if (dataSet.ContainsKey("Deleted"))
                {
                    entity.CanDeleteRecord(Scheme.UsersManager, true);

                    var table = dataSet["Deleted"];
                    dataService.Delete(entity, table);
                }

                var result = new AjaxStoreResult(StoreResponseFormat.Save);

                // TODO : сформировать список новых id-шников
                if (newIdList != null)
                {
                    result.SaveResponse.Message = String.Format("{{newId:{0}}}", newIdList.Last());
                }

                return result;
            }
            catch (Exception e)
            {
                AjaxStoreResult ajaxStoreResult = new AjaxStoreResult(StoreResponseFormat.Save);
                ajaxStoreResult.SaveResponse.Success = false;
                ajaxStoreResult.SaveResponse.Message = e.Message;
                return ajaxStoreResult;
            }
        }

        /// <summary>
        /// Возвращает список источников, для классификатора
        /// </summary>
        /// <param name="objectKey">Ключ объекта схемы (классификатора)</param>
        public AjaxStoreResult Sources(string objectKey)
        {
            // получаем сущность
            IEntity entity = Scheme.RootPackage.FindEntityByName(objectKey);
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            /* для разделимого по источникам объекта */
            if (entity is IDataSourceDividedClass)
            {
                if (((IDataSourceDividedClass)entity).IsDivided)
                {
                    Dictionary<int, string> sources = null;
                    /* получаем список источников */
                    if (entity is IClassifier)
                    {
                        sources = ((IClassifier)entity).GetDataSourcesNames();
                    }
                    else 
                    {
                        if (entity is IFactTable)
                        {
                            sources = ((IFactTable)entity).GetDataSourcesNames();
                        }
                    }

                    if (sources != null)
                    {
                        // сортируем источники по названию
                        var sortedSources = from entry in sources orderby entry.Value ascending select entry;
                        
                        // формируем данные для отправки на клиент
                        foreach (var source in sortedSources)
                        {
                            Dictionary<string, object> s = new Dictionary<string, object>();
                            s.Add("SOURCEID", source.Key);
                            s.Add("SOURCENAME", source.Value);
                            data.Add(s);
                        }

                        // в качестве дополнительного параметра - значение источника по умолчанию
                        return new AjaxStoreExtraResult(data, data.Count, NearestSource(entity, sources));
                    }
                }
            }

            return new AjaxStoreExtraResult(null, 0, User.Identity.Name);
        }

        /// <summary>
        /// Вычисляет "расстояние" до текущего момента по параметрам (ГОД) источника
        /// </summary>
        /// <param name="year">Текущий год</param>
        /// <param name="yearSource">Год источника</param>
        /// <returns>"Расстояние "</returns>        
        private static int Distance(int year, int yearSource)
        {
            if (year == yearSource)
            {
                return 50;
            }

            return Math.Abs(year - yearSource) * 100;
        }

        /// <summary>
        /// Вычиляет "расстояние" до текущего момента по параметрам (ГОД, КВАРТАЛ (ИЛИ МЕСЯЦ)) источника
        /// </summary>
        /// <param name="year">Текущий год</param>
        /// <param name="quarter">Текущий квартал/месяц</param>
        /// <param name="yearSource">Год источника</param>
        /// <param name="quarterSource">Квартал источника/месяц</param>
        /// <param name="dataSourceType">Указывает тип второго параметра: квартал или месяц</param>
        /// <returns>"Расстояние "</returns>        
        private static int Distance(int year, int quarter, int yearSource, int quarterSource, DataSourceType dataSourceType)
        {
            int d = Distance(year, yearSource);
            if (quarter == quarterSource)
            {
                return d - 5;
            }

            if (dataSourceType == DataSourceType.Quarter)
            {
                return d + (Math.Abs(quarter - quarterSource) * 10);
            }

            return d + (Math.Abs(quarter - quarterSource) * 3);
        }

        /// <summary>
        /// Вычиляет "расстояние" до текущего момента по параметрам (ГОД, КВАРТАЛ (ИЛИ МЕСЯЦ)) источника
        /// </summary>
        /// <param name="year">Текущий год</param>
        /// <param name="quarter">Текущий квартал</param>
        /// <param name="month">Текущий месяц</param>
        /// <param name="yearSource">Год источника</param>
        /// <param name="quarterSource">Квартал источника</param>
        /// <param name="monthSource">Месяц источника</param>
        /// <returns>"Расстояние "</returns>        
        private static int Distance(int year, int quarter, int month, int yearSource, int quarterSource, int monthSource)
        {
            int d = Distance(year, yearSource);
            if (quarter == quarterSource)
            {
                d -= 5;
            }

            d += Math.Abs(quarter - quarterSource) * 10;
            if (month == monthSource)
            {
                return d - 1;
            }

            return d + (Math.Abs(month - monthSource) * 3);
        }

        /// <summary>
        /// Возвращает источник данных из справочника соответствующий источнику
        /// указанному в параметре.
        /// </summary>
        /// <param name="entity">Объект схемы.</param>
        /// <param name="outerDataSourceId">Внешний источник для которого нужно найти соответствие.</param>
        private int GetDataSourceId(IClassifier entity, int outerDataSourceId)
        {
            if (!entity.IsDivided)
            {
                return -1;
            }

            DataSources dataSource;
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                DomainRepository repository = new DomainRepository(new SystemDataService(db));
                dataSource = repository.Get<DataSources>(outerDataSourceId);
            }

            if (dataSource == null)
            {
                return -1;
            }

            Dictionary<int, string> dataSourcesNames = entity.GetDataSourcesNames();
            foreach (KeyValuePair<int, string> pair in dataSourcesNames)
            {
                string year = pair.Value.Split(new[] { '-' })[1];
                string supplier = pair.Value.Split(new[] { '\\' })[0];
                if (dataSource.SupplierCode == supplier && dataSource.Year == year.Trim())
                {
                    return pair.Key;
                }
            }

            return -1;
        }

        /// <summary>
        /// Возвращает идентификатор источника, 
        /// параметы которого ближе всего к моменту обращеня.
        /// </summary>
        /// <param name="entity">Сущность .</param>
        /// <param name="sources">Список источников.</param>
        private int NearestSource(IEntity entity, Dictionary<int, string> sources)
        {
            if ((sources == null) || (sources.Count == 0) || !(entity is IDataSourceDividedClass))
            {
                return -1;
            }

            int year = DateTime.Now.Date.Year;
            int month = DateTime.Now.Date.Month;
            int quarter = (month / 3) + 1;

            // минимальное расстояние между текущим временем и параметрами источника
            int distance = int.MaxValue;
            int sourceId = sources.First().Key;
            foreach (var source in sources)
            {
                ParamKindTypes? pkt = null;
                try
                {
                    pkt = ((IDataSourceDividedClass)entity).DataSourceParameter(source.Key);
                }
                catch (Exception)
                {
                }

                if (pkt != null) 
                {
                    int curDist;
                    switch (pkt)
                    {
                        ////[Description("Год")]
                        case ParamKindTypes.Year:
                        case ParamKindTypes.YearVariant:
                        case ParamKindTypes.YearTerritory:
                            {
                                int? sourceYear = SourceData(source.Key, DataSourceType.Year);
                                if (sourceYear != null)
                                {
                                    curDist = Distance(year, (int)sourceYear);
                                    if (curDist < distance)
                                    {
                                        distance = curDist;
                                        sourceId = source.Key;
                                    }
                                }

                                break;
                            }
                        ////[Description("Год квартал")]
                        case ParamKindTypes.YearQuarter:
                        case ParamKindTypes.YearQuarterTerritory:
                            {
                                int? sourceYear = SourceData(source.Key, DataSourceType.Year);
                                int? sourceQuarter = SourceData(source.Key, DataSourceType.Quarter);
                                if (sourceYear != null && sourceQuarter != null)
                                {
                                    curDist = Distance(year, quarter, (int)sourceYear, (int)sourceQuarter, DataSourceType.Quarter);
                                    if (curDist < distance)
                                    {
                                        distance = curDist;
                                        sourceId = source.Key;
                                    }
                                }

                                break;
                            }
                        ////[Description("Год месяц")]
                        case ParamKindTypes.YearMonth:
                        case ParamKindTypes.YearMonthTerritory:
                        case ParamKindTypes.YearMonthVariant:
                        case ParamKindTypes.YearVariantMonthTerritory:
                            {
                                int? sourceYear = SourceData(source.Key, DataSourceType.Year);
                                int? sourceMonth = SourceData(source.Key, DataSourceType.Month);
                                if (sourceYear != null && sourceMonth != null)
                                {
                                    curDist = Distance(year, month, (int)sourceYear, (int)sourceMonth, DataSourceType.Month);
                                    if (curDist < distance)
                                    {
                                        distance = curDist;
                                        sourceId = source.Key;
                                    }
                                }

                                break;
                            }
                        ////[Description("Год квартал месяц")]
                        case ParamKindTypes.YearQuarterMonth:
                            {
                                int? sourceYear = SourceData(source.Key, DataSourceType.Year);
                                int? sourceMonth = SourceData(source.Key, DataSourceType.Month);
                                int? sourceQuarter = SourceData(source.Key, DataSourceType.Quarter);
                                if (sourceYear != null && sourceMonth != null && sourceQuarter != null)
                                {
                                    curDist = Distance(year, quarter, month, (int)sourceYear, (int)sourceMonth, (int)sourceQuarter);
                                    if (curDist < distance)
                                    {
                                        distance = curDist;
                                        sourceId = source.Key;
                                    }
                                }

                                break;
                            }
                    }
                }
            }

            return sourceId;
        }
        
        /// <summary>
        /// Получает значение параметра источника по его типу
        /// </summary>
        /// <param name="sourceId">Ключ источника</param>
        /// <param name="dataSourceType">Тип параметра</param>
        /// <returns>Значение параметра</returns>        
        private int? SourceData(int sourceId, DataSourceType dataSourceType)
        {
            if (Scheme.DataSourceManager.DataSources.Contains(sourceId))
            {
                switch (dataSourceType)
                {
                    case DataSourceType.Year:
                        {
                            return Scheme.DataSourceManager.DataSources[sourceId].Year;
                        }

                    case DataSourceType.Month:
                        {
                            return Scheme.DataSourceManager.DataSources[sourceId].Month;
                        }
                    
                    case DataSourceType.Quarter:
                        {
                            return Scheme.DataSourceManager.DataSources[sourceId].Quarter;
                        }
                }
            }

            return null;
        }
    }
}
