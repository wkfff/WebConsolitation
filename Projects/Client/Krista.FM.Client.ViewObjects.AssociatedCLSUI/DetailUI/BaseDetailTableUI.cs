using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Configuration;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DetailUI;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
	/// <summary>
	/// 
	/// </summary>
    public class BaseDetailTableUI : BaseClsUI
	{
		private IEntityAssociation entityAssociation;
	    private IEntity detailEntity;
        private DetailViewObject viewControl;


		public BaseDetailTableUI(IEntityAssociation entityAssociation)
            : base(entityAssociation.RoleBridge, entityAssociation.ObjectKey)
		{
			this.entityAssociation = entityAssociation;
		}

        protected override void SetViewCtrl()
        {
            fViewCtrl = new DetailViewObject();
            fViewCtrl.ViewContent = this;
        }

	    public DetailViewObject ViewObject
	    {
            get { return viewControl; }
	    }

	    public override void Initialize()
		{
            viewControl = (DetailViewObject)fViewCtrl;

            ViewObject.DetailGrid.StateRowEnable = true;
            ViewObject.DetailGrid.ServerFilterEnabled = false;

	        ViewObject.DetailGrid.OnCheckLookupValue += ugeDetail_OnCheckLookupValue;
	        ViewObject.DetailGrid.OnGetLookupValue += ugeDetail_OnGetLookupValue;

            ViewObject.DetailGrid.OnGetHierarchyInfo += ugeDetail_OnGetHierarchyInfo;
            ViewObject.DetailGrid.OnSaveChanges += ugeDetail_OnSaveChanges;
            ViewObject.DetailGrid.OnRefreshData += ugeDetail_OnRefreshData;
            ViewObject.DetailGrid.OnClearCurrentTable += ugeDetail_OnClearCurrentTable;
            ViewObject.DetailGrid.OnCancelChanges += ugeDetail_OnCancelChanges;
            ViewObject.DetailGrid.OnGetGridColumnsState += ugeDetail_OnGetGridColumnsState;
            ViewObject.DetailGrid.OnInitializeRow += ugeDetail_OnInitializeRow;
            ViewObject.DetailGrid.ugData.AfterCellUpdate += ugDetailData_AfterCellUpdate;
            ViewObject.DetailGrid.ToolClick += ugeDetail_ToolClick;
            ViewObject.DetailGrid.OnClickCellButton += ugeDetail_OnClickCellButton;
            ViewObject.DetailGrid.OnAfterRowInsert += ugeDetail_OnAfterRowInsert;

            ViewObject.DetailGrid.OnBeforeRowDeactivate += ugeDetail_OnBeforeRowDeactivate;

            ViewObject.DetailGrid.OnGridInitializeLayout += ugeDetail_OnGridInitializeLayout;

            ViewObject.DetailGrid.OnMouseEnterGridElement += ugeCls_OnMouseEnterGridElement;
            ViewObject.DetailGrid.OnMouseLeaveGridElement += ugeCls_OnMouseLeaveGridElement;

            ViewObject.DetailGrid.utmMain.Tools["menuLoad"].SharedProps.Visible = false;
            ViewObject.DetailGrid.utmMain.Tools["menuSave"].SharedProps.Visible = false;

            // настройки для кнопок копировать/вставить
            ViewObject.DetailGrid.utmMain.Tools["CopyRow"].SharedProps.Visible = true;
            ViewObject.DetailGrid.utmMain.Tools["PasteRow"].SharedProps.Visible = true;

            ViewObject.DetailGrid.GridDragDrop += ugeDetail_GridDragDrop;
            ViewObject.DetailGrid.GridDragEnter += ugeDetail_GridDragEnter;

            InfragisticComponentsCustomize.CustomizeUltraGridParams(ViewObject.DetailGrid.ugData);
            ViewObject.DetailGrid.PerfomAction("ShowGroupBy");

            ugeDetail_OnCreateGrid(ViewObject.DetailGrid);

            ViewObject.DetailGrid.ugData.MouseClick += ugDetailData_MouseClick;
		}

		public override string Key
		{
			get { return entityAssociation.ObjectKey; }
		}

        public bool InInplaceMode
        {
            get; set;
        }

		public override System.Windows.Forms.Control Control
		{
			get { return viewControl; }
		}

		public string Caption
		{
			get { return entityAssociation.RoleBridge.FullCaption; }
		}

        public DataSet DetailData
        {
            get; set;
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

        #region настройка внешнего вида грида

        private Dictionary<string, GridColumnsStates> cashedColumnsSettings =
            new Dictionary<string, GridColumnsStates>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private GridColumnsStates GetColumnStatesFromClsObject(IEntity clsObject, UltraGridEx gridEx)
        {
            // проверяем не были ли параметры закэшированы
            string objKey = String.Format("{0}.{1}", clsObject.ObjectKey, GetCurrentPresentation(clsObject));

            if (cashedColumnsSettings.ContainsKey(objKey))
            {
                return cashedColumnsSettings[objKey];
            }

            string documentColumnName = string.Empty;
            // если нет - создаем и инициализируем
            GridColumnsStates states = new GridColumnsStates();
            int attrPosition = 1;

            IDataAttributeCollection attributeCollection;
            attributeCollection = clsObject.Attributes;

            foreach (IDataAttribute item in attributeCollection.Values)
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
                state.ColumnPosition = attrPosition;
                // указываем группу аттрибута и то, является ли аттрибут в группе первым
                if (!string.IsNullOrEmpty(groupName))
                {
                    groupName = groupName.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    state.GroupName = groupName;
                    if (!gridEx.Groups.ContainsKey(groupName))
                    {
                        gridEx.Groups.Add(groupName, state.ColumnName);
                        state.FirstInGroup = true;
                    }
                }

                string lookupObjectName = String.Empty;
                state.IsLookUp = AttrIsLookup(clsObject, attr, ref lookupObjectName);

                switch (attrkind)
                {
                    case DataAttributeKindTypes.Regular:
                        state.ColumnType = UltraGridEx.ColumnType.Standart;
                        break;
                    case DataAttributeKindTypes.Sys:
                        state.ColumnType = UltraGridEx.ColumnType.System;
                        state.IsSystem = true;
                        break;
                    case DataAttributeKindTypes.Serviced:
                        state.ColumnType = UltraGridEx.ColumnType.Service;
                        state.IsSystem = true;
                        break;
                }

                if ((attrClass == DataAttributeClassTypes.Reference))
                {
                    IEntity bridgeCls = GetBridgeClsByRefName(clsObject, attr.ObjectKey);
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

                            // ... показываем колонку
                            if (!state.IsHiden)
                                state.IsHiden = InInplaceMode && state.ColumnType != UltraGridEx.ColumnType.Standart;
                            // ... цепляем к ней кнопку вызова модального справочника
                            // ... устанавливаем заголовок (он определен ранее)
                            state.ColumnCaption = referenceCaption;

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

        public static IEntity GetBridgeClsByRefName(IEntity activeDataObject, string refName)
        {
            IAssociationCollection assCol = (IAssociationCollection)(activeDataObject).Associations;

            if (assCol.ContainsKey(refName))
            {
                return assCol[refName].RoleBridge;
            }
            return null;
        }

        /// <summary>
        /// Разыменовка семантической принадлежности объекта
        /// </summary>
        /// <param name="cmnObj">объект</param>
        /// <returns>русское название семантической принадлежности</returns>
        public string GetDataObjSemanticRus(IEntity cmnObj)
        {
            return cmnObj.SemanticCaption;
        }

        /// <summary>
        /// Возвращает из контекста ключ активного представления структуры
        /// </summary>
        /// <returns></returns>
        private string GetCurrentPresentation(IEntity clsObject)
        {
            if (clsObject.Presentations.Count != 0)
            {
                LogicalCallContextData context = LogicalCallContextData.GetContext();
                if (context[String.Format("{0}.Presentation", clsObject.FullDBName)] != null)
                    return Convert.ToString(context[String.Format("{0}.Presentation", clsObject.FullDBName)]);
            }

            return Guid.Empty.ToString();
        }

        /// <summary>
        /// Явлется ли ссылочный аттрибут лукапом? (для инициализации грида)
        /// </summary>
        /// <param name="dataObj">Активный объект-поставщик данных</param>
        /// <param name="referenceAttr">Обрабатываемый аттрибут</param>
        /// <returns>true/false</returns>
        public static bool AttrIsLookup(IEntity dataObj, IDataAttribute referenceAttr, ref string lookupObjectName)
        {
            // если аттрибут не ссылка - он и не лукап
            if (referenceAttr.Class != DataAttributeClassTypes.Reference)
                return false;

            // получаем классификатор на который ссылаемся
            IEntity cls = BaseClsUI.GetBridgeClsByRefName(dataObj, referenceAttr.ObjectKey);
            lookupObjectName = cls.ObjectKey;

            // если у классификатора есть хотя бы одно поле для лукапа - исходный аттрибут является лукапом
            foreach (IDataAttribute attr in cls.Attributes.Values)
            {
                if ((attr.LookupType == LookupAttributeTypes.Primary) || (attr.LookupType == LookupAttributeTypes.Secondary))
                    return true;
            }

            // если все аттрибуты перебраны - исходный аттрибут не лукап
            return false;
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

        #endregion

        #region события грида

        HierarchyInfo ugeDetail_OnGetHierarchyInfo(object sender)
        {
            return GetHierarchyInfoFromClsObject(detailEntity, (UltraGridEx)sender);
        }

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

        public virtual HierarchyInfo GetHierarchyInfoFromClsObject(IEntity clsObject, UltraGridEx gridEx)
        {
            HierarchyInfo newHrInfo = new HierarchyInfo();
            newHrInfo.LevelsNames = new string[] {clsObject.Caption};
            newHrInfo.LevelsCount = 1;
            newHrInfo.FlatLevelName = newHrInfo.LevelsNames[0];
            gridEx.SingleBandLevelName = newHrInfo.LevelsNames[0];
            return newHrInfo;
        }

        protected virtual bool SaveDetailData(UltraGridEx gridEx)
        {
            // в случае если в деталях нет таблиц, вернем тру
            if (DetailData.Tables.Count < 1) return true;

            bool SucceessSaveChanges = true;

            if (activeDetailObject != null)
            {
                if (gridEx.ugData.ActiveCell != null)
                    if (gridEx.ugData.ActiveCell.IsInEditMode)
                        gridEx.ugData.ActiveCell = gridEx.ugData.ActiveRow.Cells[0];

                if (gridEx.ugData.ActiveRow != null)
                    gridEx.ugData.ActiveRow.Update();

                // сделаем проверку на корректность данных
                string sringError = string.Empty;
                if (ValidateDataTable(dsDetail.Tables[0], ref sringError, GetColumnStatesFromClsObject(detailEntity, gridEx)))
                {
                    Workplace.OperationObj.Text = "Сохранение изменений";
                    Workplace.OperationObj.StartOperation();
                    gridEx.ugData.BeginUpdate();
                    // получаем датасет с измененными записями
                    IDataUpdater upd = null;
                    try
                    {
                        upd = GetDetailUpdater(detailEntity, GetActiveMasterRowID());
                        // сохраняем измененные ( а так же удаленные и добавленные) записи
                        DataSet dsChanges = DetailData.GetChanges();
                        if (dsChanges != null)
                        {
                            try
                            {
                                // сохраняем основные данные
                                upd.Update(ref dsChanges);
                                // сохраняем документы
                                SaveAllDocuments(DetailData, gridEx.CurrentStates, detailEntity.FullDBName, true);
                                // применение всех изменений в источнике данных
                                DetailData.BeginInit();
                                DetailData.Tables[0].BeginLoadData();
                                DetailData.Tables[0].AcceptChanges();
                                DetailData.Tables[0].EndLoadData();
                                DetailData.EndInit();
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
                        gridEx.ugData.EndUpdate();
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

        void ugeDetail_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow tmpRow = UltraGridHelper.GetRowCells(e.Row);
            if (tmpRow.Cells["ID"].Value == DBNull.Value || tmpRow.Cells["ID"].Value == null)
                return;

            SetDocumentRow(tmpRow, activeDetailGrid.CurrentStates);

            InitializeDetailRow(sender, e);
        }

        void ugeDetail_ToolClick(object sender, ToolClickEventArgs e)
        {

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
                GridColumnsStates cs = GetColumnStatesFromClsObject(activeDetailObject, ActiveDetailGrid);
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

        void ugeDetail_OnBeforeRowDeactivate(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UltraGridRow row = UltraGridHelper.GetActiveRowCells(activeDetailGrid.ugData);
            if (!row.Cells.Exists("ID")) return;
            int id = Convert.ToInt32(row.Cells["ID"].Value);
            DataRow[] dataRows = dsDetail.Tables[0].Select(string.Format("ID = {0}", id));
            if (dataRows.Length == 0) return;
        }

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

        /// <summary>
        /// обработчик клика мыши на гриде с данными
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugDetailData_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (!InInplaceMode && ActiveGridElementIsRow)
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

	    #endregion
    }
}
