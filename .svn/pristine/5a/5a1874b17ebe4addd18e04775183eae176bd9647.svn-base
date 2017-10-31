using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public partial class BKKUIndicatorsView : BaseView
    {
        public BKKUIndicatorsView()
        {
            InitializeComponent();

			InfragisticComponentsCustomize.CustomizeUltraGridParams(this.ugeIndicators._ugData);
		}

        public override string Text
        {
            get { return "Оценка проекта бюджета"; }
        }
    }
}
