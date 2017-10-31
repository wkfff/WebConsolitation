<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FO_0035_0005_Horizontal.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0035_0016" %>

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
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
        <touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
        <div style="position: absolute; width: 1022px; height: 702px; top: 0px; left: 0px; overflow: hidden;
            z-index: 2; border: 1px solid black">
            <table style="position: absolute; width: 1014px; height: 728px; background-color: Black; top: 0px; left: 0px;
                overflow: hidden">
                <tr>
                    <td valign="top">
                        <table style="border-collapse: collapse;">
                            <tr>
                                <td valign="top">
                                    <asp:Label ID="lbHeader" runat="server" CssClass="InformationText" Text="Исполнение кассового плана"></asp:Label>
                                    <asp:Label ID="lbQuater" runat="server" CssClass="InformationText"></asp:Label>
                                    <asp:Label ID="lbDate" runat="server" CssClass="InformationText"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                
                                    <table style="border-collapse: collapse; margin-top: -4px;">
                                        <tr>
                                            <td>
                                            <uc3:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Объем производства" Width="368px"
                                                     MultitouchReport="FO_0035_0013_Horizontal" />
                                                <table>
                                                    <tr>
                                                        <td colspan="3" valign="top" style="padding-top: 0px;">
                                                            <div style="float: left; margin-right: 20px">
                                                                <asp:Image ID="Image1" runat="server" /></div>
                                                            <asp:PlaceHolder ID="PlaceHolderIncomes" runat="server"></asp:PlaceHolder>
                                                            <br />
                                                            <asp:Label ID="Label7" runat="server" CssClass="InformationText" Text="план" Style="padding-left: 70px;"></asp:Label>&nbsp;
                                                            <asp:Label ID="lbPlanIncomes" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                                                            <asp:Label ID="Label9" runat="server" CssClass="InformationText" Text="факт"></asp:Label>&nbsp;
                                                            <asp:Label ID="lbFactIncomes" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                                                            <asp:Table ID="incomes" runat="server" BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px"
                                                                GridLines="Both" Width="365px">
                                                            </asp:Table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td rowspan="2">
                                            </td>
                                            <td rowspan="2" valign="top">
                                            <uc3:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Объем производства" Width="366px"
                                                     MultitouchReport="FO_0035_0014_Horizontal" />
                                                <table>
                                                    <tr>
                                                        <td style="padding-top: 0px;" valign="top" colspan="3">
                                                            <div style="float: left; margin-top: 10px; margin-right: 20px">
                                                                <asp:Image ID="Image2" runat="server" /></div>
                                                            <asp:PlaceHolder ID="PlaceHolderOutcomes" runat="server"></asp:PlaceHolder>
                                                            <br />
                                                            <asp:Label ID="Label5" runat="server" CssClass="InformationText" Text="план" Style="padding-left: 70px;"></asp:Label>&nbsp;
                                                            <asp:Label ID="lbPlanOutcomes" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                                                            <asp:Label ID="Label10" runat="server" CssClass="InformationText" Text="факт"></asp:Label>&nbsp;
                                                            <asp:Label ID="lbFactOutcomes" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                                                            <asp:Table ID="Outcomes" runat="server" BackColor="Black" BorderColor="#323232" BorderStyle="Solid" BorderWidth="1px"
                                                                GridLines="Both" Width="365px">
                                                            </asp:Table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td rowspan="2">
                                            </td>
                                            <td valign="top" rowspan="2">
                                                <uc3:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="ГРБС" Width="260px"
                                                     MultitouchReport="FO_0035_0007_Horizontal" />
                                                <table>
                                                    <tr>
                                                        <td style="padding-left: 5px; width: 360px">
                                                        <div style="width: 270px">
                                                            <uc1:TagCloud ID="TagCloud1" runat="server" /></div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            <uc3:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Остаток средств" Width="368px"
                                                     MultitouchReport="FO_0035_0012_Horizontal" />
                                                <table>
                                                    <tr>
                                                        <td valign="top" colspan="3">
                                                            <asp:Label ID="Label1" runat="server" CssClass="InformationText" Text="Остаток средств на начало квартала"></asp:Label>
                                                            <asp:Label ID="Label11" runat="server" CssClass="InformationText" Text="план&nbsp;"></asp:Label>
                                                            <asp:Label ID="lbRestStartPlan" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                                                            <asp:Label ID="Label13" runat="server" CssClass="InformationText" Text="факт&nbsp;"></asp:Label>
                                                            <asp:Label ID="lbRestStartFact" runat="server" CssClass="DigitsValueLarge"></asp:Label><br />
                                                        </td>
                                                    </tr>
                                                    <tr runat="server" id="restEndTR">
                                                        <td runat="server" id="restEndTD" valign="top" colspan="3">
                                                            <div style="float: left; margin-top: 5px; margin-right: 20px">
                                                                <asp:Image ID="Image3" runat="server" /></div>
                                                            <asp:Label ID="Label2" runat="server" CssClass="InformationText" Text="Остаток средств на конец квартала<br/>"></asp:Label>
                                                            <asp:Label ID="Label3" runat="server" CssClass="InformationText" Text="план&nbsp;"></asp:Label>
                                                            <asp:Label ID="lbRestEndPlan" runat="server" CssClass="DigitsValueLarge"></asp:Label>
                                                            <asp:Label ID="Label4" runat="server" CssClass="InformationText" Text="факт&nbsp;"></asp:Label>
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
        </div>
    </form>
</body>
</html>
