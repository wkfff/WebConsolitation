<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FO_0008_0002.Default" Title="Untitled Page" %>

<%@ Register Src="../../Components/CustomCalendar.ascx" TagName="CustomCalendar" TagPrefix="uc2" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc6" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc7" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Src="../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td valign="top" style="width: 100%; vertical-align: top;">
                <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />&nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" CssClass="PageTitle"></asp:Label><br/>
                <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" rowspan="2" style="width: 100%;">
                <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />&nbsp;<uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right" colspan="2">
                <asp:HyperLink ID="CrossLink1" runat="server" SkinID="HyperLink"></asp:HyperLink>
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
             <td valign="top">
                   <uc2:CustomCalendar ID="CustomCalendar1" runat="server"></uc2:CustomCalendar>
               </td>
           
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
             <td valign="top" align="left" style="font-family:Verdana; font-size:12px; padding-left:20px;" colspan="2">
                            <asp:RadioButtonList ID="RubMiltiplierButtonList" runat="server" AutoPostBack="True" RepeatDirection="horizontal"
                                Width="170px">
                                <asp:ListItem Selected="True">тыс.руб.</asp:ListItem>
                                <asp:ListItem>млн.руб.</asp:ListItem>
                            </asp:RadioButtonList>
                        </td> 
        </tr>
        <tr>
             
             <td style="overflow: visible;" align="right" colspan="3">
                          <asp:CheckBox ID="CheckBox1" runat="server" Text="Код целевых средств"
                                        AutoPostBack="true" Checked="false" Style="font-family: Verdana; font-size: 10pt;" />
                          <asp:CheckBox ID="CheckBox2" runat="server" Text="Целевая статья"
                                        AutoPostBack="true" Checked="false" Style="font-family: Verdana; font-size: 10pt;" />
                          <asp:CheckBox ID="CheckBox3" runat="server" Text="СубКОСГУ"
                                        AutoPostBack="true" Checked="false" Style="font-family: Verdana; font-size: 10pt;" />
                          <asp:CheckBox ID="CheckBox4" runat="server" Text="Получатель бюджетных средств"
                                        AutoPostBack="true" Checked="false" Style="font-family: Verdana; font-size: 10pt;" />
                          <asp:CheckBox ID="CheckBox5" runat="server" Text="Условия софинансирования"
                                        AutoPostBack="true" Checked="false" Style="font-family: Verdana; font-size: 10pt;" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top; margin-top: -3px">
        <tr>
            <td valign="top" align="left">
                <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
                    <tr>
                        <td class="topleft">
                        </td>
                        <td class="top">
                        </td>
                        <td class="topright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                             <asp:CheckBox ID="CheckBox6" runat="server" Text="Остаток от плана"
                                        AutoPostBack="true" Checked="false" Style="font-family: Verdana; font-size: 10pt;" />
                            <uc5:UltraGridBrick ID="GridBrick" runat="server"></uc5:UltraGridBrick>
                            <asp:Label ID="CommentTextLabel" runat="server" CssClass="PageSubTitle"></asp:Label>
                        </td>
                        <td class="right">
                        </td>
                    </tr>
                    <tr>
                        <td class="bottomleft">
                        </td>
                        <td class="bottom">
                        </td>
                        <td class="bottomright">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
