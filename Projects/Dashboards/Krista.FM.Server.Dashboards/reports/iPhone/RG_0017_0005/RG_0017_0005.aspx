﻿<%@ Page Language="C#" AutoEventWireup="true" Codebehind="RG_0017_0005.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.RG_0017_0005" %>

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
		.TextLarge
		{
			font-size: 16px;
		}
		.TextNormal
		{
			font-size: 14px;
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
		<div style="position: absolute; overflow: hidden; width: 768px; height: 1850px; background-color: black; top: 0px; left: 0px; z-index: 2;">			

			<div style="text-align: center; margin: 10px 0;">
				<div ID="Title" runat="server" class="TextWhite TextXXLarge"></div>
				<div ID="TitleSub" runat="server" class="TextWhite TextXLarge"></div>
			</div>

			<uc:iPadElementHeader ID="Header1" runat="server" Width="100%" />
			<div style="margin-bottom: 15px; text-align: center;">
				<uc:UltraChartItem ID="UltraChartGovernment1" runat="server"></uc:UltraChartItem>
			</div>

			<uc:iPadElementHeader ID="Header2" runat="server" Width="100%" />
			<div style="margin-bottom: 15px; text-align: center;">
				<uc:UltraChartItem ID="UltraChartGovernment2" runat="server"></uc:UltraChartItem>
			</div>
			
			<uc:iPadElementHeader ID="Header3" runat="server" Width="100%" />
			<div style="margin-bottom: 15px; text-align: center;">
				<uc:UltraChartItem ID="UltraChartGovernment3" runat="server"></uc:UltraChartItem>
			</div>

			<uc:iPadElementHeader ID="Header4" runat="server" Width="100%" />
			<div style="margin-bottom: 15px; text-align: center;">
				<uc:UltraChartItem ID="UltraChartGovernment4" runat="server"></uc:UltraChartItem>
			</div>

		</div>
	</form>
</body>
</html>
