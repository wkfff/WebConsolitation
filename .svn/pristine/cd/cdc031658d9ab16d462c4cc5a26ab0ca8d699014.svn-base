<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FO_0003_0002.Default" Title="Оценка соблюдения норматива расходов на содержание органов государственной власти"%>
    
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc7" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc5" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc6" %>
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
     <asp:Label ID="Label1" CssClass="PageTitle" runat="server"></asp:Label><br/>
     <asp:Label ID="Label2" CssClass="PageSubTitle" runat="server"></asp:Label> 
    </td>
    <td valign="top">
     <uc6:UltraGridExporter ID="UltraGridExporter1" runat="server" />
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
      <td>
    <asp:RadioButtonList ID="DebtKindButtonList" runat="server" AutoPostBack="True"
        RepeatDirection="Horizontal" Width="250px" style="font-family:Verdana; font-size:12px;">
         <asp:ListItem Selected="True">План</asp:ListItem> 
         <asp:ListItem>Факт</asp:ListItem>
     </asp:RadioButtonList>
   </td>   
    <td style="width:100%;" align="right">
     <uc2:GridSearch ID="GridSearch1" runat="server"/>     
    </td>
    
  </tr>
  <tr>
   <td colspan = "2">
     <asp:RadioButtonList ID="MoneyButtonList" runat="server" AutoPostBack="True"
        RepeatDirection="Horizontal" Width="250px" style="font-family:Verdana; font-size:12px;">
         <asp:ListItem Selected="True">Тыс. руб.</asp:ListItem> 
         <asp:ListItem>Млн. руб.</asp:ListItem>
     </asp:RadioButtonList>
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
     <igmisc:WebAsyncRefreshPanel ID="chartsWebAsyncPanel" runat="server"
                                  TriggerControlIds="UltraWebGridFF" style="width:100%;"> 
        <table> 
         <tr>
          <td>
           <table style="border-collapse:collapse; background-color:white; width:100%; margin-top:10px;">
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
             <td class="headerReport"> 
               <asp:Label ID="Label3" runat="server" CssClass="ElementTitle"> </asp:Label>
             </td>
             <td class="headerright">
             </td>
            </tr>
            <tr>
             <td class="left">
             </td>
             <td valign="top" align="right">
              <asp:RadioButtonList ID="DebtKindButtonList2" runat="server" AutoPostBack="True"
                  RepeatDirection="Horizontal" Width="400px" style="font-family:Verdana; font-size:12px;">
               <asp:ListItem Selected="True">абсолютные данные</asp:ListItem> 
               <asp:ListItem>относительные данные</asp:ListItem>
              </asp:RadioButtonList>
            </td>
            <td class="right">
            </td>
            </tr> 
            <tr>
              <td class="left">
              </td>
              <td style="overflow:visible;">
                <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName="" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                    OnDataBinding="UltraChart1_DataBinding" Version="8.2">
                  <Tooltips  Font-Bold="false" Font-Italic="false" Font-Overline="false" Font-Strikeout="false" Font-Underline="false"/>
                  <ColorModel AlphaLevel="150" ColorBegin="DarkGoldenrod" ColorEnd="Navy" ModelStyle="CustomLinear">
                  </ColorModel>
                  <Effects>
                   <Effects>
                    <igchartprop:GradientEffect >
                    </igchartprop:GradientEffect >
                   </Effects>
                  </Effects>
                  <Axis>
                   <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="false" >
                    <MinorGridLines  AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1" Visible="false"/>
                    <MajorGridLines  AlphaLevel="255" Color="gainsboro" DrawStyle="Dot" Thickness="1" Visible="true" />
                    <Labels Font="Verana,12px" FontColor="dimgray" HorizontalAlign="Near"  ItemFormatString="&lt;ITEM_LABEL&gt;"
                            Orientation="Horizontal" VerticalAlign="Center"> 
                      <Layout Behavior="Auto">
                      </Layout>      
                      <SeriesLabels Font="Verdana,12px" FontColor="dimgray" HorizontalAlign="Near"  Orientation="Horizontal"
                       VerticalAlign="Center">
                       <Layout Behavior="auto">
                       </Layout>
                      </SeriesLabels>              
                    </Labels> 
                   </Z>
                   <Y2 LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="false">
                     <MinorGridLines  AlphaLevel="255" Color="lightgray" DrawStyle="Dot" Thickness="1" Visible="false"/>
                     <MajorGridLines  AlphaLevel="255" Color="gainsboro" DrawStyle="Dot" Thickness="1" Visible="true"/>
                     <Labels Font="Verdana, 12px" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                             Orientation="Horizontal" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout> 
                        <SeriesLabels Font="Verdana,12px" FontColor="gray" HorizontalAlign="Near" Orientation="VerticalLeftFacing"
                                      VerticalAlign="Center" FormatString="" >
                        <Layout Behavior="Auto">
                        </Layout>
                        </SeriesLabels>    
                     </Labels>
                   </Y2>
                   <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="true">
                    <MinorGridLines  AlphaLevel="255" Color="lightgray" DrawStyle="Dot" Thickness="1" Visible="false"/>
                    <MajorGridLines  AlphaLevel="255" Color="gainsboro" DrawStyle="Dot" Thickness="1" Visible="true"/>
                    <Labels Font="Verdana,11px" FontColor="dimgray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                            Orientation="VerticalLeftFacing" VerticalAlign="Center">
                       <Layout Behavior="Auto"> 
                       </Layout>     
                       <SeriesLabels Font="Verdana,11px" FontColor="dimgray" HorizontalAlign="Center" Orientation="Horizontal"
                                     VerticalAlign="Center">
                         <Layout Behavior="Auto">
                         </Layout>
                       </SeriesLabels>
                    </Labels>
                   </X>
                   <Y LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="true">
                    <MinorGridLines AlphaLevel="255" Color="lightgray" DrawStyle="Dot" Thickness="1" Visible="false" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1" Visible="true"/>
                    <Labels Font="Verdana, 12px" FontColor="dimgray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" 
                            Orientation="Horizontal" VerticalAlign="Center">
                     <Layout Behavior="Auto">
                     </Layout>
                     <SeriesLabels Font="Verdana, 12px" FontColor="dimgray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                                   VerticalAlign="Center" FormatString="">
                      <Layout Behavior="Auto">
                      </Layout>
                     </SeriesLabels>
                   </Labels>
                   </Y>
                   
                   <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="false">
                    <MinorGridLines  AlphaLevel="255" Color="lightgray" DrawStyle="Dot" Thickness="1" Visible="false"/>
                    <MinorGridLines  AlphaLevel="255" Color="gainsboro" DrawStyle="Dot" Thickness="1" Visible="true"/>
                     <Labels Font="Verdana, 12px" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                             Orientation="VerticalLeftFacing" VerticalAlign="Center">
                      <Layout Behavior="Auto">
                      </Layout>
                      <SeriesLabels  Font="Verdana, 12px" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                                      VerticalAlign="Center" >
                       <Layout Behavior="Auto">
                       </Layout>
                      </SeriesLabels>
                     </Labels>
                   </X2>
                   <PE  ElementType="None" Fill="Cornsilk"/>
                   <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="false">
                    <MinorGridLines  AlphaLevel="255" Color="lightgray" DrawStyle="Dot" Thickness="1" Visible="false"/>
                    <MajorGridLines  AlphaLevel="255" Color="lightgray" DrawStyle="Dot" Thickness="1" Visible="true" />
                    <Labels Font="Verdana, 12px" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                            Orientation="Horizontal"  VerticalAlign="Center">
                     <Layout Behavior="Auto">
                     </Layout>
                     <SeriesLabels Font="Verdana, 12px" FontColor="Gray" HorizontalAlign="Near"
                            Orientation="Horizontal"  VerticalAlign="Center">
                      <Layout Behavior="Auto">
                      </Layout>
                     </SeriesLabels>
                    </Labels>
                   </Z2>
                  </Axis>
                  <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_MFRF0105#SEQNUM(100).png"/> 
                  <Border Thickness="0" />
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
             <td class="bottomright">
             </td>
            </tr>
           </table>
          </td>
         </tr>                         
        </table>
     </igmisc:WebAsyncRefreshPanel >   
   </td>
  </tr>
 </table>
</asp:Content>
