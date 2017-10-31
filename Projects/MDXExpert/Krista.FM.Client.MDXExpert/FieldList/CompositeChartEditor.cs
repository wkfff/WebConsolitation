using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Infragistics.Win.UltraWinDock;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.FieldList
{
    /// <summary>
    /// Редактор композитных диаграмм
    /// </summary>
    public partial class CompositeChartEditor : UserControl
    {
        #region Поля

        private MainForm mainForm;
        private ChartReportElement reportElement;

        // Легенда
        private CompositeLegend legend;

        // индексы элементов списка
        private int dropIndex = -1;
        private int dragIndex = -1;
        // индекс списка, откуда тащим, - 0 для chartLayersList, 1 для availableChartsList
        private int dragList = -1;

        private int selectedLayer = -1;

        private Dictionary<string, string> availableDictionary = new Dictionary<string, string>();

        // поля для сохранения постоянных настроек
        private List<int> axisExtents = new List<int>();
        private List<CompositeLegend> compositeLegends = new List<CompositeLegend>();

        #endregion

        #region Свойства

        /// <summary>
        /// Выбранный слой
        /// </summary>
        public int SelectedLayer
        {
            get { return selectedLayer; }
            set { selectedLayer = value;}
        }

        /// <summary>
        /// Главная форма
        /// </summary>
        public MainForm MainForm
        {
            get { return mainForm; }
            set { mainForm = value; }
        }

        /// <summary>
        /// Элемент отчета, содержащий текущую композитную диаграмму
        /// </summary>
        public ChartReportElement ReportElement
        {
            get { return reportElement; }
            set { reportElement = value; }
        }

        #endregion

        public CompositeChartEditor()
        {
            InitializeComponent();

            chartLayersList.AllowDrop = true;
            availableChartsList.AllowDrop = true;
            chartLayersList.MouseDown += new MouseEventHandler(chartLayerList_MouseDown);
            chartLayersList.DragOver += new DragEventHandler(chartLayerList_DragOver);
            chartLayersList.DragDrop += new DragEventHandler(chartLayerList_DragDrop);
            chartLayersList.DragEnter += new DragEventHandler(chartLayersList_DragEnter);
            availableChartsList.MouseDown += new MouseEventHandler(availableChartList_MouseDown);
            availableChartsList.DragOver += new DragEventHandler(availableChartsList_DragOver);
            availableChartsList.DragDrop += new DragEventHandler(availableChartsList_DragDrop);

            RefreshEditor();
        }

        #region Обработчики

        static void chartLayersList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        void chartLayerList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string str = (string)e.Data.GetData(DataFormats.StringFormat);

                if (dragList == 0)
                {
                    reportElement.CompositeAxiesCorrection = false;
                    // если бросили внутри того же списка, то перемещаем элементы
                    string childKey = reportElement.GetChildChartKey(dragIndex);
                    ChartReportElement childReportElement = mainForm.FindChartReportElement(childKey);

                    chartLayersList.Items.RemoveAt(dragIndex);
                    reportElement.RemoveChildChart(childKey);
                    childReportElement.RemoveParentChart(reportElement.UniqueName);

                    if (dropIndex == -1)
                    {
                        // если бросили не на сам список, то добавляем элемент в конец списка
                        chartLayersList.Items.Add(str);
                        reportElement.AddChildChart(childKey);
                        childReportElement.AddParentChart(reportElement.UniqueName);
                    }
                    else
                    {
                        // иначе вставляем в то место, где отпустили
                        chartLayersList.Items.Insert(dropIndex, str);
                        reportElement.InsertChildChart(childKey, dropIndex);
                        childReportElement.AddParentChart(reportElement.UniqueName);
                    }
                    reportElement.CompositeAxiesCorrection = true;
                }
                else
                {
                    SaveParams();

                    reportElement.ResetCompositeChart();

                    // иначе бросили из другого, тогда добавляем новый элемент
                    string dragItem = availableChartsList.Items[dragIndex].ToString();
                    if (availableDictionary.ContainsKey(dragItem))
                    {
                        string childKey = availableDictionary[dragItem];
                        ChartReportElement childReportElement = mainForm.FindChartReportElement(childKey);

                        chartLayersList.Items.Add(str);
                        reportElement.AddChildChart(childKey);
                        childReportElement.AddParentChart(reportElement.UniqueName);
                    }
                    LoadParams();
                }

                RefreshEditor();

                this.MainForm.Saved = false;
            }
        }

        void chartLayerList_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            // если тащим над тем же списком, где и взяли
            if (dragList == 0)
            {
                if (e.Data.GetDataPresent(typeof(string)))
                {
                    dropIndex = chartLayersList.IndexFromPoint(chartLayersList.PointToClient(new Point(e.X, e.Y)));
                }
            }
        }

        void chartLayerList_MouseDown(object sender, MouseEventArgs e)
        {
            if (chartLayersList.Items.Count == 0)
            {
                return;
            }

            dragList = 0;
            dragIndex = chartLayersList.IndexFromPoint(e.X, e.Y);

            selectedLayer = dragIndex;
            mainForm.ToolbarsManager.ChartToolBar.RefreshTabsTools(reportElement);
            if (dragIndex != -1)
            {
                DoDragDrop(chartLayersList.Items[dragIndex].ToString(), DragDropEffects.Move);
            }
            else
            {
                chartLayersList.SelectedIndex = -1;
            }
        }

        void availableChartList_MouseDown(object sender, MouseEventArgs e)
        {
            if (availableChartsList.Items.Count == 0)
            {
                return;
            }

            dragList = 1;
            dragIndex = availableChartsList.IndexFromPoint(e.X, e.Y);
            if (dragIndex != -1)
            {
                DoDragDrop(availableChartsList.Items[dragIndex].ToString(), DragDropEffects.Move);
            }
        }

        void availableChartsList_DragDrop(object sender, DragEventArgs e)
        {
            if (dragIndex == -1)
            {
                return;
            }

            // если бросили из другого списка
            if (dragList != 1)
            {
//                SaveParams();
//                
//                reportElement.ResetCompositeChart();

                string childKey = reportElement.GetChildChartKey(dragIndex);
                ChartReportElement childReportElement = mainForm.FindChartReportElement(childKey);
                
                chartLayersList.Items.RemoveAt(dragIndex);
                reportElement.RemoveChildChart(childKey);
                childReportElement.RemoveParentChart(reportElement.UniqueName);

                RefreshEditor();

//                LoadParams();

                // если вытащили последний элемент
                if (chartLayersList.Items.Count == 0)
                {
                    reportElement.ResetCompositeLayers();
                }

                mainForm.ToolbarsManager.ChartToolBar.RefreshTabsTools(reportElement);
                this.MainForm.Saved = false;
            }
        }

        void availableChartsList_DragOver(object sender, DragEventArgs e)
        {
            // если бросили из другого списка
            if (dragList != 1)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        #endregion

        /// <summary>
        /// Сброс параметров редактора
        /// </summary>
        public void ResetEditor()
        {
            availableChartsList.Items.Clear();
            chartLayersList.Items.Clear();
        }

        /// <summary>
        /// Обновление редактора
        /// </summary>
        public void RefreshEditor()
        {
            if (mainForm == null || mainForm.ControlPanes.Count == 0 || reportElement == null)
            {
                return;
            }

            // заполняем список ключами выбранных элементов
            chartLayersList.Items.Clear();
            foreach (ChartLayerAppearance layer in reportElement.Chart.CompositeChart.ChartLayers)
            {
                string itemText = string.Format("{0} ({1})", mainForm.GetReportElementText(layer.Key), ChartTypeConverter.GetLocalizedChartType(layer.ChartType));
                chartLayersList.Items.Add(itemText);
            }

            // заполняем список доступных диаграмм
            availableDictionary = new Dictionary<string, string>();
            availableChartsList.Items.Clear();
            List<string> availableItems = mainForm.GetAvialableCompositeCharts(reportElement.UniqueName);
            foreach (string item in availableItems)
            {
                ChartReportElement element = mainForm.FindChartReportElement(item);
                string itemText = string.Format("{0} ({1})",
                    element.Title, ChartTypeConverter.GetLocalizedChartType(element.Chart.ChartType));
                availableChartsList.Items.Add(itemText);
                availableDictionary.Add(itemText, element.UniqueName);
            }

            // корректируем выделенные элементы списка слоев
            if (selectedLayer != -1)
            {
                if (chartLayersList.Items.Count == 0)
                {
                    selectedLayer = -1;
                }
                else
                {
                    if (chartLayersList.Items.Count > selectedLayer)
                    {
                        chartLayersList.SelectedIndex = selectedLayer;
                    }
                    else
                    {
                        chartLayersList.SelectedIndex = chartLayersList.Items.Count - 1;
                        selectedLayer = chartLayersList.SelectedIndex;
                    }
                }

                mainForm.ToolbarsManager.ChartToolBar.RefreshTabsTools(reportElement);
            }
        }

        /// <summary>
        /// Установка цветов контролу
        /// </summary>
        public void AdjustColors(Color panelColor, Color borderColor, Color darkPanelColor)
        {
            this.BackColor = panelColor;
            ChartListSplitContainer.SplitterWidth = 2;
            ChartListSplitContainer.BackColor = darkPanelColor;
        }

        private void SaveParams()
        {
            UltraChart chart = reportElement.Chart;
            if (chart == null)
            {
                return;
            }

            axisExtents.Clear();
            compositeLegends.Clear();

            if (chart.CompositeChart.ChartAreas.Count > 0)
            {
                for (int i = 0; i < chart.CompositeChart.ChartAreas[0].Axes.Count; i++)
                {
                    axisExtents.Add(chart.CompositeChart.ChartAreas[0].Axes[i].Extent);
                }
            }

            compositeLegends.Add(chart.CompositeChart.Legends[0]);
        }

        private void LoadParams()
        {
            UltraChart chart = reportElement.Chart;
            if (chart == null)
            {
                return;
            }

            // оси восстанавливаются отдельно в методе SetAxisExtents

            // восстанавливаем легенду
            // почему-то при восстановлении легенды добавленные слои не отображаются,
            // поэтому добавляем их заново
            chart.CompositeChart.Legends[0] = compositeLegends[0];
            chart.CompositeChart.Legends[0].ChartLayerList = string.Empty;
            chart.CompositeChart.Legends[0].ChartLayers.Clear();
            for (int j = 0; j < chart.CompositeChart.ChartLayers.Count; j++)
            {
                chart.CompositeChart.Legends[0].ChartLayers.Add(chart.CompositeChart.ChartLayers[j]);
                //chart.CompositeChart.Legends[0].ChartLayerList += chart.CompositeChart.ChartLayers[j].Key + "|";
            }
            chart.CompositeChart.Legends[0].Visible = compositeLegends[0].Visible;
            chart.InvalidateLayers();
        }
    }
}
