<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Sport_0001_0011.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.Sport_0001_0011" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader" TagPrefix="uc" %>
<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick" TagPrefix="uc" %>
<%@ Register Src="../../../Components/UltraChartItem.ascx" TagName="UltraChartItem" TagPrefix="uc"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Untitled Page</title>
</head>
<body style="background-color: black;">
	<form id="form1" runat="server"> 
		<touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
		<div style="position: absolute; width: 768px; height: 950px; background-color: black; top: 0px; left: 0px; z-index: 2;">			
						
			<uc:iPadElementHeader ID="Header" runat="server" Width="100%" />
			
			<div style="margin: 0 10px 25px 10px; width: 748px;">
				<uc:UltraChartItem ID="UltraChartCount" runat="server"></uc:UltraChartItem>				
			</div>

			<div style="margin: 0 10px 25px 10px; width: 748px;">
				<uc:UltraChartItem ID="UltraChartProvide" runat="server"></uc:UltraChartItem>				
			</div>

			<uc:iPadElementHeader ID="HeaderGymn" runat="server" Width="100%" />	
			<div style="margin: 0 10px 15px 10px; width: 748px;">
				<uc:UltraGridBrick ID="GridBrickGymn" runat="server"></uc:UltraGridBrick>					
			</div>
			
			<uc:iPadElementHeader ID="HeaderStad" runat="server" Width="100%" />	
			<div style="margin: 0 10px 15px 10px; width: 748px;">
				<uc:UltraGridBrick ID="GridBrickStad" runat="server"></uc:UltraGridBrick>					
			</div>

			<uc:iPadElementHeader ID="HeaderSwim" runat="server" Width="100%" />	
			<div style="margin: 0 10px 15px 10px; width: 748px;">
				<uc:UltraGridBrick ID="GridBrickSwim" runat="server"></uc:UltraGridBrick>					
			</div>
			
			<br />
		</div>		
	</form>
</body>
</html>
