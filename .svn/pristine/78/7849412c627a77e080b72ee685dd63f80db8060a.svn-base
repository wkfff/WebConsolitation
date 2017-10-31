using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.SchemeEditor.Services.SearchService
{
    public partial class SearchGridControl : UserControl
    {
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public SearchGridControl()
        {
            InitializeComponent();

            // Настраиваем параметры таблицы.
            ultraGridEx.AllowClearTable = false;
            ultraGridEx.AllowDeleteRows = false;
            ultraGridEx.AllowImportFromXML = false;
            ultraGridEx.ExportImportToolbarVisible = false;
            ultraGridEx.SaveMenuVisible = false;
            ultraGridEx.LoadMenuVisible = false;
            ultraGridEx.AllowAddNewRecords = false;
            ultraGridEx.ugData.ImageList = ilButtonGoTo;
            ((Infragistics.Win.UltraWinToolbars.StateButtonTool)ultraGridEx._utmMain.Tools["ShowGroupBy"]).Checked = false;
            ultraGridEx._utmMain.Tools["Refresh"].SharedProps.Visible = false;
            ultraGridEx._utmMain.Tools["ShowGroupBy"].SharedProps.Visible = false;
            ultraGridEx._utmMain.Tools["SaveChange"].SharedProps.Visible = false;
            ultraGridEx._utmMain.Tools["CancelChange"].SharedProps.Visible = false;
            ultraGridEx._utmMain.Tools["ShowHierarchy"].SharedProps.Visible = false;
            ultraGridEx._utmMain.Tools["ColumnsVisible"].SharedProps.Visible = false;
            ultraGridEx._utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
            ultraGridEx._utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = false;
        }

        // Свойство, наследуемое у класса UltraGrid
        public System.Data.DataTable DataSource
        {
            set
            {
                ultraGridEx.DataSource = value;
                // Заполнение строки статуса.
                statusLabel.Text = "Найдено объектов: " + ultraGridEx.ugData.Rows.Count;
            }
        }

        /// <summary>
        /// Инициализация параметров таблицы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraGridEx_OnGridInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn;

            // Настраиваем видимость столбцов.
            band.Columns["key"].Hidden = true;
            band.Columns["caption"].Hidden = false;
            band.Columns["path"].Hidden = false;
            band.Columns["type"].Hidden = false;
            band.Columns["property"].Hidden = false;
            //band.Columns[StateColumnName].Hidden = false;

            // Настраиваем параметры отдельных столбцов.

            clmn = band.Columns["caption"];
            clmn.CellActivation = Activation.NoEdit;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 1;
            clmn.Hidden = false;
            clmn.Header.Caption = "Русское имя";
            clmn.Width = 400;

            clmn = band.Columns["path"];
            clmn.CellActivation = Activation.NoEdit;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 2;
            clmn.Hidden = false;
            clmn.Header.Caption = "Английское имя";
            clmn.Width = 310;

            clmn = band.Columns["type"];
            clmn.CellActivation = Activation.NoEdit;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 3;
            clmn.Hidden = false;
            clmn.Header.Caption = "Тип объекта";
            clmn.Width = 120;

            clmn = band.Columns["property"];
            clmn.CellActivation = Activation.NoEdit;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 4;
            clmn.Hidden = false;
            clmn.Header.Caption = "Свойство";
            clmn.Width = 120;

            Krista.FM.Client.Common.InfragisticComponentsCustomize.CustomizeUltraGridParams(ultraGridEx._ugData);

            // Добавляем на грид кнопки для перехода к объекту.
            UltraGridColumn clmnGoTo;
            clmnGoTo = band.Columns.Add("clmnGoTo");
            clmnGoTo.Header.VisiblePosition = 0;
            Krista.FM.Client.Components.UltraGridHelper.SetLikelyButtonColumnsStyle(clmnGoTo, 0);
        }

        /// <summary>
        /// Выделение объекта в дереве по ключу.
        /// </summary>
        /// <param name="key">Уникальный ключ объекта</param>
        private void SelectObjectByKey(string key)
        {
            SchemeEditor.Instance.SelectObject(key, false);
        }
        
        /// <summary>
        /// Инициализация строки грида.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraGridEx_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["clmnGoTo"].ToolTipText = "Перейти к объекту";
        }

        /// <summary>
        /// Обработка щелчка по кнопке перехода к объекту в дереве.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraGridEx_OnClickCellButton(object sender, CellEventArgs e)
        {
            // Если щелчок произошел по кнопке перехода к объекту
            if (e.Cell.Column.Key == "clmnGoTo")
            {
                // то получаем выделенный элемент таблицы
                if (ultraGridEx.ugData.ActiveCell != null)
                {
                    string selectItem = ultraGridEx.ugData.ActiveRow.Cells["key"].Value.ToString();

                    // и если он не пустой
                    if (selectItem != System.String.Empty)
                    {
                        // то выделяем его.
                        SelectObjectByKey(selectItem);
                    }
                }
            }
        }
    }
}
