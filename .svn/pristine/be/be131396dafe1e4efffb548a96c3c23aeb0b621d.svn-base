<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FO_0135_0005.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0135_0005" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc1" %>
<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader" TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">      
    <touchelementbounds src="TouchElementBounds.xml"></touchelementbounds> 
        <div style="position: absolute; width: 767px; height: 950px; top: 0px; left: 0px; overflow: hidden; z-index: 2;">
            <table style="position: absolute; width: 760px; height: 900px; background-color: Black; top: 0px; left: 0px;
                overflow: hidden;">
                <tr>
                    <td valign="top">
                        <table style="margin-top: 0px;">
                            <tr>
                                <td valign="top">
                                    <asp:Label ID="lbHeader" runat="server" CssClass="InformationText" Text="Исполнение кассового плана"></asp:Label>
                                    <asp:Label ID="lbQuater" runat="server" CssClass="InformationText"></asp:Label>
                                    <asp:Label ID="lbDate" runat="server" CssClass="InformationText"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="width: 760px; text-align: left; background-color: Black" align="left" valign="top">
                                    <table style="margin-top: 0px; margin-left: -3px;">
                                        <tr>
                                            <td>
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
                                                            <asp:Label ID="Label16" runat="server" CssClass="ElementTitle" Text="Доходы бюджета"></asp:Label>&nbsp;<asp:Label
                                                                ID="lbExecutedIncomes" runat="server" Text="Label" CssClass="ElementTitle"></asp:Label>
                                                        </td>
                                                        <td style="background: Black url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                            width: 1px; height: 36px; background-color: Black;">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="3" valign="top" style="padding-top: 10px;">
                                                            <div style="float: left; margin-right: 20px">
                                                                <asp:Image ID="Image1" runat="server" /></div>
                                                            <asp:PlaceHolder ID="PlaceHolderIncomes" runat="server"></asp:PlaceHolder>
                                                            <div style="margin-left: -10px; margin-bottom: 10px"><asp:Label ID="Label7" runat="server" CssClass="InformationText" Text="план" Style="padding-left: 70px;"></asp:Label>&nbsp;
                                                            <asp:Label ID="lbPlanIncomes" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                                                            <asp:Label ID="Label9" runat="server" CssClass="InformationText" Text="факт"></asp:Label>&nbsp;
                                                            <asp:Label ID="lbFactIncomes" runat="server" CssClass="DigitsValueLarge"></asp:Label></div>
                                                            <asp:Table ID="incomes" runat="server" BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px"
                                                                GridLines="Both" Width="375px">
                                                            </asp:Table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td rowspan="2" valign="top">
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
                                                            <asp:Label ID="Label17" runat="server" CssClass="ElementTitle" Text="Расходы бюджета"></asp:Label>&nbsp;<asp:Label
                                                                ID="lbExecutedOucomes" runat="server" Text="Label" CssClass="ElementTitle"></asp:Label>
                                                        </td>
                                                        <td style="background: Black url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                            width: 1px; height: 36px; background-color: Black;">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-top: 10px;" valign="top" colspan="3">
                                                            <div style="float: left; margin-top: 10px; margin-right: 20px">
                                                                <asp:Image ID="Image2" runat="server" /></div>
                                                            <asp:PlaceHolder ID="PlaceHolderOutcomes" runat="server"></asp:PlaceHolder>
                                                           <div style="margin-left: -10px; margin-bottom: 10px"> <asp:Label ID="Label5" runat="server" CssClass="InformationText" Text="план" Style="padding-left: 70px;"></asp:Label>&nbsp;
                                                            <asp:Label ID="lbPlanOutcomes" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                                                            <asp:Label ID="Label10" runat="server" CssClass="InformationText" Text="факт"></asp:Label>&nbsp;
                                                            <asp:Label ID="lbFactOutcomes" runat="server" CssClass="DigitsValueLarge"></asp:Label></div>
                                                            <asp:Table ID="Outcomes" runat="server" BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px"
                                                                GridLines="Both" Width="375px">
                                                            </asp:Table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <table style="border-collapse: collapse; background-color: Black; width: 375px; height: 100%; margin-top: 30px; display: none">
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
                                                            <asp:Label ID="chart3ElementCaption" runat="server" CssClass="ElementTitle" Text="Остаток средств"></asp:Label>
                                                        </td>
                                                        <td style="background: Black url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                                                            width: 1px; height: 36px; background-color: Black;">
                                                        </td>
                                                    </tr>                                                
                                                    <tr runat="server" id="restEndTR">
                                                        <td runat="server" id="restEndTD" valign="top" colspan="3">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <div style="float: left; margin-top: 5px; margin-right: 20px">
                                                                            <asp:Image ID="Image3" runat="server" /></div>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="Label2" runat="server" CssClass="InformationText" Text="Остаток средств<br/>"></asp:Label>
                                                                        <asp:Label ID="Label3" runat="server" CssClass="InformationText" Text="план&nbsp;"></asp:Label>
                                                                        <asp:Label ID="lbRestEndPlan" runat="server" CssClass="DigitsValueLarge"></asp:Label>
                                                                        <asp:Label ID="Label4" runat="server" CssClass="InformationText" Text="факт&nbsp;"></asp:Label>
                                                                        <asp:Label ID="lbRestEndFact" runat="server" CssClass="DigitsValueLarge"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
               <tr>
                    <td style="width: 762px; text-align: left; background-color: Black; padding-left: 4px" align="left" valign="top">                        
                       <uc3:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Расходы по главным распорядителям"
                            Width="757px"  MultitouchReport="FO_0135_0007" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 760px; padding-left: 7px" valign="top">
                        <div style="margin-top: 0px">
                            <uc1:TagCloud ID="TagCloud1" runat="server" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
