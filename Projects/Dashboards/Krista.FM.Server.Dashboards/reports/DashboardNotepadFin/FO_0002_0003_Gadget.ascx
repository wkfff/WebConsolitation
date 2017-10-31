<%@ Control Language="C#" AutoEventWireup="true" Codebehind="FO_0002_0003_Gadget.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.DashboardNotepadFin.FO_0002_0003_Gadget" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<div class="GadgetTopDate">
    <asp:Label ID="topLabel" runat="server" Text="Label"></asp:Label>
</div>
<asp:Panel ID="MainPanel" runat="server" CssClass="GadgetMainPanel" Height="100%"
    Width="100%">
    <table border="0" cellspacing="0">
        <tr>
            <td style="width: 200px;">
                <igchart:UltraChart ID="UltraChart2" runat="server" BackgroundImageFileName="" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                    Version="9.1" OnDataBinding="UltraChart_DataBinding">
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
                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString=""
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                                    VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                            </Labels>
                        </Z>
                        <Y2 LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="False">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
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
                        <Y LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="True">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="VerticalLeftFacing"
                                    VerticalAlign="Center">
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
                                Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
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
                                Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                                    VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                            </Labels>
                        </Z2>
                    </Axis>
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_FK0107_#SEQNUM(100).png" />
                </igchart:UltraChart>
            </td>
        </tr>
    </table>
</asp:Panel>
