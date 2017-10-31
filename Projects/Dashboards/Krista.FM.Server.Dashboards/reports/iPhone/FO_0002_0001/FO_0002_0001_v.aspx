<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0002_0001_v.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0002_0001_v" %>

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
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <div style="position: absolute; width: 768px; background-color: black; top: 0px;
        left: 0px; z-index: 2; overflow: hidden">
        <table style="width: 768px; background-color: Black; top: 0px; left: 0px; border-collapse: collapse">
            <tr>
                <td align="center">
                    <asp:Label ID="RegionLabel" runat="server" SkinID="ImportantText" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <table style="border-collapse: collapse; background-color: Black; width: 765px; height: 100%;
                        margin-right: 10px;">
                        <tr>
                            <td>
                                <table style="border-collapse: collapse; width: 763px;">
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
                                            <asp:Label ID="Label2" runat="server" CssClass="ElementTitle" Text="Бюджет поселения"></asp:Label>
                                        </td>
                                        <td style="background: white url(../../../images/iPadContainer/headerright.gif);
                                            background-repeat: no-repeat; width: 1px; height: 36px; background-color: Black;">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="overflow: visible; background-color: Black;">
                                <igtbl:UltraWebGrid ID="UltraWebGridBudget" runat="server" Height="200px" Width="509px"
                                    OnDataBinding="UltraWebGridBudget_DataBinding" OnInitializeLayout="UltraWebGridBudget_InitializeLayout"
                                    SkinID="UltraWebGrid" OnInitializeRow="UltraWebGridBudget_InitializeRow">
                                </igtbl:UltraWebGrid>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table style="border-collapse: collapse; width: 763px;">
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
                                            <asp:Label ID="Label3" runat="server" CssClass="ElementTitle" Text="Дефицит/профицит бюджета"></asp:Label>
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
                                <table style="border-collapse: collapse; width: 763px;">
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
                                </igtbl:UltraWebGrid><br />
                                <asp:Label ID="Label6" runat="server" CssClass="TableFont" Text="5 доходных источников с наибольшей недоимкой"></asp:Label>
                                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="200px" OnDataBinding="UltraWebGrid_DataBinding"
                                    OnInitializeLayout="Grid_InitializeLayout" Width="315px" SkinID="UltraWebGrid"
                                    OnInitializeRow="Grid_InitializeRow">
                                </igtbl:UltraWebGrid><br />
                                <asp:Label ID="Label1" runat="server" CssClass="TableFont" Text="5 ОКВЭД с наибольшей недоимкой"></asp:Label>
                                <igtbl:UltraWebGrid ID="UltraWebGridOkved" runat="server" Height="200px" OnDataBinding="UltraWebGridOkved_DataBinding"
                                    OnInitializeLayout="Grid_InitializeLayout" Width="315px" SkinID="UltraWebGrid"
                                    OnInitializeRow="Grid_InitializeRow">
                                </igtbl:UltraWebGrid>
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
