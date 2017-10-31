<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0035_0035.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0035_0035" %>

<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>
<%@ Register Src="../../../Components/Gauges/BarGaugeIndicator.ascx" TagName="BarGaugeIndicator"
    TagPrefix="uc6" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="background-color: black; width: 320px; height: 360px; border-right: 0px solid;
            border-top: 0px solid; left: 0px; border-left: 0px solid; border-bottom: 0px solid;
            position: absolute; top: 0px; border-collapse: collapse;">
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Label ID="GaugeMonthLabel" runat="server" Text="Label" SkinID="InformationText"></asp:Label>&nbsp;
                    <asp:Label ID="GaugeValueLabel" runat="server" Text="Label" SkinID="ImportantText"></asp:Label><br/>
                    <asp:PlaceHolder ID="GaugePlaceHolder" runat="server"></asp:PlaceHolder>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <uc5:UltraGridBrick ID="GridBrick" runat="server">
                    </uc5:UltraGridBrick>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>