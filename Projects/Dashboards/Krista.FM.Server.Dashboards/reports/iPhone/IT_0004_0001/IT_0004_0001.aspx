<%@ Page Language="C#" AutoEventWireup="true" Codebehind="IT_0004_0001.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.IT_0004_0001" %>

<%@ Register Src="../../../iPadBricks/IT_0004_0001_Map.ascx" TagName="IT_0004_0001_Map" TagPrefix="uc9" %>
<%@ Register Src="../../../iPadBricks/IT_0004_0001_AdminOutcomesGrid.ascx" TagName="IT_0004_0001_AdminOutcomesGrid"
    TagPrefix="uc8" %>
<%@ Register Src="../../../iPadBricks/IT_0002_0001_BanksChart.ascx" TagName="IT_0002_0001_BanksChart"
    TagPrefix="uc6" %>
<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader" TagPrefix="uc7" %>
<%@ Register Src="../../../iPadBricks/UltraChartIT_0002_0001_Chart.ascx" TagName="UltraChartIT_0002_0001_Chart"
    TagPrefix="uc5" %>
<%@ Register Src="../../../iPadBricks/IT_0002_0001_DayRestsGrid.ascx" TagName="IT_0002_0001_DayRestsGrid"
    TagPrefix="uc4" %>
<%@ Register Src="../../../iPadBricks/IT_0003_0001_AssesmentGrid.ascx" TagName="IT_0003_0001_AssesmentGrid"
    TagPrefix="uc3" %>
<%@ Register Src="../../../iPadBricks/IT_0002_0001_RestsGrid.ascx" TagName="IT_0002_0001_RestsGrid" TagPrefix="uc2" %>
<%@ Register Src="../../../iPadBricks/IT_0002_0001_IncomesGrid.ascx" TagName="IT_0002_0001_IncomesGrid"
    TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body class="iphoneBody">
    <form id="form1" runat="server">
        <div style="position: absolute; width: 768px; height: 950px; top: 0px; left: 0px; z-index: 2; overflow: hidden">
            <table>
                <tr>
                    <td align="left">
                        <table style="border-collapse: collapse">
                            <tr>
                                <td align="left">
                                    <uc1:IT_0002_0001_IncomesGrid ID="IT_0002_0001_IncomesGrid1" runat="server" Width="377px" Text="������ �� 1 ��������� 2010 ����"
                                        QueryName="IT_0004_0001_incomes" ImageUrl="../Images/Incomes.png" DescriptionText="������ ������������������ ������� ����������&nbsp;<span style='color: White'><b>{0:N0}</b></span>&nbsp;���.���.">
                                    </uc1:IT_0002_0001_IncomesGrid>
                                </td>
                                <td align="left">
                                    <uc1:IT_0002_0001_IncomesGrid ID="IT_0002_0001_IncomesGrid2" runat="server" Width="377px" Text="������� �� 1 ��������� 2010 ����"
                                        QueryName="IT_0004_0001_outcomes" ImageUrl="../Images/Outcomes.png" DescriptionText="������� ������������������ ������� ����������&nbsp;<span style='color: White'><b>{0:N0}</b></span>&nbsp;���.���.">
                                    </uc1:IT_0002_0001_IncomesGrid>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table>
                <tr>
                    <td>
                        <uc8:IT_0004_0001_AdminOutcomesGrid ID="IT_0004_0001_AdminOutcomesGrid1" runat="server" QueryName="IT_0004_0001_AdminOutcomesGrid"
                            Text="������� �� ���������������-�������������� ��������" Width="760px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="margin-top: -10px">
                            <uc9:IT_0004_0001_Map ID="IT_0004_0001_Map1" runat="server" QueryName="IT_0004_0001_AdminOutcomesMap"></uc9:IT_0004_0001_Map>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>