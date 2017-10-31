using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    internal class AddNewRowCommand : AbstractCommand
    {
        internal AddNewRowCommand()
        {
            key = "btnAddNew";
            caption = "Добавить новый договор";
        }

        public override void Run()
        {
            BaseClsUI content = (BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent;
            BaseClsView vo = (BaseClsView)content.ViewCtrl;
            vo.ugeCls.ugData.DisplayLayout.Bands[0].AddNew();
        }
    }

    internal class AddNewDetailRowCommand : AbstractCommand
    {
        private UltraGridEx detailGrid;

        internal AddNewDetailRowCommand(UltraGridEx detailGrid)
        {
            key = "btnAddNew";
            caption = "Добавить новую запись";
            this.detailGrid = detailGrid;
        }

        public override void Run()
        {
            detailGrid.ugData.DisplayLayout.Bands[0].AddNew();
        }
    }
}
