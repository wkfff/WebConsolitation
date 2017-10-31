<%@ Page Language="C#" MasterPageFile="~/Reports.Master" ClientTarget="uplevel" AutoEventWireup="true"
    Codebehind="Default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.FO_0035_0002.Default"
    Title="Динамика исполнения кассового плана" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc7" %>

<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc6" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc5" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc4" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDateChooser.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebSchedule" TagPrefix="igsch" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../Components/CustomCalendar.ascx" TagName="CustomCalendar"
    TagPrefix="uc2" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td valign="middle" colspan="1" style="width: 100%;">
                <uc7:PopupInformer ID="PopupInformer1" runat="server" />&nbsp;<asp:Label ID="lbChartTitle" runat="server" Text="Label" CssClass="PageTitle" Width="95%"></asp:Label><br />
                <asp:Label ID="lbTitle" runat="server" CssClass="PageSubTitle" Text="Label"></asp:Label>&nbsp;
                <asp:Label ID="lbSubTitle" runat="server" CssClass="PageSubTitle" Text="Label"></asp:Label>
            </td>
            <td valign="top" align="right" rowspan="2" style="width: 100%;">
                <uc6:UltraGridExporter ID="UltraGridExporter1" runat="server" />
                <br />
                <asp:HyperLink ID="Link1" runat="server" SkinID="HyperLink"></asp:HyperLink><br />
                <asp:HyperLink ID="Link2" runat="server" SkinID="HyperLink"></asp:HyperLink><br />
                <asp:HyperLink ID="Link3" runat="server" SkinID="HyperLink"></asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td valign="top">
                            <uc4:CustomMultiCombo ID="ComboYear" runat="server" />
                        </td>
                        <td valign="top">
                            <uc4:CustomMultiCombo ID="ComboQuater" runat="server" />
                        </td>
                        <td valign="top">
                            <uc4:CustomMultiCombo ID="Parameter" runat="server" MultiSelect="false" />
                        </td>
                        <td valign="top">
                            <uc5:RefreshButton ID="RefreshButton1" runat="server" />
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
                        <td class="top">
                        </td>
                        <td class="topright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                            <igchart:UltraChart ID="ultraChart" runat="server" BackgroundImageFileName=""  
                                 EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                Version="9.1" ChartType="LineChart">
                                <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_FK0102_1#SEQNUM(100).png" />
                                <ColorModel AlphaLevel="150" ColorBegin="Green" ColorEnd="DarkSeaGreen" ModelStyle="CustomSkin">
                                </ColorModel>
                                <Effects>
                                    <Effects>
                                        <igchartprop:GradientEffect>
                                        </igchartprop:GradientEffect>
                                    </Effects>
                                </Effects>
                                <Axis>
                                    <PE ElementType="None" Fill="Cornsilk" />
                                    <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                            Visible="True" />
                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                            Visible="False" />
                                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Far">
                                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="VerticalLeftFacing"
                                                VerticalAlign="Center" FormatString="" Visible="False">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                            <Layout Behavior="Auto">
                                                <BehaviorCollection>
                                                    <igchartprop:ClipTextAxisLabelLayoutBehavior ClipText="False" Trimming="None" UseOnlyToPreventCollisions="False">
                                                    </igchartprop:ClipTextAxisLabelLayoutBehavior>
                                                </BehaviorCollection>
                                            </Layout>
                                        </Labels>
                                    </X>
                                    <Y LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="True">
                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                            Visible="True" />
                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                            Visible="False" />
                                        <Labels HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:N2&gt;" Orientation="Horizontal"
                                            VerticalAlign="Center">
                                            <SeriesLabels HorizontalAlign="Near" Orientation="Horizontal" VerticalAlign="Center"
                                                FormatString="&lt;SERIES_LABEL:N0&gt;">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </Labels>
                                    </Y>
                                    <Y2 LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="False">
                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                            Visible="True" />
                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                            Visible="False" />
                                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                                VerticalAlign="Center" FormatString="">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </Labels>
                                    </Y2>
                                    <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                            Visible="True" />
                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                            Visible="False" />
                                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                                                VerticalAlign="Center" FormatString="">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </Labels>
                                    </X2>
                                    <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                            Visible="True" />
                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                            Visible="False" />
                                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString=""
                                            Orientation="Horizontal" VerticalAlign="Center">
                                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                                VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </Labels>
                                    </Z>
                                    <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                            Visible="True" />
                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                            Visible="False" />
                                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                                VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </Labels>
                                    </Z2>
                                </Axis>
                                <Legend Location="Bottom" Visible="True"></Legend>
                                <Border Color="Transparent" />
                                <TitleLeft HorizontalAlign="Center" Text="тыс.руб." VerticalAlign="Far" Visible="True"
                                    Extent="20" Location="Left">
                                </TitleLeft>
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
                            <asp:PlaceHolder ID="gridPlaceHolder" runat="server"></asp:PlaceHolder>
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
