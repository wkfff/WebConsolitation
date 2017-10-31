using System;
using System.Text;
using System.Web.UI;

namespace Krista.FM.Server.Dashboards.Components
{
    public partial class GridSearch : UserControl
    {
        private string linkedGridId;

        public string LinkedGridId
        {
            set
            {
                linkedGridId = "o";
                linkedGridId += value.Replace('_', 'x');
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PrintVersion"] != null && (bool)Session["PrintVersion"])
            {
                this.Visible = false;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            StringBuilder scriptString = new StringBuilder();
            scriptString.Append("<script type='text/javascript' language='JavaScript'>\n");

            scriptString.Append("function resetFind(e) {\n");

            scriptString.Append("var keyCode = e.keyCode;\n");
            scriptString.Append("if (keyCode == 13) {\n");
            scriptString.Append("FindValue();\n");
            scriptString.Append("if(e.stopPropagation != null) e.stopPropagation();\n");
            scriptString.Append("if(e.preventDefault != null) e.preventDefault();\n");
            scriptString.Append("e.cancelBubble = true; e.returnValue = false; }\n");

            scriptString.Append("else {var indicator = igtbl_getElementById('FindIndicator');\n");
            scriptString.Append("var rowId = igtbl_getElementById('SelectedRowId');\n");
            scriptString.Append("indicator.value='Find'; rowId.value='';\n");
            scriptString.Append("}}\n");

            scriptString.Append("function FindValue() {\n");
            scriptString.Append("var indicator = igtbl_getElementById('FindIndicator');\n");
            scriptString.Append("var eVal = igtbl_getElementById('FindVal');\n");
            scriptString.Append("var rowId = igtbl_getElementById('SelectedRowId');\n");
            scriptString.Append("findValue = eVal.value;\n");
            scriptString.Append("var re = new RegExp(findValue, 'gi');\n");
            scriptString.Append("if(indicator.value=='Find') {\n");
            scriptString.Append("var oCell = " + linkedGridId + ".find(re);\n");
            scriptString.Append("if(oCell != null) {\n");
            scriptString.Append("indicator.value='Find Next';\n");
            scriptString.Append("var row = oCell.Row.ParentRow;\n");
            scriptString.Append("while(row != null) {\n");
            scriptString.Append("row.setExpanded(true);\n");
            scriptString.Append("row = row.ParentRow; }\n");
            scriptString.Append("igtbl_clearSelectionAll(" + linkedGridId + ".Id);\n");
            scriptString.Append("oCell.scrollToView();\n");
            scriptString.Append("oCell.Row.setSelected(true); rowId.value=oCell.Row.Id;}}\n");
            scriptString.Append("else { var oCell = " + linkedGridId + ".findNext();\n");
            scriptString.Append("if(oCell == null) { indicator.value='Find'; }\n");
            scriptString.Append("else {var row = oCell.Row.ParentRow;\n");
            scriptString.Append(" while(row != null) {row.setExpanded(true);\n");
            scriptString.Append("row = row.ParentRow;}\n");
            scriptString.Append("if(rowId.value== oCell.Row.Id) { FindValue();}\n");
            scriptString.Append("else {rowId.value=oCell.Row.Id;\n");
            scriptString.Append("igtbl_clearSelectionAll(" + linkedGridId + ".Id);\n");
            scriptString.Append("oCell.scrollToView();\n");
            scriptString.Append("oCell.Row.setSelected(true);\n}}}}");
            scriptString.Append("</script>\n");
            writer.Write(scriptString.ToString());
        }
    }
}