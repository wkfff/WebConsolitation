using System.Data;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.MinSportSupport
{
    public class GridConverter : GridCommon
    {

        private readonly int quantityHandBook; // количество разрезов
        private readonly bool visibleDefinition; // показывать ли столбец описания в гриде

        // гарантирует полное отображение по горизонтали (когда нельзя доверять кукисам)
        public bool SafeMode { set; get; }

        public GridConverter(UltraGridBrick grid, int quantityHandBook, bool visibleDefinition)
            : base(grid)
        {
            HeaderHeight = 167;
            this.quantityHandBook = quantityHandBook;
            this.visibleDefinition = visibleDefinition;
        }

        /// <summary>
        /// установить стиль внешнего вида элемента
        /// </summary>
        public override void SetStyle()
        {
            base.SetStyle();
            SetMaxWidth(SafeMode);
            SetHeight(SportHelper.defaultHeightFull);
        }

        /// <summary>
        /// настройка внешнего вида данных
        /// </summary>
        public override void SetDataStyle()
        {
            for (var i = 0; i < Band.Columns.Count; i++)
            {
                Band.Columns[i].Width = 100;
            }
            if (visibleDefinition)
            {
                Band.Columns[0].Width = 400;
                Band.Columns[0].CellStyle.Wrap = true;
                Band.Columns[3].CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;
            }
            else
            {
                Band.Columns[2].CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right; 
            }
        }

        /// <summary>
        /// установить одинаковую ширину для всех колонок
        /// </summary>
        public void SetConstColumnWidth()
        {
            Band.SetConstWidth(90, false);
        }

        /// <summary>
        /// настройка индикаторов
        /// </summary>
        public override void SetDataRules()
        {
            /*// правило для первой строки
            FontRowLevelRule levelRule = new FontRowLevelRule(Band.Columns.Count - 1);
            levelRule.AddFontLevel("0", Grid.BoldFont8pt);
            Grid.AddIndicatorRule(levelRule);

            // сдвиг текста согласно иерархии
            Grid.AddIndicatorRule(new PaddingRule(0, "УровеньИерархии", 10));*/
        }

        /// <summary>
        /// заголовки таблицы
        /// </summary>
        public override void SetDataHeader()
        {
            GridHeaderLayout headerLayout = Grid.GridHeaderLayout;
            if (visibleDefinition)
            {
                headerLayout.AddCell("Описание данных", 1);   
            }
            headerLayout.AddCell("Год", 1);
            headerLayout.AddCell("Отчетный период", 1);
            headerLayout.AddCell("Значение", 1);
            headerLayout.AddCell("Единица измерения", 1);
            for (var i = 1; i <= quantityHandBook + 1; i++ )
            {
                headerLayout.AddCell("Код разреза", 1);
                headerLayout.AddCell("Код признака", 1);  
            }

            Grid.GridHeaderLayout.ApplyHeaderInfo();
        }

        /// <summary>
        /// установить данные
        /// </summary>
        public override void SetData(string queryName)
        {
            Query = new QueryWorker(queryName);
            base.SetData();
        }

        /// <summary>
        /// Установить данные из готовой таблицы
        /// </summary>
        public void SetDataFromDataTable(DataTable table)
        {
            if (table.Rows.Count > 0)
            {
                if (!visibleDefinition)
                {
                    table.Columns.Remove("dummy");
                }
                Grid.DataTable = table;
            }
            Grid.DataBind();
        }

    }



}
