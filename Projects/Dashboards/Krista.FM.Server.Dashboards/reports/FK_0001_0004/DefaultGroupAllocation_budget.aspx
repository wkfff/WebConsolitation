<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="DefaultGroupAllocation_budget.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FK_0001_0004.DefaultGroupAllocation_budget" %>
 <%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc5" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td colspan="1" style="width: 100%">
                <uc5:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="DefaultGroupAllocation_budget.html" />
                &nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" Text="Label" CssClass="PageTitle"></asp:Label><br/>
                <asp:Label ID="Label2" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" valign="top">
                <uc4:UltraGridExporter ID="UltraGridExporter1" runat="server" /><br />
                <asp:HyperLink ID="CrossLink1" runat="server" SkinID="HyperLink"></asp:HyperLink><br />
                <asp:HyperLink ID="CrossLink2" runat="server" SkinID="HyperLink"></asp:HyperLink>                
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboYear" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboMonth" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboKD" runat="server" />
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboSKIFLevel" runat="server"></uc3:CustomMultiCombo>
            </td>               
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td colspan="2" align="center" valign="bottom">
                <asp:Label ID="Label4" runat="server" CssClass="ElementSubTitle" Text="Группировка субъектов по темпам роста доходов"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="padding-right: 0px;">
                <igchart:UltraChart ID="LeftChart" runat="server" BackgroundImageFileName=""  
                     EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                    OnDataBinding="LeftChart_DataBinding" Version="8.2">
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
                        <Y2 LineThickness="1" TickmarkInterval="50" Visible="False" TickmarkStyle="Smart">
                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                            </MinorGridLines>
                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                            </MajorGridLines>
                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
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
                        <Y LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart">
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
                                HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
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
                                Font="Verdana, 7pt" VerticalAlign="Center">
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
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_FK0104_4_budget_1#SEQNUM(100).png" />
                </igchart:UltraChart>
            </td>
            <td style="padding-left: 0px;">
                <igchart:UltraChart ID="RightChart" runat="server" BackgroundImageFileName=""  
                     EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                    OnDataBinding="RightChart_DataBinding" Version="8.2">
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
                        <Y2 LineThickness="1" TickmarkInterval="50" Visible="False" TickmarkStyle="Smart">
                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                            </MinorGridLines>
                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                            </MajorGridLines>
                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
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
                        <Y LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart">
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
                                HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
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
                                Font="Verdana, 7pt" VerticalAlign="Center">
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
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_FK0104_4_budget_2#SEQNUM(100).png" />
                </igchart:UltraChart>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label3" runat="server" Text="Label" CssClass="ElementSubTitle"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table style="width: 100%;">
                    <tr>
                        <td align="center" style="width: 50%;">
                            <asp:Label ID="TopMaxLabel" runat="server" Text="Label" CssClass="ElementSubTitle"></asp:Label>
                            <igtbl:UltraWebGrid ID="TopMaxGrid" runat="server" EnableAppStyling="True" Height="200px"
                                OnDataBinding="UltraWebGrid_DataBinding" OnInitializeLayout="UltraWebGrid_InitializeLayout"
                                StyleSetName="Office2007Blue" SkinID="UltraWebGrid" Width="325px" OnInitializeRow="UltraWebGrid_InitializeRow">
                                <Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                                </Bands>
                                <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                    StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                    StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1"
                                    BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                    TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer"
                                    SelectTypeRowDefault="Extended">
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
                                    <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window">
                                        <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                        <Padding Left="3px"></Padding>
                                    </RowStyleDefault>
                                    <FilterOptionsDefault>
                                        <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid"
                                            Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White"
                                            CustomRules="overflow:auto;">
                                            <Padding Left="2px"></Padding>
                                        </FilterOperandDropDownStyle>
                                        <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                        </FilterHighlightRowStyle>
                                        <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                            Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px"
                                            Height="300px" CustomRules="overflow:auto;">
                                            <Padding Left="2px"></Padding>
                                        </FilterDropDownStyle>
                                    </FilterOptionsDefault>
                                    <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                    </HeaderStyleDefault>
                                    <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                    </EditCellStyleDefault>
                                    <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window" Width="325px" Height="200px">
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
                        <td align="center" style="width: 50%;">
                            <asp:Label ID="TopMinLabel" runat="server" Text="Label" CssClass="ElementSubTitle"></asp:Label>
                            <igtbl:UltraWebGrid ID="TopMinGrid" runat="server" EnableAppStyling="True" Height="200px"
                                OnDataBinding="UltraWebGrid_DataBinding" OnInitializeLayout="UltraWebGrid_InitializeLayout"
                                StyleSetName="Office2007Blue" SkinID="UltraWebGrid" Width="325px" OnInitializeRow="UltraWebGrid_InitializeRow">
                                <Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                                </Bands>
                                <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                    StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                    StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1"
                                    BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                    TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer"
                                    SelectTypeRowDefault="Extended">
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
                                    <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window">
                                        <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                        <Padding Left="3px"></Padding>
                                    </RowStyleDefault>
                                    <FilterOptionsDefault>
                                        <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid"
                                            Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White"
                                            CustomRules="overflow:auto;">
                                            <Padding Left="2px"></Padding>
                                        </FilterOperandDropDownStyle>
                                        <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                        </FilterHighlightRowStyle>
                                        <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                            Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px"
                                            Height="300px" CustomRules="overflow:auto;">
                                            <Padding Left="2px"></Padding>
                                        </FilterDropDownStyle>
                                    </FilterOptionsDefault>
                                    <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                    </HeaderStyleDefault>
                                    <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                    </EditCellStyleDefault>
                                    <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window" Width="325px" Height="200px">
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
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
