<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FST_0001_0001.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FST_0001_0001" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Src="../../../iPadBricks/FST_0001_0001_Chart.ascx" TagName="FST_0001_0001_Chart"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: Black">
    <form id="form1" runat="server">
    <touchelementbounds src="../../../TemporaryImages/FST_0001_0001/TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 768px; height: 1250px; top: 0px; left: 0px;
        overflow: hidden; z-index: 2;">
        <table style="position: absolute; width: 760px; height: 900px; background-color: Black;
            top: 0px; left: 0px; overflow: hidden">
            <tr>
                <td style="text-align: left; background-color: Black;" align="left" valign="top">
                    <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Электроснабжение"
                        Width="100%" />
                    <uc1:FST_0001_0001_Chart ID="UltraChartFST_0001_0001_Chart1" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="text-align: left; background-color: Black;" align="left" valign="top">
                    <div style="margin-top: -5px">
                        <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Теплоснабжение"
                            Width="100%" />
                        <uc1:FST_0001_0001_Chart ID="UltraChartFST_0001_0001_Chart2" runat="server" />
                    </div>
                </td>
            </tr>
            <tr>
                <td style="text-align: left; background-color: Black;" align="left" valign="top">
                    <div style="margin-top: -5px">
                        <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Водоснабжение"
                            Width="100%" />
                        <uc1:FST_0001_0001_Chart ID="UltraChartFST_0001_0001_Chart3" runat="server" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
