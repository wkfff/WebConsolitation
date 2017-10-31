<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FNS_0004_0002.Default" Title="Задолженность по налогам и сборам, пеням и налоговым санкциям в бюджетную систему РФ" %>
    
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc7" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc5" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc9" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc8" %>
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

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1"  runat="server">
 <table>
  <tr>
    <td style= "width: 100%">
     <uc7:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl = "Default.html" /> &nbsp;&nbsp;
     <asp:Label ID="Label1" CssClass="PageTitle" runat="server"></asp:Label> <br/>
     <asp:Label ID="Label2" CssClass="PageSubTitle" runat="server"></asp:Label> 
    </td>
    <td valign="top">
      <uc9:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />&nbsp;<uc8:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
    </td>  
  </tr>
 </table>
  
 <table>
  <tr>
    <td valign="top">
      <uc3:CustomMultiCombo ID="ComboYear" runat="server"> </uc3:CustomMultiCombo>
    </td>
    <td valign="top">
      <uc3:CustomMultiCombo ID="ComboMonth" runat="server"> </uc3:CustomMultiCombo>
    </td>
    <td valign="top">
      <uc3:CustomMultiCombo ID="ComboFO" runat="server"> </uc3:CustomMultiCombo>
    </td>
           
    <td valign="top">
      <uc5:RefreshButton ID="RefresfButton1" runat="server"/>
    </td>
    
    <td style="width:100%;" align="right">
     <uc2:GridSearch ID="GridSearch1" runat="server"/>     
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
        <igtbl:UltraWebGrid  ID="UltraWebGrid" runat="server" Height="200px"  OnActiveRowChange="UltraWebGrid_ActiveRowChange" 
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
                            <td class="top">
                            </td>
                            <td class="top">
                            </td>
                            <td class="topright">
                            </td>
                        </tr>
                        <tr>
                         <td class="headerleft">
                         </td>
                         <td class="headerReport">
                           <asp:Label ID="lbSubject" runat="server" CssClass="ElementTitle"></asp:Label>
                           <asp:Label ID= "lbSubLabel" runat="server" CssClass="ElementTitle"></asp:Label>
                         </td>
                         <td class="headerright">
                         </td>
                       </tr>      
                       <tr>
                         <td class="left">
                         </td>
                            <td style="overflow: visible;" align="center">
                              
                                <igchart:UltraChart ID="UltraChartFF" runat="server" BackgroundImageFileName=""  
                                     EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                    Version="9.1" OnDataBinding="UltraChartFF_DataBinding">
                                    <Tooltips Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False"
                                        Font-Bold="False"></Tooltips>
                                    <ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink">
                                    </ColorModel>
                                    <Effects>
                                        <Effects>
                                            <igchartprop:GradientEffect>
                                            </igchartprop:GradientEffect>
                                        </Effects>
                                    </Effects>
                                    <Axis>
                                        <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Z>
                                        <Y2 LineThickness="1" TickmarkInterval="20" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                                HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Y2>
                                        <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray"
                                                HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </X>
                                        <Y LineThickness="1" TickmarkInterval="20" Visible="True" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                                                HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Y>
                                        <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray"
                                                HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </X2>
                                        <PE ElementType="None" Fill="Cornsilk"></PE>
                                        <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near"
                                                Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Z2>
                                    </Axis>
                                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_02_08_1#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                            
                            <td class="right">
                            </td>
                        </tr>
                        <tr>
                            <td class="bottomleft">
                            </td>
                            <td class="bottom">
                            </td>
                            <td class="bottom">
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
