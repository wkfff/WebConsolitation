using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common
{
    public partial class ControlSystemInfo : UserControl
    {
        private IScheme scheme;

        private Form mainForm;

        public Form MainForm
        {
            get { return mainForm; }
            set { mainForm = value; }
        }

        public ControlSystemInfo()
        {
            InitializeComponent();
        }

        public void InitializeGrid(IScheme scheme)
        {
            this.scheme = scheme;

            try
            {
                DisableUI();

                SuspectGrid.InitializeLayout += new InitializeLayoutEventHandler(InitializeLayout);
                AllGrid.InitializeLayout += new InitializeLayoutEventHandler(InitializeLayout);
                PatchGrid.InitializeLayout += new InitializeLayoutEventHandler(InitializeLayout);

                StartSecondThread();

                // пользовательские настройки
                InfragisticComponentsCustomize.CustomizeUltraGridParams(AllGrid);
                InfragisticComponentsCustomize.CustomizeUltraGridParams(PatchGrid);
                InfragisticComponentsCustomize.CustomizeUltraGridParams(SuspectGrid);
                InfragisticComponentsCustomize.CustomizeUltraTabControl(utcDetails);

                // макс. глубина иерахии. 
                SuspectGrid.DisplayLayout.MaxBandDepth = 1;
                PatchGrid.DisplayLayout.MaxBandDepth = 1;

                AllGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
                SuspectGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
                PatchGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

                //this.utcDetails.TA;
            }
            catch (Exception e)
            {
                Forms.FormException.ShowErrorForm(e);
            }
        }       

        private void StartSecondThread()
        {
            Thread thread = new Thread(new ParameterizedThreadStart(StartInformationTread));
            try
            {
                // собираем в новом потоке
                // поток с параметром
                thread.Start();
            }
            catch(Exception e)
            {
                Forms.FormException.ShowErrorForm(e);
            }
            finally
            {
                thread = null;
            }
        }
        /// <summary>
        /// Обработка дополнительного потока
        /// </summary>
        /// <param name="parametr"></param>
        public void StartInformationTread(object parametr)
        {
            Krista.FM.Client.Common.Forms.Operation operation = new Krista.FM.Client.Common.Forms.Operation();

            try
            {
                operation.Text = "Сбор информации о системе";
                operation.StartOperation();

                // общие настройки грида
                Initialize(InitializeDataSet(), new UltraGrid[] { SuspectGrid, AllGrid, PatchGrid });
                // индивидуальные настроики

                InitializeSuspectRow();
                InitializeAllRow();
                InitializePatchesRow();

                mainForm.Invoke(new VoidDelegate(EnableUI));
            }
            catch(Exception e)
            {
                Forms.FormException.ShowErrorForm(e);
            }
            finally
            {
                operation.StopOperation();

                if (operation != null)
                {
                    operation.ReleaseThread();
                    operation = null;
                }
            }
        }   
    
        private void EnableUI()
        {
            this.Enabled = true;
            AllGrid.Visible = true;
        }

        private void DisableUI()
        {
            this.Enabled = false;
            AllGrid.Visible = false;
        }

        private DataSet InitializeDataSet()
        {
            DataSet dataSet = new DataSet();

            dataSet.BeginInit();

            // таблицы

            DataTable dtServer = scheme.ServerSystemInfo;
            dtServer.TableName = "dtServer";

            DataTable dtClient = new ClientSystemInfo(scheme).GetInfo();
            dtClient.TableName = "dtClient";

            dtServer.Merge(dtClient);

            // итоговая таблица
            dataSet.Tables.Add(dtServer);

            // отношения 
            DataRelation dataRelation = new DataRelation("1", dataSet.Tables[0].Columns["ID"], dataSet.Tables[0].Columns["ParentID"], false);
            dataSet.Relations.Add(dataRelation);

            dataSet.EndInit();

            return dataSet; 
        }

        private void Initialize(DataSet dataSet, params UltraGrid[] ultraGrids)
        {
            foreach (UltraGrid ultraGrid in ultraGrids)
            {
                ultraGrid.DataSource = dataSet;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;

            if (grid != null)
            {
                InitializeGeneralGrid(grid);
            }
        }

        private static void InitializeGeneralGrid(UltraGrid grid)
        {
            foreach (UltraGridBand _band in grid.DisplayLayout.Bands)
            {
                UltraGridColumn clmn = _band.Columns["id"];
                clmn.Hidden = true;

                clmn = _band.Columns["parentID"];
                clmn.Hidden = true;

                clmn = _band.Columns["suspect"];
                clmn.Header.Caption = "Подозрение";
                clmn.Hidden = true;

                clmn = _band.Columns["suspectDescription"];
                clmn.Header.Caption = "Комментарии";
                clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                clmn.Hidden = true;

                clmn = _band.Columns["category"];
                clmn.Header.Caption = "Категория";
                clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;

                clmn = _band.Columns["application"];
                clmn.Header.Caption = "Приложение";
                clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;

                clmn = _band.Columns["name"];
                clmn.Header.Caption = "Имя параметра";
                clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;

                clmn = _band.Columns["value"];
                clmn.Header.Caption = "Значение параметра";
                clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;

                clmn = _band.Columns["uniqueName"];
                clmn.Header.Caption = "Уникальное имя";
                clmn.Hidden = true;
            }
        }

        /// <summary>
        /// Грид c подозрительными параметрами
        /// </summary>
        public UltraGrid UltraGridSuspect
        {
            get
            {
                if (SuspectGrid.Rows.FilteredInRowCount == 0)
                    return new UltraGrid();
                return SuspectGrid; 
            }
        }

        /// <summary>
        /// Грид со всеми параметрами
        /// </summary>
        public UltraGrid UltraGridAll
        {
            get { return AllGrid; }
        }

        public UltraGrid UltraGridPatch
        {
            get
            {
                if (PatchGrid.Rows.FilteredInRowCount == 0)
                    return new UltraGrid();
                 return PatchGrid;
            }
        }

        private void InitializeRow(object sender, InitializeRowEventArgs e)
        {
            //TODO: можно раскрашивать  подозрительные строки и т.п.
            if (Convert.ToBoolean(e.Row.Cells["suspect"].Value))
            {
                e.Row.Appearance.ForeColor = Color.Red;
                e.Row.ToolTipText = Convert.ToString(e.Row.Cells["suspectDescription"].Value);               
            }
        }

        private static void Reset(UltraGridBand band, bool visible)
        {
            band.SortedColumns.Clear();
            band.ColumnFilters.ClearAllFilters();

            band.Columns["application"].Hidden = visible;
            band.Columns["category"].Hidden = visible;
        }

        private void InitializeAllRow()
        {
            try
            {
                foreach (UltraGridBand band in this.AllGrid.DisplayLayout.Bands)
                {
                    Reset(band, true);

                    // настраиваем фильтр
                    ColumnFilter filter = band.ColumnFilters["ParentID"];
                    filter.FilterConditions.Add(FilterComparisionOperator.Equals, null);
                }
                // Дополнительный настройки грида
                // группировка по типу приложения
                this.AllGrid.DisplayLayout.Bands[0].SortedColumns.Add("application", false, true);

                // группировка покатегории
                this.AllGrid.DisplayLayout.Bands[0].SortedColumns.Add("category", false, true);
                this.AllGrid.DisplayLayout.Rows.CollapseAll(true);
                this.AllGrid.Selected.Rows.Clear();
            }
            catch (Exception e)
            {
                throw new Exception("При формировании списка всех параметров возникло исключение" + e.ToString());
            }
        }

        /// <summary>
        /// Инициализация примененных патчей
        /// </summary>
        private void InitializePatchesRow()
        {
            try
            {
                foreach (UltraGridBand band in this.PatchGrid.DisplayLayout.Bands)
                {
                    Reset(band, false);

                    this.PatchGrid.DisplayLayout.Rows.ExpandAll(true);

                    // фильтр по полям, отображающим версию
                    ColumnFilter filter = band.ColumnFilters["UniqueName"];
                    filter.FilterConditions.Add(FilterComparisionOperator.StartsWith, "Version");

                    // теперь фильтруем патчи
                    ColumnFilter patchFilter = band.ColumnFilters["Value"];
                    patchFilter.FilterConditions.Add(FilterComparisionOperator.NotEquals,
                                                     String.Format("{0}.0",
                                                                   Krista.FM.Common.AppVersionControl.
                                                                       GetAssemblyBaseVersion(
                                                                       Krista.FM.Common.AppVersionControl.
                                                                           GetServerLibraryVersion())));
                }
            }
            catch (Exception e)
            {
                throw new Exception("При формировании списка патчей возникло исключение" + e.ToString());
            }
        }

        /// <summary>
        /// Инициализация подозрительных параметров
        /// </summary>
        private void InitializeSuspectRow()
        {
            try
            {
                foreach (UltraGridBand band in this.SuspectGrid.DisplayLayout.Bands)
                {
                    Reset(band, false);

                    band.Columns["suspectDescription"].Hidden = false;
                    band.Columns["suspectDescription"].Width = 250;

                    this.SuspectGrid.DisplayLayout.Rows.ExpandAll(true);

                    // фильтр по подозрительным полям
                    ColumnFilter filter = band.ColumnFilters["suspect"];
                    filter.FilterConditions.Add(FilterComparisionOperator.Equals, true);
                }
            }
            catch (Exception e)
            {
                throw new Exception("При формировании списка подозрительных параметров возникло исключение" + e.ToString());
            }
        }
    }
}
