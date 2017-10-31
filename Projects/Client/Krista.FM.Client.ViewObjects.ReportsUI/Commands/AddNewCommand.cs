using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Commands
{
    internal class AddNewCommand : AbstractCommand
    {
        internal AddNewCommand()
        {
            key = "btnAddNew";
            caption = "Добавить новую запись";
        }

        public override void Run()
        {
            BaseClsUI content = (BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent;
            BaseClsView vo = (BaseClsView)content.ViewCtrl;
            vo.ugeCls.ugData.DisplayLayout.Bands[0].AddNew();
        }
    }

    internal class AddNewDetailCommand : AbstractCommand
    {
        private UltraGridEx detailGrid;

        internal AddNewDetailCommand(UltraGridEx detailGrid)
        {
            key = "btnAddNew";
            caption = "Добавить новую запись";
            this.detailGrid = detailGrid;
        }

        public override void Run()
        {
            //BaseClsUI content = (BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent;
            detailGrid.ugData.DisplayLayout.Bands[0].AddNew();
        }
    }
}
