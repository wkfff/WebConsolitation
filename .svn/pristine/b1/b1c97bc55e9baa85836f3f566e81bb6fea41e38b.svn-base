<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FO_0002_0052.Default" Title="Untitled Page" %>

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
<%@ Register Src="../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>
<%@ Register Src="../../Components/ChartBricks/PieChartBrick.ascx" TagName="PieChartBrick"
    TagPrefix="uc8" %>
<%@ Register Src="../../Components/ChartBricks/ColumnChartBrick.ascx" TagName="ColumnChartBrick"
    TagPrefix="uc9" %>
<%@ Register Src="../../Components/ChartBricks/LegendChartBrick.ascx" TagName="LegendChartBrick"
    TagPrefix="uc10" %>
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
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboYear" runat="server">
                </uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboMO" runat="server">
                </uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboAdmin" runat="server">
                </uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboFacility" runat="server">
                </uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            <td valign="top" align="left" style="font-family: Verdana; font-size: 12px;">
                <asp:RadioButtonList ID="SliceTypeButtonList" runat="server" AutoPostBack="True"
                    RepeatDirection="horizontal" Width="350px">
                    <asp:ListItem Selected="True">Ведомства</asp:ListItem>
                    <asp:ListItem>Муниципальные образования</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>
    <asp:CheckBox ID="LargestMOValueExtruding" runat="server" AutoPostBack="true" Text="Исключать г.Новосибирск" Checked="false" Style="font-family: Verdana; font-size: 10pt;" />
    <asp:CheckBox ID="LargestAdminValueExtruding" runat="server" AutoPostBack="true" Text="Исключать МФ и НП НСО" Checked="false" Style="font-family: Verdana; font-size: 10pt;" />
    <asp:Label ID="hiddenIndicatorLabel" runat="server" CssClass="PageSubTitle" Visible="false"></asp:Label>
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
                        <td class="headerleft">
                        </td>
                        <td class="headerReport" style="overflow: visible;">
                                <asp:Label ID="CommentGridCaption" runat="server" CssClass="ElementTitle"></asp:Label>
                        </td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                            <uc5:UltraGridBrick ID="CommentGridBrick" runat="server">
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
    <table>
        <tr>
            <td>
                <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
                    <tr>
                        <td class="topleft">
                        </td>
                        <td class="top" colspan="3">
                        </td>
                        <td class="topright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;" align="center">
                            <asp:Label ID="ResidualChartCaption" runat="server" CssClass="ElementTitle"></asp:Label>
                            <uc8:PieChartBrick ID="ResidualChartBrick" runat="server">
                            </uc8:PieChartBrick>
                        </td>
                        <td style="overflow: visible;" align="center">
                            <asp:Label ID="InventoriesChartCaption" runat="server" CssClass="ElementTitle"></asp:Label>
                            <uc8:PieChartBrick ID="InventoriesChartBrick" runat="server">
                            </uc8:PieChartBrick>
                        </td>
                        <td style="overflow: visible;" align="center">
                            <asp:Label ID="CreditsChartCaption" runat="server" CssClass="ElementTitle"></asp:Label>
                            <uc8:PieChartBrick ID="CreditsChartBrick" runat="server">
                            </uc8:PieChartBrick>
                        </td>
                        <td class="right">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;" colspan="3">
                            <uc10:LegendChartBrick ID="LegendChartBrick" runat="server">
                            </uc10:LegendChartBrick>
                        </td>
                        <td class="right">
                        </td>
                    </tr>
                    <tr>
                        <td class="bottomleft">
                        </td>
                        <td class="bottom" colspan="3">
                        </td>
                        <td class="bottomright">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="3" valign="top" align="right" style="width:100%">
                <asp:CheckBox ID="ResidualMeasure" runat="server" Text="Остаточная стоимость основных средств" AutoPostBack="true" Checked="true"
                    Style="font-family: Verdana; font-size: 10pt;" />
                <asp:CheckBox ID="InventoriesMeasure" runat="server" Text="Материальные запасы" AutoPostBack="true" Checked="false"
                    Style="font-family: Verdana; font-size: 10pt;" />
                <asp:CheckBox ID="CreditsMeasure" runat="server" Text="Кредиторская задолженность" AutoPostBack="true" Checked="false"
                    Style="font-family: Verdana; font-size: 10pt;" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <igmisc:WebAsyncRefreshPanel ID="chartWebAsyncPanel" runat="server">
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
                            <td class="headerleft">
                            </td>
                            <td class="headerReport" style="overflow: visible;">
                                 <asp:Label ID="DynamicChartCaption" runat="server" CssClass="ElementTitle"></asp:Label>
                            </td>
                            <td class="headerright">
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                            </td>
                            <td style="overflow: visible;">
                                <uc9:ColumnChartBrick ID="DynamicChartBrick" runat="server">
                                </uc9:ColumnChartBrick>
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
                </igmisc:WebAsyncRefreshPanel>
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        function uncheck(checkBoxId1, checkBoxId2) 
        {
            var checkbox1 = document.getElementById(checkBoxId1);
            var checkbox2 = document.getElementById(checkBoxId2);

            if (!checkbox1.checked && !checkbox2.checked) 
            {
                checkbox1.checked = true;
            }
            else 
            {
                checkbox1.checked = false;
                checkbox2.checked = false;
            }
        } 

    </script>

</asp:Content>
