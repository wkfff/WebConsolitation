<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UFK_0017_0001_V.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.UFK_0017_0001_V" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
            <table style="width: 320px; height: 360px; border-right: 0px solid; border-top: 0px solid; left: 0px; border-left: 0px solid; border-bottom: 0px solid; position: absolute; top: 0px; border-collapse: collapse; background-color: black;">
      <tr><td valign="top" style="padding-left: 5px">
          <asp:Table ID="restsTable" runat="server" GridLines="Both" Width="310px" BorderColor="#323232">
          </asp:Table>
      </td></tr>
          <tr><td style="width: 315px; border-right: 0px solid; border-top: 0px solid; padding-left: 5px; background-image: url(../../../images/servePane.gif); border-left: 0px solid; border-bottom: 0px solid; background-repeat: repeat-x; border-collapse: collapse; height: 20px; left: 0px; position: static; top: 408px;" align="left" valign="top" colspan="2">
                <asp:Label ID="Label" runat="server" SkinID="ServeText"></asp:Label><br />
                <asp:Label ID="LabelDate" runat="server" SkinID="ServeText"></asp:Label></td></tr>
             
        </table>
    </div>
    </form>
</body>
</html>

