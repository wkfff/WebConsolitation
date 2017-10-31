using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;

// Часть отвечающая за работу с гридом фильтрации по которому формируется 
// SQL-ограничение на выборку данных (сейчас только для таблиц фактов)
namespace Krista.FM.Client.Components
{
    public partial class UltraGridEx : UserControl, Infragistics.Win.IUIElementCreationFilter
    {
        // навзание пункта выпадающего списка для выбора из справочника
        public static string HandbookSelectCaption = "Выбрать из справочника ...";

        List<string> documentColumnNames;

        private bool _serverFilterEnabled = false;
        /// <summary>
        /// Доступность режима серверного фильтра
        /// </summary>
        public bool ServerFilterEnabled
        {
            get { return _serverFilterEnabled; }
            set
            {
                _serverFilterEnabled = value;
                utmMain.Tools["ClearFilter"].SharedProps.Visible = !value;
            }
        }

        private bool ServerFilterPresent()
        {
            return ((_lastFilterHash != String.Empty.GetHashCode()) && ServerFilterEnabled);
        }

        private GetHandbookValue _onGetHandbookValue;
        /// <summary>
        /// Обработчик нажатия пункта "Выбор из справочника"
        /// </summary>
        public event GetHandbookValue OnGetHandbookValue
        {
            add { _onGetHandbookValue += value; }
            remove { _onGetHandbookValue -= value; }
        }

        private GetServerFilterCustomDialogColumnsList _onGetServerFilterCustomDialogColumnsList;
        /// <summary>
        /// Обработчик события формирования списка колонок которые могут участвовать в фильтре на текущую колонку.
        /// Возникает при нажатии на выпадающий список операндов в диалоге сложного фильтра. Для операций сравнения.
        /// </summary>
        public event GetServerFilterCustomDialogColumnsList OnGetServerFilterCustomDialogColumnsList
        {
            add { _onGetServerFilterCustomDialogColumnsList += value; }
            remove { _onGetServerFilterCustomDialogColumnsList -= value; }
        }

        /// <summary>
        /// Сброс настроек фильтра
        /// </summary>
        public void ResetServerFilter()
        {
            if (!ServerFilterEnabled)
                return;
            ugFilter.DataSource = null;
            _lastFilterHash = String.Empty.GetHashCode();
        }

        // Получение списка настроенных фильтров
        private ColumnFilter[] GetGridFilters()
        {
            List<ColumnFilter> gridFilters = new List<ColumnFilter>();

            foreach (ColumnFilter filter in ugFilter.DisplayLayout.Bands[0].ColumnFilters)
            {
                // пропускаем ненастроенные фильтры
                if ((filter.FilterConditions.Count == 0) ||
                    ((filter.FilterConditions.Count == 1) &&
                    (filter.FilterConditions[0].ComparisionOperator == FilterComparisionOperator.Custom)))
                    continue;

                gridFilters.Add(filter);
            }

            if (gridFilters.Count == 0)
                return null;
            else
                return gridFilters.ToArray();
        }

        /// <summary>
        /// Вспомогательный класс для создания параметров фильтра.
        /// Хранит название параметра и его значение.
        /// </summary>
        public class FilterParamInfo
        {
            public string ParamName;
            public object ParamValue;

            public FilterParamInfo(string paramName, object paramValue)
            {
                ParamName = paramName;
                ParamValue = paramValue;
            }
        }

        // Хэш текущего фильтра. Используется для определения необходимости подсвечивания 
        // кнопки рефреша при изменении фильтра.
        private int _lastFilterHash = String.Empty.GetHashCode();

        // Вычисление хэша фильтра. Учитывает строковое представление фильтра, названия и значения параметров.
        private int CalcFilterHash(string filterExpression, List<FilterParamInfo> filterParameters)
        {
            int hash = String.Empty.GetHashCode();
            if (!String.IsNullOrEmpty(filterExpression))
            {
                hash = filterExpression.GetHashCode();
                foreach (FilterParamInfo prm in filterParameters)
                {
                    hash = hash + prm.ParamName.GetHashCode() + prm.ParamValue.GetHashCode();
                }
            }
            return hash;
        }

        // Занесение информации о параметре в список. Обрабатывает ситуацию когда значение параметра 
        // имеет недопустимый тип, в этом случае значение очищается.
        private static bool AddFilterParameter(List<FilterParamInfo> parameters, UltraGridColumn clmn, 
            FilterCondition condition)
        {
            object val = null;
            try
            {
                val = Convert.ChangeType(condition.CompareValue, clmn.DataType);
            }
            catch
            {
                // фильтр не реагирует на маску ввода колонки, может произойти исключение при преобразовании 
                // типов в этом случае параметр не обрабатываем
                condition.CompareValue = null;
                return false;
            }
            string prmName = String.Concat(clmn.Key, parameters.Count.ToString());
            //string prmName = clmn.Key;
            parameters.Add(new FilterParamInfo(prmName, val));
            return true;
        }

        // построение ограничения для одной колонки
        private bool BuildFilterForColumn(out string filterExpression, List<FilterParamInfo> filterParameters,
            ColumnFilter clmnFilter)
        {
            List<string> filtersCollection = new List<string>();
            // .. для каждого из условий фильтра формируем строку ограничения
            foreach (FilterCondition condition in clmnFilter.FilterConditions)
            {
                string expression = String.Empty;
                bool parameterIsValid = true;
                switch (condition.CompareValue.ToString())
                {
                    // отдельно обрабатываем проверки на пустоту значения
                    case "(Не пустые)":
                        expression = clmnFilter.Column.DataType == typeof(string) ? 
                            String.Format("(NOT ({0} is NULL) && NOT ({0}) = '')", clmnFilter.Column.Key) :
                            String.Format("(NOT {0} is NULL)", clmnFilter.Column.Key);
                        break;
                    case "(Пустые)":
                        expression = clmnFilter.Column.DataType == typeof(string) ? 
                            String.Format("({0} is NULL or {0} = '')", clmnFilter.Column.Key) :
                            String.Format("({0} is NULL)", clmnFilter.Column.Key);
                        break;
                    default:
                        string value;
                        if (clmnFilter.Column.DataType == typeof(string))
                            value = String.Format("'{0}'", condition.CompareValue);
                        else
                            value = condition.CompareValue.ToString();
                        #region а не является ли значение ссылкой на колонку?
                        // если значение не пусто, начинается и заканчивается со скобки, оно может являться
                        // ограничением на колонку ...
                        bool valueIsColumn = (!String.IsNullOrEmpty(value))&&
                            (value[0] == '[') && (value[value.Length - 1] == ']');
                        // ... в этом случае ищем в гриде колонку с таким же заголовком, если находим - 
                        // запоминаем ее имя и окончательно ставим флаг о том что параметр это колонка
                        string argColumnName = String.Empty;
                        if (valueIsColumn)
                        {
                            string argColumnCaption = value.Trim('[', ']');
                            foreach (UltraGridColumn clmn in ugFilter.DisplayLayout.Bands[0].Columns)
                            {
                                valueIsColumn = (clmn.Header.Caption == argColumnCaption);
                                if (valueIsColumn)
                                {
                                    argColumnName = clmn.Key;
                                    break;
                                }
                            }
                        }
                        #endregion
                        #region Формирование условия для разных типов операторов
                        // несколько громоздко, надо бы упростить...
                        switch (condition.ComparisionOperator)
                        {
                            case FilterComparisionOperator.Equals:
                                if (!valueIsColumn)
                                {
                                    expression = String.Format("({0} = ?)", clmnFilter.Column.Key);
                                    parameterIsValid = AddFilterParameter(filterParameters, clmnFilter.Column, condition);
                                }
                                else
                                {
                                    expression = String.Format("({0} = {1})", clmnFilter.Column.Key, argColumnName);
                                }
                                break;
                            case FilterComparisionOperator.NotEquals:
                                if (!valueIsColumn)
                                {
                                    expression = String.Format("({0} <> ?)", clmnFilter.Column.Key);
                                    parameterIsValid = AddFilterParameter(filterParameters, clmnFilter.Column, condition);
                                }
                                else
                                {
                                    expression = String.Format("({0} <> {1})", clmnFilter.Column.Key, argColumnName);
                                }
                                break;
                            case FilterComparisionOperator.GreaterThan:
                                if (!valueIsColumn)
                                {
                                    expression = String.Format("({0} > ?)", clmnFilter.Column.Key);
                                    parameterIsValid = AddFilterParameter(filterParameters, clmnFilter.Column, condition);
                                }
                                else
                                {
                                    expression = String.Format("({0} > {1})", clmnFilter.Column.Key, argColumnName);
                                }
                                break;
                            case FilterComparisionOperator.GreaterThanOrEqualTo:
                                if (!valueIsColumn)
                                {
                                    expression = String.Format("({0} >= ?)", clmnFilter.Column.Key);
                                    parameterIsValid = AddFilterParameter(filterParameters, clmnFilter.Column, condition);
                                }
                                else
                                {
                                    expression = String.Format("({0} >= {1})", clmnFilter.Column.Key, argColumnName);
                                }
                                break;
                            case FilterComparisionOperator.LessThan:
                                if (!valueIsColumn)
                                {
                                    expression = String.Format("({0} < ?)", clmnFilter.Column.Key);
                                    parameterIsValid = AddFilterParameter(filterParameters, clmnFilter.Column, condition);
                                }
                                else
                                {
                                    expression = String.Format("({0} < {1})", clmnFilter.Column.Key, argColumnName);
                                }
                                break;
                            case FilterComparisionOperator.LessThanOrEqualTo:
                                if (!valueIsColumn)
                                {
                                    expression = String.Format("({0} <= ?)", clmnFilter.Column.Key);
                                    parameterIsValid = AddFilterParameter(filterParameters, clmnFilter.Column, condition);
                                }
                                else
                                {
                                    expression = String.Format("({0} <= {1})", clmnFilter.Column.Key, argColumnName);
                                }
                                break;
                            case FilterComparisionOperator.Like:
                                expression = String.Format("(UPPER({0}) LIKE '%{1}%')", clmnFilter.Column.Key, condition.CompareValue.ToString().ToUpper());
                                break;
                            case FilterComparisionOperator.NotLike:
                                expression = String.Format("(NOT (UPPER({0}) LIKE '%{1}%'))", clmnFilter.Column.Key, condition.CompareValue.ToString().ToUpper());
                                break;
                            default:
                                throw new NotSupportedException("Операция не поддерживается");
                        }
                        #endregion
                        break;
                }
                // если ограничение сформировано без ошибок - добавляем его в список
                if (parameterIsValid)
                    filtersCollection.Add(expression);
                else
                {
                    if (ugFilter.ActiveCell != null)
                    {
                        // иначе - очищаем его
                        ugFilter.ActiveCell.Value = String.Empty;
                        if (ugFilter.ActiveCell.Editor != null)
                            ugFilter.ActiveCell.Editor.Value = String.Empty;
                    }
                }
            }
            // объединяем условия в зависимости от типа логической операции
            if (clmnFilter.LogicalOperator == FilterLogicalOperator.And)
                filterExpression = String.Join(" AND ", filtersCollection.ToArray());
            else
                filterExpression = String.Join(" OR ", filtersCollection.ToArray());
            // если все получилось - возвращаем сформированное ограничение и true
            if (!String.IsNullOrEmpty(filterExpression))
            {
                filterExpression = String.Format("({0})", filterExpression);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Сформировать SQL-ограничение на выборку (серверный фильтр)
        /// </summary>
        /// <param name="filterExpression">SQL-ограничение</param>
        /// <param name="filterParameters">список параметров</param>
        /// <returns>Успешность завершения операции</returns>
        public bool BuildServerFilter(out string filterExpression, out List<FilterParamInfo> filterParameters)
        {
            return BuildServerFilter(out filterExpression, out filterParameters, true);
        }

        /// <summary>
        /// Сформировать SQL-ограничение на выборку (серверный фильтр)
        /// </summary>
        /// <param name="filterExpression">SQL-ограничение</param>
        /// <param name="filterParameters">список параметров</param>
        /// <param name="calcLastFilterHash">необходимость расчета хэша фильтра</param>
        /// <returns></returns>
        private bool BuildServerFilter(out string filterExpression, out List<FilterParamInfo> filterParameters, bool calcLastFilterHash)
        {
            // инициализируем выходные параметры пустыми значениями
            filterExpression = String.Empty;
            filterParameters = null;
            if (calcLastFilterHash)
                _lastFilterHash = CalcFilterHash(filterExpression, filterParameters);
            // получаем все настроенные фильтры
            ColumnFilter[] gridFilters = GetGridFilters();
            // если их нет или фильтр отключен - возвращаем false
            if ((gridFilters == null) || (!ServerFilterEnabled))
                return false;
            List<string> filtersCollection = new List<string>();
            filterParameters = new List<FilterParamInfo>();
            // формируем SQL-выражение и параметры для каждого фильтра грида
            foreach (ColumnFilter filter in gridFilters)
            {
                string columnFilter;
                if (BuildFilterForColumn(out columnFilter, filterParameters, filter))
                    filtersCollection.Add(columnFilter);
            }
            // объединяем ограничения для всех колонок
            filterExpression = String.Join(" AND ", filtersCollection.ToArray());
            // при необходимости вычисляем хэш
            if (calcLastFilterHash)
                _lastFilterHash = CalcFilterHash(filterExpression, filterParameters);
            return true;
        }

        #region Сохранение/восстановление фильтра при рефреше
        // вспомогательный стрим для хранения настроек   
        private MemoryStream filterSettings = null;

        /// <summary>
        /// Запомнить настройки фильтра
        /// </summary>
        public void SaveServerFilter()
        {
            if (!ServerFilterEnabled)
                return;
            filterSettings = new MemoryStream();
            ugFilter.DisplayLayout.Save(filterSettings, PropertyCategories.ColumnFilters);
        }

        /// <summary>
        /// Восстановить настройки фильтра
        /// </summary>
        public void RestoreServerFilter()
        {
            if ((!ServerFilterEnabled) || (filterSettings == null))
                return;
            filterSettings.Position = 0;
            ugFilter.DisplayLayout.Load(filterSettings, PropertyCategories.ColumnFilters);
            // хоть ширину запоминать и не просили, она все равно запомнилась..
            // синхронизируем ее заново
            //SyncColumnsWidth();
            // восстанавлиаем хэш фильтра
            string newFilter;
            List<FilterParamInfo> parameters;
            BuildServerFilter(out newFilter, out parameters, true);
        }
        #endregion

        // начальная инициализация режима серверного фильтра
        private void InitServerFilter()
        {
            //ugFilter.DataSource = null;
            // Показываем или прячем грид фильтра
            ugFilter.Visible = ServerFilterEnabled;
            if (!ServerFilterEnabled)
                return;
            // клонируем источник данных основного грида (для получения таких же колонок, но без данных)
            if (_dataSourceType == typeof(DataSet))
            {
                ugFilter.DataSource = ((DataSet)_dataSource).Clone();
            }
            else if (_dataSourceType == typeof(DataTable))
                    {
                        ugFilter.DataSource = ((DataTable)_dataSource).Clone();
                    }
            // подстраиваем высоту
            //SyncColumnsWidth();
            RefreshFilterHeight();
            
        }

        // Можно ли спрятать колонку (нельзя прятать колонки на которые назначен фильтр)
        private bool CanHideColumn(string columnName)
        {
            if (String.IsNullOrEmpty(columnName) || (!ServerFilterEnabled))
                return true;

            ColumnFilter[] filters = GetGridFilters();
            if (filters == null)
                return true;

            bool finded = false;
            foreach (ColumnFilter filter in filters)
            {
                finded = filter.Column.Key == columnName;
                if (finded)
                    break;
            }
            return !finded;
        }

        #region Синхронизация состояний основного грида и фильтра
        // подстройка высоты грида фильтра (Высота Заголовка + Высота строки фильтра)
        private void RefreshFilterHeight()
        {
            UIElement headerUIElement = ugFilter.DisplayLayout.Bands[0].Header.GetUIElement();
            if (headerUIElement == null)
                headerUIElement = ugFilter.DisplayLayout.Bands[0].Groups[0].Header.GetUIElement();
            UIElement filterRowUIElement = ugFilter.DisplayLayout.Rows.FilterRow.GetUIElement();
            int h = 0;
            if (filterRowUIElement != null)
            {
                if (headerUIElement != null)
                    h = headerUIElement.Rect.Height + filterRowUIElement.Rect.Height;
                else
                    h = ugFilter.DisplayLayout.Bands[0].Header.Height + 24 + filterRowUIElement.Rect.Height;
            }
            else
            {
                if (headerUIElement != null)
                    h = headerUIElement.Rect.Height + 24;
                else
                    h = ugFilter.DisplayLayout.Bands[0].Header.Height + 24;
            }
            ugFilter.Height = h;
        }
        
        // синхронизация скроллирования по горизонтали
        private void _ugData_BeforeColRegionScroll(object sender, BeforeColRegionScrollEventArgs e)
        {
            if (!ServerFilterEnabled)
                return;
            ugFilter.DisplayLayout.ColScrollRegions[0].Position = e.NewState.Position;
            ugFilter.Refresh();
        }


        private void _ugData_AfterGroupPosChanged(object sender, AfterGroupPosChangedEventArgs e)
        {
            if (e.PosChanged == PosChanged.Sized)
            {
                UltraGridGroup group = e.GroupHeaders[0].Group;
                ColumnHeader[] headers = new ColumnHeader[] {
                    GetLastVisibleGroupColumn(group).Header};
                AfterColPosChangedEventArgs afterColPosChangedEventArgs = new AfterColPosChangedEventArgs(e.PosChanged, headers);
                _ugData_AfterColPosChanged(sender, afterColPosChangedEventArgs);
            }
        }

        private UltraGridColumn GetLastVisibleGroupColumn(UltraGridGroup group)
        {
            for (int i = group.Columns.Count - 1; i >= 0; i --)
            {
                if (!group.Columns[i].Hidden)
                    return group.Columns[i];
            }
            return null;
        }

        // синхронизация изменения ширины колонок
        private void _ugData_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            if (_createUIElement == null)
                if (maxHeight != null)
                    if (maxHeight.Length <= e.ColumnHeaders[0].Band.Index)
                        maxHeight = null;
                    else
                        maxHeight[e.ColumnHeaders[0].Band.Index] = 0;

            if (!ServerFilterEnabled)
                return;
            if (e.PosChanged == PosChanged.Sized)
            {
                UltraGridColumn clmn = e.ColumnHeaders[0].Column;
                string sourceClmnName = GetSourceColumnName(clmn.Key);
                if (ugFilter.DisplayLayout.Bands[0].Columns.Exists(sourceClmnName))
                {
                    if (ugFilter.DisplayLayout.Bands[0].Columns[sourceClmnName].Group != null)
                    {
                        UltraGridGroup group = ugFilter.DisplayLayout.Bands[0].Columns[sourceClmnName].Group;
                        group.RemoveColumn(ugFilter.DisplayLayout.Bands[0].Columns[sourceClmnName]);
                        ugFilter.DisplayLayout.Bands[0].Columns[sourceClmnName].Width = clmn.Width;
                        group.Columns.Add(ugFilter.DisplayLayout.Bands[0].Columns[sourceClmnName]);
                        ugFilter.DisplayLayout.Bands[0].Columns[sourceClmnName].Header.VisiblePosition =
                            clmn.Header.VisiblePosition;
                    }
                    else
                    {
                        if (!documentColumnNames.Contains(sourceClmnName.ToUpper()))
                        {
                            ugFilter.DisplayLayout.Bands[0].Columns[sourceClmnName].Width = clmn.Width;
                        }
                        else
                            ugFilter.DisplayLayout.Bands[0].Columns[sourceClmnName].Width = clmn.Width + 21 * 3;
                    }
                    RefreshFilterHeight();
                }
            }

            if (e.PosChanged == PosChanged.Moved)
            {
                UltraGridColumn clmn = e.ColumnHeaders[0].Column;
                string sourceClmnName = GetSourceColumnName(clmn.Key);
                if (ugFilter.DisplayLayout.Bands[0].Columns.Exists(sourceClmnName))
                {
                    ugFilter.DisplayLayout.Bands[0].Columns[sourceClmnName].Header.VisiblePosition =
                        clmn.Header.VisiblePosition;
                }
            }
        }

        private void ugFilter_PropertyChanged(object sender, Infragistics.Win.PropertyChangedEventArgs e)
        {
            if (e.ChangeInfo.FindPropId(Infragistics.Win.UltraWinGrid.PropertyIds.Hidden) != null)
            {
                RefreshFilterHeight();
            }

        }

        // тоже синхронизация только для всех колонок сразу. Нужна для настройки колонок после восстановления 
        // состояния - т.к. там почему-то ширина запоминается
        private void  SyncColumnsWidth()
        {
            if (!ServerFilterEnabled)
                return;
            ugFilter.BeginUpdate();
            ugFilter.EventManager.SetEnabled(EventGroups.AllEvents, false);
            try
            {
                for (int i = 0; i < ugData.DisplayLayout.Bands[0].Columns.Count; i++)
                {
                    string sourceClmnName = GetSourceColumnName(ugData.DisplayLayout.Bands[0].Columns[i].Key);
                    if (!documentColumnNames.Contains(sourceClmnName.ToUpper()))
                        ugFilter.DisplayLayout.Bands[0].Columns[sourceClmnName].Width = ugData.DisplayLayout.Bands[0].Columns[i].Width;
                    else
                        ugFilter.DisplayLayout.Bands[0].Columns[sourceClmnName].Width = ugData.DisplayLayout.Bands[0].Columns[i].Width + 21*3;
                }
            }
            catch
            { }
            finally
            {
                ugFilter.EventManager.SetEnabled(EventGroups.AllEvents, true);
                ugFilter.EndUpdate();
            }
        }

        #endregion

        #region Настройка внешнего вида
        // инициализация слоя
        private void ugFilter_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            documentColumnNames = new List<string>();
            e.Layout.ViewStyleBand = ViewStyleBand.Vertical;
            e.Layout.BorderStyle = UIElementBorderStyle.None;
            
            e.Layout.Override.HeaderClickAction = HeaderClickAction.Select;
            e.Layout.Override.BorderStyleHeader = UIElementBorderStyle.None;
            e.Layout.Override.HeaderAppearance.BackColor = e.Layout.GroupByBox.Appearance.BackColor;
            e.Layout.Override.HeaderAppearance.ForeColor = SystemColors.Control;
            e.Layout.Appearance.BackColor = e.Layout.GroupByBox.Appearance.BackColor;
            e.Layout.Override.TipStyleCell = TipStyle.Hide;
            e.Layout.Override.FilterUIType = FilterUIType.FilterRow;
            e.Layout.Override.FilterClearButtonLocation = FilterClearButtonLocation.Row;
            e.Layout.Override.BorderStyleFilterOperator = UIElementBorderStyle.None;
            e.Layout.Scrollbars = Scrollbars.None;

            foreach (string group in Groups.Keys)
            {
                if (ugData.DisplayLayout.Bands[0].Groups.Exists(group) && !e.Layout.Bands[0].Groups.Exists(group))
                    e.Layout.Bands[0].Groups.Add(group, group);
            }

            e.Layout.Bands[0].GroupHeadersVisible = false;

            foreach (UltraGridBand band in e.Layout.Bands)
            {
                // колонка состояния (нужна ли она вообще?)
                if (StateRowEnable)
                    AppendStateColumn(band.Columns);
                else
                    if (band.Columns.Contains(StateColumnName))
                        band.Columns.Remove(StateColumnName);
                
                // ставим для колонок грида параметры
                if (_onGetGridColumnsState == null)
                    break;
                GridColumnsStates states = _onGetGridColumnsState(this);

                // иначе - применяем настройки к каждой из колонок
                foreach (GridColumnState curState in states.Values)
                {
                    // если такой колонки нет - пропускаем описание 
                    if (!band.Columns.Exists(curState.ColumnName))
                        continue;
                    // иначе - устанавдиваем параметры
                    UltraGridColumn column = band.Columns[curState.ColumnName];

                    column.LockedWidth = true;
                    // ставим русское название в заголовок
                    column.Header.Caption = curState.ColumnCaption;
                    // позиция в гриде
                    if (curState.ColumnPosition > 0)
                        column.Header.VisiblePosition = curState.ColumnPosition;
                    // если колонка помечена только для чтения, ставим невозможность ее редактирования
                    if (curState.IsSystem)
                    {
                        column.CellActivation = Activation.NoEdit;
                        column.CellAppearance.BackColor = System.Drawing.Color.Gainsboro;
                    }

                    if (curState.IsNullable &&
                        !curState.IsSystem &&
                        !curState.IsReadOnly)
                    {
                        column.CellAppearance.BackColor = System.Drawing.SystemColors.Info;
                    }

                    column.Width = ugData.DisplayLayout.Bands[0].Columns[GetGridColumnName(curState.ColumnName)].Width;

                    if (band.Groups.Exists(curState.GroupName))
                    {
                        column.Group = band.Groups[curState.GroupName];
                    }

                    /*
                    // ставим колонке определенный размер
                    switch (column.Header.Caption)
                    {
                        case "ID":
                            column.Width = 60;
                            break;
                        case "":
                            column.Width = 20;
                            break;
                        default:
                            if (curState.ColumnWidth > 255)
                                column.Width = 255;
                            else
                                column.Width = 120;
                            break;
                    }*/

                    if (curState.IsBLOB)
                        documentColumnNames.Add(string.Format("{0}{1}", curState.ColumnName, "Name").ToUpper());

                    if (documentColumnNames.Contains(curState.ColumnName.ToUpper()))
                        column.Width = column.Width + 21 * 3;
                    // если колонка помечена как скрытая, то скрываем ее
                    column.Hidden = curState.IsHiden;

                    // если указана какой то стиль колонки, то применим его
                    column.Style = curState.ColumnStyle;
                    if (column.DataType == typeof(DateTime))
                    {
                        column.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                        column.FilterOperandStyle = FilterOperandStyle.Combo;
                    }

                    column.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnMouseEnter;
                    //убираем лишние элементы
                    if (column.DataType == typeof(string))
                    {
                        column.FilterOperatorDropDownItems =
                            FilterOperatorDropDownItems.Equals |
                            FilterOperatorDropDownItems.NotEquals |
                            FilterOperatorDropDownItems.LessThan |
                            FilterOperatorDropDownItems.LessThanOrEqualTo |
                            FilterOperatorDropDownItems.GreaterThan |
                            FilterOperatorDropDownItems.GreaterThanOrEqualTo |
                            FilterOperatorDropDownItems.Like |
                            FilterOperatorDropDownItems.NotLike;
                        column.FilterOperatorDefaultValue = FilterOperatorDefaultValue.Like;
                    }
                    if (column.DataType == typeof(int) || column.DataType == typeof(decimal) || column.DataType == typeof(double) || column.DataType == typeof(long))
                    {
                        column.FilterOperatorDropDownItems =
                            FilterOperatorDropDownItems.Equals |
                            FilterOperatorDropDownItems.NotEquals |
                            FilterOperatorDropDownItems.LessThan |
                            FilterOperatorDropDownItems.LessThanOrEqualTo |
                            FilterOperatorDropDownItems.GreaterThan |
                            FilterOperatorDropDownItems.GreaterThanOrEqualTo |
                            FilterOperatorDropDownItems.Like;
                    }
                }
            }
        }



        bool isCustomFilter;

        private void ugFilter_BeforeRowFilterDropDown(object sender, BeforeRowFilterDropDownEventArgs e)
        {
            isCustomFilter = false;
        }

        // настройка диалога создания сложных фильтров
        private void ugFilter_BeforeCustomRowFilterDialog(object sender, BeforeCustomRowFilterDialogEventArgs e)
        {
            isCustomFilter = false;
            if ((e.CustomRowFiltersDialog != null) && (e.CustomRowFiltersDialog.Grid != null))
            {
                if (e.Column.DataType == typeof(DateTime))
                {
                    e.Cancel = true;
                    isCustomFilter = true;
                    ClearFilterParams(e.Column.Key);
                    //if (isCustomFilter)
                    if (DateTimeCustomFilter.ShowDateTimeCustomFilter(e.Column.Header.Caption, ref sender, e.Column.Key))
                        BurnRefreshDataButton(true);
                }
                else
                {
                    isCustomFilter = false;
                    cancelCustomFilterDialog = false;
                    UltraGridHelper.CustomizeRowFilerDialog(e.CustomRowFiltersDialog, e.Column.Header.Caption);
                    // навешиваем обработчик на выбор из списка, чтобы отловить необходимость выбора из справочника
                    e.CustomRowFiltersDialog.Grid.AfterCellListCloseUp += new CellEventHandler(this.ugFilter_AfterCellListCloseUp);
                    // и еще один для фильтрации колонок попадающих в фильтр
                    e.CustomRowFiltersDialog.Grid.BeforeCellListDropDown += new CancelableCellEventHandler(OnCustomFilterDialogGrid_BeforeCellDropDownList);
                    // пустой текст = String.Empty, а не "DBNull" как в исходном варианте
                    e.CustomRowFiltersDialog.Grid.DisplayLayout.Bands[0].Columns["Operand"].NullText = String.Empty;
                    e.CustomRowFiltersDialog.FormClosing += new FormClosingEventHandler(CustomRowFiltersDialog_FormClosing);
                }
            }
        }

        private bool cancelCustomFilterDialog;
        void CustomRowFiltersDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (((CustomRowFiltersDialog)sender).DialogResult == DialogResult.Cancel)
            {
                cancelCustomFilterDialog = true;
            }
        }

        // вспомогательная фукция по поиску значения по заголовку в списке значений (ValueList)
        private static ValueListItem FindValueListItem(ValueList vl, string displayText)
        {
            ValueListItem vli = null;
            foreach (ValueListItem item in vl.ValueListItems)
            {
                if (item.DisplayText == displayText)
                {
                    vli = item;
                    break;
                }
            }
            return vli;
        }
         

        // Обработчик выпадения списка значения для грида диалога настройки сложных фильтров.
        // По умолчанию в этот список добавляются все колонки грида что неприемлемо.
        private void OnCustomFilterDialogGrid_BeforeCellDropDownList(object sender, Infragistics.Win.UltraWinGrid.CancelableCellEventArgs e)
        {
            // определяем а есть-ли там вообще колонки
            if (null != e && null != e.Cell && "Operand".Equals(e.Cell.Column.Key))
            {
                #region Стремный метод определения наличия элементов для колонок, получен путем декомпиляции UltraWinGrid
                UltraGridCell operatorCell = e.Cell.Row.Cells["Operator"];
                object cellVal = operatorCell.Value;
                bool hasColumnsOperands = false;
                ValueList valueList = e.Cell.Column.ValueList as ValueList;
                if (null == valueList)
                    return;
                hasColumnsOperands = ((valueList != null) && (cellVal is FilterComparisionOperator));
                if (hasColumnsOperands)
                {
                    FilterComparisionOperator filterOperator = (FilterComparisionOperator)cellVal;
                    hasColumnsOperands =
                        (filterOperator == FilterComparisionOperator.Equals) ||
                        (filterOperator == FilterComparisionOperator.NotEquals) ||
                        (filterOperator == FilterComparisionOperator.LessThan) ||
                        (filterOperator == FilterComparisionOperator.LessThanOrEqualTo) ||
                        (filterOperator == FilterComparisionOperator.GreaterThan) ||
                        (filterOperator == FilterComparisionOperator.GreaterThanOrEqualTo);
                }
                #endregion
                if (hasColumnsOperands)
                {
                    // запомнисаем был-ли элемент выбора из справочника
                    bool hadbookItemPresent = FindValueListItem(valueList, UltraGridEx.HandbookSelectCaption) != null;
                    // очищаем все значения
                    valueList.ValueListItems.Clear();
                    // вставляем элемент "Пустые"
                    valueList.ValueListItems.Insert( 0, null, "(Пустые)" );
                    // позволяем пользователю добавить новые
                    if (_onGetServerFilterCustomDialogColumnsList != null)
                        _onGetServerFilterCustomDialogColumnsList(this, valueList, ugFilter.ActiveCell.Column.Key);
                    // если был элемент справочника - добавляем его обратно
                    if (hadbookItemPresent)
                        valueList.ValueListItems.Add(null, UltraGridEx.HandbookSelectCaption);
                }
            }

        }

        // обработчие нажатия на пункт вызова справочника в выпадающем списке колонки, 
        // вызывается из грида фильтра и из формы настройки сложных фильтров
        private void ugFilter_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            // если не определен обработчик для получения значений справочника - ничего не делаем
            if (_onGetHandbookValue == null)
                return;

            // если колонка не может содержать выбор из справочника - ничего не делаем
            bool allowed = e.Cell.IsFilterRowCell || e.Cell.Column.Key == "Operand";
            if (!allowed)
                return;
            if (e.Cell.Value == null)
                return;

            // если таки запрошен выбор из справочника
            if ((e.Cell.SelText == HandbookSelectCaption) || (e.Cell.Value.ToString() == HandbookSelectCaption))
            {
                UltraGrid grid = (UltraGrid)e.Cell.Column.Band.Layout.Grid;
                string columnName = e.Cell.Column.Key;
                // если вызов произведен из диалога настройки сложных фильтров - берем имя колонки из грида фильтра
                if (columnName == "Operand")
                    columnName = ugFilter.ActiveCell.Column.Key;
                // запрашиваем у клиента значение
                object value = null;
                if (!_onGetHandbookValue(this, columnName, ref value))
                    // нажали кнопку отмена
                    cancelCustomFilterDialog = true;
                else
                    // выбрали какое то значение
                    cancelCustomFilterDialog = false;
                //if ()
                if ((value != e.Cell.Value) && (value != null) && !cancelCustomFilterDialog)
                //if ((value != null) && (Convert.ToInt32(value) >= 0))
                    // если вернули - устанаваливем
                    e.Cell.Value = value;
                else
                {
                    // нет - восстанавливаем старое
                    e.Cell.Value = e.Cell.OriginalValue;
                    BurnRefreshDataButton(false);
                }
                grid.PerformAction(UltraGridAction.ExitEditMode);
            }
        }

        // добавление пункта "Выбор из справочника" в выпадающий списоко
        private void ugFilter_BeforeRowFilterDropDownPopulate(object sender, BeforeRowFilterDropDownPopulateEventArgs e)
        {
            if (_onBeforeRowFilterDropDownPopulateEventHandler != null)
                _onBeforeRowFilterDropDownPopulateEventHandler(sender, e);
        }

        // сбрасываем активную ячейку при переходе на другой элемент
        private void ugFilter_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            UltraGridCell cell = null;
            if (e.Element is CellUIElement)
            {
                cell = ((CellUIElement)e.Element).Cell;
            }
            else
            {
                object oContext = e.Element.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridCell));
                if (oContext != null)
                    cell = (UltraGridCell)oContext;
            }

            if ((cell != null) &&
                (ugFilter.ActiveCell != null) &&
                (cell != ugFilter.ActiveCell))
            {
                ugFilter.PerformAction(UltraGridAction.ExitEditMode);
                ugFilter.ActiveCell = null;
                ugFilter.Parent.Focus();
            }
        }

        // при смене фильтра "зажигаем" кнопку обновления
        private void ugFilter_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
        {
            //if (isCustomFilter)
            //    DateTimeCustomFilter.ShowDateTimeCustomFilter(e.Column.Header.Caption, ref sender, e.Column.Key);

            FilterConditionsCollection fcs = e.NewColumnFilter.FilterConditions;
            // для промежуточных значений фильтр не строим
            bool isTemporaryValue =
                (fcs.All.Length == 1) && ((fcs[0].CompareValue.ToString() == "(Сложный фильтр)") ||
                (fcs[0].CompareValue.ToString() == HandbookSelectCaption));
            if (isTemporaryValue)
                return;

            if (cancelCustomFilterDialog)
            {
                cancelCustomFilterDialog = !cancelCustomFilterDialog;
                return;
            }

            string newFilter = String.Empty;
            List<FilterParamInfo> prms = null;
            BuildServerFilter(out newFilter, out prms, false);
            int newFilterHash = CalcFilterHash(newFilter, prms);
            // если новый фильтр отличается от теекущего - подсвечиваем кнопку рефреша
            BurnRefreshDataButton(newFilterHash != _lastFilterHash);
        }

        /// <summary>
        /// устанавливает фильтр по колонке на равенство определенному значению
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void SetServerFilterValue(UltraGrid grid, string columnName, object value1, object value2)
        {
            UltraGridBand band = grid.DisplayLayout.Bands[0];
            if (band.Columns.Exists(columnName))
            {
                UltraGridColumn column = band.Columns[columnName];
                column.FilterOperatorDefaultValue = FilterOperatorDefaultValue.Equals;
                band.ColumnFilters[columnName].FilterConditions.Clear();
                band.ColumnFilters[columnName].FilterConditions.Add(FilterComparisionOperator.GreaterThanOrEqualTo, value1);
                band.ColumnFilters[columnName].FilterConditions.Add(FilterComparisionOperator.LessThanOrEqualTo, value2);
                band.ColumnFilters[columnName].LogicalOperator = FilterLogicalOperator.And;
                band.Layout.Grid.Refresh();
            }
        }

        private void ClearFilterParams(string columnName)
        {
            UltraGridBand band = ugFilter.DisplayLayout.Bands[0];
            if (band.Columns.Exists(columnName))
                band.ColumnFilters[columnName].FilterConditions.Clear();
        }

        #endregion

        public void CollapseGroup(string groupName, bool collapse)
        {
            if (!Groups.ContainsKey(groupName))
                return;
            string mainColumnName = GetGridColumnName(Groups[groupName]);
            UltraGridGroup group = ugData.DisplayLayout.Bands[0].Groups[groupName];
            if (collapse)
            {
                if (!string.IsNullOrEmpty(mainColumnName))
                {
                    foreach (UltraGridColumn column in group.Columns)
                    {
                        if (string.Compare(column.Key, mainColumnName, true) == 0)
                            continue;
                        column.Hidden = true;
                    }
                }
            }
            else
            {
                foreach (UltraGridColumn column in group.Columns)
                {
                    column.Hidden = false;
                }
            }

            if (ServerFilterEnabled)
                CollapseFilterGroup(groupName, collapse);
        }

        private void CollapseFilterGroup(string groupName, bool collapse)
        {
            string mainColumnName = GetGridColumnName(Groups[groupName]);
            mainColumnName = GetSourceColumnName(mainColumnName);
            if (string.IsNullOrEmpty(mainColumnName))
                return;
            UltraGridGroup group = ugFilter.DisplayLayout.Bands[0].Groups[groupName];
            if (collapse)
            {
                if (!string.IsNullOrEmpty(mainColumnName))
                {
                    foreach (UltraGridColumn column in group.Columns)
                    {
                        if (string.Compare(column.Key, mainColumnName, true) == 0)
                            continue;
                        column.Hidden = true;
                    }
                }
            }
            else
            {
                foreach (UltraGridColumn column in group.Columns)
                {
                    column.Hidden = false;
                }
            }
        }

    }
}
