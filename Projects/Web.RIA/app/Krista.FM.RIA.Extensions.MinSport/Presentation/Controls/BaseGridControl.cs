using Ext.Net;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Controls
{
    public class BaseGridControl : MinSportGridControl
    {
        public string WindowName { get; set; }

        public string FormName { get; set; }

        protected override GridPanel BuildBaseGrid()
        {
            ToolBarButtons.Add(new Button
            {
                ID = "btnAdd",
                Icon = Icon.Add,
                ToolTip = "Добавить новую запись",
                Listeners =
                    {
                        Click = { Handler = "insertRecord({0}, {1}); ".FormatWith(WindowName, FormName) }
                    }
            });
            ToolBarButtons.Add(new Button
            {
                ID = "btnDelete",
                Icon = Icon.Delete,
                ToolTip = "Удалить запись",
                Disabled = true,
                Listeners =
                {
                    Click = { Handler = "deleteRecord({0}); ".FormatWith(GridName) }
                }
            });

            RowSelModel = new RowSelectionModel
            {
                SingleSelect = true,
                Listeners =
                    {
                        RowSelect = { Handler = "btnDelete.enable(); " },
                        RowDeselect = { Handler = "hasRowSelection({0})".FormatWith(GridName) }
                    }
            };

            GridListeners.Add("Command", "pressEditRecord(record, 'EditRecord', {0}, {1}); ".FormatWith(WindowName, FormName)); 

            return base.BuildBaseGrid();
        }
    }
}
