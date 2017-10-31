using System;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Collections.Generic;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTabControl;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common.Configuration;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {
        private Dictionary<string, IEntity> detailEntityObjects;
		protected Dictionary<string, string> detailRefColumns;

        protected IEntity activeDetailObject;
        protected DataSet dsDetail;
        protected UltraGridEx activeDetailGrid;

        public UltraGridEx ActiveDetailGrid
        {
            get { return activeDetailGrid; }
        }

        private bool isMasterDetail;

        protected String RefMasterColumnName
        {
            get
            {
                return detailRefColumns[vo.utcDetails.ActiveTab.Key];
            }
        }

        public Dictionary<string, string> DetailRefColumnNames
        {
            get { return detailRefColumns; }
        }

        public Dictionary<string, IEntity> DetailEntityObjects
        {
            get { return detailEntityObjects; }
        }

        public string DetailPresentationKey
        {
            get; set;
        }

        /// <summary>
        /// настройка грида детали
        /// </summary>
        protected virtual void DetailGridSetup(UltraGridEx ugeDetail)
        {
            ugeDetail.StateRowEnable = true;
            ugeDetail.ServerFilterEnabled = false;

            ugeDetail.OnGetHierarchyInfo += new GetHierarchyInfo(ugeDetail_OnGetHierarchyInfo);
            ugeDetail.OnSaveChanges += new SaveChanges(ugeDetail_OnSaveChanges);
            ugeDetail.OnRefreshData += new RefreshData(ugeDetail_OnRefreshData);
            ugeDetail.OnClearCurrentTable += new DataWorking(ugeDetail_OnClearCurrentTable);
            ugeDetail.OnCancelChanges += new DataWorking(ugeDetail_OnCancelChanges);
            ugeDetail.OnGetGridColumnsState += new GetGridColumnsState(ugeDetail_OnGetGridColumnsState);
            ugeDetail.OnInitializeRow += new InitializeRow(ugeDetail_OnInitializeRow);
            ugeDetail.ugData.AfterCellUpdate += new CellEventHandler(ugDetailData_AfterCellUpdate);
            ugeDetail.ToolClick += new ToolBarToolsClick(ugeDetail_ToolClick);
            ugeDetail.OnClickCellButton += new ClickCellButton(ugeDetail_OnClickCellButton);
            ugeDetail.OnAfterRowInsert += new AfterRowInsert(ugeDetail_OnAfterRowInsert);

            ugeDetail.OnBeforeRowDeactivate += new BeforeRowDeactivate(ugeDetail_OnBeforeRowDeactivate);

            ugeDetail.OnGridInitializeLayout += new GridInitializeLayout(ugeDetail_OnGridInitializeLayout);

            ugeDetail.OnGetLookupValue += new GetLookupValueDelegate(ugeDetail_OnGetLookupValue);
            ugeDetail.OnCheckLookupValue += new CheckLookupValueDelegate(ugeDetail_OnCheckLookupValue);

            ugeDetail.OnMouseEnterGridElement += new MouseEnterElement(ugeCls_OnMouseEnterGridElement);
            ugeDetail.OnMouseLeaveGridElement += new MouseLeaveElement(ugeCls_OnMouseLeaveGridElement);

            ugeDetail.utmMain.Tools["menuLoad"].SharedProps.Visible = false;
            ugeDetail.utmMain.Tools["menuSave"].SharedProps.Visible = false;

            // настройки для кнопок копировать/вставить
            ugeDetail.utmMain.Tools["CopyRow"].SharedProps.Visible = true;
            ugeDetail.utmMain.Tools["PasteRow"].SharedProps.Visible = true;

            ugeDetail.GridDragDrop += new MainMouseEvents(ugeDetail_GridDragDrop);
            ugeDetail.GridDragEnter += new MainMouseEvents(ugeDetail_GridDragEnter);

            InfragisticComponentsCustomize.CustomizeUltraGridParams(ugeDetail.ugData);
            ugeDetail.PerfomAction("ShowGroupBy");

            ugeDetail_OnCreateGrid(ugeDetail);

            ugeDetail.ugData.MouseClick += ugDetailData_MouseClick;
        }

        public DataRow GetActiveDetailRow()
        {
            int activeRowID = UltraGridHelper.GetActiveID(GetActiveDetailGridEx().ugData);
            DataRow[] rows = dsDetail.Tables[0].Select(String.Format("ID = {0}", activeRowID));
            return rows.GetLength(0) > 0 ? rows[0] : null;
        }

        public virtual void ugeDetail_OnCreateGrid(UltraGridEx detail)
        {

        }

        /// <summary>
        /// обработчик клика мыши на гриде с данными
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugDetailData_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (!InInplaceMode && _activeUIElementIsRow)
                {
                    //_auditShowObject = AuditShowObject.Row;
                    activeGridEx = activeDetailGrid;
                    auditObjectName = activeDetailObject.FullDBName.ToUpper();
                    classType = activeDetailObject.ClassType;
                    auditRow = GetActiveDetailRow();
                    vo.cmsAudit.Show(activeDetailGrid.ugData.PointToScreen(e.Location));
                }
            }
        }

        #region настройки грида с деталью. Основные события

        HierarchyInfo ugeDetail_OnGetHierarchyInfo(object sender)
        {
            return GetHierarchyInfoFromClsObject(activeDetailObject, (UltraGridEx)sender);
        }

        #region лукапы

        bool ugeDetail_OnCheckLookupValue(object sender, string lookupName, object value)
        {
            if ((value == null) || (value == DBNull.Value))
                return false;
            if (value.ToString() == string.Empty)
                return false;
            return LookupManager.Instance.CheckLookupValue(lookupName, Convert.ToInt32(value));
        }

        string ugeDetail_OnGetLookupValue(object sender, string lookupName, bool needFoolValue, object value)
        {
            if ((value == null) || (value == DBNull.Value))
                return String.Empty;
            if (value.ToString() == string.Empty)
                return String.Empty;
            return LookupManager.Instance.GetLookupValue(lookupName, needFoolValue, Convert.ToInt32(value));
        }

        #endregion

        /// <summary>
        /// сохранение изменений
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private bool ugeDetail_OnSaveChanges(object sender)
        {
            //            BeforeDetailSaveData(ref dsDetail);
            Boolean successful = SaveDetailData((UltraGridEx)sender);
            if (successful)
            {
                AfterDetailSaveData();
            }
            return successful;
        }

        protected virtual void BeforeDetailSaveData(ref DataSet detail)
        {

        }

        protected virtual void AfterDetailSaveData()
        {

        }

        protected virtual void OnGetChangedAfterUpdate(DataSet dsObjData, DataSet changedRecords)
        {

        }

        protected virtual void ugDetailData_AfterCellUpdate(object sender, CellEventArgs e)
        {

        }

        protected virtual bool SaveDetailData(UltraGridEx gridEx)
        {
            // в случае если в деталях нет таблиц, вернем тру
            if (dsDetail.Tables.Count < 1) return true;

            bool SucceessSaveChanges = true;

            if (activeDetailObject != null)
            {
                if (activeDetailGrid.ugData.ActiveCell != null)
                    if (activeDetailGrid.ugData.ActiveCell.IsInEditMode)
                        activeDetailGrid.ugData.ActiveCell = activeDetailGrid.ugData.ActiveRow.Cells[0];

                if (activeDetailGrid.ugData.ActiveRow != null)
                    activeDetailGrid.ugData.ActiveRow.Update();

                // сделаем проверку на корректность данных
                string sringError = string.Empty;
                if (ValidateDataTable(dsDetail.Tables[0], ref sringError, GetColumnStatesFromClsObject(activeDetailObject, gridEx, DetailPresentationKey)))
                {
                    Workplace.OperationObj.Text = "Сохранение изменений";
                    Workplace.OperationObj.StartOperation();
                    activeDetailGrid.ugData.BeginUpdate();
                    // получаем датасет с измененными записями
                    IDataUpdater upd = null;
                    try
                    {
                        upd = GetDetailUpdater(activeDetailObject, GetActiveMasterRowID());
                        // сохраняем измененные ( а так же удаленные и добавленные) записи
                        DataSet ChangedRecords = dsDetail.GetChanges();
                        if (ChangedRecords != null)
                        {
                            BeforeDetailSaveData(ref ChangedRecords);
                            // если такие записи есть
                            try
                            {
                                // сохраняем основные данные
                                upd.Update(ref ChangedRecords);
                                // сохраняем документы
                                SaveAllDocuments(dsDetail, gridEx.CurrentStates, activeDetailObject.FullDBName, true);
                                // применение всех изменений в источнике данных
                                dsDetail.BeginInit();
                                dsDetail.Tables[0].BeginLoadData();
                                dsDetail.Tables[0].AcceptChanges();
                                dsDetail.Tables[0].EndLoadData();
                                dsDetail.EndInit();
                            }
                            catch (Exception exception)
                            {
                                // в случае исключения даем пользователю изменить некорректные данные
                                SucceessSaveChanges = false;
                                Workplace.OperationObj.StopOperation();
                                if (exception.Message.Contains("ORA-02292") ||
                                    exception.Message.Contains("Конфликт инструкции DELETE с ограничением"))
                                    MessageBox.Show("Нарушено ограничение целостности. Обнаружена порожденная запись.", "Сохранение изменений", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                else
                                    throw exception;
                            }
                        }
                    }
                    finally
                    {
                        if (upd != null)
                            upd.Dispose();
                        activeDetailGrid.ugData.EndUpdate();
                        Workplace.OperationObj.StopOperation();
                    }
                }
                else
                {
                    MessageBox.Show(sringError, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SucceessSaveChanges = false;
                }
            }
            CanDeactivate = SucceessSaveChanges;
            return SucceessSaveChanges;
        }

        bool ugeDetail_OnRefreshData(object sender)
        {
            if (TrySaveCurrentDetail())
            {
                if (activeDetailGrid.Parent is UltraTabPageControl)
                    ((ViewSettings)((UltraTabPageControl)activeDetailGrid.Parent).Tab.Tag).Save();
                LoadDetailData(GetActiveMasterRowID());
                return true;
            }
            return false;
        }

        void ugeDetail_OnCancelChanges(object sender)
        {
            CancelData(activeDetailObject, dsDetail);
        }

        protected virtual void ugeDetail_OnClearCurrentTable(object sender)
        {
            if (MessageBox.Show("Удалить все данные текущей таблицы?", "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Workplace.OperationObj.Text = "Удаление данных текущей таблицы";
                Workplace.OperationObj.StartOperation();
                try
                {
                    activeDetailObject.DeleteData(string.Format("where {0}", GetQueryFilter(GetActiveMasterRowID())));
                    dsDetail.Tables[0].BeginLoadData();
                    dsDetail.Tables[0].AcceptChanges();
                    foreach (DataRow row in dsDetail.Tables[0].Rows)
                    {
                        row.Delete();
                    }
                    dsDetail.Tables[0].AcceptChanges();
                    dsDetail.Tables[0].EndLoadData();
                }
                finally
                {
                    activeDetailGrid.BurnChangesDataButtons(false);
                    Workplace.OperationObj.StopOperation();
                }
            }
        }

        protected virtual GridColumnsStates ugeDetail_OnGetGridColumnsState(object sender)
        {
            return GetColumnStatesFromClsObject(activeDetailObject, ((UltraGridEx)sender), DetailPresentationKey);
        }

        void ugeDetail_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow tmpRow = UltraGridHelper.GetRowCells(e.Row);
            if (tmpRow.Cells["ID"].Value == DBNull.Value || tmpRow.Cells["ID"].Value == null)
                return;

            SetDocumentRow(tmpRow, activeDetailGrid.CurrentStates);

            InitializeDetailRow(sender, e);
        }

        /// <summary>
        /// действия во время инициализации записей в деталях
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void InitializeDetailRow(object sender, InitializeRowEventArgs e)
        {

        }

        void ugeDetail_OnBeforeRowDeactivate(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UltraGridRow row = UltraGridHelper.GetActiveRowCells(activeDetailGrid.ugData);
            if (!row.Cells.Exists("ID")) return;
            int id = Convert.ToInt32(row.Cells["ID"].Value);
            DataRow[] dataRows = dsDetail.Tables[0].Select(string.Format("ID = {0}", id));
            if (dataRows.Length == 0) return;
        }

        void ugeDetail_ToolClick(object sender, ToolClickEventArgs e)
        {

        }

        protected virtual void ugeDetail_OnClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key.Contains(openDocument))
            {
                // открываем документ
                GetActiveBlobCellParams();
                OpenDocument(GetActiveDetailRow());
                e.Cell.Activated = false;
                return;
            }
            if (e.Cell.Column.Key.Contains(saveDocument))
            {
                // сохраняем документ
                GetActiveBlobCellParams();
                SaveDocument(GetActiveDetailRow());
            }
            else if (e.Cell.Column.Key.Contains(createNewDoc))
            {
                // показываем меню по созданию документов
                GetActiveBlobCellParams();
                Point point = ((UltraGrid)sender).PointToScreen(new Point(element.Rect.X, element.Rect.Bottom));
                vo.cmsCreateDocument.Show(point);
            }
            else if (e.Cell.Column.Key.Substring(0, 3).ToUpper() == "REF")
            {
                ShowClsModal(e.Cell, GetColumnStatesFromClsObject(activeDetailObject, ActiveDetailGrid, DetailPresentationKey),
                             activeDetailGrid, activeDetailObject);
            }
        }

        public virtual void ugeDetail_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            AddDocumentsTypeListToGrid((UltraGrid)sender);
            AddDocumentButtons(e.Layout, activeDetailGrid.CurrentStates);

            if (DetailGridInitializeLayout != null)
            {
                DetailGridInitializeLayout(sender, e);
            }
        }

        /// <summary>
        /// добавление новой записи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="row"></param>
        void ugeDetail_OnAfterRowInsert(object sender, UltraGridRow row)
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
                GridColumnsStates cs = GetColumnStatesFromClsObject(activeDetailObject, ActiveDetailGrid, DetailPresentationKey);
                foreach (GridColumnState state in cs.Values)
                {
                    if (state.DefaultValue != null && state.DefaultValue != DBNull.Value)
                        row.Cells[state.ColumnName].Value = state.DefaultValue;
                }
                // устанавливаем ссылку на мастер
                row.Cells[detailRefColumns[vo.utcDetails.ActiveTab.Key]].Value = GetActiveMasterRowID();
                // устанавалием новое ID
                object newID;
                try
                {
                    newID = activeDetailObject.GetGeneratorNextValue;
                }
                catch
                {
                    newID = DBNull.Value;
                }
                insertedRow.Cells["ID"].Value = newID;

                AfterDetailRowInsert(sender, row);
            }
            finally
            {
                ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
            }
        }

        /// <summary>
        /// дейсвия, выполняемые после добавления записи в деталь
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="row"></param>
        protected virtual void AfterDetailRowInsert(object sender, UltraGridRow row)
        {

        }


        // в различных ситуациях пытаемся сохранить данные в детали
        protected bool TrySaveCurrentDetail()
        {
            if (dsDetail == null) return true;

            if (activeDetailGrid.ugData.ActiveCell != null)
                if (activeDetailGrid.ugData.ActiveCell.IsInEditMode)
                    activeDetailGrid.ugData.ActiveCell = activeDetailGrid.ugData.ActiveRow.Cells[0];

            if (activeDetailGrid.ugData.ActiveRow != null)
                activeDetailGrid.ugData.ActiveRow.Update();

            if (dsDetail.GetChanges() != null)
            {
                if (MessageBox.Show("Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (ugeDetail_OnSaveChanges(activeDetailGrid))
                    {
                        vo.ugeCls.ClearAllRowsImages();
                        vo.ugeCls.BurnChangesDataButtons(false);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    dsDetail.RejectChanges();
                    activeDetailGrid.BurnChangesDataButtons(false);
                    activeDetailGrid.BurnRefreshDataButton(false);
                }
            }
            return true;
        }

        protected virtual void utcDetails_SelectedTabChanging(object sender, SelectedTabChangingEventArgs e)
        {
            if (DetailsSelectedTabChanging != null)
            {
                DetailsSelectedTabChanging(sender, e);
            }

            if (!TrySaveCurrentDetail())
                e.Cancel = true;
        }

        protected virtual void utcDetails_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (e.Tab == null)
                return;
            if (e.Tab.TabPage.Controls.Count == 0)
                return;
            Control ctrl = e.Tab.TabPage.Controls[0];
            if (ctrl is UltraGridEx)
                activeDetailGrid = (UltraGridEx)ctrl;
            activeDetailObject = detailEntityObjects[e.Tab.Key];

            if (DetailsSelectedTabChanged != null)
            {
                DetailsSelectedTabChanged(sender, e);
            }

            if (activeDetailObject != null)
                LoadDetailData(GetActiveMasterRowID());

            if (e.PreviousSelectedTab != null && e.PreviousSelectedTab.Tag != null)
                ((ViewSettings)e.PreviousSelectedTab.Tag).Save();
        }

        public event SelectedTabChangingEventHandler DetailsSelectedTabChanging;

        public event SelectedTabChangedEventHandler DetailsSelectedTabChanged;

        void ugeDetail_GridDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileDrop") && !InInplaceMode)
                e.Effect = DragDropEffects.Copy;
        }

        void ugeDetail_GridDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileDrop") && !InInplaceMode)
            {
                string[] files = (string[])e.Data.GetData("FileDrop");
                List<string> dropFiles = new List<string>();
                foreach (string file in files)
                {
                    FileAttributes attr = File.GetAttributes(file);
                    // если это директория 
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        dropFiles.AddRange(Directory.GetFiles(file));
                    else
                        dropFiles.Add(file);
                }
                // добавляем файлы в задачу
                Workplace.OperationObj.Text = String.Format("Добавление файлa: {0}", "...");
                Workplace.OperationObj.StartOperation();
                try
                {
                    foreach (string fileName in dropFiles)
                    {
                        // добавление записи с документом...
                        // если документов в записи несколько, документ добавляем в первый
                        AddDocumentRow(fileName, activeDetailGrid);
                    }
                }
                finally
                {
                    Workplace.OperationObj.StopOperation();
                }
            }
        }

        #endregion

        /// <summary>
        /// загрузка данных лукапа
        /// </summary>
        /// <param name="masterValue"></param>
        protected virtual void LoadDetailData(int masterValue)
        {
            if (activeDetailGrid == null)
                return;

            dsDetail = new DataSet();
            using (IDataUpdater upd = GetDetailUpdater(activeDetailObject, masterValue))
            {
                upd.Fill(ref dsDetail);

                AddBlobColumns(activeDetailObject, ref dsDetail);
                ClearCalendarData(ref dsDetail);

                LookupManager.Instance.InitLookupsCash(activeDetailObject, dsDetail);
                activeDetailGrid.DataSource = dsDetail;
                if (dsDetail.Tables[0].Rows.Count > 0)
                    activeDetailGrid.ugData.Rows[0].Activate();
                // загружаем сохраненные настройки грида
                LoadDetailPersistence();
                // после этого устанавливаем основные настройки, которые унаследованы от грида мастер-таблицы
                if (Convert.ToInt64(masterValue) == -1 || CheckAllowAddNew() == AllowAddNew.No)
                    activeDetailGrid.IsReadOnly = true;
                else
                {
                    activeDetailGrid.IsReadOnly = false;
                    SetPermissionsToDetail();
                }
                activeDetailGrid.SaveLoadFileName = vo.ugeCls.SaveLoadFileName + "_" + vo.utcDetails.ActiveTab.Text;
                AfterLoadDetailData(ref dsDetail);
            }
        }

        protected virtual void LoadDetailPersistence()
        {
            if (activeDetailGrid.Parent is UltraTabPageControl)
                ((ViewSettings)((UltraTabPageControl)activeDetailGrid.Parent).Tab.Tag).Load();
        }

    	protected virtual void AfterLoadDetailData(ref DataSet detail)
		{
		}

        /// <summary>
        /// Обновляет данные текущей детали.
        /// </summary>
        public void RefreshDetail()
        {
            LoadDetailData(GetActiveMasterRowID());
        }

        #region Возвращает табы и детали

        /// <summary>
        /// Возвращает активную деталь.
        /// </summary>
        /// <returns></returns>
        public IEntity GetDetailVisible()
        {
            return activeDetailObject;
        }

        /// <summary>
        /// Возвращает вкладку с деталью по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        public UltraTab GetDetailTab(String key)
        {
            return utcDetails.Tabs[key];
        }

        /// <summary>
        /// Возвращает активню вкладку
        /// </summary>
        /// <returns></returns>
        public UltraTab GetActiveDetailTab()
        {
            return utcDetails.ActiveTab;
        }

        /// <summary>
        /// Возвращает активный грид
        /// </summary>
        /// <returns></returns>
        public UltraGridEx GetActiveDetailGridEx()
        {
            if (GetActiveDetailTab().TabPage.Controls.Count > 0 && GetActiveDetailTab().TabPage.Controls[0] is UltraGridEx)
            {
                return (UltraGridEx)GetActiveDetailTab().TabPage.Controls[0];
            }
            return null;
        }

        /// <summary>
        /// Возвращает грид по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        public UltraGridEx GetDetailGridEx(String key)
        {
            if (utcDetails.Tabs[key].TabPage.Controls.Count > 0 && utcDetails.Tabs[key].TabPage.Controls[0] is UltraGridEx)
            {
                return (UltraGridEx)utcDetails.Tabs[key].TabPage.Controls[0];
            }
            return null;
        }
        #endregion

        /// <summary>
        /// построение и настройка всех деталей
        /// </summary>
        /// <param name="ActiveDataObj"></param>
        public void SetDetailVisible(IEntity ActiveDataObj)
        {
            if (InInplaceMode)
            {
                vo.spMasterDetail.Panel2Collapsed = true;
                isMasterDetail = false;
                return;
            }

            // создаем или очищаем списки с объектами - деталями и ссылками на мастер
            if (detailEntityObjects == null)
            {
                detailEntityObjects = new Dictionary<string, IEntity>();
                detailRefColumns = new Dictionary<string, string>();
            }
            else
            {
                detailEntityObjects.Clear();
                detailRefColumns.Clear();
            }
            // чистим все табы и гриды в них
            foreach (UltraTab tab in vo.utcDetails.Tabs)
            {
                foreach (Control ctrl in tab.TabPage.Controls)
                {
                    ctrl.Parent = null;
                    ctrl.Dispose();
                }
            }
            vo.utcDetails.Tabs.Clear();

            foreach (IEntityAssociation association in ActiveDataObj.Associated.Values)
            {
                AssociationClassTypes assType = association.AssociationClassType;
                if (assType == AssociationClassTypes.MasterDetail)
                {
                    string detailKey = association.RoleData.ObjectKey;
                    if (!CheckVisibility(detailKey))
                        continue;
                    string detailCaption = association.RoleData.Caption;
                    // действия по добавлению закладки вместе с гридом внутри нее.
                    // для каждого грида настраиваем обработчики
                    detailEntityObjects.Add(detailKey, association.RoleData);
                    detailRefColumns.Add(detailKey, association.RoleDataAttribute.Name);
                    // под каждую деталь создаем отдельную закладку
                    UltraTab tab = vo.utcDetails.Tabs.Add(detailKey, detailCaption);
                    // создаем грид
                    AttachDetaiDataComponent(tab, association);
                }
            }

            if (vo.utcDetails.Tabs.Count == 0)
            {
                // ничего нету, не показываем ничего
                vo.spMasterDetail.Panel2Collapsed = true;
                isMasterDetail = false;
            }
            else
            {
                vo.spMasterDetail.Panel2Collapsed = false;
                // получаем активный объект детали 
                activeDetailObject = detailEntityObjects[vo.utcDetails.ActiveTab.Key];
                isMasterDetail = true;
                vo.utcDetails.SelectedTab = null;
                vo.utcDetails.SelectedTab = vo.utcDetails.Tabs[0];
            }
            AfterSetDetailVisible();
        }

        protected virtual void AttachDetaiDataComponent(UltraTab tab, IEntityAssociation association)
        {
            UltraGridEx gridEx = new UltraGridEx();
            gridEx.Parent = tab.TabPage;
            gridEx.Dock = DockStyle.Fill;
            // настраиваем его как надо
            DetailGridSetup(gridEx);
            ViewSettings settings = new ViewSettings(fViewCtrl, String.Format("{0}.{1}.{2}", this.GetType().FullName, activeDataObj.ObjectKey, association.ObjectKey));
            settings.Settings.Add(new UltraGridExSettingsPartial(String.Format("{0}.{1}.{2}", this.GetType().FullName, activeDataObj.ObjectKey, association.ObjectKey), gridEx.ugData));
            tab.Tag = settings;
        }

    	/// <summary>
    	/// Если деталь с ключом key не показывать возвращаем false
    	/// </summary>
    	/// <param name="key">ключ</param>
    	/// <returns></returns>
		public virtual Boolean CheckVisibility(string key)
    	{
			return true;
    	}

        /// <summary>
        /// Вызывается после построения табой и настройки всех деталей
        /// </summary>
        public virtual void AfterSetDetailVisible()
        {

        }

        /// <summary>
        /// Срабатывает при инициализации грида детали.
        /// </summary>
        public event GridInitializeLayout DetailGridInitializeLayout;

        /// <summary>
        /// получение упдатера для получения данных
        /// </summary>
        protected virtual IDataUpdater GetDetailUpdater(IEntity activeDetailObject, object masterValue)
        {
            if (Convert.ToInt64(masterValue) >= 0)
                return activeDetailObject.GetDataUpdater(GetQueryFilter(masterValue), null, null);
            return activeDetailObject.GetDataUpdater("1 = 2", null, null);
        }

        /// <summary>
        /// Возвращает фильтровое условие по ссылке на мастер-таблицу.
        /// </summary>
        // TODO перенести в BaseDetailTableUI
        private string GetQueryFilter(object masterValue)
        {
            return String.Format("({0} = {1})", detailRefColumns[vo.utcDetails.ActiveTab.Key], masterValue);
        }

        /// <summary>
        /// устанавливаем разрешение мастера на деталь
        /// </summary>
        protected virtual void SetPermissionsToDetail()
        {
            // ставим общие параметры грида
            activeDetailGrid.AllowAddNewRecords = vo.ugeCls.AllowAddNewRecords;
            activeDetailGrid.AllowClearTable = vo.ugeCls.AllowClearTable;
            activeDetailGrid.AllowDeleteRows = vo.ugeCls.AllowDeleteRows;
            activeDetailGrid.AllowEditRows = vo.ugeCls.AllowEditRows;

            // ставим параметры в зависимости от текущей записи мастера
            if (!vo.ugeCls.AllowEditRows)
            {
                activeDetailGrid.AllowClearTable = false;
                activeDetailGrid.AllowAddNewRecords = false;
            }
        }
    }
}
