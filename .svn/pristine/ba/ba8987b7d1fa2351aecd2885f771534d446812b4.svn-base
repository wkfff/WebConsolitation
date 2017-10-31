<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0004_0003.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0004_0003_1" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Src="../../../iPadBricks/FO_0004_0002_Chart.ascx" TagName="FO_0004_0002_Chart"
    TagPrefix="uc1" %>
<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <div style="position: absolute; width: 768px; background-color: black; top: 0px;
        left: 0px; z-index: 2; overflow: visible;">
        <asp:Label ID="lbDescription" SkinID="InformationText" runat="server" Text="Label"></asp:Label>
        <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black;
            top: 0px; left: 0px">
            <tr>
                <td style="width: 380px; background-color: black;" valign="top">
                    <uc2:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Консолидированный бюджет"
                        Width="379px" />
                    <div style="margin-left: 20px;">
                        <table class="InformationText" style="border-collapse: collapse">
                            <tr>
                                <td>
                                    Уточненный план
                                </td>
                                <td style="width: 120px" align="right">
                                    <asp:Label ID="Label1" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                </td>
                                <td>
                                    &nbsp;млн.руб.
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Факт
                                </td>
                                <td style="width: 120px" align="right">
                                    <asp:Label ID="Label2" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                </td>
                                <td>
                                    &nbsp;млн.руб.
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Исполнено
                                </td>
                                <td style="width: 120px" align="right">
                                    <asp:Label ID="Label3" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="Label7" SkinID="DigitsValueSmall" runat="server" Text="%"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <asp:PlaceHolder ID="GaugeIncomesPlaceHolder" runat="server"></asp:PlaceHolder>
                    </div>
                </td>
                <td valign="top" style="padding-left: 2px">
                    <uc2:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Областной бюджет"
                        Width="382px" />
                    <div style="margin-left: 20px;">
                        <table class="InformationText" style="border-collapse: collapse">
                            <tr>
                                <td>
                                    Уточненный план
                                </td>
                                <td style="width: 120px" align="right">
                                    <asp:Label ID="Label4" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                </td>
                                <td>
                                    &nbsp;млн.руб.
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Факт
                                </td>
                                <td style="width: 120px" align="right">
                                    <asp:Label ID="Label5" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                </td>
                                <td>
                                    &nbsp;млн.руб.
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Исполнено
                                </td>
                                <td style="width: 120px" align="right">
                                    <asp:Label ID="Label6" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lbIncomesExecutedMeasures" SkinID="DigitsValueSmall" runat="server"
                                        Text="%"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <asp:PlaceHolder ID="GaugeOutcomesPlaceHolder" runat="server"></asp:PlaceHolder>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <igchart:UltraChart ID="UltraChart12" runat="server" BackgroundImageFileName=""  
                         EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                        Version="9.1" SkinID="UltraWebColumnChart">
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
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_12#SEQNUM(100).png" />
                    </igchart:UltraChart>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
