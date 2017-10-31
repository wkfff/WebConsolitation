using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Common;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Handbooks;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.References;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Server;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.PriorForecast
{
    public class PriorForecastUI : BaseViewObj
    {
        public PriorForecastUI(string key)
            : base(key)
        {
            Caption = "Предварительный прогноз";
        }

        protected override void SetViewCtrl()
        {
            fViewCtrl = new PriorForecastView();
            fViewCtrl.ViewContent = this;
        }

        protected PriorForecastView ViewObject
        {
            get; set;
        }

        public int Year
        {
            get; set;
        }

        private long NoSplitVariant
        {
            get; set;
        }

        private long SplitVariant
        {
            get; set;
        }

        private TerrType TerrType
        {
            get; set;
        }

        private PriorForecastFormParams PriorForecastFormParams
        {
            get; set;
        }

        private long SourceId
        {
            get; set;
        }

        private PriorForecastService PriorForecastService
        {
            get; set;
        }

        private DataTable PriorForecastData
        {
            get; set;
        }

        private frmModalTemplate KdForm
        {
            get; set;
        }

        private int NoSplitVariantType
        {
            get; set;
        }

        public override void Initialize()
        {
            base.Initialize();
            ViewObject = (PriorForecastView)fViewCtrl;

            var tableParams = new List<string>(new string[] { "Запись сумм в КБС (без расщепления по нормативам)",
                "Запись сумм в контингенте в КБС (для расщепления по нормативам)",
                "Запись сумм по уровням бюджета" });
            ViewObject.TableFormParams.DataSource = tableParams;
            ViewObject.TableFormParams.BeforeDropDown += TableFormParams_BeforeDropDown;
            ViewObject.TableFormParams.ValueChanged += TableFormParams_ValueChanged;
            ViewObject.TableFormParams.ActiveRow = ViewObject.TableFormParams.Rows[0];

            ViewObject.ContingentVariant.EditorButtonClick += ContingentVariant_EditorButtonClick;
            ViewObject.BudLevelsVariant.EditorButtonClick += BudLevelsVariant_EditorButtonClick;

            ViewObject.PriorForecastGrid.InitializeLayout += PriorForecastGrid_InitializeLayout;
            ViewObject.PriorForecastGrid.CellChange += PriorForecastGrid_CellChange;
            ViewObject.PriorForecastGrid.InitializeRow += PriorForecastGrid_InitializeRow;
            ViewObject.PriorForecastGrid.PreviewKeyDown += ugData_PreviewKeyDown;
            ViewObject.PriorForecastGrid.KeyUp += ugData_KeyUp;

            ViewObject.tbManager.ToolClick += tbManager_ToolClick;

            ViewObject.SplitVariantClear.Click += SplitVariantClear_Click;

            ViewObject.cbForecast.CheckedChanged += new EventHandler(cbForecast_CheckedChanged);
            ViewObject.cbEstimate.CheckedChanged += new EventHandler(cbForecast_CheckedChanged);

            Year = -1;

            TerrType =
                (TerrType) Convert.ToInt32(Workplace.ActiveScheme.GlobalConstsManager.Consts["TerrPartType"].Value);
            if (TerrType == TerrType.GO)
            {
                PriorForecastFormParams = PriorForecastFormParams.Unknown;
                ViewObject.TableFormParams.Enabled = false;
            }

            switch (TerrType)
            {
                case TerrType.MR:
                    NoSplitVariantType = 4;
                    break;
                case TerrType.SB:
                    NoSplitVariantType = 3;
                    break;
            }

            PriorForecastService = new PriorForecastService(Workplace.ActiveScheme);
            NoSplitVariant = -1;
            SplitVariant = -1;
        }

        #region настройка грида

        void ugData_KeyUp(object sender, KeyEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            switch (e.KeyValue)
            {
                case 37:// стрелка влево
                case 38:// стрелка вверх
                case 39:// стрелка вправо
                case 40:// стрелка вниз
                    grid.PerformAction(UltraGridAction.EnterEditMode);
                    break;
            }
        }

        void ugData_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            if (grid.ActiveCell == null)
                return;
            switch (e.KeyValue)
            {
                case 37:// стрелка влево
                    EmbeddableEditorBase editor = grid.ActiveCell.EditorResolved;
                    editor.ExitEditMode(false, true);
                    //grid.PerformAction(UltraGridAction.PrevCell);
                    //grid.ActiveCell.))
                    break;
                case 38:// стрелка вверх
                    editor = grid.ActiveCell.EditorResolved;
                    editor.ExitEditMode(false, true);
                    //grid.PerformAction(UltraGridAction.PageUpCell);
                    break;
                case 39:// стрелка вправо
                    editor = grid.ActiveCell.EditorResolved;
                    editor.ExitEditMode(false, true);
                    //grid.PerformAction(UltraGridAction.NextCell);
                    break;
                case 40:// стрелка вниз
                    editor = grid.ActiveCell.EditorResolved;
                    editor.ExitEditMode(false, true);
                    //grid.PerformAction(UltraGridAction.PageDownCell);
                    break;
            }
        }

        void PriorForecastGrid_CellChange(object sender, CellEventArgs e)
        {
            BurnChangeData(true);

            var calcResult = (TerrType == TerrType.MR || TerrType == TerrType.SB) &&
                             PriorForecastFormParams == PriorForecastFormParams.BudLevelsVariant;
            if (calcResult)
            {
                var cellRow = e.Cell.Row;
                string cellYear = e.Cell.Column.Key.Split('_')[1];
                string resultColumnkey = TerrType == TerrType.SB ? "KBS_" + cellYear : "KBMR_" + cellYear;
                decimal val1 = 0;
                decimal val2 = 0;
                cellRow.Update();
                if (TerrType == TerrType.SB)
                {
                    Decimal.TryParse(cellRow.Cells["OB_" + cellYear].Value.ToString(), out val1);
                    Decimal.TryParse(cellRow.Cells["KMB_" + cellYear].Value.ToString(), out val2);
                }
                if (TerrType == TerrType.MR)
                {
                    Decimal.TryParse(cellRow.Cells["MR_" + cellYear].Value.ToString(), out val1);
                    Decimal.TryParse(cellRow.Cells["POS_" + cellYear].Value.ToString(), out val2);
                }
                cellRow.Cells[resultColumnkey].Value = val1 + val2;
                cellRow.Update();
            }
        }

        void PriorForecastGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            var row = e.Row;
            if (Convert.ToBoolean(row.Cells["IsResult"].Value))
            {
                row.Activation = Activation.NoEdit;
                row.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
        }

        void PriorForecastGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            var layout = e.Layout;
            layout.Override.AllowColMoving = AllowColMoving.NotAllowed;
            layout.Override.AllowGroupMoving = AllowGroupMoving.NotAllowed;
            layout.UseFixedHeaders = true;
            layout.Bands[0].Columns["ID"].Hidden = true;
            layout.Bands[0].Columns["RefKd"].Hidden = true;
            layout.Bands[0].Columns["RefParentKd"].Hidden = true;
            layout.Bands[0].Columns["IsResult"].Hidden = true;

            var kdGroup = layout.Bands[0].Groups.Add("KD");
            kdGroup.Header.Fixed = true;
            kdGroup.Header.Caption = "Доходные источники";
            layout.Bands[0].Columns["Code"].Header.Caption = "Код";
            layout.Bands[0].Columns["Code"].Group = kdGroup;
            layout.Bands[0].Columns["Code"].Width = 140;
            layout.Bands[0].Columns["Code"].CellActivation = Activation.NoEdit;
            layout.Bands[0].Columns["Name"].Header.Caption = "Наименование";
            layout.Bands[0].Columns["Name"].Group = kdGroup;
            layout.Bands[0].Columns["Name"].Width = 240;
            layout.Bands[0].Columns["Name"].CellActivation = Activation.NoEdit;

            var calcResult = (TerrType == TerrType.MR || TerrType == TerrType.SB) &&
                             PriorForecastFormParams == PriorForecastFormParams.BudLevelsVariant;

            var groupsList = new Dictionary<string, UltraGridGroup>();
            foreach (var column in layout.Bands[0].Columns.Cast<UltraGridColumn>().Where(w => w.DataType == typeof(decimal)))
            {
                if (column.Key.Contains("_"))
                {
                    string columnKey = column.Key.Split('_')[0];
                    int year = Convert.ToInt32(column.Key.Split('_')[1]);
                    string groupName = year < Year ? "Оценка на " + year + " год" : "Прогноз на " + year + " год";
                    var group = groupsList.ContainsKey(year.ToString())
                                    ? groupsList[year.ToString()]
                                    : layout.Bands[0].Groups.Add(year.ToString());
                    group.Header.Caption = groupName;
                    if (!groupsList.ContainsKey(year.ToString()))
                        groupsList.Add(year.ToString(), group);
                    column.Group = group;
                    group.Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
                    switch (columnKey)
                    {
                        case "KBS":
                            column.Header.Caption = "КБС";
                            if (calcResult)
                            {
                                column.CellActivation = Activation.NoEdit;
                                column.CellAppearance.BackColor = Color.LightGoldenrodYellow;
                            }
                            break;
                        case "OB":
                            column.Header.Caption = "ОБ";
                            break;
                        case "KMB":
                            column.Header.Caption = "КМБ";
                            break;
                        case "KBMR":
                            column.Header.Caption = "КБМР";
                            if (calcResult)
                            {
                                column.CellActivation = Activation.NoEdit;
                                column.CellAppearance.BackColor = Color.LightGoldenrodYellow;
                            }
                            break;
                        case "MR":
                            column.Header.Caption = "Собственный бюджет МР";
                            break;
                        case "POS":
                            column.Header.Caption = "Бюджет поселения";
                            break;
                        case "GO":
                            column.Header.Caption = "Бюджет ГО";
                            break;
                    }
                    column.Width = 140;
                    column.CellMultiLine = DefaultableBoolean.False;
                    column.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                    column.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                    column.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
                    column.CellAppearance.TextHAlign = HAlign.Right;
                    column.PadChar = '_';
                    column.MaskInput = "-nnn,nnn,nnn,nnn,nnn.nn";
                }
            }
        }

        #endregion

        #region события компонентов параметров расчета

        void cbForecast_CheckedChanged(object sender, EventArgs e)
        {
            BurnRefreshData(true);
        }

        void SplitVariantClear_Click(object sender, EventArgs e)
        {
            SplitVariant = -1;
            ViewObject.BudLevelsVariant.Tag = null;
            ViewObject.BudLevelsVariant.Text = string.Empty;
        }

        void tbManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            string toolKey = e.Tool.Key;
            switch (toolKey)
            {
                case "RefreshData":
                    LoadData();
                    BurnRefreshData(false);
                    break;
                case "SaveChanges":
                    SaveData();
                    BurnChangeData(false);
                    LoadData();
                    break;
                case "CancelChanges":
                    CancelData();
                    BurnChangeData(false);
                    break;
                case "SplitData":
                    SplitData();
                    LoadData();
                    break;
                case "AddNewKd":
                    AddNewKdRows();
                    break;
            }
        }

        void TableFormParams_ValueChanged(object sender, EventArgs e)
        {
            PriorForecastFormParams = (PriorForecastFormParams)ViewObject.TableFormParams.ActiveRow.Index;
            switch (PriorForecastFormParams)
            {
                case PriorForecastFormParams.BudLevelsVariant:
                    ViewObject.ContingentVariant.Enabled = false;
                    ViewObject.BudLevelsVariant.Enabled = true;
                    ViewObject.tbManager.Tools["SplitData"].SharedProps.Enabled = false;
                    break;
                case PriorForecastFormParams.Contingent:
                    ViewObject.ContingentVariant.Enabled = false;
                    ViewObject.BudLevelsVariant.Enabled = true;
                    ViewObject.tbManager.Tools["SplitData"].SharedProps.Enabled = false;
                    break;
                case PriorForecastFormParams.ContingentSplit:
                    ViewObject.ContingentVariant.Enabled = true;
                    ViewObject.BudLevelsVariant.Enabled = true;
                    ViewObject.tbManager.Tools["SplitData"].SharedProps.Enabled = true;
                    break;
            }
            ViewObject.BudLevelsVariant.Tag = null;
            ViewObject.BudLevelsVariant.Text = string.Empty;
            ViewObject.ContingentVariant.Tag = null;
            ViewObject.ContingentVariant.Text = string.Empty;
            Year = -1;
            SplitVariant = -1;
            NoSplitVariant = -1;
        }

        void BudLevelsVariant_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            List<string> columns = new List<string>(new string[] { "ID", "Name", "RefYear", "VariantDate" });
            List<object> values = new List<object>();
            if (ChooseVariant(ObjectKeys.d_Variant_PlanIncomes, Year, -1, "Code", columns, ref values))
            {
                SplitVariant = Convert.ToInt64(values[0]);
                ViewObject.BudLevelsVariant.Text = values[1].ToString();
                if (Year == -1)
                {
                    Year = Convert.ToInt32(values[2]);
                    SourceId = PriorForecastService.GetSourceId(Year);
                }
                ViewObject.BudLevelsVariant.Tag = values[3];
                BurnRefreshData(true);
            }
        }

        void ContingentVariant_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            List<string> columns = new List<string>(new string[] { "ID", "Name", "RefYear", "VariantDate" });
            List<object> values = new List<object>();
            if (ChooseVariant(ObjectKeys.d_Variant_PlanIncomes, -1, NoSplitVariantType, "Code", columns, ref values))
            {
                NoSplitVariant = Convert.ToInt64(values[0]);
                ViewObject.ContingentVariant.Text = values[1].ToString();
                if (Year == -1)
                {
                    Year = Convert.ToInt32(values[2]);
                    SourceId = PriorForecastService.GetSourceId(Year);
                }
                ViewObject.ContingentVariant.Tag = values[3];
                BurnRefreshData(true);
            }
        }

        void TableFormParams_BeforeDropDown(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UltraCombo combo = (UltraCombo)sender;
            combo.DisplayLayout.Bands[0].ColHeadersVisible = false;
        }

        #endregion

        #region выбор значений из справочников

        private bool ChooseVariant(string clsKey, int year, int variantType, string codeName, List<string> columns, ref List<object> values)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();
            IEntity cls = ConsBudgetForecastNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(clsKey);
            BaseClsUI clsUI = new IncomesVariantCls(cls);
            clsUI.Workplace = ConsBudgetForecastNavigation.Instance.Workplace;
            if (variantType != -1)
                clsUI.AdditionalFilter = " and RefVarD = " + variantType;
            if (year != -1)
                clsUI.AdditionalFilter = " and RefYear = " + year;

            clsUI.Initialize();
            clsUI.InitModalCls(-1);
            clsUI.RefreshAttachedData();
            tmpClsForm.SuspendLayout();
            try
            {
                tmpClsForm.AttachCls(clsUI);
                ComponentCustomizer.CustomizeInfragisticsControls(tmpClsForm);
            }
            finally
            {
                tmpClsForm.ResumeLayout();
            }
            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns[codeName].SortIndicator =
                SortIndicator.Ascending;
            if (tmpClsForm.ShowDialog(Workplace.WindowHandle) == DialogResult.OK)
            {
                UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(clsUI.UltraGridExComponent.ugData);
                foreach (string columnName in columns)
                {
                    values.Add(activeRow.Cells[columnName].Value);
                }
                return true;
            }
            return false;
        }

        private bool GetPlaningKDCodes(string filter, ref DataTable dtPlaningKD, ref frmModalTemplate form)
        {
            UltraGrid ultraGrid = null;
            if (form == null)
            {
                IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(ObjectKeys.d_KD_PlanIncomes);
                DataClsUI planingKdUI = new SelectRowsClsUI(entity, Year);
                // получаем нужный классификатор
                // создаем объект просмотра классификаторов нужного типа
                planingKdUI.AdditionalFilter = filter;
                planingKdUI.Workplace = Workplace;
                planingKdUI.RestoreDataSet = false;
                planingKdUI.Initialize();
                planingKdUI.InitModalCls(-1);
                // создаем форму
                form = new frmModalTemplate();
                form.AttachCls(planingKdUI);
                planingKdUI.RefreshAttachedData();
                ComponentCustomizer.CustomizeInfragisticsControls(form);
                // ...загружаем данные
                foreach (UltraGridBand band in planingKdUI.UltraGridExComponent.ugData.DisplayLayout.Bands)
                {
                    band.Columns["CodeStr_Remasked"].SortIndicator = SortIndicator.Ascending;
                }
                ultraGrid = planingKdUI.UltraGridExComponent.ugData;
            }
            else
                ultraGrid = form.AttachedCls.UltraGridExComponent.ugData;

            if (form.ShowDialog(Workplace.WindowHandle) == DialogResult.OK)
            {
                if (ultraGrid.ActiveRow != null)
                    ultraGrid.ActiveRow.Update();
                dtPlaningKD = form.AttachedCls.GetClsDataSet().Tables[0];
                return true;
            }
            return false;
        }

        private void AddNewKdRows()
        {
            var kdData = new DataTable();
            var kdForm = KdForm;
            if (GetPlaningKDCodes(string.Empty, ref kdData, ref kdForm))
            {
                foreach (DataRow row in kdData.Select("SelectedRow = true", "ParentID desc"))
                {
                    var rows = PriorForecastData.Select(string.Format("RefKD = {0}", row["ID"]));
                    if (rows.Length == 0)
                    {
                        var newRow = PriorForecastData.NewRow();

                        newRow["RefKd"] = row["ID"];
                        newRow["RefParentKd"] = row["ParentID"];
                        newRow["Code"] = row["CodeStr"];
                        newRow["Name"] = row["Name"];
                        newRow["IsResult"] =
                            PriorForecastData.Select(string.Format("RefParentKd = {0}", row["ID"])).Length > 0;
                        PriorForecastData.Rows.Add(newRow);
                    }
                }
                KdForm = kdForm;
            }
        }

        /// <summary>
        /// наименование варианта проекта доходов по id
        /// </summary>
        /// <param name="variantId"></param>
        /// <returns></returns>
        private string GetVariantCaption(long variantId)
        {
            IEntity variantEntity = Workplace.ActiveScheme.RootPackage.FindEntityByName(ObjectKeys.d_Variant_PlanIncomes);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return db.ExecQuery(string.Format("select Name from {0} where id = ?", variantEntity.FullDBName),
                             QueryResultTypes.Scalar, new DbParameterDescriptor("p0", variantId)).ToString();
            }
        }

        #endregion

        #region загрузка и сохранение данных

        private PriorForecastParams GetDataParams()
        {
            var calcParams = new PriorForecastParams();
            calcParams.Estimate = ViewObject.cbEstimate.Checked;
            calcParams.Forecast = ViewObject.cbForecast.Checked;
            calcParams.TerrType = TerrType;
            calcParams.BudLevelVariant = SplitVariant;
            calcParams.ContingentVariant = NoSplitVariant;
            calcParams.SourceId = SourceId;
            calcParams.Year = Year;
            calcParams.PriorForecastFormParams = PriorForecastFormParams;
            return calcParams;
        }

        private void LoadData()
        {
            switch (PriorForecastFormParams)
            {
                case PriorForecastFormParams.BudLevelsVariant:
                case PriorForecastFormParams.Contingent:
                    if (SplitVariant == -1)
                    {
                        MessageBox.Show("Для отображения данных необходимо выбрать вариант проекта доходов по уровням бюджета",
                                        "Загрузка данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    break;
                case PriorForecastFormParams.ContingentSplit:
                    if (NoSplitVariant == -1)
                    {
                        MessageBox.Show("Для отображения данных необходимо выбрать вариант в контингенте",
                                        "Загрузка данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    /*if (SplitVariant == -1)
                    {
                        MessageBox.Show("Для отображения данных необходимо выбрать вариант по уровням бюджета",
                                        "Загрузка данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }*/
                    break;
            } 

            try
            {
                Workplace.OperationObj.Text = "Обработка и получение данных";
                Workplace.OperationObj.StartOperation();
                var priorForecastParams = GetDataParams();
                PriorForecastData = PriorForecastService.GetPriorForecastData(priorForecastParams);
                ViewObject.PriorForecastGrid.DataSource = null;
                ViewObject.PriorForecastGrid.DataSource = PriorForecastData;
                Workplace.OperationObj.StopOperation();
            }
            catch
            {
                Workplace.OperationObj.StopOperation();
                throw;
            }
        }

        private void SaveData()
        {
            try
            {
                Workplace.OperationObj.Text = "Сохранение изменений";
                Workplace.OperationObj.StartOperation();
                if (ViewObject.PriorForecastGrid.ActiveRow != null)
                    ViewObject.PriorForecastGrid.ActiveRow.Update();
                var priorForecastParams = GetDataParams();
                PriorForecastService.SaveData(priorForecastParams, PriorForecastData);
                PriorForecastData.AcceptChanges();
                ViewObject.PriorForecastGrid.DataSource = null;
                ViewObject.PriorForecastGrid.DataSource = PriorForecastData;
                Workplace.OperationObj.StopOperation();
            }
            catch (Exception)
            {
                Workplace.OperationObj.StopOperation();
                throw;
            }
        }

        private void CancelData()
        {
            if (ViewObject.PriorForecastGrid.ActiveRow != null)
                ViewObject.PriorForecastGrid.ActiveRow.Update();
            PriorForecastData.RejectChanges();
            ViewObject.PriorForecastGrid.DataSource = null;
            ViewObject.PriorForecastGrid.DataSource = PriorForecastData;
        }

        private void SplitData()
        {
            try
            {
                Workplace.OperationObj.Text = "Расщепление данных";
                Workplace.OperationObj.StartOperation();
                var splitVariant = PriorForecastService.SplitData((int)NoSplitVariant, NoSplitVariantType, (int)SplitVariant, Year);
                ViewObject.BudLevelsVariant.Text = GetVariantCaption(splitVariant);
                Workplace.OperationObj.StopOperation();
            }
            catch (Exception)
            {
                Workplace.OperationObj.StopOperation();
                throw;
            }
        }

        #endregion

        #region подсветка кнопочек тулбара

        private void BurnRefreshData(bool burn)
        {
            InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["RefreshData"], burn);
        }

        private void BurnChangeData(bool burn)
        {
            InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["CancelChanges"], burn);
            InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["SaveChanges"], burn);
        }

        #endregion
    }
}
