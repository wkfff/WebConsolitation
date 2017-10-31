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

namespace Krista.FM.Server.Dashboards.REGION_0010.UnEffectLessEducation_Novosib_
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

        class CalculorFormul
        {
            public event EventHandler NonFindVariable;
            #region ������ � �����������
            public class Variable
            {
                public string Name;
                public string Formula;
                public string Value;
                public Variable(string Name, string Formula, string Value)
                {
                    this.Name = Name;
                    this.Formula = Formula;
                    this.Value = Value;
                }
            }

            List<Variable> Env = new List<Variable>();

            public void AddVariable(string Name, string Value, string Formula)
            {
                Env.Add(new Variable(Name, Value, Formula));
            }

            public Variable GetVariableFromName(string NameVariable)
            {
                foreach (Variable var in Env)
                {
                    if (var.Name == NameVariable)
                    {
                        return var;
                    }
                }
                NonFindVariable(NameVariable, new EventArgs());
                return GetVariableFromName(NameVariable);
            }

            private string EqueVariable(Variable variable)
            {
                if (string.IsNullOrEmpty(variable.Value))
                {
                    return EqueFormul(variable.Formula);
                }
                else
                {
                    return variable.Value;
                }
            }

            public string GetResultOfVariable(string NameVariable)
            {
                return EqueVariable(GetVariableFromName(NameVariable));
            }

            #endregion

            string[] TermSumbol = new string[7] { "if", "*", "/", "+", "-", ")", "(" };

            int GetRangOfOperation(string Operation)
            {
                if ((Operation == "+") || (Operation == "-"))
                {
                    return 1;
                }
                if ((Operation == "*") || (Operation == "/"))
                {
                    return 0;
                }
                return 2;
            }

            bool IsTermSymbol(string lex)
            {
                foreach (string symbol in TermSumbol)
                {
                    if (lex == symbol)
                    {
                        return true;
                    }
                }
                return false;
            }

            bool IsBracket(string lex)
            {
                return ((lex == "(") || (lex == ")"));
            }

            bool IsMathAction(string lex)
            {
                return IsTermSymbol(lex) & (!IsBracket(lex));
            }

            private string[] ReplaceConst(string[] parsFormul)
            {
                int index = 0;
                foreach (string lex in parsFormul)
                {
                    if (!IsTermSymbol(lex))
                    {
                        parsFormul[index] = GetResultOfVariable(lex);
                    }
                    index++;
                }
                return parsFormul;
            }

            private string ClearOtherChar(string s)
            {
                if (s.Contains("="))
                {
                    s = s.Split('=')[1];
                }
                return s.Replace(" ", "");
            }

            private string EqueFormul(string f)
            {
                string[] parsFormul = ClearOtherChar(f).Split(TermSumbol, StringSplitOptions.None);

                parsFormul = ReplaceConst(parsFormul);

                return Eq(parsFormul);


            }

            string ExecBinaryOperation(string operation, string Arg1, string Arg2)
            {
                decimal arg1 = decimal.Parse(Arg1);
                decimal arg2 = decimal.Parse(Arg2);

                if (operation == "+")
                {
                    return (arg1 + arg2).ToString();
                }
                if (operation == "*")
                {
                    return (arg1 * arg2).ToString();
                } if (operation == "/")
                {
                    return (arg1 / arg2).ToString();
                } if (operation == "-")
                {
                    return (arg1 - arg2).ToString();
                }
                return "";
            }

            private void ExecStack(Stack<string> SOperation, Stack<string> Snumber)
            {
                string Farg1 = Snumber.Pop();
                string Farg2 = Snumber.Pop();
                Snumber.Push(ExecBinaryOperation(SOperation.Pop(), Farg2, Farg1));
                if (SOperation.Count > 0)
                {
                    AddOrExecStackOperation(SOperation, Snumber, SOperation.Pop());
                }
            }

            void AddOrExecStackOperation(Stack<string> SOperation, Stack<string> Snumber, string operation)
            {
                string lastOperation = SOperation.Peek();
                int RangLastOperation = GetRangOfOperation(lastOperation);
                int RangNewOperation = GetRangOfOperation(operation);

                if (SOperation.Count > 0)
                {
                    if (RangNewOperation <= RangLastOperation)
                    {
                        SOperation.Push(operation);
                    }
                    else
                    {
                        ExecStack(SOperation, Snumber);
                    }
                }
                else
                {
                    SOperation.Push(operation);
                }

            }



            private string Eq(string[] parsFormul)
            {
                Stack<string> StackNumber = new Stack<string>();
                Stack<string> StackOperation = new Stack<string>();
                Stack<string> StackBracket = new Stack<string>();
                foreach (string lex in parsFormul)
                {
                    if (IsMathAction(lex))
                    {
                        AddOrExecStackOperation(StackOperation,StackNumber,lex);                        
                    }else
                        if (IsBracket(lex))
                        {
                            StackBracket.Push(lex);
                        }
                        else
                        {
                            StackNumber.Push(lex);
                        };
                }

                return "=)";
            }





        }

        CustomParam SelectItemGrid;
        CustomParam Yc;
        CustomParam Ks;
        CustomParam P3;
        CustomParam region;
        CustomParam P4;

        string BN = "APPLEMAC-SAFARI";
        Double Coef = 1.0;

        protected override void Page_PreLoad(object sender, System.EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            CalculorFormul x = new CalculorFormul();


            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            if (BN == "FIREFOX")
            {
                Coef = 0.908;
            }
            if (BN == "IE")
            {
                Coef = 0.96;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                Coef = 0.95;
            }

            SelectItemGrid = UserParams.CustomParam("Param");
            Yc = UserParams.CustomParam("Yc");
            Ks = UserParams.CustomParam("Ks");
            P3 = UserParams.CustomParam("p3");
            P4 = UserParams.CustomParam("p4");
            region = UserParams.CustomParam("region");
            region.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 16);
            G.Height = System.Web.UI.WebControls.Unit.Empty;

            G2.Height = System.Web.UI.WebControls.Unit.Empty;
            int Coef_ = 0;
            if ((BN == "FIREFOX") || (BN == "IE"))
            {
                Coef_ = 10;
            }
            G2.Width = CRHelper.GetGridWidth((CustomReportConst.minScreenWidth - 16));
            G3.Width = CRHelper.GetGridWidth((CustomReportConst.minScreenWidth - 16));
            G3.Height = System.Web.UI.WebControls.Unit.Empty;

            C1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * (1.0 / 2.0) - 3 - 3 - Coef * 6);
            C2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * (1.0 / 2.0) - 3 - 3 - Coef * 6);

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
            Ks.Value = dt.Rows[0][1].ToString();
            Yc.Value = dt.Rows[1][1].ToString();
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                C1.Data.SwapRowsAndColumns = !(C1.Data.SwapRowsAndColumns);
            }
            Panel1.Visible = CheckBox1.Checked;
            Panel2.Visible = CheckBox1.Checked;
            GetOtherCubValue();
            G.DataBind();
            G2.DataBind();

            C1.DataBind();
            C2.DataBind();

            G3.DataBind();

            ColumnFormula();
            ColumnFormula2(G2, columnFormul2);
            ColumnFormula2(G3, columnFormul3);

            G.Rows[0].Activate();
            G.Rows[0].Activated = 1 == 1;
            G.Rows[0].Selected = 1 == 1;
            G2.Rows[0].Activate();
            G2.Rows[0].Activated = 1 == 1;
            G2.Rows[0].Selected = 1 == 1;
            G3.Rows[0].Activate();
            G3.Rows[0].Activated = 1 == 1;
            G3.Rows[0].Selected = 1 == 1;

            Hederglobal.Text = string.Format("������ ������������� �������� � ����� ������ ����������� ({0})", RegionSettingsHelper.Instance.Name);
            //UserComboBox.getLastBlock(region.Value));

            Label5.Text = "������ ��������� � ������������ � �������������� ������������� ���������� ��������� �� 15 ������ 2009 �. � 322 �� ����� �� ���������� ����� ���������� ���������� ��������� �� 28 ���� 2007 �. � 825� � �������� ������������� ������������� ���������� ��������� �� 18 ������� 2010 �. � 1052";// RegionSettingsHelper.Instance.GetPropertyValue("RegionSubTitle");            

            Page.Title = Hederglobal.Text;

            //CalculorFormul c = new CalculorFormul();
            //c.AddVariable("test", "", "test=2+2*2");
            //Hederglobal.Text = c.GetResultOfVariable("test");
        }
        string[] columnFormul2 = {
            "�1= �11 + �12", 
            "�11= (��� - ��/�� � ��) � (��� � (1+���) � 12 ���.)/1000", 
            "���", 
            "��", 
            "��", 
            "��",
            "���",
            "�12 = (�� - 0,53 � �� /�� � ��) � (��� � (1+���) � 12 ���.)/1000",
            "��",
            "��",
            "��",
            "��",
            "���" };
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

            for (int i = 1; i < G.Columns.Count; i++)
            {
                try
                {
                    G.Rows[4 * 3].Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
                    G.Rows[4 * 3].Cells[i].Style.BackgroundImage = "~/images/cornerRed.gif";
                    G.Rows[4 * 3].Cells[i].Title = "������� ������������� �������";
                }
                catch { }
                //TODO: �������� ���� ����
            }
            int lastColumn = G.Columns.Count;
            G.Columns.Add("�������");

            G.Rows[0 * 3].Cells[lastColumn].RowSpan = 1;
            G.Rows[0 * 3 + 1].Hidden = 1 == 1;
            G.Rows[0 * 3 + 2].Hidden = 1 == 1;
            G.Rows[0 * 3].Cells[lastColumn].Text = "����";


            G.Rows[1 * 3].Cells[lastColumn].RowSpan = 1;
            G.Rows[1 * 3 + 1].Hidden = 1 == 1;
            G.Rows[1 * 3 + 2].Hidden = 1 == 1;
            G.Rows[1 * 3].Cells[lastColumn].Text = "���� ���";

            G.Rows[2 * 3].Cells[1].Style.Font.Bold = 1 == 1;
            G.Rows[2 * 3].Cells[2].Style.Font.Bold = 1 == 1;
            G.Rows[2 * 3].Cells[lastColumn].RowSpan = 3;
            G.Rows[2 * 3].Cells[lastColumn].Text =
string.Format(
@"���� ��= ���� / � ��� � 100% <br>

���������� �������������� ����� ������� 
������ ������������� �������� � ����� ������ ����������� �� ����� ����� �������� ������������������ ������� � ��������� �� 100%
");
            G.Rows[3 * 3].Cells[1].Style.Font.Bold = 1 == 1;
            G.Rows[3 * 3].Cells[2].Style.Font.Bold = 1 == 1;
            G.Rows[3 * 3].Cells[lastColumn].RowSpan = 3;
            G.Rows[3 * 3].Cells[lastColumn].Text =
string.Format(
@"����= ���� / ���� ��� � 100%<br>

���������� �������������� ����� ������� 
������ ������������� �������� � ����� ������ �����������  �� ������� ������������������ ������� � ����� ������ ����������� �  ��������� �� 100 %
");
            G.Rows[4 * 3].Cells[2].Style.Font.Bold = 1 == 1;
            G.Rows[4 * 3].Cells[1].Style.Font.Bold = 1 == 1;
            G.Rows[4 * 3].Cells[lastColumn].RowSpan = 3;
            G.Rows[4 * 3].Cells[lastColumn].Text =
string.Format(
@"����= �1 + �2 <br>

���������� �������������� ����� ������������ ������ ������������� �������� �� ���������� ��������� ��������� (�1) � ������ ������������� �������� �� ���������� �������������� ������� � ������������������� ����������� (O2)");
            G.Columns[lastColumn].Width = 290;
            G.Columns[lastColumn].CellStyle.Wrap = 1 == 1;
            G.Columns[lastColumn].Header.Caption = "�������";
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
                if (i > 1)
                {
                    dtNew.Rows[i * 3][0] = "<b>" + dt.Rows[i][0] + "</b>";
                }
                else
                {
                    dtNew.Rows[i * 3][0] = dt.Rows[i][0];
                }
                dtNew.Rows[i * 3 + 1][0] = "<div style=\"FLOAT: Right;\">���������� ����������&nbsp&nbsp</div>";
                dtNew.Rows[i * 3 + 2][0] = "<div style=\"FLOAT: Right;\">���� ��������&nbsp&nbsp</div>";
                //���������� ����������

                for (int j = dt.Columns.Count - 1; j >= 1; j--)
                {
                    try
                    {
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
                        if (dtNew.Rows[i * 3 + 2][j].ToString() == "-")
                        {
                            dtNew.Rows[i * 3 + 1][j] = "-";
                        }
                    }
                    catch { }
                }
            }

            dtNew.Rows[0][0] = dtNew.Rows[0][0].ToString() + ", ���. ������";
            dtNew.Rows[3][0] = dtNew.Rows[3][0].ToString() + ", ���. ������";

            G.DataSource = dtNew;
        }

        int ld;
        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {   
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Caption = "����������";             
            
            if (G.Columns.Count > 3)
            {
               e.Layout.Bands[0].Columns[1].Hidden = 1 == 1;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth((500) * Coef);
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.00");
                e.Layout.Bands[0].Columns[i].Width = 200; 
                    //CRHelper.GetColumnWidth(
                //    ((CustomReportConst.minScreenWidth) / 
                //    (e.Layout.Bands[0].Columns.Count - 1)) * Coef);
            }
            e.Layout.AllowSortingDefault = AllowSorting.No;
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (string.IsNullOrEmpty(e.Row.Cells[i].Text))
                    {
                        e.Row.Cells[i].Text = "-";
                    }
                }

                if (e.Row.Cells[0].Text[0] == '_')
                {
                    e.Row.Cells[0].Style.Padding.Left = 15;
                    e.Row.Cells[0].Text = e.Row.Cells[0].Text.Remove(0, 1);
                }

                if ((e.Row.Index == 0) & ((UltraWebGrid)(sender) != G))
                {
                    e.Row.Cells[0].Style.Font.Bold = 1 == 1;
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
            e.CurrentWorksheet.Rows[Row].Cells[Col].CellFormat.WrapText = ExcelDefaultableBoolean.True;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {

            for (int i = 0; i < G.Columns.Count + 10; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 6000;
                for (int j = 0; j < G.Rows.Count + 10; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].Value = "";
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

            e.CurrentWorksheet.Name = "������������� �������";

            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
            SetCellFormat(e, 0, 0, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 280);
            e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 3);

            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label5.Text;
            SetCellFormat(e, 0, 1, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 11), 240);
            e.CurrentWorksheet.MergedCellsRegions.Add(1, 0, 1, 3);

            e.CurrentWorksheet.Rows[2].Cells[0].Value = Label1.Text;
            SetCellFormat(e, 0, 2, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 260);
            e.CurrentWorksheet.MergedCellsRegions.Add(2, 0, 2, 3);

            DataTable dt = GetDSForChart("G");
            //dt.Columns.Remove(dt.Columns[1]);
            int offset = Export_DT(dt, 3, e, 1 == 1);
            e.CurrentWorksheet.Rows[3].Height = 350;
            e.CurrentWorksheet.Rows[4 + 1].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[5 + 1].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[7 + 1].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[8 + 1].Hidden = 1 == 1;

            e.CurrentWorksheet.Rows[10].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[13].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[16].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;


            e.CurrentWorksheet.Columns[0].Width = 24000;
            if (CheckBox1.Checked)
            {
                e.Workbook.Worksheets.Add("������ ����������� 1");
                e.CurrentWorksheet = e.Workbook.Worksheets["������ ����������� 1"];
                e.CurrentWorksheet.Rows[0].Cells[0].Value = Label3.Text;
                SetCellFormat(e, 0, 0, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 260);
                e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 3);

                dt = GetDSForChart("G2");
                //dt.Columns.Remove(dt.Columns[1]); 
                offset = Export_DT(dt, 1, e, 1 == 2);

                e.CurrentWorksheet.Rows[1].Height = 350;
                e.CurrentWorksheet.Columns[0].Width = 24000;
                e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

                e.Workbook.Worksheets.Add("������ ����������� 2");
                e.CurrentWorksheet = e.Workbook.Worksheets["������ ����������� 2"];
                e.CurrentWorksheet.Rows[0].Cells[0].Value = "����� ������������� �������� �� ���������� �������������� ������� � ������������������� �����������";//Label3.Text;
                SetCellFormat(e, 0, 0, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 260);
                e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 3);
                dt = GetDSForChart("G3");
                //dt.Columns.Remove(dt.Columns[1]);
                offset = Export_DT(dt, 1, e, 1 == 2);
                e.CurrentWorksheet.Rows[1].Height = 350;
                e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            }
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
                    SetCellFormat(e, i, offset, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Top, new Font("Arial", 1, FontStyle.Bold), 200);
                    e.CurrentWorksheet.Columns[i].Width = 6000;
                    e.CurrentWorksheet.Rows[offset].Height = !CalcTemp ? 1500 : 700;

                    for (int j = 0; j < dt.Rows.Count * xz; j += xz)
                    {
                        e.CurrentWorksheet.Rows[j + offset + 1].Height = 1000;
                        if (dt.Rows[j / xz][i].ToString()[0] == '_')
                        {
                            dt.Rows[j / xz][i] = dt.Rows[j / xz][i].ToString().Remove(0, 1);
                            e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].CellFormat.Indent = 4;
                        }
                        else
                        {

                        }

                        e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].Value = dt.Rows[j / xz][i];
                        SetCellFormat(e, i, j + offset + 1, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);

                        try
                        {
                            e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].Value = ((System.Decimal)(dt.Rows[j / xz][i])).ToString("### ### ##0.00");
                        }
                        catch { }
                        if (i != 0)
                        {
                            if (CalcTemp)
                            {

                                try
                                {
                                    SetCellFormat(e, i, j + offset + 2, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                                    SetCellFormat(e, i, j + offset + 3, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                                    e.CurrentWorksheet.Rows[j + offset + 2].Cells[i].Value = string.Format("{0:### ### ##0.00}", (System.Decimal)(dt.Rows[j / xz][i]) - (System.Decimal)(dt.Rows[j / xz][i - 1]));
                                    e.CurrentWorksheet.Rows[j + offset + 3].Cells[i].Value = string.Format("{0:### ### ##0.00}", (((System.Decimal)(dt.Rows[j / xz][i]) / (System.Decimal)(dt.Rows[j / xz][i - 1]) - 1) * 100));

                                }
                                catch { }
                            }

                        }
                        else
                        {
                            SetCellFormat(e, i, j + offset + 1, -1, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Top, new Font("Arial", 1), 210);
                            if (CalcTemp)
                            {
                                e.CurrentWorksheet.Rows[j + offset + 2].Cells[i].Value = "���������� ����������";
                                e.CurrentWorksheet.Rows[j + offset + 3].Cells[i].Value = "���� ��������";

                                SetCellFormat(e, i, j + offset + 2, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                                SetCellFormat(e, i, j + offset + 3, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
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
            //C.DataSource = GetDSForChart("C");
        }

        protected void C1_DataBinding(object sender, EventArgs e)
        {

            C1.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(C1_InvalidDataReceived);
            //if (G.Columns.Count < 5)
            //{

            //    C1.EmptyChartText = "NON";

            //    return;
            //}

            DataTable dt = GetDSForChart("C1");
            dt.Rows[0][0] = @"���� ������������� �������� � ����� ������ ����������� �
����� ������ �������� ������������������ ������� �������� ��
 �� ����� �����������";

            dt.Rows[1][0] = @"���� ������������� �������� � ����� ������ �����������
� ����� ������ �������� ������������������ �������";
            System.Decimal max = 0;
            for (int i = 1; i < dt.Columns.Count - 1; i++)
            {
                try
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
                catch { }
            }

            C1.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            C1.Axis.Y.RangeMax = (System.Double)max * 1.2;
            C1.Axis.Y.RangeMin = 0;
            C1.DataSource = dt;
        }

        void C1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
            e.Text = "������������ ������ ��� �����������";
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.FontSizeBestFit = true;
        }

        protected void C2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("C2");
            //dt.Rows[0][0] = @"����� ����������� �������� � ����� ������ �����������";
            //dt.Rows[1][0] = @"����� ������������� �������� � ����� ������ �����������";
            dt.Rows[0][0] = @"����� ������������� �������� �� ���������� ��������� ���������";
            dt.Rows[1][0] = @"����� ������������� �������� �� ���������� �������������� �������";
            //dt.Rows[0].Delete();
            //dt.Rows[0].Delete();

            try
            {
                C2.Series.Add(CRHelper.GetNumericSeries(1, dt));
                C2.Series.Add(CRHelper.GetNumericSeries(2, dt));
                C2.Series.Add(CRHelper.GetNumericSeries(3, dt));
            }
            catch { }

        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {

        }

        protected void G2_DataBinding(object sender, EventArgs e)
        {
            G2.Rows.Clear();
            G2.Columns.Clear();
            G2.Bands.Clear();
            G2.DataSource = GetDSForChart("G2");
        }

        protected void G3_DataBinding(object sender, EventArgs e)
        {
            G3.Rows.Clear();
            G3.Columns.Clear();
            G3.Bands.Clear();
            G3.DataSource = GetDSForChart("G3");
        }

        protected void G_DblClick(object sender, ClickEventArgs e)
        {

        }

        protected void G3_DataBound(object sender, EventArgs e)
        {

        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Panel1.Visible = CheckBox1.Checked;
            Panel2.Visible = CheckBox1.Checked;
        }

        protected void C1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            Infragistics.UltraChart.Core.Primitives.Text textLabel;
            int textWidth = 550;
            int textHeight = 20;

            if (C1.EmptyChartText != "NON")
                try
                {
                    textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                    textLabel.labelStyle.Font = new Font("Microsoft Sans Serif", 11);
                    textLabel.PE.Fill = Color.Black;
                    textLabel.bounds = new Rectangle(30 + 1, 243 - 2 - 2 - 3 - 3, textWidth, textHeight);
                    textLabel.Path = ".";
                    textLabel.SetTextString("���� ������������� �������� � ����� �������� �� ����� �����������");

                    e.SceneGraph.Add(textLabel);
                    textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                    textLabel.labelStyle.Font = new Font("Microsoft Sans Serif", 11);
                    textLabel.PE.Fill = Color.Black;
                    textLabel.bounds = new Rectangle(30 + 1, 243 + 20 - 1 - 3 - 3, textWidth, textHeight);

                    textLabel.SetTextString("���� ������������� �������� � ����� ��������");
                    textLabel.Path = ".";
                    e.SceneGraph.Add(textLabel);
                }
                catch
                {

                }

        }

        protected void C2_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }


        protected void C1_ChartDrawItem(object sender, Infragistics.UltraChart.Shared.Events.ChartDrawItemEventArgs e)
        {

            if ((e.Primitive is Infragistics.UltraChart.Core.Primitives.Text))
            {

                Infragistics.UltraChart.Core.Primitives.Text t = (Infragistics.UltraChart.Core.Primitives.Text)(e.Primitive);

                if (t.GetTextString() == ".")
                {
                    t.SetTextString("");
                }


            }

        }
        int mov = 0;
        int mov_ = 0;
        protected void C2_ChartDrawItem(object sender, Infragistics.UltraChart.Shared.Events.ChartDrawItemEventArgs e)
        {

            if ((e.Primitive is Infragistics.UltraChart.Core.Primitives.Box))
            {
                Infragistics.UltraChart.Core.Primitives.Box b = (Infragistics.UltraChart.Core.Primitives.Box)(e.Primitive);
                if (b.rect.Width == b.rect.Height)
                {
                    mov++;
                    mov++;
                    mov++;
                    b.rect.Y += mov;
                    b.rect.X += 2;

                }
            }
            if ((e.Primitive is Infragistics.UltraChart.Core.Primitives.Text) && (string.IsNullOrEmpty(e.Primitive.Path)))
            {
                Infragistics.UltraChart.Core.Primitives.Text t = (Infragistics.UltraChart.Core.Primitives.Text)(e.Primitive);
                mov_++;
                mov_++;
                mov_++;
                t.bounds.Y += mov_;


            }


        }




        //#########################################################






    }
}
