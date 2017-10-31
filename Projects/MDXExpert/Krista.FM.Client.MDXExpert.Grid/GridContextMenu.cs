using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Data;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.Grid
{
    public partial class GridContextMenu : Control
    {
        private GridControl selectedControl;
        private ExpertGrid grid;
        private bool isMultiSelect = false;

        public GridContextMenu(ExpertGrid grid)
        {
            InitializeComponent();
            this.grid = grid;
            this.contextMenuStrip.ImageList = this.grid.imageList;
        }

        /// <summary>
        /// Проинициализируем и покажем контекстное меню
        /// </summary>
        /// <param name="selectedControl">элемент для которого показ. меню</param>
        /// <param name="location">позиция меню на экране</param>
        public void Show(GridControl selectedControl, Point location, bool multiSelect)
        {
            this.isMultiSelect = multiSelect;
            this.selectedControl = selectedControl;
            this.InitContextItems(selectedControl, multiSelect);
            this.contextMenuStrip.Show(location);
        }

        /// <summary>
        /// Выставляет сепаратор
        /// </summary>
        private void SetSeparator()
        {
            if (contextMenuStrip.Items.Count > 0)
                this.contextMenuStrip.Items.Add(new ToolStripSeparator());
        }

        /// <summary>
        /// В зависимости от выделеного элемента, в контескстном меню отображаются разные пункты
        /// </summary>
        /// <param name="selectedControl"></param>
        public void InitContextItems(GridControl selectedControl, bool multiSelect)
        {
            this.contextMenuStrip.Items.Clear();
            if (multiSelect)
            {
                //Добавляем пункт "Копировать"
                this.AddCopyItems();
                return;
            }

            if (selectedControl == null)
                return;

            //множестов всех контролов
            object[] allControl = new object[] {GridObject.CaptionCell, GridObject.DimensionCell,
                GridObject.MeasureCell};

            if (Common.CommonUtils.IsInSet(allControl, selectedControl.GridObject))
            {
                //Добавляем пункт "Копировать"
                this.AddCopyItems();
                //Добавляем пункты сортировки
                this.AddSortItems();
            }


            switch (selectedControl.GridObject)
            {
                case GridObject.CaptionCell:
                    {
                        this.AddExpandedMembersItems();
                        break;
                    }
                case GridObject.DimensionCell:
                    {
                        //поставим сепаратор
                        this.SetSeparator();

                        ToolStripItem filterByVisibleItem = this.contextMenuStrip.Items.Add("Фильтр по выделенному");
                        filterByVisibleItem.Click += new EventHandler(filterByVisibleItem_Click);
                        filterByVisibleItem.ImageIndex = 1;
                        break;
                    }
                case GridObject.MeasureCell:
                    {
                        //для вычислимых мер невозможно показать детальные данные 
                        MeasureCell measureCell = (MeasureCell)this.selectedControl;
                        if (measureCell.MeasureData.MeasureCaption.IsCalculate)
                            break;
                        
                        //поставим сепаратор
                        this.SetSeparator();

                        List<string> actions = GetActions();

                        if (!actions.Contains("Детальные данные"))
                        {
                            ToolStripItem drillTroughtItem = this.contextMenuStrip.Items.Add("Детальные данные");
                            drillTroughtItem.Click += new EventHandler(drillTroughtItem_Click);
                        }

                        AddActionItems(actions);
                        break;
                    }
            }
        }

        private void AddActionItems(List<string> actions)
        {
            foreach (string actionName in actions)
            {
                ToolStripItem actionItem = this.contextMenuStrip.Items.Add(actionName);
                actionItem.Click += new EventHandler(actionItem_Click);
            }
        }

        private List<string> GetActions()
        {
            List<string> result = new List<string>();

            AdomdRestrictionCollection oRest = new AdomdRestrictionCollection();
            //oRest.Add("ACTION_NAME", sActionName);
            oRest.Add("CUBE_NAME", this.grid.PivotData.CubeName);
            MeasureCell measureCell = (MeasureCell)this.selectedControl;
            string coordinate = measureCell.MeasureData.MeasureCaption.UniqueName;

            oRest.Add("COORDINATE", coordinate);
            oRest.Add("COORDINATE_TYPE", 6);
            DataSet ds = PivotData.AdomdConn.GetSchemaDataSet("MDSCHEMA_ACTIONS", oRest, true);

            if (ds.Tables.Count > 0)
            {
                foreach(DataRow row in ds.Tables[0].Rows)
                {
                    string actionName = (string) row["ACTION_NAME"];
                    result.Add(actionName);
                }
            }

            return result;
        }

        /// <summary>
        /// Добавления пункта контекстного меню "Копирование"
        /// </summary>
        private void AddCopyItems()
        {            
            //поставим сепаратор
            this.SetSeparator();

            ToolStripItem copyValueItem = this.contextMenuStrip.Items.Add("Копировать");
            copyValueItem.Click += new EventHandler(copyValueItem_Click);
            copyValueItem.ImageIndex = 0;        
        }

        /// <summary>
        /// Добавляем пункты для сортировки элемента
        /// </summary>
        private void AddSortItems()
        {
            //поставим сепаратор
            this.SetSeparator();

            ToolStripItem askSortItem = this.contextMenuStrip.Items.Add("Сортировать по возрастанию");
            askSortItem.Click += new EventHandler(askSortItem_Click);
            ToolStripItem deskSortItem = this.contextMenuStrip.Items.Add("Сортировать по убыванию");
            deskSortItem.Click += new EventHandler(deskSortItem_Click);
            CaptionCell caption = this.GetCaptionCell(this.selectedControl);
            if (caption != null)
            {
                switch (caption.SortType)
                {
                    case Krista.FM.Client.MDXExpert.Data.SortType.ASC:
                    case Krista.FM.Client.MDXExpert.Data.SortType.BASC:
                        {
                            askSortItem.ImageIndex = 2;
                            break;
                        }
                    case Krista.FM.Client.MDXExpert.Data.SortType.BDESC:
                    case Krista.FM.Client.MDXExpert.Data.SortType.DESC:
                        {
                            deskSortItem.ImageIndex = 2;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Если у контрола есть ячейка с заголовком, вернет ее
        /// </summary>
        /// <param name="gridControl"></param>
        /// <returns></returns>
        private CaptionCell GetCaptionCell(GridControl gridControl)
        {
            if (gridControl != null)
            {
                switch (gridControl.GridObject)
                {
                    case GridObject.CaptionCell:
                        {
                            return (CaptionCell)gridControl;
                        }
                    case GridObject.DimensionCell:
                        {
                            return ((DimensionCell)gridControl).CaptionCell;
                        }
                    case GridObject.MeasureCell:
                        {
                            return ((MeasureCell)gridControl).MeasureData.MeasureCaption;
                        }
                }
            }
            return null;
        }

        /// <summary>
        /// Добавить пункты для развертывания свертывания элементов на уровне
        /// </summary>
        private void AddExpandedMembersItems()
        {
            CaptionCell captionCell = (CaptionCell)this.selectedControl;
            if ((captionCell.Captions.Type == CaptionType.Columns)
                || (captionCell.Captions.Type == CaptionType.Rows))
            {
                PivotField field = this.grid.PivotData.GetPivotField(captionCell.UniqueName);
                //если уровень входит в запрос, то у него можно развернуть элементы
                if ((field != null) && field.IsIncludeToQuery)
                {
                    //поставим сепаратор
                    this.SetSeparator();

                    ToolStripItem expandMembersItem = this.contextMenuStrip.Items.Add("Развернуть элементы");
                    expandMembersItem.Click += new EventHandler(expandMembersItem_Click);
                    expandMembersItem.ImageIndex = 3;
                    ToolStripItem collapseMembersItem = this.contextMenuStrip.Items.Add("Cвернуть элементы");
                    collapseMembersItem.Click += new EventHandler(collapseMembersItem_Click);
                    collapseMembersItem.ImageIndex = 4;
                }
            }
        }

        #region Обработчики элементов в контекстном меню

        void copyValueItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            if (this.isMultiSelect)
            {
                Clipboard.SetText(this.grid.SelectedCells.GetSelectedText());
            }
            else
            {
                if(!String.IsNullOrEmpty(this.selectedControl.GetValue()))
                    Clipboard.SetText(this.selectedControl.GetValue());
            }
        }

        void askSortItem_Click(object sender, EventArgs e)
        {
            CaptionCell caption = this.GetCaptionCell(this.selectedControl);
            if (caption != null)
            {
                //если уже было по возрастанию, значит сбрасываем сортировку
                SortType sortType = (caption.SortType == SortType.ASC) ? SortType.None : SortType.ASC;
                caption.Sort(sortType);
            }
        }

        void deskSortItem_Click(object sender, EventArgs e)
        {
            CaptionCell caption = this.GetCaptionCell(this.selectedControl);
            if (caption != null)
            {
                //если уже было по убыванию, значит сбрасываем сортировку
                SortType sortType = (caption.SortType == SortType.DESC) ? SortType.None : SortType.DESC;
                caption.Sort(sortType);
            }
        }

        void expandMembersItem_Click(object sender, EventArgs e)
        {
            CaptionCell captionCell = (CaptionCell)this.selectedControl;
            Axis axis = captionCell.Captions.Type == CaptionType.Rows ? (Axis)this.grid.Row
                : (Axis)this.grid.Column;
            axis.ExpandAllMember(captionCell, true);
        }

        void collapseMembersItem_Click(object sender, EventArgs e)
        {
            CaptionCell captionCell = (CaptionCell)this.selectedControl;
            Axis axis = captionCell.Captions.Type == CaptionType.Rows ? (Axis)this.grid.Row
                : (Axis)this.grid.Column;
            axis.ExpandAllMember(captionCell, false);
            
        }

        void filterByVisibleItem_Click(object sender, EventArgs e)
        {
            DimensionCell cell = (DimensionCell)this.selectedControl;
            this.grid.PivotData.FilterBySelected(cell.ClsMember);
        }

        void drillTroughtItem_Click(object sender, EventArgs e)
        {
            MeasureCell measureCell = (MeasureCell)this.selectedControl;
            measureCell.DrillTrought(String.Empty);
        }

        void actionItem_Click(object sender, EventArgs e)
        {
            MeasureCell measureCell = (MeasureCell)this.selectedControl;
            string actionName = ((ToolStripMenuItem)sender).Text;
            measureCell.DrillTrought(actionName);
        }


        #endregion
    }
}
