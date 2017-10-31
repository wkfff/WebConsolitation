using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;
using Infragistics.UltraGauge.Resources;
using Dundas.Maps.WebControl;


namespace Krista.FM.Server.Dashboards.REGION_0010.UnEffectLessAdmin_Novosib
{
    public partial class _default : CustomReportPage
    {
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "�����");
            return dt;
        }

        CustomParam SelectItemGrid;
        CustomParam Chn;
        CustomParam Ks;
        CustomParam P3;
        CustomParam region;
        CustomParam P4;
        CustomParam RegionComparableDimention;

        string BN = "APPLEMAC-SAFARI";
        Double Coef = 1.0;
        Double Coef2 = 1.0;

        protected override void Page_PreLoad(object sender, System.EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            if (BN == "FIREFOX")
            {
                Coef = 0.908;
                Coef2 = 1.20;
            }
            if (BN == "IE")
            {
                Coef = 0.96;
                Coef2 = 1.11;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                Coef = 0.95;
                Coef2 = 0.97;
            }

            SelectItemGrid = UserParams.CustomParam("Param");
            Chn = UserParams.CustomParam("Chn");
            Ks = UserParams.CustomParam("Ks");
            P3 = UserParams.CustomParam("p3");
            P4 = UserParams.CustomParam("p4");
            region = UserParams.CustomParam("region");
            region.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            G.Width = (CustomReportConst.minScreenWidth - 15 - 5 - 5);
            G.Height = Unit.Empty;


            G2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5 - 5 - 5);
            G2.Height = Unit.Empty;

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15 - 5 - 5);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.85);

            if (BN == "IE")
            {
                C1.Width = (CustomReportConst.minScreenWidth * 1 - 12 - 4 - 5);
            }
            else
                if (BN == "APPLEMAC-SAFARI")
                {
                    C1.Width = (CustomReportConst.minScreenWidth * 1 - 6 - 4 - 5);
                }
                else
                {
                    C1.Width = (CustomReportConst.minScreenWidth * 1 - 8 - 4 - 5);
                }

            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;

            ////###########################################################################
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            //############################################################################

        }

        protected Dictionary<string, int> GetParam(string q, bool nofirst)
        {
            Dictionary<string, int> areas = new Dictionary<string, int>();


            DataTable dt = GetDSForChart(q);
            if (nofirst)
            {
                dt.Rows[0].Delete();
            }

            for (int i = dt.Rows.Count - 1; i > -1; i--)
            {
                areas.Add(dt.Rows[i].ItemArray[0].ToString(), 0);
            }

            return areas;

        }

        string realRegion = "Gubkinski";

        void GetOtherCubValue()
        {
            DataTable dt = GetDSForChart("G0");
            Chn.Value = dt.Rows[0][1].ToString().Replace(',', '.');


        }

        void MoveRang(int LastColumn, int ColRang)
        {
            //int LastColumn = G2.Columns.Count - 1;
            //int ColRang = 5;
            for (int i = 0; i < G2.Rows.Count; i++)
            {
                string[] b = { "\">" };
                string As0 = "";
                string BaseVal = "";
                try
                {
                    As0 = G2.Rows[i].Cells[ColRang].Text.Split(b, StringSplitOptions.None)[G2.Rows[i].Cells[ColRang].Text.Split(b, StringSplitOptions.None).Length - 2];
                    As0 += "\">";
                }
                catch { }
                //
                string As = G2.Rows[i].Cells[ColRang].Text;
                As = As.Split('>')[As.Split('>').Length - 1];
                try
                {
                    BaseVal = As.Split('(')[As.Split('(').Length - 2];
                    G2.Rows[i].Cells[ColRang].Text = BaseVal;
                }
                catch { }
                As = As.Split('(')[As.Split('(').Length - 1];

                As = As.Replace(")", "");
                G2.Rows[i].Cells[LastColumn].Text = As0 + "<font style=\"FLOAT: Right\">" + As + "&nbsp&nbsp&nbsp</font>";

                G2.Rows[i].Cells[LastColumn].Title = G2.Rows[i].Cells[ColRang].Title;
                G2.Rows[i].Cells[ColRang].Title = "";

            }
            G2.Columns[LastColumn].Width = 50;
            G2.Columns[LastColumn].Header.Style.HorizontalAlign = HorizontalAlign.Center;
        }


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //�������� ������� ������ ���� ��������� ���� ��� ������ �������� �����
            if (!Page.IsPostBack)
            {
                //�����..
            }
            RegionComparableDimention = UserParams.CustomParam("RegionComparableDimention");
            RegionComparableDimention.Value = RegionSettingsHelper.Instance.RegionBaseDimension;//RegionSettingsHelper.Instance.GetPropertyValue("RegionComparableDimention");
            Panel1.Visible = CheckBox1.Checked;
            Panel2.Visible = CheckBox1.Checked;
            //����� ����� ��� ����������, ������ ��� ���
            G.DataBind();
            object[] xz = {"",
"����",
"����",
"��",
"����� = ���� - ������ / ������ � ���� � ��",

"����� ��� = ����� / ���� � 100%",
                "",
                "����� = ����� / ���� � 100%",
                "",

"�� =(����� ��� ���� � ����� ���) / (����� ��� ���� � ����� ��� ���)",
"�� =(����� ��� ���� � ����� ���) / (����� ��� ���� � ����� ��� ���)"};
            //������-�������� ��
            object[] xz2 = {  RegionSettingsHelper.Instance.GetPropertyValue("UpLevelName"), 
                "���� �����", "� �����", "����", 
                "����=����/���� �����*100%", "���� ���=����/������*100%", "", "" };

            G2.DataBind();
            //����� ��� ���������� �����..
            GetRang(G2, 5 + 1, 1 == 2);
            GetRang(G2, 4 + 1, 1 == 2);
            GetRang(G2, 7 + 1, 1 == 1);
            GoMap();

            G2.Columns.Add("Rang1", "����");
            G2.Columns.Add("Rang2", "����");
            G2.Columns.Add("Rang3", "����");

            C1.DataBind();

            ColumnFormula();

            Hederglobal.Text = string.Format("������ ������������� �������� � ����� ����������� ���������������� � �������������� ���������� ({0})", RegionSettings.Instance.Name);
                //UserComboBox.getLastBlock(region.Value));
            Page.Title = Hederglobal.Text;


            G2.DisplayLayout.ScrollBar = Infragistics.WebUI.UltraWebGrid.ScrollBar.Never;
            G2.DisplayLayout.ScrollBarView = ScrollBarView.Vertical;

            //������ ��������!
            try
            {
                ScanGridPreText(7 + 1, G2.Rows[IndexMaxG2_6].Cells[7 + 1].Text.Split('(')[0], "<img style=\"FLOAT: left;\" src=\"../../../../images/starYellowBB.png\">");
                ScanGridPreText(7 + 1, G2.Rows[IndexMinG2_6].Cells[7 + 1].Text.Split('(')[0], "<img style=\"FLOAT: left;\" src=\"../../../../images/starGrayBB.png\">");
                ScanGridPreText(4 + 1, G2.Rows[IndexMinG2].Cells[4 + 1].Text.Split('(')[0], "<img style=\"FLOAT: left;\" src=\"../../../../images/starYellowBB.png\">");
                ScanGridPreText(4 + 1, G2.Rows[IndexMaxG2].Cells[4 + 1].Text.Split('(')[0], "<img style=\"FLOAT: left;\" src=\"../../../../images/starGrayBB.png\">");
                ScanGridPreText(5 + 1, G2.Rows[IndexMinG2_5].Cells[5 + 1].Text.Split('(')[0], "<img style=\"FLOAT: left;\" src=\"../../../../images/starYellowBB.png\">");
                ScanGridPreText(5 + 1, G2.Rows[IndexMaxG2_5].Cells[5 + 1].Text.Split('(')[0], "<img style=\"FLOAT: left;\" src=\"../../../../images/starGrayBB.png\">");

                MoveRang(G2.Columns.Count - 3, 8);
                MoveRang(G2.Columns.Count - 2, 5);
                MoveRang(G2.Columns.Count - 1, 6);

                //G2.Columns[G2.Columns.Count - 1].Move(8);
                G2.Columns[G2.Columns.Count - 1].Move(7);
                G2.Columns[G2.Columns.Count - 1].Move(6);
            }
            catch { }

            //������ �� ��� ���� ������ ������ � ������ �����
            for (int i = 0; i < G2.Columns.Count; i++)
            {
                //    G2.Rows[0].Cells[i].Style.HorizontalAlign = HorizontalAlign.Left;
            }
            //�������� ����
            G2.Rows.Insert(0, new UltraGridRow(xz));
            G2.Rows.Insert(1, new UltraGridRow(xz2));
            G2.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            G2.Rows[1].Cells[0].ColSpan = 10;
            G2.Rows[1].Cells[0].Text = "��������� ����������� �����";
            G2.Rows[1].Cells[0].Style.Font.Bold = 1 == 1;

            //������ ��������� �������, ���� ������ ��� ��� �� ����, ��� ��� ����� �� ������ �� �� ��
            G.Rows[0].Activate();
            G.Rows[0].Selected = 1 == 1;
            G.Rows[0].Activated = 1 == 1;

            try
            {
                G2.Rows[2].Activate();
                G2.Rows[2].Selected = 1 == 1;
                G2.Rows[2].Activated = 1 == 1;
            }
            catch { }

            Label5.Text = "������ ��������� � ������������ � �������������� ������������� ���������� ��������� �� 15 ������ 2009 �. � 322 �� ����� �� ���������� ����� ���������� ���������� ��������� �� 28 ���� 2007 �. � 825� � �������� ������������� ������������� ���������� ��������� �� 18 ������� 2010 �. � 1052";//RegionSettingsHelper.Instance.GetPropertyValue("RegionSubTitle");



        }

        void ScanGridPreText(int Column, string Val, string image)
        {
            for (int i = 0; i < G2.Rows.Count; i++)
            {
                if (Val == G2.Rows[i].Cells[Column].Text.Split('(')[0])
                {
                    G2.Rows[i].Cells[Column].Text = string.Format("{0}{1:### ### ###0.00}", image, G2.Rows[i].Cells[Column].Value);
                    if (image == "<img style=\"FLOAT: left;\" src=\"../../../../images/starYellowBB.png\">")
                    {
                        G2.Rows[i].Cells[Column].Title = Column == 8 ? "����� ������� ������� �������������" : "����� ������ ���� ������������� ��������";

                    }
                    else
                    {
                        G2.Rows[i].Cells[Column].Title = Column == 8 ? "����� ������ ������� �������������" : "����� ������� ���� ������������� ��������";
                    }
                }
            }
        }

        string[] columnFormul1 = {
            "����", 
            "����", 
            "������",
            "������",
            @"��", 
            @"����� =    ����� / ���� � 100%<br>
���������� �������������� ����� ������� 
������ ������������� �������� � ����� ���������������� � �������������� ���������� �� ������� ������������������ ������� �������� �� �� ���������� ���������� ������� ��������������� ������ � �������� �������������� � ��������� �� 100%

", 
            @"����� ��� = ����� / ���� � 100%<br>
���������� �������������� ����� ������� 
������ ������������� �������� � ����� ���������������� � �������������� ���������� �� ������� ������������������ ������� �������� �� � ��������� �� 100%
", 
            @"����� = ���� - ������ / ������ � ���� � ��<br>
���������� ����������� ����� ������� �������� �������� ����������������� �������� ��������� �� �� ���������� ���������� ������� ���. ������ � �������� �������������� � ����� �� �� �� ����� ����� �������� ����������������� �������� ��������� ��, ���������� �� ����� ����� �������� ������������������ ������� �������� �� � ����������� ��������; ���������� �������� �������� �� �������� ������������������ ������� �������� �� �� ���������� ���������� ������� ��������������� ������ � �������� ��������������
"
        };
        string[] columnFormul2 = {
            "�4 = (�14���)/1000", 
            "��", 
            "P14 = (�� - �� � ��) � �� � ��", 
            "��", 
            "��",
            "��",
            "��",            
            "��",
            "�5 = (�15���)/1000",
            "��",
            "P15 = (H� - H�) � �� � ��", 
            "��", 
            "��", 
            "��",
            "��",
            "�6 = (�16���)/1000",
            "��",
            "�16 = (�� - ��) � ��� � ����",
            "��",
            "��",
            "����",
            "���"};
        string[] columnFormul3 = {
            "�2= �21 + �22", 
            "�21= (��� / ��� - ��� /��� � ��) � �� /1000", 
            "���", 
            "���", 
            "���", 
            "��",
            "��",
            "�22= (��� / ��� - ��� /��� � ��) � �� /1000",
            "���",
            "���",
            "���",
            "��",
            "��"
             };
        void ColumnFormula2(UltraWebGrid G_, string[] columnFormul)
        {
            //�����*�����
            int lastColumn = G_.Columns.Count;
            G_.Columns.Add("�������");
            G_.Columns[lastColumn].Header.Caption = "�������";
            try
            {
                for (int i = 0; i < G_.Rows.Count; i++)
                {
                    G_.Rows[i].Cells[lastColumn].Text += columnFormul[i];
                }
            }
            catch { }

            G_.Columns[lastColumn].Width = 300;
            G_.Columns[lastColumn].CellStyle.Wrap = 1 == 1;
            G_.Columns[lastColumn].Move(1);
        }

        void ColumnFormula()
        {
            for (int j = 7; j < 9; j++)
                for (int i = 1; i < G.Columns.Count; i++)
                {
                    try
                    {
                        //������ ������� ������������� � ������� ����� �� ����������
                        G.Rows[j * 3].Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
                        G.Rows[j * 3].Cells[i].Style.BackgroundImage = "~/images/cornerRed.gif";
                        G.Rows[j * 3].Cells[i].Title = "������� ������������� �������";
                    }
                    catch { }
                    //TODO: �������� ���� ����
                }


            int lastColumn = G.Columns.Count;
            G.Columns.Add("�������");
            try
            {
                //�� ��� ��� �������, � - �� - ��!!!!!
                G.Rows[0 * 3].Cells[lastColumn].Text = columnFormul1[0];
                //G.Rows[0 * 3].Cells[0].Style.Font.Bold = 1 == 1;
                G.Rows[0 * 3 + 1].Hidden = 1 == 1;
                G.Rows[0 * 3 + 2].Hidden = 1 == 1;

                G.Rows[1 * 3].Cells[lastColumn].Text = columnFormul1[1];
                //G.Rows[1 * 3].Cells[0].Style.Font.Bold = 1 == 1;
                G.Rows[1 * 3 + 1].Hidden = 1 == 1;
                G.Rows[1 * 3 + 2].Hidden = 1 == 1;

                G.Rows[2 * 3].Cells[lastColumn].Text = columnFormul1[2];
                //G.Rows[2 * 3].Cells[0].Style.Font.Bold = 1 == 1;
                G.Rows[2 * 3 + 1].Hidden = 1 == 1;
                G.Rows[2 * 3 + 2].Hidden = 1 == 1;

                G.Rows[3 * 3].Cells[lastColumn].Text = columnFormul1[3];
                //G.Rows[3 * 3].Cells[0].Style.Font.Bold = 1 == 1;
                G.Rows[3 * 3 + 1].Hidden = 1 == 1;
                G.Rows[3 * 3 + 2].Hidden = 1 == 1;

                G.Rows[4 * 3].Cells[lastColumn].Text = columnFormul1[4];
                //G.Rows[4 * 3].Cells[0].Style.Font.Bold = 1 == 1;
                G.Rows[4 * 3 + 1].Hidden = 1 == 1;
                G.Rows[4 * 3 + 2].Hidden = 1 == 1;

                G.Rows[7 * 3].Cells[lastColumn].RowSpan = 3;
                G.Rows[7 * 3].Cells[lastColumn].Text = columnFormul1[7];
                G.Rows[7 * 3].Cells[0].Style.Font.Bold = 1 == 1;
                G.Rows[7 * 3].Cells[1].Style.Font.Bold = 1 == 1;
                G.Rows[7 * 3].Cells[2].Style.Font.Bold = 1 == 1;

                //G.Rows[7 * 3].Cells[2].Style.Font.Bold = 1 == 1;
                //G.Rows[7 * 3].Cells[3].Style.Font.Bold = 1 == 1;

                G.Rows[6 * 3].Cells[lastColumn].RowSpan = 3;
                G.Rows[6 * 3].Cells[lastColumn].Text = columnFormul1[6];
                G.Rows[6 * 3].Cells[0].Style.Font.Bold = 1 == 1;
                G.Rows[6 * 3].Cells[1].Style.Font.Bold = 1 == 1;
                G.Rows[6 * 3].Cells[2].Style.Font.Bold = 1 == 1;
                //G.Rows[6 * 3].Cells[2].Style.Font.Bold = 1 == 1;
                //G.Rows[6 * 3].Cells[3].Style.Font.Bold = 1 == 1;

                G.Rows[5 * 3].Cells[lastColumn].RowSpan = 3;
                G.Rows[5 * 3].Cells[lastColumn].Text = columnFormul1[5];
                G.Rows[5 * 3].Cells[0].Style.Font.Bold = 1 == 1;
                G.Rows[5 * 3].Cells[1].Style.Font.Bold = 1 == 1;
                G.Rows[5 * 3].Cells[2].Style.Font.Bold = 1 == 1;
                //G.Rows[5 * 3].Cells[2].Style.Font.Bold = 1 == 1;
                //G.Rows[5 * 3].Cells[3].Style.Font.Bold = 1 == 1;





            }
            catch { }
            //����������� ������� � �������
            G.Columns[lastColumn].Width = 300;
            G.Columns[lastColumn].CellStyle.Wrap = 1 == 1;
            G.Columns[lastColumn].Header.Caption = "�������";
            //������ ����� ������� ���� ���� ��� � �����
            G.Columns[lastColumn].Move(1);

        }



        Dictionary<string, int> param_3;

        System.Double FirstCell = 0;
        System.Double up = 0;
        System.Double down = 0;

        string CalcCell()
        {
            string res = "";
            System.Double OLOLO = down - up;
            System.Double OlOlO = 100.0 * (down / up - 1);
            int xz = FirstCell > down ? -1 : FirstCell == down ? 0 : 1;

            string image = "";
            if (OlOlO < 0)
            {
                image = "<img style=\"FLOAT: left;\" src=\"../../../../images/arrowGreenDownBB.png\">";
            }
            if (OlOlO > 0)
            {
                image = "<img style=\"FLOAT: left;\" src=\"../../../../images/arrowRedUpBB.png\">";
            }

            if (System.Double.IsInfinity(OlOlO))
            {
                return "-";
            }
            res = image + OlOlO.ToString("### ### ##0.00") + "%";
            return res;
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {

            //���������� �� ���� ��� G.DataSource = GETDS("G"); �� ��� �� ����� ��������� ���� ����������...
            G.Rows.Clear();
            G.Columns.Clear();
            G.Bands.Clear();

            DataTable dt = GetDSForChart("G");
            DataTable dtNew = new DataTable();
            try
            { }
            catch { }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dtNew.Columns.Add(dt.Columns[i].Caption, dt.Columns[0].DataType);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtNew.Rows.Add(new object[dt.Columns.Count]);
                dtNew.Rows.Add(new object[dt.Columns.Count]);
                dtNew.Rows.Add(new object[dt.Columns.Count]);
            }
            for (int i = 0; i < dtNew.Rows.Count / 3; i++)
            {
                dtNew.Rows[i * 3][0] = dt.Rows[i][0];
                dtNew.Rows[i * 3 + 1][0] = "<div style=\"FLOAT: Right;\">���������� ����������&nbsp&nbsp</div>";
                dtNew.Rows[i * 3 + 2][0] = "<div style=\"FLOAT: Right;\">���� ��������&nbsp&nbsp</div>";
                //���������� ����������

                for (int j = dt.Columns.Count - 1; j >= 1; j--)
                {
                    try
                    {
                        dtNew.Rows[i * 3 + 2][j] = "-";
                        dtNew.Rows[i * 3 + 1][j] = "-";

                        down = (System.Double)(System.Decimal)dt.Rows[i][j];
                        dtNew.Rows[i * 3][j] = down.ToString("### ### ##0.00");

                        up = (System.Double)(System.Decimal)dt.Rows[i][j - 1];

                        for (int x = 1; x < dt.Columns.Count - 1; x++)
                        {
                            try
                            {
                                FirstCell = (System.Double)(System.Decimal)dt.Rows[i][x];
                                if (FirstCell == 0) continue;
                                break;
                            }
                            catch { }
                        }

                        dtNew.Rows[i * 3 + 1][j] = ((System.Double)(down - up)).ToString("### ### ##0.00");
                        dtNew.Rows[i * 3 + 2][j] = CalcCell();
                    }
                    catch { }
                }
            }
            G.DataSource = dtNew;
        }

        int ld;
        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Caption = "����������";

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth((520) * Coef);
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.00");
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(((CustomReportConst.minScreenWidth - 820 - 140) / (e.Layout.Bands[0].Columns.Count - 1)) * Coef);
            }
            e.Layout.AllowSortingDefault = AllowSorting.No;
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[0].Text[0] == '_')
                {
                    e.Row.Cells[0].Style.Padding.Left = 15;
                    e.Row.Cells[0].Text = e.Row.Cells[0].Text.Remove(0, 1);
                }
                else
                {
                    if ((UltraWebGrid)(sender) != G)
                        e.Row.Cells[0].Style.Font.Bold = 1 == 1;
                }
                try
                {
                    e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split('_')[1];
                }
                catch { }
                if ((e.Row.Index == 0) & ((UltraWebGrid)(sender) != G))
                {
                    e.Row.Cells[0].Style.Font.Bold = 1 == 1;
                }
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (string.IsNullOrEmpty(e.Row.Cells[i].Text))
                    {
                        e.Row.Cells[i].Text = "-";
                    }
                }

            }
            catch { }
        }

        protected void Type1_Load(object sender, EventArgs e)
        {

        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        }

        void SetLengColumn(EndExportEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 6000;
            }
        }

        void SetCellFormat(EndExportEventArgs e, int Col, int Row, int CellHeight, int CellWidth, HorizontalCellAlignment HCA, VerticalCellAlignment VCA, Font F, int FHeight)
        {
            if (CellHeight >= 0)
            {
                e.CurrentWorksheet.Rows[Row].Height = CellHeight;
            }
            if (CellWidth >= 0)
            {
                e.CurrentWorksheet.Columns[Col].Width = CellWidth;
            }
            if (HCA != HorizontalCellAlignment.Default)
            {
                e.CurrentWorksheet.Rows[Row].Cells[Col].CellFormat.Alignment = HCA;
            }
            if (VCA != VerticalCellAlignment.Default)
            {
                e.CurrentWorksheet.Rows[Row].Cells[Col].CellFormat.VerticalAlignment = VCA;
            }
            if (F != null)
            {
                e.CurrentWorksheet.Rows[Row].Cells[Col].CellFormat.Font.Name = F.Name;

                if (F.Bold)
                {
                    e.CurrentWorksheet.Rows[Row].Cells[Col].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                }

            }
            if (FHeight > 0)
            {
                e.CurrentWorksheet.Rows[Row].Cells[Col].CellFormat.Font.Height = FHeight;
            }
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {

            for (int i = 0; i < G.Columns.Count + 10; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 6000;
                for (int j = 0; j < G.Rows.Count + 10; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].Value = "";
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Font.Name = "Arial";
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Font.Bold = ExcelDefaultableBoolean.False;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.FillPatternBackgroundColor = Color.White;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.FillPatternForegroundColor = Color.White;

                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.BottomBorderColor = Color.LightGray;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.BottomBorderStyle = CellBorderLineStyle.Default;

                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.LeftBorderColor = Color.LightGray;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.LeftBorderStyle = CellBorderLineStyle.Default;

                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.RightBorderColor = Color.LightGray;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.RightBorderStyle = CellBorderLineStyle.Default;

                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.TopBorderColor = Color.LightGray;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.TopBorderStyle = CellBorderLineStyle.Default;
                }
            }
            e.Workbook.Worksheets[0].Name = "������������� �������";
            e.CurrentWorksheet = e.Workbook.Worksheets["������������� �������"];
            SetLengColumn(e);

            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
            SetCellFormat(e, 0, 0, -1, -1, HorizontalCellAlignment.Default, VerticalCellAlignment.Default, new Font("Arial", 13, FontStyle.Bold), 270);
            e.CurrentWorksheet.Rows[0].Height = 1000;

            e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 4);
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label5.Text;
            e.CurrentWorksheet.Rows[1].Height = 1000;
            e.CurrentWorksheet.Rows[1].Cells[0].CellFormat.Font.Height = 240;
            e.CurrentWorksheet.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.MergedCellsRegions.Add(1, 0, 1, 3);

            e.CurrentWorksheet.Rows[2].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[2].Height = 1000;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.Font.Height = 270;
            e.CurrentWorksheet.MergedCellsRegions.Add(2, 0, 2, 3);

            DataTable dt = GetDSForChart("G");
            //dt.Columns.Remove(dt.Columns[1]);

            int offset = Export_DT(dt, 4, e, 1 == 1);
            e.CurrentWorksheet.Rows[3].Height = 0;
            //��������� 2 ������� ������ ��������� ���� :(
            //e.CurrentWorksheet.Rows[5].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[6].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[7].Hidden = 1 == 1;
            //e.CurrentWorksheet.Rows[8].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[9].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[10].Hidden = 1 == 1;
            //e.CurrentWorksheet.Rows[11].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[12].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[13].Hidden = 1 == 1;
            //e.CurrentWorksheet.Rows[14].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[15].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[16].Hidden = 1 == 1;
            //e.CurrentWorksheet.Rows[17].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[18].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[19].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[20].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[23].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[26].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            offset = 2;
            if (CheckBox1.Checked)
            {
                e.Workbook.Worksheets.Add("��������� ���������");
                e.CurrentWorksheet = e.Workbook.Worksheets["��������� ���������"];
                SetLengColumn(e);
                e.CurrentWorksheet.Rows[offset - 2].Cells[0].Value = Label2.Text;
                e.CurrentWorksheet.Rows[offset - 2].Cells[0].CellFormat.Font.Height = 270;
                e.CurrentWorksheet.Rows[offset - 2].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[offset - 2].Height = 1000;
                e.CurrentWorksheet.MergedCellsRegions.Add(offset - 2, 0, offset - 2, 4);
                e.CurrentWorksheet.Rows[offset - 1].Cells[0].CellFormat.Font.Height += 80;
                e.CurrentWorksheet.MergedCellsRegions.Add(offset - 1, 0, offset - 1, 5);


                GlobalDT2.Columns.Remove(GlobalDT2.Columns[GlobalDT2.Columns.Count - 2]);
                offset = Export_DT(GlobalDT2, offset, e, 1 == 2);
                e.CurrentWorksheet.Rows[1].Height = 0;
            }
            e.CurrentWorksheet.Columns[0].Width = 8000;
            e.CurrentWorksheet = e.Workbook.Worksheets["������������� �������"];
            e.CurrentWorksheet.Columns[0].Width = 24000;

        }

        int Export_DT(DataTable dt, int offset, EndExportEventArgs e, bool CalcTemp)
        {
            int xz = CalcTemp ? 3 : 1;
            for (int i = 0; i < dt.Columns.Count; i++)
            {

                try
                {
                    e.CurrentWorksheet.Rows[offset].Cells[i].Value = dt.Columns[i].Caption;
                    e.CurrentWorksheet.Rows[offset].Cells[i].CellFormat.FillPattern = FillPatternStyle.Default;
                    e.CurrentWorksheet.Rows[offset].Cells[i].CellFormat.FillPatternBackgroundColor = Color.Gray;
                    e.CurrentWorksheet.Rows[offset].Cells[i].CellFormat.FillPatternForegroundColor = Color.Gray;
                    e.CurrentWorksheet.Rows[offset].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                    e.CurrentWorksheet.Rows[offset].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Left;
                    e.CurrentWorksheet.Rows[offset].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                    e.CurrentWorksheet.Rows[offset].Height = !CalcTemp ? 1500 : 700;


                    for (int j = 0; j < dt.Rows.Count * xz; j += xz)
                    {
                        e.CurrentWorksheet.Rows[j + offset + 1].Height = 1000;
                        if (dt.Rows[j / xz][i].ToString()[0] == '_')
                        {
                            dt.Rows[j / xz][i] = "  " + dt.Rows[j / xz][i].ToString().Remove(0, 1);
                        }

                        e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].Value = dt.Rows[j / xz][i];
                        try
                        {
                            e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].Value = ((System.Decimal)(dt.Rows[j / xz][i])).ToString("### ### ##0.00");
                        }
                        catch { }
                        e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].CellFormat.Font.Height = 200;
                        e.CurrentWorksheet.Rows[j + offset + 2].Cells[i].CellFormat.Font.Height = 200;
                        e.CurrentWorksheet.Rows[j + offset + 3].Cells[i].CellFormat.Font.Height = 200;
                        e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                        e.CurrentWorksheet.Rows[j + offset + 2].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                        e.CurrentWorksheet.Rows[j + offset + 3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                        if (i != 0)
                        {
                            e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                            e.CurrentWorksheet.Rows[j + offset + 2].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                            e.CurrentWorksheet.Rows[j + offset + 3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                            if (CalcTemp)
                            {

                                try
                                {
                                    e.CurrentWorksheet.Rows[j + offset + 2].Cells[i].Value = string.Format("{0:### ### ##0.00}", (System.Decimal)(dt.Rows[j / xz][i]) - (System.Decimal)(dt.Rows[j / xz][i - 1]));
                                    e.CurrentWorksheet.Rows[j + offset + 3].Cells[i].Value = string.Format("{0:### ### ##0.00}", (((System.Decimal)(dt.Rows[j / xz][i]) / (System.Decimal)(dt.Rows[j / xz][i - 1]) - 1) * 100));
                                }
                                catch { }
                            }

                        }
                        else
                        {
                            e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Left;
                            e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                            e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].CellFormat.Font.Height = 200;
                            e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].CellFormat.Font.Name = "Arial";

                            if (CalcTemp)
                            {
                                e.CurrentWorksheet.Rows[j + offset + 2].Cells[i].CellFormat.Font.Name = "Arial";
                                e.CurrentWorksheet.Rows[j + offset + 3].Cells[i].CellFormat.Font.Name = "Arial";

                                e.CurrentWorksheet.Rows[j + offset + 2].Cells[i].Value = "���������� ����������";
                                e.CurrentWorksheet.Rows[j + offset + 2].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                                e.CurrentWorksheet.Rows[j + offset + 3].Cells[i].Value = "���� ��������";
                                e.CurrentWorksheet.Rows[j + offset + 3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                            }
                        }
                    }
                }
                catch { }

            }
            e.CurrentWorksheet.Rows[offset].Cells[0].Value = "����������";
            return dt.Rows.Count * xz + 6 + offset;
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = G.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            e.HeaderText = e.HeaderText.Replace("&quot;", "\"");
            if (col.Hidden)
            {
                offset++;
            }

        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            for (int j = 0; j < G.Rows.Count; j++)
            {
            }

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(G);
        }

        protected void C_DataBinding(object sender, EventArgs e)
        {
        }

        void C1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
            e.Text = "������������ ������ ��� �����������";
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.FontSizeBestFit = false;
            e.LabelStyle.Font = new Font("Verdana", 26);
        }
        protected void C1_DataBinding(object sender, EventArgs e)
        {
            C1.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(C1_InvalidDataReceived);
            //if (G.Columns.Count < 5)
            //{
            //    C1.EmptyChartText = "NON";
            //    return;
            //}

            DataTable dt = GetDSForChart("C2");
            dt.Rows[0][0] = @"���� ������������� �������� � ����� ������ �������� �� 
����������� ���������������� � �������������� ����������";
            dt.Rows[1][0] = @"���� ������������� �������� � ����� ������ �������� 
������������������ �������";
            System.Decimal max = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                if (max < (System.Decimal)(dt.Rows[0][i]))
                {
                    max = (System.Decimal)(dt.Rows[0][i]);
                }
                if (max < (System.Decimal)(dt.Rows[1][i]))
                {
                    max = (System.Decimal)(dt.Rows[1][i]);
                }
            }
            C1.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            C1.Axis.Y.RangeMax = (System.Double)max * 1.1;
            C1.Axis.Y.RangeMin = 0;
            C1.DataSource = dt;
        }

        protected void C2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("C2");
            dt.Rows[0][0] = @"����� ����������� �������� � ����� ������ �����������";
            dt.Rows[1][0] = @"����� ������������� �������� � ����� ������ �����������";
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {

        }

        int IndexMaxG2 = 0, IndexMinG2 = 0;
        int IndexMaxG2_5 = 0, IndexMinG2_5 = 0;
        int IndexMaxG2_6 = 0, IndexMinG2_6 = 0;
        protected void G2_DataBinding(object sender, EventArgs e)
        {
            G2.Rows.Clear();
            G2.Columns.Clear();
            G2.Bands.Clear();

            DataTable dt = GetDSForChart("G2");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    if (((System.Decimal)(dt.Rows[i][4 + 1])) > ((System.Decimal)(dt.Rows[IndexMaxG2][4 + 1])))
                    {
                        IndexMaxG2 = i;
                    }
                    if (((System.Decimal)(dt.Rows[i][4 + 1])) < ((System.Decimal)(dt.Rows[IndexMinG2][4 + 1])))
                    {
                        IndexMinG2 = i;
                    }
                    if (((System.Decimal)(dt.Rows[i][5 + 1])) > ((System.Decimal)(dt.Rows[IndexMaxG2_5][5 + 1])))
                    {
                        IndexMaxG2_5 = i;
                    }
                    if (((System.Decimal)(dt.Rows[i][5 + 1])) < ((System.Decimal)(dt.Rows[IndexMinG2_5][5 + 1])))
                    {
                        IndexMinG2_5 = i;
                    }
                }
                catch { }
            }

            dt.Columns.Add("������� ������������� ������������ ��������� �������, %", dt.Columns[1].DataType);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    dt.Rows[i][dt.Columns.Count - 1] = 100 * ((System.Decimal)(dt.Rows[IndexMaxG2][4 + 1]) - ((System.Decimal)(dt.Rows[i][4 + 1])))
                        / ((System.Decimal)(dt.Rows[IndexMaxG2][4 + 1]) - (System.Decimal)(dt.Rows[IndexMinG2][4 + 1]));

                }
                catch
                {
                    dt.Rows[i][dt.Columns.Count - 1] = 0;
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                if (((System.Decimal)(dt.Rows[i][7 + 1])) > ((System.Decimal)(dt.Rows[IndexMaxG2_6][7 + 1])))
                {
                    IndexMaxG2_6 = i;
                }
                if (((System.Decimal)(dt.Rows[i][7 + 1])) < ((System.Decimal)(dt.Rows[IndexMinG2_6][7 + 1])))
                {
                    IndexMinG2_6 = i;
                }
            }

            GlobalDT2 = dt;
            G2.DataSource = GlobalDT2;

        }
        DataTable GlobalDT2;

        void GetRang(UltraWebGrid dt, int indexColumn, bool reverce)
        {
            System.Decimal[] Values = new System.Decimal[dt.Rows.Count];
            int[] Nomber = new int[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    Values[i] = (System.Decimal)(dt.Rows[i].Cells[indexColumn].Value);
                    Nomber[i] = i;
                }
                catch
                {
                    Values[i] = 0;
                }
            }
            for (int i = 0; i < Values.Length; i++)
            {
                for (int j = i; j < Values.Length; j++)
                {


                    if (((Values[i] > Values[j]) & (!reverce)) || ((Values[i] < Values[j]) & (reverce)))
                    {
                        System.Decimal buf = Values[i];
                        Values[i] = Values[j];
                        Values[j] = buf;

                        int buf_ = Nomber[i];
                        Nomber[i] = Nomber[j];
                        Nomber[j] = buf_;
                    }
                }
            }

            int mine = 0;
            for (int i = 0; i < Nomber.Length; i++)
            {
                try
                {
                    if (Values[i] == Values[i - 1])
                    {
                        mine++;
                    }
                }
                catch { }
                if (reverce)
                {
                    dt.Rows[Nomber[i]].Cells[indexColumn].Text = string.Format("{0:### ##0.00}({1})", Values[i], i + 1 - mine);
                }
                else
                {
                    dt.Rows[Nomber[i]].Cells[indexColumn].Text = string.Format("{0:### ##0.00}({1})", Values[i], i + 1 - mine);
                }
            }
        }



        protected void G3_DataBinding(object sender, EventArgs e)
        {

        }
        protected void G3_DataBound(object sender, EventArgs e)
        {

        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Panel1.Visible = CheckBox1.Checked;
        }

        protected void C1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            Infragistics.UltraChart.Core.Primitives.Text textLabel;

            int textWidth = 1000;
            int textHeight = 12;
            if (C1.EmptyChartText != "NON")
                try
                {
                    textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                    textLabel.PE.Fill = Color.Black;
                    textLabel.bounds = new Rectangle(26, 243 - 7 + 1, textWidth, textHeight);
                    //textLabel.SetTextString("���� ������������� �������� � ����� ����� �������� �� ��������������� � ������������� ����������");
                    e.SceneGraph.Add(textLabel);
                    textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                    textLabel.PE.Fill = Color.Black;
                    textLabel.bounds = new Rectangle(26, 243 + 21 - 7 + 2, textWidth, textHeight);
                    //textLabel.SetTextString("���� ������������� �������� � ����� ����� ��������");
                    e.SceneGraph.Add(textLabel);
                }
                catch
                {

                }

        }

        protected void G2_InitializeLayout(object sender, LayoutEventArgs e)
        //protected void G2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Header.Caption = "�������";
            System.Double Coef2 = 1;
            if (BN == "FIREFOX")
            {
                Coef2 = 0.945;
            }
            if (BN == "IE")
            {
                Coef2 = 0.92;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                Coef2 = 0.95;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(170 * Coef2 * 0.765);
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Replace("���", "���");

            }
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = 2 == 2;
            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count-1], "### ##0.00");
        }
        //{
        //    Double cc = 1;
        //    if (BN == "FIREFOX")
        //    {
        //        cc = 0.98;
        //    }
        //    if (BN == "IE")
        //    {
        //        cc = 0.975;
        //    }
        //    if (BN == "APPLEMAC-SAFARI")
        //    {
        //        cc = 0.97;
        //    }


        //    e.Layout.Bands[0].Columns[0].Header.Caption = "�������";
        //    for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
        //    {
        //        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(145 * cc);
        //        e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
        //        e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
        //        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
        //        e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = 2 == 2;
        //    }
        //}

        protected void G2_InitializeRow(object sender, RowEventArgs e)
        {

        }
        //#########################################################
        #region map


        private void GoMap()
        {
            DundasMap.Shapes.Clear();

            if (RegionSettingsHelper.Instance.Name == "������������� �������")
            {
                //   DundasMap.LoadFromShapeFile(Server.MapPath(string.Format("../../../../maps/��������/��������/��������.shp")), "NAME", true);
                DundasMap.LoadFromShapeFile(Server.MapPath(string.Format("../../../../maps/���/���.shp")), "NAME", true);
            }
            else
            {
                DundasMap.LoadFromShapeFile(Server.MapPath(string.Format("../../../../maps/��������/����/��������.shp")), "NAME", true);
            }

            //DundasMap.LoadFromShapeFile(Server.MapPath("../../../../maps/��������/������_��������_�����������_�����/map.shp"), "NAME", true);
            //DundasMap.LoadFromShapeFile(Server.MapPath(string.Format("../../../../maps/{0}/{1}.shp",
            //    "����",//RegionSettingsHelper.Instance.GetRegionSetting("MapPathSubject"),
            //    //RegionSettingsHelper.Instance.GetRegionSetting("MapPathSubject")
            //    "����"))
            //    , "NAME", true);
            //DundasMap.LoadFromShapeFile(Server.MapPath("../../../../maps/��������/������_��������_�����������_�����/map.emt"), "NAME", true);

            //DundasMap.Serializer.Format = SerializationFormat.;
            //DundasMap.Serializer.Load(
            //    (Server.MapPath("../../../../maps/��������/������_��������_�����������_�����/map.shp")));

            SetupMap();
            // ��������� ����� �������
            FillMapData();
        }



        private void SetupMap()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            DundasMap.Viewport.EnablePanning = true;


            // ��������� �������
            Legend legend = new Legend("CompleteLegend");
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = "������� �������������\n ������������ ���������\n �������";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // ��������� ����
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Complete");
            DundasMap.ShapeFields["Complete"].Type = typeof(double);
            DundasMap.ShapeFields["Complete"].UniqueIdentifier = false;
            DundasMap.ShapeFields.Add("CompletePercent");
            DundasMap.ShapeFields["CompletePercent"].Type = typeof(double);
            DundasMap.ShapeFields["CompletePercent"].UniqueIdentifier = false;



            // ��������� ������� ���������
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "CompletePercent";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = "#FROMVALUE{### ##0.0##}% - #TOVALUE{### ##0.0##}%";
            DundasMap.ShapeRules.Add(rule);
        }
        #region ����������� �����


        /// <summary>
        /// ����� ����� �����
        /// </summary>
        /// <param name="map">�����</param>
        /// <param name="patternValue">������� ��� �����</param>
        /// <param name="searchFO">true, ���� ���� ��</param>
        /// <returns>��������� �����</returns>
        public static Shape FindMapShape(MapControl map, string patternValue, bool searchFO)
        {
            string subject = string.Empty;
            bool isRepublic = patternValue.Contains("����������");
            bool isTown = patternValue.Contains("�.");
            string[] subjects = patternValue.Split(' ');
            if (subjects.Length > 1)
            {
                // ���� ����� ������ ������ ������������� ���� ���������
                switch (subjects[0])
                {
                    case "���������":
                        {
                            subject = "�����";
                            break;
                        }
                    case "���������-����������":
                        {
                            subject = "���������-���������";
                            break;
                        }
                    case "���������-����������":
                        {
                            subject = "���������-��������";
                            break;
                        }
                    case "����������":
                        {
                            subject = "��������";
                            break;
                        }
                    case "���������":
                        {
                            subject = "�������";
                            break;
                        }
                    default:
                        {
                            subject = (isRepublic || isTown) ? subjects[1] : subjects[0];
                            break;
                        }
                }
            } 

            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }

        string ClearOtherCaption(string s)
        {
            return s.Replace("�������", "").Replace("�.", "").Replace("���.", "").Replace("����������", "").Replace(" ", "");
        }

        public void FillMapData()
        {

            int CodeColumn = G2.Columns.Count - 2;

            for (int j = 0; j < DundasMap.Shapes.Count; j++)
            {
                bool ShapeIsEmpty = 1 == 1;
                for (int i = 0; i < G2.Rows.Count; i++)
                {
                    try
                    {

                        Shape s = DundasMap.Shapes[j];

                        string ss = s.Name;//s["CODE"].ToString();

                        if (ClearOtherCaption(G2.Rows[i].Cells[0].Value.ToString()) == ClearOtherCaption(ss))
                        {

                            //String.Format("Cool!");
                            Shape shape = DundasMap.Shapes[j];
                            shape.TextVisibility = TextVisibility.Shown;
                            shape["Name"] = G2.Rows[i].Cells[0].Text;
                            shape["Complete"] = Double.Parse(G2.Rows[i].Cells[CodeColumn + 1].Text.Split('(')[0]);
                            shape["CompletePercent"] = Double.Parse(G2.Rows[i].Cells[CodeColumn + 1].Text.Split('(')[0]);
                            shape.ToolTip = string.Format("{0}\n{1}% \n���� �� ��:{2}", G2.Rows[i].Cells[0].Text, G2.Rows[i].Cells[CodeColumn + 1].Text.Split('(')[0], G2.Rows[i].Cells[CodeColumn + 1].Text.Split('(', ')')[1]);
                            shape.Text = string.Format("{0}\n{1}%({2})",
                                ss,
                                G2.Rows[i].Cells[CodeColumn + 1].Text.Split('(')[0], G2.Rows[i].Cells[CodeColumn + 1].Text.Split('(', ')')[1]);
                            ShapeIsEmpty = 1 == 2;
                        }
                    }
                    catch { }

                }
                if (ShapeIsEmpty)
                {
                    try
                    {
                        DundasMap.Shapes[j].Text = "";//DundasMap.Shapes[j]["CODE"].ToString();
                    }
                    catch { }
                }

            }



        }

        #endregion

        protected void G_DblClick(object sender, Infragistics.WebUI.UltraWebGrid.ClickEventArgs e)
        {

        }
        #endregion

        protected void C1_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }



    }
}
