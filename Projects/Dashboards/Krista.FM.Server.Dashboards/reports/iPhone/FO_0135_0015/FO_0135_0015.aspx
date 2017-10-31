<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FO_0135_0015.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0135_0015" %>

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
        <div style="position: absolute; width: 768px; height: 950px; background-color: black; top: 0px; left: 0px;
            z-index: 2; overflow: hidden">
            <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black; top: 0px;
                left: 0px">
                <tr>
                    <td>
                        <table style="border-collapse: collapse;">
                            <tr>
                                <td align="left" valign="top">
                                    <table style="border-collapse: collapse; background-color: Black; width: 100%; height: 100%;">
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
                                                <asp:Label ID="chart3ElementCaption" runat="server" CssClass="ElementTitle" Text="Доходы"></asp:Label>
                                            </td>
                                            <td style="background: Black url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                    <table style="border-collapse: collapse;">
                                        <tr>
                                            <td style="overflow: visible; background-color: Black">
                                                <div runat="server" id="HeraldImageContainer" style="float: left; margin-left: 32px">
                                                </div>
                                                <table style="border-collapse: collapse; margin-left: 20px">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lbIncomesPlanTitle" runat="server" SkinID="InformationText" Text="План&nbsp;"></asp:Label></td>
                                                        <td align="right">
                                                            <asp:Label ID="lbIncomesPlanValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></td>
                                                        <td>
                                                            <asp:Label ID="lbIncomesPlanMeasures" runat="server" SkinID="InformationText" Text="&nbsp;тыс.руб."></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lbIncomesFactTitle" runat="server" SkinID="InformationText" Text="Факт&nbsp;"></asp:Label></td>
                                                        <td align="right">
                                                            <asp:Label ID="lbIncomesFactValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></td>
                                                        <td>
                                                            <asp:Label ID="lbIncomesFactMeasures" SkinID="InformationText" runat="server" Text="&nbsp;тыс.руб."></asp:Label></td>
                                                    </tr>
                                                    <tr style="margin-top: -3px;">
                                                        <td>
                                                            <asp:Label ID="lbIncomesExecutedTitle" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label></td>
                                                        <td align="right">
                                                            <asp:Label ID="lbIncomesExecutedValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></td>
                                                        <td>
                                                            <asp:Label ID="lbIncomesExecutedMeasures" SkinID="DigitsValueSmall" runat="server" Text="%"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                                                            <asp:Label ID="lbIncomesRankFO" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label>
                                                            <asp:Label ID="lbIncomesRankFOValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <div style="margin-left: 20px; margin-top: -10px">
                                                    <asp:PlaceHolder ID="GaugeIncomesPlaceHolder" runat="server"></asp:PlaceHolder>
                                                </div>
                                                <div style="margin-left: 15px; margin-top: -1px">
                                                    <div style="margin-top: -1px">
                                                        <asp:Label ID="lbPopulation" runat="server" SkinID="InformationText" Text=""></asp:Label><br />
                                                        <asp:Label ID="lbIncomesAverageTitle" runat="server" SkinID="InformationText" Text="Среднедушевые&nbsp;доходы&nbsp;"></asp:Label>
                                                        <asp:Label ID="lbIncomesAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                        <asp:Label SkinID="InformationText" ID="lbIncomesAverageMeasures" runat="server" Text="&nbsp;руб./чел."></asp:Label></div>
                                                    <div style="margin-left: 104px;">
                                                        <asp:Label ID="lbIncomesRankFOAverage" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label>
                                                        <asp:Label ID="lbIncomesRankFOAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></div>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                </td>
                                <td style="text-align: left; background-color: Black" align="left" valign="top">
                                    <table style="border-collapse: collapse; background-color: Black; width: 100%; height: 100%;">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px;">
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
                                                width: 2px; height: 36px;">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label1" runat="server" CssClass="ElementTitle" Text="Расходы"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                    <table style="border-collapse: collapse;">
                                        <tr>
                                            <td style="overflow: visible; background-color: Black">
                                                <table style="border-collapse: collapse; margin-left: 30px">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lbOutcomesPlanTitle" runat="server" SkinID="InformationText" Text="План&nbsp;"></asp:Label></td>
                                                        <td align="right">
                                                            <asp:Label ID="lbOutcomesPlanValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></td>
                                                        <td>
                                                            <asp:Label SkinID="InformationText" ID="lbOutcomesPlanMeasures" runat="server" Text="&nbsp;тыс.руб. "></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lbOutcomesFactTitle" runat="server" SkinID="InformationText" Text="Факт&nbsp;"></asp:Label></td>
                                                        <td align="right">
                                                            <asp:Label ID="lbOutcomesFactValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></td>
                                                        <td>
                                                            <asp:Label ID="lbOutcomesFactMeasures" SkinID="InformationText" runat="server" Text="&nbsp;тыс.руб."></asp:Label></td>
                                                    </tr>
                                                    <tr style="margin-top: -3px;">
                                                        <td>
                                                            <asp:Label ID="lbOutcomesExecutedTitle" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label></td>
                                                        <td align="right">
                                                            <asp:Label ID="lbOutcomesExecutedValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></td>
                                                        <td>
                                                            <asp:Label ID="lbOutcomesExecutedMeasures" SkinID="DigitsValueSmall" runat="server" Text="%"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                                                            <asp:Label ID="lbOutcomesRankFO" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label>
                                                            <asp:Label ID="lbOutcomesRankFOValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <div style="margin-left: 2px; margin-top: -15px">
                                                    <asp:PlaceHolder ID="GaugeOutcomesPlaceHolder" runat="server"></asp:PlaceHolder>
                                                </div>
                                                <div style="margin-left: 59px; margin-top: -1px">
                                                    <div style="margin-top: -1px">
                                                        <asp:Label ID="lbOutcomesAverageTitle" runat="server" SkinID="InformationText" Text="Бюджетные расходы на душу населения&nbsp;"></asp:Label>
                                                        <asp:Label ID="lbOutcomesAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                        <asp:Label SkinID="InformationText" ID="lbOutcomesAverageMeasures" runat="server" Text="&nbsp;руб./чел."></asp:Label></div>
                                                    <div style="margin-left: 27px;">
                                                        <asp:Label ID="lbOutcomesRankFOAverage" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label>
                                                        <asp:Label ID="lbOutcomesRankFOAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <table style="border-collapse: collapse; background-color: Black; width: 100%; height: 100%; margin-right: 10px;
                                        margin-top: -15px;">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px;">
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
                                                width: 2px; height: 36px;">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label2" runat="server" CssClass="ElementTitle" Text="Основные показатели"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="overflow: visible; background-color: Black;">
                                                <igtbl:UltraWebGrid ID="UltraWebGridBudget" runat="server" Height="200px" Width="509px" OnDataBinding="UltraWebGridBudget_DataBinding"
                                                    OnInitializeLayout="UltraWebGridBudget_InitializeLayout" SkinID="UltraWebGrid" OnInitializeRow="UltraWebGridBudget_InitializeRow">
                                                    <Bands>
                                                        <igtbl:UltraGridBand>
                                                            <AddNewRow View="NotSet" Visible="NotSet">
                                                            </AddNewRow>
                                                        </igtbl:UltraGridBand>
                                                    </Bands>
                                                    <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient" StationaryMargins="Header"
                                                        AllowColSizingDefault="Free" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti"
                                                        Name="UltraWebGrid" BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                                        TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer" SelectTypeRowDefault="Extended">
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
                                                        <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif"
                                                            BackColor="Window">
                                                            <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                                            <Padding Left="3px"></Padding>
                                                        </RowStyleDefault>
                                                        <FilterOptionsDefault>
                                                            <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                                                Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" CustomRules="overflow:auto;">
                                                                <Padding Left="2px"></Padding>
                                                            </FilterOperandDropDownStyle>
                                                            <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                                            </FilterHighlightRowStyle>
                                                            <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                                                                BackColor="White" Width="200px" Height="300px" CustomRules="overflow:auto;">
                                                                <Padding Left="2px"></Padding>
                                                            </FilterDropDownStyle>
                                                        </FilterOptionsDefault>
                                                        <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                                        </HeaderStyleDefault>
                                                        <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                                        </EditCellStyleDefault>
                                                        <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif"
                                                            BackColor="Window" Width="509px" Height="200px">
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
                                                </igtbl:UltraWebGrid></td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                </td>
                                <td valign="top">
                                    <table style="border-collapse: collapse; background-color: Black; width: 100%; height: 100%; margin-top: -15px;">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px;">
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
                                                width: 2px; height: 36px;">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label3" runat="server" CssClass="ElementTitle" Text="Соблюдение БК РФ"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                    <table>
                                        <tr>
                                            <td style="overflow: visible; background-color: Black">
                                                <table class="TableFont" style="border-collapse: collapse">
                                                    <tr>
                                                        <td align="center" style="width: 110px">
                                                            Муниципальн. долг<br />
                                                            <asp:Label ID="lbDebts" runat="server" CssClass="TableFont" Text=""></asp:Label><br />
                                                            <asp:Label ID="Label31" runat="server" SkinID="InformationText" Text=" тыс.руб."></asp:Label>
                                                            <igchart:UltraChart ID="UltraChart11" runat="server" BackgroundImageFileName=""  
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
                                                                <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_11#SEQNUM(100).png" />
                                                            </igchart:UltraChart>
                                                        </td>
                                                        <td align="center" style="width: 110px">
                                                            Дефицит<br />
                                                            <br />
                                                            <asp:Label ID="lbDeficite" runat="server" CssClass="TableFont" Text=""></asp:Label><br />
                                                            <asp:Label ID="Label30" runat="server" SkinID="InformationText" Text=" тыс.руб."></asp:Label>
                                                            <igchart:UltraChart ID="UltraChart12" runat="server" BackgroundImageFileName=""  
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
                                                                <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_12#SEQNUM(100).png" />
                                                            </igchart:UltraChart>
                                                        </td>
                                                        <td align="center" style="width: 110px;">
                                                            Расходы на обслуж. долга<br />
                                                            <asp:Label ID="lbDebtServe" runat="server" CssClass="TableFont" Text=""></asp:Label><br />
                                                            <asp:Label ID="Label29" runat="server" SkinID="InformationText" Text=" тыс.руб."></asp:Label>
                                                            <igchart:UltraChart ID="UltraChart13" runat="server" BackgroundImageFileName=""  
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
                                                                <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_21#SEQNUM(100).png" />
                                                            </igchart:UltraChart>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                    <table style="border-collapse: collapse; background-color: Black; width: 100%; height: 100%; margin-top: -5px">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px;">
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
                                                width: 2px; height: 36px;">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label4" runat="server" CssClass="ElementTitle" Text="Кредиторская задолженность"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 315px; background-color: black; padding-left: 6px" valign="top" colspan="3">
                                                <asp:Label ID="creditDebts" runat="server" SkinID="InformationText" Text="Количество нарушений требований бюджетного кодекса РФ"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <table style="border-collapse: collapse; background-color: Black; width: 100%; height: 100%; margin-top: 1px">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px;">
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
                                                width: 2px; height: 36px;">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label5" runat="server" CssClass="ElementTitle" Text="Недоимка"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                    <table>
                                        <tr>
                                            <td style="width: 315px; background-color: black;" valign="top">
                                                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="200px" OnDataBinding="UltraWebGrid_DataBinding"
                                                    OnInitializeLayout="Grid_InitializeLayout" Width="315px" SkinID="UltraWebGrid" OnInitializeRow="Grid_InitializeRow">
                                                    <Bands>
                                                        <igtbl:UltraGridBand>
                                                            <AddNewRow View="NotSet" Visible="NotSet">
                                                            </AddNewRow>
                                                        </igtbl:UltraGridBand>
                                                    </Bands>
                                                    <DisplayLayout AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" HeaderClickActionDefault="SortMulti"
                                                        Name="UltraWebGrid" RowHeightDefault="20px" RowSelectorsDefault="No" ScrollBar="Never" SelectTypeRowDefault="Extended"
                                                        StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00"
                                                        BorderCollapseDefault="Separate" GridLinesDefault="None">
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
                                                        <RowStyleDefault BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px" Font-Bold="False"
                                                            Font-Names="Arial" Font-Size="16px" ForeColor="White" HorizontalAlign="Right" Wrap="True">
                                                            <Padding Bottom="5px" Left="3px" Right="3px" Top="5px" />
                                                        </RowStyleDefault>
                                                        <FilterOptionsDefault>
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
                                                        <HeaderStyleDefault BackColor="#323232" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px" Font-Bold="True"
                                                            Font-Names="Arial" Font-Size="18px" ForeColor="White" HorizontalAlign="Center" Wrap="True">
                                                            <BorderDetails WidthLeft="1px" WidthTop="1px" />
                                                        </HeaderStyleDefault>
                                                        <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                                                        </EditCellStyleDefault>
                                                        <FrameStyle BackColor="Black" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif"
                                                            Font-Size="8.25pt" Height="200px" Width="315px">
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
                                        </tr>
                                    </table>
                                    <table style="border-collapse: collapse; background-color: Black; width: 100%; height: 100%;" runat="server"
                                        id="settlementHeader">
                                        <tr>
                                            <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                                width: 1px;">
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
                                                width: 2px; height: 36px;">
                                            </td>
                                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px; padding-left: 3px;
                                                height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label6" runat="server" CssClass="ElementTitle" Text="Поселения"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                    <igtbl:UltraWebGrid ID="UltraWebGridSettlements" runat="server" Height="200px" Width="315px" SkinID="UltraWebGrid">
                                        <Bands>
                                            <igtbl:UltraGridBand>
                                                <AddNewRow View="NotSet" Visible="NotSet">
                                                </AddNewRow>
                                            </igtbl:UltraGridBand>
                                        </Bands>
                                        <DisplayLayout AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" HeaderClickActionDefault="SortMulti"
                                            Name="UltraWebGrid" RowHeightDefault="20px" RowSelectorsDefault="No" ScrollBar="Never" SelectTypeRowDefault="Extended"
                                            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00"
                                            BorderCollapseDefault="Separate" GridLinesDefault="None">
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
                                            <RowStyleDefault BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px" Font-Bold="False"
                                                Font-Names="Arial" Font-Size="16px" ForeColor="White" HorizontalAlign="Right" Wrap="True">
                                                <Padding Bottom="5px" Left="3px" Right="3px" Top="5px" />
                                            </RowStyleDefault>
                                            <FilterOptionsDefault>
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
                                            <HeaderStyleDefault BackColor="#323232" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px" Font-Bold="True"
                                                Font-Names="Arial" Font-Size="18px" ForeColor="White" HorizontalAlign="Center" Wrap="True">
                                                <BorderDetails WidthLeft="1px" WidthTop="1px" />
                                            </HeaderStyleDefault>
                                            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                                            </EditCellStyleDefault>
                                            <FrameStyle BackColor="Black" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif"
                                                Font-Size="8.25pt" Height="200px" Width="315px">
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
                                    <div runat="server" id="settlementsDiv" style="margin-left: 5px">
                                        <asp:Label ID="Label36" runat="server" SkinID="InformationText" Text="Просроченная кредиторская задолженность&nbsp;"></asp:Label>
                                        <asp:Label ID="settlementDebts" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                        <asp:Label ID="Label28" runat="server" SkinID="InformationText" Text="&nbsp;тыс.руб. "></asp:Label></div>
                                </td>
                                <td>
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
