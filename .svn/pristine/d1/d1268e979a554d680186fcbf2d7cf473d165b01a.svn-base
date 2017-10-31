<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Oil_0006_0003.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPad.Oil_0006_0003" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>
    <%@ Register src="../../../iPadBricks/OIL_0006_0003_Text.ascx" tagname="Oil_0006_0003_Text" tagprefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
     <div style="position: absolute; width: 768px; top: 0px; left: 0px; overflow: hidden;
        z-index: 2;">
        <table style="width: 765; height: 950; border-collapse: collapse; background-color: Black;
            top: 0px; left: 0px">
            <tr>
                <td>
                    <asp:Label ID="lbDescription" runat="server" SkinID="InformationText" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="left">
                   <uc5:UltraGridBrick ID="GridBrick" runat="server"></uc5:UltraGridBrick>
                    <div style="float: left">
                        <asp:Label ID="lbRests" runat="server" SkinID="ImportantText" Text=""></asp:Label></div>
                </td>
            </tr>
        </table>
        <table style="width: 768px; border-collapse: collapse; background-color: Black;
            top: 0px; left: 0px;">
            <tr>
                <td align="left" valign="top">
                    <uc1:OIL_0006_0003_Text ID="UltraChart1" runat="server" />
                    <uc1:OIL_0006_0003_Text ID="UltraChart2" runat="server" />
                    <uc1:OIL_0006_0003_Text ID="UltraChart3" runat="server" />
                </td>
            </tr>
        </table>
    </div>    
            <div style="margin-top: 100px; display:none">
                <asp:PlaceHolder ID="PlaceHolder" runat="server"></asp:PlaceHolder>
            </div>
    </form>
</body>
</html>
