<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FO_0035_0012.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0035_0012" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
        <div style="position: absolute; width: 768px; height: 900px; background-color: Transparent; top: 0px;
            left: 0px; overflow: hidden; z-index: 2">
        
        <table style="border-collapse: collapse; background-color: Black; width: 764px; height: 100%;">
            <tr>
                <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                    width: 1px; background-color: Black">
                </td>
                <td style="background-image: url(../../../images/iPadContainer/Top.gif); background-repeat: repeat-x;
                    background-color: Black; height: 3px;">
                </td>
                <td style="background: white url(../../../images/iPadContainer/righttop.gif); background-repeat: no-repeat;
                    width: 2px; background-color: Black;">
                </td>
            </tr>
            <tr>
                <td style="background-image: url(../../../images/iPadContainer/headerleft.gif); background-repeat: no-repeat;
                    width: 2px; height: 36px; background-color: Black">
                </td>
                <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                    background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                    height: 36px; text-align: center; vertical-align: middle;">
                    <asp:Label ID="Label16" runat="server" CssClass="ElementTitle" Text="Остаток средств"></asp:Label>
                </td>
                <td style="background: Black url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                    width: 1px; height: 36px; background-color: Black;">
                </td>
            </tr>
            <tr>
                <td colspan="3" valign="top" style="padding-top: 0px;">
                    <asp:Label ID="Label1" runat="server" CssClass="InformationText" Text="Остаток средств на начало квартала"></asp:Label>
                    <asp:Label ID="Label11" runat="server" CssClass="InformationText" Text="план&nbsp;"></asp:Label>
                    <asp:Label ID="lbRestStartPlan" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                    <asp:Label ID="Label13" runat="server" CssClass="InformationText" Text="факт&nbsp;"></asp:Label>
                    <asp:Label ID="lbRestStartFact" runat="server" CssClass="DigitsValueLarge"></asp:Label><asp:Label ID="Label5"
                        runat="server" CssClass="InformationText" Text="тыс.руб."></asp:Label><br />
                    <asp:Label ID="Label2" runat="server" CssClass="InformationText" Text="Остаток средств на конец квартала"></asp:Label>
                    <asp:Label ID="lbRestEndFact" runat="server" CssClass="DigitsValueLarge"></asp:Label><asp:Label ID="Label3"
                        runat="server" CssClass="InformationText" Text="&nbsp;тыс.руб."></asp:Label>
                    <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName=""  
                        EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                        Version="9.1" ChartType="LineChart" SkinID="UltraWebColumnChart">
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
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="False" />
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
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="False" />
                                <Labels HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:N2&gt;" Orientation="Horizontal" VerticalAlign="Center">
                                    <SeriesLabels HorizontalAlign="Near" Orientation="Horizontal" VerticalAlign="Center" FormatString="&lt;SERIES_LABEL:N0&gt;">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Y>
                            <Y2 LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="False">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="False" />
                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal" VerticalAlign="Center"
                                        FormatString="">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Y2>
                            <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="False" />
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
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="False" />
                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="" Orientation="Horizontal"
                                    VerticalAlign="Center">
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
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="False" />
                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="" Orientation="Horizontal"
                                    VerticalAlign="Center" Visible="False">
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal" VerticalAlign="Center">
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
                        <TitleLeft HorizontalAlign="Center" Text="тыс.руб." VerticalAlign="Far" Visible="True" Extent="20" Location="Left">
                        </TitleLeft>
                    </igchart:UltraChart>
                </td>
            </tr>
        </table></div>
    </form>
</body>
</html>
