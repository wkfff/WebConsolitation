<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FO_0002_0002.Default" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc2" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc1" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc4" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>

<%@ Register Src="../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc6" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc7" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td style="width: 100%" valign="top">
                <uc2:PopupInformer ID="PopupInformer1" runat="server" />
                &nbsp;&nbsp;
                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label>&nbsp;&nbsp;
                <asp:Label ID="PageSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td valign="top">
               <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />&nbsp;<uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboYear" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboMonth" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc4:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            <td valign="top" align="left" style="font-family:Verdana; font-size:12px;" >
                <asp:RadioButtonList ID="RubMiltiplierButtonList" runat="server" AutoPostBack="True" RepeatDirection="horizontal"
                    Width="170px">
                    <asp:ListItem>тыс.руб.</asp:ListItem>
                    <asp:ListItem Selected="True">млн.руб.</asp:ListItem>
                </asp:RadioButtonList>
            </td>     
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top" align="left" style="padding-right: 3px">
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
            </td>
        </tr>
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
                        <td class="headerReport">
                            <asp:Label ID="chart1Label" runat="server" CssClass="ElementTitle"></asp:Label>
                        </td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td valign="top" align="right">
                            <asp:CheckBox ID="MeasureFact" runat="server" Text="‘акт" AutoPostBack="true" Checked="true"
                                Style="font-family: Verdana; font-size: 10pt;" />
                            <asp:CheckBox ID="MeasurePlan" runat="server" Text="ѕлан" AutoPostBack="true" Checked="false"
                                Style="font-family: Verdana; font-size: 10pt;" />
                        </td>
                        <td class="right">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                            <igmisc:WebAsyncRefreshPanel ID="chartWebAsyncPanel" runat="server">
                                <igchart:UltraChart ID="UltraChart" runat="server" BackgroundImageFileName="" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                    Version="9.1" OnDataBinding="UltraChart_DataBinding">
                                    <Tooltips Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False"
                                        Font-Bold="False"></Tooltips>
                                    <ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink">
                                    </ColorModel>
                                    <Effects>
                                        <Effects>
                                            <igchartprop:GradientEffect>
                                            </igchartprop:GradientEffect>
                                        </Effects>
                                    </Effects>
                                    <Axis>
                                        <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Z>
                                        <Y2 LineThickness="1" TickmarkInterval="20" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                                HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Y2>
                                        <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray"
                                                HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </X>
                                        <Y LineThickness="1" TickmarkInterval="20" Visible="True" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                                                HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Y>
                                        <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray"
                                                HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </X2>
                                        <PE ElementType="None" Fill="Cornsilk"></PE>
                                        <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near"
                                                Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Z2>
                                    </Axis>
                                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_02_02#SEQNUM(100).png" />
                                </igchart:UltraChart>
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
            </td>
        </tr>
    </table>

    <script type="text/javascript">

function uncheck(checkBoxId)
{ 
	var checkbox = document.getElementById(checkBoxId);
	checkbox.checked = !checkbox.checked;
} 
    </script>

</asp:Content>
