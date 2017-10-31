<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FK_0001_0004_wm1_H.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FK_0001_0004_wm1_H" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body bgcolor="black">
    <table style="border-collapse: collapse;">
        <tr>
            <td style="border-right: lime 0px solid; border-top: lime 0px solid;
                border-left: lime 0px solid; border-bottom: lime 0px solid;
                background-color: black; border-collapse: collapse;" align="left">
                <asp:Label ID="LabelTitle1" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Table ID="gridTable1" runat="server" BackColor="Black" BorderColor="#323232"
                    BorderStyle="Solid" BorderWidth="1px" GridLines="Both">
                </asp:Table>
            </td>
        </tr>
        </table>
    <table style="border-collapse: collapse;">        
        <tr>
            <td>
                <asp:Label ID="LabelTitle2" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Table ID="gridTable2" runat="server" BackColor="Black" BorderColor="#323232"
                    BorderStyle="Solid" BorderWidth="1px" GridLines="Both">
                </asp:Table>
            </td>
        </tr>
        </table>
    <table style="border-collapse: collapse;">            
        <tr>
            <td>
                <asp:Label ID="LabelTitle3" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Table ID="gridTable3" runat="server" BackColor="Black" BorderColor="#323232"
                    BorderStyle="Solid" BorderWidth="1px" GridLines="Both">
                </asp:Table>
            </td>
        </tr>        
        <tr>
            <td>
                <asp:Label ID="LabelComments1" runat="server" Text="Label" SkinID="InformationText"></asp:Label><br />
                <asp:Label ID="LabelComments2" runat="server" Text="Label" SkinID="InformationText"></asp:Label><br />
                <asp:Label ID="LabelComments3" runat="server" Text="Label" SkinID="InformationText"></asp:Label><br />
                <asp:Label ID="LabelComments4" runat="server" Text="Label" SkinID="InformationText"></asp:Label><br />
                <asp:Label ID="LabelComments5" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="border-right: 0px solid; border-top: 0px solid; background-image: url(../../../images/servePane.gif);
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
