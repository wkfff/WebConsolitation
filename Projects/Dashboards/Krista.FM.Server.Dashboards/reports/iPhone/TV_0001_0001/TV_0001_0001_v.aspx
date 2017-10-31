<%@ Page Language="C#" AutoEventWireup="true" Codebehind="TV_0001_0001_v.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.TV_0001_0001_v" %>

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
        <table style="position: absolute; background-color: Black; top: 0px; left: 0px; overflow: hidden">
            <tr>
                <td style="text-align: left; background-color: Black;" align="left" valign="top">
                    <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>                    
                    
                </td>
            </tr>
        </table>

        <SCRIPT type="text/javascript">

            function resize(objCell)
            { 
	            var objParent; 
	            objParent=objCell.parentNode.parentNode; 	
	            for(var j=1;j<objParent.childNodes.length;j++)
	            { 	    		
		            if(objParent.childNodes[j].className == "ReportRowFirstState")
		            { 
		                objParent.childNodes[j].className = "ReportRowSecondState";	
		            } 
		            else
		            { 
		                objParent.childNodes[j].className = "ReportRowFirstState";
		            } 		
	            }
	            if(objCell.className == "GroupReportExpandCellFirstState")
	            { 			    
		            objCell.className = "GroupReportExpandCellSecondState";	
		            objCell.style.backgroundImage = "url(../../../images/CollapseIpad.png)";
	            } 
	            else
	            { 			    
		            objCell.className = "GroupReportExpandCellFirstState"; 
		            objCell.style.backgroundImage = "url(../../../images/ExpandIpad.png)";
	            } 
            } 
        </SCRIPT>

    </form>
</body>
</html>
