<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FK_0001_0001_wm2_h.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FK_0001_0001_wm2_h" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body bgcolor="black">
    <table style="width:100%; border-collapse: collapse;">
            <tr>
            <td style="width:100%; border-right: lime 0px solid; border-top: lime 0px solid; border-left: lime 0px solid; 
                border-bottom: lime 0px solid; background-color: black; border-collapse: collapse;"
                align="left" valign="bottom">
                <asp:Label ID="LabelPageTitle" runat="server" SkinID="InformationText" Text="Label"></asp:Label>                
            </td>
            </tr>
        <tr>
            <td style="width:100%; border-right: lime 0px solid; border-top: lime 0px solid;
                border-left: lime 0px solid; border-bottom: lime 0px solid; 
                background-color: black; border-collapse: collapse;"
                align="left" valign="top">           
                <asp:Table ID="gridTable" runat="server" BackColor="Black" BorderColor="#323232"
                    BorderStyle="Solid" BorderWidth="1px" GridLines="Both" Width="800px">
                </asp:Table>
                <asp:Label ID="Comment1" runat="server" SkinID="ServeText" Text="Label"></asp:Label><br />
                <asp:Label ID="Comment2" runat="server" SkinID="ServeText" Text="Label"></asp:Label><br />
                <asp:Label ID="Comment3" runat="server" SkinID="ServeText" Text="Label"></asp:Label><br />
                <asp:Label ID="Comment4" runat="server" SkinID="ServeText" Text="Label"></asp:Label><br />
            </td>
        </tr>
        <tr>
            <td style="width:100%; border-right: 0px solid; border-top: 0px solid; background-image: url(../../../images/servePane.gif);
                border-left: 0px solid; border-bottom: 0px solid; background-repeat: repeat-x;
                border-collapse: collapse;" align="left" valign="top">
                <asp:Table ID="Table1" runat="server">
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">
                            <asp:Label ID="Label2" runat="server" Font-Names="Arial" ForeColor="#BFBFBF" Text="Label"
                                SkinID="ServeText"></asp:Label><br />
                            <asp:Label ID="Label1" runat="server" Font-Names="Arial" ForeColor="#BFBFBF" Text="Label"
                                SkinID="ServeText"></asp:Label></asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </td>
        </tr>
    </table>
    <div id="adminPanel" style="display: none;">
        <form id="form1" runat="server">
            <div>
            </div>
        </form>
    </div>
</body>
</html>
