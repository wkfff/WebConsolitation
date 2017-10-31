<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Oil_0004_0001.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.Oil_0004_0001" %>

<%@ Register Src="OIL_0004_0001_Text.ascx" TagName="OIL_0004_0001_Text" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 768px; height: 950px; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: hidden;">
        <table style="width: 768px; height: 900; border-collapse: collapse; background-color: Black;
            top: 0px; left: 0px;">
            <tr>
                <td align="left" valign="top">
                    <uc1:OIL_0004_0001_Text ID="OIL_0004_0001_Text2" runat="server" />
                    <uc1:OIL_0004_0001_Text ID="OIL_0004_0001_Text3" runat="server" />
                    <uc1:OIL_0004_0001_Text ID="OIL_0004_0001_Text4" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
