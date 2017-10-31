<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="DefaultEvennessChart.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.MFRF_0004_0002.DefaultEvennessChart"
    Title="" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc5" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td style="width: 100%">
                <uc5:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="DefaultEvennessChartHelp.html" />
                &nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" Text="Label" CssClass="PageTitle"></asp:Label>
                &nbsp;&nbsp;
                <asp:Label ID="Label2" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td colspan="1" align="right" valign="top">
                <uc4:UltraGridExporter ID="UltraGridExporter1" runat="server" /><br />
                <asp:HyperLink ID="CrossLink1" runat="server" SkinID="HyperLink"></asp:HyperLink><br />
                <asp:HyperLink ID="CrossLink2" runat="server" SkinID="HyperLink"></asp:HyperLink>
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboYear" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top" align="left">
                <table width="100%">
                    <tr>
                        <td align="center" runat="server" id="tdChart1">
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
                                    <td class="header">
                                        <asp:Label ID="lbSubject1" runat="server" Text="Label" CssClass="ElementTitle"></asp:Label>
                                    </td>
                                    <td class="headerright" align="center">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                    </td>
                                    <td style="overflow: visible;">
                                        <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName=""  
                                             EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                            Version="8.2" OnDataBinding="UltraChart1_DataBinding">
                                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                Font-Underline="False" />
                                            <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear">
                                            </ColorModel>
                                            <Effects>
                                                <Effects>
                                                    <igchartprop:GradientEffect>
                                                    </igchartprop:GradientEffect>
                                                </Effects>
                                            </Effects>
                                            <Axis>
                                                <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                                            VerticalAlign="Center">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                    </Labels>
                                                </Z>
                                                <Y2 LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="False">
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="VerticalLeftFacing"
                                                            VerticalAlign="Center" FormatString="">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                    </Labels>
                                                </Y2>
                                                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                                                            VerticalAlign="Center">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                    </Labels>
                                                </X>
                                                <Y LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="True">
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                                                            VerticalAlign="Center" FormatString="">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                    </Labels>
                                                </Y>
                                                <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                                                            VerticalAlign="Center">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                    </Labels>
                                                </X2>
                                                <PE ElementType="None" Fill="Cornsilk" />
                                                <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                                            VerticalAlign="Center">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                    </Labels>
                                                </Z2>
                                            </Axis>
                                            <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_MFRF0402_03#SEQNUM(100).png" />
                                            <Border Thickness="0" />
                                        </igchart:UltraChart>
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
                        <td align="center">
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
                                    <td class="header">
                                        <asp:Label ID="lbSubject2" runat="server" Text="Label" CssClass="ElementTitle"></asp:Label>
                                    </td>
                                    <td class="headerright">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                    </td>
                                    <td style="overflow: visible;">
                                        <igchart:UltraChart ID="UltraChart2" runat="server" BackgroundImageFileName=""  
                                             EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                            Version="8.2" OnDataBinding="UltraChart1_DataBinding">
                                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                Font-Underline="False" />
                                            <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear">
                                            </ColorModel>
                                            <Effects>
                                                <Effects>
                                                    <igchartprop:GradientEffect>
                                                    </igchartprop:GradientEffect>
                                                </Effects>
                                            </Effects>
                                            <Axis>
                                                <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                                            VerticalAlign="Center">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                    </Labels>
                                                </Z>
                                                <Y2 LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="False">
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="VerticalLeftFacing"
                                                            VerticalAlign="Center" FormatString="">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                    </Labels>
                                                </Y2>
                                                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                                                            VerticalAlign="Center">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                    </Labels>
                                                </X>
                                                <Y LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="True">
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                                                            VerticalAlign="Center" FormatString="">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                    </Labels>
                                                </Y>
                                                <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                                                            VerticalAlign="Center">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                    </Labels>
                                                </X2>
                                                <PE ElementType="None" Fill="Cornsilk" />
                                                <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                                            VerticalAlign="Center">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                    </Labels>
                                                </Z2>
                                            </Axis>
                                            <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_MFRF0402_04#SEQNUM(100).png" />
                                            <Border Thickness="0" />
                                        </igchart:UltraChart>
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
            </td>
        </tr>
    </table>
</asp:Content>
