<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Food_0006_0003.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.Food_0006_0003" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body link="White" vlink="White" style="background-color: black;">
    <form id="form1" runat="server">
    <div style="position: absolute; width: 768px; background-color: black; top: 0px;
        left: 0px; z-index: 2; overflow: hidden;">
        <table style="border-collapse: collapse; background-color: Black; top: 0px; left: 0px;">
            <tr>
                <td align="left" valign="top">
                    <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" SkinID="UltraWebGrid">
                    </igtbl:UltraWebGrid>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
