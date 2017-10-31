<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FST_0001_0001_Chart.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.Dashboard.FST_0001_0001_Chart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<table style="border-collapse: collapse; height: 100%">
    <tr>
        <td valign="bottom">
            <div style="margin-top: -5px">
                <asp:Label ID="Label1" runat="server" SkinID="InformationText"></asp:Label>
                <div style="margin-top: -10px">
                    <igchart:UltraChart ID="UltraChart1" runat="server" SkinID="UltraWebColumnChart">
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_FK0101Gadget_#SEQNUM(100).png" />
                    </igchart:UltraChart>
                </div>
            </div>
             <div style="margin-top: -15px; padding-left: 30px"><asp:Label ID="Label3" runat="server" SkinID="ServeText" Visible="false" Text="* Возможно уточнение тарифа"></asp:Label></div>
        </td>
    </tr>
</table>
