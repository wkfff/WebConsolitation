<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="sgm_0009.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.SGM.SGM_0009.sgm_0009" Title="5 самых болеющих субъектов РФ(на 100 тыс. населения)" %>

<%@ Register Src="../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc2" %>
<%@ Register Src="../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <table style="vertical-align: top;">
            <tr>
                <td style="width: 100%">
                    <asp:Label ID="LabelTitle" runat="server" CssClass="PageTitle"></asp:Label>
                </td>
                <td>
                    <uc3:UltraGridExporter ID="UltraGridExporter1" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="width: 100%">
                    <asp:Label ID="LabelSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
                </td>
                <td>
                </td>
            </tr>
        </table>
        <table style="vertical-align: top;">
            <tr>
                <td valign="top">
                    <uc1:CustomMultiCombo ID="ComboDeseases" runat="server" MultiSelect="false" Title="Заболевание"
                        Width="350" ParentSelect="true" />
                </td>
                <td valign="top">
                    <uc1:CustomMultiCombo ID="ComboYear" runat="server" MultiSelect="false" Title="Год" />
                </td>
                <td valign="top">
                    <uc1:CustomMultiCombo ID="ComboMonth" runat="server" MultiSelect="true" Title="Месяц"
                        Width="150" />
                </td>
                <td valign="top">
                    <uc1:CustomMultiCombo ID="ComboGroup" runat="server" MultiSelect="false" Title="Группа населения"
                        Width="150" />
                </td>
                <td valign="middle" align="left">
                    <asp:Label ID="LabelCriterium" runat="server" CssClass="ParamChooseTitle" Text="Label"
                        Width="175px"></asp:Label>
                </td>
                <td valign="top" align="left">
                    <asp:RadioButtonList ID="RadioCriterium" runat="server" AutoPostBack="True" RepeatDirection="Horizontal"
                        Width="100px">
                        <asp:ListItem Selected="True">РФ</asp:ListItem>
                        <asp:ListItem>ФО</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td valign="top">
                    <uc2:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <table style="vertical-align: top;">
        <tr>
            <td colspan="3">
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
                            <igchart:UltraChart ID="chart" runat="server" BackgroundImageFileName=""  
                                 EmptyChartText="Нет данных"
                                Version="9.1" OnFillSceneGraph="chart_FillSceneGraph">
                                <TitleTop HorizontalAlign="Center" Font="Microsoft Sans Serif, 9.75pt, style=Bold"
                                    Text="5 самых болеющих субъектов РФ">
                                </TitleTop>
                                <Tooltips Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False"
                                    Font-Bold="False" FormatString="&lt;ITEM_LABEL&gt;: &lt;DATA_VALUE:00.##&gt;">
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
                                    <Y2 LineThickness="1" TickmarkInterval="10" Visible="False" TickmarkStyle="Smart">
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
                                    <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart" Extent="20">
                                        <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                        </MinorGridLines>
                                        <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                        </MajorGridLines>
                                        <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray"
                                            HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" Visible="False">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                            <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                                Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                        </Labels>
                                    </X>
                                    <Y LineThickness="1" TickmarkInterval="10" Visible="True" TickmarkStyle="Smart" Extent="40">
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
                                <Legend SpanPercentage="7" Visible="True" Location="Bottom"></Legend>
                                <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_sgm0009_01_#SEQNUM(100).png" />
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
            <td colspan="3">
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
                            <igtbl:UltraWebGrid ID="grid" runat="server" Height="200px" OnInitializeLayout="grid_InitializeLayout"
                                OnInitializeRow="grid_InitializeRow" SkinID="UltraWebGrid" Width="325px">
                                <Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                                </Bands>
                                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                    AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                                    HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1" RowHeightDefault="20px"
                                    RowSelectorsDefault="No" SelectTypeRowDefault="Extended" StationaryMargins="Header"
                                    StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
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
                                        BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="200px"
                                        Width="325px">
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
</asp:Content>
