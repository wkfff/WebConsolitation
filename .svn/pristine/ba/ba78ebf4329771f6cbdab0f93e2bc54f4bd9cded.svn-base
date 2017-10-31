<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SGM_0006.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.SGM_0006" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table id="TABLE1" style="border-right: 0px solid; border-top: 0px solid; left: 0px;
                border-left: 0px solid; width: 320px; border-bottom: 0px solid; position: absolute;
                top: 0px; border-collapse: collapse; height: 360px; background-color: black;">
                <tr>
                    <td valign="top">
                        <table id="table1Line" style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px;
                            margin: 0px; border-top-style: none; padding-top: 0px; border-right-style: none;
                            border-left-style: none; border-bottom-style: none; border-collapse: collapse;">
                            <tr>
                                <td style="width: 310px;">
                                    <asp:Label ID="LabelSubject" runat="server" Text="Label" EnableTheming="True" SkinID="InformationText"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <table id="table2Line" style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px;
                            margin: -3px 0px 0px; border-top-style: none; padding-top: 0px; border-right-style: none;
                            border-left-style: none; border-bottom-style: none; border-collapse: collapse;">
                            <tr>
                                <td style="width: 310px;">
                                    <asp:Label ID="LabelCurValCaption" runat="server" SkinID="InformationText" Text="Label"
                                        BackColor="Transparent"></asp:Label>
                                    &nbsp;<asp:Label ID="LabelCurVal" runat="server" SkinID="DigitsValue" Text="Label"
                                        BackColor="Transparent"></asp:Label>
                                    &nbsp;<asp:Label ID="LabelMeasure" runat="server" SkinID="InformationText" Text="Label"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <table id="table3Line" style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px;
                            margin: -6px 0px 0px; border-top-style: none; padding-top: 0px; border-right-style: none;
                            border-left-style: none; border-bottom-style: none; border-collapse: collapse;">
                            <tr>
                                <td style="padding-left: 30px; width: 310px;">
                                    <asp:Label ID="LabelRankFOCaption" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
                                    &nbsp;<asp:Label ID="LabelRankFO" runat="server" SkinID="DigitsValue" Text="Small"></asp:Label>
                                    &nbsp;<asp:Image ID="ImageRankFO" runat="server" BackColor="Black" Height="17px"
                                        ImageUrl="~/images/arrowRedUpBB.png" Width="17px" ImageAlign="Bottom" />
                                    &nbsp;<asp:Label ID="LabelRankRFCaption" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
                                    &nbsp;<asp:Label ID="LabelRankRF" runat="server" SkinID="DigitsValue" Text="Small"></asp:Label>
                                    &nbsp;<asp:Image ID="ImageRankRF" runat="server" BackColor="Black" Height="17px"
                                        ImageUrl="~/images/arrowRedUpBB.png" Width="17px" ImageAlign="Bottom" />
                                </td>
                            </tr>
                        </table>
                        <table id="table4Line" style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px;
                            margin: -3px 0px 0px; border-top-style: none; padding-top: 0px; border-right-style: none;
                            border-left-style: none; border-bottom-style: none; border-collapse: collapse;">
                            <tr>
                                <td style="width: 310px;">
                                    <asp:Label ID="LabelPrevValCaption" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
                                    &nbsp;<asp:Label ID="LabelPrevVal" runat="server" SkinID="DigitsValue" Text="Small"></asp:Label>&nbsp;
                                    &nbsp;<asp:Image ID="ImageDifference" runat="server" BackColor="Black" Height="15px"
                                        ImageUrl="~/images/arrowRedUpBB.png" Width="12px" ImageAlign="Bottom" />
                                    &nbsp;<asp:Label ID="LabelPercent" runat="server" SkinID="DigitsValue" Text="Label"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <table id="table5Line" style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px;
                            margin: 3px 0px 0px; border-top-style: none; padding-top: 0px; border-right-style: none;
                            border-left-style: none; border-bottom-style: none; border-collapse: collapse;">
                            <tr>
                                <td align="left" valign="middle">
                                    <asp:Label ID="LabelActualCaption" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <table id="table6Line" style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px;
                            margin: -7px 0px 0px; border-top-style: none; padding-top: 0px; border-right-style: none;
                            border-left-style: none; border-bottom-style: none; border-collapse: collapse;">
                            <tr>
                                <td align="left" valign="middle">
                                    <asp:Label ID="LabelActualCaptionDetail" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <table id="table7Line" style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px;
                            margin: -2px 0px 0px; border-top-style: none; padding-top: 0px; border-right-style: none;
                            border-left-style: none; border-bottom-style: none; border-collapse: collapse;">
                            <tr>
                                <td align="left" valign="top">
                                    <table id="table71Line" style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                        padding-top: 0px; border-collapse: collapse;">
                                        <tr>
                                            <td align="center" rowspan="2" style="border-right: #333333 1px solid; padding-right: 0px;
                                                border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                border-left: #333333 1px solid; width: 190px; padding-top: 0px; border-bottom: #333333 1px solid;
                                                border-collapse: collapse">
                                                <asp:Label ID="LabelTableDeseaseName" runat="server" EnableTheming="True" SkinID="InformationText"
                                                    Text="Label"></asp:Label>
                                            </td>
                                            <td align="center" style="width: 52px; border-right: #333333 1px solid; padding-right: 0px;
                                                border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                border-collapse: collapse;">
                                                <asp:Label ID="LabelTableRF" runat="server" EnableTheming="True" SkinID="InformationText"
                                                    Text="Label"></asp:Label></td>
                                            <td align="center" style="width: 68px; border-right: #333333 1px solid; padding-right: 0px;
                                                border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                border-collapse: collapse;">
                                                <asp:Label ID="LabelTableSubject" runat="server" EnableTheming="True" SkinID="InformationText"
                                                    Text="Label"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="table72Line" style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px 0px 0px 1px;
                                        padding-top: 0px; border-collapse: collapse;">
                                        <tr style="height: 10px;">
                                            <td align="center" style="border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 0px solid;
                                                padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: #333333 0px solid;
                                                width: 190px; padding-top: 0px; border-bottom: #333333 0px solid; border-collapse: collapse; height: 10px;">
                                            </td>
                                            <td align="center" style="width: 52px; border-right: #333333 1px solid; padding-right: 0px;
                                                border-top: #333333 0px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 0px solid;
                                                border-collapse: collapse; height: 10px;">
                                            </td>
                                            <td align="center" style="width: 68px; border-right: #333333 1px solid; padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 0px solid;
                                                border-collapse: collapse; border-top-width: 0px; border-top-color: #333333; height: 10px;">
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="table73Line" style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: -20px 0px 0px;
                                        padding-top: 0px; border-collapse: collapse;">
                                        <tr>
                                            <td align="left" style="width: 188px; border-collapse: collapse;">
                                                <igchart:UltraChart ID="chart" runat="server" BackgroundImageFileName=""  
                                                     ChartType="BarChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                                    Height="250px" SkinID="UltraWebColumnChart" Version="9.1" Width="180px" OnFillSceneGraph="chart_FillSceneGraph"
                                                    CrossHairColor="Crimson">
                                                    <TitleTop Visible="False">
                                                    </TitleTop>
                                                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                        Font-Underline="False" />
                                                    <TitleBottom Visible="False" Extent="33" Location="Bottom">
                                                    </TitleBottom>
                                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_sgm0006_01_#SEQNUM(100).png" />
                                                    <ColorModel AlphaLevel="150" ColorBegin="SkyBlue" ColorEnd="Teal" ModelStyle="LinearRandom">
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
                                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString=""
                                                                Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                                                    VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                            </Labels>
                                                        </Z>
                                                        <Y2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                                Visible="False" />
                                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                                Visible="True" />
                                                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                                Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="VerticalLeftFacing"
                                                                    VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                            </Labels>
                                                        </Y2>
                                                        <X LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="False">
                                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                                Visible="False" />
                                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                                Visible="False" />
                                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                                Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                            </Labels>
                                                        </X>
                                                        <Y LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                                Visible="False" />
                                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                                Visible="False" />
                                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                                Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="VerticalLeftFacing"
                                                                    VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                            </Labels>
                                                        </Y>
                                                        <X2 LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="False">
                                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                                Visible="False" />
                                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                                Visible="True" />
                                                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                                Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
                                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                                                                Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="VerticalLeftFacing"
                                                                    VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                            </Labels>
                                                        </Z2>
                                                    </Axis>
                                                </igchart:UltraChart>
                                            </td>
                                            <td align="left" colspan="2" valign="top" style="width: 123px; border-collapse: collapse;">
                                                <table style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 15px 0px;
                                                    width: 100%; border-top-style: none; padding-top: 0px; border-right-style: none;
                                                    border-left-style: none; border-bottom-style: none; border-collapse: collapse;">
                                                    <tr style="height: 24px;">
                                                        <td align="right" style="width: 53px; border-right: #333333 1px solid; padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse; border-top-width: 1px; border-top-color: #333333;">
                                                            <asp:Label ID="LabelTableRFValue1" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                        <td align="right" style="width: 69px; border-right: #333333 1px solid; padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse; border-top-width: 1px; border-top-color: #333333;">
                                                            <asp:Label ID="LabelTableSubjectValue1" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr style="height: 24px;">
                                                        <td align="right" style="width: 53px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableRFValue2" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                        <td align="right" style="width: 69px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableSubjectValue2" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr style="height: 24px;">
                                                        <td align="right" style="width: 53px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableRFValue3" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                        <td align="right" style="width: 69px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableSubjectValue3" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr style="height: 24px;">
                                                        <td align="right" style="width: 53px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableRFValue4" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                        <td align="right" style="width: 69px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableSubjectValue4" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr style="height: 24px;">
                                                        <td align="right" style="width: 53px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableRFValue5" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                        <td align="right" style="width: 69px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableSubjectValue5" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr style="height: 24px;">
                                                        <td align="right" style="width: 53px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableRFValue6" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                        <td align="right" style="width: 69px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableSubjectValue6" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr style="height: 24px;">
                                                        <td align="right" style="width: 53px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableRFValue7" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                        <td align="right" style="width: 69px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableSubjectValue7" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr style="height: 24px;">
                                                        <td align="right" style="width: 53px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableRFValue8" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                        <td align="right" style="width: 69px; border-right: #333333 1px solid; padding-right: 0px;
                                                            border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px;
                                                            border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid;
                                                            border-collapse: collapse;">
                                                            <asp:Label ID="LabelTableSubjectValue8" runat="server" EnableTheming="True" SkinID="InformationText"
                                                                Text="Label"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="table2" style="width: 315px; padding-right: 0px; padding-left: 0px; padding-bottom: 0px;
                            border-top-style: none; padding-top: 0px; border-right-style: none;
                            border-left-style: none; border-bottom-style: none; border-collapse: collapse; margin-top: -15px;">
                            <tr>
                                <td style="width: 315;" align="right" valign="middle">
                                    <asp:Label ID="Label100K" runat="server" Text="Label" SkinID="ServeText"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
