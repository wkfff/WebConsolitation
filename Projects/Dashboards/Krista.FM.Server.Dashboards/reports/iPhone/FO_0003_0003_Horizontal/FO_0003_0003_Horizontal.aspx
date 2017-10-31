<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FO_0003_0003_Horizontal.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0003_0003_Horizontal" %>

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
<body link="White" vlink="White" style="background-color: #a8a8a8">
<!--<ireportdescription width="690" height="500"></ireportdescription>-->
    <form id="form1" runat="server">
        <table style="border-collapse: collapse; background-color: #a8a8a8; width: 670px; height: 100%;">            
            <tr>
                <td valign="top" colspan="3">
                    <table>
                        <tr>
                            <td>
                                <div style="float: left; margin-right: 10px">
                                    <asp:Image ID="Image1" runat="server" /></div>
                            </td>
                            <td>
                                <table Class="InformationTextPopup">                                 
                                    <tr>
                                        <td>
                                            <asp:HyperLink ID="HyperLinkSite" runat="server" CssClass="TableFontPopup">HyperLink</asp:HyperLink>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbFIO" runat="server" CssClass="InformationTextPopup"  Font-Names="Arial" Style="margin-top: 10px;"></asp:Label><br />
                                            <asp:Label ID="lbDirector" runat="server" CssClass="InformationTextPopup" Font-Names="Arial" Text="√убернатор ярославской области, руководитель правительства ярославской области"></asp:Label><br />
                                            <asp:Label ID="lbPhone" runat="server"  Font-Names="Arial" CssClass="InformationTextPopup"></asp:Label><br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" CssClass="InformationTextPopup" Style="margin-top: 10px;"  Font-Names="Arial" Text="E-mail:"></asp:Label>&nbsp;<asp:HyperLink
                                                ID="HyperLinkMail" runat="server" CssClass="TableFontPopup">HyperLink</asp:HyperLink>
                                        </td>
                                    </tr>                                    
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>           
        </table>
    </form>
</body>
</html>
