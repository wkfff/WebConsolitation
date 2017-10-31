<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="DefaultCompare.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FK_0001_0004.DefaultCompare" Title="��������� ����� ����� ������� �� ��������� ��" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc5" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td style="width: 100%">
                <uc5:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="DefaultCompare.html" />
                &nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" Text="Label" CssClass="PageTitle"></asp:Label>&nbsp;&nbsp;
                <asp:Label ID="Label2" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label></td>
            <td>
                <uc4:UltraGridExporter ID="UltraGridExporter1" runat="server" />
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
                <uc3:CustomMultiCombo ID="ComboFO" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboSKIFLevel" runat="server"></uc3:CustomMultiCombo>
            </td> 
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            <td style="width: 100%" align="right" valign="top">
                <uc2:GridSearch ID="GridSearch1" runat="server" />
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
                            <igmisc:WebAsyncRefreshPanel ID="gridWebAsyncPanel" runat="server" TriggerControlIDs="FlatCheckBox">
                                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" EnableAppStyling="True" Height="200px"
                                    OnDataBinding="UltraWebGrid_DataBinding" OnInitializeLayout="UltraWebGrid_InitializeLayout"
                                    StyleSetName="Office2007Blue" SkinID="UltraWebGrid" Width="325px" OnInitializeRow="UltraWebGrid_InitializeRow"
                                    OnActiveRowChange="UltraWebGrid_ActiveRowChange">
                                    <Bands>
                                        <igtbl:UltraGridBand>
                                            <AddNewRow View="NotSet" Visible="NotSet">
                                            </AddNewRow>
                                        </igtbl:UltraGridBand>
                                    </Bands>
                                    <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                        StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                        StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1"
                                        BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                        TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer"
                                        SelectTypeRowDefault="Extended">
                                        <GroupByBox>
                                            <BoxStyle BorderColor="Window" BackColor="ActiveBorder">
                                            </BoxStyle>
                                        </GroupByBox>
                                        <GroupByRowStyleDefault BorderColor="Window" BackColor="Control">
                                        </GroupByRowStyleDefault>
                                        <ActivationObject BorderWidth="" BorderColor="">
                                        </ActivationObject>
                                        <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                        </FooterStyleDefault>
                                        <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt"
                                            Font-Names="Microsoft Sans Serif" BackColor="Window">
                                            <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                            <Padding Left="3px"></Padding>
                                        </RowStyleDefault>
                                        <FilterOptionsDefault>
                                            <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid"
                                                Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White"
                                                CustomRules="overflow:auto;">
                                                <Padding Left="2px"></Padding>
                                            </FilterOperandDropDownStyle>
                                            <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                            </FilterHighlightRowStyle>
                                            <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                                Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px"
                                                Height="300px" CustomRules="overflow:auto;">
                                                <Padding Left="2px"></Padding>
                                            </FilterDropDownStyle>
                                        </FilterOptionsDefault>
                                        <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                        </HeaderStyleDefault>
                                        <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                        </EditCellStyleDefault>
                                        <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                            Font-Names="Microsoft Sans Serif" BackColor="Window" Width="325px" Height="200px">
                                        </FrameStyle>
                                        <Pager MinimumPagesForDisplay="2">
                                            <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                            </PagerStyle>
                                        </Pager>
                                        <AddNewBox Hidden="False">
                                            <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                            </BoxStyle>
                                        </AddNewBox>
                                    </DisplayLayout>
                                </igtbl:UltraWebGrid>
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
        <tr>
            <td>
                <igmisc:WebAsyncRefreshPanel ID="chartWebAsyncPanel" runat="server" TriggerControlIDs="UltraWebGrid">
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
                                <asp:Label ID="lbSubject" runat="server" Text="Label" CssClass="ElementTitle"></asp:Label>&nbsp;
                                <asp:Label ID="lbSubjectSub" runat="server" Text="Label" CssClass="ElementSubTitle"></asp:Label>
                            </td>
                            <td class="headerright">
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                            </td>
                            <td style="overflow: visible;">
                                <table>
                                    <tr>
                                        <td>
                                            <igchart:UltraChart ID="UltraChart" runat="server" OnDataBinding="UltraChart_DataBinding"
                                                Version="8.2" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                                BackgroundImageFileName="">
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
                                                            HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
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
                                                            HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
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
                                                            Font="Verdana, 7pt" VerticalAlign="Center">
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
                                                <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_FK0104_#SEQNUM(100).png" />
                                            </igchart:UltraChart>
                                        </td>
                                    </tr>
                                </table>
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
                </igmisc:WebAsyncRefreshPanel>
            </td>
        </tr>
    </table>
</asp:Content>