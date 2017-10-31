<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UFK_0017_0001_en.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.UFK_0017_0001_en" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head2" runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form2" runat="server">
    <div>
            <table style="width: 320px; height: 360px; border-right: 0px solid; border-top: 0px solid; left: 0px; border-left: 0px solid; border-bottom: 0px solid; position: absolute; top: 0px; border-collapse: collapse; background-color: black;">
            <tr><td align="left" valign="middle" colspan="2">
            <table>
            <tr>
            <td><asp:Label ID="lbDate" runat="server" SkinID="InformationText" Text="24 ноября"></asp:Label>&nbsp;
                     <asp:Label ID="lbNoTargetSum" runat="server" Font-Names="Arial" Font-Size="18px" ForeColor="White"
                         Text="3 805 235" Font-Bold="True" SkinID="DigitsValueLarge"></asp:Label>&nbsp;
                <asp:Image ID="imageNoTarget" runat="server" BackColor="Black" Height="22px" ImageUrl="~/images/arowGreyDown.png"
                                 Width="21px" />&nbsp;
            </td><td>
              &nbsp;<asp:Label ID="lbNotargetOffset" runat="server" Text="+12 578" Font-Bold="True" Font-Size="14px" SkinID="DigitsValue"></asp:Label><br />
              &nbsp;<asp:Label ID="lbNotargetOffsetPercents" runat="server" Font-Bold="True" Text="+3%" Font-Size="14px" SkinID="DigitsValue"></asp:Label></td></tr></table>
                
                </td></tr>
      <tr><td valign="top" style="width: 200px">  <igGauge:UltraGauge ID="ugNoTarget" runat="server" BackColor="Transparent" Height="160px"
        Width="198px">
                 <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Gaude_ufk_17_01_#SEQNUM(100).png" />
                 <Gauges>
                     <igGaugeProp:LinearGauge CornerExtent="3" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical" Bounds="25, 0, 0, 0">
                         <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="11" OuterExtent="50" InnerExtent="10">
<MinorTickmarks EndExtent="40" Frequency="12.5" StartExtent="20">
<StrokeElement Color="Transparent"></StrokeElement>
</MinorTickmarks>
<BrushElements>
<igGaugeProp:MultiStopLinearGradientBrushElement><ColorStops>
<igGaugeProp:ColorStop Color="135, 135, 135"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="60, 60, 60" Stop="0.5448276"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="135, 135, 135" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</BrushElements>
<Markers>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" ValueString="65" OuterExtent="100" InnerExtent="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="70, 180, 26" StartColor="206, 249, 104" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>

<StrokeElement Color="Transparent" Thickness="3"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Transparent" StartColor="Transparent" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</StrokeElement>
</igGaugeProp:LinearGaugeBarMarker>
</Markers>

<MajorTickmarks PreTerminal="25" PostInitial="25" EndExtent="48" Frequency="25" StartExtent="12">
<StrokeElement Color="Transparent"></StrokeElement>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis></igGaugeProp:NumericAxis>
</Axes>

<StrokeElement Color="Black" Thickness="3"><BrushElements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="360"><ColorStops>
<igGaugeProp:ColorStop Color="90, 90, 90"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="180, 180, 180" Stop="0.5285714"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="135, 135, 135" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</BrushElements>
</StrokeElement>

<Labels Frequency="50" Extent="68" FormatString="&lt;DATA_VALUE:N2&gt;" Font="Trebuchet MS, 10pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                         <brushelements>
<igGaugeProp:BrushElementGroup RelativeBounds="10, 0, 0, 0"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Black"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</brushelements>
                         <strokeelement color="Black" thickness="10"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                     </igGaugeProp:LinearGauge>
                 </Gauges>
             </igGauge:UltraGauge></td><td style="left: 0px; position: static; top: 0px; color: white; font-family: arial; background-color: black; font-weight: normal; font-size: 15px; padding-top: 5px; padding-right: 5px;" valign="top" align="right">
                 <asp:Label ID="Label4" runat="server" SkinID="InformationText" Text="Maximum"></asp:Label><br />
                 <asp:Label ID="lbNoTargetMaxDate" runat="server" Font-Names="Arial" Font-Size="14px"
                     ForeColor="#D1D1D1" Text="27.08.2008" Font-Bold="False" SkinID="InformationText"></asp:Label><br />
                 <asp:Label ID="lbNoTargetMax" runat="server" Font-Bold="True" Font-Names="Arial"
                     Font-Size="14px" ForeColor="White" Text="8 429 447" SkinID="TableFont"></asp:Label><br />
                 <asp:Label ID="Label10" runat="server" SkinID="InformationText" Text="Mean"></asp:Label><br />
                 <asp:Label ID="lbNoTargetAvg" runat="server" Font-Names="Arial" Font-Size="14px"
                     ForeColor="White" Text="3 110 128" Font-Bold="True" SkinID="TableFont"></asp:Label><br />
                 <asp:Label ID="Label7" runat="server" SkinID="InformationText" Text="Minimum"></asp:Label><br />
                 <asp:Label ID="lbNoTargetMinDate" runat="server" Font-Bold="False" Font-Names="Arial"
                     Font-Size="14px" ForeColor="#D1D1D1" Text="12.03.2008" SkinID="InformationText"></asp:Label><br />
                 <asp:Label ID="lbNoTargetMin" runat="server" Font-Names="Arial" Font-Size="14px"
                     ForeColor="White" Text="351 357" Font-Bold="True" SkinID="TableFont"></asp:Label></td>
            </tr>
              <tr><td style="padding-left: 5px" colspan="2">
                 <asp:Label ID="lbRestDate" runat="server" SkinID="InformationText" Text="До конца ноября остатки"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label12" runat="server" SkinID="ServeText" Text="thous. rub"></asp:Label></td></tr>           
             <tr>             
             <td style="position: static; top: 243px; padding-left: 5px; padding-top: 5px; border-right: #323232 1px solid; border-collapse: collapse; font-size: 10px;" valign="top" colspan="2">
                 <asp:Table ID="tableStat" runat="server" BorderColor="#333333" CellPadding="1" CellSpacing="0">
                     <asp:TableRow runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px">
                         <asp:TableCell runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px" Width="165px"><asp:Label ID="Label1" runat="server" SkinID="InformationText" Text="Receipts"></asp:Label></asp:TableCell>
                         <asp:TableCell runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px" Width="135px" HorizontalAlign="Right"><asp:Label ID="lbIncome" runat="server" SkinID="TableFont" Text="2 998 093"></asp:Label></asp:TableCell>
                     </asp:TableRow>
                     <asp:TableRow runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px">
                         <asp:TableCell runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px" Width="165px"><asp:Label ID="Label5" runat="server" SkinID="InformationText" Text="Expenditures "></asp:Label></asp:TableCell>
                         <asp:TableCell runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px" Width="135px" HorizontalAlign="Right"><asp:Label ID="lbOutcome" runat="server" SkinID="TableFont" Text="30509 744"></asp:Label></asp:TableCell>
                     </asp:TableRow>
                     <asp:TableRow runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px">
                         <asp:TableCell runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px" Width="165px"><asp:Label ID="Label9" runat="server" SkinID="InformationText" Text="Borrowings "></asp:Label></asp:TableCell>
                         <asp:TableCell runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px" Width="135px" HorizontalAlign="Right"><asp:Label ID="lbBorrow" runat="server" SkinID="TableFont" Text="0"></asp:Label></asp:TableCell>
                     </asp:TableRow>
                     <asp:TableRow runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px">
                         <asp:TableCell runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px" Width="165px"><asp:Label ID="Label14" runat="server" SkinID="InformationText" Text="Other funding sources "></asp:Label></asp:TableCell>
                         <asp:TableCell runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px" Width="135px" HorizontalAlign="Right"><asp:Label ID="lbOther" runat="server" SkinID="TableFont" Text="35 308"></asp:Label></asp:TableCell>
                     </asp:TableRow>
                     <asp:TableRow runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px">
                         <asp:TableCell runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px" Width="165px"><asp:Label ID="Label17" runat="server" SkinID="InformationText" Text="Expected balance "></asp:Label></asp:TableCell>
                         <asp:TableCell runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px" Width="135px" HorizontalAlign="Right"><asp:Label ID="lbRest" runat="server" SkinID="DigitsValue" Text="2998093"></asp:Label></asp:TableCell>
                     </asp:TableRow>
                 </asp:Table></td></tr>
        </table>
    </div>
    </form>
</body>
</html>