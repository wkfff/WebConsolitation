<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UltraGridBrick.ascx.cs" Inherits="Krista.FM.Server.Dashboards.Components.UltraGridBrick" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
	Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<!--
UltraGridBrick - скин по умолчанию. Типовые сайты используют именно этот скин. При необходимости изменения,
переопределяются только необходимые атрибуты скина для соответствующего сайта/региона
-->
<igtbl:UltraWebGrid ID="GridControl" runat="server"
	EnableTheming="True" SkinID="UltraGridBrick" EnableAppStyling="True" StyleSetName="Office2007Blue">

	<Bands>
		<igtbl:UltraGridBand>
			<AddNewRow View="NotSet" Visible="NotSet">
			</AddNewRow>
		</igtbl:UltraGridBand>
	</Bands>

	<DisplayLayout Name="UltraWebGrid" Version="4.00" ViewType="Flat" 
		AllowSortingDefault="No" HeaderClickActionDefault="SortMulti" BorderCollapseDefault="Separate"
		RowSelectorsDefault="Yes" SelectTypeRowDefault="Extended" CellClickActionDefault="RowSelect"
		StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
		AllowColumnMovingDefault="None" AllowColSizingDefault="Fixed" AllowUpdateDefault="Yes" AllowDeleteDefault="No"
		RowHeightDefault="20px" >

		<GroupByBox Hidden="True">
		</GroupByBox>

		<GroupByRowStyleDefault BorderColor="White" BackColor="Control">
		</GroupByRowStyleDefault>
				
		<HeaderStyleDefault BorderStyle="Solid" BackColor="#B2B2B2" Wrap="True" HorizontalAlign="Center">
			<BorderDetails ColorTop="White" WidthTop="1px" ColorLeft="White" WidthLeft="1px">
			</BorderDetails>
		</HeaderStyleDefault>

		<FooterStyleDefault BorderStyle="Solid" BackColor="#B2B2B2">
			<BorderDetails ColorTop="White" WidthTop="1px" ColorLeft="White" WidthLeft="1px">
			</BorderDetails>
		</FooterStyleDefault>		

		<RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" 
			Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" BackColor="White">
			<BorderDetails ColorTop="White" ColorLeft="White"></BorderDetails>
			<Padding Left="3px"></Padding>
		</RowStyleDefault>
		
		<RowAlternateStyleDefault BackColor="#F1F1F1">
		</RowAlternateStyleDefault>
		
		<SelectedRowStyleDefault BackColor="#EFF1B4">
		</SelectedRowStyleDefault>   

		<FilterOptionsDefault>
			<FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid"
				Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White"
				CustomRules="overflow:auto;">
				<Padding Left="2px"></Padding>
			</FilterOperandDropDownStyle>
			<FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
			</FilterHighlightRowStyle>
			<FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
				Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px" Height="300px" 
				CustomRules="overflow:auto;">
				<Padding Left="2px"></Padding>
			</FilterDropDownStyle>
		</FilterOptionsDefault>
			
		<EditCellStyleDefault BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
			BackColor="White" Font-Names="Arial" Font-Size="8.25pt">
			<BorderDetails ColorLeft="White" ColorTop="White" />
			<Padding Left="3px" />
		</EditCellStyleDefault>
				
		<Pager MinimumPagesForDisplay="2">
			<PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
				<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
				</BorderDetails>
			</PagerStyle>
		</Pager>
	
		<AddNewBox Hidden="False">
			<BoxStyle BackColor="White" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
				<BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
			</BoxStyle>
		</AddNewBox>
		
	</DisplayLayout>

</igtbl:UltraWebGrid>
