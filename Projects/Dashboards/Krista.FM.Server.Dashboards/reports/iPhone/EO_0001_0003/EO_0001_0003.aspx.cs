using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class EO_0001_0003 : CustomReportPage
    {
        private DateTime currentDate;
        private DataTable addDt;

        private CustomParam currentPeriod;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            currentPeriod = UserParams.CustomParam("current_period");

            currentDate = CubeInfo.GetLastDate(DataProvidersFactory.PrimaryMASDataProvider, "EO_0001_0003_lastDate");
            currentPeriod.Value = CRHelper.PeriodMemberUName("[������__������].[������__������]", currentDate, 4);

            //AddGridCaption.Text = "������� � ������ ������� ��������  ����������� �������������� (����� 50% � ������ �������������)";
            AddGridBrick.BrowserSizeAdapting = false;
            AddGridBrick.Height = Unit.Empty;
            AddGridBrick.Width = Unit.Empty;
            AddGridBrick.RedNegativeColoring = false;
            AddGridBrick.HeaderVisible = false;
            AddGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(AddGridBrick_InitializeLayout);
            AddGridBrick.Grid.InitializeRow += new InitializeRowEventHandler(AddGridBrick_InitializeRow);

            GridCaption.Text = "����� 50% �������� ���������� ������� � ������ �������������";
            DateCaption.Text = String.Format("�� {0:dd.MM.yyyy}", currentDate.AddMonths(1));

            AddGridDataBind();
        }

        #region ����������� ��������������� �����

        private void AddGridDataBind()
        {
            string query = DataProvider.GetQueryText("EO_0001_0003_addGrid");
            addDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "����������", addDt);

            if (addDt.Rows.Count > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("��������", typeof(string)));
                dt.Columns.Add(new DataColumn("��������", typeof(string)));
                dt.Columns.Add(new DataColumn("��� ������", typeof(string)));

                foreach (DataRow row in addDt.Rows)
                {
                    string customerName = GetStringDTValue(row, "��������");
                    customerName = customerName.Replace("(�������� �������� �������������� ���������)", String.Empty);
                    customerName = customerName.Replace("(��������������� �������� �������� �������� �������������� ���������)", String.Empty);
                    string objectName = GetStringDTValue(row, "������");
                    string objectCode = GetStringDTValue(row, "��� �������");
                    double contractRank = GetDoubleDTValue(row, "���� ���������");
                  
                    if (contractRank == 0)
                    {
                        DataRow groupRow = dt.NewRow();
                        groupRow[0] = customerName;
                        groupRow[2] = contractRank;
                        dt.Rows.Add(groupRow);
                    }

                    string subjectBudget = GetBudgetCompete(row, "������ �������", "������ �������� ��");
                    string moBudget = GetBudgetCompete(row, "������ ��", "������ �������������� �����������");
                    string othersBudget = GetBudgetCompete(row, "������", "������ ���������");

                    DataRow objectRow = dt.NewRow();
                    objectRow[0] = String.Format("<a href='http://monitoring.yanao.ru/site/reports/EO_0001_0003/Index.aspx?paramlist=object_code={1}'>{0}</a>", objectName, objectCode);
                    objectRow[1] = String.Format("{0}{1}{2}", subjectBudget, moBudget, othersBudget);
                    dt.Rows.Add(objectRow);
                }

                dt.AcceptChanges();
                AddGridBrick.DataTable = dt;
            }
        }

        private string GetBudgetCompete(DataRow row, string budgetName, string fullBugdetName)
        {
            double masteredVolume = GetDoubleDTValue(row, "������� " + budgetName);
            double fundedVolume = GetDoubleDTValue(row, "�������������� " + budgetName);
            double masteredPercent = GetDoubleDTValue(row, "������� �������� " + budgetName);
            double buildYear = GetDoubleDTValue(row, "������ �������������");
            double debtVolume = GetDoubleDTValue(row, "����������� ������������� " + budgetName);
            string indicatorImage = String.Format("<img style=\"FLOAT: center;\" src=\"../../../Images/{0}.png\"/>", masteredPercent < 0.3 ? "ballRedBB": "ballYellowBB");

            if (masteredVolume != Double.MinValue && fundedVolume != Double.MinValue && masteredPercent != Double.MinValue && buildYear != Double.MinValue && debtVolume != Double.MinValue && masteredPercent < 0.5)
            {
//                return String.Format("{5}&nbsp;� ������ �������������&nbsp;(<b>{0}</b>&nbsp;���) ��&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;�������&nbsp;<b>{2:N2}</b>&nbsp;���.���. (<b>{4:P2}</b>) ��&nbsp;<b>{3:N2}</b>&nbsp;���.���. ����������� �������������� ��&nbsp;<b>{6}</b>.<br/>" +
//                           "����������� ������������� �����&nbsp;<b>{7}</b>&nbsp;��&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;���������&nbsp;<b>{8:N2}</b>&nbsp;���.���.<br/>",
//                        buildYear, currentDate.AddMonths(1), masteredVolume, fundedVolume, masteredPercent, indicatorImage, genetiveBugdetName, instrumentalBudgetName, debtVolume);

                return String.Format("{0}&nbsp;{1}<br/>� ������ ������������� ({2}&nbsp;���):<br/>- ��������&nbsp;{3}<br/>- �������&nbsp;{4} ({5})<br/>- ����������� �������������&nbsp;{6}", indicatorImage, GetTextSpan(fullBugdetName), GetDigitSpan(buildYear, "", ""),
                    GetDigitSpan(fundedVolume, "N2", "���.���."), GetDigitSpan(masteredVolume, "N2", "���.���."), GetDigitSpan(masteredPercent, "P2", ""), GetDigitSpan(debtVolume, "N2", "���.���."));
            }
            return String.Empty;
        }

        private string GetDigitSpan(double value, string format, string unit)
        {
            return String.Format("<span style='font-size:16px;font-family: Arial;color: white;'>{0}</span>{1}", value.ToString(format), unit == String.Empty ? unit : "&nbsp;" + unit);
        }

        private string GetTextSpan(string text)
        {
            return String.Format("<span style='font-size:16px;font-family: Arial;color: white;'>{0}</span>", text);
        }

        private void AddGridBrick_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.White;

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[0].Width = 330;
            e.Layout.Bands[0].Columns[1].Width = 430;

            e.Layout.Bands[0].Columns[2].Hidden = true;
        }

        protected void AddGridBrick_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count - 1;
            int rowType = -1;
            if (e.Row.Cells[cellCount].Value != null && e.Row.Cells[cellCount].Value.ToString() != String.Empty)
            {
                rowType = Convert.ToInt32(e.Row.Cells[cellCount].Value);
            }

            string customerName = e.Row.Cells[0].Value.ToString();

            if (rowType == 0)
            {
                e.Row.Style.BorderDetails.WidthTop = 3;
                e.Row.Cells[0].ColSpan = 2;
                e.Row.Cells[0].Style.Font.Size = 14;
            }
            else
            {
                e.Row.Cells[0].Value = String.Format("<a href='webcommand?showPinchReport=EO_0001_0002_BUILDERCUSTOMER={0}' style='color:green'>{1}</a>", CustomParams.GetBuilderCustomerIdByName(customerName), customerName.Replace("\"", "&quot;"));
                e.Row.Cells[0].Style.ForeColor = Color.FromArgb(209, 209, 209);

                if (e.Row.Index == e.Row.Band.Grid.Rows.Count - 1)
                {
                    e.Row.Style.BorderDetails.WidthBottom = 3;
                }
            }
        }

        private static Double GetDoubleDTValue(DataRow row, string columnName)
        {
            return GetDoubleDTValue(row, columnName, Double.MinValue);
        }

        private static Double GetDoubleDTValue(DataRow row, string columnName, double defaultValue)
        {
            if (row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
            {
                return Convert.ToDouble(row[columnName].ToString());
            }
            return defaultValue;
        }

        private static string GetStringDTValue(DataRow row, string columnName)
        {
            if (row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
            {
                return row[columnName].ToString();
            }
            return String.Empty;
        }

        #endregion
    }
}