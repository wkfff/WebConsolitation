using System;
using System.Text;
using System.Xml;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using System.Collections.Generic;


namespace Krista.FM.Client.MDXExpert.Data
{
    public class MDXQueryBuilder
    {
        #region Приватные поля и перечисления
        //Тип элемента, для которого подготавливается запрос
        private ReportElementType elementType = ReportElementType.eTable;        
        /// <summary>
        /// Название для сета строк уровня сессии подключения
        /// </summary>
        const string RowsSessionSetName = "[SessionRows]";
        /// <summary>
        /// Не отфильтрованное от лишних кортежей множество оси строк
        /// </summary>
        const string NonFilterRowSetName = "[NonFilterRowSet]";
        /// <summary>
        /// Имя для отфильтрованного множества элементов последнего уровня на оси строк
        /// </summary>
        const string ChildRowsetName = "[ChildRowset]";
        /// <summary>
        /// Не отфильтрованное от лишних кортежей множество оси столбцов
        /// </summary>
        const string NonFilterColumnSetName = "[NonFilterColumnSet]";
        /// <summary>
        /// Не отфильтрованное от лишних кортежей множество оси столбцов
        /// </summary>
        const string NonFilterSerieSetName = "[NonFilterSerieSet]";
        /// <summary>
        /// Не отфильтрованный от лишних кортежей заголовок оси строк
        /// </summary>
        const string NonFilterHeadRowSetName = "[NonFilterHeaderRowSet]";
        //Использовать постраничный вывод
        public bool usePaging = false;
        //нижняя граница постраничного вывода
        public int lowBorder = 0;
        //Кол-во элементов в постраничном выводе
        public int pageSize = 0;
        //Начальный сет строк. Если он указан, то добавляется в начало штатного набора оси строк.
        //Нужно для постраничного режима.
        public string headRowSet = string.Empty;       
        //Использовать ли при сборке запроса заранее подготовленное в сессии множество строк
        //ответственность за фактическое наличие этго множества в сессии лежит на вызывающем модуле.
        public bool useRowsSessionSet = false;                      
        //разделитель запятая
        const string commaDelim = ", ";
        //разделитель звездочка
        const string asterixDelim = " * ";
        //имя фиктивной меры - пустышки
        const string dummyMeasureName = " ";
        //имя для сета-слайсера (для множественных фильтров)
        const string slicerName = "Slicer";
        /*10395*/
        //признаки, что мы насильно упорядочили данные в оси
        private bool isOrderedRowAxis = false;
        private bool isOrderedColumnAxis = false;

        //строка форматирования по умолчанию
        private string defaultFormatString = "# ##,0.00";

        //текущий режим констриуирование оси
        private WithBuildMode currentBuildMode;
        
        //Данные таскалок
        private PivotData pivot;
        //Куб элемента
        private CubeDef cube;
        
        private List<string> penultimateLevelMembers = new List<string>();

        #endregion

        /// <summary>
        /// Элементы предпоследнего уровня оси строк. Сейчас используются для расчета средних значений
        /// </summary>
        public List<string> PenultimateLevelMembers
        {
            get { return this.penultimateLevelMembers; }
            set { this.penultimateLevelMembers = value; }
        }

        #region Утилиты сборки MDX
        
        /// <summary>
        /// Конкатенация при условии непустой первой части
        /// </summary>
        private string AddTail(string head, string tail)
        {
            return (!string.IsNullOrEmpty(head))? head + tail: string.Empty;       
        }

        /// <summary>
        /// Конкатенация при условии непустой второй части
        /// </summary>
        private string AddHead(string head, string tail)
        {
            return (!string.IsNullOrEmpty(tail)) ? head + tail : string.Empty;
        }        

        /// <summary>
        /// Взятие в скобки.       
        /// </summary>
        /// <param name="src">содержимое</param>
        /// <param name="leftBracket">символ левой скобки</param>
        /// <param name="rightBracket">символ правой скобки</param>        
        private string _Brackets(string src, char leftBracket, char rightBracket)
        {
            if (String.IsNullOrEmpty(src))
                return string.Empty;
                
            if ((src[0] == leftBracket) && (src[src.Length - 1] == rightBracket))
                return src;

            return leftBracket + src + rightBracket;
        }

        /// <summary>
        /// Взятие строки в сет-конструктор
        /// Пользоваться остарожно, из за оптимизации. 
        /// Например, такую структуру {..}*{..} не закавычит
        /// </summary>
        /// <param name="src">содержимое множества</param>        
        private string SetBrackets(string src)
        {
            return _Brackets(src, '{', '}');
        }

        /// <summary>
        /// Взятие в скобки мембера
        /// Пользоваться остарожно, из за оптимизации. 
        /// Например, такую структуру [..] .. [..] не закавычит
        /// </summary>
        /// <param name="src">содержимое</param>        
        private string MemberBrackets(string src)
        {
            return _Brackets(src, '[', ']');
        }

        /// <summary>
        /// Взятие строки кортежа
        /// Пользоваться остарожно, из за оптимизации. 
        /// Например, такую структуру (..)..(..) не закавычит        
        /// </summary>
        /// <param name="src">содержимое</param>        
        private string TupleBrackets(string src)
        {
            return _Brackets(src, '(', ')');
        }

        /// <summary>
        /// Скрывать ли пустые позиции в оси
        /// </summary>
        private bool HideEmptyPositions(QueryAxisType axType)
        {
            if (this.IsTableElement && (pivot.TotalAxis.Totals.Count == 0))
            {
                //Если нет показателей, то пустые скрывать не будем никогда, что бы
                //пользователь видел оси, если только они и вытащены
                return false;
            }
            else
            {
                //берем из настроек 
                //для объектов карты берем и пустые элементы тоже
                if (this.IsMapElement && (GetPivotAxis(axType).AxisType == AxisType.atRows))
                {
                    return false;
                }
                else
                {
                    return GetPivotAxis(axType).HideEmptyPositions;
                }
            }
        }
        
        /// <summary>
        /// Этот юник-нэйм принадлежит измерению мер
        /// </summary>
        private bool IsMeasureUName(string uName)
        {
            return uName.StartsWith("[Measures].", true, System.Globalization.CultureInfo.CurrentCulture);
        }
        
        /// <summary>
        /// Возвращает MDX-выражение для множества потомков мембера
        /// </summary>
        /// <param name="un">у-нэйм мембера</param>
        /// <param name="includeSelf">включать сам мембер, или только потомков</param>
        private string GetDescendantsSet(string un, bool includeSelf)
        {
            //пустой юник нэйм дает пустое множество
            if (un == string.Empty) 
                return "{}";
            //у-нэйм меры дает сам себя
            if (IsMeasureUName(un)) 
                return un; 
            
            if (includeSelf)
            {
                return string.Format("Descendants({0}, {0}.Level, SELF_AND_AFTER)", un);
            }
            else
	        {
                return string.Format("Descendants({0}, 0, AFTER)", un);
	        }
        }

        /// <summary>
        /// Будет ли фактически использована ф-я NonEmptyCrossJoin для даннной оси
        /// </summary>        
        private bool UseNECJinFact(QueryAxisType axType)
        {
            //Ответ "да", если по данной оси нужно скрывать пустые позиции
            //и при этом функция разрешена настройками
            return ((this.GetPivotAxis(axType).HideEmptyMode == HideEmptyMode.NonEmptyCrossJoin)
                && HideEmptyPositions(axType));
        }

        /// <summary>
        /// Сцепляет два выражения сета через разделитель.
        /// Разделитель зависит от настроек запроса
        /// </summary>
        private string ConcatenateSet(string srcSet, string partSet, QueryAxisType axType)
        {
            //Соединяем строки через разделитель, который вычисляем так:
            //Если будет использована ф-я NonEmptyСrossJoin, тогда разделитель - "запятая",
            //в противном случае - "звездочка", поскольку будет простой set-constructor
            string delimiter = UseNECJinFact(axType) ? commaDelim : asterixDelim;
            if (partSet != string.Empty)
            {
                return AddTail(srcSet, delimiter) + partSet;
            }
            else
            {
                return srcSet;
            }
        }

        /// <summary>
        /// Накладывает фильтр на исключение левых картежей (детализирующих нелистья)
        /// </summary>
        private string MakeMissplacedTuplesFilter(QueryAxisType axType, string baseSet, bool canSortAdd)
        {
            string result = baseSet;
            PivotAxis ax = this.GetPivotAxis(axType);
            //Навешиваем условие на отбрасывание "левых" кортежей, которые детализируют
            //не листовые элементы, такие кортежи нам не нужны.
            string misplacedTupleConditions = HiddenTuplesFilterCondition(axType);
            if (misplacedTupleConditions != string.Empty)
            {
                result = string.Format("Filter({0}, {1})", baseSet, misplacedTupleConditions);
                /*10395 Если случилось так что у элемента таблица режим скрытия пустых Non Empty, 
                 и в одном (или нескольких) измерений нет уровня олл, были вынуждены сначала 
                 отсортировать все по убыванию чтобы не пустые элементы оказались сверху, и мы
                 могли правильно отфильтровать лишние кортежи. Теперь если у измерения была какая либо 
                 сортировка выставляем ее, если не было возвращаем все на место*/
                if (this.IsTableElement)
                {
                    if (((axType == QueryAxisType.axRows) && this.isOrderedRowAxis) ||
                        (axType == QueryAxisType.axColumns) && this.isOrderedColumnAxis)
                    {
                        if (ax.IsSorted)
                            result = this.AddAxisOrder(ax, result);
                        else
                            result = "Hierarchize(" + result + ")";
                    }
                }
            }
            //result = MakeDataMemberFilter(axType, result);

            if (canSortAdd)
                result = SortedAxisSet(axType, result);

            return result;
        }

        /// <summary>
        /// Накладывает фильтр на исключение дата мемберов
        /// </summary>
        /// <param name="axType"></param>
        /// <param name="baseSet"></param>
        /// <returns></returns>
        private string MakeDataMemberFilter(QueryAxisType axType, string baseSet)
        {
            string result = baseSet;
            PivotAxis ax = this.GetPivotAxis(axType);
            string dataMemberFilter = string.Empty;

            foreach(FieldSet fs in ax.FieldSets)
            {
                if (!fs.IsVisibleDataMembers)
                {
                    dataMemberFilter = AddTail(dataMemberFilter, " and ");
                    dataMemberFilter += string.Format("not {0}.CurrentMember is {0}.CurrentMember.Parent.DataMember", fs.UniqueName);

                    continue;
                }
                foreach(PivotField f in fs.Fields)
                {
                    if (!f.IsVisibleDataMember)
                    {
                        dataMemberFilter = AddTail(dataMemberFilter, " and ");
                        dataMemberFilter += string.Format("(not {0}.CurrentMember is {0}.CurrentMember.Parent.DataMember or " +
                                                            "not {0}.CurrentMember.Level is {1})", fs.UniqueName, f.UniqueName);
                    }
                }
            }

            if (dataMemberFilter != string.Empty)
            {
                result = string.Format("Filter({0}, {1})", result, dataMemberFilter);
            }

            return result;
        }

        /// <summary>
        /// Возвращает ось, по ее типу
        /// </summary>        
        private PivotAxis GetPivotAxis(QueryAxisType axType)
        {
            return ((axType == QueryAxisType.axColumns)||(axType == QueryAxisType.axSeries)) ? pivot.ColumnAxis : pivot.RowAxis;
        }
        
        /// <summary>
        /// Преобразует юник-нэйм измерения в юник-нэйм вычислимого сета, 
        /// который может быть сопоставлен 1:1 с этим измерением
        /// </summary>
        /// <param name="dimUName">Юник-нэйм измерения</param>
        /// <returns>Имя вычислимого множества</returns>
        private string CalcSetNameForDim(string dimUName)
        {
            //алгоритмы тут могут быть разные, мы будем просто заменять в у-нэйме
            //убираем квадратные скобки. При этом и недопустимых символов не будет
            //соответствие сохранитья 
            string result = dimUName;
            result = result.Replace('[', ' ');
            result = result.Replace(']', ' ');
            return MemberBrackets(result + " LEVELS");
        }       
        
        /// <summary>
        /// Нужно ли использовать в запросе фиктивную меру - пустышку
        /// </summary>        
        private bool NeedDummyMeasure()
        {
            //Пустышку используем для таблицы, если нормальных мер нет. Для диаграммы не используем
            return (pivot.TotalAxis.IsEmpty && this.IsTableElement);
        } 
        
        /// <summary>
        /// Является ли мембер включаемым? Может быть и исключаемым, завист от предка
        /// </summary>
        private bool ItsIncludedMember(XmlNode mem)
        {
            if (mem.ParentNode == null)
            {
                return true; //по умолчанию считаем что да.
            }
            else
            {
                return (XmlHelper.GetStringAttrValue(mem.ParentNode, "childrentype", "") == "included");
            }
        }

        /// <summary>
        /// Возвращает У-нэйм первого попавшегося выбранного элемнта из переданного измерения
        /// </summary>
        private string GetSingleFilterMemberUName(FieldSet fs)
        {
            if (fs == null) return string.Empty;
            
            XmlNode node = fs.MemberNames.SelectSingleNode(".//*[(@uname) and not(*)]");
            return XmlHelper.GetStringAttrValue(node, "uname", string.Empty);

        }

        /// <summary>
        /// Имя вычислимого сета для множественного фильтра
        /// </summary>
        private string GetMultipleFilterSetName(FieldSet fs)
        {
            if (fs == null)
            {
                return string.Empty;
            }
            else
            {
                return string.Format("{0}.[{1}]", fs.UniqueName, slicerName);
            }
        }

        /// <summary>
        /// Имя вычислимого сета для расчета итога по видимым
        /// </summary>
        private string GetVisualTotalsSetName(FieldSet fs)
        {
            if (fs == null)
            {
                return string.Empty;
            }
            else
            {
                string result = string.Format("{0}.[{1}]", fs.UniqueName, "VisualTotals");
                return result.Replace("].[", "_");
            }
        }


        /// <summary>
        /// Условия на непустые значения для таблицы и диаграммы будут выглядеть по разному
        /// </summary>        
        private string GetNonEmptyCondition(PivotAxis ax)
        {
            switch (ElementType)
            {
                case ReportElementType.eChart:
                case ReportElementType.eMultiGauge:
                    return this.NonEmptyConditionForAxis(ax);
                case ReportElementType.eTable: return this.NonEmptyConditionForTable(ax);
            }
            return "true";
        }

        private string NonEmptyConditionForTable(PivotAxis ax)
        {
            if (pivot.TotalAxis.VisibleTotals.Count == 0)
                return "true";

            string condition = string.Empty;

            foreach (PivotTotal measure in pivot.TotalAxis.VisibleTotals)
            {
                condition = AddTail(condition, " or ");
                if ((ax.AxisType == AxisType.atRows) && (((this.usePaging) && (this.lowBorder > 0)) || this.currentBuildMode == WithBuildMode.ForRecordCount))
                {
                    condition += string.Format("not IsEmpty(({0} {1}))", measure.UniqueName, AddHead(commaDelim, FilterExpressionForSessionRows()));
                }
                else
                {
                    condition += string.Format("not IsEmpty({0})", measure.UniqueName);
                }
            }
            return TupleBrackets(condition);
        }

        private string NonEmptyConditionForAxis(Data.Axis ax)
        {
            string condition = string.Empty;
            if (ax.FieldSets.Count > 0)
            {
                foreach (FieldSet fieldSet in ax.FieldSets)
                {
                    if (condition != string.Empty)
                        condition += ",";
                    condition += string.Format("{0}.CurrentMember", fieldSet.UniqueName);
                }
                condition = "not IsEmpty((" + condition + "))";
            }
            return TupleBrackets(condition);
        }

        /// <summary>
        /// Кодирует символы запроса, пользоваться осторожно, т.к. в секции 
        /// With используются апострофы, их дублировать не надо.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private string EncodeSet(string query)
        {
            string result = query;
            if (result != string.Empty)
            {
                result = result.Replace("'", "''");
            }
            return result;
        }

        #endregion
                               
        #region WITH - Сборка предварительных вычислений
        
        /// <summary>
        /// MDX-выражение для множества всех элементов на включенных уровнях
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        private string DimensionCheckedLevelsSet(FieldSet fs)
        {
            //формируем перечисление уровней
            string levels = string.Empty;
            string levelUName = string.Empty;

            foreach (PivotField f in fs.Fields)
            {
                if (!f.IsIncludeToQuery)
                    continue;

                levelUName = f.UniqueName;
                levels = AddTail(levels, commaDelim);
                levels += string.Format("{0}.AllMembers", levelUName);
            }

            //Вообще-то межэлементыные итоги нужно добавлять не через фильтры по уровням, а явно.
            //Поскольку узловой элемент мемберов может быть excluded, тогда итоги не попадут.
            /*Убрана проверка, для того, подсчет общиг итогов по видимым элементам был корректен*/
            //if (fs.ParentCollection[0] != fs)
            //для карты не нужны общие итоги
            //if (!IsMapElement)
            {                        
                levels = AddTail(levels, commaDelim);
                levels += fs.GrandMemberUN;
            }
                  
            return SetBrackets(levels);   
        }
        
        /// <summary>
        /// Передается FieldSet и для соответсвующего измерения вернет
        /// определение множества всех мемберов включенных уровней
        /// </summary>
        private string DimensionCheckedLevelsSetDefinition(FieldSet fs)
        {            
            string setTemplate = " SET {0} as '{1}'";
            return string.Format(setTemplate, CalcSetNameForDim(fs.UniqueName), DimensionCheckedLevelsSet(fs));
        }
        
        /// <summary>
        /// Выражение для группы вычислимых сетов, для каждого измерения в оси.
        /// В каждый такой сет включаются все элементы этого измерения, 
        /// расположенные на включенных уровнях
        /// </summary>
        private string AxisSetsByCheckedLevels(QueryAxisType axType)
        {
            string result = string.Empty;
            PivotAxis ax = this.GetPivotAxis(axType);

            //собираем для каждого измерения
            foreach (FieldSet fs in ax.FieldSets)
            {
                result += DimensionCheckedLevelsSetDefinition(fs);                    
            }
            return result;
        }        
        
        /// <summary>
        /// Выражение для группы вычислимых сетов, для каждого измерения используемого в осях.
        /// В каждый такой сет включаются все элементы этого измерения, 
        /// расположенные на включенных уровнях
        /// </summary>
        private string AxesSetsByCheckedLevels()
        {
            return AxisSetsByCheckedLevels(QueryAxisType.axColumns) +
                AxisSetsByCheckedLevels(QueryAxisType.axRows);
        }
        
        /// <summary>
        /// Объявление фиктивной меры - пустышки.
        /// Строк без столбцов в MDX-запросе быть не может. А вот в программе, впролне допустимо.
        /// Оси столбцов нет, когда мы туда не вынесили ни одного измерения, и показатели тоже "забыли".
        /// В этом случае, что бы правильно отобразились строки, будем по столбцам выбирать
        /// фиктивную меру.
        /// </summary>        
        private string DummyMeasureDeclaration()
        {
            //return string.Format(" MEMBER [Measures].[{0}] as '\"\"'", dummyMeasureName);
            return string.Format(" MEMBER [Measures].[{0}] as 'null'", dummyMeasureName);
        }

        /// <summary>
        /// Получить множестов всех детей
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private string GetChildrenSet(XmlNode parent)
        {
            string result = string.Empty;
            if (parent != null)
            {
                foreach (XmlNode child in parent.ChildNodes)
                {
                    //если включает детей, то дети должны быть без потомков
                    if (!(this.IsChildrenIncluding(parent) && child.HasChildNodes))
                    {
                        if (result != string.Empty)
                            result += ",";
                        result += XmlHelper.GetStringAttrValue(child, "uname", string.Empty);
                    }
                }
            }
            return this.SetBrackets(result);
        }

        /// <summary>
        /// Описание множественного фильтра
        /// </summary>
        private string FullCheckedElementsSet(XmlNode root, string dimUName, bool isIncludeParents)
        {
            if (root == null)
                return string.Empty;

            string childrenEnum = string.Empty;
            string un = XmlHelper.GetStringAttrValue(root, "uname", "");

            //Формируем множество потомков корня
            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.HasChildNodes)
                {
                    string resultSet = FullCheckedElementsSet(node, dimUName, isIncludeParents);
                    if (childrenEnum != string.Empty)
                    {
                        childrenEnum += commaDelim;
                    }
                    childrenEnum += resultSet;
                }
            }            

            if (un == string.Empty) //это может быть тоолько корень всей структуры
            {
                if (IsChildrenIncluding(root))
                {
                    return SetBrackets(childrenEnum);
                }
                else
                {
                    return String.Format("Except({0}.Levels(0).Members, {{{1}}})", dimUName, childrenEnum);
                }
            }

            string childSet = this.GetChildrenSet(root);

            if (IsChildrenIncluding(root))
            {
                string result = String.Empty;

                if ((childrenEnum != string.Empty) && (childSet != string.Empty))
                {
                    result = SetBrackets(childrenEnum) + ", " + childSet;
                }
                else
                {
                    result = childrenEnum + childSet;
                }

                if (isIncludeParents)
                    result = un + ", " + result;
                return result;

            }
            else
            {
                if (childrenEnum == string.Empty)
                    return isIncludeParents
                               ? String.Format("{0}, Except({0}.Children , {{{1}}})", un, childSet)
                               : String.Format("Except({0}.Children , {{{1}}})", un, childSet);
                else
                    return isIncludeParents
                               ? String.Format("{0}, Except({0}.Children , {{{1}}}), {2}", un, childSet, childrenEnum)
                               : String.Format("Except({0}.Children , {{{1}}}), {2}", un, childSet, childrenEnum);
            }
        }        
        
        /// <summary>
        /// Выражения для сетов множественных фильтров
        /// </summary>        
        private string MultipleFilterSets()
        {
            string resSets = string.Empty;
            string slicerValue = string.Empty;
            string slicerName = string.Empty;
            
            //для каждого множественного фильтра
            foreach (FieldSet fs in pivot.FilterAxis.FieldSets)
            {
                if (fs.IsMultipleChoise())
                {
                    slicerName = GetMultipleFilterSetName(fs);

                    slicerValue = "{" + FullCheckedElementsSet(fs.MemberNames, fs.UniqueName, false) + "}";

                    slicerValue = this.EncodeSet(slicerValue);

                    resSets += string.Format(" MEMBER {0} AS 'AGGREGATE({1})'", slicerName, slicerValue);
                }
            }

	        return resSets;
        }

        /// <summary>
        /// Выражения для сетов множественных фильтров
        /// </summary>        
        private string MultipleFilterSets(FieldSetCollection fieldSets)
        {
            string resSets = string.Empty;
            string slicerValue = string.Empty;
            string slicerName = string.Empty;

            //для каждого множественного фильтра
            foreach (FieldSet fs in fieldSets)
            {
                if (fs.IsMultipleChoise())
                {
                    slicerName = GetMultipleFilterSetName(fs);
                    if (this.currentBuildMode != WithBuildMode.ForRecordCount)
                        slicerName = String.Format("{0}.{1}", MemberBrackets(cube.Name), slicerName);

                    slicerValue = "{" + FullCheckedElementsSet(fs.MemberNames, fs.UniqueName, false) + "}";

                    slicerValue = this.EncodeSet(slicerValue);

                    resSets += string.Format(" MEMBER {0} AS 'AGGREGATE({1})'", slicerName, slicerValue);
                }
            }

            return resSets;
        }

        /// <summary>
        /// Выражения для множественных фильтров сессионных сетов
        /// </summary>
        /// <returns></returns>
        private string MultipleFiltersForRowSessionSets()
        {
            string resSets = string.Empty;
            resSets += MultipleFilterSets(this.pivot.FilterAxis.FieldSets);
            resSets += MultipleFilterSets(this.pivot.ColumnAxis.FieldSets);
            return resSets;
        }

        /// <summary>
        /// Получения выражения для вычисления итога по видимым с помощью функции VisualTotals для множеств элементов филдсетов конкретной оси
        /// </summary>
        /// <returns></returns>
        private string GetVisualTotalsSet(Axis axis)
        {
            string resSets = String.Empty;

            foreach (FieldSet fs in axis.FieldSets)
            {
                if (fs.UniqueName == "[Measures]")
                    continue;

                string memberSetName = GetVisualTotalsSetName(fs);

                string memberSet = "{" + FullCheckedElementsSet(fs.MemberNames, fs.UniqueName, true) + "}";
                memberSet = this.EncodeSet(memberSet);
                resSets += string.Format(" SET {0} AS 'VISUALTOTALS( HIERARCHIZE({1}))'", memberSetName, memberSet);
            }
            return resSets;
        }

        /// <summary>
        /// Получения выражения для вычисления итога по видимым с помощью функции VisualTotals для множеств элементов филдсетов всех осей (кроме фильтров)
        /// </summary>
        /// <returns></returns>
        private string GetVisualTotalsSet()
        {
            string result = String.Empty;

            if (((this.currentBuildMode != WithBuildMode.ForRecordCount) && this.pivot.IsVisualTotals) &&
                (!IsMapElement))
            {
                result += GetVisualTotalsSet(this.pivot.ColumnAxis);
                result += GetVisualTotalsSet(this.pivot.RowAxis);
                result += GetVisualTotalsSet(this.pivot.TotalAxis);
            }

            return result;
        }

        /// <summary>
        /// Секция предварительных вычислений запроса
        /// </summary>        
        private string WithClause(WithBuildMode buildMode)
        {
            //Сейчас фильтровые сеты по уровням не описываются отдельными предвычисленимями, идут прямо в запросе.            
            //string result = AxesSetsByCheckedLevels() + MultipleFilterSets();            
            string result = string.Empty;

            result = GetVisualTotalsSet();

            //когда множество созданно, в нем созданы и вычислимые мемберы
            if (buildMode != WithBuildMode.ForRowSesionSet)
            {
                result += this.GetChildRowsets();
                result += this.GetCalcMembersDeclaration(false);
                result += this.GetAverageMemberExpression(false);
                result += this.GetStandartDeviationMemberExpression(false);
                result += this.GetMedianMemberExpression(false);
                result += this.GetTopCountMeasuresExpression();
                result += this.GetBottomCountMeasuresExpression();
            }

            result += this.MultipleFilterSets();

            if (this.NeedDummyMeasure())
                result += this.DummyMeasureDeclaration();

            /*10395*/
            //сбросим признаки упорядочивания данных в осях
            this.isOrderedRowAxis = false;
            this.isOrderedColumnAxis = false;
            result += this.NonFilterAxisSetsDeclaration(buildMode);

            return AddHead("WITH ", result);
        }

        /// <summary>
        /// Здесь добавляем не отфильтрованные от лишних кортежей множества осей
        /// </summary>
        /// <param name="buildMode"></param>
        /// <returns></returns>
        private string NonFilterAxisSetsDeclaration(WithBuildMode buildMode)
        {
            string result = "";

            switch (buildMode)
            {
                case WithBuildMode.ForRecordCount:
                    return this.GetNonFilterAxisSet(QueryAxisType.axRows, this.GetHideEmptyMode(this.pivot.RowAxis, true)) + MultipleFiltersForRowSessionSets();;
                case WithBuildMode.ForRowSesionSet:
                    {
                        return 
                            //колонки
                            this.GetNonFilterAxisSet(QueryAxisType.axColumns, this.pivot.ColumnAxis.HideEmptyMode) + 
                            //заголовок оси строк
                            this.GetNonFilterHeaderRowSet();
                    }
                case WithBuildMode.Standart:
                    {
                        result =  
                            //колонки
                            this.GetNonFilterAxisSet(QueryAxisType.axColumns,
                            this.pivot.ColumnAxis.HideEmptyMode) +
                            //строки
                            this.GetNonFilterAxisSet(QueryAxisType.axRows,
                            this.GetHideEmptyMode(this.pivot.RowAxis, false));

                        if (IsMapElement)
                        {
                            result +=
                            //серии
                            this.GetNonFilterAxisSet(QueryAxisType.axSeries,
                            this.pivot.ColumnAxis.HideEmptyMode);
                        }
                        return result;
                    }
            }
            return string.Empty;
        }

        /// <summary>
        /// Получить множество заголовка строк
        /// </summary>
        /// <returns></returns>
        private string GetNonFilterHeaderRowSet()
        {
            string result = string.Empty;
            if (this.headRowSet != string.Empty)
            {
                result = string.Format(" SET {0} AS '{1}'", NonFilterHeadRowSetName,
                    this.headRowSet);
            }
            return result;
        }

        /// <summary>
        /// Получить множество указанной оси
        /// </summary>
        /// <param name="axType"></param>
        /// <param name="hideEmptyMode"></param>
        /// <returns></returns>
        private string GetNonFilterAxisSet(QueryAxisType axType, HideEmptyMode hideEmptyMode)
        {
            string result = string.Empty;

            if (this.IsIncludeAxis(axType))
            {
                result = string.Format(" SET {0} AS '{1}'", this.GetNonFilterSetName(axType),
                    this.NonFilterAxisSet(axType, hideEmptyMode));
            }
            return result;
        }

        private string GetCalcMembersDeclaration(bool isIncludeCubeName)
        {
            string result = string.Empty;

            for(int i = 0; i < this.pivot.TotalAxis.Totals.Count; i++)
            {
                string totalClause = this.GetCalcMemberDeclaration(i, true, isIncludeCubeName);
                if (totalClause != string.Empty)
                {
                    result += totalClause;
                }
            }
            return result;
        }

        private string GetLookupMemberName(PivotTotal total)
        {
            if ((total.IsCustomTotal)&&(total.IsLookupMeasure))
            {
                return String.Format("[Measures].[lc_{0}]", GetNameFromUniqueName(total.UniqueName));
            }
            return String.Empty;
        }

        /// <summary>
        /// Получение формулы для вычислимой меры
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        public string GetCalcMemberExpression(PivotData pivotData, PivotTotal total)
        {
            if (total == null)
                return String.Empty;

            InitFields(pivotData);

            string result = String.Empty;

            if (total.FormulaType != MeasureFormulaType.Custom)
            {
                this.pivot.IsDeferDataUpdating = true;
                if ((total.IsLookupMeasure))// && (total.FormulaType == MeasureFormulaType.None))
                {
                    //если применять к вытащешной из другого куба мере никаких вычислений не будем
                    //то просто оставляем выражение для lookupcube
                    total.Expression = GetLookupCubeExpression(total);
                }
                else
                {
                    total.Expression = GetCalcMemberTypicalExpression(total);
                }

                this.pivot.IsDeferDataUpdating = false;
                result = total.Expression;
            }
            return result;
        }

        private string GetCalcMemberDeclaration(int totalIndex, bool isIncludeExpression, bool isIncludeCubeName)
        {
            if ((totalIndex >= 0) && (totalIndex < this.pivot.TotalAxis.Totals.Count))
            {
                PivotTotal total = this.pivot.TotalAxis.Totals[totalIndex];
                if (total.IsCustomTotal)
                {
                    string result = total.UniqueName;
                    if (isIncludeCubeName)
                        result = string.Format("{0}.{1}", MemberBrackets(cube.Name), result);
                    result = string.Format(" MEMBER {0}", result);

                    if (isIncludeExpression)
                    {
                        if (total.FormulaType != MeasureFormulaType.Custom)
                        {
                            this.pivot.IsDeferDataUpdating = true;

                            if ((total.IsLookupMeasure) && (total.FormulaType == MeasureFormulaType.None))
                            {
                                //если применять к вытащешной из другого куба мере никаких вычислений не будем
                                //то просто оставляем выражение для lookupcube
                                total.Expression = GetLookupCubeExpression(total);
                            }
                            else
                            {
                                if (total.IsLookupMeasure)
                                {
                                    //Если берем меру из другого куба, то нужно создать ее(то есть дополнительный вычислимый элемент)
                                    result = string.Format("MEMBER {0} AS '{1}' {2}",
                                                           GetLookupMemberName(total),
                                                           GetLookupCubeExpression(total), result);
                                }

                                total.Expression = GetCalcMemberTypicalExpression(total);
                            }

                            this.pivot.IsDeferDataUpdating = false;
                        }

                        result = string.Format("{0} AS '{1}'", result, total.Expression);
                    }

                    return result;
                }
            }
            return string.Empty;
        }

        //очищаем имя от скобок
        private string ClearBrackets(string source)
        {
            source = source.Replace("[", "");
            int lastBracketPos = source.LastIndexOf(']');
            if (lastBracketPos > 0)
            {
                source = source.Remove(lastBracketPos);
            }
            return source; 
        }

        //получение имени объекта по юникнейму
        private string GetNameFromUniqueName(string uniqueName)
        {
            string[] arrStr = uniqueName.Split('[');
            if (arrStr.Length > 0)
            {
                return ClearBrackets(arrStr[arrStr.Length - 1]);
            }

            return uniqueName;
        }

        /// <summary>
        /// Проверяем есть ли такая мера(в том числе и вычислимая)
        /// </summary>
        /// <param name="measureName">юник нейм меры</param>
        /// <returns></returns>
        private bool CheckMeasure(string measureName)
        {
            if (this.pivot.Cube.Measures.Find(GetNameFromUniqueName(measureName)) == null)
            {
                if (!this.pivot.TotalAxis.TotalIsPresent(measureName))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// получение типовой формулы для вычислимой меры
        /// </summary>
        /// <returns></returns>
        private string GetCalcMemberTypicalExpression(PivotTotal total)
        {
            if (total.MeasureSource == "")
                return "null";
            /*
            try
            {
                if (!CheckMeasure(total.MeasureSource))
                    return "null";
            }
            catch(Exception exc)
            {
                if ((exc is AdomdException)&&(AdomdExceptionHandler.ProcessOK((AdomdException)exc)))
                {
                    AdomdExceptionHandler.IsRepeatedProcess = true;
                    bool checkMeasure = CheckMeasure(total.MeasureSource);
                    AdomdExceptionHandler.IsRepeatedProcess = false;
                    if (!checkMeasure)
                        return "null";
                }

            }*/

            string measureSource = total.IsLookupMeasure ? GetLookupMemberName(total) : total.MeasureSource;

            string rowsMembers = GetAxisAllMembers(this.pivot.RowAxis);
            string columnsMembers = GetAxisAllMembers(this.pivot.ColumnAxis);

            string rowCurrentMember = GetAxisCurrentMember(this.pivot.RowAxis);
            string rowCurrentMemberParent = GetAxisCurrentMemberParent(this.pivot.RowAxis);
            string rowCurrentMemberSibligs = GetAxisMembers(this.pivot.RowAxis);

            string colCurrentMember = GetAxisCurrentMember(this.pivot.ColumnAxis);
            string colCurrentMemberParent = GetAxisCurrentMemberParent(this.pivot.ColumnAxis);
            string colCurrentMemberSiblings = GetAxisMembers(this.pivot.ColumnAxis);


            switch (total.FormulaType)
            {
                case MeasureFormulaType.RowTotalPercent:
                    return String.Format("{0}/({0}{1})', format_string = '0.00%", measureSource, columnsMembers, colCurrentMember);
                case MeasureFormulaType.ColumnTotalPercent:
                    return String.Format("{0}/({0}{1})', format_string = '0.00%", measureSource, rowsMembers);
                case MeasureFormulaType.ParentRowPercent:
                    return rowCurrentMemberParent != "" ?
                        String.Format("IIF(IsEmpty(({2},{1})), null, IIf(not IsNull(({0})), {1}/({1}, ({0})), 1))', format_string = '0.00%", rowCurrentMemberParent, measureSource, rowCurrentMember) :
                        "1', format_string = '0.00%";
                case MeasureFormulaType.ParentColumnPercent:
                    return colCurrentMemberParent != "" ?
                        String.Format("IIF(IsEmpty(({2},{1})), null, IIf(not IsNull(({0})), {1}/({1}, ({0})), 1))', format_string = '0.00%", colCurrentMemberParent, measureSource, colCurrentMember) :
                        "1', format_string = '0.00%";
                case MeasureFormulaType.GrandTotalPercent:
                    return String.Format("{0}/({0}{1}{2})', format_string = '0.00%", measureSource, columnsMembers,
                                         rowsMembers);

                case MeasureFormulaType.GrandTotalRank:
                    return GetTotalRank(rowCurrentMember, colCurrentMember, measureSource, "BDESC");

                case MeasureFormulaType.GrandTotalInverseRank:
                    return GetTotalRank(rowCurrentMember, colCurrentMember, measureSource, "BASC");

                case MeasureFormulaType.ParentColumnRank:
                    return colCurrentMember != "" ?
                        String.Format("IIF(IsEmpty(({0},{2})), null, Rank(({0}), ORDER({1}, {2}, BDESC)))", colCurrentMember, colCurrentMemberSiblings, measureSource) :
                        String.Format("IIF(IsEmpty({0}), null, 1)", measureSource);
                case MeasureFormulaType.ParentColumnInverseRank:
                    return colCurrentMember != "" ?
                        String.Format("IIF(IsEmpty(({0},{2})), null, Rank(({0}), ORDER({1}, {2}, BASC)))", colCurrentMember, colCurrentMemberSiblings, measureSource) :
                        String.Format("IIF(IsEmpty({0}), null, 1)", measureSource);
                case MeasureFormulaType.ParentRowRank:
                    return rowCurrentMember != "" ?
                        String.Format("IIF(IsEmpty(({0},{2})), null, Rank(({0}), ORDER({1}, {2}, BDESC)))", rowCurrentMember, rowCurrentMemberSibligs, measureSource) :
                        String.Format("IIF(IsEmpty({0}), null, 1)", measureSource);
                case MeasureFormulaType.ParentRowInverseRank:
                    return rowCurrentMember != "" ?
                        String.Format("IIF(IsEmpty(({0},{2})), null, Rank(({0}), ORDER({1}, {2}, BASC)))", rowCurrentMember, rowCurrentMemberSibligs, measureSource) :
                        String.Format("IIF(IsEmpty({0}), null, 1)", measureSource);
                case MeasureFormulaType.None:
                    return measureSource;


            }
            return "null";
        }

        /// <summary>
        /// Есть ли иерархия в кубе
        /// </summary>
        /// <param name="cubeName"></param>
        /// <param name="hierarchyUniqueName"></param>
        /// <returns></returns>
        private bool IsCubeHasHierarchy(string cubeName, string hierarchyUName)
        {
            CubeDef cube = PivotData.AdomdConn.Cubes.Find(cubeName);
            if (cube != null)
            {
                foreach(Dimension dim in cube.Dimensions)
                {
                    foreach(Hierarchy h in dim.Hierarchies)
                    {
                        if (h.UniqueName == hierarchyUName)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Добавляем к частным фильтрам из другого куба элементы общих измерений
        /// </summary>
        /// <param name="cubeName"></param>
        /// <param name="axis"></param>
        /// <param name="filterSet"></param>
        /// <returns></returns>
        private string AddSharedDimensionsFromCube(PivotTotal total, PivotAxis axis, string filterSet)
        {
            
            //список общих измерений
            List<string> fsList = new List<string>();

            foreach (FieldSet fs in axis.FieldSets)
            {
                if (fs.UniqueName == "[Measures]")
                    continue;

                if (IsCubeHasHierarchy(total.LookupCubeName, fs.UniqueName))
                {
                    //тут еще надо проверить присутствует ли измерение в частных фильтрах, если да - то здесь включать не надо
                    if (total.Filters.SelectSingleNode("/fieldSet[@uname=\"" + fs.UniqueName + "\"]") == null)
                        fsList.Add(fs.UniqueName);
                }
            }

            if (fsList.Count == 0)
                return filterSet;

            //для меры из другого куба нужно по особому рассчитывать итоги
            if ((axis.AxisType == AxisType.atRows) || (axis.AxisType == AxisType.atColumns))
            {
                string filterExpression = String.Empty;
                foreach (string fsName in fsList)
                {
                    filterExpression = AddTail(filterExpression, " or ");
                    filterExpression += String.Format("{0}.CurrentMember is \" + {0}.CurrentMember.UniqueName + \"", fsName);
                }

                string axisMembers = String.Empty;
                if (this.IsMapElement)
                {
                    axisMembers = axis.AxisType == AxisType.atColumns ? NonFilterSerieSetName : NonFilterRowSetName;
                }
                else
                {
                    axisMembers = axis.AxisType == AxisType.atColumns ? NonFilterColumnSetName : NonFilterRowSetName;
                }

                filterSet = AddTail(filterSet, commaDelim);


                //Если для строк добавляются вычислимые элементы, то нужно их отфильтровать, т.к. в другом кубе их нет
                if (this.IsTableElement && (axis.AxisType == AxisType.atRows) && (this.pivot.IsNeedAverageCalculation() || this.pivot.IsNeedMedianCalculation()))
                {
                    string calcMembers = GetAverageMemberExpression(true);
                    string stDevMembers = GetStandartDeviationMemberExpression(true);
                    string medianMembers = GetMedianMemberExpression(true);

                    if (!String.IsNullOrEmpty(calcMembers) && !String.IsNullOrEmpty(stDevMembers))
                    {
                        calcMembers += ", " + stDevMembers;
                    }
                    else
                    {
                        calcMembers += stDevMembers;
                    }

                    if (!String.IsNullOrEmpty(calcMembers))
                    {
                        calcMembers += AddHead(", ", medianMembers);
                    }
                    else
                    {
                        calcMembers = medianMembers;
                    }

                    axisMembers = String.Format("Except({0}, {{{1}}})", axisMembers, calcMembers);
                }


                filterSet += String.Format("Filter(VisualTotals(\" + SetToStr({0}) + \"), {1}).Item(0)", axisMembers, filterExpression);
            }
            else
            {
                foreach (string fsName in fsList)
                {
                    filterSet = AddTail(filterSet, commaDelim);
                    filterSet += String.Format("\" + {0}.CurrentMember.UniqueName + \"", fsName);
                }
            }


            return filterSet;
        }

        /// <summary>
        /// Получение частных фильтров
        /// </summary>
        /// <param name="fsNode">xml с выбранными элементами</param>
        /// <param name="filterSet"></param>
        /// <returns></returns>
        private string AddPrivateFilter(XmlNode memberNames, string fsName, ref string filterSet)
        {
            string slicerValue = FullCheckedElementsSet(memberNames, fsName, false);
            if (slicerValue != String.Empty)
            {
                slicerValue = SetBrackets(this.EncodeSet(slicerValue));
                filterSet = String.Format("AGGREGATE({0}, {1})", slicerValue, filterSet);
            }

            return filterSet;
        }

        /// <summary>
        /// Получение формулы для вытаскивания меры из другого куба
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        private string GetLookupCubeExpression(PivotTotal total)
        {

            XmlNodeList fsNodes = total.Filters.SelectNodes("fieldSet");

            string resSets = total.MeasureSource;

            resSets = AddSharedDimensionsFromCube(total, this.pivot.RowAxis, resSets);
            resSets = AddSharedDimensionsFromCube(total, this.pivot.ColumnAxis, resSets);
            //resSets = AddSharedDimensionsFromCube(total, this.pivot.FilterAxis, resSets);

            resSets = TupleBrackets(resSets);

            foreach (FieldSet fs in this.pivot.FilterAxis.FieldSets)
            {
                AddPrivateFilter(fs.MemberNames, fs.UniqueName, ref resSets);
            }

            foreach (XmlNode fsNode in fsNodes)
            {
                XmlNode memberNames = fsNode.SelectSingleNode("dummy");
                string fsName = XmlHelper.GetStringAttrValue(fsNode, "uname", "");

                AddPrivateFilter(memberNames, fsName, ref resSets);
            }

            /*
            foreach (XmlNode fsNode in fsNodes)
            {
                XmlNode memberNames = fsNode.SelectSingleNode("dummy");
                string fsName = XmlHelper.GetStringAttrValue(fsNode, "uname", "");

                string slicerValue = FullCheckedElementsSet(memberNames, fsName);
                if (slicerValue != String.Empty)
                {
                    slicerValue = SetBrackets(this.EncodeSet(slicerValue));
                    resSets = String.Format("AGGREGATE({0}, {1})", slicerValue, resSets);
                }
            }*/


            resSets = TupleBrackets(resSets);

            string result = String.Format("LookupCube(\"{0}\", \"{1}\")", total.LookupCubeName, resSets);

            return result;
        }

        /// <summary>
        /// Получение элементов иерархии, соседних с текущим
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        private string GetCurrentMemberSiblings(FieldSet fs)
        {
            if (fs == null)
                return "";

            //собираем множество по выбранным мемберам           
            string dimSet = "{" + DimensionSet(fs.MemberNames, fs.UniqueName) + "}";

            return string.Format("Intersect({0}, {1}.CurrentMember.Siblings)", dimSet, fs.UniqueName);
        }

        /// <summary>
        /// Применить фильтр по видимым итогам
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        private string SetFilterByVisibleTotals(QueryAxisType axType, string currentSet)
        {
            string result = currentSet;

            PivotAxis ax = null;
            if (axType == QueryAxisType.axColumns)
            {
                ax = this.pivot.ColumnAxis;
            }
            if (axType == QueryAxisType.axRows)
            {
                ax = this.pivot.RowAxis;
            }
            if (ax == null)
                return result;

            string filterExpression = string.Empty;

            foreach (FieldSet fs in ax.FieldSets)
            {
                foreach (PivotField f in fs.Fields)
                {

                    if (!f.IsIncludeToQuery)
                        continue;

                    if (!f.IsVisibleTotal)
                    {
                        if (f.IsLastFieldInSet)
                            continue;

                        filterExpression = AddTail(filterExpression, " and ");
                        filterExpression += string.Format("not {0}.CurrentMember.Level is {1}", fs.UniqueName,
                                                              f.UniqueName);
                    }
                }
            }
            if ((!ax.GrandTotalVisible)&&(ax.FieldSets.Count > 0))
            {
                FieldSet fs = ax.FieldSets[0];
                if (fs.UniqueName != "[Measures]")
                {
                    filterExpression = AddTail(filterExpression, " and ");
                    filterExpression += string.Format("not {0}.CurrentMember is {1}", fs.UniqueName, fs.GrandMemberUN);
                }
            }

            if (filterExpression != string.Empty)
                result = String.Format("Filter({0}, {1})", result, filterExpression);
            return result;
        }

        private string GetAxisMembers(PivotAxis axis)
        {
            string result = "";
            foreach(FieldSet fs in axis.FieldSets)
            {
                string fsMembers = GetCurrentMemberSiblings(fs);
                if (fsMembers != "")
                {
                    result += (result != "") ? "*" + fsMembers : fsMembers;
                }
            }
            return result != "" ? String.Format("NonEmptyCrossjoin({0})", result) : "";
        }

        private string GetAxisAllMembers(PivotAxis axis)
        {
            string result = "";

            foreach (FieldSet fs in axis.FieldSets)
            {
                if (fs.AllMemberUN != "")
                {
                    result += String.Format(", {0}", fs.AllMemberUN);
                }
            }
            return result;
        }

        private string GetAxisCurrentMember(PivotAxis axis)
        {
            string curMember = "";
            
            foreach(FieldSet fs in axis.FieldSets)
            {
                if (!fs.UniqueName.Contains("[Measures]"))
                {
                    curMember = AddTail(curMember, commaDelim);
                    curMember += String.Format("{0}.CurrentMember", fs.UniqueName);
                }
            }
            return curMember; 
        }

        private string GetAxisCurrentMemberParent(PivotAxis axis)
        {
            FieldSet fieldSet = axis.FieldSets.GetLastItem();
            string curMember = fieldSet != null
                                                ? String.Format("{0}.CurrentMember.Parent", fieldSet.UniqueName)
                                                : "";
            return curMember;
        }


        /// <summary>
        /// Получение формулы ранга для общего итога
        /// </summary>
        /// <param name="rowCurrentMember"></param>
        /// <param name="colCurrentMember"></param>
        /// <param name="measureName"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private string GetTotalRank(string rowCurrentMember, string colCurrentMember, string measureName, string order)
        {
            string colMemberSet = "";
            if (this.IsIncludeAxis(QueryAxisType.axColumns))
            {   
                colMemberSet = IsMapElement ? this.GetNonFilterSetName(QueryAxisType.axSeries) : this.GetNonFilterSetName(QueryAxisType.axColumns);

                colMemberSet = MakeMissplacedTuplesFilter(QueryAxisType.axColumns, colMemberSet, false);
                colMemberSet = SetFilterByVisibleTotals(QueryAxisType.axColumns, colMemberSet);

            }
            string rowMemberSet = "";
            if (this.IsIncludeAxis(QueryAxisType.axRows))
            {
                rowMemberSet = this.GetNonFilterSetName(QueryAxisType.axRows);
                rowMemberSet = MakeMissplacedTuplesFilter(QueryAxisType.axRows, rowMemberSet, false);
                rowMemberSet = SetFilterByVisibleTotals(QueryAxisType.axRows, rowMemberSet);
            }

            if ((rowCurrentMember != "") && (colCurrentMember != ""))
            {
                return String.Format("IIF(IsEmpty(({0},{1},{4})), null, Rank(({0}, {1}), {{ORDER(NonEmptyCrossjoin({2}*{3}), {4}, {5})}}))",
                                     rowCurrentMember, colCurrentMember, rowMemberSet,
                                     colMemberSet, measureName, order);
            }
            else if ((rowCurrentMember != "") && (colCurrentMember == ""))
            {
                return String.Format("IIF(IsEmpty(({0},{2})), null, Rank(({0}), ORDER(NonEmptyCrossjoin({1}), {2}, {3})))", rowCurrentMember, rowMemberSet, measureName, order);

            }
            else if ((rowCurrentMember == "") && (colCurrentMember != ""))
            {
                return String.Format("IIF(IsEmpty(({0},{2})), null, Rank(({0}), ORDER(NonEmptyCrossjoin({1}), {2}, {3})))", colCurrentMember, colMemberSet, measureName, order);
            }
            else if ((rowCurrentMember == "") && (colCurrentMember == ""))
            {
                return String.Format("IIF(IsEmpty({0}), null, 1)", measureName);
            }

            return String.Format("IIF(IsEmpty({0}), null, 1)", measureName);
        }


        /// <summary>
        /// Включать ли в запрос ось
        /// </summary>
        /// <param name="axType"></param>
        /// <returns></returns>
        private bool IsIncludeAxis(QueryAxisType axType)
        {
            PivotAxis axis = this.GetPivotAxis(axType);
            {
                return (!axis.IsEmpty ||
                    ((axType == QueryAxisType.axColumns) && (this.NeedDummyMeasure() ||
                    (!this.pivot.TotalAxis.IsEmpty && !IsChartElement && !IsMultiGaugeElement))));
            }
        }

        /// <summary>
        /// Получение выражения для всех мер BottomCount
        /// </summary>
        /// <returns></returns>
        private string GetBottomCountMeasuresExpression()
        {
            string result = String.Empty;
            foreach (PivotTotal measure in this.pivot.TotalAxis.Totals)
            {
                result += GetBottomCountMeasure(measure.UniqueName, false);
            }
            return result;
        }

        /// <summary>
        /// Меры для определения к-последних элементов
        /// </summary>
        /// <param name="measureSource"></param>
        /// <returns></returns>
        private string GetBottomCountMeasure(string measureSource, bool isOnlyMemberName)
        {
            if (!this.pivot.IsNeedBottomCountCalculation())
                return String.Empty;

            if (!this.IsTableElement)
                return String.Empty;

            string result = String.Empty;

            string measureName = this.pivot.BottomCountSettings.GetBottomCountMeasureName(measureSource);
            if (isOnlyMemberName)
                return measureName;

            FieldSet fs = this.pivot.RowAxis.FieldSets.GetLastItem();
            string lastLevelUN = fs.GetLastField().UniqueName;

            string expression = String.Empty;

            if (fs.Fields.Count > 1)
            {
                string parentLevelUniqueName = fs.Fields[fs.Fields.Count - 2].UniqueName;
                expression = String.Format(
                        @"'IIF( {0}.CurrentMember.Level is {1} and not IsEmpty(({0}.CurrentMember, {4})), Intersect(BottomCount(Intersect(Descendants(Ancestor({0}.CurrentMember, {2}), 
                        {1}), [ChildRowset]), {3},  {4}), {{{0}.CurrentMember}}).Count, null)'"
                        , fs.UniqueName, lastLevelUN, parentLevelUniqueName, this.pivot.BottomCountSettings.BottomCount,
                        measureSource);
            }
            else
            {
                expression = String.Format(
                        @"'IIF( {0}.CurrentMember.Level is {1} and not IsEmpty(({0}.CurrentMember, {3})), Intersect(BottomCount([ChildRowset], {2}, {3}), 
                                    {{{0}.CurrentMember}}).Count, null)'",
                        fs.UniqueName, lastLevelUN, this.pivot.BottomCountSettings.BottomCount, measureSource);
            }

            result = String.Format(" MEMBER {0} AS {1}, SOLVE_ORDER = 1 ", measureName, expression);

            return result;
        }

        /// <summary>
        /// Получение выражения для всех мер TopCount
        /// </summary>
        /// <returns></returns>
        private string GetTopCountMeasuresExpression()
        {
            string result = String.Empty;
            foreach(PivotTotal measure in this.pivot.TotalAxis.Totals)
            {
                result += GetTopCountMeasure(measure.UniqueName, false);
            }
            return result;
        }

        /// <summary>
        /// Меры для определения к-первых элементов
        /// </summary>
        /// <param name="measureSource"></param>
        /// <returns></returns>
        private string GetTopCountMeasure(string measureSource, bool isOnlyMemberName)
        {
            if (!this.pivot.IsNeedTopCountCalculation())
                return String.Empty;

            if (!this.IsTableElement)
                return String.Empty;

            string result = String.Empty;

            string measureName = this.pivot.TopCountSettings.GetTopCountMeasureName(measureSource);
            if (isOnlyMemberName)
                return measureName;

            FieldSet fs = this.pivot.RowAxis.FieldSets.GetLastItem();
            string lastLevelUN = fs.GetLastField().UniqueName;

            string expression = String.Empty;

            if (fs.Fields.Count > 1)
            {
                string  parentLevelUniqueName = fs.Fields[fs.Fields.Count - 2].UniqueName;
                expression = String.Format(
                        @"'IIF( {0}.CurrentMember.Level is {1} and not IsEmpty(({0}.CurrentMember, {4})), Intersect(TopCount(Intersect(Descendants(Ancestor({0}.CurrentMember, {2}), 
                        {1}), [ChildRowset]), {3},  {4}), {{{0}.CurrentMember}}).Count, null)'"
                        , fs.UniqueName, lastLevelUN, parentLevelUniqueName, this.pivot.TopCountSettings.TopCount,
                        measureSource);
            }
            else
            {
                expression = String.Format(
                        @"'IIF( {0}.CurrentMember.Level is {1} and not IsEmpty(({0}.CurrentMember, {3})), Intersect(TopCount([ChildRowset], {2}, {3}), 
                                    {{{0}.CurrentMember}}).Count, null)'", 
                        fs.UniqueName, lastLevelUN, this.pivot.TopCountSettings.TopCount, measureSource);
            }

            result = String.Format(" MEMBER {0} AS {1}, SOLVE_ORDER = 1 ", measureName, expression);

            return result;
        }


        #region Расчет медианы


        /// <summary>
        /// выражение для элемента "Медиана"
        /// </summary>
        /// <param name="isOnlyMemberNames">если true - возвращает только имена вычислимых мемберов</param>
        /// <returns></returns>
        private string GetMedianMemberExpression(bool isOnlyMemberNames)
        {
            if (!this.pivot.IsNeedMedianCalculation())
                return String.Empty;

            if (!this.IsTableElement)
                return String.Empty;

            //При разном составе элементов на оси строк среднее будем получать по разному
            //1) Если у последнего филдсета оси всего один уровень, то нужен все один мембер для расчета медианы

            FieldSet fs = this.pivot.RowAxis.FieldSets.GetLastItem();
            if (fs.Fields.Count == 1)
            {
                string memberName = String.Format("{0}.[{1}]", fs.GrandMemberUN, this.pivot.MedianSettings.MedianMemberName);
                if (isOnlyMemberNames)
                    return memberName;

                string expression =  String.Format("Median({0})", ChildRowsetName);
                expression = String.Format("\n /* Медиана */ \n  MEMBER {0} AS 'IIF({2}.Count > 0, {1}, null) '", memberName,
                                                   expression, ChildRowsetName);
                //добавим формат значения если нужно
                expression += String.Format(", format_string = '{0}' ", defaultFormatString);
                //Задаем порядок вычисления элемента, чтобы на него не влияли другие элементы
                expression += ", SOLVE_ORDER=1 ";
                return expression;
            }

            //2) Если родительские элементы из той же иерархии создавать придется отдельный мембер для каждого родительского элемента
            if (fs.Fields.Count > 1)
            {
                if (isOnlyMemberNames)
                {
                    string members = String.Empty;
                    foreach (string mbr in this.PenultimateLevelMembers)
                    {
                        members = AddTail(members, ", ");
                        members += String.Format("{0}.[{1}]", mbr, this.pivot.MedianSettings.MedianMemberName);
                    }
                    return members;
                }

                string memberExpressions = String.Empty;
                int membersCount = 1;
                foreach (string mbr in this.PenultimateLevelMembers)
                {
                    string memberName = String.Format("{0}.[{1}]", mbr, this.pivot.MedianSettings.MedianMemberName);
                    string curMemberRowSetName = String.Format("[ChildRowset{0}]", membersCount);
                    string expression = String.Format("Median({0})", curMemberRowSetName);
                    memberExpressions += String.Format("\n /* Медиана */ \n MEMBER {0} AS '{1}'\n", memberName, expression);

                    //добавим формат значения если нужно
                    memberExpressions += String.Format(", format_string = '{0}' ", defaultFormatString);
                    //Задаем порядок вычисления элемента, чтобы на него не влияли другие элементы
                    memberExpressions += ", SOLVE_ORDER=1 ";

                    membersCount++;
                }
                return memberExpressions;
            }

            return String.Empty;
        }

        #endregion

        /// <summary>
        /// Создание вспомогательных сетов для создания вычислимых мемберов на оси строк. Пойдет в секцию With
        /// </summary>
        /// <returns></returns>
        private string GetChildRowsets()
        {
            if (!this.IsTableElement)
                return String.Empty;

            if (!this.pivot.IsNeedAverageCalculation() && !this.pivot.IsNeedMedianCalculation() && !this.pivot.IsNeedTopCountCalculation() && !this.pivot.IsNeedBottomCountCalculation())
                return String.Empty;

            //При разном составе элементов на оси строк среднее будем получать по разному
            //1) Если у последнего филдсета оси всего один уровень, то нужен все один мембер для расчета среднего

            FieldSet fs = this.pivot.RowAxis.FieldSets.GetLastItem();

            if (fs.Fields.Count == 1)
            {
                //Получаем все выбранные элементы иерархии на выбранном единственном уровне
                string childMemberSet = DimensionSet(fs.MemberNames, fs.UniqueName);

                string membersLevelName = fs.GetLastField().UniqueName;

                string childRowset = string.Format(" SET {3} AS 'Filter(Intersect({0}, {{{1}.AllMembers}}), not IsEmpty({2}.CurrentMember))' ", childMemberSet, membersLevelName, fs.UniqueName, ChildRowsetName);

                return childRowset;
            }

            //2) Если родительские элементы из той же иерархии создавать придется отдельный мембер для каждого родительского элемента
            if (fs.Fields.Count > 1)
            {
                //Получаем множество всех элементов последнего филдсета
                string fsMemberSet = DimensionSet(fs.MemberNames, fs.UniqueName);
                string childsLevelName = fs.GetLastField().UniqueName;

                string childRowSet = String.Format(" SET {2} AS 'Intersect({0},{{{1}.AllMembers}})' \n", fsMemberSet,
                                                   childsLevelName, ChildRowsetName);

                string setExpressions = String.Empty;
                if ((this.PenultimateLevelMembers.Count > 0) || this.pivot.IsNeedTopCountCalculation() || this.pivot.IsNeedBottomCountCalculation())
                {
                    setExpressions = childRowSet;
                }

                int setCount = 1;
                foreach (string mbr in this.PenultimateLevelMembers)
                {
                    string expression =
                        String.Format(
                            "Filter(Intersect({{{3}}}, Descendants({0}, {1})), not IsEmpty({2}.CurrentMember))", mbr,
                            childsLevelName, fs.UniqueName, ChildRowsetName);

                    string curMemberRowSetName = String.Format("[ChildRowset{0}]", setCount);
                    string curMemberRowSet = String.Format(" SET {0} AS '{1}' ", curMemberRowSetName, expression);

                    setExpressions += curMemberRowSet;
                    setCount++;
                }
                return setExpressions;
            }
            return String.Empty;
        }

        #region Расчет среднего


        /// <summary>
        /// Добавление функции расчета среднего к набору элементов
        /// </summary>
        /// <param name="memberSet"></param>
        /// <returns></returns>
        private string AddAverageFunction(string memberSet)
        {
            //Среднее геометрическое GEOMEAN()
            //Среднее гармоническое HARMEAN()
            switch(this.pivot.AverageSettings.AverageType)
            {
                case AverageType.Arithmetical:
                    memberSet = "AVG(" + memberSet + ")";
                    break;

                case AverageType.Geometrical:
                    //memberSet = "Excel!GEOMEAN(SetToArray(" + memberSet + "))";
                    //тоже самое но без использования экселя
                    memberSet = String.Format(
                            "power(10, (Sum({0}, LOG10([Measures].CurrentMember))/{0}.Count))", memberSet);
                    
                    break;
                case AverageType.Harmonic:
                    //memberSet = "Excel!HARMEAN(SetToArray(" + memberSet + "))";

                    memberSet = String.Format("{0}.Count / (Sum({0}, 1/([Measures].CurrentMember)))", memberSet);

                    break;
            }
            return memberSet;
        }




        /// <summary>
        /// выражение для элемента "Среднее"
        /// </summary>
        /// <param name="isOnlyMemberNames">если true - возвращает только имена вычислимых мемберов</param>
        /// <returns></returns>
        private string GetAverageMemberExpression(bool isOnlyMemberNames)
        {
            if (!this.pivot.IsNeedAverageCalculation())
                return String.Empty;

            if (!this.IsTableElement)
                return String.Empty;

            //При разном составе элементов на оси строк среднее будем получать по разному
            //1) Если у последнего филдсета оси всего один уровень, то нужен все один мембер для расчета среднего

            FieldSet fs = this.pivot.RowAxis.FieldSets.GetLastItem();
            if (fs.Fields.Count == 1)
            {
                string memberName = String.Format("{0}.[{1}]", fs.GrandMemberUN, this.pivot.AverageSettings.AverageMemberName);
                if (isOnlyMemberNames)
                    return memberName;
                
                string membersLevelName = fs.GetLastField().UniqueName;

                /*
                //Получаем все выбранные элементы иерархии на выбранном единственном уровне
                string childMemberSet = DimensionSet(fs.MemberNames, fs.UniqueName);

                string childRowset = string.Format("SET {3} AS 'Filter(Intersect({0}, {{{1}.AllMembers}}), not IsEmpty({2}.CurrentMember))'", childMemberSet, membersLevelName, fs.UniqueName, ChildRowsetName);
                */

                string expression = AddAverageFunction(ChildRowsetName);

                if ((this.pivot.AverageSettings.AverageType == AverageType.Geometrical) || (this.pivot.AverageSettings.AverageType == AverageType.Harmonic))
                {
                    //Эксель если все значения отрицательные вырубает весь запрос, будем проверять сами
                    expression = String.Format("\n /* Среднее значение */ \n  MEMBER {0} AS 'IIF({2}.Count > 0, IIF(Min({2}, [Measures].CurrentMember) > 0, {1}, 1/0) , null) '",
                                               memberName,
                                               expression, ChildRowsetName);
                }
                else
                {
                    expression = String.Format("\n /* Среднее значение */ \n  MEMBER {0} AS 'IIF({2}.Count > 0, {1}, null) '", memberName,
                                                   expression, ChildRowsetName);
                }

                //добавим формат значения
                expression += String.Format(", format_string = '{0}' ", defaultFormatString);

                //Задаем порядок вычисления элемента, чтобы на него не влияли другие элементы
                expression += ", SOLVE_ORDER=1 ";


                foreach(PivotTotal total in this.pivot.TotalAxis.Totals)
                {
                    expression += GetAverageDeviationMeasureExpression(total.UniqueName, fs.UniqueName, fs.GrandMemberUN + ".Level", membersLevelName);
                }

                return expression;
            }

            //2) Если родительские элементы из той же иерархии создавать придется отдельный мембер для каждого родительского элемента
            if (fs.Fields.Count > 1)
            {
                if (isOnlyMemberNames)
                {
                    string members = String.Empty;
                    foreach (string mbr in this.PenultimateLevelMembers)
                    {
                        members = AddTail(members, ", ");
                        members += String.Format("{0}.[{1}]", mbr, this.pivot.AverageSettings.AverageMemberName);
                    }
                    return members;
                }

                string childsLevelName = fs.GetLastField().UniqueName;

                //Получаем множество всех элементов последнего филдсета
                /*
                string fsMemberSet = DimensionSet(fs.MemberNames, fs.UniqueName);
                

                string childRowSet = String.Format("SET {2} AS 'Intersect({0},{{{1}.AllMembers}})'\n", fsMemberSet, childsLevelName, ChildRowsetName);


                
                if (this.PenultimateLevelMembers.Count > 0)
                {
                    memberExpressions = childRowSet;
                }
                */

                string memberExpressions = String.Empty;

                int membersCount = 1;
                foreach (string mbr in this.PenultimateLevelMembers)
                {
                    string memberName = String.Format("{0}.[{1}]", mbr, this.pivot.AverageSettings.AverageMemberName);
                    /*
                    string expression = String.Format("Filter(Intersect({{{3}}}, Descendants({0}, {1})), not IsEmpty({2}.CurrentMember))", mbr, childsLevelName, fs.UniqueName, ChildRowsetName);

                    string curMemberRowSetName = String.Format("[ChildRowset{0}]", membersCount);
                    string curMemberRowSet = String.Format(" SET {0} AS '{1}' ", curMemberRowSetName, expression);

                    memberExpressions += curMemberRowSet;
                    */
                    string curMemberRowSetName = String.Format("[ChildRowset{0}]", membersCount);
                    string expression = AddAverageFunction(curMemberRowSetName);

                    if ((this.pivot.AverageSettings.AverageType == AverageType.Geometrical) || (this.pivot.AverageSettings.AverageType == AverageType.Harmonic))
                    {
                        memberExpressions += String.Format("\n /* Среднее значение */ \n MEMBER {0} AS 'IIF({2}.Count > 0, IIF(Min({2}, [Measures].CurrentMember) > 0, {1}, 1/0), null)' \n", memberName, expression, curMemberRowSetName);
                    }
                    else
                    {
                        memberExpressions += String.Format("\n /* Среднее значение */ \n MEMBER {0} AS '{1}' \n", memberName, expression);
                    }



                    //добавим формат значения
                    memberExpressions += String.Format(", format_string = '{0}' ", defaultFormatString);

                    //Задаем порядок вычисления элемента, чтобы на него не влияли другие элементы
                    memberExpressions += ", SOLVE_ORDER=1 ";

                    membersCount++;
                }

                foreach (PivotTotal total in this.pivot.TotalAxis.Totals)
                {
                    memberExpressions += GetAverageDeviationMeasureExpression(total.UniqueName, fs.UniqueName, fs.Fields[fs.Fields.Count - 2].UniqueName, childsLevelName);
                }


                return memberExpressions;
            }

            return String.Empty;
        }



        /// <summary>
        /// выражение для элемента "Стандартное отклонение"
        /// </summary>
        /// <param name="isOnlyMemberNames">если true - возвращает только имена вычислимых мемберов</param>
        /// <returns></returns>
        private string GetStandartDeviationMemberExpression(bool isOnlyMemberNames)
        {
            if (!this.pivot.IsNeedAverageCalculation())
                return String.Empty;

            if ((!this.pivot.AverageSettings.IsStandartDeviationCalculate) || (!this.IsTableElement))
                return String.Empty;

            //При разном составе элементов на оси строк среднее будем получать по разному
            //1) Если у последнего филдсета оси всего один уровень, то нужен все один мембер для расчета среднего

            FieldSet fs = this.pivot.RowAxis.FieldSets.GetLastItem();
            if (fs.Fields.Count == 1)
            {
                string memberName = String.Format("{0}.[{1}]", fs.GrandMemberUN,
                                                  this.pivot.AverageSettings.StandartDeviationName);
                if (isOnlyMemberNames)
                    return memberName;

                //множество всех листовых элементов уже было создано при расчете среднего значения, поэтому не будем его создавать еще раз
                /*
                //Получаем все выбранные элементы иерархии на выбранном единственном уровне
                string childMemberSet = DimensionSet(fs.MemberNames, fs.UniqueName);
                string membersLevelName = fs.GetLastField().UniqueName;
                */

                string expression = String.Empty;
                expression = "\n /* Стандартное отклонение */ \n " + expression;
                expression =
                    String.Format(
                        @"MEMBER {0} AS 'IIF(Not IsEmpty(StrToMember(MemberToStr(Ancestor({2}.CurrentMember, {3})) + "".[{4}]""))  ,IIF({1}.Count > 0, Stdev({1}), null), null) '",
                        memberName, ChildRowsetName, fs.UniqueName, fs.GrandMemberUN + ".Level",
                        this.pivot.AverageSettings.AverageMemberName);


                //добавим формат значения 
                expression += String.Format(", format_string = '{0}' ", defaultFormatString);
                
                //Задаем порядок вычисления элемента, чтобы на него не влияли другие элементы
                expression += ", SOLVE_ORDER=1 ";
                return expression;
            }

            //2) Если родительские элементы из той же иерархии создавать придется отдельный мембер для каждого родительского элемента
            if (fs.Fields.Count > 1)
            {
                if (isOnlyMemberNames)
                {
                    string members = String.Empty;
                    foreach (string mbr in this.PenultimateLevelMembers)
                    {
                        members = AddTail(members, ", ");
                        members += String.Format("{0}.[{1}]", mbr, this.pivot.AverageSettings.StandartDeviationName);
                    }
                    return members;
                }

                //Получаем множество всех элементов последнего филдсета
                string childsLevelName = fs.GetLastField().UniqueName;

                string memberExpressions = String.Empty;

                foreach (string mbr in this.PenultimateLevelMembers)
                {
                    string memberName = String.Format("{0}.[{1}]", mbr, this.pivot.AverageSettings.StandartDeviationName);

                    string expression =
                        String.Format(
                            @"IIF(Not IsEmpty(StrToMember(MemberToStr(Ancestor({4}.CurrentMember, {5})) + "".[{6}]""))  ,  Stdev(Filter(Intersect({{{3}}}, Descendants({0}, {1})), not IsEmpty({2}.CurrentMember))), null)",
                            mbr, childsLevelName, fs.UniqueName, ChildRowsetName, fs.UniqueName,
                            fs.Fields[fs.Fields.Count - 2].UniqueName, this.pivot.AverageSettings.AverageMemberName);

                    memberExpressions += String.Format("\n /* Стандартное отклонение */ \n MEMBER {0} AS '{1}'\n", memberName, expression);
                    //добавим формат значения 
                    memberExpressions += String.Format(", format_string = '{0}' ", defaultFormatString);

                    //Задаем порядок вычисления элемента, чтобы на него не влияли другие элементы
                    memberExpressions += ", SOLVE_ORDER=1 ";

                }

                return memberExpressions;
            }

            return String.Empty;
        }



        /// <summary>
        /// Получение выражения для отклонения от среднего
        /// </summary>
        /// <returns></returns>
        private string GetAverageDeviationMeasureExpression(string measureName, string fsName, string parentlevelName, string childrenLevelName)
        {
            if (!this.pivot.AverageSettings.IsAverageDeviationCalculate)
                return String.Empty;

            string memberName = PivotData.GetAverageDeviationMeasureName(measureName);
            //Условие, что элемент не должен являтся вычислимым (для них отклонение от среднего не считаем)
            string notCalculatedMemberCondition = String.Empty;
            if (this.pivot.AverageSettings.IsAverageDeviationCalculate)
                notCalculatedMemberCondition = String.Format(@"and not({0}.CurrentMember is StrToMember(MemberToStr(Ancestor({0}.CurrentMember, {1})) + "".[{2}]"")) ", 
                    fsName, parentlevelName, this.pivot.AverageSettings.AverageMemberName);
            if (this.pivot.AverageSettings.IsStandartDeviationCalculate)
                notCalculatedMemberCondition += String.Format(@"and not({0}.CurrentMember is StrToMember(MemberToStr(Ancestor({0}.CurrentMember, {1})) + "".[{2}]"")) ", 
                    fsName, parentlevelName, this.pivot.AverageSettings.StandartDeviationName);
            if (this.pivot.MedianSettings.IsMedianCalculate)
                notCalculatedMemberCondition += String.Format(@"and not({0}.CurrentMember is StrToMember(MemberToStr(Ancestor({0}.CurrentMember, {1})) + "".[{2}]"")) ",
                    fsName, parentlevelName, this.pivot.MedianSettings.MedianMemberName);



            string result =
                string.Format(@"MEMBER {0} AS 
                            'IIF(not IsEmpty({1}) and ({3}.CurrentMember.Level is {5}) {7}, 
                                                    ({1} - ({2}, StrToMember(MemberToStr(Ancestor({3}.CurrentMember, {4})) + "".[{6}]""))), 
                                                    null)' ",
                    memberName, measureName, measureName, fsName, parentlevelName, childrenLevelName,
                    this.pivot.AverageSettings.AverageMemberName, notCalculatedMemberCondition);

            result += String.Format(", format_string = '{0}' ", defaultFormatString);
            result += ", SOLVE_ORDER=2";
            return result;

        }



        #endregion





        #endregion

        #region AXES - Сборка осей запроса

        /// <summary>
        /// Множество мер запроса
        /// </summary>
        private string MeasuresSet()
        {
            string measureEnum = string.Empty;
            
            if (NeedDummyMeasure())
            {
                measureEnum = string.Format("[Measures].[{0}]", dummyMeasureName);
            }
            else
            {
                string measureName = String.Empty;    
                foreach (PivotTotal measure in pivot.TotalAxis.VisibleTotals)
                {
                    measureName = measure.UniqueName;
                    measureEnum = AddTail(measureEnum, commaDelim) + measureName;

                    if (this.pivot.IsNeedAverageCalculation() && this.pivot.AverageSettings.IsAverageDeviationCalculate && this.IsTableElement)
                    {
                        measureName = PivotData.GetAverageDeviationMeasureName(measure.UniqueName);
                        measureEnum = AddTail(measureEnum, commaDelim) + measureName;
                    }

                    if (this.pivot.IsNeedTopCountCalculation() && this.IsTableElement)
                    {
                        measureName = this.pivot.TopCountSettings.GetTopCountMeasureName(measure.UniqueName);
                        measureEnum = AddTail(measureEnum, commaDelim) + measureName;
                    }

                    if (this.pivot.IsNeedBottomCountCalculation() && this.IsTableElement)
                    {
                        measureName = this.pivot.BottomCountSettings.GetBottomCountMeasureName(measure.UniqueName);
                        measureEnum = AddTail(measureEnum, commaDelim) + measureName;
                    }

                }


            }

            return SetBrackets(measureEnum);
        }

        
        /// <summary>
        /// Включить (или исключать) перечисленные потомки мембера
        /// </summary>
        private bool IsChildrenIncluding(XmlNode memNode)
        {
            if (memNode != null)
            {
                return (XmlHelper.GetStringAttrValue(memNode, "childrentype", "") == "included");
            }
            else
                return true;
        }
        
        /*
        /// <summary>
        /// MDX-множество элементов одного измерения оси
        /// </summary>
        /// <param name="root">корень элементов - dummy</param>        
        /// 
        private string DimensionSet(XmlNode root, string dimUName)
        {
            if (root == null)
                return string.Empty;

            string childrenEnum = string.Empty;
            string childSet = string.Empty;
            string un = XmlHelper.GetStringAttrValue(root, "uname", "");            

            //Формируем множество потомков корня
            foreach (XmlNode node in root.ChildNodes)
            {
                childSet = DimensionSet(node, dimUName);
                if ((childSet != string.Empty) && (childrenEnum != string.Empty))
                {
                    childrenEnum = childrenEnum + commaDelim;
                }
                childrenEnum = childrenEnum + childSet;
            }
                        

            if (un == string.Empty) //это может быть тоолько корень всей структуры
            {
                if (IsChildrenIncluding(root))
                {
                    return SetBrackets(childrenEnum);
                }
                else
                {
                    return String.Format("Except({0}.AllMembers, {{{1}}})", dimUName, SetBrackets(childrenEnum));
                }
            }

            //потомков нет - "черный флажок" - все включено
            if (childrenEnum == string.Empty)
            {
                return GetDescendantsSet(un, true);
            }

            //потомки есть ("серый флажок")            
            childrenEnum = "{" + childrenEnum + "}";                       
            if (IsChildrenIncluding(root))
            {
                //в зависимости от харрактера своего предка серый флажок обрабатывается по разному...
                if (IsChildrenIncluding(root.ParentNode))
                {
                    //и корень и его предок включают потомков
                    //итоговый сет: корень + сеты потомков
                    return SetBrackets(un + commaDelim + childrenEnum);
                }
                else
                {
                    //корень включает потомков, а вот предок исключает. В этом случае..
                    //множество элемента - это все неперечисленные подчиненные, сам корень не включается.
                    return String.Format("Except({0}, {{{1}}})", GetDescendantsSet(un, false), SetBrackets(childrenEnum));
                }            
            }
            else
            {
                if (IsChildrenIncluding(root.ParentNode))
                {
                    //итоговый сет: все подчиненные корня минус сеты потомков
                    return String.Format("Except({0}, {{{1}}})", GetDescendantsSet(un, true), SetBrackets(childrenEnum));
                }
                else
                {
                    //return SetBrackets(un + commaDelim + childrenEnum);
                    return SetBrackets(childrenEnum);
                }
            }           
        }
        */
        
        private string DimensionSet(XmlNode root, string dimUName)
        {
            if (root == null)
                return string.Empty;

            StringBuilder childrenEnum = new StringBuilder();
            string childSet = string.Empty;
            string un = XmlHelper.GetStringAttrValue(root, "uname", "");

            //Формируем множество потомков корня
            foreach (XmlNode node in root.ChildNodes)
            {
                childSet = DimensionSet(node, dimUName);
                if ((childSet != string.Empty) && (childrenEnum.ToString() != string.Empty))
                {
                    childrenEnum.Append(commaDelim);
                }
                childrenEnum.Append(childSet);
            }


            if (un == string.Empty) //это может быть тоолько корень всей структуры
            {
                if (IsChildrenIncluding(root))
                {
                    return SetBrackets(childrenEnum.ToString());
                }
                else
                {
                    return String.Format("Except({0}.AllMembers, {{{1}}})", dimUName, SetBrackets(childrenEnum.ToString()));
                }
            }

            //потомков нет - "черный флажок" - все включено
            if (childrenEnum.ToString() == string.Empty)
            {
                return GetDescendantsSet(un, true);
            }

            //потомки есть ("серый флажок")            
            childrenEnum.Insert(0, "{");
            childrenEnum.Append("}");
            if (IsChildrenIncluding(root))
            {
                //в зависимости от харрактера своего предка серый флажок обрабатывается по разному...
                if (IsChildrenIncluding(root.ParentNode))
                {
                    //и корень и его предок включают потомков
                    //итоговый сет: корень + сеты потомков
                    return SetBrackets(String.Format("{0}{1}{2}", un, commaDelim, childrenEnum));
                }
                else
                {
                    //корень включает потомков, а вот предок исключает. В этом случае..
                    //множество элемента - это все неперечисленные подчиненные, сам корень не включается.
                    return String.Format("Except({0}, {{{1}}})", GetDescendantsSet(un, false), SetBrackets(childrenEnum.ToString()));
                }
            }
            else
            {
                if (IsChildrenIncluding(root.ParentNode))
                {
                    //итоговый сет: все подчиненные корня минус сеты потомков
                    return String.Format("Except({0}, {{{1}}})", GetDescendantsSet(un, true), SetBrackets(childrenEnum.ToString()));
                }
                else
                {
                    //return SetBrackets(un + commaDelim + childrenEnum);
                    return SetBrackets(childrenEnum.ToString());
                }
            }
        }        
        

        /// <summary>
        /// MDX-множество состоящее из одного кортежа, представляющего общий итог
        /// </summary>
        /// <param name="ax">Ось</param>
        /// <returns></returns>
        private string GrandTotalSet(PivotAxis ax)
        {
            if (!ax.GrandTotalExists)
                return string.Empty;
                
            string grandMembersEnum = string.Empty;
            foreach (FieldSet fs in ax.FieldSets)
            {
                if (fs.UniqueName == "[Measures]")
                    continue;
                grandMembersEnum = AddTail(grandMembersEnum, commaDelim);
                grandMembersEnum += fs.GrandMemberUN;
            }            
            
            return SetBrackets(TupleBrackets(grandMembersEnum));            
        }
        
        /// <summary>
        /// Выражение для MP для элементов оси
        /// </summary>
        private string MemberPropertiesClause(PivotAxis ax)
        {
            string  mpEnum = string.Empty;
            foreach (FieldSet fs in ax.FieldSets)
                foreach (PivotField pf in fs.Fields)
                    foreach (string mpName in pf.MemberProperties.VisibleProperties)
                    {
                        mpEnum = AddTail(mpEnum, commaDelim);
                        mpEnum += string.Format("{0}.[{1}]", pf.UniqueName, mpName);                                 
                    }                      
            
            if (mpEnum != string.Empty)
            {
                return string.Format(" DIMENSION PROPERTIES {0} ", mpEnum);
            }
            else
            {
                return string.Empty;
            }
        }
                
        /// <summary>
        /// Нужно ли строки выбирать как столбцы?
        /// </summary>
        private bool RowsGoesAsColumns()
        {
            ///1) Это может понадобиться только тогда, когда в запросе должна быть ось строк,
            /// а ось столбцов отсутствует. Ситуация для MDX не допустимая.
            /// Поэтому, что бы хоть что-то выводить эти самые строки будем запихивать в 
            /// отсутствующие столбцы.
            /// 2) Делать это будем только для диаграммы, поскольку для таблицы в этой ситуации
            /// создается фиктивная мера-пустышка. Поэтому в таблице ось столбцов есть всегда
            return ((((this.elementType == ReportElementType.eChart)) || (this.elementType == ReportElementType.eMultiGauge)) && (pivot.ColumnAxis.FieldSets.Count == 0));
        }


        private string MemberSet(FieldSet fs, HideEmptyMode hideEmptyMode, QueryAxisType axType)
        {
            PivotAxis ax = this.GetPivotAxis(axType);

            FieldSetCollection fieldSets = (IsMapElement && (axType == QueryAxisType.axColumns)) ?
                pivot.TotalAxis.FieldSets : ax.FieldSets;

            //собираем множество по выбранным мемберам           
            string dimSet = "{" + DimensionSet(fs.MemberNames, fs.UniqueName) + "}";

            if (fs.UniqueName != "[Measures]")
            {
                //фильтруем по выбранным уровням
                dimSet = string.Format("Intersect({0}, {1})", dimSet, DimensionCheckedLevelsSet(fs));
                //скрываем датамемберы, если нужно
                dimSet = MakeDataMemberFilter(axType, dimSet);

                //если итоги подсчитываем только по видимым элементам, добавим этот признак, при запросе на 
                //получение строк итоги по видимым нам не важны
                /*задача #19053: не всегда корректно считались итоги по видимым. Сейчас создаем отделые сеты для этих целей
                if (((this.currentBuildMode != WithBuildMode.ForRecordCount) && this.pivot.IsVisualTotals) &&
                    (!IsMapElement))
                    dimSet = string.Format("VisualTotals ({0})", dimSet);
                 */
            }

            //Добавляем вычислимые элементы для оси строк, если нужно
            if ((axType == QueryAxisType.axRows)&&(ax.FieldSets.Count > 0))
            {
                if (ax.FieldSets.GetLastItem() == fs)
                {
                    string calcMembers = GetAverageMemberExpression(true);
                    string stDevMembers = GetStandartDeviationMemberExpression(true);
                    string medianMembers = GetMedianMemberExpression(true);

                    if (!String.IsNullOrEmpty(calcMembers) && !String.IsNullOrEmpty(stDevMembers))
                    {
                        calcMembers += ", " + stDevMembers;
                    }
                    else
                    {
                        calcMembers += stDevMembers;
                    }

                    if (!String.IsNullOrEmpty(calcMembers))
                    {
                        calcMembers += AddHead(", ", medianMembers);
                    }
                    else
                    {
                        calcMembers = medianMembers;
                    }



                    if (!String.IsNullOrEmpty(calcMembers))
                    {
                        dimSet = "Hierarchize({" + dimSet + ", " + calcMembers + "})";
                    }
                }
            }
            /*10395 Если элемент таблица и у измерения нет уровня олл, что бы не нарушить структуру 
             требуется сортировка по убыванию, что бы элементы со значениями переместились на верх, потом 
             вернем сортировку в первоночальное состятние*/
            if (this.IsTableElement && (hideEmptyMode == HideEmptyMode.NonEmpty) && (fieldSets[0] != fs) && !fs.ExistLevelAll)
            {
                if (axType == QueryAxisType.axRows)
                    this.isOrderedRowAxis = true;
                else
                    this.isOrderedColumnAxis = true;
            }
            else
                //если у измериня включена сортировка, добавим ее. При конструировании запроса для
                //получения количества строк, сортировка ни к чему...
                if (ax.IsSorted && (this.currentBuildMode != WithBuildMode.ForRecordCount))
                {
                    dimSet = this.AddFieldSetOrder(dimSet, fs);
                }
            return dimSet;
        }

        /// <summary>
        /// Получаем запрос для оси
        /// </summary>
        /// <param name="axType"></param>
        /// <param name="hideEmptyMode"></param>
        /// <returns></returns>
        private string NonFilterAxisSet(QueryAxisType axType, HideEmptyMode hideEmptyMode)
        {
            string result = string.Empty;
            PivotAxis ax = this.GetPivotAxis(axType);

            FieldSetCollection fieldSets;

            fieldSets = (IsMapElement && (axType == QueryAxisType.axColumns)) ? 
                pivot.TotalAxis.FieldSets : ax.FieldSets;
            

             //собираем перечисление множеств по всем измерениям
            foreach (FieldSet fs in fieldSets)
            {
                //собираем множество по выбранным мемберам           
                string dimSet = MemberSet(fs, hideEmptyMode, axType);
                //склеиваем
                if ((fs.UniqueName != "[Measures]")||(this.IsMapElement))
                {
                    result = ConcatenateSet(result, dimSet, axType);
                }
                else
                {
                    if (this.IsChartElement || this.IsMultiGaugeElement)
                    {
                        FieldSet measureFS = this.pivot.GetFieldSet("[Measures]");
                        if (measureFS != null)
                        {
                            if ((measureFS.AxisType == ax.AxisType)&&(ax.FieldSets.Count == 1))
                            {
                                result = ConcatenateSet(result, dimSet, axType);
                            }
                        }
                    }

                }
            }

            //К колонкам подцепляем еще показатели
            bool isMeasureHook = (this.IsTableElement && (axType == QueryAxisType.axColumns) && (ax.IsEmpty));

            if (isMeasureHook)
            {
                result = ConcatenateSet(result, MeasuresSet(), axType);
            }

            //Если поставлено скрытие пустых, то здесь устанавливаются признаки скрытия пустых 
            //свойственные множеству, а не оси (тоесть Non Empty, если требуется этот атрибут, будет
            //выставлен на уровне выше, в AxisClause)
            if (!string.IsNullOrEmpty(result))
            {
                //Если это ось столбцов, а фактически коллекция столбцов пуста. Т.е есть только показатели,
                //Тогда вообще исключать пустые не будем никогда
                if (!((axType == QueryAxisType.axColumns) && (pivot.ColumnAxis.FieldSets.Count == 0)))
                {
                    if (this.HideEmptyPositions(axType))
                    {

                        switch (hideEmptyMode)
                        {
                            case HideEmptyMode.NonEmptyCrossJoin:
                                result = string.Format("NONEMPTYCROSSJOIN({0})", result);
                                break;
                            case HideEmptyMode.UsingFilter:
                                result = string.Format("Filter({0}, {1})", "{" + result + "}",
                                    GetNonEmptyCondition(ax));
                                break;
                            case HideEmptyMode.NonEmpty2005:
                                if (isMeasureHook || fieldSets.IsContainMeasures)
                                        //если показатели уже подцеплены, то в качестве фильтров 
                                    //в NONEMPTY их передавать не будем
                                    result = string.Format("NONEMPTY({0})", result);
                                else
                                    result = string.Format("NONEMPTY({0}, {1})", result, this.GetSetTotalsID());
                                break;
                        }
                    }
                }
                /*
                if (((axType == QueryAxisType.axRows) && this.isOrderedRowAxis) ||
                    (axType == QueryAxisType.axColumns) && this.isOrderedColumnAxis)
                    //10395 Если в нем содержаться измерения без уровня All, и режим скрытия пустых NonEmpty
                    //надо упорядочить данные по убыванию, чтобы не пустые элементы оказались свверху, 
                    //это пригодиться при отсеивании лишних картежей
                    result = string.Format("Order({0}, {1}, DESC)", result, this.pivot.GetCurrentTotalID());
                else
                    //В методе накладывается сортировка на всю ось, если это требуется.
                    //При сборхе оси для получения количества строк, сортировку включать не будем
                    //т.к. это лишняя нагрузка на проц.
                    if (this.currentBuildMode != WithBuildMode.ForRecordCount)
                        result = this.AddAxisOrder(ax, result);
                 */
            }
            else
            {
                result = "{}";
            }

            return this.EncodeSet(result);
        }

        /// <summary>
        /// Отсортированное множество для оси
        /// </summary>
        /// <param name="axType"></param>
        /// <param name="currentSet"></param>
        /// <returns></returns>
        private string SortedAxisSet(QueryAxisType axType, string currentSet)
        {
            PivotAxis ax = this.GetPivotAxis(axType);
            string result = currentSet;
            if (!string.IsNullOrEmpty(result))

            if (((axType == QueryAxisType.axRows) && this.isOrderedRowAxis) ||
                (axType == QueryAxisType.axColumns) && this.isOrderedColumnAxis)
                //10395 Если в нем содержаться измерения без уровня All, и режим скрытия пустых NonEmpty
                //надо упорядочить данные по убыванию, чтобы не пустые элементы оказались свверху, 
                //это пригодиться при отсеивании лишних картежей
                result = string.Format("Order({0}, {1}, DESC)", result, this.pivot.GetCurrentTotalID());
            else
                //В методе накладывается сортировка на всю ось, если это требуется.
                //При сборхе оси для получения количества строк, сортировку включать не будем
                //т.к. это лишняя нагрузка на проц.
                if (this.currentBuildMode != WithBuildMode.ForRecordCount)
                    result = this.AddAxisOrder(ax, result);
            
            return result;

        }

        /// <summary>
        /// Вернет множество ID показателей
        /// </summary>
        /// <returns></returns>
        private string GetSetTotalsID()
        {
            List<string> totals = pivot.GetTotalsID();
            string result = string.Empty;
            foreach (string total in totals)
            {
                if (result == string.Empty)
                    result = total;
                else
                    result += ", " + total;
            }
            return string.Format("{{{0}}}", result);
        }

        #region сортировка
        /// <summary>
        /// Если надо, накладываем сортировку на всю ось
        /// </summary>
        /// <param name="axType"></param>
        /// <param name="axisSet"></param>
        private string AddAxisOrder(PivotAxis axis, string axisSet)
        {
            if (axis.SortType != SortType.None)
            {
                //сортировку оси у таблиц, будем выполнять на измерениях (AddFieldSetOrder)
                if (!IsTableElement)
                {
                    string sortTypeStr = axis.SortType.ToString();
                    string condion = this.pivot.GetCurrentTotalID();
                    axisSet = String.Format(@"Order ({0}, {1}, {2})", axisSet, condion, sortTypeStr);
                }
            }
            else
            {
                //Если ось строк и собираем множестово не для получения количества строк, добавим сортировку 
                //показателей, если оно того требует.
                if (axis.AxisType == AxisType.atRows)
                    axisSet = this.AddTotalOrder(axisSet);
            }
            return axisSet;
        }

        /// <summary>
        /// Добавить сортировку показателя
        /// </summary>
        /// <param name="axisSet"></param>
        /// <returns></returns>
        private string AddTotalOrder(string axisSet)
        {
            foreach (PivotTotal total in pivot.TotalAxis.VisibleTotals)
            {
                if (total.SortType != SortType.None)
                {
                    string sortTypeStr = total.SortType.ToString();
                    string condion = total.UniqueName;
                    
                    //если сортировка меры происходит по конкретному кортежу, добавим его в условие сортировки
                    if (total.SortedTupleUN != string.Empty)
                    {
                        string sortertedTupleUN = total.SortedTupleUN.Replace("][", "], [");
                        condion = string.Format("{0}, {1}", sortertedTupleUN, condion);
                    }
                    return String.Format(@"Order ({0}, ({1}), {2})", axisSet, condion, sortTypeStr);
                }
            }
            return axisSet;
        }

        /// <summary>
        /// Добавить измерению сортировку. Если элемент таблица, то сортировку оси, добавляем
        /// сортировкой каждого измериня в этой оси (т.к. по другому по алфавиту 
        /// не отсортировать). Если признак сортировки стоит у уровня, выставляем ее здесь.
        /// </summary>
        /// <param name="dimSet"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        private string AddFieldSetOrder(string dimSet, Data.FieldSet fs)
        {
            string clause = string.Empty;
            string sortTypeStr = string.Empty;
            //ось сортируемого измерения
            PivotAxis axis = (fs.AxisType == AxisType.atRows) ? fs.ParentPivotData.RowAxis : 
                fs.ParentPivotData.ColumnAxis;
            //использовать сортировку оси
            bool isUseAxisSort = (this.IsTableElement && (axis.SortType != SortType.None));

            if ((fs.SortType != SortType.None) || isUseAxisSort)
            {
                sortTypeStr = isUseAxisSort ? axis.SortType.ToString() : fs.SortType.ToString();
                //в диаграмме будем сортировать по значению
                if (this.IsChartElement || this.IsMultiGaugeElement)
                {
                    clause = string.Format("{0}, {1}", pivot.GetCurrentTotalID(), sortTypeStr);
                }
                //в таблице по алфавиту
                else
                    clause = String.Format("{0}.CurrentMember.Name, {1}", fs.UniqueName, sortTypeStr);
            }
            else
            {
                foreach (Data.PivotField field in fs.Fields)
                {
                    sortTypeStr = field.SortType.ToString(); 
                    if (field.SortType != SortType.None)
                    {
                        clause = String.Format("IIF({0}.CurrentMember.Level IS {1}, {0}.CurrentMember.Name, \"\"), {2}", fs.UniqueName, field.UniqueName, sortTypeStr);
                    }
                }
            }
            if (clause != string.Empty)
                return string.Format(@"Order ({0}, {1})", dimSet, clause);
            else
                return dimSet;
        }

        #endregion

        /// <summary>
        /// Получить имя множества в котором содержатся не фильтрованные данные оси
        /// </summary>
        /// <param name="axType"></param>
        /// <returns></returns>
        private string GetNonFilterSetName(QueryAxisType axType)
        {
            //если используется сессионное множество, значит ось строк уже создана, возврящаем 
            //ее заголовок
            if ((axType == QueryAxisType.axRows) && this.useRowsSessionSet)
            {
                return NonFilterHeadRowSetName;
            }
            else
            {
                switch (axType)
                {
                    case QueryAxisType.axRows:
                        return NonFilterRowSetName;
                    case QueryAxisType.axColumns:
                        return NonFilterColumnSetName;
                    case QueryAxisType.axSeries:
                        return NonFilterSerieSetName;
                }
                return "";
            }

        }

        /// <summary>
        /// Полное множество оси
        /// </summary>
        /// <param name="axType">Тип оси</param>
        private string FullAxisSet(QueryAxisType axType)
        {
            string result = string.Empty;
            PivotAxis ax = this.GetPivotAxis(axType);

            if (this.IsIncludeAxis(axType))
            {
                //Получи имя множества оси
                result = this.GetNonFilterSetName(axType);

                result = MakeMissplacedTuplesFilter(axType, result, true);
                //Если это таблица и включен пэйджинг, тогда ставим ограничение на кол-во записей
                if ((axType == QueryAxisType.axRows) && usePaging)
                {
                    result = string.Format("subset({0}, {1}, {2})", result, lowBorder, pageSize);
                }

                bool isMeasureHook = (this.IsTableElement && (axType == QueryAxisType.axColumns) && !ax.IsEmpty);
                if (isMeasureHook)
                {
                    string measureSet = MeasuresSet();
                    if (measureSet != string.Empty)
                    {
                        result = AddTail(result, asterixDelim) + measureSet;
                    }

                    //result = ConcatenateSet(result, measureSet, axType);
                }

                result = AppendChartMeasureSet(result, axType);
            }

            result = AppendGrandTotalToSet(axType, result);
            return result;
        }

        /// <summary>
        /// Добавление показателей  в диаграмме
        /// </summary>
        /// <param name="mainSet"></param>
        /// <param name="axType"></param>
        /// <returns></returns>
        private string AppendChartMeasureSet(string mainSet, QueryAxisType axType)
        {
            if (this.IsChartElement || this.IsMultiGaugeElement)
            {
                FieldSet measureFS = this.pivot.GetFieldSet("[Measures]");
                if (measureFS != null)
                {
                    PivotAxis ax = this.GetPivotAxis(axType);
                    if ((measureFS.AxisType == ax.AxisType) && (ax.FieldSets.Count > 1))
                    {
                        mainSet = ConcatenateSet(mainSet, MemberSet(measureFS, ax.HideEmptyMode, axType), axType);
                    }
                }
            }
            return mainSet;
        }

        private string AppendGrandTotalToSet(QueryAxisType axType, string mainSet)
        {
            PivotAxis ax = this.GetPivotAxis(axType);
            if (ax.GrandTotalExists && (ax.FieldSets.Count > 0))
            {
                //если на карте 3 оси, то в столбцах находятся показатели и добавлять итоги сюда не надо
                if ((ax.AxisType == AxisType.atColumns) && (elementType == ReportElementType.eMap))
                    return mainSet;

                string finalGrandTotalSet;
                if ((ax.AxisType == AxisType.atColumns) && (elementType == ReportElementType.eTable))
                {
                    finalGrandTotalSet = "{" + GrandTotalSet(ax) + " * " + MeasuresSet() + "}";
                }
                else
                {
                    finalGrandTotalSet = GrandTotalSet(ax);
                }

                finalGrandTotalSet = this.EncodeSet(finalGrandTotalSet);

                if (finalGrandTotalSet != String.Empty)
                {
                    finalGrandTotalSet = AppendChartMeasureSet(finalGrandTotalSet, axType);
                    return "{" + string.Format("{0}, {1}", mainSet, finalGrandTotalSet) + "}";
                }
                else
                    return mainSet;
            }
            else
            {
                return mainSet;
            }
        }
                
        /// <summary>
        /// Сборка оси
        /// </summary>
        /// <param name="axType">Тип оси (колонки, строки)</param> 
        /// <param name="isBuildSet">Собрать ось для запроса множества (тоесть атрибуты присущие 
        /// оси, сюда не войдут)</param>
        private string AxisClause(QueryAxisType axType, bool isBuildSet)
        {
            string result = string.Empty;
            PivotAxis ax = GetPivotAxis(axType);

            if ((axType == QueryAxisType.axRows) && useRowsSessionSet)
            {
                //множество строк должно быть уже подготовлено и храниться в сессии подключения
                result = RowsSessionSetName;
                
                //Прицепляем инициализирующее множество, если нужно
                if (headRowSet != string.Empty)
                {
                    string filteredHeadRowSet = this.MakeMissplacedTuplesFilter(QueryAxisType.axRows, NonFilterHeadRowSetName, true);
                    result = "{" + filteredHeadRowSet + " , " + result + "}";
                }
            }
            else
            {
                //собираем множество (либо это не строки, либо сессионное множество не создавали)
                result = FullAxisSet(axType);                            
            }

            if ((result != string.Empty) && !isBuildSet)
            {
                bool isColumnAxisEmpty = ((axType == QueryAxisType.axColumns) && (ax.FieldSets.Count == 0));

                HideEmptyMode hideEmptyMode = this.GetHideEmptyMode(ax, false);
                //Если требуется скрытие пустых, поставим это атрибут
                if ((hideEmptyMode == HideEmptyMode.NonEmpty) && !isColumnAxisEmpty &&
                    HideEmptyPositions(axType))
                {
                    result = "NON EMPTY{" + result + "}";
                }
                else
                {
                    result = "{" + result + "}";
                }

                //добавим в запрос выбранные свойства элементов
                result = AddTail(result, MemberPropertiesClause(ax));
            }

            return result;
        }

        /// <summary>
        /// MDX название оси
        /// </summary>
        private string AxisName(QueryAxisType axType)
        {
            if (RowsGoesAsColumns())
            {
                return "columns";
            }

            switch (axType)
            {
                case QueryAxisType.axColumns:
                    return "columns";
                case QueryAxisType.axRows:
                    return "rows";
                case QueryAxisType.axSeries:
                    return "pages";
            }

            return "";
        }
        
        /// <summary>
        /// Секция осей запроса
        /// </summary>        
        private string AxesClause()
        {            
            string columns = AddTail(AxisClause(QueryAxisType.axColumns, false), " on " + AxisName(QueryAxisType.axColumns));
            string rows = AddTail(AxisClause(QueryAxisType.axRows, false), " on " + AxisName(QueryAxisType.axRows));
            string axDelim = (string.IsNullOrEmpty(rows) || string.IsNullOrEmpty(columns)) ? "" : commaDelim;
            string result = columns + axDelim + rows;
            if (!String.IsNullOrEmpty(result))
                result = " SELECT " + result;

            if (IsMapElement)
            {
                //Для карты необходимо, чтобы были выбраны хотя бы объекты и значения
                if (!(string.IsNullOrEmpty(rows) || string.IsNullOrEmpty(columns)))
                {
                    string series = AddTail(AxisClause(QueryAxisType.axSeries, false), " on " + AxisName(QueryAxisType.axSeries));
                    axDelim = string.IsNullOrEmpty(series) ? "" : commaDelim;
                    result += axDelim + series;
                }
                else
                {
                    result = " SELECT ";
                }
            }
            return result;                
        }
        #endregion

        #region FROM - Выражение куба
        /// <summary>
        /// Секция куба
        /// </summary>
        private string FromClause()
        {
            return " FROM " + MemberBrackets(pivot.CubeName);
        }
        #endregion

        #region WHERE - Сборка фильтров
                       
        /// <summary>
        /// Кортеж представляющий фильтры запроса
        /// </summary>        
        private string FiltersExpression()
        {
            string filtersEnum = string.Empty;

            foreach (FieldSet fs in pivot.FilterAxis.FieldSets)
	        {
                filtersEnum = AddTail(filtersEnum, commaDelim);
                if (fs.IsMultipleChoise()) //множественный фильтр
		        {
                    filtersEnum += GetMultipleFilterSetName(fs); 
		        }
                else //одиночный фильтр
                {
                    filtersEnum += GetSingleFilterMemberUName(fs);
                }
	        }
                        
            return TupleBrackets(filtersEnum);
        }

        /// <summary>
        /// Кортеж представляющий фильтры запроса
        /// </summary>        
        private string FiltersExpression(FieldSetCollection fieldSets)
        {
            string filtersEnum = string.Empty;

            foreach (FieldSet fs in fieldSets)
            {
                filtersEnum = AddTail(filtersEnum, commaDelim);
                if (fs.IsMultipleChoise()) //множественный фильтр
                {
                    filtersEnum += GetMultipleFilterSetName(fs);
                }
                else //одиночный фильтр
                {
                    filtersEnum += GetSingleFilterMemberUName(fs);
                }
            }

            return filtersEnum;
        }

        /// <summary>
        /// Фильтры для сессионного множества строк
        /// </summary>
        /// <returns></returns>
        private string FilterExpressionForSessionRows()
        {
            string filtersEnum = string.Empty;
            filtersEnum = FiltersExpression(this.pivot.FilterAxis.FieldSets);
            filtersEnum = AddTail(filtersEnum, commaDelim);
            filtersEnum += FiltersExpression(this.pivot.ColumnAxis.FieldSets);
            return filtersEnum;
        }

        /// <summary>
        /// Секция фильтров
        /// </summary>        
        private string WhereClause()
        {
            return AddHead(" WHERE ", FiltersExpression());
        }
        #endregion

        /// <summary>
        ///инициализируем поля, данными элемента. 
        ///Здесь это лучше, чем тащить кучу параметров вверх по стеку вызовов,
        ///Поскольку public метод у нас тут будет всего один (этот).
        /// </summary>        
        private void InitFields(PivotData pivotData)
        {
            pivot = pivotData;
            cube = pivotData.Cube;
        }

        /// <summary>
        /// Попытка закончить операцию при обрыве подключения
        /// </summary>
        private string CatchException(PivotData pivotData, Exception e, bool isCountQuery)
        {
            if (e is AdomdCacheExpiredException)
            {
                #warning Зацыкливание здесь может быть
                pivotData.RefreshMetadata();
                if (isCountQuery)
                {
                    return BuildMDXQuery(pivotData);    
                }
                else
                {
                    return BuildMDXQueryForRecordCount(pivotData);
                }
            }

            Common.CommonUtils.ProcessException(e);
            return string.Empty;
        }

        /// <summary>
        /// Сборка MDX-запроса для получения данных элемента
        /// </summary>
        /// <param name="elem">Элемент</param>
        /// <returns>Строка запроса</returns>
        public string BuildMDXQuery(PivotData pivotData)
        {        
            try
            {
                InitFields(pivotData);
                this.currentBuildMode = this.useRowsSessionSet ? WithBuildMode.ForRowSesionSet : WithBuildMode.Standart;
                //если используем сессионное множество, сборка секции With должна идти в соответствующем режиме
                string withClause = WithClause(this.currentBuildMode);
                string axesClause = AxesClause();

                if (!String.IsNullOrEmpty(axesClause))
                {
                    return withClause + axesClause + FromClause() + WhereClause();
                }
                else
                {
                    return String.Empty;
                }

            }
            catch(Exception e)
            {
                return CatchException(pivotData, e, false);                
            }
        }
        
        /// <summary>
        /// Запрос для получения кол-ва элеменов в строках
        /// </summary>
        public string BuildMDXQueryForRecordCount(PivotData pivotData)
        {
            try
            {
                this.InitFields(pivotData);
                if (pivot.RowAxis.FieldSets.Count == 0) 
                    return string.Empty;

                this.currentBuildMode = WithBuildMode.ForRecordCount;
                
                string result = this.WithClause(this.currentBuildMode);
                if (result == string.Empty) 
                    result = "WITH ";                
                
                result = string.Format("{0} MEMBER [Measures].[RowCount] AS 'count({1})'",
                    result, this.FullAxisSet(QueryAxisType.axRows));

                result += " SELECT {[Measures].[RowCount]} ON COLUMNS " + FromClause() + WhereClause();
                return result;
            }
            catch (Exception e)
            {
                return CatchException(pivotData, e, true);                
            }
        }

        /// <summary>
        /// Запрос для получения элементов в строках
        /// </summary>
        public string BuildMDXQueryForPenultimateMembers(PivotData pivotData)
        {
            try
            {
                this.InitFields(pivotData);
                if (pivot.RowAxis.FieldSets.Count == 0)
                    return string.Empty;

                this.currentBuildMode = WithBuildMode.Standart;
                string parentsMembersSet = String.Empty;

                if (this.pivot.RowAxis.FieldSets.Count > 0)
                {
                    FieldSet fs = this.pivot.RowAxis.FieldSets.GetLastItem();
                    if (fs.Fields.Count > 1)
                    {
                        //Получаем множество всех элементов последнего филдсета
                        string fsMemberSet = DimensionSet(fs.MemberNames, fs.UniqueName);

                        string parentsLevelName = fs.Fields[fs.Fields.Count - 2].UniqueName;
                        //множество элеметов на последнем уровне
                        parentsMembersSet = "{Intersect(" + fsMemberSet + ", {" + parentsLevelName + ".AllMembers})}";
                        
                        if (pivot.RowAxis.HideEmptyPositions)
                        {
                            parentsMembersSet = String.Format("Non Empty({0})", parentsMembersSet);
                        }
                    }
                }

                if (String.IsNullOrEmpty(parentsMembersSet))
                {
                    return String.Empty;
                }

                string result = "SELECT " + parentsMembersSet + " ON COLUMNS " + FromClause();
                return result;
            }
            catch (Exception e)
            {
                return CatchException(pivotData, e, true);
            }
        }


        /// <summary>
        /// Если сбор запроса идет для множества или для оси строк в в многостраничной таблице,
        /// то режим Non Empty мы использовать не можем, будем применять альтернативный UsingFilter
        /// </summary>
        /// <param name="isBuildSet"></param>
        /// <returns></returns>
        private HideEmptyMode GetHideEmptyMode(PivotAxis ax, bool isBuildSet)
        {
            if ((isBuildSet || (this.IsTableElement && this.usePaging && ax.AxisType == AxisType.atRows))
                && (ax.HideEmptyMode == HideEmptyMode.NonEmpty))
            {
                HideEmptyMode defaultEmptyMode = (PivotData.AnalysisServicesVersion != AnalysisServicesVersion.v2000) ?
                    HideEmptyMode.NonEmpty2005 : HideEmptyMode.UsingFilter;
                return (ax.HideEmptyMode == HideEmptyMode.NonEmpty) ? defaultEmptyMode : ax.HideEmptyMode;
            }
            else
                return ax.HideEmptyMode;
        }

        /// <summary>
        /// Оператор для создания сессионного сета, представляющего строки
        /// </summary>
        public string BuildMDXQueryForRowsSessionSet(PivotData pivotData)
        {
            try            
            {
                InitFields(pivotData);

                string result = string.Format("CREATE SESSION SET {0}.{1} AS '{2}' SET {3}.{4} AS '{5}'", 
                    //Не фильтрованное множество
                    MemberBrackets(cube.Name), NonFilterRowSetName, NonFilterAxisSet(QueryAxisType.axRows, 
                    this.GetHideEmptyMode(this.pivot.RowAxis, false)),
                    //Сдесь уже фильтруем
                    MemberBrackets(cube.Name), RowsSessionSetName, AxisClause(QueryAxisType.axRows, true));

                result += MultipleFiltersForRowSessionSets();

                string calculateMember = this.GetCalcMembersDeclaration(true);
                if (calculateMember != string.Empty)
                    result = string.Format("{0} {1}", result, calculateMember);

                return result;
            }
            catch (Exception e)
            {                
                return CatchException(pivotData, e, false);
            }
        }
        
        /// <summary>
        /// Конструкция для удаления сессионого сета строк
        /// </summary>
        public string MDXDropRowsSessionSet(PivotData pivotData)
        {
            return string.Format("DROP SET {0}.{1}", MemberBrackets(pivotData.Cube.Name), 
                RowsSessionSetName);
        }

        public string MDXDropNonFilterRowSessionSet(PivotData pivotData)
        {
            return string.Format("DROP SET {0}.{1}", MemberBrackets(pivotData.Cube.Name), 
                NonFilterRowSetName);   
        }

        public string MDXDropVisualTotals(PivotData pivotData)
        {
            this.InitFields(pivotData);
            return string.Format("DROP VISUAL TOTALS FOR {0}", MemberBrackets(pivotData.Cube.Name));
        }

        public string MDXDropCalulateMember(PivotData pivotData, int totalIndex)
        {
            this.InitFields(pivotData);
            string totalClause = this.GetCalcMemberDeclaration(totalIndex, false, true);
            if (totalClause != string.Empty)
                return string.Format("DROP {0}", totalClause);
            else
                return string.Empty;
        }

        public string MDXDropLookupCubeMember(PivotTotal total)
        {
            string totalClause = GetLookupMemberName(total);

            if (totalClause != string.Empty)
            {
                return string.Format("DROP MEMBER {0}", totalClause);
            }
            else
                return string.Empty;
        }

        public string MDXDropSessionFilterSet(PivotData pivotData, FieldSet fs)
        {
            this.InitFields(pivotData);
            if (fs.IsMultipleChoise())
            {
                return string.Format("DROP MEMBER {0}.{1}", MemberBrackets(cube.Name), GetMultipleFilterSetName(fs));
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Запрос для получения первого кортежа строк.
        /// </summary>
        public string BuildMDXQueryForFirstTupleOfRows(PivotData pivotData)
        {
            try
            {
                this.InitFields(pivotData);
                string axisStr = string.Format(" SELECT subset({0}, 0, 1) on 0 ", RowsSessionSetName);                
                return axisStr + FromClause();
            }
            catch (Exception e)
            {
                return CatchException(pivotData, e, false);
            }
        }

        /// <summary>
        /// MDX-множеств дополнящющее строки одной страницы
        /// </summary>
        /// <param name="pos">Позиция оси селлсета по кторой собирается множество</param>
        /// <param name="elem">элемент</param>       
        public string HeaderSetForPage(Position pos, PivotData pivotData)
        {
            InitFields(pivotData);
            string result = string.Empty;
            string setByMember = string.Empty;
            FieldSetCollection fsc = pivot.RowAxis.FieldSets;

            for (int i = 0; i < fsc.Count; i++)
            {
                if (fsc[i].Fields[0].UniqueName != pos.Members[i].LevelName)
                {
                    setByMember = string.Format("Hierarchize(Intersect(Ascendants({0}), {1}))",
                        pos.Members[i].UniqueName, DimensionCheckedLevelsSet(fsc[i]));
                }
                else
                {
                    string allMemberUN = fsc[i].AllMemberUN;

                    if ((i == 0) || ((i != 0) && (allMemberUN == pos.Members[i].UniqueName)) || (allMemberUN == string.Empty))
                        setByMember = SetBrackets(pos.Members[i].UniqueName);
                    else
                        setByMember = SetBrackets(string.Format("{0}, {1}", allMemberUN,
                            pos.Members[i].UniqueName));
                }

                result = AddTail(result, asterixDelim);
                result += setByMember;
            }
            return this.EncodeSet(result);
        }


        #region Temp. For replace
        
        /// <summary>
        /// MDX-условие для исключения "левых" кортежей из множества.
        /// "Левый" кортеж - это такой, который детализирует мемберы не листовых уровней.
        /// Условие прохождения кортежа: (если уровень мембера является последним включенным из измерения)
        /// or (мембер листовой) or (элемент кортежа не равен предыдущему на этом же уровне)
        /// </summary>
        /// <param name="fs">Набор полей, соответствующей оси пивот даты.</param>
        private string HiddenTuplesFilterCondition(QueryAxisType axType)
        {
            string result = string.Empty; //собираем все условие
            FieldSetCollection fs = this.GetPivotAxis(axType).FieldSets;

            int fsCount = fs.FieldSetIsPreset("[Measures]") ? fs.Count - 1 : fs.Count;
            if (fsCount > 1)//для одного измерения собирать фильтры не имеет смысла
            {
                for (int i = 0; i < fs.Count - 1; i++)
                {
                    FieldSet curFieldSet = fs[i];
                    if (curFieldSet.UniqueName == "[Measures]")
                        continue;

                    string lastIncludeInMdxLevelUn = this.GetLastIncludedInMdxLevelUN(curFieldSet);
                    if (lastIncludeInMdxLevelUn == string.Empty)
                        continue;
                    
                    string condForDim = string.Empty;//условие для одного измерения

                    string changedLevels = String.Empty;
                    //для диаграммы, синхронизированной по таблице нужно иметь возможность отображать 
                    //нелистовые элементы, которые в таблице схлопнуты 
                    if (this.IsChartElement || this.IsMultiGaugeElement)
                    {
                        foreach (PivotField field in curFieldSet.Fields)
                        {
                            if (changedLevels != String.Empty)
                                changedLevels += " OR ";

                            changedLevels += string.Format("({0}.CurrentMember.Level IS {1})",
                                                          curFieldSet.UniqueName, field.UniqueName);
                        }
                    }
                    else
                    {
                        changedLevels = string.Format("({0}.CurrentMember.Level IS {1})",
                                                             curFieldSet.UniqueName, lastIncludeInMdxLevelUn);
                    }

                    string notEqualPrepare = string.Format("not ({0}.Item(Rank({0}.Current, {0}) -2).Item({1}) is {0}.Current.Item({1}))", 
                        this.GetNonFilterSetName(axType), i);

                    condForDim = string.Format("{0} or IsLeaf({1}) or {2}",
                        changedLevels,
                        curFieldSet.UniqueName,
                        notEqualPrepare);

                    condForDim = string.Format("({0})", condForDim);

                    result = AddTail(result, " and ");
                    result += TupleBrackets(condForDim);
                }
            }
            return result;
        }

        /// <summary>
        /// Получить юник нейм, последнего включенного в запрос уровня измерения
        /// </summary>
        /// <param name="fieldSet"></param>
        /// <returns></returns>
        private string GetLastIncludedInMdxLevelUN(FieldSet fieldSet)
        {
            for (int i = fieldSet.Fields.Count - 1; i >= 0; i--)
            {
                PivotField field = fieldSet.Fields[i];
                if (field.IsIncludeToQuery)
                    return field.UniqueName;
            }
            return string.Empty;
        }
        #endregion

        #region Свойства
        public ReportElementType ElementType
        {
            get { return elementType; }
            set { elementType = value; }
        }

        private bool IsTableElement
        {
            get { return this.ElementType == ReportElementType.eTable; }
        }

        private bool IsChartElement
        {
            get { return this.ElementType == ReportElementType.eChart; }
        }

        private bool IsMultiGaugeElement
        {
            get { return this.ElementType == ReportElementType.eMultiGauge; }
        }


        private bool IsMapElement
        {
            get { return this.ElementType == ReportElementType.eMap; }
        }

        #endregion

        #region Вспомогательные классы
        /// <summary>
        /// Тип оси запроса
        /// </summary>
        private enum QueryAxisType
        {
            axRows,
            axColumns,
            axSeries
        }

        /// <summary>
        /// Режим построения секции With
        /// </summary>
        private enum WithBuildMode
        {
            /// <summary>
            /// стандартный (включает не фильтрованные множества строк и колонок)
            /// </summary>
            Standart,
            /// <summary>
            /// для уже созданного сессионого множества (включает только не фильтрованное 
            /// множество столбцов)
            /// </summary>
            ForRowSesionSet,
            /// <summary>
            /// для получения количества строк (включает только не фильтрованное 
            /// множество строк)
            /// </summary>
            ForRecordCount

        }
        #endregion
    }
}
