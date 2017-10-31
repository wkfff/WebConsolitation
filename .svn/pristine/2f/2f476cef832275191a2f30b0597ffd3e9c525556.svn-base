<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.OMCY_0004._default" %>


<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc6" %>

<%@ Register Src="../../Components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc2" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc3" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc4" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc5" %>
    
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>

<%--<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>--%>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
    


<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>



<asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
    <div>
        <table>
            <tr>
                <td style="width: 100%">
                    <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Help.html" />
                    <asp:Label ID="Hederglobal" runat="server" CssClass="PageTitle">Размещение заказа по основным группам закупаемой продукции</asp:Label><br />
                    <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle">Анализ социально-экономического положения территории по выбранному показателю</asp:Label></td>
                <td>
                    &nbsp;<uc5:UltraGridExporter ID="UltraGridExporter1" runat="server" Visible="true" />
                </td>
            </tr>
        </table>
        <table style="vertical-align: top">
            <tr>
                <td valign="top" colspan="2">
                    <uc2:CustomMultiCombo ID="FOPARAM" runat="server" Title="Месяц" />
                </td>
                <td colspan="1" valign="top">
                    <uc2:CustomMultiCombo ID="MRGO" runat="server" Title="ФО" Visible="false" />
                </td>
                <td valign="top">
                    <uc3:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
                <td align="right" style="width: 100%" valign="top">
                    &nbsp;&nbsp;
                    </td>
            </tr>
        </table>
        <table>
            <tr>
                <td colspan="2">
                    <asp:RadioButton ID="RadioButton1" runat="server" AutoPostBack="True" GroupName="MO"
                        Text="Городские округа" ValidationGroup="MO" /><br />
                    <asp:RadioButton ID="UserMO" runat="server" AutoPostBack="True" Checked="True" GroupName="MO"
                        Text="Муниципальные районы" ValidationGroup="MO" /><table>
                        <tr>
                            <td colspan="2"><table style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;">
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
                                    <td style="vertical-align: top;">
                                        <asp:Label ID="Label4" runat="server" CssClass="PageSubTitle">Расчет неэффективных расходов в сфере  «Образование» за 2009 год</asp:Label>
                                            <igtbl:UltraWebGrid
                                    ID="G" runat="server"
                                    SkinID="UltraWebGrid" StyleSetName="Office2007Blue" OnDataBinding="G_DataBinding" OnInitializeLayout="G_InitializeLayout" OnInitializeRow="G_InitializeRow">
                                                <Bands>
                                                    <igtbl:UltraGridBand>
                                                        <AddNewRow View="NotSet" Visible="NotSet">
                                                        </AddNewRow>
                                                    </igtbl:UltraGridBand>
                                                </Bands>
                                                <DisplayLayout Name="G"
                                        RowHeightDefault="20px" RowSelectorsDefault="No" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                                        Version="4.00" ViewType="OutlookGroupBy" SortCaseSensitiveDefault="False" SortingAlgorithmDefault="NotSet" HeaderClickActionDefault="NotSet">
                                                    <GroupByBox>
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
                                                    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                            Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                                                        <BorderDetails ColorLeft="Window" ColorTop="Window" />
                                                        <Padding Left="3px" />
                                                    </RowStyleDefault>
                                                    <FilterOptionsDefault>
                                                        <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid"
                                                BorderWidth="1px" CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                                                Font-Size="11px">
                                                            <Padding Left="2px" />
                                                        </FilterOperandDropDownStyle>
                                                        <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                                                        </FilterHighlightRowStyle>
                                                        <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                                CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                                                Font-Size="11px" Height="300px" Width="200px">
                                                            <Padding Left="2px" />
                                                        </FilterDropDownStyle>
                                                    </FilterOptionsDefault>
                                                    <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
                                                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                                    </HeaderStyleDefault>
                                                    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                                                    </EditCellStyleDefault>
                                                    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
                                            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                                                    </FrameStyle>
                                                    <Pager MinimumPagesForDisplay="2">
                                                        <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                                        </PagerStyle>
                                                    </Pager>
                                                    <AddNewBox Hidden="False">
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
                            </td>
                        </tr>
                    </table>
                                <table runat="server"  style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;" id="CTable">
                                    <tr>
                                        <td class="topleft">
                                        </td>
                                        <td class="top">
                                        </td>
                                        <td class="topright">
                                        </td>
                                    </tr><tr>
                                        <td class="left">
                                        </td>
                                        <td style="vertical-align: top; height: 360px;">
                                            <asp:Label ID="CL" runat="server" CssClass="ElementTitle"></asp:Label><igchart:UltraChart ID="C" runat="server" BackgroundImageFileName=""  
                                                 EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                                Height="400px" Version="9.1" Width="500px" OnDataBinding="C_DataBinding" ChartType="StackColumnChart">
                                                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                    Font-Underline="False" FormatString="&lt;b&gt;&lt;DATA_VALUE:### ### ##0.00&gt;&lt;/b&gt; тыс. рублей" />
                                                <Border Color="Transparent" />
                                                <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_03_08_2.png" />
                                                <ColorModel AlphaLevel="150" ColorBegin="128, 64, 0" ColorEnd="224, 224, 224">
                                                </ColorModel>
                                                <Effects>
                                                    <Effects>
                                                        <igchartprop:GradientEffect>
                                                        </igchartprop:GradientEffect>
                                                    </Effects>
                                                </Effects>
                                                <Axis>
                                                    <PE ElementType="None" Fill="Cornsilk" />
                                                    <X Extent="44" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                                        <Margin>
                                                            <Near MarginType="Pixels" />
                                                        </Margin>
                                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                            Visible="False" />
                                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                            Visible="False" />
                                                        <Labels Font="Verdana, 8pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                            Orientation="VerticalLeftFacing" VerticalAlign="Near">
                                                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                                                                VerticalAlign="Near">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                            </SeriesLabels>
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </Labels>
                                                    </X>
                                                    <Y Extent="50" TickmarkInterval="200" TickmarkStyle="Smart" Visible="True">
                                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="0"
                                                            Visible="False" />
                                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="DashDotDot" Thickness="0"
                                                            Visible="False" />
                                                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:### ### ##0.##&gt;"
                                                            Orientation="Horizontal" VerticalAlign="Near">
                                                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                                                Orientation="Horizontal" OrientationAngle="95" VerticalAlign="Near">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                            </SeriesLabels>
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </Labels>
                                                    </Y>
                                                    <Y2 Extent="0" LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="False">
                                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                            Visible="True" />
                                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                            Visible="False" />
                                                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                            Orientation="Horizontal" VerticalAlign="Near" Visible="False">
                                                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                                                Orientation="Horizontal" VerticalAlign="Near">
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
                                                            Orientation="VerticalLeftFacing" VerticalAlign="Near" Visible="False">
                                                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                                                                VerticalAlign="Center">
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
                                                            Orientation="Horizontal" VerticalAlign="Near">
                                                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                                                VerticalAlign="Near">
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
                                                            Orientation="Horizontal" VerticalAlign="Near" Visible="False">
                                                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                                                VerticalAlign="Near">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                            </SeriesLabels>
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </Labels>
                                                    </Z2>
                                                </Axis>
                                                <ColumnChart NullHandling="DontPlot" SeriesSpacing="0">
                                                    <ChartText>
                                                        <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:### ##0.##&gt;"
                                                            Row="-2" VerticalAlign="Far">
                                                        </igchartprop:ChartTextAppearance>
                                                    </ChartText>
                                                </ColumnChart>
                                                <Legend Location="Bottom" SpanPercentage="16" Visible="True"></Legend>
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
            <tr>
                <td colspan="2">
                    <asp:CheckBox ID="CheckBox1" runat="server" Text="Сравнить с предыдущим годом" /><br />
                    <table style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;">
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
                            <td style="vertical-align: top; height: 360px;">
                                <asp:Label ID="Label3" runat="server" CssClass="ElementTitle"></asp:Label><igchart:UltraChart ID="C2" runat="server" BackgroundImageFileName=""  
                                                 EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                                Height="400px" Version="9.1" Width="500px" OnDataBinding="C2_DataBinding" ChartType="StackColumnChart" OnFillSceneGraph="C2_FillSceneGraph">
                                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                    Font-Underline="False" FormatString="&lt;b&gt;&lt;DATA_VALUE:### ### ##0.00&gt;&lt;/b&gt; тыс. рублей" />
                                    <Border Color="Transparent" />
                                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_05_08_2.png" />
                                    <ColorModel AlphaLevel="150" ColorBegin="128, 64, 0" ColorEnd="224, 224, 224">
                                    </ColorModel>
                                    <Effects>
                                        <Effects>
                                            <igchartprop:GradientEffect>
                                            </igchartprop:GradientEffect>
                                        </Effects>
                                    </Effects>
                                    <Axis>
                                        <PE ElementType="None" Fill="Cornsilk" />
                                        <X Extent="44" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                            <Margin>
                                                <Near MarginType="Pixels" />
                                            </Margin>
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                            Visible="False" />
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                            Visible="False" />
                                            <Labels Font="Verdana, 8pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                            Orientation="VerticalLeftFacing" VerticalAlign="Near">
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                                                                VerticalAlign="Near">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </Labels>
                                        </X>
                                        <Y Extent="50" TickmarkInterval="200" TickmarkStyle="Smart" Visible="True">
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="0"
                                                            Visible="False" />
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="DashDotDot" Thickness="0"
                                                            Visible="False" />
                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:### ### ##0.##&gt;"
                                                            Orientation="Horizontal" VerticalAlign="Near">
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                                                Orientation="Horizontal" OrientationAngle="95" VerticalAlign="Near">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </Labels>
                                        </Y>
                                        <Y2 Extent="0" LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="False">
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                            Visible="True" />
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                            Visible="False" />
                                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                            Orientation="Horizontal" VerticalAlign="Near" Visible="False">
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                                                Orientation="Horizontal" VerticalAlign="Near">
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
                                                            Orientation="VerticalLeftFacing" VerticalAlign="Near" Visible="False">
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                                                                VerticalAlign="Center">
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
                                                            Orientation="Horizontal" VerticalAlign="Near">
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                                                VerticalAlign="Near">
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
                                                            Orientation="Horizontal" VerticalAlign="Near" Visible="False">
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                                                VerticalAlign="Near">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </Labels>
                                        </Z2>
                                    </Axis>
                                    <ColumnChart NullHandling="DontPlot" SeriesSpacing="0">
                                        <ChartText>
                                            <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:### ##0.##&gt;"
                                                            Row="-2" VerticalAlign="Far">
                                            </igchartprop:ChartTextAppearance>
                                        </ChartText>
                                    </ColumnChart>
                                    <Legend Location="Bottom" SpanPercentage="16" Visible="True"></Legend>
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
                    <table style="width: 100%; border-collapse: collapse; height: 100%; background-color: white">
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
                            <td style="vertical-align: top; height: 360px">
                                <asp:Label ID="Label1" runat="server" CssClass="ElementTitle"></asp:Label><br />
                                <DMWC:MapControl ID="map" runat="server" BackColor="White" ImageUrl="../../TemporaryImages/map_fk_01_02_#SEQ(300,3)"
                                    RenderingImageUrl="../../TemporaryImages/" ResourceKey="#MapControlResKey#MapControl1#">
                                    <NavigationPanel>
                                        <Location X="0" Y="0" />
                                        <Size Height="90" Width="90" />
                                    </NavigationPanel>
                                    <Viewport>
                                        <Location X="0" Y="0" />
                                        <Size Height="100" Width="100" />
                                    </Viewport>
                                    <ZoomPanel>
                                        <Location X="0" Y="0" />
                                        <Size Height="200" Width="40" />
                                    </ZoomPanel>
                                    <ColorSwatchPanel>
                                        <Size Height="60" Width="350" />
                                        <Location X="0" Y="0" />
                                    </ColorSwatchPanel>
                                    <DistanceScalePanel>
                                        <Location X="0" Y="0" />
                                        <Size Height="55" Width="130" />
                                    </DistanceScalePanel>
                                </DMWC:MapControl>
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
        </div>


 </asp:Content>

