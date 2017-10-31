<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FO_0016_0001.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0016_0001" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
     <touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
        <div style="position: absolute; width: 768px; height: 950px; background-color: black; top: 0px; left: 0px;
            z-index: 2; overflow: auto;">
            <table style="width: 768;  height: 900; border-collapse: collapse; background-color: Black; top: 0px;
                left: 0px; ">
                <tr>
                    <td class="InformationText" style="text-align: left; width:100%">
                        <asp:Label ID="Title" runat="server" Text="Label"></asp:Label>
                        <asp:Label ID="SubTitle" runat="server" Text="Label"></asp:Label>
                    </td>
                </tr>        
                <tr>
                     <td style="text-align: left; background-color: Black; " align="left" valign="top">
                    <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>                    
                    
                </td>
                </tr>                
            </table>
        </div>

        <SCRIPT type="text/javascript">

            function resize(objCell,id) {
                if (document.getElementById(id).style.display == "none") {
                    document.getElementById(id).style.display = "block";
                }
                else {
                    document.getElementById(id).style.display = "none";
                }
                if (objCell.className == "GroupReportExpandCellFirstState") {
                    objCell.className = "GroupReportExpandCellSecondState";
                    objCell.style.backgroundImage = "url(../../../images/CollapseIpad.png)";
                }
                else {
                    objCell.className = "GroupReportExpandCellFirstState";
                    objCell.style.backgroundImage = "url(../../../images/ExpandIpad.png)";
                }
            }
        </SCRIPT>
    </form>
</body>
</html>
