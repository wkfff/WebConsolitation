using System;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.Common;
using Krista.FM.Common;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common.Controls;
using Krista.FM.Client.Common.Configuration;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {
        #region Внутренние вспомогательные объекты

        // класс объекта-поставщика данных
        protected ClassTypes clsClassType;
        // датасет с данными классификатора
        protected DataSet dsObjData;
        // объект просмотра 
        protected BaseClsView vo;
        // протокол для записи событий в классификаторе
        protected IClassifiersProtocol protocol;

        public const int developerRowDiapazon = 1000000000;

        public bool InAssociateMode
        {
            get; set;
        }

        public int MaxFactTableRecordCount
        {
            get; set;
        }

        //public const int MaxFactTableRecordCount = 5000;

        IWin32Window parentWindow;

        protected Infragistics.Win.UltraWinTabControl.UltraTabControl utcDetails
        {
            get { return vo.utcDetails; }
        }

        public string PresentationKey
        {
            get; set;
        }

        #endregion

        #region Внутренние вспомогательные события и свойства

        private AfterSourceSelect _AfterSourceSelect = null;

        public event AfterSourceSelect OnAfterSourceSelect
        {
            add { _AfterSourceSelect += value; }
            remove { _AfterSourceSelect -= value; }
        }

        private string _rusObjectName = string.Empty;
        /// <summary>
        /// русское наименование объкта просмотра
        /// </summary>
        internal string RusObjectName
        {
            get { return _rusObjectName; }
            set
            {
                _rusObjectName = value;
            }
        }

        public int CurrentRecordsCount
        {
            get; set;
        }

        public int AllRecordsCount
        {
            get; set;
        }

        #endregion

        #region Создание и инициализация объекта
        // конструктор класса
        public BaseClsUI(IEntity dataObject)
            : this(dataObject, dataObject.ObjectKey)
        {
        }

        public BaseClsUI(IEntity dataObject, string key)
            : base(key)
        {
            MaxFactTableRecordCount = 5000;
            Caption = "Базовый класс для классификаторов";
            dsObjData = new DataSet("dsObjData");
            dsObjData.RemotingFormat = SerializationFormat.Binary;
            activeDataObj = dataObject;
            SaveLastSelectedDataSource = true;
        }

        public override void InternalFinalize()
        {
            SaveDataThenExit();
            //            lookupManager.Dispose();
            protocol.Dispose();
            base.InternalFinalize();
        }

        // создание объекта просмотра
        protected override void SetViewCtrl()
        {
            fViewCtrl = new BaseClsView();
            fViewCtrl.ViewContent = this;

            ViewSettings viewSettings = new ViewSettings(fViewCtrl, String.Format("{0}.{1}", this.GetType().FullName, Key));
            viewSettings.Settings.Add(new UltraGridExSettingsPartial(String.Format("{0}.{1}", this.GetType().FullName, Key), ((BaseClsView)fViewCtrl).ugeCls.ugData));
        }

        private DataSourcesHelper dataSourcesHelper = null;
        public DataSourcesHelper DataSourcesHelper
        {
            get { return dataSourcesHelper; }
            private set { dataSourcesHelper = value; }
        }

        public ImageList clsImageList
        {
            get { return vo.ilImages; }
        }

        // инициализация и предварительная настройка интерфейса пользователя
        public virtual void InitializeUI()
        {
            vo = (BaseClsView)fViewCtrl;
            vo.ugeCls.Caption = FullCaption;

            // инициализация объекта просмотра
            // мэнэджер тулбаров грида с данными объекта
            vo.utbToolbarManager.AfterToolCloseup += new Infragistics.Win.UltraWinToolbars.ToolDropdownEventHandler(this.utbToolbarManager_AfterToolCloseup);
            vo.utbToolbarManager.BeforeToolDropdown += new BeforeToolDropdownEventHandler(utbToolbarManager_BeforeToolDropdown);
            vo.utbToolbarManager.ToolClick += new ToolClickEventHandler(utbToolbarManager_ToolClick);
            // грид с данными по ассоциациям объекта
            vo.ugAssociations.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(Associations_ButtonClick);

            // настройки для нового компонента
            vo.ugeCls.OnAfterRowInsert += new CC.AfterRowInsert(ugeCls_OnAfterRowInsert);
            vo.ugeCls.OnBeforeRowsDelete += new Krista.FM.Client.Components.BeforeRowsDelete(ugeCls_OnBeforeRowsDelete);
            vo.ugeCls.OnAfterRowsDelete += new Krista.FM.Client.Components.AfterRowsDelete(ugeCls_OnAfterRowsDelete);

            vo.ugeCls.OnGetHierarchyRelation += new CC.GetHierarchyRelation(this.GetHierarchyRelation);
            vo.ugeCls.OnGetHierarchyInfo += new CC.GetHierarchyInfo(this.GetHierarchyInfo);

            vo.ugeCls.OnMouseEnterGridElement += new CC.MouseEnterElement(ugeCls_OnMouseEnterGridElement);
            vo.ugeCls.OnMouseLeaveGridElement += new CC.MouseLeaveElement(ugeCls_OnMouseLeaveGridElement);
            vo.ugeCls.OnGetGridColumnsState += new CC.GetGridColumnsState(ugeCls_OnGetGridColumnsState);
            vo.ugeCls.OnClickCellButton += new CC.ClickCellButton(ugeCls_OnClickCellButton);
            vo.ugeCls.ugData.BeforeCellDeactivate += new System.ComponentModel.CancelEventHandler(ugData_BeforeCellDeactivate);
            vo.ugeCls.OnChangeHierarchyView += new Krista.FM.Client.Components.ChangeHierarchyView(ugeCls_OnChangeHierarchyView);

            vo.ugeCls.OnSaveChanges += new CC.SaveChanges(ugeCls_OnSaveChanges);
            vo.ugeCls.OnRefreshData += new CC.RefreshData(ugeCls_OnRefreshData);
            vo.ugeCls.OnCancelChanges += new CC.DataWorking(ugeCls_OnCancelChanges);
            vo.ugeCls.OnClearCurrentTable += new CC.DataWorking(ugeCls_OnClearCurrentTable);

            vo.ugeCls.GridDragDrop += new CC.MainMouseEvents(ugeCls_GridDragDrop);
            vo.ugeCls.GridDragEnter += new CC.MainMouseEvents(ugeCls_GridDragEnter);
            vo.ugeCls.GridDragLeave += new CC.DragLeave(ugeCls_GridDragLeave);
            vo.ugeCls.GridDragOver += new CC.MainMouseEvents(ugeCls_GridDragOver);
            vo.ugeCls.GridSelectionDrag += new CC.SelectionDrag(ugeCls_GridSelectionDrag);
            vo.ugeCls.OnInitializeRow += new Krista.FM.Client.Components.InitializeRow(ugeCls_OnInitializeRow);
            vo.ugeCls.OnAfterRowActivate += new Krista.FM.Client.Components.AfterRowActivate(ugeCls_OnAfterRowActivate);
            vo.ugeCls.OnBeforeRowDeactivate += new Krista.FM.Client.Components.BeforeRowDeactivate(ugeCls_OnBeforeRowDeactivate);
            vo.ugeCls.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ugeCls_OnGridInitializeLayout);
            vo.ugeCls.OnAftertImportFromXML += new CC.AftertImportFromXML(ugeCls_OnAftertImportFromXML);
            vo.ugeCls.OnSaveToXML += new CC.SaveLoadXML(ugeCls_OnSaveToXML);
            vo.ugeCls.OnSaveToExcel += new Krista.FM.Client.Components.SaveLoadXML(ugeCls_OnSaveToExcel);
            vo.ugeCls.OnLoadFromExcel += new Krista.FM.Client.Components.SaveLoadXML(ugeCls_OnLoadFromExcel);
            vo.ugeCls.OnLoadFromXML += new CC.SaveLoadXML(ugeCls_OnLoadFromXML);
            vo.ugeCls.OnDataSelect += new Krista.FM.Client.Components.DataSelect(ugeCls_OnDataSelect);
            vo.ugeCls.OnNeedLoadChildRows += new Krista.FM.Client.Components.NeedLoadChildRows(this.OnNeedLoadChildRows);
            vo.ugeCls.OnGridCellError += new Krista.FM.Client.Components.GridCellError(ugeCls_OnGridCellError);
            vo.ugeCls.ToolClick += new Krista.FM.Client.Components.ToolBarToolsClick(ugeCls_ToolClick);
            vo.ugeCls.OnBeforeRowFilterDropDownPopulateEventHandler += new BeforeRowFilterDropDownPopulateEventHandler(this.ugFilter_BeforeRowFilterDropDownPopulate);
            vo.ugeCls.OnGetHandbookValue += new CC.GetHandbookValue(this.ugeCls_OnGetHandbookValue);
            vo.ugeCls.OnGetServerFilterCustomDialogColumnsList += new CC.GetServerFilterCustomDialogColumnsList(this.GetServerFilterCustomDialogColumnsList);
            vo.ugeCls.StateRowEnable = true;
            vo.ugeCls.OnBeforeHierarchyChange += new HierarchyChange(ugeCls_OnHierarchyChange);
            vo.ugeCls.OnAfterHierarchyChange += new HierarchyChange(ugeCls_OnAfterHierarchyChange);

            vo.utcDataCls.ActiveTabChanged += new Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventHandler(utcDataCls_ActiveTabChanged);
            vo.ugeCls.ugData.MouseClick += new MouseEventHandler(ugData_MouseClick);

            vo.ugeCls.OnCreateUIElement += new Krista.FM.Client.Components.CreateUIElement(ugeCls_OnCreateUIElement);

            // лукапы
            vo.ugeCls.OnGetLookupValue += new CC.GetLookupValueDelegate(ugeCls_OnGetLookupValue);
            vo.ugeCls.OnCheckLookupValue += new CC.CheckLookupValueDelegate(ugeCls_OnCheckLookupValue);

            vo.cmsAudit.ItemClicked += new ToolStripItemClickedEventHandler(cmsAudit_ItemClicked);
            vo.cmsAuditSchemeObject.ItemClicked += new ToolStripItemClickedEventHandler(cmsAuditSchemeObject_ItemClicked);

            // протокол для записи событий
            protocol = (IClassifiersProtocol)this.Workplace.ActiveScheme.GetProtocol("Workplace.exe");

            vo.ugeCls.ugData.AfterCellUpdate += new CellEventHandler(ugData_AfterCellUpdate);

            vo.ugeCls._ugData.AfterColPosChanged += new AfterColPosChangedEventHandler(_ugData_AfterColPosChanged);

            _toolTipTimer = new Timer();
            _toolTipTimer.Interval = 500;
            _toolTipTimer.Tick += new EventHandler(toolTipTimer_Tick);

            vo.ugeCls.ugData.DisplayLayout.Override.WrapHeaderText = DefaultableBoolean.Default;

            // настройки для кнопок копировать/вставить
            vo.ugeCls.utmMain.Tools["CopyRow"].SharedProps.Visible = true;
            vo.ugeCls.utmMain.Tools["PasteRow"].SharedProps.Visible = true;

            vo.ugeCls.utmMain.Tools["SaveDataSetXML"].SharedProps.Visible = true;

            vo.ugeCls.OnCopyRow += new Krista.FM.Client.Components.RefreshData(ugeCls_OnCopyRow);
            vo.ugeCls.OnPasteRow += new Krista.FM.Client.Components.RefreshData(ugeCls_OnPasteRow);

            vo.ugeCls.OnSaveToDataSetXML += new Krista.FM.Client.Components.SaveLoadXML(ugeCls_OnSaveToDataSetXML);

            parentWindow = Workplace.WindowHandle;

            vo.utcDetails.SelectedTabChanged += new Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventHandler(utcDetails_SelectedTabChanged);
            vo.utcDetails.SelectedTabChanging += new Infragistics.Win.UltraWinTabControl.SelectedTabChangingEventHandler(utcDetails_SelectedTabChanging);

            vo.cmsCreateDocument.ItemClicked += new ToolStripItemClickedEventHandler(cmsCreateDocument_ItemClicked);
            vo.ugeCls.OnAfterColumnHideShow += new Krista.FM.Client.Components.ColumnHideShow(ugeCls_OnAfterColumnHideShow);

            // устанавливаем флаг того, что не находимся в режиме сопоставления
            InAssociateMode = false;
        }

        #region реализация копировать/вставить

        bool ugeCls_OnPasteRow(object sender)
        {
            if (copyTable != null)
            {
                Workplace.OperationObj.Text = "Обработка данных";
                Workplace.OperationObj.StartOperation();
                vo.ugeCls._ugData.BeginUpdate();
                dsObjData.Tables[0].BeginLoadData();
                copyTable.BeginLoadData();
                try
                {
                    PasteCopyRows();
                    return false;
                }
                finally
                {
                    copyTable.EndLoadData();
                    dsObjData.Tables[0].EndLoadData();
                    vo.ugeCls._ugData.EndUpdate();
                    Workplace.OperationObj.StopOperation();
                }
            }
            return true;
        }

        /// <summary>
        /// копирование записей классификатора
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        bool ugeCls_OnCopyRow(object sender)
        {
            UltraGrid grid = (UltraGrid)sender;
            bool hasChilds = false;
            List<UltraGridRow> rows = new List<UltraGridRow>();
            foreach (UltraGridRow selectedRow in grid.Selected.Rows)
            {
                rows.Add(selectedRow);
                hasChilds = hasChilds || selectedRow.HasChild();
            }
            if (rows.Count == 0)
            {
                if (grid.ActiveRow != null)
                {
                    rows.Add(grid.ActiveRow);
                    hasChilds = hasChilds || grid.ActiveRow.HasChild();
                }
                else
                    return true;
            }

            if (hasChilds && MessageBox.Show(parentWindow, "Копировать подчиненные записи?", "Копирование записей",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                copyTable = dsObjData.Tables[0].Clone();
                copyRowsLevels = new List<int>();
                grid.BeginUpdate();
                dsObjData.Tables[0].BeginLoadData();
                copyTable.BeginLoadData();
                try
                {
                    foreach (UltraGridRow row in rows)
                    {
                        // копируем все подчиненные записи вместе с родительской
                        CopyWithChildRows(row);
                    }
                }
                finally
                {
                    copyTable.EndLoadData();
                    dsObjData.Tables[0].EndLoadData();
                    grid.EndUpdate();
                }
                return false;
            }
            // копируем только одну запись средствами компонента
            return true;
        }

        // структура для хранения копируемых записей
        private DataTable copyTable;
        private List<int> copyRowsLevels;

        /// <summary>
        /// копирование записи и всех подчиненных в структуру для хранения
        /// </summary>
        /// <param name="row"></param>
        private void CopyWithChildRows(UltraGridRow row)
        {
            // копируем запись в DataTable, имеющему аналогичную структуру
            // получаем ID, потом запросом берем запись из текущего источника данных
            int rowID = Convert.ToInt32(UltraGridHelper.GetRowCells(row).Cells["ID"].Value);
            DataRow copyRow = dsObjData.Tables[0].Select(string.Format("ID = {0}", rowID))[0];
            // сохраняем запись во временной таблице
            copyTable.Rows.Add(copyRow.ItemArray);
            // сохраняем уровень иерархии записи
            copyRowsLevels.Add(row.Band.Index);

            // затем копируем все подчиненные записи родительской.
            if (row.ChildBands != null)
                foreach (UltraGridChildBand band in row.ChildBands)
                    foreach (UltraGridRow childRow in band.Rows)
                        CopyWithChildRows(childRow);
        }

        /// <summary>
        /// вставка записи вместе с подчиненными
        /// </summary>
        private void PasteCopyRows()
        {
            string refColumnName = vo.ugeCls.HierarchyInfo.ParentRefClmnName;
            int index = 0;
            // идем по всем скопированным записям и добавляем их через грид
            // вставляем запись на тот же уровень, что и текущая активная
            // если такой нет, тогда на верхний уровень
            int currentBandIndex = vo.ugeCls.ugData.ActiveRow != null ? vo.ugeCls.ugData.ActiveRow.Band.Index : 0;
            int bandIndexOffset = copyRowsLevels[index] - currentBandIndex;
            foreach (DataRow row in copyTable.Rows)
            {
                UltraGridRow addRow = vo.ugeCls._ugData.DisplayLayout.Bands[copyRowsLevels[index] - bandIndexOffset].AddNew();
                int newID = Convert.ToInt32(addRow.Cells["ID"].Value);
                int oldID = Convert.ToInt32(row["ID"]);
                // меняем ссылки со старым ID на новый
                ChangeParentReferences(refColumnName, newID, oldID, ref copyTable);

                row["ID"] = newID;
                if (copyTable.Columns.Contains("ParentID"))
                    row["ParentID"] = addRow.Cells["ParentID"].Value;
                // восстанавливаем значения записей
                foreach (DataColumn column in copyTable.Columns)
                {
                    if (column.ColumnName != "ID" && column.ColumnName != "ParentID")
                        addRow.Cells[column.ColumnName].Value = row[column.ColumnName];
                }
                addRow.Update();
                vo.ugeCls._ugData.ActiveRow = addRow;
                index++;
            }
        }

        /// <summary>
        /// изменение ссылок со старого ID на новый
        /// </summary>
        /// <param name="refColumnName"></param>
        /// <param name="newRefValue"></param>
        /// <param name="oldRefValue"></param>
        /// <param name="table"></param>
        private void ChangeParentReferences(string refColumnName, int newRefValue, int oldRefValue, ref DataTable table)
        {
            DataRow[] rows = table.Select(string.Format("{0} = {1}", refColumnName, oldRefValue));
            foreach (DataRow row in rows)
            {
                row[refColumnName] = newRefValue;
            }
        }

        #endregion


        void _ugData_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            if (e.PosChanged == PosChanged.Sized)
            {
                maxHeight[e.ColumnHeaders[0].Band.Index] = 0;
            }
        }

        #region Встроенные элементы

        // хранит высоту заголовков колонок для каждого банда
        int[] maxHeight;

        /// <summary>
        /// создание элементов интерфейсаа в колонках грида
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="parent"></param>
        protected virtual void ugeCls_OnCreateUIElement(object sender, UIElement parent)
        {
            if (parent is HeaderUIElement)
            {
                HeaderBase aHeader = ((HeaderUIElement)parent).Header;

                if (aHeader.Column == null)
                    return;

                string caption = aHeader.Column.Header.Caption;
                if (caption != string.Empty)
                {
                    CheckBoxUIElement cbWrapWordsUIElement = (CheckBoxUIElement)parent.GetDescendant(typeof(ButtonUIElement));
                    string columnKey = UltraGridEx.GetSourceColumnName(aHeader.Column.Key);

                    bool readonlyColumn = false;

                    if (currentStates.ContainsKey(UltraGridEx.GetSourceColumnName(columnKey)))
                        readonlyColumn = currentStates[UltraGridEx.GetSourceColumnName(columnKey)].IsSystem ||
                            currentStates[UltraGridEx.GetSourceColumnName(columnKey)].IsReadOnly;

                    // для строковых колонок создаем чекбокс для изменения высоты строк
                    if (aHeader.Column.DataType == typeof(String))
                    {
                        if (cbWrapWordsUIElement == null)
                        {
                            cbWrapWordsUIElement = new CheckBoxUIElement(parent);
                        }

                        Infragistics.Win.UltraWinGrid.ColumnHeader aColumnHeader =
                            (Infragistics.Win.UltraWinGrid.ColumnHeader)cbWrapWordsUIElement.GetAncestor(typeof(HeaderUIElement))
                            .GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

                        if (aColumnHeader.Tag == null)
                            aColumnHeader.Tag = aHeader.Column.CellMultiLine == DefaultableBoolean.True
                                                    ? CheckState.Checked
                                                    : CheckState.Unchecked;
                        else
                            cbWrapWordsUIElement.CheckState = (CheckState)aColumnHeader.Tag;

                        parent.ChildElements.Add(cbWrapWordsUIElement);

                        cbWrapWordsUIElement.ElementClick += new UIElementEventHandler(aButtonUIElement_ElementClick);

                        cbWrapWordsUIElement.Rect = new Rectangle(parent.Rect.X + 3, parent.Rect.Y + ((parent.Rect.Height - cbWrapWordsUIElement.CheckSize.Height) / 2), cbWrapWordsUIElement.CheckSize.Width, cbWrapWordsUIElement.CheckSize.Height);
                    }

                    ImageAndTextButtonUIElement btnSetChildlValueUIElement = null;
                    Infragistics.Win.Appearance mainAppearance = null;

                    // для иерархических гридов создаем кнопочку для установки значений как у родителя
                    if (!vo.ugeCls.IsReadOnly && vo.ugeCls.AllowAddNewRecords)
                        if (maxHeight.Length > 1 && !InInplaceMode && !readonlyColumn)
                        {
                            btnSetChildlValueUIElement = (ImageAndTextButtonUIElement)parent.GetDescendant(typeof(ImageAndTextButtonUIElement));

                            if (btnSetChildlValueUIElement == null)
                                //Create a New ButtonUIElement
                                btnSetChildlValueUIElement = new ImageAndTextButtonUIElement(parent);

                            parent.ChildElements.Add(btnSetChildlValueUIElement);
                            btnSetChildlValueUIElement.ElementClick += btnSetChildlValueUIElement_ElementClick;
                            mainAppearance = new Infragistics.Win.Appearance();
                            btnSetChildlValueUIElement.Appearance = mainAppearance;
                        }

                    TextUIElement aTextUIElement = (TextUIElement)parent.GetDescendant(typeof(TextUIElement));

                    // Sanity check
                    if (aTextUIElement == null)
                        return;

                    FilterDropDownButtonUIElement aFilterUIElement = (FilterDropDownButtonUIElement)parent.GetDescendant(typeof(FilterDropDownButtonUIElement));

                    SortIndicatorUIElement aSortIndicatorElement = (SortIndicatorUIElement)parent.GetDescendant(typeof(SortIndicatorUIElement));

                    // опытным путем подобранные значения для уменьшения текста заголовка 
                    // для того, чтобы другие элементы были видны в этом заголовке
                    int textLengh = 0;
                    if (aSortIndicatorElement != null && cbWrapWordsUIElement != null)
                        textLengh = 56;
                    else if (aSortIndicatorElement == null && cbWrapWordsUIElement != null)
                        textLengh = 40;
                    else if (aSortIndicatorElement == null && cbWrapWordsUIElement == null)
                        textLengh = 40;
                    else if (aSortIndicatorElement != null && cbWrapWordsUIElement == null)
                        textLengh = 32;
                    if (btnSetChildlValueUIElement == null)
                        textLengh = textLengh - 16;

                    if (cbWrapWordsUIElement != null)
                        aTextUIElement.Rect = new Rectangle(cbWrapWordsUIElement.Rect.Right + 3, aTextUIElement.Rect.Y, parent.Rect.Width - (cbWrapWordsUIElement.Rect.Right - parent.Rect.X) - textLengh, aTextUIElement.Rect.Height);
                    else
                        aTextUIElement.Rect = new Rectangle(parent.Rect.X + 3, aTextUIElement.Rect.Y, aTextUIElement.Rect.Width - textLengh, aTextUIElement.Rect.Height);

                    #region херня по нормализации надписей в заголовках колонок грида (вынести в отдельный метод)
                    // получение количества строк заголовка по текущей ширине заголовка поля
                    int fullHeight = vo.ugeCls.GetStringHeight(caption, vo.ugeCls._ugData.Font, aTextUIElement.Rect.Width) + 10;
                    float fontHeight = vo.ugeCls._ugData.Font.GetHeight();
                    int linesCount = (int)(fullHeight / fontHeight);

                    UltraGridBand band = aHeader.Column.Band;
                    int bandIndex = band.Index;
                    // установка высоты заголовка всех полей исходя из количества строк заголовка текущего поля
                    if (maxHeight[bandIndex] < linesCount)
                    {
                        // если текущая высота всех полей меньше текущего поля
                        // то ее ставим как высоту всего заголовка
                        maxHeight[bandIndex] = linesCount;
                        // максимальное значение строк в заголовке не может превышать 10
                        if (maxHeight[bandIndex] > 10)
                            maxHeight[bandIndex] = 10;

                        if (band.ColHeaderLines != maxHeight[bandIndex])
                        {
                            band.Layout.Grid.BeginUpdate();
                            // устанавливаем высоту
                            band.ColHeaderLines = maxHeight[bandIndex];
                            // информируем грид о том, что установили высоту
                            band.NotifyPropChange(Infragistics.Win.UltraWinGrid.PropertyIds.ColHeaderLines);
                            band.Layout.Grid.EndUpdate();
                        }
                    }
                    // выставляем минимальную ширину поля для того, что бы значение строк в заголовке поля не превышало 10
                    int minWidth = GetMinimalColWidth(caption, aHeader.Column.Width, aTextUIElement.Rect.Width, vo.ugeCls._ugData.Font);
                    if (aHeader.Column.MinWidth != minWidth)
                        aHeader.Column.MinWidth = minWidth;

                    aTextUIElement.WrapText = true;
                    #endregion

                    if (aSortIndicatorElement != null)
                        aSortIndicatorElement.Rect = new Rectangle(aTextUIElement.Rect.Right + 3, aSortIndicatorElement.Rect.Y, 13, aSortIndicatorElement.Rect.Height);

                    if (btnSetChildlValueUIElement != null)
                    {
                        if (aSortIndicatorElement != null)
                            btnSetChildlValueUIElement.Rect = new Rectangle(aSortIndicatorElement.Rect.Right + 3, aTextUIElement.Rect.Y + ((parent.Rect.Height - 16) / 2), 16, 16);
                        else
                            btnSetChildlValueUIElement.Rect = new Rectangle(aTextUIElement.Rect.Right + 3, aTextUIElement.Rect.Y + ((parent.Rect.Height - 16) / 2), 16, 16);
                        btnSetChildlValueUIElement.Image = Properties.Resources.SetHyerarchy_header;
                    }

                    if (aFilterUIElement != null)
                        if (btnSetChildlValueUIElement != null)
                            aFilterUIElement.Rect = new Rectangle(btnSetChildlValueUIElement.Rect.Right + 3, aFilterUIElement.Rect.Y, 13, aFilterUIElement.Rect.Height);
                }
            }
        }

        /// <summary>
        /// получение минимальной ширины колонки исходя из расчета 10 строк заголовка
        /// </summary>
        /// <returns></returns>
        private int GetMinimalColWidth(string columnCaption, int columnWidth, int captionWidth, Font font)
        {
            int lineSymbolsCount = columnCaption.Length / 10;
            if ((columnCaption.Length % 10) != 0)
                lineSymbolsCount++;
            string tmpStr = string.Empty.PadLeft(lineSymbolsCount, 'm');
            int lineWidth = vo.ugeCls.GetStringWidth(tmpStr, font);
            return columnWidth - captionWidth + lineWidth;
        }

        private bool AllowEditCurrentRow(UltraGridRow row)
        {
            if (!vo.ugeCls.AllowEditRows)
                return false;

            if (!Workplace.IsDeveloperMode && Convert.ToInt32(row.Cells["ID"].Value) >= developerRowDiapazon)
                return false;

            return true;
        }

        private bool AllowEditCurrentRow(DataRow row)
        {
            if (!vo.ugeCls.AllowEditRows)
                return false;

            if (!Workplace.IsDeveloperMode && Convert.ToInt32(row["ID"]) >= developerRowDiapazon)
                return false;

            return true;
        }

        void btnSetChildlValueUIElement_ElementClick(object sender, UIElementEventArgs e)
        {
            if (vo.ugeCls.ugData.Selected.Rows.Count == 0)
            {
                MessageBox.Show(parentWindow, "Необходимо выделить нужные записи.", "Установка значений", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!vo.ugeCls.AllowEditRows)
                return;

            ImageAndTextButtonUIElement btn = (ImageAndTextButtonUIElement)e.Element;
            Infragistics.Win.UltraWinGrid.ColumnHeader aColumnHeader = (Infragistics.Win.UltraWinGrid.ColumnHeader)btn.GetAncestor(typeof(HeaderUIElement)).GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));
            string columnName = CC.UltraGridEx.GetSourceColumnName(aColumnHeader.Column.Key);
            bool setAllChildValues = false;
            if (frmSetParentValueToChild.GetSetValuesParam(ref setAllChildValues, aColumnHeader.Column.Header.Caption))
            {
                // что то делаем
                this.Workplace.OperationObj.Text = "Обработка данных";
                this.Workplace.OperationObj.StartOperation();
                try
                {
                    foreach (UltraGridRow row in vo.ugeCls.ugData.Selected.Rows)
                    {
                        object value = row.Cells[columnName].Value;
                        if (row.ChildBands != null)
                            foreach (UltraGridChildBand childBand in row.ChildBands)
                            {
                                SetChildValue(childBand.Rows, value, setAllChildValues, columnName);
                            }
                        row.Expanded = true;
                    }
                }
                finally
                {
                    this.Workplace.OperationObj.StopOperation();
                }
            }
        }

        private void SetChildValue(RowsCollection rows, object value, bool setAllChildValues, string columnName)
        {
            foreach (UltraGridRow row in rows)
            {
                // ничего не делаем для записей, которые помечены как зарезервированные и их потомков
                if (AllowEditCurrentRow(row))
                {
                    if (setAllChildValues)
                        row.Cells[columnName].Value = value;
                    else
                        if (row.Cells[columnName].Value == null || row.Cells[columnName].Value == DBNull.Value)
                            row.Cells[columnName].Value = value;
                    row.Update();
                    if (row.ChildBands != null)
                        foreach (UltraGridChildBand childBand in row.ChildBands)
                        {
                            if (childBand.Rows != null)
                                SetChildValue(childBand.Rows, value, setAllChildValues, columnName);
                        }
                }
            }
        }

        /// <summary>
        /// выставляем параметры для колонки на перенос слов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void aButtonUIElement_ElementClick(object sender, UIElementEventArgs e)
        {

            CheckBoxUIElement checkBox = (CheckBoxUIElement)e.Element;

            HeaderBase header = ((HeaderUIElement)checkBox.Parent).Header;

            Infragistics.Win.UltraWinGrid.ColumnHeader aColumnHeader = (Infragistics.Win.UltraWinGrid.ColumnHeader)checkBox.GetAncestor(typeof(HeaderUIElement)).GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

            aColumnHeader.Tag = checkBox.CheckState;

            UltraGridColumn column = header.Column;

            // ставим свойство на перенос слов на следующую строку
            if (checkBox.CheckState == CheckState.Checked)
            {
                column.CellMultiLine = DefaultableBoolean.True;
            }
            else
            {
                column.CellMultiLine = DefaultableBoolean.False;
            }
            int columnWidth = column.Width;
            column.PerformAutoResize(PerformAutoSizeType.None);
            column.Width = columnWidth;
        }
        #endregion

        /// <summary>
        /// удаление лишних символов при редактировании строковых полей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            UltraGridCell cell = e.Cell;
            if (e.Cell.Column.DataType == typeof(String))
            {
                string columnName = UltraGridEx.GetSourceColumnName(cell.Column.Key);
                string value = e.Cell.Value.ToString();
                vo.ugeCls.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                try
                {
                    if (currentStates.ContainsKey(columnName.ToUpper()))
                    {
                        if (currentStates[columnName].isTextColumn)
                            e.Cell.Value = StringHelper.GetNormalizeTextString(value);
                        else
                            e.Cell.Value = StringHelper.GetNormalizeString(value);
                    }
                }
                finally
                {
                    vo.ugeCls.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                }
            }
        }

        /// <summary>
        /// Возвращает текущeую запись мастер-грида.
        /// </summary>
        protected UltraGridRow GetActiveMasterGridRow()
        {
            return UltraGridHelper.GetRowCells(vo.ugeCls._ugData.ActiveRow);
        }

		/// <summary>
		/// Возвращает ID текущей записи мастер-таблицы.
		/// </summary>
		protected int GetActiveMasterRowID()
		{
			return UltraGridHelper.GetRowID(GetActiveMasterGridRow());
		}

        protected virtual void ugeCls_OnAfterRowActivate(object sender, EventArgs e)
        {
            UltraGridRow tmpRow = GetActiveMasterGridRow();
            if (tmpRow.Cells["ID"].Value == DBNull.Value || tmpRow.Cells["ID"].Value == null)
            {
                if (isMasterDetail && !InInplaceMode)
                    LoadDetailData(-1);
                return;
            }
            if (allowDelRecords)
            {
                if (CheckDeveloperRow(vo.ugeCls.ugData.ActiveRow) && !Workplace.IsDeveloperMode)
                {
                    if (vo.ugeCls.AllowEditRows)
                        vo.ugeCls.AllowEditRows = false;
                    if (vo.ugeCls.AllowDeleteRows)
                        vo.ugeCls.AllowDeleteRows = false;
                }
                else
                {
                    if (!vo.ugeCls.AllowDeleteRows)
                        vo.ugeCls.AllowEditRows = true;
                    if (!vo.ugeCls.AllowDeleteRows)
                        vo.ugeCls.AllowDeleteRows = true;
                }
            }
            else
                vo.ugeCls.AllowDeleteRows = false;

            // в случае мастер-детали загружаем или фильтруем данные в детали
            if (isMasterDetail && !InInplaceMode)
            {
                DataRow[] rows = dsObjData.Tables[0].Select(string.Format("ID = {0}", GetActiveMasterRowID()));
                if (rows.Length > 0)
                {
                    if (rows[0].RowState == DataRowState.Added || rows[0].RowState == DataRowState.Deleted)
                        LoadDetailData(-1);
                    else
                        LoadDetailData(GetActiveMasterRowID());
                }
                else
                    // для удаляемых записей тоже будем запрашивать информацию в детали
                    LoadDetailData(GetActiveMasterRowID());
            }
        }


        void ugeCls_OnBeforeRowDeactivate(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isMasterDetail && dsDetail != null && dsDetail.Tables.Count > 0)
            {
                DataTable changes = dsDetail.Tables[0].GetChanges();
                if (changes != null)
                    if (MessageBox.Show(parentWindow, "Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        ugeDetail_OnSaveChanges(activeDetailGrid);
            }
        }


        public virtual void ugeCls_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow tmpRow = CC.UltraGridHelper.GetRowCells(e.Row);

            SetDocumentRow(tmpRow, vo.ugeCls.CurrentStates);

            // в таблицах фактов нет работы с фиксироваными записями
            if (clsClassType == ClassTypes.clsFactData)
                return;
            if (clsClassType == ClassTypes.clsFixedClassifier)
                return;

            if (tmpRow.Cells["ID"].Value == DBNull.Value || tmpRow.Cells["ID"].Value == null)
                return;


            if (Workplace.IsDeveloperMode)
            {
                UltraGridCell lockCellButton = e.Row.Cells["RowReserved_Image"];
                if (CheckDeveloperRow(e.Row))
                {
                    lockCellButton.Appearance.Image = vo.ilImages.Images[16];
                    lockCellButton.ToolTipText = "Запись зарезервирована";
                }
                else
                {
                    lockCellButton.Appearance.ResetImage();
                    lockCellButton.ToolTipText = string.Empty;
                }
            }
            else
            {
                if (CheckDeveloperRow(e.Row))
                {
                    foreach (UltraGridCell cell in e.Row.Cells)
                        cell.Appearance.BackColor = Color.Gainsboro;
                }
                else
                {
                    foreach (UltraGridCell cell in e.Row.Cells)
                        cell.Appearance.ResetBackColor();
                }
            }
        }

        protected bool CheckDeveloperRow(UltraGridRow row)
        {
            int rowID = UltraGridHelper.GetRowID(row);
            return rowID >= developerRowDiapazon;
        }

        public virtual void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // запрещаем тултипы везде
            //e.Layout.Override.TipStyleCell = TipStyle.Hide;
            // для вариантных классификаторов добалвяем колонку с кнопкой закрытия классификатора
            // в таблицах фактов нет фиксированных записей
            if (ActiveDataObj.ClassType == ClassTypes.clsFixedClassifier)
                return;
            // для таблиц фактов нету защищенных записей
            if (ActiveDataObj.ClassType != ClassTypes.clsFactData)
                if (Workplace.IsDeveloperMode)
                {
                    UltraGridColumn btnColumn = null;
                    foreach (UltraGridBand band in e.Layout.Bands)
                    {
                        if (!band.Columns.Exists("RowReserved_Image"))
                            btnColumn = band.Columns.Add("RowReserved_Image");
                        else
                            btnColumn = band.Columns["RowReserved_Image"];
                        btnColumn.Header.Caption = string.Empty;
                        btnColumn.Header.VisiblePosition = 1;
                        UltraGridHelper.SetLikelyImageColumnsStyle(btnColumn, -1);
                    }
                }
            // для полей типа BLOB создаем по паре дополнительных полей-кнопок для работы с документами
            AddDocumentsTypeListToGrid((UltraGrid)sender);
            AddDocumentButtons(e.Layout, vo.ugeCls.CurrentStates);
        }

        void utbToolbarManager_ToolClick(object sender, ToolClickEventArgs e)
        {

        }

        void ugeCls_OnHierarchyChange(object sender)
        {
            vo.SavePersistence();
        }

        void ugeCls_OnAfterHierarchyChange(object sender)
        {
            LoadLoadPersistence();
        }

        void ugeCls_OnGridCellError(object sender, CellDataErrorEventArgs e)
        {
            if (showErrorMessage)
                MessageBox.Show(parentWindow, "Введенные данные не соответствуют маске.", "Ввод данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public override string FullCaption
        {
            get
            {
                return String.Format("{0}.{1}", ActiveDataObj.SemanticCaption, ActiveDataObj.Caption);
            }
        }

        #region удаление отдельных записей

        List<int> childRowsID = new List<int>();
        List<int> deletedRowsID = new List<int>();
        bool isHierarchyView;
        bool deletingHierarchyRows = false;

        protected bool IsChanges()
        {
            DataTable dtChanges = dsObjData.Tables[0].GetChanges();
            if (dtChanges == null)
                return false;
            return true;
        }

        /// <summary>
        /// действия, выполняемые после удаления записей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ugeCls_OnAfterRowsDelete(object sender, BeforeRowsDeletedEventArgs e)
        {

            foreach (int i in childRowsID)
            {
                vo.ugeCls.SetRowToStateByID(i, vo.ugeCls.ugData.Rows, UltraGridEx.LocalRowState.Modified);
            }
        }

        /// <summary>
        /// Проверяем записи перед удалением на ссылки на эти записи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ugeCls_OnBeforeRowsDelete(object sender, BeforeRowsDeletedEventArgs e)
        {
            BeforeRowsDeleteOperations(sender, e);
        }

        protected virtual void BeforeRowsDeleteOperations(object sender, BeforeRowsDeletedEventArgs e)
        {
            // нельзя удалять записи, выходим
            if (!allowDelRecords) e.Cancel = true;
            // очистим списки удаляемых записей
            childRowsID.Clear();
            deletedRowsID.Clear();
            UltraGridEx gridEx = sender as UltraGridEx;
            if (gridEx == null)
                return;

            HierarchyInfo hi = gridEx.HierarchyInfo;
            if (hi.loadMode == LoadMode.OnParentExpand)
            {
                // для классификаторов, которые грузятся по уровням иерархии, раскроем выделенные записи
                gridEx.ugData.BeginUpdate();
                foreach (UltraGridRow expandedRow in e.Rows)
                {
                    expandedRow.Expanded = true;
                }
                gridEx.ugData.EndUpdate();
            }

            // ищем в классификаторе хоть какие то подчиненные записи
            bool hasChild = hi.ParentRefClmnName != string.Empty;
            if (hasChild)
            {
                // если классификатор иерархический, проверим, есть ли у удаляемых записей подчиненные
                hasChild = false;
                isHierarchyView = vo.ugeCls.ugData.DisplayLayout.MaxBandDepth > 1;
                for (int i = 0; i <= e.Rows.Length - 1; i++)
                {
                    if (isHierarchyView)
                    {
                        // в иерархическом виде
                        if (e.Rows[i].HasChild())
                            hasChild = true;
                    }
                    else
                    {
                        // в плоском виде
                        if (UltraGridHelper.FindGridRow(gridEx.ugData, hi.ParentRefClmnName, e.Rows[i].Cells["ID"].Value.ToString()) != null)
                            hasChild = true;
                    }
                }
            }
            // если есть, то предлагаем выбор или все удалять или очищать ссылки на удаляемые записи
            if (hasChild)
                if (!deletingHierarchyRows)
                {
                    bool deleteChildRows = false;
                    if (frmDeleteRows.DeleteRowsMode(ref deleteChildRows, parentWindow))
                    {
                        if (deleteChildRows)
                            DeleteHierarchyRow(false, e.Rows);
                        else
                            DeleteHierarchyRow(true, e.Rows);
                    }
                    else
                        e.Cancel = true;
                }
        }


        /// <summary>
        /// В зависимости от выбраного действия при удалении или удаляем все записи или очищаем ссылки
        /// </summary>
        /// <param name="onlyCurrentRow"></param>
        private void DeleteHierarchyRow(bool onlyCurrentRow, UltraGridRow[] rows)
        {
            // удаляем только выбранные записи, ссылки чистим
            HierarchyInfo hi = vo.ugeCls.HierarchyInfo;// GetHierarchyInfo(vo.ugeCls.ugData);
            string refParentColumnName = hi.ParentRefClmnName;
            if (onlyCurrentRow)
            {
                foreach (UltraGridRow row in rows)
                    ClearChildRowsRef(row, refParentColumnName);

                foreach (int id in childRowsID)
                {
                    DataRow row = dsObjData.Tables[0].Select(String.Format("ID = {0}", id))[0];
                    row[refParentColumnName] = DBNull.Value;
                }
            }
            else
            {
                Workplace.OperationObj.Text = "Обработка данных";
                Workplace.OperationObj.StartOperation();
                vo.ugeCls.ugData.BeginUpdate();
                dsObjData.Tables[0].BeginLoadData();
                try
                {
                    foreach (UltraGridRow row in rows)
                    {
                        deletingHierarchyRows = true;
                        // собираем список всех подчиненных записей на удаление
                        DeleteChildRows(row, refParentColumnName);
                        // удаляем все записи
                        deletingHierarchyRows = false;
                    }
                    foreach (int id in deletedRowsID)
                    {
                        DataRow[] dataRows = dsObjData.Tables[0].Select(String.Format("ID = {0}", id));
                        if (dataRows.Length > 0)
                            dataRows[0].Delete();
                    }
                }
                finally
                {
                    dsObjData.Tables[0].EndLoadData();
                    vo.ugeCls.ugData.EndUpdate();
                    Workplace.OperationObj.StopOperation();
                }
            }
        }

        /// <summary>
        /// Убираем ссылки на удаляемую запись
        /// </summary>
        /// <param name="onlyCurrentRow"></param>
        void ClearChildRowsRef(UltraGridRow parentRow, string parentRefClmnName)
        {
            // в зависимости от текущего вида грида по разному ищем ID непосредственных потомков записи
            if (isHierarchyView)
            {
                foreach (UltraGridRow row in parentRow.ChildBands[0].Rows)
                {
                    childRowsID.Add(Convert.ToInt32(row.Cells["ID"].Value));
                }
            }
            else
            {
                List<UltraGridRow> findRows = CC.UltraGridHelper.FindRowsFromFlatUltraGrid(vo.ugeCls.ugData, parentRefClmnName, parentRow.Cells["ID"].Value.ToString());
                foreach (UltraGridRow row in findRows)
                {
                    childRowsID.Add(Convert.ToInt32(row.Cells["ID"].Value));
                }
            }
        }


        /// <summary>
        /// Удаляем все записи со ссылками на удаляемую запись 
        /// </summary>
        /// <param name="parentRow"></param>
        void DeleteChildRows(UltraGridRow parentRow, string parentRefClmnName)
        {
            if (isHierarchyView)
            {
                if (parentRow.HasChild())
                {
                    foreach (UltraGridRow row in parentRow.ChildBands[0].Rows)
                    {
                        vo.ugeCls.SetRowAppearance(row, Krista.FM.Client.Components.UltraGridEx.LocalRowState.Deleted);
                        deletedRowsID.Add(Convert.ToInt32(row.Cells["ID"].Value));
                        if (row.HasChild())
                            DeleteChildRows(row, parentRefClmnName);
                    }
                }
            }
            else
            {
                List<UltraGridRow> findRows = CC.UltraGridHelper.FindFlatGridRows(vo.ugeCls.ugData,
                    parentRefClmnName, parentRow.Cells[parentRefClmnName].Column.DataType,
                    parentRow.Cells["ID"].Value);
                if (findRows != null)
                    foreach (UltraGridRow row in findRows)
                    {
                        vo.ugeCls.SetRowAppearance(row, Krista.FM.Client.Components.UltraGridEx.LocalRowState.Deleted);
                        deletedRowsID.Add(Convert.ToInt32(row.Cells["ID"].Value));
                        DeleteChildRows(row, parentRefClmnName);
                    }
            }
        }

        #endregion


        /// <summary>
        /// выбор даты в календаре, расчет числа, получаемого из этой даты и запись в таблицу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugeCls_OnDataSelect(object sender, DateRangeEventArgs e)
        {
            int initDateValue = 0;
            initDateValue += Convert.ToInt32(e.Start.Day.ToString());
            initDateValue += Convert.ToInt32(e.Start.Month.ToString()) * 100;
            initDateValue += Convert.ToInt32(e.Start.Year.ToString()) * 10000;
            vo.ugeCls.ugData.ActiveCell.Value = initDateValue;
            vo.ugeCls.ugData.ActiveRow.Update();
        }

        private bool showErrorMessage = true;

        public override void Activate(bool Activated)
        {
            if (Activated)
            {
                if (!InInplaceMode)
                    SetViewObjectCaption();
                if (CurrentDataSourceID != DataSourceContext.CurrentDataSourceID)
                    TrySetDataSource(DataSourceContext.CurrentDataSourceID);

                AttachTaskID();
                AssociationPageLoad(vo.utcDataCls.ActiveTab);
            }
            else
            {
                try
                {
                    showErrorMessage = false;
                    if (vo.ugeCls.ugData.ActiveRow != null)
                        vo.ugeCls.ugData.ActiveRow.Update();
                }
                catch
                {
                    //showErrorMessage = false;
                }
                SaveDataThenExit();
                showErrorMessage = true;
            }
            vo.ugeCls.BurnChangesDataButtons(false);
        }

        private void SaveDataThenExit()
        {
            if (ActiveDataObj != null)
                SaveAllDocuments(dsObjData, vo.ugeCls.CurrentStates, ActiveDataObj.FullDBName);
            if (activeDetailObject != null)
                SaveAllDocuments(dsDetail, activeDetailGrid.CurrentStates, activeDetailObject.FullDBName);

            // попытаемся сохранить детали
            TrySaveCurrentDetail();

            // потом уже мастер
            if (dsObjData.GetChanges() != null)
            {
                if (MessageBox.Show(parentWindow, "Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (SaveData(null, null))
                    {
                        vo.ugeCls.ClearAllRowsImages();
                        vo.ugeCls.BurnChangesDataButtons(false);
                    }
                }
                else
                {
                    dsObjData.RejectChanges();
                    vo.ugeCls.ClearAllRowsImages();
                    vo.ugeCls.BurnChangesDataButtons(false);
                    CanDeactivate = true;
                }
            }
        }


        void ugData_BeforeCellDeactivate(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // если данные не изменены - ничего проверять не надо
            CancelableCellEventArgs ce = (CancelableCellEventArgs)e;
            if (!ce.Cell.DataChanged)
                return;
            // изменить после реализации лукапов
            //e.Cancel = !CheckReferenceCellValue(referenceKind.reference);
            //e.Cancel = !CheckReferenceCellValue(referenceKind.lookup);
#warning Вставить проверку значений лукапов
        }

        /// <summary>
        /// очищаем текущий классификатор
        /// </summary>
        protected virtual void ugeCls_OnClearCurrentTable(object sender)
        {
            if (MessageBox.Show(parentWindow, "Удалить все данные текущей таблицы?", "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Workplace.OperationObj.Text = "Удаление данных текущей таблицы";
                Workplace.OperationObj.StartOperation();
                string message = string.Empty;
                try
                {
                    string deleteFilter = string.Empty;
                    int deletedRowsCount = 0;
                    if (clsClassType != ClassTypes.clsFactData)
                    {
                        deleteFilter = String.Concat("(RowType = 0) and ", GetDataSourcesFilter());
                        if (!Workplace.IsDeveloperMode)
                            deleteFilter = deleteFilter + String.Format(" and ID < {0}", developerRowDiapazon);
                        message = "Операция очистки данных классификатора.";
                    }
                    else
                    {
                        deleteFilter = dataQuery;
                        message = "Операция очистки данных таблицы фактов.";
                    }
                    // добавляем фильтр на id больше -1
                    deleteFilter = string.IsNullOrEmpty(deleteFilter) ? "(id > -1)" : string.Concat(deleteFilter, " and (id > -1)");

                    deletedRowsCount = string.IsNullOrEmpty(deleteFilter) ?
                        ActiveDataObj.DeleteData(string.Empty, true, null) :
                        ActiveDataObj.DeleteData(String.Format("where {0}", deleteFilter), true, ParametersListToValueArray(GetServerFilterParameters()));

                    dsObjData.Tables[0].BeginLoadData();
                    if (!Workplace.IsDeveloperMode)
                    {
                        foreach (DataRow row in dsObjData.Tables[0].Rows)
                        {
                            if (Convert.ToInt32(row["ID"]) < developerRowDiapazon)
                            {
                                if (row.RowState != DataRowState.Added)
                                    row.Delete();
                                else
                                {
                                    row.AcceptChanges();
                                    row.Delete();
                                }
                            }
                        }
                    }
                    else
                        dsObjData.Tables[0].Clear();
                    dsObjData.Tables[0].AcceptChanges();
                    dsObjData.Tables[0].EndLoadData();

                    protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
                        activeDataObj.OlapName, -1, this.CurrentDataSourceID, (int)this.clsClassType, string.Format("{0} Удалено записей: {1}", message, deletedRowsCount));

                    if (isMasterDetail && !InInplaceMode)
                    {
                        LoadDetailData(GetActiveMasterRowID());
                    }
                }
                catch (Exception exception)
                {
                    protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceError,
                        FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, "Очистка данных закончилась с ошибками");

                    if (exception.Message.Contains("ORA-02292") || exception.Message.Contains("Конфликт инструкции DELETE с ограничением"))
                    {
                        MessageBox.Show(parentWindow, "Нарушено ограничение целостности. Обнаружена порожденная запись", "Удаление данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (exception.Message.Contains("ORA-20101"))
                    {
                        MessageBox.Show(parentWindow, "Вариант закрыт от изменений. Запись и изменение данных варианта запрещены", "Удаление данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                        throw;
                }
                finally
                {
                    this.CanDeactivate = true;
                    this.Workplace.OperationObj.StopOperation();
                }
            }
        }

        private object[] ParametersListToValueArray(List<Krista.FM.Client.Components.UltraGridEx.FilterParamInfo> parameters)
        {
            if (parameters == null)
                return null;
            object[] values = new object[parameters.Count];
            int counter = 0;
            foreach (Krista.FM.Client.Components.UltraGridEx.FilterParamInfo paramInfo in parameters)
            {
                values[counter] = paramInfo.ParamValue;
                counter++;
            }
            return values;
        }

        /// <summary>
        /// Отмена изменений
        /// </summary>
        void ugeCls_OnCancelChanges(object sender)
        {
            CancelData(ActiveDataObj, dsObjData);
            if (vo.ugeCls.ugData.Rows.Count != 0 && vo.ugeCls.ugData.ActiveRow == null)
                vo.ugeCls.ugData.Rows[0].Activate();
            CancelChanges();
        }

        protected void CancelData(IEntity clsDataObject, DataSet dsClsData)
        {
            Workplace.OperationObj.Text = "Отмена изменений";
            Workplace.OperationObj.StartOperation();
            try
            {
                if (clsDataObject != null)
                {
                    dsClsData.Tables[0].BeginLoadData();
                    dsClsData.RejectChanges();
                    dsClsData.Tables[0].EndLoadData();
                }
            }
            finally
            {
                CanDeactivate = true;
                Workplace.OperationObj.StopOperation();
            }
        }

        private object InnerRefreshData()
        {
            // перед рефрешем запомним некоторые параметры... например источник, запись, которая была активна
            List<int> ids = new List<int>();
            if (vo.ugeCls.ugData.ActiveRow != null)
                ids = UltraGridHelper.GetParentsIds(vo.ugeCls.ugData.ActiveRow);

            vo.SavePersistence();

            LoadData(vo.ugeCls, null);

            if (refreshData != null)
                refreshData();

            UltraGridRow findRow = null;
            for (int i = ids.Count - 1; i >= 0; i--)
                findRow = UltraGridHelper.FindChildRow(vo.ugeCls.ugData, findRow, "ID", ids[i]);

            if (findRow != null)
            {
                findRow.Activate();
                return findRow;
            }
            return null;
        }

        /// <summary>
        /// обновление данных классификатора
        /// </summary>
        bool ugeCls_OnRefreshData(object sender)
        {
            return Refresh();
        }

        public virtual bool Refresh()
        {
            RefreshRowsCountInAllAssociations();
            // перед рефрешем проверим на изменения данных деталь, если есть
            if (isMasterDetail)
            {
                if (!TrySaveCurrentDetail())
                    return false;
            }

            // если были внесены изменения спросим, нужно ли их сохранять
            if (dsObjData.GetChanges() != null)
                if (MessageBox.Show(parentWindow, "Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (vo.ugeCls.ugData.ActiveRow != null)
                        vo.ugeCls.ugData.ActiveRow.Update();
                    if (!vo.ugeCls.PerfomAction("SaveChange"))
                        CanDeactivate = false;
                }
            if (CanDeactivate)
            {
                InnerRefreshData();
                return true;
            }
            return false;
        }

        bool ugeCls_OnSaveChanges(object sender)
        {
            return SaveData(sender, null);
        }

        protected virtual void BeforeSaveData()
        {

        }

        private void GetServerFilterCustomDialogColumnsList(object sender, ValueList valueList, string columnName)
        {
            Type sourceColumnType = vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns[columnName].DataType;
            foreach (UltraGridColumn clmn in vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns)
            {
                if ((clmn.Hidden) || (clmn.Key == columnName) || (clmn.Header.Caption == String.Empty))
                    continue;
                if (clmn.DataType == sourceColumnType)
                    valueList.ValueListItems.Add(null, String.Format("[{0}]", clmn.Header.Caption));
            }
        }

        private void ugFilter_BeforeRowFilterDropDownPopulate(object sender, BeforeRowFilterDropDownPopulateEventArgs e)
        {
            CC.GridColumnsStates states = this.ugeCls_OnGetGridColumnsState(vo.ugeCls);
            if (!states.ContainsKey(e.Column.Key))
                return;

            CC.GridColumnState gcs = states[e.Column.Key];
            if ((gcs.IsReference) && (!(gcs.CalendarColumn)))
                e.ValueList.ValueListItems.Add(null, CC.UltraGridEx.HandbookSelectCaption);
        }


        // временная заплата для получения интерфейса объекта-ссылки (классификатора)
        // c этим связано много путаницы
        private IEntity GetReferenceClassifier(string clmnName, IEntity activeObject)
        {
            string clsName = CC.UltraGridEx.GetSourceColumnName(clmnName);
            IDataAttribute attr = GetAttrByName(clsName, activeObject);
            if (attr == null)
                throw new Exception(String.Format("Аттрибут '{0}' не найден", clsName));
            IEntity classifier = null;
            string lookupFieldName = string.Empty;
            string attrName = attr.Name;
            // если ячейка с лукапом
            if (attr.IsLookup)
            {
                lookupFieldName = attr.LookupAttributeName;
                string LookupObjectName = attr.LookupObjectName;

                // иначе получим классификатор, который связан через лукап
                classifier = GetClassifierByName(attr.LookupObjectName);
            }
            // ячейка со ссылкой на какой то справочник
            else
            {
                classifier = GetBridgeClsByRefName(activeObject, attr.ObjectKey);
            }
            if (classifier == null)
                throw new InvalidOperationException("Классификатор " + clsName + " не найден в списке ассоциаций");

            return classifier;
        }

        // Этот метод и следующий содержит дублирование кода.
        // Учесть при переделке лукапов.
        private bool ugeCls_OnGetHandbookValue(object sender, string columnName, ref object handbookValue)
        {
            IEntity classifier = GetReferenceClassifier(columnName, activeDataObj);
            object newClsID = -1;
            if (Workplace.ClsManager.ShowClsModal(classifier.ObjectKey, -1, CurrentDataSourceID, ref newClsID))
            {
                handbookValue = newClsID;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Обработчик нажатия кнопки на ячейке в гриде
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ugeCls_OnClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key.Contains(openDocument))
            {
                // открываем документ
                GetActiveBlobCellParams();
                OpenDocument(GetActiveDataRow());
            }
            else if (e.Cell.Column.Key.Contains(saveDocument))
            {
                // сохраняем документ
                GetActiveBlobCellParams();
                SaveDocument(GetActiveDataRow());
            }
            else if (e.Cell.Column.Key.Contains(createNewDoc))
            {
                // показываем меню по созданию документов
                GetActiveBlobCellParams();
                Point point = ((UltraGrid)sender).PointToScreen(new Point(element.Rect.X, element.Rect.Bottom));
                vo.cmsCreateDocument.Show(point);
            }
            else if (e.Cell.Column.Key.Substring(0, 3).ToUpper() == "REF")
                // показываем vo.ugeCls
                ShowClsModal(e.Cell, ugeCls_OnGetGridColumnsState(vo.ugeCls), vo.ugeCls, activeDataObj);
        }


        private void ShowClsModal(UltraGridCell cell, CC.GridColumnsStates states, CC.UltraGridEx uge, IEntity activeObject)
        {
            vo.ToolTip.Hide();
            string columnName = cell.Column.Key;
            bool isLookup = uge.ColumnIsLookup(columnName);
            if (isLookup)
                columnName = CC.UltraGridEx.GetSourceColumnName(columnName);

            IEntity classifier = GetReferenceClassifier(columnName, activeObject);

            object newClsID = -10;
            int oldID;
            object val = cell.Row.Cells[columnName].Value;
            if (val == null || val == DBNull.Value)
                oldID = -10;
            else
                oldID = Convert.ToInt32(val);

            if (Workplace.ClsManager.ShowClsModal(classifier.ObjectKey, oldID, CurrentDataSourceID, ref newClsID))
            {
                if (vo.ugeCls.IsReadOnly)
                    return;
                if (CheckAllowEdit() == DefaultableBoolean.True)
                {
                    string columnKey = CC.UltraGridEx.GetSourceColumnName(cell.Column.Key);
                    if (states[columnKey].IsReadOnly || states[columnKey].IsSystem)
                        return;
                    // если это не таблица фактов, то проверим, зарезервирована запись или нет
                    if (this.clsClassType != ClassTypes.clsFactData)
                        if (CheckDeveloperRow(cell.Row) && !(this.Workplace.IsDeveloperMode))
                            return;

                    if (uge.CurrentRowState == Krista.FM.Client.Components.UltraGridEx.RowEditorState.ReadonlyState)
                        return;

                    cell.Row.Cells[columnName].Value = newClsID;
                }
            }
        }

        private Dictionary<string, GridColumnsStates> cashedColumnsSettings =
            new Dictionary<string, GridColumnsStates>();

        /// <summary>
        /// Возвращает из контекста ключ активного представления структуры
        /// </summary>
        /// <returns></returns>
        public virtual string GetCurrentPresentation(IEntity dataObject)
        {
            if (dataObject.Presentations.Count != 0)
            {
                LogicalCallContextData context = LogicalCallContextData.GetContext();
                if (context[String.Format("{0}.Presentation", activeDataObj.FullDBName)] != null)
                    return Convert.ToString(context[String.Format("{0}.Presentation", activeDataObj.FullDBName)]);
            }

            return string.Empty;
        }

        private GridColumnsStates currentStates;

        /// <summary>
        /// Передаем параметры отображения колонок в общий для всех модулей компонент
        /// </summary>
        /// <returns></returns>
        protected virtual GridColumnsStates ugeCls_OnGetGridColumnsState(object sender)
        {
            PresentationKey = GetCurrentPresentation(ActiveDataObj);
            currentStates = GetColumnStatesFromClsObject(ActiveDataObj, ((UltraGridEx)sender), PresentationKey);
            return currentStates;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private GridColumnsStates GetColumnStatesFromClsObject(IEntity clsObject, UltraGridEx gridEx, string currentPresentation)
        {
            // проверяем не были ли параметры закэшированы
            string objKey = String.Format("{0}.{1}", clsObject.ObjectKey, currentPresentation);

            if (cashedColumnsSettings.ContainsKey(objKey))
            {
                return cashedColumnsSettings[objKey];
            }

            string documentColumnName = string.Empty;
            // если нет - создаем и инициализируем
            var states = new GridColumnsStates();
            int attrPosition = 1;

            IDataAttributeCollection attributeCollection = !string.IsNullOrEmpty(currentPresentation) && clsObject.Presentations.ContainsKey(currentPresentation) ?
                clsObject.Presentations[currentPresentation].Attributes :
                clsObject.Attributes;

            foreach (var item in attributeCollection.Values)
            {
                // получаем прокси атрибута
                IDataAttribute attr = item;
                // **** Запоминаем необходимые параметры чтобы лишний раз не дергать прокси в цикле ****
                string attrName = attr.Name;
                string attrCaption = attr.Caption;
                int attrSize = attr.Size;
                int attrMantissaSize = attr.Scale;
                DataAttributeClassTypes attrClass = attr.Class;
                DataAttributeTypes attrType = attr.Type;
                string attrMask = attr.Mask;
                string referenceCaption = "???";
                bool nullableColumn = attr.IsNullable;
                // по свойству будем разносить в разные места
                DataAttributeKindTypes attrkind = attr.Kind;
                // название группировки аттрибута
                string groupName = attr.GroupTags;
                // свойства для группировки колонок на системные и т.п.
                GridColumnState state = new GridColumnState();
                if (nullableColumn)
                    state.IsNullable = true;

                if (attrSize > 20 && attrSize < 80)
                    state.ColumnWidth = 20;
                else
                    state.ColumnWidth = attrSize;
                state.IsHiden = !attr.Visible;
                state.IsReadOnly = attr.IsReadOnly;
                state.ColumnName = attrName;
                state.DefaultValue = attr.DefaultValue;
                state.isTextColumn = attr.StringIdentifier;
                state.IsBLOB = attr.Type == DataAttributeTypes.dtBLOB;
                state.ColumnPosition = attr.Position;
                // указываем группу аттрибута и то, является ли аттрибут в группе первым
                if (!string.IsNullOrEmpty(groupName))
                {
                    groupName = groupName.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    state.GroupName = groupName;
                    if (!gridEx.Groups.ContainsKey(groupName))
                    {
                        //gridEx.Groups.Add(groupName, state.ColumnName);
                        state.FirstInGroup = true;
                    }
                    else
                        state.FirstInGroup = false;
                }

                string lookupObjectName = String.Empty;
                state.IsLookUp = AttrIsLookup(clsObject, attr, ref lookupObjectName);

                switch (attrkind)
                {
                    case DataAttributeKindTypes.Regular:
                        state.ColumnType = CC.UltraGridEx.ColumnType.Standart;
                        break;
                    case DataAttributeKindTypes.Sys:
                        state.ColumnType = CC.UltraGridEx.ColumnType.System;
                        state.IsSystem = true;
                        break;
                    case DataAttributeKindTypes.Serviced:
                        state.ColumnType = CC.UltraGridEx.ColumnType.Service;
                        state.IsSystem = true;
                        break;
                }

                if ((attrClass == DataAttributeClassTypes.Reference) &&
                    (clsClassType == ClassTypes.clsFactData)
                    || (clsClassType == ClassTypes.clsDataClassifier))
                {
                    IEntity bridgeCls = GetBridgeClsByRefName(attr.ObjectKey);
                    if (bridgeCls != null)
                        if (attrCaption == string.Empty || attrCaption == null)
                            attrCaption = GetDataObjSemanticRus(bridgeCls) + '.' + bridgeCls.Caption;
                }

                switch (attrClass)
                {
                    // если аттрибут ссылочный...
                    case DataAttributeClassTypes.Reference:
                        // и мы работаем с таблицей фактов или сопоставимым...
                        state.IsReference = true;
                        if (
                                (clsClassType == ClassTypes.clsFactData)
                                || (clsClassType == ClassTypes.clsDataClassifier)
                                || (clsClassType == ClassTypes.clsBridgeClassifier)
                            )
                        {
                            // ... показываем колонку
                            if (!state.IsHiden)
                                state.IsHiden = InInplaceMode && state.ColumnType != Krista.FM.Client.Components.UltraGridEx.ColumnType.Standart;
                            // ... цепляем к ней кнопку вызова модального справочника
                            // ... устанавливаем заголовок (он определен ранее)
                            state.ColumnCaption = referenceCaption;
                        }
                        else
                        {
                            // для всех остальных типов объектов (различных классификаторов)
                            // ссылочные аттрибуты не видны
                            state.IsHiden = true;
                        }
                        break;
                }

                switch (attrType)
                {
                    case DataAttributeTypes.dtBoolean:
                        state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                        break;
                    case DataAttributeTypes.dtChar:
                        break;
                    case DataAttributeTypes.dtDate:
                        state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.Date;
                        break;
                    case DataAttributeTypes.dtDateTime:
                        state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTime;
                        break;
                    case DataAttributeTypes.dtDouble:
                        if (attrMantissaSize >= 20)
                            attrMantissaSize = 19;
                        string newMask = GetMask(attrSize - attrMantissaSize);
                        state.Mask = String.Concat('-', newMask, '.', String.Empty.PadRight(attrMantissaSize, 'n'));
                        break;
                    case DataAttributeTypes.dtBLOB:
                        // колонку, которая будет содержать сам документ показывать не будем
                        attrCaption = string.Empty;
                        state.IsHiden = true;
                        documentColumnName = state.ColumnName;
                        state.IsBLOB = true;
                        break;
                    // для целочисленных типов - устанавливаем маску по умолчанию
                    // равной размеру атрибута. Может перекрываться маской
                    // определенной в XML-е схемы
                    case DataAttributeTypes.dtInteger:
                        if (state.IsReference)
                        {
                            string tmpName = attr.LookupObjectName;
                            // для лукапа типа календарь нужно будет создавать дополнительное поле типа string
                            if (tmpName.Contains("fx.Date.YearDay"))
                            {
                                state.Mask = "nnnn.nn.nn";
                                state.CalendarColumn = true;
                                state.IsSystem = false;
                                state.ColumnType = UltraGridEx.ColumnType.Standart;
                            }
                            else
                            {
                                state.Mask = string.Concat("-", string.Empty.PadLeft(attrSize, 'n'));
                            }
                        }
                        else
                        {
                            state.Mask = string.Compare("ID", attrName, true) == 0 ?
                                string.Concat("-", string.Empty.PadLeft(attrSize, 'n')) :
                                String.Concat("-", GetMask(attrSize));
                        }
                        break;
                    case DataAttributeTypes.dtString:
                        state.Mask = String.Empty.PadRight(attrSize, 'a');
                        break;
                }

                if ((attrMask != null) && (attrMask != String.Empty))
                    state.Mask = attrMask;

                if (state.IsLookUp)
                {
                    // ... цепляем к ней кнопку вызова модального справочника
                    /*if (!state.IsHiden)
                        state.IsHiden = (InInplaceMode);*/
                    // пишем в TAG название исходного лукапа
                    state.Tag = lookupObjectName;
                }

                if (state.ColumnName.Contains(documentColumnName + "Type") && documentColumnName != string.Empty)
                {
                    state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
                    state.Mask = string.Empty;
                }

                state.ColumnCaption = attrCaption;
                states.Add(attrName, state);
                attrPosition++;
            }
            cashedColumnsSettings.Add(objKey, states);
            //currentStates = states;
            return states;
        }

        private static string GetMask(int masklength)
        {
            int charges = masklength / 3;
            int remainder = masklength % 3;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= charges - 1; i++)
            {
                sb.Append(",nnn");
            }
            if (remainder == 0)
                sb.Remove(0, 1);
            return string.Concat(String.Empty.PadRight(remainder, 'n'), sb.ToString());
        }

        /// <summary>
        /// Главная процедура инициализации объекта
        /// </summary>
        public override void Initialize()
        {
            // вызываем метод предка
            base.Initialize();
            // инициализируем интерфейс пользователя
            InitializeUI();
            // загружаем данные активного объекта
            //LoadData(null, null);

            AssociationPageLoad(vo.utcDataCls.ActiveTab);

            ViewCtrl.Text = FullCaption;
            //if (!InInplaceMode)
            //    ReloadData();
        }
        #endregion

        #region Сохранение данных
        /// <summary>
        /// Отмена сделанных пользователем изменений
        /// </summary>
        public override void CancelChanges()
        {
        }

        /// <summary>
        /// Сохранение сделанных пользователем изменений (метод предка)
        /// </summary>
        public override void SaveChanges()
        {
            SaveData(null, null);
        }

         public bool SaveLastSelectedDataSource
        {
            get;
            set;
        }

        /// <summary>
        /// Типа проверяет правильность ввода информации, если что не ввели, то выдает строку и поле
        /// </summary>
        /// <param name="table"></param>
        /// <param name="errorString"></param>
        /// <returns></returns>
        protected virtual bool ValidateDataTable(DataTable table, ref string errorString, CC.GridColumnsStates states)
        {
            table.BeginLoadData();
            Dictionary<string, IEntity> classifiers = new Dictionary<string, IEntity>();
            // ищем в таблице построчно, если нашли незаполненное поле, выходим
            for (int i = 0; i <= table.Rows.Count - 1; i++)
            {
                // для записей помеченых на удаление ничего не делаем
                if (table.Rows[i].RowState == DataRowState.Added || table.Rows[i].RowState == DataRowState.Modified)
                    // проверяем для каждого обязательного поля его заполненность в записи
                    foreach (GridColumnState state in states.Values)
                    {
                        if (state.ColumnName != "ID" && !state.IsBLOB)
                            if (!state.IsNullable && (table.Columns.Contains(state.ColumnName)))
                            {
                                if (table.Rows[i][state.ColumnName] == DBNull.Value || table.Rows[i][state.ColumnName].ToString() == string.Empty)
                                {
                                    object DefaultValue = state.DefaultValue;
                                    if (DefaultValue != null && DefaultValue.ToString() != string.Empty)
                                        table.Rows[i][state.ColumnName] = DefaultValue;
                                    else
                                    {
                                        string str = state.ColumnCaption;
                                        // проверим по сцылкам на классификаторы
                                        if (state.IsReference)
                                        {
                                            if (str == string.Empty || str == null)
                                            {
                                                IEntity bridgeCls = null;
                                                string classifierName = state.ColumnName;
                                                if (!classifiers.ContainsKey(classifierName))
                                                {
                                                    bridgeCls = GetBridgeClsByRefName(classifierName);
                                                    classifiers.Add(classifierName, bridgeCls);
                                                }
                                                else
                                                    classifiers.TryGetValue(classifierName, out bridgeCls);
                                                str = GetDataObjSemanticRus(bridgeCls) + '.' + bridgeCls.Caption;
                                                bridgeCls = null;
                                            }
                                        }
                                        errorString = string.Format("Обязательное поле '{0}' записи с ID = {1} не заполнено", str, table.Rows[i]["ID"]);
                                        table.EndLoadData();
                                        return false;
                                    }
                                }
                            }
                    }
            }
            table.EndLoadData();
            return true;
        }

        /// <summary>
        /// Сохранение сделанных пользователем изменений (для внутреннего использования)
        /// </summary>
        public virtual bool SaveData(object sender, EventArgs e)
        {
            BeforeSaveData();
            if (dsObjData.Tables.Count < 1) return false;
            bool SucceessSaveChanges = false;

            if (ActiveDataObj != null)
            {
                // сделаем проверку на корректность данных
                string stringError = string.Empty;
                if (ValidateDataTable(dsObjData.Tables[0], ref stringError, ugeCls_OnGetGridColumnsState(vo.ugeCls)))
                {
                    // сохраняем изменения 
                    if (isMasterDetail)
                        if (!ugeDetail_OnSaveChanges(activeDetailGrid))
                            return false;
                    // получаем датасет с измененными записями
                    using (IDataUpdater upd = GetActiveUpdater(null))
                    {
                        // сохраняем измененные ( а так же удаленные и добавленные) записи
                        DataSet ChangedRecords = dsObjData.GetChanges();
                        if (ChangedRecords != null)
                        {
                            Workplace.OperationObj.Text = "Сохранение изменений";
                            Workplace.OperationObj.StartOperation();
                            // если такие записи есть
                            try
                            {
                                // сохраняем основные данные
                                upd.Update(ref ChangedRecords);
                                // сохраняем документы
                                SaveAllDocuments(dsObjData, vo.ugeCls.CurrentStates, activeDataObj.FullDBName, true);

                                OnGetChangedAfterUpdate(dsObjData, ChangedRecords);

                                // применение всех изменений в источнике данных
                                dsObjData.Tables[0].BeginLoadData();
                                dsObjData.Tables[0].AcceptChanges();
                                dsObjData.Tables[0].EndLoadData();
                                SucceessSaveChanges = true;
                                Workplace.OperationObj.StopOperation();
                                if (vo.ugeCls.ugData.ActiveRow != null)
                                    ugeCls_OnAfterRowActivate(vo.ugeCls.ugData, new EventArgs());
                            }
                            catch (Exception exception)
                            {
                                // в случае исключения даем пользователю изменить некорректные данные
                                Workplace.OperationObj.StopOperation();
                                throw new Exception(exception.Message, exception.InnerException);
                            }
                        }
                        else
                            SucceessSaveChanges = true;
                    }
                }
                else
                {
                    MessageBox.Show(parentWindow, stringError, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            if (vo.ugeCls.ugData.Rows.Count != 0 && vo.ugeCls.ugData.ActiveRow == null)
                vo.ugeCls.ugData.Rows[0].Activate();

            CanDeactivate = SucceessSaveChanges;
            return SucceessSaveChanges;
        }
        #endregion

        #region Обработчики тулбаров грида с данными объекта-поставщика данных

        #region работа с изменением диапазона ID

        /// <summary>
        /// изменение диапазона ID всего классификатора
        /// </summary>
        /// <param name="reserveRows"></param>
        private void ChangeAllRowsDiapazonID(bool reserveRows)
        {
            this.Workplace.OperationObj.Text = "Обработка данных";
            this.Workplace.OperationObj.StartOperation();
            List<int> classifierRowsIds = new List<int>();
            try
            {
                foreach (DataRow dataRow in dsObjData.Tables[0].Rows)
                {
                    if (dataRow.RowState != DataRowState.Deleted)
                    {
                        int id = Convert.ToInt32(dataRow["ID"]);
                        if ((reserveRows && id < developerRowDiapazon) || (!reserveRows && id >= developerRowDiapazon))
                            classifierRowsIds.Add(id);
                    }
                }
                ((IClassifier)this.ActiveDataObj).ReverseRowsRange(classifierRowsIds.ToArray());
                ugeCls_OnRefreshData(null);
            }
            finally
            {
                this.Workplace.OperationObj.StopOperation();
            }
        }

        /// <summary>
        /// изменяем ID у записей для визуализации изменений
        /// </summary>
        /// <param name="selectedNonDeletedRowsID"></param>
        /// <param name="newIds"></param>
        /// <param name="reserveChildRows"></param>
        /// <param name="parentRefClmnName"></param>
        private void ChangeRowsDiapazonID(List<int> selectedNonDeletedRowsID, int[] newIds,
            bool reserveChildRows, bool reserveRows, string parentRefClmnName)
        {
            int i = 0;
            foreach (UltraGridRow selectedRow in this.vo.ugeCls._ugData.Selected.Rows)
            {
                int selectedRowId = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                DataRow[] selectedDataRow = dsObjData.Tables[0].Select(String.Format("ID = {0}", selectedRowId));
                // если запись не удалена, то 
                if (selectedDataRow.Length > 0)
                {
                    // получаем ID записи
                    int oldId = Convert.ToInt32(selectedDataRow[0]["ID"]);
                    int newId = 0;
                    // если ID подходит под замену, то получаем новый ID из списка, полученного с сервера
                    if ((oldId < developerRowDiapazon && reserveRows) || (oldId >= developerRowDiapazon && !reserveRows))
                    {
                        newId = newIds[i];
                        i++;
                    }
                    else
                        newId = oldId;
                    // меняем ID для подчиненных записей
                    if (reserveChildRows)
                        SetChildRowsChangedID(selectedRow, newIds, ref i, newId, parentRefClmnName, reserveRows);
                    // меняем иконки в строках, если новый ID подходит по критериям замены старого на новый
                    if (newId < developerRowDiapazon && !reserveRows)
                    {
                        selectedRow.Cells["RowReserved_Image"].Appearance.ResetImage();
                        selectedRow.Cells["RowReserved_Image"].ToolTipText = string.Empty;
                    }
                    else if (newId >= developerRowDiapazon && reserveRows)
                    {
                        selectedRow.Cells["RowReserved_Image"].Appearance.Image = vo.ilImages.Images[16];
                        selectedRow.Cells["RowReserved_Image"].ToolTipText = "Запись зарезервирована";
                    }

                    // если иерархия, но не меняем подчиненные записи, то изменим ссылки на текущую запись
                    if (parentRefClmnName != string.Empty && !reserveChildRows)
                    {
                        DataRow[] childRows = dsObjData.Tables[0].Select(String.Format("{0} = {1}",
                            parentRefClmnName, selectedDataRow[0]["ID"]));
                        foreach (DataRow childRow in childRows)
                        {
                            if ((reserveRows && newId < developerRowDiapazon) || (!reserveRows && newId >= developerRowDiapazon))
                                SetChangesWithoutSaveQuestion(childRow, newId, parentRefClmnName);
                        }
                    }
                    // применяем изменения если нужно, что бы при рефреше не ругалось на изменения
                    if ((oldId >= developerRowDiapazon && !reserveRows) || (oldId < developerRowDiapazon && reserveRows))
                        SetChangesWithoutSaveQuestion(selectedDataRow[0], newId, "ID");
                    selectedRow.Update();
                    // почему то иерархия раскрывается только если запись сделать активной
                    selectedRow.Activate();
                }
            }
        }

        /// <summary>
        /// внесение изменений в запись без вопроса о сохранении измений
        /// </summary>
        /// <param name="row"></param>
        /// <param name="value"></param>
        /// <param name="fieldName"></param>
        private void SetChangesWithoutSaveQuestion(DataRow row, object value, string fieldName)
        {
            if (row.RowState == DataRowState.Unchanged)
            {
                row[fieldName] = value;
                row.AcceptChanges();
            }
            else
                row[fieldName] = value;
        }


        private void GetChildRowsID(UltraGridRow parentRow, List<int> idList, bool reserveRows)
        {
            // пока будем считать, что перед вызовом уже проверили, что запись не удаленная
            if (parentRow.ChildBands != null)
                foreach (UltraGridChildBand childBand in parentRow.ChildBands)
                {
                    if (childBand.Rows != null)
                        GetChildRowsID(idList, childBand.Rows, reserveRows);
                }
        }

        /// <summary>
        /// рекурсивное получение списка ID у подчиненных записей
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="rows"></param>
        private void GetChildRowsID(List<int> idList, RowsCollection rows, bool reserveRows)
        {
            foreach (UltraGridRow row in rows)
            {
                // только для не удаленных записей просматриваем подчиненные
                int idValue = Convert.ToInt32(row.Cells["ID"].Value);
                if (dsObjData.Tables[0].Select(string.Format("ID = {0}", idValue)).Length > 0)
                {
                    if ((reserveRows && idValue < developerRowDiapazon) || (!reserveRows && idValue >= developerRowDiapazon))
                        idList.Add(idValue);

                    if (row.ChildBands != null)
                        foreach (UltraGridChildBand childBand in row.ChildBands)
                        {
                            if (childBand.Rows != null)
                                GetChildRowsID(idList, childBand.Rows, reserveRows);
                        }
                }
            }
        }

        private void SetChildRowsChangedID(UltraGridRow parentRow, int[] newIdList,
            ref int currentIndex, int parentID, string refClmnName, bool reserveRows)
        {
            if (parentRow.ChildBands != null)
                foreach (UltraGridChildBand childBand in parentRow.ChildBands)
                {
                    if (childBand.Rows != null)
                    {
                        List<UltraGridRow> rowList = new List<UltraGridRow>();
                        foreach (UltraGridRow row in childBand.Rows)
                        {
                            rowList.Add(row);
                        }
                        SetChildRowsChangedID(rowList, newIdList, ref currentIndex, parentID, refClmnName, reserveRows);
                    }
                }
        }


        private void SetChildRowsChangedID(List<UltraGridRow> rows, int[] newIdList,
            ref int currentIndex, int parentID, string refClmnName, bool reserveRows)
        {
            foreach (UltraGridRow row in rows)
            {
                // только для не удаленных записей просматриваем подчиненные
                int oldID = Convert.ToInt32(row.Cells["ID"].Value);
                DataRow[] dataRows = dsObjData.Tables[0].Select(string.Format("ID = {0}", oldID));
                if (dataRows.Length > 0)
                {
                    // получим новый ID. Если надо изменить, то берем из коллекции новый, если нет, то старый
                    int newID = 0;
                    if ((reserveRows && oldID < developerRowDiapazon) || (!reserveRows && oldID >= developerRowDiapazon))
                    {
                        newID = newIdList[currentIndex];
                        currentIndex++;
                    }
                    else
                        newID = oldID;
                    // пробежим по подчиненным записям изменим ID у них, если необходимо
                    if (row.ChildBands != null)
                        foreach (UltraGridChildBand childBand in row.ChildBands)
                        {
                            if (childBand.Rows != null)
                                if (childBand.Rows.Count > 0)
                                {
                                    List<UltraGridRow> rowList = new List<UltraGridRow>();
                                    foreach (UltraGridRow row1 in childBand.Rows)
                                    {
                                        rowList.Add(row1);
                                    }
                                    SetChildRowsChangedID(rowList, newIdList, ref currentIndex, newID, refClmnName, reserveRows);
                                }
                        }
                    // поменяем картинки и хинты
                    if (newID < developerRowDiapazon && !reserveRows)
                    {
                        row.Cells["RowReserved_Image"].Appearance.ResetImage();
                        row.Cells["RowReserved_Image"].ToolTipText = string.Empty;
                    }
                    else
                    {
                        if (newID >= developerRowDiapazon && reserveRows)
                        {
                            row.Cells["RowReserved_Image"].Appearance.Image = vo.ilImages.Images[16];
                            row.Cells["RowReserved_Image"].ToolTipText = "Запись зарезервирована";
                        }
                    }
                    // изменим ID на новое
                    if (dataRows[0].RowState == DataRowState.Unchanged)
                    {
                        dataRows[0][refClmnName] = parentID;
                        dataRows[0]["ID"] = newID;
                        dataRows[0].AcceptChanges();
                    }
                    else
                    {
                        dataRows[0][refClmnName] = parentID;
                        dataRows[0]["ID"] = newID;
                    }
                    row.Update();
                }
                if (rows.Count == 0)
                    return;
            }
        }

        #endregion


        #region варианты



        #endregion

        #endregion

        #region Методы загрузки данных
        // проверка возможности добавления записи в зависимости от текущего состояния объекта
        protected virtual AllowAddNew CheckAllowAddNew()
        {
            if (InInplaceMode)
                return AllowAddNew.No;

            if (ActiveDataObj == null)
                return AllowAddNew.No;

            if (ActiveDataObj.ClassType == ClassTypes.clsFixedClassifier)
                return AllowAddNew.No;

            if (ActiveDataObj.SubClassType == SubClassTypes.Pump)
                return AllowAddNew.No;

            if (HasDataSources() && (CurrentDataSourceID == NullDataSource))
                return AllowAddNew.No;

            if (!AllowAddNewToFacts())
                return AllowAddNew.No;

            return AllowAddNew.Yes;
        }

        private bool CheckAllowImportFromXml()
        {
            if (InInplaceMode)
                return false;

            if (ActiveDataObj == null)
                return false;

            if (clsClassType == ClassTypes.clsFixedClassifier)
                return false;

            if (ActiveDataObj.SubClassType == SubClassTypes.Pump)
                return false;

            if (clsClassType != ClassTypes.clsFactData)
            {
                if (HasDataSources() && (CurrentDataSourceID == NullDataSource))
                    return true;
            }
            else
                if (HasDataSources() && (CurrentDataSourceID == NullDataSource) && CurrentTaskID > -1)
                    return true;
            return false;
        }

        /// <summary>
        /// проверяем, можно ли добавлять новые записи в таблицы фактов, где нужен ID задачи
        /// </summary>
        /// <returns></returns>
        internal virtual bool AllowAddNewToFacts()
        {
            return true;
        }

        /// <summary>
        /// проверяем, редактируемый ли текущий классификатор
        /// </summary>
        /// <returns></returns>
        protected DefaultableBoolean CheckAllowEdit()
        {
            if (InAssociateMode)
                return DefaultableBoolean.True;

            if (InInplaceMode)
                return DefaultableBoolean.False;

            if (ActiveDataObj == null)
                return DefaultableBoolean.False;

            if (ActiveDataObj.ClassType == ClassTypes.clsFixedClassifier)
                return DefaultableBoolean.False;

            if (ActiveDataObj.SubClassType == SubClassTypes.Pump)
                return DefaultableBoolean.False;

            return DefaultableBoolean.True;
        }

        /// <summary>
        /// очищает грид от данных
        /// </summary>
        private void ClearData(ref DataSet ds)
        {
            ds.BeginInit();
            if (ds.Tables.Count > 0)
                ds.Tables[0].Rows.Clear();
            ds.Relations.Clear();
            ds.Tables.Clear();
            ds.Clear();
            ds.RemotingFormat = SerializationFormat.Binary;
            ds.EndInit();
            if (copyTable != null)
            {
                copyTable.Clear();
                copyRowsLevels.Clear();
            }
            vo.ugeCls.utmMain.Tools["PasteRow"].SharedProps.Enabled = false;
        }

        public IDbDataParameter GetDbDataParameter(Type paramType, String paramName, object paramValue)
        {
            return new DbParameterDescriptor(paramName, paramValue);
        }

        public static IDbDataParameter[] ParametersListToArray(List< UltraGridEx.FilterParamInfo> parameters)
        {
            if ((parameters == null) || (parameters.Count == 0))
                return null;

            IDbDataParameter[] result = new IDbDataParameter[parameters.Count];
            int ind = 0;
            foreach (UltraGridEx.FilterParamInfo paramInfo in parameters)
            {
                result[ind] = new DbParameterDescriptor(paramInfo.ParamName, paramInfo.ParamValue);
                ind++;
            }
            return result;
        }

        /// <summary>
        /// Возвращает набор параметров для фильтра таблиц фактов.
        /// </summary>
        /// <returns></returns>
        protected virtual List<UltraGridEx.FilterParamInfo> GetServerFilterParameters()
        {
            return FilterParameters;
        }

        private List<UltraGridEx.FilterParamInfo> serverFilterParameters;

        internal List<UltraGridEx.FilterParamInfo> FilterParameters
        {
            get { return serverFilterParameters; }
            set { serverFilterParameters = value; }
        }

        // фильтр для получения данных
#warning Поле удалить и сделать функцию, которая формирует фильтр
        protected string dataQuery;
        public string DataQuery
        {
            get { return dataQuery; }
            set { dataQuery = value; }
        }

        public IDataUpdater GetActiveUpdater(int? parentID)
        {
            string filterStr;
            return GetActiveUpdater(parentID, out filterStr);
        }

        protected virtual IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            filterStr = dataQuery;
            if (!InInplaceMode)
                dataQuery = string.IsNullOrEmpty(dataQuery) ? "id > -1" : dataQuery + " and (id > -1)";
            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }

        protected virtual IDataUpdater GetActiveUpdater(int? parentID, int recordsCount, out string filterStr)
        {
            filterStr = dataQuery;
            if (!InInplaceMode)
                dataQuery = string.IsNullOrEmpty(dataQuery) ? "id > -1" : dataQuery + " and (id > -1)";
            return ActiveDataObj.GetDataUpdater(dataQuery, recordsCount, ParametersListToArray(FilterParameters));
        }

        /// <summary>
        /// Заполнение датасета данными классификатора
        /// </summary>
        protected void InternalLoadData()
        {
            InternalLoadData(ref dsObjData);
        }

        private string additionalFilter;
        public string AdditionalFilter
        {
            get { return additionalFilter; }
            set { additionalFilter = value; }
        }

        protected virtual void AddFilter()
        {
            if (!string.IsNullOrEmpty(AdditionalFilter))
                dataQuery += AdditionalFilter;
        }

        /// <summary>
        /// Заполнение датасета данными классификатора
        /// </summary>
        private void InternalLoadData(ref DataSet ds)
        {
            // очищаем данные предыдущего объекта
            ClearData(ref ds);

            IDataUpdater upd = null;
            try
            {
                string filterStr = null;
                upd = GetActiveUpdater(null, out filterStr);
                upd.Fill(ref ds);
                AddBlobColumns(ref ds);
                ClearCalendarData(ref ds);
                // инициализируем кэш лукапов
                LookupManager.Instance.InitLookupsCash(ActiveDataObj, ds);
            }
            finally
            {
                if (upd != null)
                    upd.Dispose();
            }
        }

        /// <summary>
        /// добавление блобовых полей в датасет
        /// </summary>
        /// <param name="ds"></param>
        private void AddBlobColumns(ref DataSet ds)
        {
            foreach (IDataAttribute item in activeDataObj.Attributes.Values)
            {
                if (item.Type == DataAttributeTypes.dtBLOB)
                {
                    ds.Tables[0].Columns.Add(item.Name, typeof(byte[]));
                }
            }
        }

        /// <summary>
        /// добавление блобовых полей в датасет
        /// </summary>
        /// <param name="ds"></param>
        private void AddBlobColumns(IEntity dataObj, ref DataSet ds)
        {
            foreach (IDataAttribute item in dataObj.Attributes.Values)
            {
                if (item.Type == DataAttributeTypes.dtBLOB)
                {
                    ds.Tables[0].Columns.Add(item.Name, typeof(byte[]));
                }
            }
        }

        private void ClearCalendarData(ref DataSet ds)
        {
            foreach (DataTable table in ds.Tables)
            {
                table.BeginLoadData();
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        if (column.DataType == typeof(DateTime) && !row.IsNull(column))
                        {
                            DateTime dt = DateTime.Parse(row[column].ToString());
                            if (dt.Year == DateTime.MinValue.Year)
                            {
                                row[column] = DBNull.Value;
                                row.AcceptChanges();
                            }
                        }
                    }
                }
                table.EndLoadData();
            }
        }

        // Метод установки состояний кнопок тулбара
        // Все подобные действия следует делать только здесь, не размазывать по коду
        public virtual void UpdateToolbar()
        {
            // устанавливаем видимость списка истоников данных
            vo.utbToolbarManager.Toolbars["utbFilters"].Visible = HasDataSources();
            // если есть поле для ввода задачи, то показываем список задач для выбора
            vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
            // для классификаторов данных делаем экспорт в XML всегда доступным
            vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Enabled = true;

            ButtonTool btnSearchText = (ButtonTool)vo.ugeCls.utmMain.Tools["btnSearchText"];
            btnSearchText.SharedProps.Visible = vo.ugeCls.HierarchyInfo.CurViewState != ViewState.Hierarchy;

            
            HierarchyInfo hi = vo.ugeCls.HierarchyInfo;// GetHierarchyInfo(vo.ugeCls);
            // если нету маски ращепления, то иерархию ставить вообще не будем
            UltraToolbar tb = vo.ugeCls.utmMain.Toolbars["utbColumns"];
            ButtonTool SetHierarchy = null;
            if (!vo.ugeCls.utmMain.Tools.Exists("SetHierarchy"))
            {
                SetHierarchy = new ButtonTool("SetHierarchy");
                SetHierarchy.SharedProps.ToolTipText = "Выполнить расщепление кода и установку иерархии";
                SetHierarchy.SharedProps.AppearancesSmall.Appearance.Image = vo.ilImages.Images[10];
                vo.ugeCls.utmMain.Tools.Add(SetHierarchy);
                tb.Tools.AddTool("SetHierarchy");
                SetHierarchy.SharedProps.Visible = hi.isDivideCode;
            }
            else
                SetHierarchy = (ButtonTool)vo.ugeCls.utmMain.Tools["SetHierarchy"];

            SetHierarchy.SharedProps.Visible = hi.isDivideCode && (!InInplaceMode || InAssociateMode);

            if (InInplaceMode)
                return;

            if (Workplace.IsDeveloperMode &&
                (clsClassType != ClassTypes.clsFactData && clsClassType != ClassTypes.clsFixedClassifier))
            {
                ButtonTool btnReserveRow = null;
                if (!vo.ugeCls.utmMain.Tools.Exists("btnReserveRow"))
                {
                    btnReserveRow = new ButtonTool("btnReserveRow");
                    btnReserveRow.SharedProps.AppearancesSmall.Appearance.Image = vo.ilImages.Images[16];
                    btnReserveRow.SharedProps.Caption = "Заблокировать/разблокировать записи";
                    vo.ugeCls.utmMain.Tools.Add(btnReserveRow);
                    tb.Tools.AddTool("btnReserveRow");
                }
                else
                    btnReserveRow = (ButtonTool)vo.ugeCls.utmMain.Tools["btnReserveRow"];
                btnReserveRow.SharedProps.Visible = Workplace.IsDeveloperMode;
            }

            // Добавляем кнопку показа, где есть зависимые данные.
            ButtonTool btnShowDependedData = null;
            if (!vo.ugeCls.utmMain.Tools.Exists("ShowDependedData"))
            {
                btnShowDependedData = new ButtonTool("ShowDependedData");
                btnShowDependedData.SharedProps.ToolTipText = "Показать наличие зависимых данных";
                Infragistics.Win.Appearance appearanceTool = new Infragistics.Win.Appearance();
                btnShowDependedData.SharedProps.AppearancesSmall.Appearance.Image =
                    Properties.Resources.GetDependedData;
                vo.ugeCls._utmMain.Tools.Add(btnShowDependedData);
                tb.Tools.AddTool("ShowDependedData");
            }
            else
                btnShowDependedData = (ButtonTool)vo.ugeCls.utmMain.Tools["ShowDependedData"];
            btnShowDependedData.SharedProps.Visible = true;
            // кнопка слияния дубликатов.
            if (!vo.ugeCls.utmMain.Tools.Exists("btnMergingDuplicates"))
            {
                ButtonTool btnMergingDuplicates = new ButtonTool("btnMergingDuplicates");
                btnMergingDuplicates.SharedProps.ToolTipText = "Объединить дубликаты";
                btnMergingDuplicates.SharedProps.AppearancesSmall.Appearance.Image =
                    Properties.Resources.MergeDuplicates;
                btnMergingDuplicates.SharedProps.Visible =
                    (this.clsClassType == ClassTypes.clsBridgeClassifier ||
                    this.clsClassType == ClassTypes.clsDataClassifier);
                vo.ugeCls.utmMain.Tools.Add(btnMergingDuplicates);
                tb.Tools.AddTool("btnMergingDuplicates");
            }

            //кнопка отображения деталей
            ButtonTool btnDetailVisible = null;
            if (!vo.ugeCls.utmMain.Tools.Exists("btnDetailVisible"))
            {
                btnDetailVisible = new ButtonTool("btnDetailVisible");
                btnDetailVisible.SharedProps.ToolTipText = "Скрыть детали";
                btnDetailVisible.SharedProps.AppearancesSmall.Appearance.Image = vo.ilImages.Images[12];
                vo.ugeCls.utmMain.Tools.Add(btnDetailVisible);
                tb.Tools.AddTool("btnDetailVisible");
            }
            else
                btnDetailVisible = (ButtonTool)vo.ugeCls.utmMain.Tools["btnDetailVisible"];
            btnDetailVisible.SharedProps.Visible = false;
        }

        public override void InitializeData()
        {
            LoadData(vo.ugeCls, null);
        }

        /// <summary>
        /// Главная процедура загрузки данных активного объекта
        /// Требуется переделка на загрузку данных только активной страницы
        /// </summary>
        protected virtual void LoadData(object sender, EventArgs e)
        {
            if (associationDataTable != null)
                associationDataTable.Rows.Clear();

            UltraGrid grid = vo.ugeCls.ugData;

            // очищаем список ассоцоаций
            vo.udsAssociations.Rows.Clear();

            // Если объект есть
            if (ActiveDataObj != null)
            {
                // показываем окно запроса данных
                Workplace.OperationObj.Text = "Запрос данных";
                Workplace.OperationObj.StartOperation();
                // настраиваем панель с деталями
                SetDetailVisible(ActiveDataObj);
                // отцепляем грид от старого объекта просмотра
                //grid.DataSource = null;
                grid.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                try
                {
                    // очищаем все от данных
                    if (dsObjData.Tables.Count > 0)
                        ClearData(ref dsObjData);

                    if (!InInplaceMode)
                        CheckClassifierPermissions();
                    
                    // пытаемся заполнить список источников данных
                    AttachDataSources(ActiveDataObj);
                    RefreshVisibleTask();

                    AttachTaskID();

                    SetDataSourcesFilter();
                    SetVersionID();

                    if (string.IsNullOrEmpty(vo.ugeCls.sortColumnName))
                    {
                        if (DataAttributeHelper.GetByName(ActiveDataObj.Attributes, "Code") != null)
                            vo.ugeCls.sortColumnName = "Code";
                        else if (DataAttributeHelper.GetByName(ActiveDataObj.Attributes, "CodeStr") != null)
                            vo.ugeCls.sortColumnName = "CodeStr";
                        else
                            vo.ugeCls.sortColumnName = "ID";
                    }

                    // заполняем датасет данными объекта
                    InternalLoadData();

                    vo.ugeCls.ServerFilterEnabled = EnableServerFilter();
                    // в соответсвии с количеством уровней устанавливаем количесво бандов
                    // это надо делать до установки гриду источника данных, иначе не проканает
                    grid.DisplayLayout.ResetMaxBandDepth();
                    //=========================================================================//
                    // Назначаем гриду источник данных. Инициирует процедуру InitializeLayout, //
                    // настраивающую внешний вид (имена колонок, маски, размеры и т.п.).       //
                    // После этого никаких действий с датасетом производить нельзя.            //
                    //=========================================================================//
                    grid.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);

                    vo.ugeCls.DataSource = dsObjData;
                    // Если есть текущий источник данных, проверяем блокровку.
                    DataTable dtSource = DataSourcesHelper.GetDataSourcesInfo(CurrentDataSourceID, Workplace.ActiveScheme);
                    if (dtSource.Rows.Count > 0 && !vo.ugeCls.IsReadOnly)
                    {
                        vo.ugeCls.IsReadOnly = Convert.ToBoolean(dtSource.Rows[0]["Locked"]);
                    }

                    ultraGridExComponent = vo.ugeCls;
                    UpdateToolbar();
                    ultraGridExComponent.SaveLoadFileName = GetFileName();

                    // видимость кнопки показа деталей
                    if (vo.ugeCls.utmMain.Tools.Exists("btnDetailVisible"))
                        vo.ugeCls.utmMain.Tools["btnDetailVisible"].SharedProps.Visible = (isMasterDetail && !InInplaceMode);

                    vo.ugeCls.ugData.DisplayLayout.Override.SelectTypeRow = SelectType.Default;

                    HierarchyInfo hi = ((UltraGridEx)sender).HierarchyInfo;
                    isHierarchy = !String.IsNullOrEmpty(hi.ParentClmnName) && !String.IsNullOrEmpty(hi.ParentRefClmnName);
					// Загружаем сохраненные настройки
                    LoadLoadPersistence();
                    // после загрузки настроек проверим и установим права
                    if (CheckAllowAddNew() == AllowAddNew.Yes || InAssociateMode)
                    {
                        SetPermissionsToClassifier(vo.ugeCls);
                    }
                    else
                    {
                        if (!InInplaceMode)
                            vo.ugeCls.IsReadOnly = true;
                        // проверим, можно ли импортировать данные с импортом источника
                        vo.ugeCls.AllowImportFromXML = CheckAllowImportFromXml();

                    }
                    // загрузим данные в деталь. Делаем это после того, как были установлены права в мастер-таблице
                    if (dsObjData.Tables[0].Rows.Count > 0)
                        vo.ugeCls.ugData.Rows[0].Activate();
                    else if (isMasterDetail)
                    {
                        LoadDetailData(-1);
                    }

                    vo.ugeCls.SetVisibleColumnsButtons();
                    if (InAssociateMode)
                    {
                        vo.ugeCls.IsReadOnly = false;
                        allowDelRecords = false;
                        vo.ugeCls.AllowDeleteRows = false;
                        allowAddRecord = false;
                        vo.ugeCls.AllowAddNewRecords = false;
                        vo.ugeCls.AllowClearTable = false;
                        vo.ugeCls.AllowEditRows = true;
                    }
                }
                finally
                {
                    grid.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                    Workplace.OperationObj.StopOperation();
                }
            }
        }

        public virtual void LoadLoadPersistence()
        {
            vo.LoadPersistence();
        }

        public virtual void RefreshVisibleTask()
        {

        }

        public virtual ObjectType GetClsObjectType()
        {
            return ObjectType.Classifier;
        }

        public virtual bool EnableServerFilter()
        {
            return false;
        }


        void ugeCls_OnAfterRowInsert(object sender, UltraGridRow row)
        {
            UltraGridRow insertedRow = row;
            if (row.Cells["ID"].Value.ToString() != string.Empty)
            {
                if (((UltraGrid)sender).ActiveRow.Cells["ID"].Value.ToString() == string.Empty)
                    insertedRow = ((UltraGrid)sender).ActiveRow;
                else
                    return;
            }
            UltraGrid ug = (UltraGrid)sender;
            ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
            try
            {

                // для добавленной записи устанавливаем значения по умолчанию
                CC.GridColumnsStates cs = ugeCls_OnGetGridColumnsState(vo.ugeCls);
                foreach (CC.GridColumnState state in cs.Values)
                {
                    if (state.DefaultValue != null && state.DefaultValue != DBNull.Value)
                        row.Cells[state.ColumnName].Value = state.DefaultValue;
                }


                // если активный объект имеет источники данных - устанавливаем тот, по которому построен фильтр
                if (HasDataSources())
                {
                    UltraGridCell sourceIDCell = insertedRow.Cells["SourceID"];
                    if ((sourceIDCell.Value.ToString() == String.Empty) && (CurrentDataSourceID != NullDataSource))
                    {
                        sourceIDCell.Value = CurrentDataSourceID;
                    }
                }
                SetTaskId(ref insertedRow);

                // устанавалием новое ID
                insertedRow.Cells["ID"].Value = GetNewId();
            }
            finally
            {
                ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
            }
        }

        public virtual void SetTaskId(ref UltraGridRow row)
        {

        }

        public virtual object GetNewId()
        {
            return ActiveDataObj.GetGeneratorNextValue;
        }

        #endregion

        #region Обработчики события грида навигации
        /// <summary>
        /// Обработчик перемещения по гриду навигации (перед активацией сроки).
        /// Сохраняет изменения предыдущего объекта, освобождает интерфейс поставщика данных
        /// </summary>
        private void ugDataClsList_BeforeRowActivate(object sender, EventArgs e)
        {
            try
            {
                if (vo.ugeCls.ugData.ActiveRow != null)
                    vo.ugeCls.ugData.ActiveRow.Update();
            }
            catch
            {
                dsObjData.RejectChanges();
            }

            SaveDataThenExit();
            //ActiveDataObj = null;
            vo.spMasterDetail.Panel2Collapsed = true;
        }

        void ugDataClsList_BeforeRowDeactivate(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveDataThenExit();
            if (!CanDeactivate)
            {
                e.Cancel = true;
                return;
            }

            e.Cancel = false;
            HierarchyInfo hi = vo.ugeCls.HierarchyInfo;
            if (hi.LevelsCount > 1)
                hi.CurViewState = Krista.FM.Client.Components.ViewState.Hierarchy;
        }

        protected virtual void SetViewObjectCaption()
        {
            Workplace.ViewObjectCaption = "Не указано";
        }

        #endregion

        #region Лукапы
        public string ugeCls_OnGetLookupValue(object sender, string lookupName, bool needFoolValue, object value)
        {
            if ((value == null) || (value == DBNull.Value))
                return String.Empty;
            if (value.ToString() == string.Empty)
                return String.Empty;
            return LookupManager.Instance.GetLookupValue(lookupName, needFoolValue, Convert.ToInt32(value));
        }

        public bool ugeCls_OnCheckLookupValue(object sender, string lookupName, object value)
        {
            if ((value == null) || (value == DBNull.Value))
                return false;
            if (value.ToString() == string.Empty)
                return false;
            return LookupManager.Instance.CheckLookupValue(lookupName, Convert.ToInt32(value));
        }

        #endregion
    }
}
