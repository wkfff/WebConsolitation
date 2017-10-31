<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FO_0003_0003_Person.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.iPhone.IT_0002_0001.reports.iPhone.FO_0003_0003.FO_0003_0003_Person" %>
<%@ Register Src="../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<table class="InformationTextPopup" style="border-collapse: collapse; background-color: #a8a8a8;
    width: 500px; height: 100%; margin-top: 20px">
    <tr>
        <td valign="middle" align="center">
            <div style="margin-right: 10px; width: 190px">
                <asp:Image ID="Image1" runat="server" /></div>
        </td>
        <td valign="middle">
            <table class="InformationTextPopup">
                <tr>
                    <td valign="top">
                        <asp:Label ID="lbDepfinName" runat="server" CssClass="InformationTextPopup" Font-Names="Arial"
                            Style="margin-top: 10px;"></asp:Label><br />
                        <asp:HyperLink ID="HyperLinkSite" runat="server" CssClass="TableFontPopup">HyperLink</asp:HyperLink>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:Label ID="lbFIO" runat="server" CssClass="InformationTextPopup" Font-Names="Arial"
                            Style="margin-top: 10px;"></asp:Label><br />
                        <asp:Label ID="lbDirector" runat="server" CssClass="InformationTextPopup" Font-Names="Arial"
                            Text="√убернатор ярославской области, руководитель правительства ярославской области"></asp:Label><br />
                        <asp:Label ID="lbPhone" runat="server" Font-Names="Arial" CssClass="InformationTextPopup"></asp:Label><br />
                    </td>
                </tr>
                <tr>
                    <td class="TableFontPopup" valign="top">
                        <asp:Label ID="Label2" runat="server" CssClass="InformationTextPopup" Style="margin-top: 10px;"
                            Font-Names="Arial" Text="e-mail:"></asp:Label>&nbsp;<asp:HyperLink ID="LabelMail"
                                runat="server" Font-Names="Arial" Font-Size="14px" ForeColor="Black" CssClass="TableFontPopup">HyperLink</asp:HyperLink><br/>
                        <asp:Label ID="Label1" runat="server" CssClass="InformationTextPopup" Style="margin-top: 10px;"
                            Font-Names="Arial" Text="видеозвонок FaceTime:"></asp:Label>&nbsp;<asp:HyperLink
                                ID="HyperLinkFaceTime" runat="server" CssClass="TableFontPopup">HyperLink</asp:HyperLink>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
