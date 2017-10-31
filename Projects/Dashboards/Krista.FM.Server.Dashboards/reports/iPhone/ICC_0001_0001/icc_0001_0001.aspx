<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ICC_0001_0001.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPhone.ICC_0001_0001" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="background-color: black; width: 320px; height: 480px; border-right: 0px solid;
            border-top: 0px solid; left: 0px; border-left: 0px solid; border-bottom: 0px solid;
            position: absolute; top: 0px; border-collapse: collapse; overflow: hidden;">
            <tr>
                <td valign="top">
                    <table>
                        <tr>
                            <td valign="top" style="padding-left: 5px; width: 320px">
                                <asp:Label ID="LabelTitle" runat="server" SkinID="InformationText"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" style="padding-left: 5px; width: 320px">
                                <asp:Label ID="LabelText" runat="server" SkinID="InformationText"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" style="padding-left: 5px; width: 320px">
                                <asp:Label ID="LabelGrid" runat="server" SkinID="InformationText"></asp:Label>
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
