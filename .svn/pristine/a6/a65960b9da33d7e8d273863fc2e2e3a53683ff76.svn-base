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
    public partial class MapPivotDataContainer : PivotDataContainer
    {
        private MapReportElement mapElement;

        public MapReportElement MapElement
        {
            get { return this.mapElement; }
        }

        public MapPivotDataContainer()
        {
        }

        public MapPivotDataContainer(Control parent, Data.PivotData pivotData, MapReportElement element)
            : base(parent, pivotData, element)
        {
            InitializeComponent();

            AreaList.Add(aCols);
            AreaList.Add(aRows);
            AreaList.Add(aFilters);
            AreaList.Add(aTotals);

            aCols.Init(this, "Разместите здесь измерения или меры");
            aRows.Init(this, "Размеcтите здесь измерение, содержащее список территорий (Районы Сопоставимый; Районы Сопоставимый Планирование; Территории Сопоставимый)");
            aFilters.Init(this, "Разместите здесь измерения, выберите элементы, по которым требуется осуществлять фильтрацию данных");
            aTotals.Init(this, "Размеcтите здесь измерения или меры");

            InitCaptions();

            this.mapElement = element;
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


        public override void SetEnabledGroup(CustomReportElement reportElement)
        {
            if (reportElement != null)
            {
                if (!(reportElement is MapReportElement))
                    return;

                bool contEnabled = (!reportElement.PivotData.IsCustomMDX &&
                                    (String.IsNullOrEmpty(((MapReportElement)reportElement).Synchronization.BoundTo)));

                gbColumnsArea.Enabled = contEnabled;
                gbRowsArea.Enabled = contEnabled;
                gbFiltersArea.Enabled = contEnabled;
                gbValuesArea.Enabled = contEnabled;
            }
            /*
            if (reportElement != null)
            {
                gbColumnsArea.Enabled = !reportElement.PivotData.IsCustomMDX;
                gbRowsArea.Enabled = !reportElement.PivotData.IsCustomMDX;
                gbFiltersArea.Enabled = !reportElement.PivotData.IsCustomMDX;
                gbValuesArea.Enabled = !reportElement.PivotData.IsCustomMDX;
            }*/
        }
    }
}
