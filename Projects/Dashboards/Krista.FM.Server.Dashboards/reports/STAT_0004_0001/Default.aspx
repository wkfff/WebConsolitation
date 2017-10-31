<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.STAT_0004_0001.Default" Title="" %>
    
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc8" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc5" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc3" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc6" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc7" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.DocumentExport.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid.DocumentExport" TagPrefix="igtbldocexp" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.ExcelExport.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid.ExcelExport" TagPrefix="igtblexp" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %> 
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1"  runat="server">
 <table>
  <tr>
    <td style= "width: 100%">
     <uc8:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl = "Default.html" /> &nbsp;&nbsp;
     <asp:Label ID="Label1" CssClass="PageTitle" runat="server"></asp:Label> &nbsp;&nbsp;
     <asp:Label ID="Label2" CssClass="PageSubTitle" runat="server"></asp:Label> 
    </td>
    <td align="right">
     <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />  &nbsp;<uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
    </td>
  </tr>
 </table>
  
 <table>
  <tr>
    <td valign="top">
      <uc3:CustomMultiCombo ID="ComboYear" runat="server"> </uc3:CustomMultiCombo>
    </td>
    <td valign="top">
      <uc5:RefreshButton ID="RefresfButton1" runat="server"/>
    </td> 
  </tr>
 </table>

 <table>
  <tr>
    <td valign="top" align="left" style="padding-right: 6px; padding-right:3px">
     <table style="border-collapse:collapse; background-color:White; width:100%; margin-top:5px;">
     
      <tr>  
       <td class="topleft">
       </td>
       <td class="top">
       </td>
       <td class="topright">
       </td>
      </tr> 
         
      <tr>
       <td class="left">
       </td>
       
       <td class="overflow: visible;">
        <igtbl:UltraWebGrid  ID="UltraWebGrid" runat="server" Height="200px"
         OnDataBinding="UltraWebGrid_DataBinding"  Width="400px" 
         EnableAppStyling="True" OnInitializeLayout="UltraWebGrid_InitializeLayout" 
         StyleSetName="Office2007Blue" OnInitializeRow="UltraWebGrid_InitializeRow" SkinID="UltraWebGrid">
          <Bands>
             <igtbl:UltraGridBand>
               <AddNewRow View="NotSet" Visible="NotSet">
               </AddNewRow>
             </igtbl:UltraGridBand>
          </Bands>
          <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                         AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                         HeaderClickActionDefault="SortMulti" Name="UltraWebGridFF" RowHeightDefault="20px"
                         SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                         TableLayout="Fixed" Version="4.00" NoDataMessage="Нет данных для отображения"
                         ViewType="OutlookGroupBy">
              <GroupByBox>
                <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                </BoxStyle>
              </GroupByBox>
              <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
              </GroupByRowStyleDefault>
              <ActivationObject BorderColor="" BorderWidth="">
              </ActivationObject>
              <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
               <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
              </FooterStyleDefault>
              <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                               Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                <BorderDetails ColorLeft="Window" ColorTop="Window" />
                   <Padding Left="3px" />
              </RowStyleDefault>
               <FilterOptionsDefault>
                 <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid"
                                             BorderWidth="1px" CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                                             Font-Size="11px">
                    <Padding Left="2px" />
                 </FilterOperandDropDownStyle>
                 <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                 </FilterHighlightRowStyle>
                 <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                      CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                                      Font-Size="11px" Height="300px" Width="200px">
                   <Padding Left="2px" />
                 </FilterDropDownStyle>
               </FilterOptionsDefault>
               <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
               </HeaderStyleDefault>
               <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
               </EditCellStyleDefault>
                <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
                            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="200px"
                            Width="400px">
                </FrameStyle>
                 <Pager MinimumPagesForDisplay="2">
                  <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                  </PagerStyle>
                 </Pager>
                 <AddNewBox Hidden="False">
                  <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                   <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                  </BoxStyle>
                 </AddNewBox>
                  <SelectedRowStyleDefault BackColor="#D7F6AF">
                  </SelectedRowStyleDefault>
          </DisplayLayout>
        </igtbl:UltraWebGrid>
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
    </td>
  </tr>
  <tr>
   <td>
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
            <td class="left">
            </td>
            <td style="overflow: visible;">
                <asp:Label ID="LabelMap" runat="server" CssClass="ElementTitle"></asp:Label>
                <DMWC:MapControl ID="DundasMap" runat="server" BackColor="White" ResourceKey="#MapControlResKey#DundasMap2#"
                    ImageUrl="../../TemporaryImages/map_stat_01_02_02#SEQ(300,3)" RenderingImageUrl="../../TemporaryImages/"
                    RenderType="ImageTag">
                    <NavigationPanel>
                        <Location X="0" Y="0"></Location>
                        <Size Height="90" Width="90"></Size>
                    </NavigationPanel>
                    <Viewport>
                        <Location X="0" Y="0"></Location>
                        <Size Height="100" Width="100"></Size>
                    </Viewport>
                    <ZoomPanel>
                        <Size Height="200" Width="40"></Size>
                        <Location X="0" Y="0"></Location>
                    </ZoomPanel>
                    <ColorSwatchPanel>
                        <Location X="0" Y="0"></Location>
                        <Size Height="60" Width="350"></Size>
                    </ColorSwatchPanel>
                    <DistanceScalePanel>
                        <Location X="0" Y="0"></Location>
                        <Size Height="55" Width="130"></Size>
                    </DistanceScalePanel>
                </DMWC:MapControl>
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
   </td>
  </tr>
  
 </table>
</asp:Content>
