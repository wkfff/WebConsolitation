<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FK_0004_0005.Default" Title="Untitled Page" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc6" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc7" %>
<%@ Register Src="../../Components/ChartBricks/PeriodSplineChartBrick.ascx" TagName="PeriodSplineChartBrick"
    TagPrefix="uc10" %>
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
<%@ Register Src="../../Components/CustomCalendar.ascx" TagName="CustomCalendar"
    TagPrefix="uc9" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td valign="top" style="width: 100%; vertical-align: top;">
                <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html"
                    Visible="false" />
                &nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" CssClass="PageTitle"></asp:Label>&nbsp;
                <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" style="width: 100%;">
                <nobr><uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                &nbsp;<uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" /></nobr>
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc9:CustomCalendar ID="CustomCalendar1" runat="server"></uc9:CustomCalendar>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboIncomes" runat="server" />
            </td>
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            <td valign="top" align="left" style="font-family: Verdana; font-size: 12px;">
                
                    <asp:RadioButtonList ID="RubMiltiplierButtonList" runat="server" AutoPostBack="True"
                        RepeatDirection="horizontal" Width="170px">
                        <asp:ListItem>млн.руб.</asp:ListItem>
                        <asp:ListItem Selected="True">млрд.руб.</asp:ListItem>
                    </asp:RadioButtonList>
                    </td>
                 <td valign="top" align="left" style="font-family: Verdana; font-size: 12px; padding-left: 20px">
                    <asp:RadioButtonList ID="DailyMonthlyButtonList" runat="server" AutoPostBack="True"
                        RepeatDirection="horizontal" Width="200px">
                        <asp:ListItem>по&nbsp;дням</asp:ListItem>
                        <asp:ListItem Selected="True">по&nbsp;месяцам</asp:ListItem>
                    </asp:RadioButtonList>
                
            </td>
        </tr>
    </table>
    <table style="width: 100%">
        <tr>
            <td>
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
                            <asp:Label ID="Label3" runat="server" CssClass="ElementTitle"></asp:Label>
                        </td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td valign="top" align="right" style="padding-top: 4px">
                            <asp:CheckBox ID="CheckBox1" runat="server" Text="Подписи значений" AutoPostBack="true"
                                Checked="true" Style="font-family: Verdana; font-size: 10pt;" />
                        </td>
                        <td class="right">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                            <igmisc:WebAsyncRefreshPanel ID="chartWebAsyncPanel" runat="server">
                                <uc10:PeriodSplineChartBrick ID="DynamicChartBrick" runat="server"></uc10:PeriodSplineChartBrick>
                            </igmisc:WebAsyncRefreshPanel>
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
                        <td valign="top" align="right" style="padding-top: 4px">
                            <asp:CheckBox ID="normalize" runat="server" Text="Нормировать до 100%" AutoPostBack="true"
                                Checked="false" Style="font-family: Verdana; font-size: 10pt;" Visible="false"/>
                        </td>
                        <td class="right">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                            <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server">
                                <igchart:UltraChart ID="ChartControl" runat="server" SkinID="UltraChart">
                                    <Effects>
                                        <Effects>
                                            <igchartprop:GradientEffect>
                                            </igchartprop:GradientEffect>
                                        </Effects>
                                    </Effects>
                                    <Axis>
                                        <Y2 LineThickness="1" TickmarkInterval="20" Visible="False" TickmarkStyle="Smart">
                                        </Y2>
                                        <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                        </X>
                                        <Y LineThickness="1" TickmarkInterval="20" Visible="True" TickmarkStyle="Smart">
                                        </Y>
                                    </Axis>
                                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_Fo0243#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </igmisc:WebAsyncRefreshPanel>
                         <span class="PageSubTitle"><b>Примечание:</b> объем квазифискальных операций включает в себя:  средства ФНБ, размещенные на депозиты в ВЭБ (до 2019 г.) и средства Резервного фонда, перечисленные на пополнение резервной позиции  РФ в МВФ</span>
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
</asp:Content>
