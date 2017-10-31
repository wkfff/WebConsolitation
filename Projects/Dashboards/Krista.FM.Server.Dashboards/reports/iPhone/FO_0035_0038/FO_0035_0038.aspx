<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FO_0035_0038.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0035_0038" %>

<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>
<%@ Register Src="../../../Components/ChartBricks/PieChartBrick.ascx" TagName="PieChartBrick"
    TagPrefix="uc8" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
        <div style="position: absolute; width: 768px; background-color: black; top: 0px; left: 0px;
            z-index: 2; overflow: auto;">
            <table style="width: 768; border-collapse: collapse; background-color: Black; top: 0px;
                left: 0px">
                <tr>
                    <td class="InformationText">
                        <asp:Label ID="lbDescription" runat="server" Text="Label"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="background-color: black;padding-left:5px;" valign="top">
                     <uc8:PieChartBrick ID="UltraChart" runat="server">
                           </uc8:PieChartBrick>
                    </td>
                </tr>
                <tr>
                    <td style="background-color: black;padding-left:5px;" valign="top" align="left">
                           <uc5:UltraGridBrick ID="UltraWebGrid" runat="server">
                           </uc5:UltraGridBrick>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
