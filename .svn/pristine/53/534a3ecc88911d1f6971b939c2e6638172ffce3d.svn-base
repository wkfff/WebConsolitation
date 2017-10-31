<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FO_0001_0002.Default" Title="Численность работников государственных органов" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter" TagPrefix="uc2" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo" TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc9" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc8" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td style= "width: 100%">
                <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" Visible="true" />
                &nbsp;&nbsp;
                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label><br />
                <asp:Label ID="PageSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
               
            </td>
            <td align="right" rowspan="2" style="width: 100%;">
                <uc9:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />&nbsp;<uc8:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
                <br />
                <asp:HyperLink ID="CrossLink1" runat="server" SkinID="HyperLink"></asp:HyperLink>
                <br/>
                <asp:HyperLink ID="CrossLink2" runat="server" SkinID="HyperLink"></asp:HyperLink>
                <br/>
                <asp:HyperLink ID="CrossLink3" runat="server" SkinID="HyperLink"></asp:HyperLink>
                <br/>
                <asp:HyperLink ID="CrossLink4" runat="server" SkinID="HyperLink"></asp:HyperLink>
            </td>
       </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboPeriod" runat="server"></uc3:CustomMultiCombo>
                        </td>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboQuarter" runat="server"></uc3:CustomMultiCombo>
                        </td>                        
                        <td valign="top">
                            <uc1:RefreshButton ID="RefreshButton1" runat="server" />
                        </td>
                                             
                    </tr>
                 
                </table>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td valign="top" align="left" style="padding-right: 6px; padding-right:3px">
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
                        <asp:Label ID="gridCaptionElement" runat="server" CssClass="ElementTitle"></asp:Label>
                            <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Height="200px" Width="325px" OnDataBinding="UltraWebGrid_DataBinding"
                                OnInitializeLayout="UltraWebGrid_InitializeLayout"  OnInitializeRow="UltraWebGrid_InitializeRow" SkinID="UltraWebGrid">
                                <Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                                </Bands>
                                <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient" StationaryMargins="Header"
                                    AllowColSizingDefault="Free" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti"
                                    Name="UltraWebGrid1" BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                    TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer" SelectTypeRowDefault="Extended">
                                    <GroupByBox>
                                        <BoxStyle BorderColor="Window" BackColor="ActiveBorder">
                                        </BoxStyle>
                                    </GroupByBox>
                                    <GroupByRowStyleDefault BorderColor="Window" BackColor="Control">
                                    </GroupByRowStyleDefault>
                                    <ActivationObject BorderWidth="" BorderColor="">
                                    </ActivationObject>
                                    <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                    </FooterStyleDefault>
                                    <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif"
                                        BackColor="Window">
                                        <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                        <Padding Left="3px"></Padding>
                                    </RowStyleDefault>
                                    <FilterOptionsDefault>
                                        <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                            Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" CustomRules="overflow:auto;">
                                            <Padding Left="2px"></Padding>
                                        </FilterOperandDropDownStyle>
                                        <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                        </FilterHighlightRowStyle>
                                        <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                                            BackColor="White" Width="200px" Height="300px" CustomRules="overflow:auto;">
                                            <Padding Left="2px"></Padding>
                                        </FilterDropDownStyle>
                                    </FilterOptionsDefault>
                                    <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                    </HeaderStyleDefault>
                                    <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                    </EditCellStyleDefault>
                                    <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif"
                                        BackColor="Window" Width="325px" Height="200px">
                                    </FrameStyle>
                                    <Pager MinimumPagesForDisplay="2">
                                        <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                        </PagerStyle>
                                    </Pager>
                                    <AddNewBox Hidden="False">
                                        <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
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
               <asp:Label ID="chart1ElementCaption" runat="server" CssClass="ElementTitle"> </asp:Label>
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
               <asp:ListItem Selected="True">Утверждено на дату</asp:ListItem> 
               <asp:ListItem>Фактически на дату</asp:ListItem>
              </asp:RadioButtonList>
            </td>
            <td class="right">
            </td>
            </tr> 
            <tr>
              <td class="left">
              </td>
              <td style="overflow:visible;">
                <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName="" Border-Color="black"
                                     EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
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
