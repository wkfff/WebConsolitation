<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OIL_0006_0002.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.OIL_0006_0002" %>

<%@ Register src="../../../iPadBricks/OIL_0006_0001_Text.ascx" tagname="Oil_0006_0001_Text" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <touchelementbounds src="../../../TemporaryImages/oil_0006_0002/TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 768px; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: hidden;">
        <table style="width: 768px; border-collapse: collapse; background-color: Black;
            top: 0px; left: 0px;">
            <tr>
                <td align="left" valign="top">
                    <uc1:OIL_0006_0001_Text ID="UltraChart1" runat="server" />
                    <uc1:OIL_0006_0001_Text ID="UltraChart2" runat="server" />
                    <uc1:OIL_0006_0001_Text ID="UltraChart3" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>