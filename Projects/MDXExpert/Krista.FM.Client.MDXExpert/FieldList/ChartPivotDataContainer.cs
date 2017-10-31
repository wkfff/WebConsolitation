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
    public partial class ChartPivotDataContainer : PivotDataContainer
    {
        public ChartPivotDataContainer()
        {
            InitializeComponent();
        }

        public ChartPivotDataContainer(Control parent, PivotData pivotData)
            : base(parent, pivotData, null)
        {
            InitializeComponent();

            AreaList.Add(aCols);
            AreaList.Add(aRows);
            AreaList.Add(aFilters);

            aCols.Init(this, "Разместите здесь измерения или меры");
            aRows.Init(this, "Разместите здесь измерения или меры");
            aFilters.Init(this, "Разместите здесь измерения, выберите элементы, по которым требуется осуществлять фильтрацию данных или меру");

            InitCaptions();
        }

        public override void InitCaptions()
        {
            gbColumnsArea.Text = PivotData.ColumnAxis.Caption;
            gbRowsArea.Text = PivotData.RowAxis.Caption;
            gbFiltersArea.Text = PivotData.FilterAxis.Caption;
        }

        public override void SetEnabledGroup(CustomReportElement reportElement)
        {
            if (reportElement != null)
            {
                bool contEnabled = true;
                if (reportElement is ChartReportElement)
                    {
                        contEnabled = (!reportElement.PivotData.IsCustomMDX &&
                                       (String.IsNullOrEmpty(
                                           ((ChartReportElement) reportElement).Synchronization.BoundTo)));
                    }
                gbColumnsArea.Enabled = contEnabled;
                gbRowsArea.Enabled = contEnabled;
                gbFiltersArea.Enabled = contEnabled;
            }
        }
    }
}
