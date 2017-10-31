using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    public class EntityGridView : GridModelControl
    {
        public override List<Component> Build(ViewPage page)
        {
            var viewport = new Viewport();
            var borderLayout = new BorderLayout();

            borderLayout.Center.Items.Add(base.Build(page));

            // Настраиваем и добавляем кнопочку Выбрать
            const string Script = "function rowSelected(record)                                     " +
                                  "   {{                                                            " +
                                  "     {0}.getBody().Extension.entityBook.selectedRecord = record; " +
                                  "     {1}.setDisabled(false);                                     " +
                                  "   }}                                                            " +
                                  "  {0}.getBody().Extension.entityBook.onRowSelect = rowSelected;  ";
            ((GridPanel)borderLayout.Center.Items[0]).Listeners.AfterRender.Handler = Script.FormatWith("top.HBWnd", "HandBookSelectButton");
            ((GridPanel)borderLayout.Center.Items[0]).Listeners.RowClick.Handler = "HandBookSelectButton.setDisabled(false);";
            var selectButton = new Button
            {
                ID = "HandBookSelectButton",
                Disabled = true,
                Text = @"Выбрать",
                Listeners =
                {
                    Click =
                    {
                        Handler = @"
                            HandBookSelectButton.setDisabled(true);
                            var selectedBookRecord = top.HBWnd.getBody().Extension.entityBook.selectedRecord;
                            top.window.Workbench.extensions.HandBooks.onSelectRow(selectedBookRecord, top.HBWnd.getBody().Extension.entityBook.getLookupValue());
                            top.HBWnd.hide();"
                    }
                } 
            };
            ((GridPanel)borderLayout.Center.Items[0]).Buttons.Add(selectButton);

            viewport.Items.Add(borderLayout);
            var script = ViewService.GetClientScript();
            ResourceManager.GetInstance(page).RegisterAfterClientInitScript(script);


            return new List<Component> { viewport };
        }

        protected override void CreateColumnModel(GridPanel grid)
        {
            base.CreateColumnModel(grid);

            var nameColumn = Columns.First(x => x.ColumnID == "Name");

            if (Columns.Count != 1)
            {
                grid.AutoExpandColumn = MasterColumnId.IsNotNullOrEmpty() ? MasterColumnId : nameColumn.DataIndex;
            }

            grid.AutoExpandMin = 250;
            
            var style = new StringBuilder();

            style.Append(".x-grid3-col-")
                .Append(nameColumn.DataIndex)
                .AppendLine("{white-space: normal;}");

            RefFieldStyles += style.ToString();
        }
    }
}
