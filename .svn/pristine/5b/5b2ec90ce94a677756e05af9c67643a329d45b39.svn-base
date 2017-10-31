<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SGM_0009.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.SGM_0009" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table id="TABLE1" style="border-right: 0px solid; border-top: 0px solid; left: 0px; border-left: 0px solid; width: 320px; border-bottom: 0px solid; position: absolute; top: 0px; border-collapse: collapse; height: 360px; background-color: black">
            <tr>
                <td colspan="2" valign="top">
                    <table style="width: 314px; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none; padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: -3px 0px 0px; padding-top: 0px;" id="table1Line">
                        <tr>
                            <td colspan="3" style="height: 21px; padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px; padding-top: 0px;">
                                <asp:Label ID="LabelRFCurrentPart1" runat="server" Text="Label" EnableTheming="True" SkinID="InformationText" Height="3px"></asp:Label></td>
                        </tr>
                    </table>
                    <table id="table2Line" style="width: 315px; margin-top: -12px;">
                        <tr>
                            <td style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px; padding-top: 0px">
                                <asp:Label ID="LabelRFCurrentPart2" runat="server" SkinID="InformationText" Text="Label" BackColor="Transparent" Width="103px"></asp:Label>
                    <asp:Label ID="LabelRfValueCurrent" runat="server" SkinID="DigitsValue" Text="Label" BackColor="Transparent"></asp:Label>
                                <asp:Label ID="LabelMeasure" runat="server" SkinID="InformationText" Text="Label"></asp:Label></td>
                        </tr>
                    </table>
                    <table style="width: 314px; padding-right: 0px; padding-left: 0px; left: 1px; padding-bottom: 0px; margin: -6px 0px 0px; border-top-style: none; padding-top: 0px; border-right-style: none; border-left-style: none; top: 39px; border-bottom-style: none;" id="table3Line">
                        <tr>
                            <td style="width: 314px; padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px; padding-top: 0px;" >
                    <asp:Label ID="LabelRFPrev" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
                    <asp:Label ID="LabelRfValuePrev" runat="server" SkinID="DigitsValue" Text="Small"></asp:Label>&nbsp;
                                <asp:Image ID="image" runat="server" BackColor="Black" Height="17px" ImageUrl="~/images/arrowRedUpBB.png"
                                    Width="13px" ImageAlign="Bottom" />
                                &nbsp;<asp:Label ID="LabelPercentText" runat="server" SkinID="InformationText" Text="Label"></asp:Label>&nbsp;
                                <asp:Label ID="LabelProcent" runat="server" SkinID="DigitsValue" Text="Label" Width="50px"></asp:Label></td>
                        </tr>
                    </table>
                    <table style="width: 292px; margin-top: -2px; left: 1px; margin-bottom: 1px; margin-left: 0px;" id="tableShpric">
                        <tr>
                            <td>
                    <asp:Label ID="Label1" runat="server" SkinID="InformationText" Text="Самые болеющие"></asp:Label></td>
                            <td>
                                <asp:Image ID="Image2" runat="server" BackColor="Transparent" Height="40px" ImageUrl="~/images/maxDeseases.png"
                                    Width="98px" /></td>
                        </tr>
                    </table><table style="width: 307px; border-top-style: none; border-collapse: collapse; margin-left: 4px;" id="tableMax">
                        <tr>
                            <td style="width: 213px; border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse;">
                                <asp:Label ID="LabelMax11" runat="server" SkinID="InformationText" Text="доходы "></asp:Label>
                            </td>
                            <td style="border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px 3px 0px 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse" align="right">
                                <asp:Label ID="LabelMax12" runat="server" SkinID="TableFont" Text="2 998 093"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 213px; border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse;">
                                <asp:Label ID="LabelMax21" runat="server" SkinID="InformationText" Text="доходы "></asp:Label>
                            </td>
                            <td style="border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px 3px 0px 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse;" align="right">
                                <asp:Label ID="LabelMax22" runat="server" SkinID="TableFont" Text="2 998 093"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 213px; border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse;">
                                <asp:Label ID="LabelMax31" runat="server" SkinID="InformationText" Text="доходы "></asp:Label>
                            </td>
                            <td style="border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px 3px 0px 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse" align="right">
                                <asp:Label ID="LabelMax32" runat="server" SkinID="TableFont" Text="2 998 093"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 213px; border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse;">
                                <asp:Label ID="LabelMax41" runat="server" SkinID="InformationText" Text="доходы "></asp:Label>
                            </td>
                            <td style="border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px 3px 0px 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse" align="right">
                                <asp:Label ID="LabelMax42" runat="server" SkinID="TableFont" Text="2 998 093"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 213px; border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse;">
                                <asp:Label ID="LabelMax51" runat="server" SkinID="InformationText" Text="доходы "></asp:Label>
                            </td>
                            <td style="border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px 3px 0px 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse" align="right">
                                <asp:Label ID="LabelMax52" runat="server" SkinID="TableFont" Text="2 998 093"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 285px; margin-top: 3px; margin-bottom: 2px; margin-left: 0px;" id="tableBall">
                        <tr>
                            <td style="width: 154px">
                    <asp:Label ID="Label2" runat="server" SkinID="InformationText" Text="Самые здоровые"></asp:Label></td>
                            <td>
                                <asp:Image ID="Image1" runat="server" BackColor="Transparent" Height="40px" ImageUrl="~/images/minDeseases.png"
                                    Width="40px" /></td>
                        </tr>
                    </table>
                    <table style="width: 307px; border-top-style: none; border-collapse: collapse; margin-left: 4px;" id="tableMin">
                        <tr>
                            <td style="width: 213px; border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse;">
                                <asp:Label ID="LabelMin11" runat="server" SkinID="InformationText" Text="доходы "></asp:Label>
                            </td>
                            <td style="border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px 3px 0px 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse" align="right">
                                <asp:Label ID="LabelMin12" runat="server" SkinID="TableFont" Text="2 998 093"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 213px; border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse;">
                                <asp:Label ID="LabelMin21" runat="server" SkinID="InformationText" Text="доходы "></asp:Label>                            
                            </td>
                            <td style="border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px 3px 0px 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse;" align="right">
                                <asp:Label ID="LabelMin22" runat="server" SkinID="TableFont" Text="2 998 093"></asp:Label>                            
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 213px; border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse;">
                                <asp:Label ID="LabelMin31" runat="server" SkinID="InformationText" Text="доходы "></asp:Label>                            
                            </td>
                            <td style="border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px 3px 0px 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse" align="right">
                                <asp:Label ID="LabelMin32" runat="server" SkinID="TableFont" Text="2 998 093"></asp:Label>                            
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 213px; border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse;">
                                <asp:Label ID="LabelMin41" runat="server" SkinID="InformationText" Text="доходы "></asp:Label>                            
                            </td>
                            <td style="border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px 3px 0px 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse" align="right">
                                <asp:Label ID="LabelMin42" runat="server" SkinID="TableFont" Text="2 998 093"></asp:Label>                            
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 213px; border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse;">
                                <asp:Label ID="LabelMin51" runat="server" SkinID="InformationText" Text="доходы "></asp:Label>                            
                            </td>
                            <td style="border-right: #333333 1px solid; padding-right: 0px; border-top: #333333 1px solid; padding-left: 0px; padding-bottom: 0px; margin: 0px 3px 0px 0px; border-left: #333333 1px solid; padding-top: 0px; border-bottom: #333333 1px solid; border-collapse: collapse" align="right">
                                <asp:Label ID="LabelMin52" runat="server" SkinID="TableFont" Text="2 998 093"></asp:Label>                            
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
