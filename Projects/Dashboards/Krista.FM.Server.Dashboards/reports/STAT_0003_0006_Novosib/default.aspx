<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.STAT_0003_0006_Novosib.Default" %>

<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc1" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc2" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc3" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc5" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td valign="top" style="width: 100%; vertical-align: top;">
                <uc3:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="default.html"
                    Visible="true"></uc3:PopupInformer>&nbsp;&nbsp;&nbsp;
                <asp:Label ID="PageTitle" runat="server" Text="Label" CssClass="PageTitle"></asp:Label>
                <br />
                <asp:Label ID="PageSubTitle" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" style="width: 100%;">
                <uc4:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                &nbsp;
                <uc5:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td>
                <table style="vertical-align: top;">
                    <tr>
                        <td valign="top">
                            <uc1:CustomMultiCombo ID="ComboDate" runat="server"></uc1:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc1:CustomMultiCombo ID="ComboCompareMode" runat="server"></uc1:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc1:CustomMultiCombo ID="ComboRegion" runat="server"></uc1:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc2:RefreshButton ID="RefreshButton1" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table id="TextArea" runat="server" style="border-collapse: collapse; background-color: White;
        width: 100%; margin-top: 10px;">
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
            <td>
                <asp:Label ID="LabelText" runat="server" CssClass="PageSubTitle"></asp:Label>
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
                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" SkinID="UltraWebGrid">
                </igtbl:UltraWebGrid>
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
            <td>
                <igmisc:WebAsyncRefreshPanel ID="PanelCaption1" runat="server">
                    <asp:Label ID="LabelChart1" runat="server" CssClass="ElementTitle"></asp:Label>
                </igmisc:WebAsyncRefreshPanel>
            </td>
            <td class="right">
            </td>
        </tr>
        <tr runat="server" id="cbMORow">
            <td class="left">
            </td>
            <td>
                <asp:CheckBox ID="cbMO1" runat="server" Text="Уровень регистрируемой безработицы, % от численности трудоспособного населения в трудоспособном возрасте" AutoPostBack="true"
                    Checked="true" Style="font-family: Verdana; font-size: 10pt;" /><br />
                <asp:CheckBox ID="cbMO2" runat="server" Text="Уровень напряженности на рынке труда, ед." AutoPostBack="true" Checked="false"
                    Style="font-family: Verdana; font-size: 10pt;" /><br />
                <asp:CheckBox ID="cbMO3" runat="server" Text="Средняя продолжительность безработицы, мес." AutoPostBack="true" Checked="false"
                    Style="font-family: Verdana; font-size: 10pt;" /><br />
                <asp:CheckBox ID="cbMO4" runat="server" Text="Уровень трудоустройства (с начала года), %" AutoPostBack="true" Checked="false"
                    Style="font-family: Verdana; font-size: 10pt;" /><br />
            </td>
            <td class="right">
            </td>
        </tr>
        <tr runat="server" id="cbSubjectRow">
            <td class="left">
            </td>
            <td>
                <asp:CheckBox ID="cbSubject1" runat="server" Text="Уровень зарегистрированной безработицы (от экономически активного населения), %" AutoPostBack="true"
                    Checked="true" Style="font-family: Verdana; font-size: 10pt;" /><br />
                <asp:CheckBox ID="cbSubject2" runat="server" Text="Уровень напряженности на рынке труда, ед." AutoPostBack="true" Checked="false"
                    Style="font-family: Verdana; font-size: 10pt;" /><br />
            </td>
            <td class="right">
            </td>
        </tr>
        <tr>
            <td class="left">
            </td>
            <td>
                <igmisc:WebAsyncRefreshPanel ID="PanelChart1" runat="server">
                    <igchart:UltraChart ID="UltraChart1" runat="server">
                        <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_40_01#SEQNUM(100).png" />
                    </igchart:UltraChart>
                </igmisc:WebAsyncRefreshPanel>
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
            <td>
                <asp:Label ID="LabelChart2" runat="server" CssClass="ElementTitle"></asp:Label>
                <igchart:UltraChart ID="UltraChart2" runat="server">
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_40_01#SEQNUM(100).png" />
                </igchart:UltraChart>
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
    <igtbl:UltraWebGrid ID="EmptyGrid" runat="server" Visible="false">
    </igtbl:UltraWebGrid>

    <script type="text/javascript">

        function uncheck(checkBoxId1, checkBoxId2, checkBoxId3) {
            var checkbox = document.getElementById(checkBoxId1);
            checkbox.checked = false;
            checkbox = document.getElementById(checkBoxId2);
            checkbox.checked = false;
            checkbox = document.getElementById(checkBoxId3);
            checkbox.checked = false;
        }
    </script>
</asp:Content>
