using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert.FieldList
{
    public partial class TablePivotDataContainer : PivotDataContainer
    {

        public TablePivotDataContainer()
        {
        }

        public TablePivotDataContainer(Control parent, Data.PivotData pivotData)
            : base(parent, pivotData, null)
        {
            InitializeComponent();

            AreaList.Add(aCols);
            AreaList.Add(aRows);
            AreaList.Add(aFilters);
            AreaList.Add(aTotals);

            aCols.Init(this, "Размеcтите здесь измерения");
            aRows.Init(this, "Размеcтите здесь измерения");
            aFilters.Init(this, "Размеcтите здесь измерения, выберите элементы, по которым требуется осуществлять фильтрацию данных");
            aTotals.Init(this, "Разместите здесь меры из куба");

            InitCaptions();
        }

        /// <summary>
        /// Инициализация заголовков областей
        /// </summary>
        public override void InitCaptions()
        {
            gbColumnsArea.Text = PivotData.ColumnAxis.Caption;
            gbRowsArea.Text = PivotData.RowAxis.Caption;
            gbFiltersArea.Text = PivotData.FilterAxis.Caption;
            gbValuesArea.Text = PivotData.TotalAxis.Caption;
        }

        public void SetEnabledGroup(CustomReportElement reportElement)
        {
            if (reportElement != null)
            {
                gbColumnsArea.Enabled = !reportElement.PivotData.IsCustomMDX;
                gbRowsArea.Enabled = !reportElement.PivotData.IsCustomMDX;
                gbFiltersArea.Enabled = !reportElement.PivotData.IsCustomMDX;
                gbValuesArea.Enabled = !reportElement.PivotData.IsCustomMDX;
            }
        }
    }
}
