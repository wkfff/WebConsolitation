<%@ Page Language="C#" AutoEventWireup="true" Codebehind="RG_0017_0004.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.RG_0017_0004" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader" TagPrefix="uc" %>
<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick" TagPrefix="uc" %>
<%@ Register Src="../../../Components/UltraChartItem.ascx" TagName="UltraChartItem" TagPrefix="uc"%>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Untitled Page</title>
	<style type="text/css">
		.TextWhite
		{
			font-size: 14px;
			color: white;
			font-family: Arial;
		}
		.TextXXLarge
		{
			font-size: 22px;
			font-weight: bold;
		}
		.TextXLarge
		{
			font-size: 18px;
			font-weight: bold;
		}
		table.htmlGrid
		{			
			border-top: 1px solid #777;
			border-left: 1px solid #777;
		}
		table.htmlGrid td 
		{
			height: 70px;
			padding-bottom: 20px;
			border-right: 1px solid #777;
			border-bottom: 1px solid #777;
						
			color: #fff;
			font: 18px Arial;
			text-align: center;
		}	
		table.htmlGrid td.header 
		{			
			width: 40px;
			height: 180px;
			padding: 1px;
			
			font: 15px Arial;
			background: #323232;			
		}
		table.htmlGrid td.header .wrapper
		{	
			position: relative;
			display: inline-block;			
		}
		table.htmlGrid .rotate{
			position: absolute;
			left: -90px;
			top: -15px;
			width: 180px;
			
			-webkit-transform: rotate(-90deg); 			
			-moz-transform: rotate(-90deg);			
			text-align: left;			
			z-index: 5;
		}
		table.htmlGrid td.header:nth-child(1)
		{
			width: 150px;
		}
		table.htmlGrid td.photo
		{			
			padding: 1px;					
		}
		.ShadowGreen
		{			
			text-shadow:0px 0px 2px black, 
			0px 0px 2px black, 
						1px 0px 3px rgb(50, 255, 50), 
						-1px 0px 3px rgb(50, 255, 50), 
						0px 1px 3px rgb(50, 255, 50), 
						0px -1px 3px rgb(50, 255, 50), 
						0px 0px 5px rgb(0, 255, 0),
						0px 0px 7px rgb(0, 200, 0);
		}
		.ShadowRed
		{
			text-shadow:0px 0px 2px black, 
			0px 0px 2px black, 
						1px 0px 3px rgb(255, 50, 50), 
						-1px 0px 3px rgb(255, 50, 50), 
						0px 1px 3px rgb(255, 50, 50), 
						0px -1px 3px rgb(255, 50, 50), 
						0px 0px 5px rgb(255, 0, 0),
						0px 0px 7px rgb(200, 0, 0);
		}
	</style>
</head>
<body style="background-color: black;">
	<form id="form1" runat="server">
		<touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
		<div style="position: absolute; width: 768px; height: 950px; overflow: hidden; background-color: black; top: 0px; left: 0px; z-index: 2;">			

			<div style="text-align: center; margin: 10px 0;">
				<div ID="Title" runat="server" class="TextWhite TextXXLarge"></div>
				<div ID="TitleSub" runat="server" class="TextWhite TextXLarge"></div>
			</div>
						
			<div id="ElectorateGrid" runat="server" style="margin: 0 10px 15px 10px;">				
			</div>
			
		</div>
	</form>
</body>
</html>
