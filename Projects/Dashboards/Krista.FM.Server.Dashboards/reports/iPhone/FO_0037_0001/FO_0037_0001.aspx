<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FO_0037_0001.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0037_0001" %>

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
            <table style="border-right: 0px solid; border-top: 0px solid; left: 0px; border-left: 0px solid; width: 320px;
                border-bottom: 0px solid; position: absolute; top: 0px; border-collapse: collapse; height: 360px;
                background-color: black">              
                <tr>
                    <td valign="top" style="padding-left: 5px; padding-top: 6px;" colspan="2">
                        <asp:Label ID="lbHeader2" runat="server" SkinID="InformationText" Text="Label"></asp:Label>
                        <asp:Label ID="lbValue2" runat="server" SkinID="DigitsValue" Text="Label"></asp:Label>
                        <asp:Label ID="lbHeader3" runat="server" SkinID="InformationText" Text="Label"></asp:Label>
                        <asp:Label ID="lbValue3" runat="server" SkinID="DigitsValue" Text="Label"></asp:Label>
                        <asp:Label ID="lbHeader4" runat="server" SkinID="InformationText" Text="Label"></asp:Label>
                        <asp:Label ID="lbValue4" runat="server" SkinID="DigitsValue" Text="Label"></asp:Label>
                        <asp:Label ID="lbHeader8" runat="server" SkinID="InformationText" Text="Label"></asp:Label><asp:Label ID="lbValue8" runat="server" SkinID="DigitsValue" Text="Label"></asp:Label><asp:Image ID="Image1" runat="server" />
                        <asp:PlaceHolder ID="PlaceHolderIncomes" runat="server"></asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <td style="padding-left: 5px; padding-top: 0px;" valign="top">
                        <asp:Label ID="lbHeader5" runat="server" SkinID="InformationText" Text="Label"></asp:Label>
                        <asp:Label ID="lbValue5" runat="server" SkinID="DigitsValue" Text="Label"></asp:Label>
                        <asp:Label ID="lbHeader6" runat="server" SkinID="InformationText" Text="Label"></asp:Label>
                        <asp:Label ID="lbValue6" runat="server" SkinID="DigitsValue" Text="Label"></asp:Label>
                        <asp:Label ID="lbHeader7" runat="server" SkinID="InformationText" Text="Label"></asp:Label>
                        <asp:Label ID="lbValue7" runat="server" SkinID="DigitsValue" Text="Label"></asp:Label>
                        <asp:Label ID="lbHeader9" runat="server" SkinID="InformationText" Text="Label"></asp:Label><asp:Label ID="lbValue9" runat="server" SkinID="DigitsValue" Text="Label"></asp:Label><asp:Image ID="Image2" runat="server" />
                        <asp:PlaceHolder ID="PlaceHolderOutcomes" runat="server"></asp:PlaceHolder>
                    </td>
                </tr>
                <tr style="height: 100%">
                    <td style="height: 100%">
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
