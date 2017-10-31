<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0735_0005.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0735_0005" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc1" %>
<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 767px; height: 950px; top: 0px; left: 0px;
        overflow: hidden; z-index: 2;">
        <table style="margin-left: -7px">
            <tr>
                <td>
                    <table style="margin-top: -6px;">
                        <tr>
                            <td valign="top">
                                <asp:Label ID="lbHeader" runat="server" CssClass="InformationText" Text="Исполнение кассового плана"></asp:Label>
                                <asp:Label ID="lbQuater" runat="server" CssClass="InformationText"></asp:Label>
                                <asp:Label ID="lbDate" runat="server" CssClass="InformationText"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 760px; text-align: left; background-color: Black" align="left"
                                valign="top">
                                <table style="margin-top: -6px; margin-left: -3px;">
                                    <tr>
                                        <td>
                                            <uc3:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Объем производства"
                                                Width="380px" />
                                            <div style="float: left; margin-right: 20px">
                                                <asp:Image ID="Image1" runat="server" /></div>
                                            <asp:PlaceHolder ID="PlaceHolderIncomes" runat="server"></asp:PlaceHolder>
                                            <asp:Label ID="Label7" runat="server" CssClass="InformationText" Text="план" Style="padding-left: 70px;"></asp:Label>&nbsp;
                                            <asp:Label ID="lbPlanIncomes" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                                            <asp:Label ID="Label9" runat="server" CssClass="InformationText" Text="факт"></asp:Label>&nbsp;
                                            <asp:Label ID="lbFactIncomes" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                                            <asp:Table ID="incomes" runat="server" BackColor="Black" BorderColor="#323232" BorderStyle="Solid"
                                                BorderWidth="1px" GridLines="Both" Width="380px">
                                            </asp:Table>
                                        </td>
                                        <td rowspan="2" valign="top">
                                            <uc3:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Объем производства"
                                                Width="380px" />
                                            <div style="float: left; margin-top: 10px; margin-right: 20px">
                                                <asp:Image ID="Image2" runat="server" /></div>
                                            <asp:PlaceHolder ID="PlaceHolderOutcomes" runat="server"></asp:PlaceHolder>
                                            <asp:Label ID="Label5" runat="server" CssClass="InformationText" Text="план" Style="padding-left: 70px;"></asp:Label>&nbsp;
                                            <asp:Label ID="lbPlanOutcomes" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                                            <asp:Label ID="Label10" runat="server" CssClass="InformationText" Text="факт"></asp:Label>&nbsp;
                                            <asp:Label ID="lbFactOutcomes" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                                            <asp:Table ID="Outcomes" runat="server" BackColor="Black" BorderColor="#323232" BorderStyle="Solid"
                                                BorderWidth="1px" GridLines="Both" Width="380px">
                                            </asp:Table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top" style="padding-top: 0px">
                                            <uc3:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Остаток средств"
                                                Width="380px" />
                                            <table style="width: 370px">
                                                <tr>
                                                    <td valign="top">
                                                        <asp:Label ID="Label1" runat="server" CssClass="InformationText" Text="Остаток средств на начало года&nbsp;"></asp:Label>
                                                        <asp:Label ID="lbRestStartFact" runat="server" CssClass="DigitsValueLarge"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr runat="server" id="restEndTR">
                                                    <td runat="server" id="restEndTD" valign="top">
                                                        <div style="float: left; margin-top: 5px; margin-right: 20px; margin-bottom: 20px">
                                                            <asp:Image ID="Image3" runat="server" /></div>
                                                        <asp:Label ID="Label2" runat="server" CssClass="InformationText" Text="Остаток средств на дату<br/>"></asp:Label>
                                                        <asp:Label ID="Label3" runat="server" CssClass="InformationText" Text="план&nbsp;"></asp:Label>
                                                        <asp:Label ID="lbRestEndPlan" runat="server" CssClass="DigitsValueLarge"></asp:Label>
                                                        <asp:Label ID="Label4" runat="server" CssClass="InformationText" Text="&nbsp;факт&nbsp;"></asp:Label>
                                                        <asp:Label ID="lbRestEndFact" runat="server" CssClass="DigitsValueLarge"></asp:Label>
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
        </table>
        <table style="margin-top: -10px; margin-left: -7px">
            <tr>
                <td style="width: 775px; text-align: left; background-color: Black; padding-left: 4px"
                    align="left" valign="top">
                    <uc3:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Расходы по главным распорядителям"
                        Width="764px" MultitouchReport="FO_0735_0007" />
                </td>
            </tr>
            <tr>
                <td style="width: 763px; padding-left: 7px">
                    <div style="margin-top: -5px">
                        <uc1:TagCloud ID="TagCloud1" runat="server" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
