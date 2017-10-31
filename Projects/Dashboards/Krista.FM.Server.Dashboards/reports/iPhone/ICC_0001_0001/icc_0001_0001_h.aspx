<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ICC_0001_0001_h.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPhone.ICC_0001_0001_h" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="background-color: black; width: 470px; height: 320px; border-right: 0px solid;
            border-top: 0px solid; left: 0px; border-left: 0px solid; border-bottom: 0px solid;
            position: absolute; top: 0px; border-collapse: collapse; overflow: visible;">
            <tr>
                <td valign="top" style="padding-left: 5px; width: 470px">
                    <asp:Label ID="LabelText" runat="server" SkinID="InformationText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td valign="top" style="padding-left: 5px; width: 470px">
                    <asp:Label ID="LabelGrid" runat="server" SkinID="InformationText"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
