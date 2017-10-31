using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI
{
    public partial class ConsBudgetForecastView : BaseViewObject.BaseView
    {
        public ConsBudgetForecastView()
        {
            InitializeComponent();

            InfragisticComponentsCustomize.CustomizeUltraGridParams(ugData);
        }

        public override void Customize()
        {
            ComponentCustomizer.CustomizeInfragisticsComponents(components);

            base.Customize();
            ugData.DisplayLayout.Override.CellClickAction = CellClickAction.EditAndSelectText;
            ugData.DisplayLayout.Override.MaxSelectedRows = 1;
            ugData.DisplayLayout.Override.MaxSelectedCells = 1;
        }

        public override string Text
        {
            get { return "Доходы"; }
        }

        private void tbManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {

        }
    }
}
