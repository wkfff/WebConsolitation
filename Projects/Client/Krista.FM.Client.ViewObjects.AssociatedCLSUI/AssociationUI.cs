using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;

using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Infragistics.Excel;
using Krista.FM.Client.Workplace.Gui;


namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Association
{
    public partial class AssociationUI : BaseViewObj
    {
        // объекты отображения сопоставления
        private AssociationView associateView;
        // текущая ассоциация
        private IBridgeAssociation curentAssociation = null;
        // тулбары левой и правой частей сопоставления
        private UltraToolbarsManager toolBar = null;
        private UltraToolbarsManager bridgeToolBar = null;
        // показывают, являются ли иерархическими 
        private bool IsattClsDataHierarchy = true;
        // объекты классификатора данных
        //attClsData attClsData = null;
        private IInplaceClsView attClsData = null;
        private UltraGrid clsDataGrid = null;
        // объекты сопоставимого классификатора
        //attClsData attClsBridge = null;
        private IInplaceClsView attClsBridge = null;
        private UltraGrid clsBridgeGrid = null;
        // фильтр для классификатора данных
        private object FilterCondition = string.Empty;
        // параметры перехода на сопоставление
        private object[] activeAssociationParams = null;

        private const int developerRowDiapazon = 1000000000;

        private string associationName = string.Empty;

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.cls_Associate_16.GetHicon()); }
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_Associate_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_Associate_24; }
        }


        public AssociationUI(string key)
            : base(key)
        {
            Index = 3;
            Caption = "Сопоставление классификаторов";
        }

        protected override void SetViewCtrl()
        {
            fViewCtrl = new AssociationView();
            associateView = (AssociationView)fViewCtrl;
        }

        /// <summary>
        ///  Инициализация
        /// </summary>
        public override void Initialize()
        {
            curentAssociation = GetActiveAssociation();

            base.Initialize();

            associateView.utbmAssociate.ToolClick += utbmAssociate_ToolClick;
            associateView.rbAllRecords.CheckedChanged += rbAllRecords_CheckedChanged;
            associateView.rbCurrentAssociate.CheckedChanged += rbCurrentAssociate_CheckedChanged;
            associateView.rbUnAssociate.CheckedChanged += rbUnAssociate_CheckedChanged;
            associateView.ultraTabControl1.ActiveTabChanged += ultraTabControl1_ActiveTabChanged;
            associateView.rbAllRecords.Checked = true;

            ViewCtrl.Text = curentAssociation.FullCaption;

            ReloadData();
        }

        /// <summary>
        /// обработка тулбара с кнопкой "Обновить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void utbRefreshData_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "RefreshData":
                    ReloadData();
                    break;
                default:
                    break;
            }
        }

        private string captionString;

        /// <summary>
        /// скрывает ссылки не текущего сопоставления
        /// </summary>
        private void HideNoAssociationRef()
        {
            List<string> refClmns = new List<string>();
            // получаем все ссылки классификатора на сопоставимые
            foreach (IDataAttribute attr in attClsData.ActiveDataObj.Attributes.Values)
            {
                if (attr.Class == DataAttributeClassTypes.Reference)
                    refClmns.Add(attr.Name);
            }
            // показываем колонку со ссылкой на текущий сопоставимый классификатор
            associationName = curentAssociation.FullDBName;

            string associateColumnName = associationName + UltraGridEx.LOOKUP_COLUMN_POSTFIX;
            if (!attClsData.UltraGridExComponent.utmMain.Tools.Exists(associateColumnName))
                associateColumnName = associationName;
            ((StateButtonTool)attClsData.UltraGridExComponent.utmMain.Tools[associateColumnName]).Checked = true;
        }

        void rbUnAssociate_CheckedChanged(object sender, EventArgs e)
        {
            ShowNoAssociate();
        }

        void ShowNoAssociate()
        {
            if (associateView.rbUnAssociate.Checked)
            {
                HierarchyInfo hi = attClsData.GetHierarchyInfo(attClsData.UltraGridExComponent);
                hi.CurViewState = ViewState.Flat;
                LoadMode loadMode = hi.loadMode;
                hi.loadMode = LoadMode.AllRows;
                ViewUnAssciatedRows();
                attClsData.RefreshAttachedData();
                attClsData.UltraGridExComponent.SetHierarchyFilter(UltraGridEx.FilterConditionAction.Clear);
                hi.loadMode = loadMode;
            }
        }

        void rbCurrentAssociate_CheckedChanged(object sender, EventArgs e)
        {
            ShowCurrentAssociate();
        }

        void ShowCurrentAssociate()
        {
            if (associateView.rbCurrentAssociate.Checked)
            {
                HierarchyInfo hi = attClsData.GetHierarchyInfo(attClsData.UltraGridExComponent);
                hi.CurViewState = ViewState.Flat;
                LoadMode loadMode = hi.loadMode;
                hi.loadMode = LoadMode.AllRows;
                ViewCurrentAssociatedRow();
                attClsData.RefreshAttachedData();
                attClsData.UltraGridExComponent.SetHierarchyFilter(UltraGridEx.FilterConditionAction.Clear);
                hi.loadMode = loadMode;
            }
        }

        /// <summary>
        ///  Обработчик выбора фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void rbAllRecords_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                //SetFilter();
                ShowAllRows();
            }
        }

        private void ShowAllRows()
        {
            HierarchyInfo hi = attClsData.GetHierarchyInfo(attClsData.UltraGridExComponent);
            hi.CurViewState = hi.LevelsCount == 1 ? ViewState.Flat : ViewState.Hierarchy;
            attClsData.AdditionalFilter = string.Empty;
            attClsData.RefreshAttachedData();
        }

        //private bool isSetFilter;

        /// <summary>
        ///  Установка фильтра в зависимости от выбранного фильтра
        /// </summary>
        private void SetFilter()
        {
            /*
            StateButtonTool btn = (StateButtonTool)toolBar.Tools["ShowHierarchy"];
            // Проверяем, какой же всетаки фильтр установлен, т.е. какой RadioButton выбран
            // фильтр стандартный, показывает все записи в классификаторе данных
            if (associateView.rbAllRecords.Checked)
            {
                // убираем принудительную установку в
                HierarchyInfo hi = attClsData.UltraGridExComponent.HierarchyInfo;
                btn.SharedProps.Enabled = hi.LevelsCount > 1;
                //FilterCondition = null;
                //clsDataGrid.DisplayLayout.Bands[0].ColumnFilters[curentAssociation.FullDBName].FilterConditions.Clear();
                isSetFilter = false;
            }
            // фильтр, который показывает несопоставленные записи
            if (associateView.rbUnAssociate.Checked)
            {
                //attClsData.UltraGridExComponent.SetFilter(curentAssociation.FullDBName, -1);
                isSetFilter = true;
            }
            // фильтр, который по записи в сопоставимом показывает записи, которые ему сопоставлены
            if (associateView.rbCurrentAssociate.Checked)
            {
                
                if (clsBridgeGrid.Rows.Count >= 0 && clsBridgeGrid.ActiveRow == null)
                    clsBridgeGrid.Rows[0].Activate();
                if (clsBridgeGrid.Rows.Count == 0)
                {
                    MessageBox.Show("Установка фильтра невозможна, сопоставимый классификатор пуст", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    associateView.rbAllRecords.Checked = true;
                    return;
                }

                FilterCondition = clsBridgeGrid.ActiveRow.Cells["ID"].Value;
                attClsData.UltraGridExComponent.SetFilter(curentAssociation.FullDBName, FilterCondition);
                 * 
                isSetFilter = true;
            }*/
        }

        private void ViewUnAssciatedRows()
        {
            attClsData.AdditionalFilter = string.Format(" and {0} = -1", curentAssociation.FullDBName);
        }

        private void ViewCurrentAssociatedRow()
        {
            attClsData.AdditionalFilter = clsBridgeGrid.ActiveRow != null ?
                string.Format(" and {0} = {1}", curentAssociation.FullDBName, clsBridgeGrid.ActiveRow.Cells["ID"].Value) :
                string.Format(" and {0} is NULL", curentAssociation.FullDBName);
        }

        public override void Activate(bool Activated)
        {
            if (Activated)
            {
                Workplace.ViewObjectCaption = captionString;
                attClsData.TrySetDataSource(DataSourceContext.CurrentDataSourceID);
            }
        }

        /// <summary>
        ///  Получение текущей ассоциации
        /// </summary>
        /// <returns></returns>
        private IBridgeAssociation GetActiveAssociation()
        {
            IEntityAssociation entityAssociation = Workplace.ActiveScheme.RootPackage.FindAssociationByName(Key);
            if (entityAssociation is IBridgeAssociation)
                return entityAssociation as IBridgeAssociation;
            throw new InvalidOperationException(String.Format("Объект с ключом ObjectKey={0} не соответствует типу IBridgeAssociation.", entityAssociation.ObjectKey));
        }

        /// <summary>
        /// Наполнение интерфейса данными.
        /// </summary>
        public override void InitializeData()
        {
            #region установка видимости кнопок работы с сопоставлением

            ButtonTool btn1 = (ButtonTool)associateView.utbmAssociate.Tools["CreateBridge"];
            btn1.SharedProps.Enabled = curentAssociation.MappingRuleExist;

            CheckPermissions();
            SetPervissionsToAssociation();

            btn1 = (ButtonTool)associateView.utbmAssociate.Tools["AddToBridge"];
            btn1.SharedProps.Enabled = curentAssociation.MappingRuleExist;

            btn1 = (ButtonTool) associateView.utbmAssociate.Tools["AddToBridgeAll"];
            btn1.SharedProps.Visible = curentAssociation is IBridgeAssociationReport ? true : false;

            #endregion


            // добавляем классификаторы как объекты в интерфейс сопоставления
            AttachClsClassifiers();

            attClsData.UltraGridExComponent.ugData.BeginUpdate();
            attClsBridge.UltraGridExComponent.ugData.BeginUpdate();
            // обновляем данные в классификаторах
            attClsData.RefreshAttachedData();
            attClsBridge.RefreshAttachedData();

            StateButtonTool btn = (StateButtonTool)toolBar.Tools["ShowHierarchy"];
            IsattClsDataHierarchy = btn.SharedProps.Enabled;
            // показываем количество всех и несопоставленных записей в классификаторе данных
            SetAssociateCount();
            // устанавливаеам фильтр на все записи
            associateView.rbAllRecords.Checked = true;
            // показываем полное название сопоставления
            captionString = string.Format("Сопоставление: {0}.{1} -> {2}.{3}", CommonMethods.GetDataObjSemanticRus(this.Workplace.ActiveScheme.Semantics, curentAssociation.RoleData), curentAssociation.RoleData.Caption,
                CommonMethods.GetDataObjSemanticRus(this.Workplace.ActiveScheme.Semantics, curentAssociation.RoleBridge), curentAssociation.RoleBridge.Caption);
            this.Workplace.ViewObjectCaption = captionString;

            this.Workplace.OperationObj.Text = "Инициализация данных";
            this.Workplace.OperationObj.StartOperation();
            if (attClsData.UltraGridExComponent.ugData.Rows.Count > 0)
                attClsData.UltraGridExComponent.ugData.Rows[0].Activate();


            attClsData.UltraGridExComponent.ugData.EndUpdate();
            attClsBridge.UltraGridExComponent.ugData.EndUpdate();

            // прячем все ссылки на сопоставимый кроме той, которая относится к сопоставлению
            HideNoAssociationRef();

            TranslationTablesPageLoad(associateView.ultraTabControl1.ActiveTab);

            this.Workplace.OperationObj.StopOperation();
        }

        #region РеализациЯ внедрения классификаторов

        /// <summary>
        /// очищает компонент от внедренного классификатора
        /// </summary>
        /// <param name="CLSUI"></param>
        private void ClearAttachedCLS(IInplaceClsView CLSUI)
        {
            CLSUI.DetachViewObject();
            CLSUI.FinalizeViewObject();
            CLSUI = null;
        }

        /// <summary>
        ///  получение классификаторов с гридами, отображение данных классификаторов
        /// </summary>
        private void AttachClsClassifiers()
        {
            // Создаем 2 объекта с классификаторами. 
            // Сопоставимый в правую часть, а классификатор данных в левую часть панели

            // очищаем предыдущий классификатор данных
            if (attClsData != null)
                ClearAttachedCLS(attClsData);
            // Получаем классификатор данных
            IEntity clsData = curentAssociation.RoleData;
            attClsData = this.Workplace.GetClsView(clsData);
            //attClsData = attClsData.ProtocolsInplacer;
            attClsData.AttachCls(associateView.pDataCls, ref attClsData);

            // Подменяем  некоторые обработчики грида и прочего
            toolBar = attClsData.GetClsToolBar();

            attClsData.SelectDataSource += new VoidDelegate(attClsData_SelectDataSource);
            attClsData.RefreshData += new VoidDelegate(attClsData_RefreshData);

            clsDataGrid = attClsData.UltraGridExComponent.ugData;
            clsDataGrid.AfterRowActivate += new EventHandler(DataGrid_AfterRowActivate);
            clsDataGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(clsDataGrid_InitializeRow);
            clsDataGrid.SelectionDrag += new System.ComponentModel.CancelEventHandler(clsDataGrid_SelectionDrag);

            // Получаем сопоставимый классификатор
            // удаляем предыдущий сопоставимый классификатор
            if (attClsBridge != null)
                ClearAttachedCLS(attClsBridge);

            attClsBridge = this.Workplace.GetClsView(curentAssociation.RoleBridge);
            attClsBridge.AttachCls(associateView.scAssociation.Panel2, ref attClsBridge);


            clsBridgeGrid = attClsBridge.UltraGridExComponent.ugData;
            bridgeToolBar = attClsBridge.GetClsToolBar();
            // Обработчики для Drag&Drop
            clsBridgeGrid.AfterRowActivate += new EventHandler(BridgeGrid_AfterRowActivate);
            clsBridgeGrid.DragDrop += new DragEventHandler(clsDataGrid_DragDrop);
            clsBridgeGrid.DragEnter += new DragEventHandler(clsDataGrid_DragEnter);
            clsBridgeGrid.DragOver += new DragEventHandler(clsDataGrid_DragOver);
        }

        void attClsData_SelectDataSource()
        {
            ((IBridgeAssociation)curentAssociation).RefreshRecordsCount();
            SetAssociateCount();
            //ShowCurrentAssociate();
            if (associateView.rbCurrentAssociate.Checked || associateView.rbUnAssociate.Checked)
            {
                HierarchyInfo hi = attClsData.GetHierarchyInfo(attClsData.UltraGridExComponent);
                hi.CurViewState = ViewState.Hierarchy;
                hi.loadMode = LoadMode.AllRows;
                attClsData.UltraGridExComponent.PerfomAction("ShowHierarchy");
            }
        }

        void attClsData_RefreshData()
        {
            curentAssociation.RefreshRecordsCount();
            SetAssociateCount();
            HideNoAssociationRef();

            if (associateView.rbCurrentAssociate.Checked || associateView.rbUnAssociate.Checked)
            {
                HierarchyInfo hi = attClsData.GetHierarchyInfo(attClsData.UltraGridExComponent);
                hi.CurViewState = ViewState.Hierarchy;
                hi.loadMode = LoadMode.AllRows;
                attClsData.UltraGridExComponent.PerfomAction("ShowHierarchy");
            }
        }

        /// <summary>
        /// перерасчет количества записей после выбора нового источника
        /// </summary>
        void attClsData_OnAfterSourceSelect()
        {
            SetAssociateCount();
            HideNoAssociationRef();
        }

        /// <summary>
        /// перерасчет количества записей после обновления данных классификатора данных
        /// </summary>
        void attClsData_OnAfterRefreshData()
        {
            curentAssociation.RefreshRecordsCount();
            SetAssociateCount();
            HideNoAssociationRef();
        }

        #endregion

        #region РеализациЯ Drag&Drop

        UltraGridRow DragDropRow = null;

        UltraGridRow LastEnteredRow = null;

        void clsDataGrid_DragOver(object sender, DragEventArgs e)
        {

            e.Effect = DragDropEffects.Move;
            UltraGridRow tmpRow = GetRowFromPos((UltraGrid)sender, e.X, e.Y);
            if (tmpRow != LastEnteredRow)
            {
                ResetROwColor(LastEnteredRow, (UltraGrid)sender);
                LastEnteredRow = tmpRow;
                SetRowColor(LastEnteredRow);
            }
        }

        void clsDataGrid_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        bool needRefresh = false;

        /// <summary>
        ///  Сам Drag&Drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clsDataGrid_DragDrop(object sender, DragEventArgs e)
        {
            UltraGridRow tmpRow = GetRowFromPos((UltraGrid)sender, e.X, e.Y);
            if (tmpRow != LastEnteredRow && tmpRow != DragDropRow)
            {
                ResetROwColor(LastEnteredRow, (UltraGrid)sender);
                LastEnteredRow = tmpRow;
            }
            if (LastEnteredRow != null)
            {
                if (MessageBox.Show("Сопоставить запись классификатора данных с записью сопоставимого классификатора?", "Сопоставление записи", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    clsDataGrid.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                    try
                    {
                        DragDropRow.Cells[curentAssociation.FullDBName].Value = LastEnteredRow.Cells["ID"].Value;
                    }
                    finally
                    {
                        clsDataGrid.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                    }
                    DragDropRow.Update();
                    LastEnteredRow.Activate();
                    needRefresh = false;
                }
            }
            else
                if (MessageBox.Show("Добавить запись в сопоставимый классификатор?", "Добавление записи", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    try
                    {
                        curentAssociation.CopyAndAssociateRow(Convert.ToInt32(DragDropRow.Cells["ID"].Value));
                        DragDropRow.Update();
                        needRefresh = true;
                    }
                    catch
                    {
                        throw;
                    }
        }

        /// <summary>
        ///  Начинаем Drag&Drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clsDataGrid_SelectionDrag(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!allowAddRecordsToBridgeCls) return;
            DragDropRow = clsDataGrid.ActiveRow;
            if (Convert.ToInt32(DragDropRow.Cells["ID"].Value) >= developerRowDiapazon && !this.Workplace.IsDeveloperMode)
                return;
            clsDataGrid.DoDragDrop(DragDropRow, DragDropEffects.Move);
            e.Cancel = true;
            DragDropRow.Refresh(RefreshRow.FireInitializeRow);
            ResetROwColor(LastEnteredRow, (UltraGrid)sender);
            DragDropRow = null;
            LastEnteredRow = null;
            SaveChanges();
            if (needRefresh)
            {
                attClsData.RefreshAttachedData();
                attClsBridge.RefreshAttachedData();
            }
            SetAssociateCount();
        }

        void clsDataGrid_DragLeave(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///  Сбрасывает все настройки цвета записи в стандартные
        /// </summary>
        /// <param name="row"></param>
        /// <param name="grid"></param>
        void ResetROwColor(UltraGridRow row, UltraGrid grid)
        {
            if (row != null)
            {
                row.Appearance.AlphaLevel = grid.DisplayLayout.Override.RowPreviewAppearance.AlphaLevel;
                row.Appearance.BackColor2 = grid.DisplayLayout.Override.RowPreviewAppearance.BackColor2;
                row.Appearance.BackHatchStyle = grid.DisplayLayout.Override.RowPreviewAppearance.BackHatchStyle;
            }
        }

        void SetRowColor(UltraGridRow row)
        {
            if (row == null) return;
            row.Appearance.AlphaLevel = 150;
            row.Appearance.BackColor2 = Color.Red;
            row.Appearance.BackHatchStyle = BackHatchStyle.Percent50;
        }

        /// <summary>
        /// Получить UltraGridRow по экранным координатам. Опрашиваются и потомки.
        /// </summary>
        private UltraGridRow GetRowFromPos(UltraGrid grid, int X, int Y)
        {
            Point pt = new Point(X, Y);
            pt = grid.PointToClient(pt);
            UIElement elem = grid.DisplayLayout.UIElement.ElementFromPoint(pt);
            return GetRowFromElement(elem);
        }

        /// <summary>
        /// Получить UltraGridRow от UIElement. Опрашиваются и потомки.
        /// </summary>
        /// <param name="elem">элемент</param>
        /// <returns>строка</returns>
        private UltraGridRow GetRowFromElement(UIElement elem)
        {
            UltraGridRow row = null;

            row = (UltraGridRow)elem.GetContext(typeof(UltraGridRow), true);

            return row;
        }
        #endregion

        /// <summary>
        ///  Подкрашивает несопоставленные записи в классификаторе данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clsDataGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            string str = curentAssociation.FullDBName;
            UltraGridRow row = e.Row;
            if (row.Cells[str].Value == DBNull.Value ||
                Convert.ToInt32(row.Cells[str].Value) == -1)
            {
                //row.Appearance.BackColor = Color.Salmon;
                foreach (UltraGridCell cell in row.Cells)
                {
                    if (cell.Appearance.BackColor != Color.Empty)
                    {
                        cell.Appearance.BackColor2 = Color.Salmon;
                        cell.Appearance.AlphaLevel = 250;
                        cell.Appearance.BackHatchStyle = BackHatchStyle.SmallCheckerBoard;
                    }
                    else
                        cell.Appearance.BackColor = Color.Salmon;
                }
            }
            else
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    if (cell.Appearance.BackColor != Color.Salmon)
                    {
                        cell.Appearance.ResetBackColor2();
                    }
                    else
                        cell.Appearance.ResetBackColor();
                }
            }
        }

        /// <summary>
        ///  Обработчик перемещения по гриду с классификатором данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DataGrid_AfterRowActivate(object sender, EventArgs e)
        {
            if (CurrentClsBridgeRow != null)
            {
                // сбрасываем цвета с предыдущей записи
                //CurrentClsBridgeRow.Appearance.ResetBackColor();
                foreach (UltraGridCell cell in CurrentClsBridgeRow.Cells)
                {
                    if (cell.Appearance.BackColor != Color.YellowGreen)
                    {
                        cell.Appearance.ResetBackColor2();
                    }
                    else
                        cell.Appearance.ResetBackColor();
                }
                CurrentClsBridgeRow.Selected = false;
                CurrentClsBridgeRow.Refresh();
            }
            CurrentClsBridgeRow = null;
        }

        void BridgeGrid_AfterRowActivate(object sender, EventArgs e)
        {
            if (associateView.rbCurrentAssociate.Checked)
                ShowCurrentAssociate();
                //SetFilter();
        }

        public override void InternalFinalize()
        {
            attClsData.FinalizeViewObject();
            attClsData = null;
            attClsBridge.FinalizeViewObject();
            attClsBridge = null;
            attClsData = null;
            attClsBridge = null;
            clsBridgeGrid = null;
            clsDataGrid = null;

            base.InternalFinalize();
        }

        /// <summary>
        ///  Сохранение всех изменений
        /// </summary>
        public override void SaveChanges()
        {
            attClsBridge.SaveChanges();
            attClsData.SaveChanges();
        }

        /// <summary>
        ///  Отмена всех изменений
        /// </summary>
        public override void CancelChanges()
        {
            
        }

        /// <summary>
        ///  Получение данных о количестве всех записей и сопоставленных записей
        /// </summary>
        private void SetAssociateCount()
        {
            associateView.lAllDataCls.Text = string.Format("Всего записей: {0}", curentAssociation.GetRecordsCountByCurrentDataSource(attClsData.CurrentSourceID));
            associateView.lNotClsData.Text = string.Format("Не сопоставлено: {0}", curentAssociation.GetUnassociateRecordsCountByCurrentDataSource(attClsData.CurrentSourceID));

            associateView.lAllDataClsAllSources.Text = string.Format("по всем источникам: {0}", curentAssociation.GetAllRecordsCount());
            associateView.lUnAssociateAllSources.Text = string.Format("по всем источникам: {0}", curentAssociation.GetAllUnassociateRecordsCount());
        }

        /// <summary>
        ///  Подмененный обработчик для тулбара. Вызывается при выборе источника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolBar_AfterToolCloseup(object sender, ToolDropdownEventArgs e)
        {
            attClsData.DataSourceSelected(sender, e);
            SetAssociateCount();
            SetFilter();
        }

        #region получение отчета и сохранение его

        private int dataClsVisibleColumnsCount = 0;

        private int bridgeClsVisibleColumnsCount = 0;

        private void GetExcelReport()
        {
            // соствление запроса на получение данных по сопоставлению по текущему источнику
            List<string> FieldsCaptions = new List<string>();
            string fieldsNames = string.Empty;
            GetColumnsNamesAndCaptions(ref fieldsNames, ref FieldsCaptions);
            string query = "select " + fieldsNames;
            query = query + string.Format(" from {0} dataCls, {1} bridgeCls where ((dataCls.{2} = bridgeCls.{3}) or ({2} is null))",
                curentAssociation.RoleData.FullDBName, curentAssociation.RoleBridge.FullDBName,
                curentAssociation.FullDBName, "ID");

            string fileName = this.captionString.Replace(">", string.Empty) + ".xls";
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, true, ref fileName))
            {
                this.Workplace.OperationObj.Text = "Построение отчета";
                this.Workplace.OperationObj.StartOperation();
                Infragistics.Excel.Workbook wb = new Infragistics.Excel.Workbook();
                UltraGridExcelExporter excelExpoter = new UltraGridExcelExporter();
                excelExpoter.FileLimitBehaviour = FileLimitBehaviour.TruncateData;
                UltraGrid tmpGrid = new UltraGrid();
                tmpGrid.Parent = associateView.panel1;
                tmpGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(tmpGrid_InitializeRow);
                tmpGrid.Visible = true;
                InfragisticComponentsCustomize.CustomizeUltraGridParams(tmpGrid);
                // получаем все источники данных по классификатору
                Dictionary<int, string> dataSources = ((IClassifier)curentAssociation.RoleData).GetDataSourcesNames();

                try
                {
                    // проходим по всем источникам, берем данные и записываем в отдельную заклдку
                    foreach (KeyValuePair<int, string> kvp in dataSources)
                    {
                        // получение и обработка данных
                        string filter = string.Format(" and dataCls.RowType = 0 and dataCls.SourceID = {0} order by dataCls.{1}",
                            kvp.Key, curentAssociation.FullDBName);
                        DataTable dt = GetAssociationData(query + filter);
                        if (dt != null)
                        {
                            tmpGrid.DataSource = dt;

                            for (int i = 0; i <= FieldsCaptions.Count - 1; i++)
                                tmpGrid.DisplayLayout.Bands[0].Columns[i].Header.Caption = FieldsCaptions[i];

                            Infragistics.Excel.Worksheet ws = wb.Worksheets.Add(string.Format("SourceID={0}", kvp.Key));

                            Infragistics.Excel.WorksheetRow excelRow = ws.Rows[0];

                            WorksheetMergedCellsRegion reg = ws.MergedCellsRegions.Add(2, 0, 2, dataClsVisibleColumnsCount - 1);
                            reg.Value = DataClsCaption;

                            reg.CellFormat.RightBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.TopBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.Alignment = HorizontalCellAlignment.Center;

                            reg = ws.MergedCellsRegions.Add(2, dataClsVisibleColumnsCount, 2, dataClsVisibleColumnsCount + bridgeClsVisibleColumnsCount - 1);
                            reg.Value = BridgeClsCaption;

                            reg.CellFormat.RightBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.TopBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.Alignment = HorizontalCellAlignment.Center;

                            excelRow.Cells[0].Value = kvp.Value;
                            excelRow.Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                            excelExpoter.Export(tmpGrid, ws, 3, 0);

                            for (int i = 3; i <= tmpGrid.Rows.Count + 3; i++)
                            {
                                WorksheetRow row = ws.Rows[i];
                                row.CellFormat.WrapText = ExcelDefaultableBoolean.True;
                                row.Cells[dataClsVisibleColumnsCount - 1].CellFormat.RightBorderStyle = CellBorderLineStyle.Thick;
                                row.CellFormat.ShrinkToFit = ExcelDefaultableBoolean.True;
                            }
                            tmpGrid.DataSource = null;
                        }
                    }
                    if (wb.Worksheets.Count == 0)
                        wb.Worksheets.Add("Нету данных");
                    //wb.Save(fileName);
                    Infragistics.Excel.BIFF8Writer.WriteWorkbookToFile(wb, fileName);
                }
                finally
                {
                    excelExpoter.Dispose();
                    tmpGrid.Parent = null;
                    this.Workplace.OperationObj.StopOperation();
                }
            }
        }

        /// <summary>
        /// получение данных по сопоставлению. Будет перенесен на сервеную часть
        /// </summary>
        /// <param name="SelectDataQuery">запрос получения данных</param>
        /// <returns></returns>
        DataTable GetAssociationData(string SelectDataQuery)
        {
            IDatabase db = this.Workplace.ActiveScheme.SchemeDWH.DB;
            DataTable dt = null;
            try
            {
                dt = (DataTable)db.ExecQuery(SelectDataQuery, QueryResultTypes.DataTable);
            }
            finally
            {
                db.Dispose();
            }
            return dt;
        }

        private string DataClsCaption;
        private string BridgeClsCaption;

        /// <summary>
        ///  получение строки с именами полей для запроса и списка русских заголовков колонок
        /// </summary>
        /// <returns></returns>
        void GetColumnsNamesAndCaptions(ref string columnsNames, ref List<string> columnsCaptions)
        {
            columnsCaptions.Clear();
            // названия классификаторов в сопоставлении
            // название классификатора данных
            DataClsCaption = curentAssociation.RoleData.FullCaption;
            // название сопоставимого классификатора
            BridgeClsCaption = curentAssociation.RoleBridge.FullCaption;

            // получаем список полей классификатора данных
            Dictionary<int, string> DataClsFieldDictionary = new Dictionary<int, string>();
            //int i = 1;
            foreach (UltraGridColumn clmn in clsDataGrid.DisplayLayout.Bands[0].Columns)
            {
                if (clmn.Hidden == false && clmn.Header.Caption != string.Empty)
                    DataClsFieldDictionary.Add(clmn.Header.VisiblePosition, string.Format("dataCls.{0}", CC.UltraGridEx.GetSourceColumnName(clmn.Key)));
            }
            List<string> DataClsFields = new List<string>();
            string val = string.Empty;
            for (int i = 0; i <= 50; i++)
            {
                DataClsFieldDictionary.TryGetValue(i, out val);
                if (val != null)
                    DataClsFields.Add(val);
            }

            // если скрыта колонка ссылки на сопоставимый, добавим ее 
            if (!DataClsFields.Contains(string.Format("dataCls.{0}", curentAssociation.FullDBName.ToUpper())))
                DataClsFields.Add(string.Format("dataCls.{0}", curentAssociation.FullDBName));
            // получаем заголовки для колонок классификатора данных
            List<string> DataClsFieldsCaptions = new List<string>();
            GetColumnsCaptions(clsDataGrid, DataClsFields, ref DataClsFieldsCaptions);
            // добавляем к заголовкам колонок название классификатора
            foreach (string columnCaption in DataClsFieldsCaptions)
            {
                columnsCaptions.Add(columnCaption);// + "." + columnCaption);
            }
            dataClsVisibleColumnsCount = DataClsFieldsCaptions.Count;
            // получаем список полей сопоставимого классификатора
            Dictionary<int, string> BridgeClsFieldsDictionary = new Dictionary<int, string>();
            //i = 1;
            foreach (UltraGridColumn clmn in clsBridgeGrid.DisplayLayout.Bands[0].Columns)
            {
                if (clmn.Hidden == false && clmn.Header.Caption != string.Empty)
                    BridgeClsFieldsDictionary.Add(clmn.Header.VisiblePosition, string.Format("bridgeCls.{0}", CC.UltraGridEx.GetSourceColumnName(clmn.Key)));
            }

            List<string> BridgeClsFields = new List<string>();
            val = string.Empty;
            for (int i = 0; i <= 50; i++)
            {
                BridgeClsFieldsDictionary.TryGetValue(i, out val);
                if (val != null)
                    BridgeClsFields.Add(val);
            }

            // получаем заголовки для колонок сопоставимого классификатора
            List<string> BridgeClsFieldsCaptions = new List<string>();
            GetColumnsCaptions(clsBridgeGrid, BridgeClsFields, ref BridgeClsFieldsCaptions);
            // добавляем к заголовкам колонок название классификатора
            foreach (string columnCaption in BridgeClsFieldsCaptions)
            {
                columnsCaptions.Add(columnCaption);// + "." + columnCaption);
            }
            bridgeClsVisibleColumnsCount = BridgeClsFieldsCaptions.Count;
            string retutnValue = String.Join(", ", DataClsFields.ToArray());
            retutnValue = String.Concat(retutnValue, ", ");
            retutnValue = String.Concat(retutnValue, String.Join(", ", BridgeClsFields.ToArray()));
            columnsNames = retutnValue;
            columnsNames = columnsNames.Replace("_Remasked", string.Empty);
        }

        /// <summary>
        /// возвращает русские наименования для колонок
        /// </summary>
        /// <returns></returns>
        void GetColumnsCaptions(UltraGrid grid, List<string> columnsNames, ref List<string> Captions)
        {
            // получаем все видимые атрибуты полей
            string str = string.Empty;
            foreach (string clmnName in columnsNames)
            {
                str = clmnName.Split('.')[1];
                Captions.Add(grid.DisplayLayout.Bands[0].Columns[str].Header.Caption);
            }
        }

        /// <summary>
        /// красит в цвет те записи, которые не сопоставлены
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tmpGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            string str = curentAssociation.FullDBName;
            UltraGridRow Row = e.Row;
            if (Row.Cells[str].Value == DBNull.Value ||
                Convert.ToInt32(Row.Cells[str].Value) == -1)
            {
                Row.Appearance.AlphaLevel = 150;
                Row.Appearance.BackColor = Color.Salmon;
            }
        }
        #endregion

        #region проверка прав и установка их же

        private bool allowAssociate = false;
        private bool allowClearAssociate = false;
        private bool allowAddRecordsToBridgeCls = false;
        private bool allowDelRecordsFromBridgeCls = false;

        /// <summary>
        /// получаем права, которые установлены на текущее сопоставление
        /// </summary>
        private void CheckPermissions()
        {
            string objectKey = this.curentAssociation.ObjectKey;

            IUsersManager um = this.Workplace.ActiveScheme.UsersManager;
            // проверим общие права на все сопоставления
            allowAssociate = um.CheckPermissionForSystemObject(objectKey, (int)AssociateForAllClassifiersOperations.Associate, false);
            allowClearAssociate = um.CheckPermissionForSystemObject(objectKey, (int)AssociateForAllClassifiersOperations.ClearAssociate, false);
            allowAddRecordsToBridgeCls = um.CheckPermissionForSystemObject(objectKey, (int)AssociateForAllClassifiersOperations.AddRecordIntoBridgeTable, false);
            allowDelRecordsFromBridgeCls = um.CheckPermissionForSystemObject(objectKey, (int)AssociateForAllClassifiersOperations.DelRecordFromBridgeTable, false);

            // проверим права на конкретное сопоставление
            if (!allowAssociate)
                allowAssociate = um.CheckPermissionForSystemObject(objectKey, (int)AssociateOperations.Associate, false);
            if (!allowClearAssociate)
                allowClearAssociate = um.CheckPermissionForSystemObject(objectKey, (int)AssociateOperations.ClearAssociate, false);
            if (!allowAddRecordsToBridgeCls)
                allowAddRecordsToBridgeCls = um.CheckPermissionForSystemObject(objectKey, (int)AssociateOperations.AddRecordIntoBridgeTable, false);
            if (!allowDelRecordsFromBridgeCls)
                allowDelRecordsFromBridgeCls = um.CheckPermissionForSystemObject(objectKey, (int)AssociateOperations.DelRecordFromBridgeTable, false);
        }

        /// <summary>
        /// В зависимости от прав дизейблит различные кнопочки
        /// </summary>
        private void SetPervissionsToAssociation()
        {
            ButtonTool btnAssociateMaster1 = (ButtonTool)associateView.utbmAssociate.Tools["AssociateMaster"];
            ButtonTool btnAssociateMaster2 = (ButtonTool)associateView.utbmAssociate.Tools["HandAssociate"];
            ButtonTool btnClearAssociateRef = (ButtonTool)associateView.utbmAssociate.Tools["ClearAllBridgeRef"];
            ButtonTool btnClearCurrentAssociateRef = (ButtonTool)associateView.utbmAssociate.Tools["ClearCurentBridgeRef"];
            ButtonTool btnAddRowToBridge = (ButtonTool)associateView.utbmAssociate.Tools["AddToBridge"];

            bool canAssociate = curentAssociation.AssociateRules.Count > 0;
            if (allowAssociate && canAssociate)
            {
                btnAssociateMaster1.SharedProps.Enabled = true;
                btnAssociateMaster2.SharedProps.Enabled = true;
            }
            else
            {
                btnAssociateMaster1.SharedProps.Enabled = false;
                btnAssociateMaster2.SharedProps.Enabled = false;
            }

            if (allowClearAssociate)
            {
                btnClearAssociateRef.SharedProps.Enabled = true;
                btnClearCurrentAssociateRef.SharedProps.Enabled = true;
            }
            else
            {
                btnClearAssociateRef.SharedProps.Enabled = false;
                btnClearCurrentAssociateRef.SharedProps.Enabled = false;
            }

            if (allowDelRecordsFromBridgeCls)
            {
                btnAddRowToBridge.SharedProps.Enabled = true;
            }
            else
            {
                btnAddRowToBridge.SharedProps.Enabled = false;
            }
        }
        #endregion
    }
}