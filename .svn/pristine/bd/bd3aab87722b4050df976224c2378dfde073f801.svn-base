<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0002_0001.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0002_0001" %>

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
    <div style="position: absolute; width: 768px; height: 950px; background-color: black;
        top: 0px; left: 0px; z-index: 2;">
        <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black;
            top: 0px; left: 0px">
            <tr>
                <td align="center">                    
                    <asp:Label ID="RegionLabel" runat="server" SkinID="ImportantText" Text="Label"></asp:Label>
                </td>
            </tr>
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
                                            background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px;
                                            padding-left: 3px; height: 36px; text-align: center; vertical-align: middle;">
                                            <asp:Label ID="chart3ElementCaption" runat="server" CssClass="ElementTitle" Text="Доходы"></asp:Label>
                                        </td>
                                        <td style="background: Black url(../../../images/iPadContainer/headerright.gif);
                                            background-repeat: no-repeat; width: 1px; height: 36px; background-color: Black;">
                                        </td>
                                    </tr>
                                </table>
                                <table style="border-collapse: collapse;">
                                    <tr>
                                        <td style="overflow: visible; background-color: Black">
                                            <table style="border-collapse: collapse; margin-left: 30px">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbIncomesPlanTitle" runat="server" SkinID="InformationText" Text="План&nbsp;"></asp:Label>
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lbIncomesPlanValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbIncomesPlanMeasures" runat="server" SkinID="InformationText" Text="&nbsp;тыс.руб."></asp:Label>
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
                                                        <asp:Label ID="lbIncomesFactMeasures" SkinID="InformationText" runat="server" Text="&nbsp;тыс.руб."></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr style="margin-top: -3px;">
                                                    <td>
                                                        <asp:Label ID="lbIncomesExecutedTitle" runat="server" SkinID="InformationText" Text="Исполнено&nbsp;"></asp:Label>
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lbIncomesExecutedValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbIncomesExecutedMeasures" SkinID="DigitsValueSmall" runat="server"
                                                            Text="%"></asp:Label><br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <asp:Label ID="lbIncomesRankFO" runat="server" SkinID="InformationText" Text="ранг по району&nbsp;"></asp:Label>
                                                        <asp:Label ID="lbIncomesRankFOValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>&nbsp;&nbsp;&nbsp;
                                                        <asp:Label ID="Label3" runat="server" SkinID="InformationText" Text="ранг по области&nbsp;"></asp:Label>
                                                        <asp:Label ID="lbIncomesRankAllValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div style="margin-left: -17px; margin-top: -10px">
                                                <asp:PlaceHolder ID="GaugeIncomesPlaceHolder" runat="server"></asp:PlaceHolder>
                                            </div>
                                            <div style="margin-left: 15px; margin-top: -1px">
                                                <div style="margin-top: -1px">
                                                    <asp:Label ID="lbPopulation" runat="server" SkinID="InformationText" Text=""></asp:Label><br />
                                                    <asp:Label ID="lbIncomesAverageTitle" runat="server" SkinID="InformationText" Text="Бюдж.&nbsp;дох.&nbsp;на&nbsp;душу&nbsp;населения&nbsp;"></asp:Label>
                                                    <asp:Label ID="lbIncomesAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                    <asp:Label SkinID="InformationText" ID="lbIncomesAverageMeasures" runat="server"
                                                        Text="&nbsp;руб./чел."></asp:Label></div>
                                                <div style="margin-left: 15px;">
                                                    <asp:Label ID="lbIncomesRankFOAverage" runat="server" SkinID="InformationText" Text="ранг по району&nbsp;"></asp:Label>
                                                    <asp:Label ID="lbIncomesRankFOAverageValue" runat="server" SkinID="DigitsValueSmall"
                                                        Text="Label"></asp:Label>&nbsp;&nbsp;
                                                    <asp:Label ID="Label8" runat="server" SkinID="InformationText" Text="ранг по области&nbsp;"></asp:Label>
                                                    <asp:Label ID="lbIncomesRankAllAverageValue" runat="server" SkinID="DigitsValueSmall"
                                                        Text="Label"></asp:Label></div>
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
                                            background-color: black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px;
                                            padding-left: 3px; height: 36px; text-align: center; vertical-align: middle;">
                                            <asp:Label ID="Label1" runat="server" CssClass="ElementTitle" Text="Расходы"></asp:Label>
                                        </td>
                                        <td style="background: white url(../../../images/iPadContainer/headerright.gif);
                                            background-repeat: no-repeat; width: 1px; height: 36px; background-color: Black;">
                                        </td>
                                    </tr>
                                </table>
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
                                                        <asp:Label SkinID="InformationText" ID="lbOutcomesPlanMeasures" runat="server" Text="&nbsp;тыс.руб. "></asp:Label>
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
                                                        <asp:Label ID="lbOutcomesFactMeasures" SkinID="InformationText" runat="server" Text="&nbsp;тыс.руб."></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr style="margin-top: -3px;">
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
                                                <tr>
                                                    <td colspan="3">
                                                        <asp:Label ID="lbOutcomesRankFO" runat="server" SkinID="InformationText" Text="ранг по району&nbsp;"></asp:Label>
                                                        <asp:Label ID="lbOutcomesRankFOValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>&nbsp;&nbsp;&nbsp;
                                                        <asp:Label ID="Label11" runat="server" SkinID="InformationText" Text="ранг по области&nbsp;"></asp:Label>
                                                        <asp:Label ID="lbOutcomesRankAllValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div style="margin-left: 13px; margin-top: -15px">
                                                <asp:PlaceHolder ID="GaugeOutcomesPlaceHolder" runat="server"></asp:PlaceHolder>
                                            </div>
                                            <div style="margin-left: 59px; margin-top: -1px">
                                                <div style="margin-top: -1px">
                                                    <asp:Label ID="lbOutcomesAverageTitle" runat="server" SkinID="InformationText" Text="Бюджетные расходы на душу населения&nbsp;"></asp:Label>
                                                    <asp:Label ID="lbOutcomesAverageValue" runat="server" SkinID="DigitsValueSmall" Text="Label"></asp:Label>
                                                    <asp:Label SkinID="InformationText" ID="lbOutcomesAverageMeasures" runat="server"
                                                        Text="&nbsp;руб./чел."></asp:Label></div>
                                                <div style="margin-left: -28px;">
                                                    <asp:Label ID="lbOutcomesRankFOAverage" runat="server" SkinID="InformationText" Text="ранг по району&nbsp;"></asp:Label>
                                                    <asp:Label ID="lbOutcomesRankFOAverageValue" runat="server" SkinID="DigitsValueSmall"
                                                        Text="Label"></asp:Label>&nbsp;&nbsp;&nbsp;
                                                    <asp:Label ID="Label9" runat="server" SkinID="InformationText" Text="ранг по области&nbsp;"></asp:Label>
                                                    <asp:Label ID="lbOutcomesRankAllAverageValue" runat="server" SkinID="DigitsValueSmall"
                                                        Text="Label"></asp:Label>
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
                                <table style="border-collapse: collapse; background-color: Black; width: 100%; height: 100%;
                                    margin-right: 10px; margin-top: -5px;">
                                    <tr>
                                        <td>
                                            <igtbl:UltraWebGrid ID="UltraWebGridBudget" runat="server" Height="200px" Width="509px"
                                                OnDataBinding="UltraWebGridBudget_DataBinding" OnInitializeLayout="UltraWebGridBudget_InitializeLayout"
                                                SkinID="UltraWebGrid" OnInitializeRow="UltraWebGridBudget_InitializeRow">
                                            </igtbl:UltraWebGrid>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                            </td>
                            <td valign="top">
                                <table style="border-collapse: collapse; background-color: Black; width: 100%; height: 100%;
                                    margin-right: 10px; margin-top: -5px;">
                                    <tr>
                                        <td>
                                            <igtbl:UltraWebGrid ID="UltraWebGridOutcomes" runat="server" Height="200px" Width="509px"
                                                OnDataBinding="UltraWebGridBudget_DataBinding" OnInitializeLayout="UltraWebGridBudget_InitializeLayout"
                                                SkinID="UltraWebGrid" OnInitializeRow="UltraWebGridBudget_InitializeRow">
                                            </igtbl:UltraWebGrid>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <table style="border-collapse: collapse; width: 760px;">
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
                                <asp:Label ID="Label2" runat="server" CssClass="ElementTitle" Text="Дефицит/профицит бюджета"></asp:Label>
                            </td>
                            <td style="background: white url(../../../images/iPadContainer/headerright.gif);
                                background-repeat: no-repeat; width: 1px; height: 36px; background-color: Black;">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="left" class="TableFont">
                    <table style="width: 763px">
                        <tr>
                            <td valign="top">
                                <igchart:UltraChart ID="UltraChart12" runat="server" SkinID="UltraWebColumnChart">
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_12#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                            <td>
                                <asp:Label ID="lbDeficite" runat="server" CssClass="InformationText" Text=""></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table style="border-collapse: collapse; width: 760px;">
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
                                <asp:Label ID="Label5" runat="server" CssClass="ElementTitle" Text="Недоимка"></asp:Label>
                            </td>
                            <td style="background: white url(../../../images/iPadContainer/headerright.gif);
                                background-repeat: no-repeat; width: 1px; height: 36px; background-color: Black;">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <igtbl:UltraWebGrid ID="UltraWebGridArrearAll" runat="server" Height="200px" OnDataBinding="UltraWebGridArrearAll_DataBinding"
                        OnInitializeLayout="Grid_InitializeLayout" Width="315px" SkinID="UltraWebGrid"
                        OnInitializeRow="Grid_InitializeRow">
                    </igtbl:UltraWebGrid>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
