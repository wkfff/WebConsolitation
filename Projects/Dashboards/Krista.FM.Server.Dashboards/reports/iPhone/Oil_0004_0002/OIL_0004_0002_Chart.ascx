<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OIL_0004_0002_Chart.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.Dashboard.OIL_0004_0002_Chart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
    <%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader" TagPrefix="uc1" %>
    <uc1:iPadElementHeader ID="IncomesHeader" runat="server" Text="Розничные цены на нефтепродукты"
                            Width="100%" />
<asp:Label ID="Label1" runat="server" SkinID="InformationText"></asp:Label>
<igchart:UltraChart ID="UltraChart1" runat="server" SkinID="UltraWebColumnChart">
    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_FK0101Gadget_#SEQNUM(100).png" />
</igchart:UltraChart>
