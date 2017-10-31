<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0035_0020_v.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0035_0020_v" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <table style="width: 320px; height: 360px; border-right: 0px solid; border-top: 0px solid; left: 0px; border-left: 0px solid; border-bottom: 0px solid; position: absolute; top: 0px; border-collapse: collapse;">
        <tr><td colspan=2 style="background-color: black; border-right: 0px solid; border-top: 0px solid; padding-left: 5px; left: 0px; border-left: 0px solid; width: 314px; border-bottom: 0px solid; position: static; top: 195px; border-collapse: collapse; height: 211px;" valign="top">
            <asp:Label ID="lbHeader" runat="server" CssClass="InformationText" Text="Исполнение"></asp:Label>&nbsp;
            <asp:Label
                ID="lbQuater" runat="server" CssClass="InformationText"></asp:Label><asp:Label ID="lbDate"
                    runat="server" CssClass="InformationText"></asp:Label>
            <asp:Table ID="table" runat="server" BackColor="Black" BorderColor="#323232"
            BorderStyle="Solid" BorderWidth="1px" GridLines="Both" Width="310px">
            </asp:Table>
        </td>
            </tr>
                
            <tr><td style="width: 315px; border-right: 0px solid; border-top: 0px solid; padding-left: 5px; border-left: 0px solid; border-bottom: 0px solid; background-repeat: repeat-x; border-collapse: collapse; height: 25px; left: 0px; position: static; top: 408px; background-image: url(../../../images/servePane.gif);" align="left" valign="top" colspan="2">
                <asp:Label ID="Label" runat="server" Font-Names="Arial" Font-Size="12px" ForeColor="#BFBFBF"
                    Text="Label" SkinID="ServeText"></asp:Label><br />
                <asp:Label ID="LabelDate" runat="server" Font-Names="Arial" Font-Size="12px" ForeColor="#BFBFBF"
                    Text="Label" SkinID="ServeText"></asp:Label></td></tr> 
       </table>
    </div>
    </form>
</body>
</html>
