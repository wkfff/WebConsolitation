<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.Oil_0001_0001.Default" %>

<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc6" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc7" %>
    <%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc5" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
    <%@ Register Src="../../Components/CustomCalendar.ascx" TagName="CustomCalendar"
    TagPrefix="uc3" %>

<%@ Register Src="../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>

<%@ Register Src="OIL_0001_0001_Chart.ascx" TagName="OIL_0001_0001_Chart" TagPrefix="uc10" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td colspan="1" style="width: 100%;">                
                <asp:Label ID="Label1" runat="server" Text="Label" CssClass="PageTitle"></asp:Label> <br/>
                <asp:Label ID="Label2" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right">
                <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
            </td>
            <td>
                &nbsp;<uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
               <uc3:CustomCalendar ID="CustomCalendar1" runat="server" Width="300px" ></uc3:CustomCalendar>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboFO" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboOil" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
    </table>
 <uc10:OIL_0001_0001_Chart ID="UltraChart1" runat="server" />
 <uc10:OIL_0001_0001_Chart ID="UltraChart2" runat="server" />
 <uc10:OIL_0001_0001_Chart ID="UltraChart3" runat="server" />
                <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
                    <tr>
                        <td class="topleft">
                        </td>
                        <td class="top">
                        </td>
                        <td class="topright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                           <uc5:UltraGridBrick ID="GridBrick" runat="server"></uc5:UltraGridBrick>
                        </td>
                        <td class="right">
                        </td>
                    </tr>
                    <tr>
                        <td class="bottomleft">
                        </td>
                        <td class="bottom">
                        </td>
                        <td class="bottomright">
                        </td>
                    </tr>
               </table> 
</asp:Content>
