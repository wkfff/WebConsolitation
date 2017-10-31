using System;
using System.Data;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Web;
using System.IO;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Food_0001_0002 : CustomReportPage
    {
        private DateTime lastDate;
        private DateTime currentDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/Food_0001_0002/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/Food_0001_0002/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"Food_0001_0003\" bounds=\"x=0;y=400;width=768;height=100\" openMode=\"incomes\"/></touchElements>"));
            
            string multitouchIcon = String.Empty;
            multitouchIcon = "<img src='../../../images/detail.png'>";
            detalizationIconDiv.InnerHtml = String.Format("<table><tr><td><a href='webcommand?showPinchReport=Food_0001_0002_v'>{0}</a></td>", multitouchIcon);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0001_0002_incomes_date");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dtDate);

			lastDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0]["��������"].ToString(), 3);
			currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[1]["��������"].ToString(), 3);

			UserParams.PeriodCurrentDate.Value = dtDate.Rows[1]["��������"].ToString();
			UserParams.PeriodLastDate.Value = dtDate.Rows[0]["��������"].ToString();

            InitializeDescription();
            InitializeChart();
        }

        #region �����

        private DataTable dt;

        private void InitializeDescription()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("Food_0001_0002_Text");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "�����", dt);

            string grownGoods = String.Empty;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
				if (!(dt.Rows[i]["���� �������� (�������� � ����������� �������) ������"].ToString().Contains("-")))
                {
					grownGoods += String.Format(" {0}&nbsp;<nobr>(��&nbsp;<span class='DigitsValue'>{1:N2}%</span>)</nobr>,", dt.Rows[i]["�����"].ToString().ToLower(), dt.Rows[i]["���� �������� (�������� � ����������� �������) ������"]);
                }
            }
			if (String.IsNullOrEmpty(grownGoods))
				grownGoods = "�� �������� ���� ����� ��� �� �������� ������� �� �����������";

            string fallingGoods = String.Empty;
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
				if (dt.Rows[i]["���� �������� (�������� � ����������� �������) ������"].ToString().Contains("-"))
                {
					fallingGoods += String.Format(" {0}&nbsp;<nobr>(��&nbsp;<span class='DigitsValue'>{1:N2}%</span>)</nobr>,", dt.Rows[i]["�����"].ToString().ToLower(), dt.Rows[i]["���� �������� (�������� � ����������� �������) ������"]);
                }
            }
			if (String.IsNullOrEmpty(fallingGoods))
				fallingGoods = "�� �������� ���� �������� ��� �� �������� ������� �� �����������";

            lbDescription.Text = String.Format("<div style='padding-left: 29px; clear: both'>��������� ���� �� �������� ������� ��&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy}</span>&nbsp;(��������� �� ������ �&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>)</div><table style='margin-top: 12px'><tr><td align='right' style='color: white'><span class='DigitsValue'>���������� ����������&nbsp;���</span></td><td style='padding-left: 15px'><img src='../../../images/arrowRedUpBB.png'></td><td style='padding-left: 15px'>{2}</td></tr><tr><td align='right' style='color: white'><span  class='DigitsValue' style='padding-top: 5px'>���������� ��������&nbsp;���</span></td><td style='padding-left: 15px; padding-top: 5px'><img src='../../../images/arrowGreenDownBB.png'></td><td style='padding-left: 15px; padding-top: 5px'>{3}</td></tr></table>", lastDate, currentDate, grownGoods.TrimEnd(','), fallingGoods.TrimEnd(','));
        }

        #endregion
     
        #region ���������

        private DataTable dtChart;

        private void InitializeChart()
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("Food_0001_0002_charts");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
			
			CustomParam foodParam = UserParams.CustomParam("food");
			foreach (DataRow row in dtChart.Rows)
			{
				foodParam.Value = row["MDX"].ToString();
				DataTable dtData = new DataTable();
				query = DataProvider.GetQueryText("Food_0001_0002_chart_data");
				DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "�������", dtData);
                query = DataProvider.GetQueryText("Food_0001_0002_beloyarskiy");
                DataRow beloyarRow = DataProvider.GetDataTableForChart("Food_0001_0002_beloyarskiy", DataProvidersFactory.SpareMASDataProvider).Rows[0];
				object[] data = new object[8];
                ParseData(dtData, data, beloyarRow);
				double minValue, maxValue;
                if (Double.TryParse(data[0].ToString(), out minValue) && (minValue < Convert.ToDouble(row["������� "].ToString())))
				{
                    row["������� "] = data[0];
                    row["���� �������� (�������� � ����������� �������) �������"] = data[1];
                    row["���������� ���������� � ������� ���� �������"] = data[2];
                    row["������� �����"] = data[3];
				}
                if (Double.TryParse(data[4].ToString(), out maxValue) && (maxValue > Convert.ToDouble(row["�������� "].ToString())))
				{
                    row["�������� "] = data[4];
                    row["���� �������� (�������� � ����������� �������) ��������"] = data[5];
                    row["���������� ���������� � ������� ���� ��������"] = data[6];
                    row["�������� �����"] = data[7];
				}
			}

			dtChart.Columns.Remove("MDX");

            IPadElementHeader1.Text = dtChart.Rows[0][0].ToString();
            IPadElementHeader2.Text = dtChart.Rows[1][0].ToString();
            IPadElementHeader3.Text = dtChart.Rows[2][0].ToString();
            IPadElementHeader4.Text = dtChart.Rows[3][0].ToString();
            IPadElementHeader5.Text = dtChart.Rows[4][0].ToString();
            IPadElementHeader6.Text = dtChart.Rows[5][0].ToString();
			
			#region ��������� ������ �������� � ����������, ����� ������� ������������ ��������

			UltraChartFood_0001_0002_Chart1.LabelAreaHeight = 100;
			UltraChartFood_0001_0002_Chart2.LabelAreaHeight = 100;
			UltraChartFood_0001_0002_Chart3.LabelAreaHeight = 100;
			UltraChartFood_0001_0002_Chart4.LabelAreaHeight = 100;
			UltraChartFood_0001_0002_Chart5.LabelAreaHeight = 100;
			UltraChartFood_0001_0002_Chart6.LabelAreaHeight = 100;
			UltraChartFood_0001_0002_Chart1.City = "����";
			UltraChartFood_0001_0002_Chart2.City = "����";
			UltraChartFood_0001_0002_Chart3.City = "����";
			UltraChartFood_0001_0002_Chart4.City = "����";
			UltraChartFood_0001_0002_Chart5.City = "����";
			UltraChartFood_0001_0002_Chart6.City = "����";

			#endregion
			
			UltraChartFood_0001_0002_Chart1.Source = dtChart.Rows[0];
            UltraChartFood_0001_0002_Chart2.Source = dtChart.Rows[1];
            UltraChartFood_0001_0002_Chart3.Source = dtChart.Rows[2];
            UltraChartFood_0001_0002_Chart4.Source = dtChart.Rows[3];
            UltraChartFood_0001_0002_Chart5.Source = dtChart.Rows[4];
            UltraChartFood_0001_0002_Chart6.Source = dtChart.Rows[5];
        }

        #endregion

		#region ������ ��������������� ��������

		/// <summary>
		/// ��������� ������� � �������. �� ������ ������: ����������� � ������������ �������� �������� �� ��
		/// </summary>
		/// <param name="dtData">������� � �������</param>
		/// <param name="results">������ � ������������</param>
        protected void ParseData(DataTable dtData, object[] results, DataRow beloyarRow)
		{
			if (dtData == null || dtData.Rows.Count == 0)
			{
				for (int i = 0; i < results.Length; ++i)
					results[i] = DBNull.Value;
				return;
			}
			double[] gmValues = new double[dtData.Rows.Count / 2];
			string[] gmRegions = new string[dtData.Rows.Count / 2];
			for (int i = 0; i < dtData.Rows.Count - 1; i += 2)
			{
				int index = i / 2;
				DataRow row = dtData.Rows[i];
				DataRow prevRow = dtData.Rows[i + 1];
				double value = 0, prevValue = 0, delta = 0, rate = 0;
                if (row[0].ToString().Contains("���������� ������������� �����"))
                {
                    value = Convert.ToDouble(MathHelper.GeoMean(row, 1, 1, 0, beloyarRow["������� ����"]));
                    prevValue = Convert.ToDouble(MathHelper.GeoMean(prevRow, 1, 1, 0, beloyarRow["���� �� ���������� ����"]));
                }
                else
                {
                    value = MathHelper.GeoMean(row, 1, row.ItemArray.Length - 1, 1, 0);
                    prevValue = MathHelper.GeoMean(prevRow, 1, prevRow.ItemArray.Length - 1, 1, 0);
                }
				delta = value - prevValue;
				rate = (value / prevValue - 1) * 100;
				gmValues[index] = value;
				string region = row[0].ToString().Split(';')[0].Replace("����� ", String.Empty).Replace(" ������������� �����", String.Empty);
				gmRegions[index] = MergeTriple(region, rate, delta);
			}
			Array.Sort(gmValues, gmRegions);
			SplitTriple(gmRegions[0], out results[3], out results[1], out results[2]);
			results[0] = gmValues[0];
			int n = gmValues.Length - 1;
			SplitTriple(gmRegions[n], out results[7], out results[5], out results[6]);
			results[4] = gmValues[n];
		}

		protected string MergeTriple(string name, double rate, double delta)
		{
			return String.Format("{0};{1:N2};{2:N2}", name, rate, delta);
		}

		protected void SplitTriple(string triple, out object name, out object rate, out object delta)
		{
			string[] data = triple.Split(';');
			name = data[0];
			rate = Convert.ToDouble(data[1]);
			delta = Convert.ToDouble(data[2]);
		}

		#endregion
	}
}
