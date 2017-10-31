<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="STAT_0001_0007_v.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.STAT_0001_0007_v" %>

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
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: Black">
    <form id="form1" runat="server">
    <div style="position: absolute; width: 760px; height: 2350px; top: 0px; left: 0px;
        overflow: visible; z-index: 2">
        <table style="position: absolute; width: 760px; height: 2350px; background-color: Black;
            top: 0px; left: 0px; overflow: visible">
            <tr>
                <td style="text-align: left; background-color: Black;" align="left" valign="top">
                    <table style="position: absolute; width: 760px; background-color: Black; top: 0px;
                        left: 0px">
                        <tr>
                            <td style="width: 760px; text-align: left; background-color: Black; padding-left: 4px"
                                align="left" valign="top">
                                <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Уровень безработицы" Width="760px" />
                                <asp:Label ID="TextBox1" runat="server" SkinID="ServeText" Width="100%" ></asp:Label>
                                <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" SkinID="UltraWebGrid" Width="760px" >
                                </igtbl:UltraWebGrid>
                                <div style='height: 10px; clear: both'>
                                </div>
                                <asp:Label ID="TextBox2" runat="server" SkinID="ServeText"></asp:Label>
                                <igtbl:UltraWebGrid ID="UltraWebGrid2" runat="server" SkinID="UltraWebGrid">
                                </igtbl:UltraWebGrid>
                                <div style='height: 10px; clear: both'>
                                </div>
                                <uc1:iPadElementHeader ID="mapElementCaption" runat="server" Text="Уровень безработицы"
                                     Width="760px"/>
                                <DMWC:MapControl ID="DundasMap1" runat="server" BackColor="White" ResourceKey="#MapControlResKey#DundasMap1#"
                                    ImageUrl="../../../TemporaryImages/map_stat_01_02_01#SEQ(300,3)" RenderingImageUrl="../../../TemporaryImages/"
                                    RenderType="ImageTag">
                                </DMWC:MapControl>
                                <uc1:iPadElementHeader ID="map2ElementCaption" runat="server" Text="Уровень безработицы"
                                     Width="760px"/>
                                <DMWC:MapControl ID="DundasMap2" runat="server" BackColor="White" ResourceKey="#MapControlResKey#DundasMap2#"
                                    ImageUrl="../../TemporaryImages/map_stat_01_02_02#SEQ(300,3)" RenderingImageUrl="../../TemporaryImages/"
                                    RenderType="ImageTag">
                                </DMWC:MapControl>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
