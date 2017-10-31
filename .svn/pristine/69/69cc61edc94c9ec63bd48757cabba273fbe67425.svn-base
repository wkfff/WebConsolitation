<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.STAT_0023_0006.Default" Title="Untitled Page" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc6" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc7" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
    <%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Src="../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>
<%@ Register Src="../../Components/ChartBricks/PieChartBrick.ascx" TagName="PieChartBrick"
    TagPrefix="uc9" %>
<%@ Register Src="../../Components/ChartBricks/LegendChartBrick.ascx" TagName="LegendChartBrick"
    TagPrefix="uc10" %>
<%@ Register Src="../../Components/ChartBricks/StackColumnChartBrick.ascx" TagName="StackColumnChartBrick"
    TagPrefix="uc11" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td valign="top" style="width: 100%; vertical-align: top;">
                <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />
                &nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" CssClass="PageTitle"></asp:Label><br />
                <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" rowspan="2" style="width: 100%;">
                <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />&nbsp;<uc7:ReportPDFExporter
                    ID="ReportPDFExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td>
                <table>
                    <tr>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboYear" runat="server">
                            </uc3:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc1:RefreshButton ID="RefreshButton1" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        
    </table>
    <asp:Label ID="GridCaption" runat="server" CssClass="ElementTitle"></asp:Label>
    <table style="vertical-align: top; margin-top: -3px">
        <tr>
            <td valign="top" align="left">
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
                            <uc5:UltraGridBrick ID="GridBrick" runat="server">
                            </uc5:UltraGridBrick>
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
            </td>
        </tr>
    </table>
 
  

   <script type="text/javascript">
       window.onload = function () {
           getSize();
           init();
       };

       function init() {
           var objParent = document.getElementById("ctl00xContentPlaceHolder1xGridBrickxGridControl_r_0").parentNode;
           for (var j = 0; j < objParent.childNodes.length; j++) {
               var row = objParent.childNodes[j];
               var tdLevelText = GetInnerText(row.childNodes[row.childNodes.length - 1]);
               if (tdLevelText == "1") {
                   row.style.display = "none";
               }
           }
       }

       function resize(objCell) {
           var objRow = objCell.parentNode.parentNode.parentNode.parentNode.parentNode;
           var tdBaseTypeText = GetInnerText(objRow.childNodes[objRow.childNodes.length - 3]);

           var objParent = objCell.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
           for (var j = 0; j < objParent.childNodes.length; j++) {
               var row = objParent.childNodes[j];
               var tdLevelText = GetInnerText(row.childNodes[row.childNodes.length - 1]);
               var tdTypeText = GetInnerText(row.childNodes[row.childNodes.length - 3]);
               if ((tdTypeText == tdBaseTypeText) && (tdLevelText == "1")) {
                   if (row.style.display == "none") {
                       row.style.display = "";
                   }
                   else {
                       row.style.display = "none";
                   }
               }
           }

           if (objCell.className == "ExpandBlockFirstState") {
               objCell.className = "ExpandBlockSecondState";
           }
           else {
               objCell.className = "ExpandBlockFirstState";
           }
       }

       function GetInnerText(element) {
           if (typeof (element.textContent) != "undefined") {
               return element.textContent;
           }
           return element.innerText;
       }
    </script>
</asp:Content>
