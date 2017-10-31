<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0035_0035_v.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0035_0035_v" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
    <%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
        TagPrefix="uc5" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="width: 320px; height: 360px; border-right: 0px solid; border-top: 0px solid; left: 0px; border-left: 0px solid; border-bottom: 0px solid; position: absolute; top: 0px; border-collapse: collapse;">
        <tr><td runat="server" ID="mainTD" align="left" valign="top" style="left: 0px; width: 110px; position: static; top: 45px; border-collapse: collapse; background-color: black; padding-left: 4px;">
            <asp:Label ID="Label3" runat="server" Text="Label" SkinID="InformationText"></asp:Label><br/>
                    <uc5:UltraGridBrick ID="GridBrick" runat="server">
                    </uc5:UltraGridBrick>
 <asp:Label ID="planHintLabel" runat="server" Text="Label" SkinID="ServeText"></asp:Label><br/>
  <asp:Label ID="factHintLabel" runat="server" Text="Label" SkinID="ServeText"></asp:Label><br/>
   <asp:Label ID="percentHintLabel" runat="server" Text="Label" SkinID="ServeText"></asp:Label>
           <br /></td></tr>
        <tr><td style="width: 315px; border-right: 0px solid; border-top: 0px solid; padding-left: 5px; border-left: 0px solid; border-bottom: 0px solid; background-repeat: repeat-x; border-collapse: collapse; height: 25px; left: 0px; position: static; top: 408px; background-image: url(../../../images/servePane.gif);" align="left" valign="top" colspan="2">
            <asp:Label ID="Label1" runat="server" Text="Label" SkinID="ServeText"></asp:Label><br/>
            <asp:Label ID="Label2" runat="server" Text="Label" SkinID="ServeText"></asp:Label></td></tr> 
        </table>
    </div>
    </form>
</body>
</html>
