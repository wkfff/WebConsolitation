<%@ Page Language="C#" Title="Отчёт" AutoEventWireup="true" MasterPageFile="~/Reports.Master" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.CTAT.CTAT_0105._default" %>

<%@ Register Src="~/Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc8" %>

<%@ Register Src="~/Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"    TagPrefix="uc7" %>



<%@ Register Src="~/components/HeaderPR.ascx" TagName="HeaderPR" TagPrefix="uc4" %>

<%@ Register Src="~/components/UserComboBox.ascx" TagName="UserComboBox" TagPrefix="uc3" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>

<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Src="~/components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="~/Components/UltraGridExporter.ascx" TagName="UltraGridExporter" TagPrefix="uc4" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table>
        <tr>
            <td style="width: 100%">
                <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="16pt" Text="Интегральная оценка органов местного самоуправления" Font-Names="Arial" SkinID="PageTitle"></asp:Label>
            </td>
            <td>            
               &nbsp;<uc4:UltraGridExporter id="UltraGridExporter1" runat="server"></uc4:UltraGridExporter>                        
            </td>                        
        </tr>
    </table>


    
    <table>
         <tr><td>
             <table>
                 <tr>
                     <td>
                         <uc7:CustomMultiCombo ID="CustomMultiCombo1" runat="server" />
                     </td>
                     <td>
                         <uc8:RefreshButton ID="RefreshButton1" runat="server" />
                     </td>
                 </tr>
             </table>
         </td><td></td></tr>
          <tr>
          <td style="vertical-align: top;">
              <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="10pt" Text="Интегральная оценка органов местного самоуправления по городским округам"></asp:Label>
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
              <igtbl:UltraWebGrid ID= "TG" runat=server OnDataBinding="UltraWebGrid1_DataBinding" OnInitializeLayout="TG_InitializeLayout" SkinID="UltraWebGrid" StyleSetName="Office2007Blue" OnInitializeRow="BG_InitializeRow">
              <Bands>
                  <igtbl:UltraGridBand>
                      <AddNewRow View="NotSet" Visible="NotSet">
                      </AddNewRow>
                  </igtbl:UltraGridBand>
              </Bands>
              <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                  AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                  HeaderClickActionDefault="SortMulti" Name="TG" RowHeightDefault="20px" RowSelectorsDefault="No"
                  SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                  TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
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
                      Width="325px">
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
    
    
                    <asp:Label ID="Label6" runat="server"></asp:Label>
               </td>
               <td style="height: 320px">
               <igchart:UltraChart ID="C" runat=server OnDataBinding="C_DataBinding" BackgroundImageFileName=""   EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" Version="9.1" OnChartDataClicked="C_ChartDataClicked" OnInvalidDataReceived="SetErorFonn">
              <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                  Font-Underline="False" />
              <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear">
              </ColorModel>
              <Effects>
                  <Effects>
                      <igchartprop:GradientEffect>
                      </igchartprop:GradientEffect>
                  </Effects>
              </Effects>
              <Axis>
                  <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                      <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                          Visible="False" />
                      <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                          Visible="True" />
                      <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString=""
                          Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                          <Layout Behavior="Auto">
                          </Layout>
                          <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                              VerticalAlign="Center">
                              <Layout Behavior="Auto">
                              </Layout>
                          </SeriesLabels>
                      </Labels>
                  </Z>
                  <Y2 LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="False">
                      <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                          Visible="False" />
                      <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                          Visible="True" />
                      <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                          Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                          <Layout Behavior="Auto">
                          </Layout>
                          <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                              Orientation="VerticalLeftFacing" VerticalAlign="Center">
                              <Layout Behavior="Auto">
                              </Layout>
                          </SeriesLabels>
                      </Labels>
                  </Y2>
                  <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                      <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                          Visible="False" />
                      <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                          Visible="True" />
                      <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                          Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                          <Layout Behavior="Auto">
                          </Layout>
                          <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                              VerticalAlign="Center">
                              <Layout Behavior="Auto">
                              </Layout>
                          </SeriesLabels>
                      </Labels>
                  </X>
                  <Y LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="True">
                      <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                          Visible="False" />
                      <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                          Visible="True" />
                      <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                          Orientation="Horizontal" VerticalAlign="Center">
                          <Layout Behavior="Auto">
                          </Layout>
                          <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                              Orientation="VerticalLeftFacing" VerticalAlign="Center">
                              <Layout Behavior="Auto">
                              </Layout>
                          </SeriesLabels>
                      </Labels>
                  </Y>
                  <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                      <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                          Visible="False" />
                      <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                          Visible="True" />
                      <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                          Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                          <Layout Behavior="Auto">
                          </Layout>
                          <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                              VerticalAlign="Center">
                              <Layout Behavior="Auto">
                              </Layout>
                          </SeriesLabels>
                      </Labels>
                  </X2>
                  <PE ElementType="None" Fill="Cornsilk" />
                  <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                      <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                          Visible="False" />
                      <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                          Visible="True" />
                      <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                          Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                          <Layout Behavior="Auto">
                          </Layout>
                          <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                              VerticalAlign="Center">
                              <Layout Behavior="Auto">
                              </Layout>
                          </SeriesLabels>
                      </Labels>
                  </Z2>
              </Axis>
                  <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
          </igchart:UltraChart>
               </td>
             </tr>
           <tr>
                <td style="vertical-align: top">
                <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="10pt"
                    Text="Интегральная оценка органов местного самоуправления по муниципальным районам"></asp:Label></td><td style="vertical-align: top">
                <asp:Label id="Label4" runat="server" Text="Label" Font-Bold="True" Font-Names="Arial" Font-Size="10pt"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top">
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
<igtbl:UltraWebGrid ID= "BG" runat=server OnDataBinding="BG_DataBinding" OnInitializeLayout="BG_InitializeLayout" SkinID="UltraWebGrid" StyleSetName="Office2007Blue" Height="200px" OnClick="BG_Click" Width="325px" OnInitializeRow="BG_InitializeRow">
                <Bands>
                    <igtbl:UltraGridBand>
                        <AddNewRow View="NotSet" Visible="NotSet">
                        </AddNewRow>
                    </igtbl:UltraGridBand>
                </Bands>
                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                  AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                  HeaderClickActionDefault="SortMulti" Name="BG" RowHeightDefault="20px" RowSelectorsDefault="No" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                  TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy" SelectTypeColDefault="Extended" SelectTypeCellDefault="Extended">
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
                      Width="325px">
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
                
                <asp:Label ID="Label7" runat="server"></asp:Label>
                </td>
                <td style="vertical-align: top">
               <!----> 
                <igmisc:WebAsyncRefreshPanel ID="MP" runat="server" style="vertical-align: top" RefreshTargetIDs="Label4"> 
                <DMWC:MapControl ID="DundasMap" runat="server" BackColor="Transparent" BackSecondaryColor="Transparent"
                    Height="800px" ImageType="Bmp" ImageUrl="../../../../TemporaryImages/MapPic_#SEQ(300,3)"
                    ResourceKey="#MapControlResKey#MapControl1#" Width="400px" BorderLineColor="Black" BorderLineWidth="1">
                    <NavigationPanel>
                        <Location X="0" Y="0" />
                        <Size Height="90" Width="90" />
                    </NavigationPanel>
                    <Viewport>
                        <Location X="0.250626564" Y="0.125156447" />
                        <Size Height="99.75" Width="99.5" />
                    </Viewport>
                    <ZoomPanel>
                        <Size Height="200" Width="40" />
                        <Location X="0" Y="0" />
                    </ZoomPanel>
                    <ColorSwatchPanel Visible="True">
                        <Location X="0.250626564" Y="89.98749" />
                        <Size Height="80" Width="180" />
                    </ColorSwatchPanel>
                    <DistanceScalePanel>
                        <Location X="0" Y="0" />
                        <Size Height="55" Width="130" />
                    </DistanceScalePanel>
                </DMWC:MapControl>
                </igmisc:WebAsyncRefreshPanel>
            </td></tr>
    </table> 
    <asp:PlaceHolder ID="ContactInformationPlaceHolder" runat="server"></asp:PlaceHolder>
       
</asp:Content>
