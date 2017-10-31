<%@ Page Language="C#" AutoEventWireup="true" Codebehind="iPad_0001_0001.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.iPad_0001_0001" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader" TagPrefix="uc1" %>
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
                    <td align="left" valign="top">
                        <uc1:iPadElementHeader ID="IncomesHeader" runat="server" Text="Доходы" Width="100%" />
                        <div runat="server" id="HeraldImageContainer" style="float: left; margin-left: 32px">
                        </div>
                        <table style="border-collapse: collapse">
                            <tr>
                                <td>
                                    <asp:Label ID="lbIncomesPlanTitle" runat="server" SkinID="InformationText" Text="План&nbsp;"></asp:Label></td>
                                <td align="right">
                                    <asp:Label ID="lbIncomesPlanValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></td>
                                <td>
                                    <asp:Label ID="lbIncomesPlanMeasures" runat="server" SkinID="InformationText" Text="&nbsp;млн.руб. "></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lbIncomesFactTitle" runat="server" SkinID="InformationText" Text="Факт&nbsp;"></asp:Label></td>
                                <td align="right">
                                    <asp:Label ID="lbIncomesFactValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></td>
                                <td>
                                    <asp:Label ID="lbIncomesFactMeasures" SkinID="InformationText" runat="server" Text="&nbsp;млн.руб."></asp:Label></td>
                            </tr>
                            <tr style="margin-top: -3px;">
                                <td>
                                    <asp:Label ID="lbIncomesExecutedTitle" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label></td>
                                <td align="right">
                                    <asp:Label ID="lbIncomesExecutedValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></td>
                                <td>
                                    <asp:Label ID="lbIncomesExecutedMeasures" SkinID="DigitsValueSmall" runat="server" Text="%"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div style="margin-top: -7px; vertical-align: bottom;">
                                        <asp:Label ID="lbIncomesRankFO" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label>
                                        <asp:Label ID="lbIncomesRankFOValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                        <asp:Label ID="lbIncomesRankRF" runat="server" SkinID="InformationText" Text="ранг РФ&nbsp;"></asp:Label>
                                        <asp:Label ID="lbIncomesRankRFValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></div>
                                </td>
                            </tr>
                        </table>
                        <div style="margin-left: 2px; margin-top: -12px">
                            <asp:PlaceHolder ID="GaugeIncomesPlaceHolder" runat="server"></asp:PlaceHolder>
                        </div>
                        <div style="margin-left: 49px; margin-top: -1px">
                            <asp:Label ID="lbPopulationTitle" runat="server" SkinID="InformationText" Text="Численность&nbsp;пост.&nbsp;населения&nbsp;"></asp:Label>
                            <asp:Label ID="lbPopulationValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                            <asp:Label SkinID="InformationText" ID="lbPopulationMeasures" runat="server" Text="&nbsp;тыс.чел."></asp:Label><br />
                            <div style="margin-top: -1px">
                                <asp:Label ID="lbIncomesAverageTitle" runat="server" SkinID="InformationText" Text="Среднедушевые&nbsp;доходы&nbsp;"></asp:Label>
                                <asp:Label ID="lbIncomesAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                <asp:Label SkinID="InformationText" ID="lbIncomesAverageMeasures" runat="server" Text="&nbsp;тыс.руб./чел."></asp:Label></div>
                            <div style="margin-left: 38px">
                                <asp:Label ID="lbIncomesRankFOAverage" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label>
                                <asp:Label ID="lbIncomesRankFOAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                <asp:Label ID="lbIncomesRankRFAverage" runat="server" SkinID="InformationText" Text="ранг РФ&nbsp;"></asp:Label>
                                <asp:Label ID="lbIncomesRankRFAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></div>
                        </div>
                    </td>
                    <td>
                    </td>
                    <td align="left" valign="top">
                        <uc1:iPadElementHeader ID="OutcomesHeader" runat="server" Text="Расходы" Width="340px" />
                        <table style="border-collapse: collapse; margin-left: 69px">
                            <tr>
                                <td>
                                    <asp:Label ID="lbOutcomesPlanTitle" runat="server" SkinID="InformationText" Text="План&nbsp;"></asp:Label></td>
                                <td align="right">
                                    <asp:Label ID="lbOutcomesPlanValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></td>
                                <td>
                                    <asp:Label SkinID="InformationText" ID="lbOutcomesPlanMeasures" runat="server" Text="&nbsp;млн.руб. "></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lbOutcomesFactTitle" runat="server" SkinID="InformationText" Text="Факт&nbsp;"></asp:Label></td>
                                <td align="right">
                                    <asp:Label ID="lbOutcomesFactValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></td>
                                <td>
                                    <asp:Label ID="lbOutcomesFactMeasures" SkinID="InformationText" runat="server" Text="&nbsp;млн.руб."></asp:Label></td>
                            </tr>
                            <tr style="margin-top: -3px;">
                                <td>
                                    <asp:Label ID="lbOutcomesExecutedTitle" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label></td>
                                <td align="right">
                                    <asp:Label ID="lbOutcomesExecutedValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></td>
                                <td>
                                    <asp:Label ID="lbOutcomesExecutedMeasures" SkinID="DigitsValueSmall" runat="server" Text="%"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <div style="margin-left: 70px; margin-top: -7px; vertical-align: bottom">
                            <asp:Label ID="lbOutcomesRankFO" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label>
                            <asp:Label ID="lbOutcomesRankFOValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                            <asp:Label ID="lbOutcomesRankRF" runat="server" SkinID="InformationText" Text="ранг РФ&nbsp;"></asp:Label>
                            <asp:Label ID="lbOutcomesRankRFValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></div>
                        <div style="margin-left: 70px; margin-top: -10px">
                            <asp:PlaceHolder ID="GaugeOutcomesPlaceHolder" runat="server"></asp:PlaceHolder>
                        </div>
                        <div style="margin-left: 32px; margin-top: -1px">
                            <asp:Label ID="lbOutcomesAverageTitle" runat="server" SkinID="InformationText" Text="Бюджетные расходы на душу"></asp:Label>
                            <div style="margin-top: -1px">
                                <asp:Label ID="Label4" runat="server" SkinID="InformationText" Text="населения&nbsp;"></asp:Label>
                                <asp:Label ID="lbOutcomesAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                <asp:Label SkinID="InformationText" ID="lbOutcomesAverageMeasures" runat="server" Text="&nbsp;тыс.&nbsp;руб./чел."></asp:Label>
                                <div style="margin-left: 39px">
                                    <asp:Label ID="lbOutcomesRankFOAverage" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label>
                                    <asp:Label ID="lbOutcomesRankFOAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                    <asp:Label ID="lbOutcomesRankRFAverage" runat="server" SkinID="InformationText" Text="ранг РФ&nbsp;"></asp:Label>
                                    <asp:Label ID="lbOutcomesRankRFAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label></div>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top"><div style="margin-top: -12px">
                        <uc1:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Основные показатели" Width="100%" />
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
                        </igtbl:UltraWebGrid></div>
                    </td>
                    <td>
                    </td>
                    <td align="left" valign="top"><div style="margin-top: -12px">
                        <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Финансовая помощь"  Width="100%" />
                        <div style="padding-left: 13px" runat="server" id="mbtRankIng">
                            <asp:Label ID="lbRankCaption" runat="server" Text="Label"></asp:Label><asp:Label ID="Rank" runat="server"
                                Text="Label"></asp:Label>&nbsp;<asp:Image ID="imgMbt" runat="server" /><asp:Label ID="lbRankDescription"
                                    runat="server" Text="Label"></asp:Label></div>
                        <igchart:UltraChart ID="UltraChartFonds" runat="server" BackgroundImageFileName=""  
                             EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Height="315px" SkinID="UltraWebColumnChart" Version="8.2" Width="308px">
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
                                        Orientation="Horizontal" VerticalAlign="Center">
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
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                                        VerticalAlign="Center">
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
                            <DeploymentScenario FilePath="../../../TemporaryImages/" ImageURL="../../../TemporaryImages/Chart_mfrf01_05_#SEQNUM(100).png" />
                        </igchart:UltraChart>
                        <uc1:iPadElementHeader ID="IPadElementHeader5" runat="server" Text="Государственный долг" Width="100%" />
                        <asp:Label ID="crimeText" runat="server" SkinID="InformationText" Text="Государственный долг отсутствует"
                            Visible="false"></asp:Label>
                        <div id="debtsDiv" runat="server" style="padding-left: 13px">
                            <table style="border-collapse: collapse">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" SkinID="InformationText" Text="Объем долга"></asp:Label><br />
                                        <asp:Label ID="gosDebt" runat="server" SkinID="DigitsValueSmall" Text=""></asp:Label>&nbsp;
                                        <asp:Label ID="Label3" runat="server" SkinID="InformationText" Text="млн.руб."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 23px">
                                        <div style="margin-top: -7px">
                                            <asp:Label ID="lbGosDebtRankFO" runat="server" SkinID="InformationText" Text=""></asp:Label>
                                            <asp:Label ID="lbGosDebtRankFOValue" runat="server" SkinID="DigitsValueSmall" Text=""></asp:Label>&nbsp;
                                            <asp:Label ID="lbGosDebtRankRF" runat="server" SkinID="InformationText" Text="ранг&nbsp;РФ&nbsp;"></asp:Label>
                                            <asp:Label ID="lbGosDebtRankRFValue" runat="server" SkinID="DigitsValueSmall" Text=""></asp:Label>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 23px">
                                        <div runat="server" id="UltraChartcrimeDiv">
                                            <asp:Label ID="crime" runat="server" SkinID="InformationText" Text="Долговая нагрузка "></asp:Label></div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-top: 10px">
                                        <asp:Label ID="Label2" runat="server" SkinID="InformationText" Text="На душу населения "></asp:Label><br />
                                        <asp:Label ID="gosDebtAvg" runat="server" SkinID="DigitsValueSmall" Text=""></asp:Label>&nbsp;
                                        <asp:Label ID="Label8" runat="server" SkinID="InformationText" Text="руб./чел"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 23px">
                                        <div style="margin-top: -7px">
                                            <asp:Label ID="lbGosDebtAvgRankFO" runat="server" SkinID="InformationText" Text=""></asp:Label>
                                            <asp:Label ID="lbGosDebtAvgRankFOValue" runat="server" SkinID="DigitsValueSmall" Text=""></asp:Label>&nbsp;
                                            <asp:Label ID="lbGosDebtAvgRankRF" runat="server" SkinID="InformationText" Text="ранг&nbsp;РФ&nbsp;"></asp:Label>
                                            <asp:Label ID="lbGosDebtAvgRankRFValue" runat="server" SkinID="DigitsValueSmall" Text=""></asp:Label></div>
                                    </td>
                                </tr>
                            </table>
                        </div></div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
