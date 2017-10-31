<%@ Page Language="C#" AutoEventWireup="true" Codebehind="MFRF_0002_0002_H.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.MFRF_0002_0002_H" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
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
            <table style="left: 0px; position: absolute; top: 0px; border-collapse: collapse; width: 480px;">
                <tr>
                    <td style="width: 315px; height: 360px; background-color: black; padding-left: 5px;" valign="top">
                                      
                    <asp:TextBox ID="LabelState" runat="server" CssClass="InformationText" SkinID="TextBoxInformationText"
                        Width="470px" ReadOnly="True"></asp:TextBox><br />                                                  
                        <asp:Table ID="IndicatorsTable" runat="server" BackColor="Black" BorderColor="#323232" BorderStyle="Solid"
                            BorderWidth="1px" GridLines="Both" Width="470px">
                        </asp:Table>
                    </td>
                </tr>
                <tr>
                    <td style="padding-left: 5px; background-image: url(../../../images/servePane.gif)">
                        <asp:Table ID="Table1" runat="server">
                            <asp:TableRow runat="server">
                                <asp:TableCell runat="server">
                                    <asp:Label ID="Label" runat="server" SkinID="ServeText" Text="Label"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label1" runat="server" SkinID="ServeText" Text="Label"></asp:Label></asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
