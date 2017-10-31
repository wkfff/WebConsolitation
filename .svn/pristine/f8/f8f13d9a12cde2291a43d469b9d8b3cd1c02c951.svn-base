<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FO_0135_0001.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0135_0001" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div>
                <table style="border-right: 0px solid; border-top: 0px solid; left: 0px; border-left: 0px solid;
                    width: 320px; border-bottom: 0px solid; position: absolute; top: 0px; border-collapse: collapse;
                    height: 360px; background-color: black">
                    <tr>
                        <td valign="top" style="padding-left: 5px;" colspan="2">
                            <asp:Label ID="lbHeader" runat="server" CssClass="InformationText" Text="Исполнение"></asp:Label>
                            <asp:Label ID="lbQuater" runat="server" CssClass="InformationText"></asp:Label>
                            <asp:Label ID="lbDate" runat="server" CssClass="InformationText"></asp:Label></td>
                    </tr>                   
                    <tr runat="server" id="restEndTR">
                        <td  runat="server" id="restEndTD" valign="top" style="padding-left: 5px; padding-top: 6px; display: none" colspan="2">
                            <asp:Label ID="Label2" runat="server" CssClass="InformationText" Text="Остаток средств<br />"></asp:Label>
                            <asp:Label ID="Label3" runat="server" CssClass="InformationText" Text="план&nbsp;"></asp:Label>
                            <asp:Label ID="lbRestEndPlan" runat="server" CssClass="DigitsValueLarge"></asp:Label>
                            <asp:Label ID="Label4" runat="server" CssClass="InformationText" Text="факт&nbsp;"></asp:Label>
                            <asp:Label ID="lbRestEndFact" runat="server" CssClass="DigitsValueLarge"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 5px; padding-top: 10px;" valign="top">
                            <asp:Label ID="Label6" runat="server" Text="Доходы всего" CssClass="InformationText"></asp:Label>&nbsp;
                            <asp:Label ID="lbExecutedIncomes" runat="server" Text="Label" CssClass="DigitsValueLarge"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" valign="top" style="padding-left: 5px; padding-top: 0px;">
                            <asp:Label ID="Label7" runat="server" CssClass="InformationText" Text="план"></asp:Label>&nbsp;
                            <asp:Label ID="lbPlanIncomes" runat="server" CssClass="DigitsValueLarge"></asp:Label><br/>
                            <asp:Label ID="Label9" runat="server" CssClass="InformationText" Text="факт"></asp:Label>&nbsp;
                            <asp:Label ID="lbFactIncomes" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                            <asp:PlaceHolder ID="PlaceHolderIncomes" runat="server"></asp:PlaceHolder>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="padding-left: 5px; padding-top: 6px;">
                            <asp:Label ID="Label8" runat="server" CssClass="InformationText" Text="Расходы всего"></asp:Label>&nbsp;
                            <asp:Label ID="lbExecutedOucomes" runat="server" Text="Label" CssClass="DigitsValueLarge"></asp:Label></td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding-left: 5px; padding-top: 0px;" valign="top">
                            <asp:Label ID="Label5" runat="server" CssClass="InformationText" Text="план"></asp:Label>&nbsp;
                            <asp:Label ID="lbPlanOutcomes" runat="server" CssClass="DigitsValueLarge"></asp:Label><br/>
                            <asp:Label ID="Label10" runat="server" CssClass="InformationText" Text="факт"></asp:Label>&nbsp;
                            <asp:Label ID="lbFactOutcomes" runat="server" CssClass="DigitsValueLarge"></asp:Label>&nbsp;
                            <asp:PlaceHolder ID="PlaceHolderOutcomes" runat="server"></asp:PlaceHolder>
                        </td>
                    </tr>
                    <tr style="height:100%">
                        <td style="height:100%">
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
