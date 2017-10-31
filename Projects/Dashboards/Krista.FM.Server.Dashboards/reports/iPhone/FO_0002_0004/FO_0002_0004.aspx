<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0002_0004.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0002_0004" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 768px; height: 950px; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: hidden;">
        <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black;
            top: 0px; left: 0px">
            <tr>
                <td>
                    <table style="border-collapse: collapse;">
                        <tr>
                            <td align="left" valign="top">
                                <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Доходы" Width="100%" />
                                <table style="border-collapse: collapse;">
                                    <tr>
                                        <td>
                                            <table style="border-collapse: collapse; margin-left: 80px">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbIncomesPlanTitle" runat="server" SkinID="InformationText" Text="План&nbsp;"></asp:Label>
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lbIncomesPlanValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbIncomesPlanMeasures" runat="server" SkinID="InformationText" Text="&nbsp;млн.руб."></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbIncomesFactTitle" runat="server" SkinID="InformationText" Text="Факт&nbsp;"></asp:Label>
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lbIncomesFactValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbIncomesFactMeasures" SkinID="InformationText" runat="server" Text="&nbsp;млн.руб."></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbIncomesExecutedTitle" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label>
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lbIncomesExecutedValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbIncomesExecutedMeasures" SkinID="DigitsValueSmall" runat="server"
                                                            Text="%"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div style="margin-left: 20px;">
                                                <asp:PlaceHolder ID="GaugeIncomesPlaceHolder" runat="server"></asp:PlaceHolder>
                                            </div>
                                            <div style="margin-left: 15px;">
                                                <asp:Label ID="lbPopulation" runat="server" SkinID="InformationText" Text=""></asp:Label><br />
                                                <asp:Label ID="lbIncomesAverageTitle" runat="server" SkinID="InformationText" Text="Бюдж. доходы на душу населения"></asp:Label>
                                                <asp:Label ID="lbIncomesAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                <asp:Label SkinID="InformationText" ID="lbIncomesAverageMeasures" runat="server"
                                                    Text="&nbsp;руб./чел."></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                            </td>
                            <td style="text-align: left; background-color: Black" align="left" valign="top">
                                <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Расходы" Width="100%" />
                                <table style="border-collapse: collapse;">
                                    <tr>
                                        <td style="overflow: visible; background-color: Black">
                                            <table style="border-collapse: collapse; margin-left: 30px">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbOutcomesPlanTitle" runat="server" SkinID="InformationText" Text="План&nbsp;"></asp:Label>
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lbOutcomesPlanValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label SkinID="InformationText" ID="lbOutcomesPlanMeasures" runat="server" Text="&nbsp;млн.руб. "></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbOutcomesFactTitle" runat="server" SkinID="InformationText" Text="Факт&nbsp;"></asp:Label>
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lbOutcomesFactValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbOutcomesFactMeasures" SkinID="InformationText" runat="server" Text="&nbsp;млн.руб."></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbOutcomesExecutedTitle" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label>
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lbOutcomesExecutedValue" runat="server" SkinID="DigitsValueSmall"
                                                            Text="Label"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbOutcomesExecutedMeasures" SkinID="DigitsValueSmall" runat="server"
                                                            Text="%"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div style="margin-left: 2px;">
                                                <asp:PlaceHolder ID="GaugeOutcomesPlaceHolder" runat="server"></asp:PlaceHolder>
                                            </div>
                                            <div style="margin-left: 59px;">
                                                <asp:Label ID="lbOutcomesAverageTitle" runat="server" SkinID="InformationText" Text="Бюджетные расходы на душу населения&nbsp;"></asp:Label>
                                                <asp:Label ID="lbOutcomesAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                <asp:Label SkinID="InformationText" ID="lbOutcomesAverageMeasures" runat="server"
                                                    Text="&nbsp;руб./чел."></asp:Label>
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
                                <uc1:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Основные показатели"
                                    Width="100%" />
                                <igtbl:UltraWebGrid ID="UltraWebGridBudget" runat="server" Height="200px" Width="509px"
                                    OnDataBinding="UltraWebGridBudget_DataBinding" OnInitializeLayout="UltraWebGridBudget_InitializeLayout"
                                    SkinID="UltraWebGrid" OnInitializeRow="UltraWebGridBudget_InitializeRow">
                                    <Bands>
                                        <igtbl:UltraGridBand>
                                            <AddNewRow View="NotSet" Visible="NotSet">
                                            </AddNewRow>
                                        </igtbl:UltraGridBand>
                                    </Bands>
                                    <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                        StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                        StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid"
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
                                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                            </BorderDetails>
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
                                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                            </BorderDetails>
                                        </HeaderStyleDefault>
                                        <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                        </EditCellStyleDefault>
                                        <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                            Font-Names="Microsoft Sans Serif" BackColor="Window" Width="509px" Height="200px">
                                        </FrameStyle>
                                        <Pager MinimumPagesForDisplay="2">
                                            <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                                </BorderDetails>
                                            </PagerStyle>
                                        </Pager>
                                        <AddNewBox Hidden="False">
                                            <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                                </BorderDetails>
                                            </BoxStyle>
                                        </AddNewBox>
                                    </DisplayLayout>
                                </igtbl:UltraWebGrid>
                            </td>
                            <td>
                            </td>
                            <td align="left" valign="top">
                                <div runat="server" id="FinHelp">
                                    <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Финансовая помощь"
                                        Width="100%" />
                                    <div style="padding-left: 13px" runat="server" id="mbtRankIng">
                                        <asp:Label ID="lbRankCaption" runat="server" Text="Label"></asp:Label><asp:Label
                                            ID="Rank" runat="server" Text="Label"></asp:Label><br/><asp:Label
                                                ID="lbRankDescription" runat="server" Text="Label" SkinID="InformationText"></asp:Label></div>
                                    <igchart:UltraChart ID="UltraChartFonds" runat="server" SkinID="UltraWebColumnChart"
                                        Version="8.2" Width="308px">
                                        <DeploymentScenario FilePath="../../../TemporaryImages/" ImageURL="../../../TemporaryImages/Chart_mfrf01_05_#SEQNUM(100).png" />
                                    </igchart:UltraChart>
                                </div>
                                <div runat="server" id="CreditDebtsDiv" style="margin-top: 5px">
                                    <table style="border-collapse: collapse; background-color: Black; width: 100%; height: 100%;
                                        margin-top: -5px">
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
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px;
                                                padding-left: 3px; height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label6" runat="server" CssClass="ElementTitle" Text="Просроченная кредиторская задолженность"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif);
                                                background-repeat: no-repeat; width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 315px; background-color: black; padding-left: 6px" valign="top"
                                                colspan="3">
                                                <asp:Label ID="creditDebts" runat="server" SkinID="InformationText" Text="Количество нарушений требований бюджетного кодекса РФ"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <table style="border-collapse: collapse; background-color: Black; width: 100%; height: 100%;
                                        margin-top: 1px">
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
                                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px;
                                                padding-left: 3px; height: 36px; text-align: center; vertical-align: middle;">
                                                <asp:Label ID="Label11" runat="server" CssClass="ElementTitle" Text="Недоимка"></asp:Label>
                                            </td>
                                            <td style="background: white url(../../../images/iPadContainer/headerright.gif);
                                                background-repeat: no-repeat; width: 1px; height: 36px; background-color: Black;">
                                            </td>
                                        </tr>
                                    </table>
                                    <table>
                                        <tr>
                                            <td style="width: 315px; background-color: black;" valign="top">
                                                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="200px" OnDataBinding="UltraWebGrid_DataBinding"
                                                    OnInitializeLayout="Grid_InitializeLayout" Width="315px" SkinID="UltraWebGrid"
                                                    OnInitializeRow="Grid_InitializeRow">
                                                    <Bands>
                                                        <igtbl:UltraGridBand>
                                                            <AddNewRow View="NotSet" Visible="NotSet">
                                                            </AddNewRow>
                                                        </igtbl:UltraGridBand>
                                                    </Bands>
                                                    <DisplayLayout AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" HeaderClickActionDefault="SortMulti"
                                                        Name="UltraWebGrid" RowHeightDefault="20px" RowSelectorsDefault="No" ScrollBar="Never"
                                                        SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                                                        TableLayout="Fixed" Version="4.00" BorderCollapseDefault="Separate" GridLinesDefault="None">
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
                                                        <RowStyleDefault BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px"
                                                            Font-Bold="False" Font-Names="Arial" Font-Size="16px" ForeColor="White" HorizontalAlign="Right"
                                                            Wrap="True">
                                                            <Padding Bottom="5px" Left="3px" Right="3px" Top="5px" />
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
                                                        <HeaderStyleDefault BackColor="#323232" BorderColor="#323232" BorderStyle="Solid"
                                                            BorderWidth="1px" Font-Bold="True" Font-Names="Arial" Font-Size="18px" ForeColor="White"
                                                            HorizontalAlign="Center" Wrap="True">
                                                            <BorderDetails WidthLeft="1px" WidthTop="1px" />
                                                        </HeaderStyleDefault>
                                                        <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                                                        </EditCellStyleDefault>
                                                        <FrameStyle BackColor="Black" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
                                                            Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="200px" Width="315px">
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
                                </div>
                                <uc1:iPadElementHeader ID="IPadElementHeader5" runat="server" Text="Государственный долг"
                                    Width="100%" />
                                <asp:Label ID="crimeText" runat="server" SkinID="InformationText" Text="Государственный долг отсутствует"
                                    Visible="false"></asp:Label>
                                <div id="debtsDiv" runat="server" style="padding-left: 13px">
                                    <table style="border-collapse: collapse">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label1" runat="server" SkinID="InformationText" Text="Объем долга"></asp:Label><br />
                                                <asp:Label ID="gosDebt" runat="server" SkinID="DigitsValueSmall" Text=""></asp:Label>&nbsp;
                                                <asp:Label ID="Label4" runat="server" SkinID="InformationText" Text="млн.руб."></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label3" runat="server" SkinID="InformationText" Text="Отношение долга к доходам (без безвозмездных поступлений)"></asp:Label><br />
                                                <asp:Label ID="Label5" runat="server" SkinID="DigitsValueSmall" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-top: 10px">
                                                <asp:Label ID="Label2" runat="server" SkinID="InformationText" Text="На душу населения "></asp:Label><br />
                                                <asp:Label ID="gosDebtAvg" runat="server" SkinID="DigitsValueSmall" Text=""></asp:Label>&nbsp;
                                                <asp:Label ID="Label8" runat="server" SkinID="InformationText" Text="руб./чел"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label7" runat="server" SkinID="InformationText" Text="Расходы на обслуживание долга"></asp:Label>
                                                <asp:Label ID="Label9" runat="server" SkinID="DigitsValueSmall" Text=""></asp:Label>&nbsp;
                                                <asp:Label ID="Label10" runat="server" SkinID="InformationText" Text="млн.руб."></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
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
