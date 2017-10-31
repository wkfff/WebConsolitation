<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FK_0001_0002_wm1_v.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FK_0001_0002_wm1_v" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body bgcolor="black">
    <table style="border-right: 0px solid; border-top: 0px solid; border-left: 0px solid;
        border-bottom: 0px solid; border-collapse: collapse;">
        <tr>
            <td align="left" valign="bottom" style="border-collapse: collapse; background-color: black;">
                <asp:Label ID="Label3" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left" valign="top" style="border-collapse: collapse; background-color: black;">
                <asp:Table ID="gridTable" runat="server" BackColor="Black" BorderColor="#323232"
                    BorderStyle="Solid" BorderWidth="1px" GridLines="Both">
                </asp:Table>
            </td>
        </tr>
        <tr>
            <td style="border-right: 0px solid; border-top: 0px solid; padding-left: 5px; border-left: 0px solid;
                border-bottom: 0px solid; background-repeat: repeat-x; border-collapse: collapse;
                height: 25px; left: 0px; position: static; top: 408px; background-image: url(../../../images/servePane.gif);"
                align="left" valign="top" colspan="2">
                <asp:Label ID="Label1" runat="server" Text="Label" SkinID="ServeText"></asp:Label><br />
                <asp:Label ID="Label2" runat="server" Text="Label" SkinID="ServeText"></asp:Label></td>
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
