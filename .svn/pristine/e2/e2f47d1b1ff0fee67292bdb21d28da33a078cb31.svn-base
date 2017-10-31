<%@ Page Language="C#" AutoEventWireup="true" Codebehind="STAT_0001_0003.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.STAT_0001_0003" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc2" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
        <div style="position: absolute; width: 768px; height: 950px; background-color: black;
            top: 0px; left: 0px; z-index: 2; overflow: hidden;">
            <div style="position: absolute; width: 315px; height: 120px; background-color: transparent;
                top: 70px; left: 447px; z-index: 20; overflow: hidden; background-image: url(../../../images/peoples.png);
                background-repeat: repeat-x">
            </div>
            <div style="position: absolute; width: 315px; height: 120px; background-color: transparent;
                top: 425px; left: 447px; z-index: 20; overflow: hidden; background-image: url(../../../images/peoples.png);
                background-repeat: repeat-x">
            </div>
            <table style="margin-top: -5px">
                <tr>
                    <td>
                        <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Зарегистрированная безработица"
                            Width="100%" />
                        <table>
                            <tr>
                                <td>
                                    <table style="margin-top: -12px">
                                        <tr>
                                            <td style="width: 440px" width="440px">
                                                <asp:Label ID="CommentText1" runat="server" SkinID="InformationText"></asp:Label>
                                            </td>
                                            <td>
                                                <div style="margin-right: -15px; margin-left: -15px">
                                                    <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName=""  
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
                                                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_01#SEQNUM(100).png" />
                                                    </igchart:UltraChart>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="margin-top: -11px">
                            <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Безработица по критериям МОТ"
                                Width="100%" />
                        </div>
                        <table style="margin-top: -20px">
                            <tr>
                                <td style="width: 440px" width="440px">
                                    <asp:Label ID="lbDescription" runat="server" SkinID="InformationText"></asp:Label>
                                </td>
                                <td>
                                    <div style="margin-right: -12px; margin-left: -20px">
                                        <igchart:UltraChart ID="UltraChart3" runat="server" BackgroundImageFileName=""  
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
                                            <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_01#SEQNUM(100).png" />
                                        </igchart:UltraChart>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="margin-top: -18px;">
                            <uc1:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Задолженность по выплате заработной платы"
                                Width="100%" />
                        </div>
                        <table>
                            <tr>
                                <td>
                                    <div style="margin-top: -5px">
                                        <asp:Label ID="lbDebtDescription" runat="server" SkinID="InformationText"></asp:Label></div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="margin-top: -8px;">
                            <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Уровень безработицы по муниципальным образованиям"
                                Width="100%" />
                            <uc2:TagCloud ID="CloudTag1" runat="server" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
