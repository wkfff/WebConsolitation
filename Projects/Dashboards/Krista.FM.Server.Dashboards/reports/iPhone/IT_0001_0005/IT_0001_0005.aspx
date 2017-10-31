<%@ Page Language="C#" AutoEventWireup="true" Codebehind="IT_0001_0005.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.IT_0001_0005" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: Black">
    <form id="form1" runat="server">
     <touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
        <div style="position: absolute; width: 768px; height: 950px; top: 0px; left: 0px; overflow: hidden; z-index: 2;">
            <table style="position: absolute; width: 760px; height: 900px; background-color: Black; top: 0px; left: 0px;
                overflow: hidden">
                <tr>
                    <td>
                        <table style="border-collapse: collapse;">
                            <tr>
                                <td align="left" valign="top">
                                    <table style="margin-left: -4px; margin-top: -4px">
                                        <tr>
                                            <td align="left" valign="top">
                                                <table style="border-collapse: collapse; background-color: Black; width: 100%;">
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
                                                            <asp:Label ID="Label1" runat="server" CssClass="ElementTitle" Text="Структура потребления ИТ"></asp:Label>
                                                        </td>
                                                        <td class="headerright">
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
                                                <asp:Label ID="lbMarketConsumeDescription" runat="server" SkinID="InformationTextLarge" Text=""></asp:Label>
                                                <igchart:UltraChart ID="UltraChartConsume" runat="server" BackgroundImageFileName=""  
                                                     EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                                    Version="9.1" SkinID="UltraWebColumnChart">
                                                    <Tooltips Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False" Font-Bold="False">
                                                    </Tooltips>
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
                                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt"
                                                                VerticalAlign="Center">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                                    VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                            </Labels>
                                                        </Z>
                                                        <Y2 LineThickness="1" TickmarkInterval="40" Visible="False" TickmarkStyle="Smart">
                                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray" HorizontalAlign="Near"
                                                                Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                                <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                                    VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                            </Labels>
                                                        </Y2>
                                                        <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                                            <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray" HorizontalAlign="Near"
                                                                Font="Verdana, 7pt" VerticalAlign="Center">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                                    VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                            </Labels>
                                                        </X>
                                                        <Y LineThickness="1" TickmarkInterval="40" Visible="True" TickmarkStyle="Smart">
                                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray" HorizontalAlign="Far"
                                                                Font="Verdana, 7pt" VerticalAlign="Center">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                                <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                                    VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                            </Labels>
                                                        </Y>
                                                        <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                                            <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray" HorizontalAlign="Far"
                                                                Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                                    VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                            </Labels>
                                                        </X2>
                                                        <PE ElementType="None" Fill="Cornsilk"></PE>
                                                        <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Visible="False"
                                                                Font="Verdana, 7pt" VerticalAlign="Center">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                                    VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                            </Labels>
                                                        </Z2>
                                                    </Axis>
                                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_11#SEQNUM(100).png" />
                                                    <ClientSideEvents ClientOnMouseOver="UltraChartMarketSegments_ClientOnMouseOver" ClientOnMouseClick="UltraChartMarketSegments_ClientOnMouseOver" />
                                                </igchart:UltraChart>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" valign="top" class="InformationTextLarge" style="text-align: right; padding-right: 5px">
                                                Источник:&nbsp;<asp:HyperLink ID="HyperLink4" runat="server"></asp:HyperLink></td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="left" valign="top">
                                    <table style="margin-right: -4px; margin-top: -4px">
                                        <tr>
                                            <td align="left" valign="top">
                                                <table style="border-collapse: collapse; background-color: Black; width: 100%;">
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
                                                            <asp:Label ID="Label2" runat="server" CssClass="ElementTitle" Text="Тop-5 потребителей в корпоративном секторе"></asp:Label>
                                                        </td>
                                                        <td class="headerright">
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
                                                <uc1:TagCloud ID="TagCloud1" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" valign="top" class="InformationTextLarge" style="text-align: right; padding-right: 5px; padding-top: 10px">
                                                Источник:&nbsp;<asp:HyperLink ID="HyperLink1" runat="server"></asp:HyperLink></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table style="border-collapse: collapse; background-color: Black; width: 100%; margin-top: 20px">
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
                                                            <asp:Label ID="Label5" runat="server" CssClass="ElementTitle" Text="Структура затрат"></asp:Label>
                                                        </td>
                                                        <td class="headerright">
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
                                                <asp:Label ID="Label4" runat="server" SkinID="InformationTextLarge" Text=""></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td align="right" valign="top" class="InformationTextLarge" style="text-align: right; padding-right: 5px; padding-top: 5px">
                                                Источник:&nbsp;<asp:HyperLink ID="HyperLink2" runat="server"></asp:HyperLink></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top">
                        <table style="border-collapse: collapse; background-color: Black; width: 760px;">
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
                                    <asp:Label ID="Label3" runat="server" CssClass="ElementTitle" Text="Региональная структура затрат на ИТ "></asp:Label>
                                </td>
                                <td class="headerright">
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
                    <td align="left" valign="top">
                        <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName=""  
                            EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" SkinID="UltraWebColumnChart">
                            <Tooltips Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False" Font-Bold="False">
                            </Tooltips>
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
                                    <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                    <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                    <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt"
                                        VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                            VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                    </Labels>
                                </Z>
                                <Y2 LineThickness="1" TickmarkInterval="20" Visible="False" TickmarkStyle="Smart">
                                    <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                    <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                    <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray" HorizontalAlign="Near"
                                        Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                            VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                    </Labels>
                                </Y2>
                                <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                    <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                    <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                    <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray" HorizontalAlign="Near"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                            VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                    </Labels>
                                </X>
                                <Y LineThickness="1" TickmarkInterval="20" Visible="True" TickmarkStyle="Smart">
                                    <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                    <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                    <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray" HorizontalAlign="Far"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                            VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                    </Labels>
                                </Y>
                                <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                    <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                    <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                    <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray" HorizontalAlign="Far"
                                        Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                            VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                    </Labels>
                                </X2>
                                <PE ElementType="None" Fill="Cornsilk"></PE>
                                <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                    <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                    <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                    <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Visible="False"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                            VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                    </Labels>
                                </Z2>
                            </Axis>
                            <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_01#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </td>
                </tr>
                <tr>
                    <td align="right" valign="top" class="InformationTextLarge" style="text-align: right; padding-right: 5px; padding-top: 10px">
                        Источник:&nbsp;<asp:HyperLink ID="HyperLink3" runat="server"></asp:HyperLink></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
