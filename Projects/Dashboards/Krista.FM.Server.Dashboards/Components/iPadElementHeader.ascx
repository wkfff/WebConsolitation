<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="iPadElementHeader.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.Components.iPadElementHeader" %>
<table style="border-collapse: collapse;" runat="server" id="headerTable">
    <tr>
        <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
            width: 1px; background-color: Black">
        </td>
        <td style="background-image: url(../../../images/iPadContainer/Top.gif); background-repeat: repeat-x;
            background-color: Black; height: 3px;">
        </td>
        <td style="background: white url(../../../images/iPadContainer/righttop.gif); background-repeat: no-repeat;
            width: 2px; background-color: Black;">
        </td>
    </tr>
    <tr>
        <td style="background-image: url(../../../images/iPadContainer/headerleft.gif); background-repeat: no-repeat;
            width: 2px; height: 36px; background-color: Black">
        </td>
        <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
            background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px;
            padding-left: 3px; height: 36px; text-align: center; vertical-align: top;">
            <table style="border-collapse: collapse; width: 100%">
                <tr>
                    <td style="width: 100%">
                        <asp:Label ID="elementCaption" runat="server" CssClass="ElementTitle" Text=""></asp:Label>
                    </td>
                    <td valign="top">
                        <div style="float: right;" id="detalizationIconDiv" runat="server">
                        </div>
                    </td>
                </tr>
            </table>
        </td>
        <td style="background: Black url(../../../images/iPadContainer/headerright.gif);
            background-repeat: no-repeat; width: 1px; height: 36px; background-color: Black;">
        </td>
    </tr>
</table>
