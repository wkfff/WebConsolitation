using System;
using System.Data;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.reports.SGM;
using System.Drawing;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SGM_0009 : CustomReportPage 
    {
        protected int year;
        protected string months;
        protected bool useLongNames = false; 
        protected string dies;
        protected string groupName;
        protected DataTable dtFullData;
        protected SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        protected readonly SGMSupport supportClass = new SGMSupport();

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataObject.reportFormRotator = dataRotator;
            base.Page_Load(sender, e);
            dataRotator.formNumber = 1;   
            dataObject.InitObject();
            dataRotator.FillDeseasesList(null, 0);
            year = dataRotator.GetLastYear();
            months = dataRotator.GetMonthParamIphone();
            dies = dataRotator.GetDeseaseCodes(0);
            groupName = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll));
            FillComponentData();
        }

        protected virtual DataTable GetGridDataSet()
        {
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            dataObject.useLongNames = useLongNames;

            for (int i = 0; i < 2; i++)
            {
                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctAbs,
                    Convert.ToString(year - i),
                    months,
                    string.Empty,
                    groupName,
                    dies);

                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctRelation,
                    Convert.ToString(i * 3 + 1));

                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctRank,
                    Convert.ToString(i * 3 + 2));
            }
            return dataObject.FillData(2);
        }

        protected virtual string GetPercentText(DataTable dt)
        {
            double percent = 100 * (1 - GetRFValue(dt, 0) / GetRFValue(dt, 1));
            LabelPercentText.Text = string.Empty;
            
            if (percent > 0)
            {
                image.ImageUrl = "~/images/arrowGreenDownBB.png";
                return string.Format("{0:N2}%", percent); ;
            }
            
            image.ImageUrl = "~/images/arrowRedUpBB.png";
            return string.Format("{0:N2}%", Math.Abs(percent)); ;
        }

        protected virtual double GetRFValue(DataTable dt, int isPrevValue)
        {
            DataRow[] dr = dt.Select(string.Format("{0} = 'РФ'", dt.Columns[0].ColumnName));
            return Convert.ToDouble(dr[0][2 + isPrevValue * 3]);
        }

        protected virtual void FillComponentData()
        {
            DataTable dtGrid = GetGridDataSet();
            dtFullData = dataObject.CloneDataTable(dtGrid);

            LabelMax11.Text = string.Format("{0}", dtGrid.Rows[0][0]);
            LabelMax21.Text = string.Format("{0}", dtGrid.Rows[1][0]);
            LabelMax31.Text = string.Format("{0}", dtGrid.Rows[2][0]);
            LabelMax41.Text = string.Format("{0}", dtGrid.Rows[3][0]);
            LabelMax51.Text = string.Format("{0}", dtGrid.Rows[4][0]);

            LabelMax12.Text = string.Format("{0:N2}", dtGrid.Rows[0][2]);
            LabelMax22.Text = string.Format("{0:N2}", dtGrid.Rows[1][2]);
            LabelMax32.Text = string.Format("{0:N2}", dtGrid.Rows[2][2]);
            LabelMax42.Text = string.Format("{0:N2}", dtGrid.Rows[3][2]);
            LabelMax52.Text = string.Format("{0:N2}", dtGrid.Rows[4][2]);

            LabelMin11.Text = string.Format("{0}", dtGrid.Rows[dtGrid.Rows.Count - 1][0]);
            LabelMin21.Text = string.Format("{0}", dtGrid.Rows[dtGrid.Rows.Count - 2][0]);
            LabelMin31.Text = string.Format("{0}", dtGrid.Rows[dtGrid.Rows.Count - 3][0]);
            LabelMin41.Text = string.Format("{0}", dtGrid.Rows[dtGrid.Rows.Count - 4][0]);
            LabelMin51.Text = string.Format("{0}", dtGrid.Rows[dtGrid.Rows.Count - 5][0]);

            LabelMin12.Text = string.Format("{0:N2}", dtGrid.Rows[dtGrid.Rows.Count - 1][2]);
            LabelMin22.Text = string.Format("{0:N2}", dtGrid.Rows[dtGrid.Rows.Count - 2][2]);
            LabelMin32.Text = string.Format("{0:N2}", dtGrid.Rows[dtGrid.Rows.Count - 3][2]);
            LabelMin42.Text = string.Format("{0:N2}", dtGrid.Rows[dtGrid.Rows.Count - 4][2]);
            LabelMin52.Text = string.Format("{0:N2}", dtGrid.Rows[dtGrid.Rows.Count - 5][2]);

            LabelRFCurrentPart1.Width = Unit.Empty;
            LabelRFCurrentPart1.Height = 25;
            LabelRFCurrentPart2.Width = Unit.Empty;
            LabelRfValueCurrent.Width = Unit.Empty;
            LabelRfValuePrev.Width = Unit.Empty;
            LabelPercentText.Width = Unit.Empty;
            LabelRFPrev.Width = Unit.Empty;

            LabelRFCurrentPart1.Text = string.Format("Общая инф. заболеваемость по РФ");
            LabelRFCurrentPart2.Text = string.Format("{1} {0}    ", year, supportClass.GetMonthLabelShort(months));
            LabelMeasure.Text = " на 100 тыс.";
            LabelRfValueCurrent.Text = string.Format("{0:N2}   ", GetRFValue(dtGrid, 0));
            LabelRFPrev.Text = string.Format("{1} {0}     ", year - 1, supportClass.GetMonthLabelShort(months));
            LabelRfValuePrev.Text = string.Format("{0:N2}", GetRFValue(dtGrid, 1));
            LabelProcent.Text = GetPercentText(dtGrid);
            LabelProcent.CssClass = "margin-left: 6px";

            Label1.ForeColor = Color.FromArgb(0xE64140);
            Label1.Font.Bold = true;
            Label1.Font.Size = FontUnit.Parse("17px");
            Label1.Width = 180;
            Label2.ForeColor = Color.FromArgb(0x1CD618);
            Label2.Font.Bold = true;
            Label2.Width = 182;
            Label2.Font.Size = FontUnit.Parse("17px");

        }
    }
}

