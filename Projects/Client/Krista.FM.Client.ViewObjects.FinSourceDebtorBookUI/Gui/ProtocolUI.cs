using System;
using System.Drawing;
using Infragistics.Win;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.ServerLibrary;
using System.Data;
using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI
{
    public class ProtocolUI : BaseClsUI
    {
        private DataTable dtRegions;
        private ControlsOnGroupByCreationFilter creationFilter;

        public ProtocolUI(IEntity entity)
            : base(entity)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            creationFilter = new ControlsOnGroupByCreationFilter(Workplace.ActiveScheme, RefreshGridData);

            vo.ugeCls.ServerFilterEnabled = false;
            vo.ugeCls.ugData.AfterCellUpdate += new CellEventHandler(ugData_AfterCellUpdate);
            vo.ugeCls.OnCreateUIElement += new CreateUIElement(ugeCls_OnCreateUIElement);
            vo.ugeCls.ugData.InitializeGroupByRow += new InitializeGroupByRowEventHandler(ugData_InitializeGroupByRow);
        }

        void ugData_InitializeGroupByRow(object sender, InitializeGroupByRowEventArgs e)
        {
            string description = string.Format("{0} ({1})", e.Row.Rows[0].Cells["RegionName"].Value, e.Row.Rows[0].Cells["RegionCode"].Value);

            e.Row.Description = description;
        }
        private void RefreshGridData()
        {
            Refresh();
        }

        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            DataRow activeRow = GetActiveDataRow();
            if (string.Compare(e.Cell.Column.Key, "CheckTransfer", true) == 0)
            {
                if (!Convert.ToBoolean(e.Cell.Value))
                    activeRow["RefStatusSchb"] = 1;
                else
                    activeRow["RefStatusSchb"] = 2;
            }
            if (string.Compare(e.Cell.Column.Key, "RefStatusSchb", true) == 0)
            {
                int dataState = Convert.ToInt32(e.Cell.Value);
                activeRow["CheckTransfer"] = dataState == 1 ? false : true;
            }
        }

        public override void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.ugeCls_OnGridInitializeLayout(sender, e);
            foreach (UltraGridBand band in e.Layout.Bands)
            {
                if (!band.Columns.Exists("RegionName"))
                {
                    UltraGridColumn column = band.Columns.Add("RegionName");
                    column.Header.Caption = "Район";
                    column.Hidden = true;
                    column.CellActivation = Activation.NoEdit;
                }

                if (!band.Columns.Exists("RegionCode"))
                {
                    UltraGridColumn column = band.Columns.Add("RegionCode");
                    column.Header.Caption = "Код";
                    //column.Hidden = true;
                    column.CellActivation = Activation.NoEdit;
                }

                band.Columns["ID"].CellActivation = Activation.NoEdit;
            }
            //Group a column in the grid. 
            UltraGridBand gband = e.Layout.Bands[0];
            e.Layout.GroupByBox.Hidden = true;
            gband.Columns["RegionName"].Header.Enabled = false;
            vo.ugeCls.utmMain.Tools["ShowGroupBy"].SharedProps.Visible = false;

            e.Layout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
            e.Layout.Bands[0].SortedColumns.Add(gband.Columns["RegionCode"], false, true);
            e.Layout.Bands[0].SortedColumns.Add(gband.Columns["ID"], true, false);
            e.Layout.Bands[0].Columns["RegionCode"].SortIndicator = SortIndicator.Ascending;
        }

        public override void ugeCls_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            base.ugeCls_OnInitializeRow(sender, e);
            if (e.Row.Band.Columns.Exists("RegionName") && e.Row.Cells["RegionName"].Value == null)
            {
                DataRow[] rows = dtRegions.Select(string.Format("ID = {0}", e.Row.Cells["RefRegion"].Value));
                if (rows != null && rows.Length > 0)
                {
                    e.Row.Cells["RegionName"].Value = rows[0]["Name"];
                    e.Row.Cells["RegionCode"].Value = rows[0]["Code"];
                }
            }

            UltraGridRow activeRow = e.Row;
            activeRow.Cells["CheckTransfer"].Hidden = true;
            activeRow.Cells["RefStatusSchb" + UltraGridEx.LOOKUP_COLUMN_POSTFIX].Activation = Activation.Disabled;
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            dataQuery = "RefVariant = ?";

            AddFilter();
            filterStr = dataQuery;

            return ActiveDataObj.GetDataUpdater(dataQuery, null, 
                new DbParameterDescriptor("Variant", DebtBookNavigation.Instance.CurrentVariantID));
        }
        
        protected override void LoadData(object sender, EventArgs e)
        {
            using (IDatabase db = DebtBookNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                dtRegions = (DataTable)db.ExecQuery(
                    "select ID, Code, Name from d_Regions_Analysis where SourceID = ? and (RefTerr = 4 or RefTerr = 7)",
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("SourceID", DebtBookNavigation.Instance.CurrentAnalizSourceID));
            }
            base.LoadData(sender, e);

            vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns["RegionCode"].SortIndicator = SortIndicator.Ascending;
        }

        public override void SetPermissionsToClassifier(UltraGridEx gridEx)
        {
            ((BaseClsView)ViewCtrl).ugeCls.ServerFilterEnabled = false;
            ((BaseClsView)ViewCtrl).ugeCls.AllowAddNewRecords = false;
            ((BaseClsView)ViewCtrl).ugeCls.AllowDeleteRows = false;
            ((BaseClsView)ViewCtrl).ugeCls.AllowEditRows = true;
            ((BaseClsView)ViewCtrl).ugeCls.IsReadOnly = false;
        }

        protected override void ugeCls_OnAfterRowActivate(object sender, EventArgs e)
        {
            // делаем невозможным поставить галку о передаче данных. Только возможность ее снять
            base.ugeCls_OnAfterRowActivate(sender, e);
            UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells((UltraGrid) sender);
            if (!Convert.ToBoolean(activeRow.Cells["CheckTransfer"].Value))
                activeRow.Cells["CheckTransfer"].Activation = Activation.Disabled;
            else
                activeRow.Cells["CheckTransfer"].Activation = Activation.AllowEdit;

            if (Convert.ToInt32(activeRow.Cells["RefStatusSchb"].Value) == 1)
                activeRow.Cells["RefStatusSchb" + UltraGridEx.LOOKUP_COLUMN_POSTFIX].Activation = Activation.Disabled;
            else
                activeRow.Cells["RefStatusSchb" + UltraGridEx.LOOKUP_COLUMN_POSTFIX].Activation = Activation.AllowEdit;
        }

        void ugeCls_OnCreateUIElement(object sender, UIElement parent)
        {
            creationFilter.AfterCreateChildElements(parent);
        }

        public class ControlsOnGroupByCreationFilter : IUIElementCreationFilter
        {
            public delegate void RefreshGridHandler();
            private event RefreshGridHandler RefreshGrid;
            private readonly IScheme scheme;

            public ControlsOnGroupByCreationFilter(IScheme scheme, RefreshGridHandler refreshGridHandler)
            {
                this.scheme = scheme;
                RefreshGrid = refreshGridHandler;
            }

            public bool BeforeCreateChildElements(UIElement parent)  
            {
                return false;
            }

            public void AfterCreateChildElements(UIElement parent) 
            {
                if (parent is GroupByRowDescriptionUIElement)
                {
                    DependentTextUIElement aDependentTextUIElement = (DependentTextUIElement)parent
                        .GetDescendant(typeof(DependentTextUIElement));

                    if (aDependentTextUIElement == null) return;

                    ((GroupByRowDescriptionUIElement)parent).GroupByRow.Height = 21;
                    object obj = ((GroupByRowDescriptionUIElement)parent).GroupByRow.Rows[0].Cells["RefStatusSchb"].Value;
                    int status = Convert.ToInt32(obj);

                    ImageUIElement imageUIElement;
                    ImageAndTextButtonUIElement rejectButtonUIElement;
                    switch (status)
                    {
                        case 2:
                            imageUIElement = new ImageUIElement(parent, Properties.Resources.FlagOrange);

                            ImageAndTextButtonUIElement applyButtonUIElement = new ImageAndTextButtonUIElement(parent);
                            parent.ChildElements.Add(applyButtonUIElement);
                            applyButtonUIElement.Rect = new Rectangle(parent.Rect.X + 20, parent.Rect.Y, parent.Rect.Height + 2, parent.Rect.Height + 1);
                            applyButtonUIElement.Image = Properties.Resources.Accept;
                            applyButtonUIElement.ElementClick += AcceptButtonUIElementElementClick;

                            rejectButtonUIElement = new ImageAndTextButtonUIElement(parent);
                            parent.ChildElements.Add(rejectButtonUIElement);
                            rejectButtonUIElement.Rect = new Rectangle(parent.Rect.X + 40, parent.Rect.Y, parent.Rect.Height + 2, parent.Rect.Height + 1);
                            rejectButtonUIElement.Image = Properties.Resources.Decline;
                            rejectButtonUIElement.ElementClick += RejectButtonUIElementElementClick;
                            break;
                        case 3:
                            imageUIElement = new ImageUIElement(parent, Properties.Resources.FlagGreen);

                            rejectButtonUIElement = new ImageAndTextButtonUIElement(parent);
                            parent.ChildElements.Add(rejectButtonUIElement);
                            rejectButtonUIElement.Rect = new Rectangle(parent.Rect.X + 20, parent.Rect.Y, parent.Rect.Height + 2, parent.Rect.Height + 1);
                            rejectButtonUIElement.Image = Properties.Resources.Decline;
                            rejectButtonUIElement.ElementClick += RejectButtonUIElementElementClick;
                            break;
                        default:
                            imageUIElement = new ImageUIElement(parent, Properties.Resources.FlagWhite);
                            break;
                    }

                    parent.ChildElements.Add(imageUIElement);
                    imageUIElement.Rect = new Rectangle(parent.Rect.X + 3, parent.Rect.Y, parent.Rect.Height, parent.Rect.Height);

                    //Сдвигаем DependentTextUIElement вправо для того чтобы освободить
                    //место для наших контролов.
                    aDependentTextUIElement.Rect = new Rectangle(imageUIElement.Rect.Right + 3 + 34, aDependentTextUIElement.Rect.Y, parent.Rect.Width - imageUIElement.Rect.Right, aDependentTextUIElement.Rect.Height);
                }
            }

            private void AcceptButtonUIElementElementClick(object sender, UIElementEventArgs e)
            {
                ImageAndTextButtonUIElement element = (ImageAndTextButtonUIElement)e.Element;

                //Получаем GroupByRow связанную с текущим элементом
                UltraGridGroupByRow groupByRow = (UltraGridGroupByRow)element.GetAncestor(typeof(GroupByRowUIElement)).GetContext(typeof(UltraGridGroupByRow));

                int regionId = Convert.ToInt32(groupByRow.Rows[0].Cells["RefRegion"].Value);
                int variantId = Convert.ToInt32(groupByRow.Rows[0].Cells["RefVariant"].Value);

                SetState(3, regionId, variantId);

                RefreshGrid();
            }

            private void RejectButtonUIElementElementClick(object sender, UIElementEventArgs e)
            {
                ImageAndTextButtonUIElement element = (ImageAndTextButtonUIElement)e.Element;

                //Получаем GroupByRow связанную с текущим элементом
                UltraGridGroupByRow groupByRow = (UltraGridGroupByRow)element.GetAncestor(typeof(GroupByRowUIElement)).GetContext(typeof(UltraGridGroupByRow));

                int regionId = Convert.ToInt32(groupByRow.Rows[0].Cells["RefRegion"].Value);
                int variantId = Convert.ToInt32(groupByRow.Rows[0].Cells["RefVariant"].Value);

                SetState(1, regionId, variantId);

                RefreshGrid();
            }

            /// <summary>
            /// Установка состояния варианта района
            /// </summary>
            private void SetState(int state, int regionId, int variantId)
            {
                IEntity protocoEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_ProtocolTransfer);
                using (IDatabase db = scheme.SchemeDWH.DB)
                {
                    db.ExecQuery("insert into t_S_ProtocolTransfer (ID, DataTransfer, RefStatusSchb, RefRegion, RefVariant) values (?, ?, ?, ?, ?)",
                        QueryResultTypes.NonQuery,
                        new DbParameterDescriptor("ID", protocoEntity.GetGeneratorNextValue),
                        new DbParameterDescriptor("DataTransfer", DateTime.Now, DbType.DateTime),
                        new DbParameterDescriptor("state", state),
                        new DbParameterDescriptor("regionId", regionId),
                        new DbParameterDescriptor("variantId", variantId));
                }
            }
        }
    }
}
