<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="Index.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.IndexPage" Title="Title" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="PresentationText" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label><br />
    <div style="width: 68%; float: left">
        <asp:PlaceHolder ID="ReportsPlaceHolder" runat="server"></asp:PlaceHolder>
    </div>
    <div style="width: 32%; height: 70%; float: right;">
        <asp:PlaceHolder ID="HotReportsPlaceHolder" runat="server"></asp:PlaceHolder>
        <asp:PlaceHolder ID="WebNewsPlaceHolder" runat="server"></asp:PlaceHolder>
        <asp:PlaceHolder ID="DocumentsPlaceHolder" runat="server"></asp:PlaceHolder>
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
                <td class="headerleft">
                </td>
                <td class="header">                    
                       <div style="vertical-align:top; float: left;">Партнеры&nbsp;</div>
                        
                </td>
                <td class="headerright">
                </td>
            </tr>
            <tr>
                <td class="left">
                </td>
                <td style="overflow: visible;">
                    <a href="http://www.bujet.ru">
                        <img alt="Издательский дом «Бюджет»" src="App_Themes/Default/Images/bujet.gif" title="Издательский дом «Бюджет»" /></a>
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
        <asp:PlaceHolder ID="ContactInformationPlaceHolder" runat="server"></asp:PlaceHolder>
    </div>
    <div runat="server" id="metrikaContainer" visible="false"></div>
    <script type="text/javascript">

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
	} 
	else
	{ 			    
		objCell.className = "GroupReportExpandCellFirstState"; 
	} 
} 
    </script>

</asp:Content>
