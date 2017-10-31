<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FO_0035_0008_Horizontal.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0035_0018" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body link="White" vlink="White" style="background-color: black;">
    <form id="form1" runat="server">
        <table style="border-collapse: collapse; background-color: Black; width: 936px; height: 100%;">
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
                    <asp:Label ID="Label1" runat="server" CssClass="ElementTitle" Text="Расходы по главному распорядителю"></asp:Label>
                </td>
                <td style="background: Black url(../../../images/iPadContainer/headerright.gif); background-repeat: no-repeat;
                    width: 1px; height: 36px; background-color: Black;">
                </td>
            </tr>
            <tr>
                <td valign="top" colspan="3">
                    <table>
                        <tr>
                            <td>
                                <div style="float: left; margin-right: 10px">
                                    <asp:Image ID="Image1" runat="server" /></div>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbName" runat="server" CssClass="ImportantText" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:HyperLink ID="HyperLinkSite" runat="server" CssClass="TableFont">HyperLink</asp:HyperLink>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbFIO" runat="server" CssClass="TableFont" Style="margin-top: 10px;"></asp:Label><br />
                                            <asp:Label ID="lbDirector" runat="server" CssClass="InformationText">Директор департамента</asp:Label><br />
                                            <asp:Label ID="lbPhone" runat="server" CssClass="InformationText"></asp:Label><br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" CssClass="InformationText" Style="margin-top: 10px;">E-mail:</asp:Label>&nbsp;<asp:HyperLink
                                                ID="HyperLinkMail" runat="server" CssClass="TableFont">HyperLink</asp:HyperLink>
                                        </td>
                                    </tr>                                    
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td valign="top" style="padding-top: 0px;" colspan="3">
                    <asp:Table ID="detailTable" runat="server" BackColor="Black" BorderColor="#323232" BorderStyle="Solid"
                        BorderWidth="1px" GridLines="Both" Width="936px">
                    </asp:Table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
