using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Server.Dashboards.Core
{
    public static class IndexPageHelper
    {
        public static Control GetReportTable(DataTable allowedReports, TemplateTypes templateType)
        {
            Panel reports = new Panel();
            foreach (DataRow sourceRow in allowedReports.Rows)
            {
                // Если он верхнего уровня
                if (String.IsNullOrEmpty(sourceRow["PARENTID"].ToString()))
                {
                    reports.Controls.Add(MakeReportTable(sourceRow, 0, allowedReports, templateType));
                }
                else
                {
                    // иначе пропускаем
                }
            }
            return reports;
        }

        private static HtmlTable MakeReportTable(DataRow row, int indent, DataTable allowedReports, TemplateTypes templateType)
        {
            HtmlTable htmlTable = new HtmlTable();
            htmlTable.Width = "100%";
            // Если это группа отчетов
            if (string.IsNullOrEmpty(row["DOCUMENTFILENAME"].ToString()))
            {
                // Добавим имя группы
                HtmlTableRow htmlRow = GetGroupTitle(row, indent, templateType);
                htmlTable.Rows.Add(htmlRow);

                // дочерние
                string id = row["Id"].ToString();
                DataRow[] rows = allowedReports.Select(string.Format("ParentId = {0}", id));
                if (rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        htmlRow = new HtmlTableRow();
                        HtmlTableCell htmlCell = new HtmlTableCell();
                        htmlCell.Controls.Add(MakeReportTable(rows[i], indent + 1, allowedReports, templateType));
                        htmlRow.Cells.Add(htmlCell);
                        //htmlRow.Style.Add("display", "");
                        htmlRow.Attributes.Add("class", "ReportRowFirstState");
                        htmlTable.Rows.Add(htmlRow);
                    }
                }
            }
            // это отчет 
            else
            {
                // Добавим имя со ссылкой
                HtmlTableCell htmlCell = GetReportNameCell(row, templateType);

                htmlCell.Style.Add("padding-left", String.Format("{0}px", indent * 30));
                AddFlags(htmlCell, row);
                Label code = new Label();
                code.Text = String.Format("({0})", row["CODE"]);
                code.CssClass = "ReportCode";
                htmlCell.Controls.Add(code);
                HtmlTableRow htmlRow = new HtmlTableRow();
                htmlRow.Cells.Add(htmlCell);
                htmlTable.Rows.Add(htmlRow);
                // Описание
                htmlRow = new HtmlTableRow();
                htmlCell = new HtmlTableCell();
                htmlCell.InnerText = row[4].ToString();
                htmlCell.Attributes.Add("Class", "ReportDescription");
                htmlCell.Style.Add("padding-left", String.Format("{0}px", indent * 30));
                htmlRow.Cells.Add(htmlCell);
                htmlTable.Rows.Add(htmlRow);
            }
            return htmlTable;
        }

        private static HtmlTableCell GetReportNameCell(DataRow row, TemplateTypes templateType)
        {
            if (templateType == TemplateTypes.Web)
            {
                return GetBBReportNameCell(row);
            }
            else
            {
                return GetIPhoneReportNameCell(row);
            }
        }

        private static HtmlTableCell GetIPhoneReportNameCell(DataRow row)
        {
            HtmlTableCell htmlCell = new HtmlTableCell();
            // Если web администратор
            if (HttpContext.Current.Session["IsWebAdministrator"] != null &&
                (bool)(HttpContext.Current.Session["IsWebAdministrator"]))
            {
                // Добавим чекбокс для генератора.
                HtmlGenericControl cb = new HtmlGenericControl("input");
                cb.ID = "checkBoxGenerate_" + row["CODE"];
                cb.Attributes.Add("Name", "checkBoxGenerate_" + row["CODE"]);
                cb.Attributes.Add("type", "checkbox");
                htmlCell.Controls.Add(cb);
            }
            Label title = new Label();
            title.CssClass = "ReportTitle";
            title.Text = row["NAME"] + " ";
            htmlCell.Controls.Add(title);

            HyperLink link = new HyperLink();
            link.NavigateUrl = "~/reports/IPhone/Index.aspx?reportid=" + row["CODE"];
            link.Text = "О";
            link.CssClass = "ReportTitle";
            htmlCell.Controls.Add(link);

            Label whiteSpace = new Label();
            whiteSpace.Text = "&nbsp;";
            htmlCell.Controls.Add(whiteSpace);

            if (!row["CODE"].ToString().ToLower().Contains("fo_0035_001") || row["CODE"].ToString().ToLower().Contains("fo_0035_0015"))
            {
                link = new HyperLink();
                link.NavigateUrl = "~/reports/IPhone/Index.aspx?reportid=" + row["CODE"] + "_V";
                link.Text = "В";
                link.CssClass = "ReportTitle";
                htmlCell.Controls.Add(link);

                whiteSpace = new Label();
                whiteSpace.Text = "&nbsp;";
                htmlCell.Controls.Add(whiteSpace);
            }

            if (!row["CODE"].ToString().ToLower().Contains("fo_0035_001"))
            {
                link = new HyperLink();
                link.NavigateUrl = "~/reports/IPhone/Index.aspx?reportid=" + row["CODE"] + "_H";
                link.Text = "Г";
                link.CssClass = "ReportTitle";
                htmlCell.Controls.Add(link);

                whiteSpace = new Label();
                whiteSpace.Text = "&nbsp;";
                htmlCell.Controls.Add(whiteSpace);
            }
            return htmlCell;
        }

        private static HtmlTableCell GetBBReportNameCell(DataRow row)
        {
            HtmlTableCell htmlCell = new HtmlTableCell();
            HyperLink link = new HyperLink();
            link.NavigateUrl = link.ResolveUrl("~/" + row["DOCUMENTFILENAME"]);
            link.Text = row["NAME"].ToString();
            link.CssClass = "ReportTitle";
            htmlCell.Controls.Add(link);
            return htmlCell;
        }

        //private static HtmlTableRow GetGroupTitle(DataRow row, int indent)
        //{
        //    HtmlTableRow htmlRow = new HtmlTableRow();
        //    HtmlTableCell htmlCell = new HtmlTableCell();
        //    htmlCell.Attributes.Add("onclick", "resize(this)");
        //    htmlCell.Attributes.Add("Class", "GroupReportExpandCellFirstState");
        //    htmlCell.Style.Add("padding-left", String.Format("{0}px", indent + 1 * 40));
        //    htmlCell.Style.Add("background-position", String.Format("{0}px Center", indent * 15));
        //    htmlRow.Cells.Add(htmlCell);
        //    htmlCell.InnerText = row["NAME"].ToString();
        //    return htmlRow;
        //}

        private static HtmlTableRow GetGroupTitle(DataRow row, int indent, TemplateTypes templateType)
        {           
            HtmlTableRow htmlRow = new HtmlTableRow();
            HtmlTableCell htmlCell = new HtmlTableCell();
            
            if (templateType == TemplateTypes.IPhone &&
                HttpContext.Current.Session["IsWebAdministrator"] != null &&
                (bool)(HttpContext.Current.Session["IsWebAdministrator"]))
            {
                // Добавим чекбокс для генератора.               
                HtmlGenericControl cb = new HtmlGenericControl("input");
                cb.ID = "checkBoxGroup_" + row["CODE"];
                cb.Attributes.Add("Name", "checkBoxGroup_" + row["CODE"]);
                cb.Attributes.Add("type", "checkbox");
                cb.Attributes.Add("onclick", "recheck(this.parentNode.parentNode.parentNode.parentNode, this.checked)");
                htmlCell.Controls.Add(cb);
            }

            htmlCell.Attributes.Add("onclick", "resize(this)");
            htmlCell.Attributes.Add("Class", "GroupReportExpandCellFirstState");
            htmlCell.Style.Add("padding-left", String.Format("{0}px", indent + 1 * 30));
            htmlCell.Style.Add("background-position", String.Format("{0}px Center", indent * 15));

            Label title = new Label();
            title.CssClass = "ReportTitle";
            title.Text = row["NAME"].ToString();
            htmlCell.Controls.Add(title);
            htmlRow.Cells.Add(htmlCell);
            return htmlRow;
        }

        private static void AddFlags(HtmlTableCell htmlCell, DataRow row)
        {
            TemplateFlags flag = (TemplateFlags)ParseFlags(row["FLAGS"].ToString());

            // картинки
            if (IsFavorite(flag))
            {
                Image image = new Image();
                image.ImageUrl = "~/images/BudgetReport.png";
                image.ToolTip = "Журнал \"Бюджет\"";
                htmlCell.Controls.Add(image);
            }

            if (IsImportant(flag))
            {
                Image image = new Image();
                image.ImageUrl = "~/images/ImportantReport.png";
                image.ToolTip = "Важный";
                htmlCell.Controls.Add(image);
            }

            if (IsNew(flag))
            {
                Image image = new Image();
                image.ImageUrl = "~/images/NewReport.png";
                image.ToolTip = "Новый";
                htmlCell.Controls.Add(image);
            }
            if (row["IsVisible"].ToString().ToLower() == "false")
            {
                Image image = new Image();
                image.ImageUrl = "~/images/Lock.png";
                image.ToolTip = "Заблокированый";
                htmlCell.Controls.Add(image);
            }
        }

        private static int ParseFlags(string s)
        {
            int value;
            Int32.TryParse(s, out value);
            return value;
        }

        private static bool IsFavorite(TemplateFlags v)
        {
            return (v & TemplateFlags.Favorite) == TemplateFlags.Favorite;
        }

        private static bool IsImportant(TemplateFlags v)
        {
            return (v & TemplateFlags.Important) == TemplateFlags.Important;
        }

        private static bool IsNew(TemplateFlags v)
        {
            return (v & TemplateFlags.New) == TemplateFlags.New;
        }
    }
}