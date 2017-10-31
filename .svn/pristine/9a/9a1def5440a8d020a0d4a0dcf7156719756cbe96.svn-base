<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FO_0002_0017_02.Default" %>

<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter" TagPrefix="uc4" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc2" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
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
                <span class="help_sp" title="Справка"></span>&nbsp;&nbsp;
                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label><br />
                <asp:Label ID="PageSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td>
                <uc4:UltraGridExporter ID="UltraGridExporter1" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <table style="vertical-align: top;">
                    <tr>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboYear" runat="server"></uc3:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboBill" runat="server"></uc3:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboFKR" runat="server"></uc3:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc1:RefreshButton ID="RefreshButton1" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top" align="left">
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
                            <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" EnableAppStyling="True" EnableTheming="True" Height="114px"
                                OnDataBinding="UltraWebGrid_DataBinding" OnInitializeLayout="UltraWebGrid_InitializeLayout" StyleSetName="Office2007Blue"
                                Width="627px" SkinID="UltraWebGrid">
                                <Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                                </Bands>
                                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                    AllowSortingDefault="OnClient" BorderCollapseDefault="Separate" CellClickActionDefault="RowSelect"
                                    ColWidthDefault="130px" HeaderClickActionDefault="SortMulti" Name="grid3" NoDataMessage="Нет данных за указанный период"
                                    RowHeightDefault="15px" SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                                    TableLayout="Fixed" Version="4.00" ViewType="Hierarchical">
                                    <GroupByBox Hidden="True">
                                        <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                                        </BoxStyle>
                                    </GroupByBox>
                                    <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
                                    </GroupByRowStyleDefault>
                                    <ActivationObject BorderColor="" BorderWidth="">
                                    </ActivationObject>
                                    <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                    </FooterStyleDefault>
                                    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif"
                                        Font-Size="8.25pt">
                                        <BorderDetails ColorLeft="Window" ColorTop="Window" />
                                        <Padding Left="3px" />
                                    </RowStyleDefault>
                                    <FilterOptionsDefault AllowRowFiltering="OnClient">
                                        <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                            CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px">
                                            <Padding Left="2px" />
                                        </FilterOperandDropDownStyle>
                                        <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                                        </FilterHighlightRowStyle>
                                        <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;"
                                            Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px" Height="300px" Width="200px">
                                            <Padding Left="2px" />
                                        </FilterDropDownStyle>
                                    </FilterOptionsDefault>
                                    <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left" Wrap="True">
                                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                    </HeaderStyleDefault>
                                    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                                    </EditCellStyleDefault>
                                    <FrameStyle BackColor="Window" BorderStyle="Solid" BorderWidth="1px" Font-Italic="False" Font-Names="Microsoft Sans Serif"
                                        Font-Overline="False" Font-Size="8.25pt" Font-Strikeout="False" Font-Underline="False" Height="114px"
                                        Width="627px">
                                        <Padding Bottom="0px" Left="0px" Right="0px" Top="0px" />
                                    </FrameStyle>
                                    <Pager MinimumPagesForDisplay="2">
                                        <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                        </PagerStyle>
                                    </Pager>
                                    <AddNewBox>
                                        <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                        </BoxStyle>
                                    </AddNewBox>
                                </DisplayLayout>
                            </igtbl:UltraWebGrid>
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
                <table style="vertical-align: top;">
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
                                        <igchart:UltraChart ID="UltraChart" runat="server" BackgroundImageFileName=""
                                            EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                            Version="9.1" OnDataBinding="UltraChart_DataBinding">
                                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
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
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="" Orientation="Horizontal"
                                                        VerticalAlign="Center">
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
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="True" />
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
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="True" />
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
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="True" />
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
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="True" />
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
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="False" />
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="True" />
                                                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="" Orientation="Horizontal"
                                                        VerticalAlign="Center" Visible="False">
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
                                            <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_FO0207_#SEQNUM(100).png" />
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
