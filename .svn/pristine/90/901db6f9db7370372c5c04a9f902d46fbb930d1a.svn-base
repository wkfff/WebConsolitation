<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.CTAT_0001._default" %>

<%@ Register Src="../../../components/HeaderPR.ascx" TagName="HeaderPR" TagPrefix="uc4" %>

<%@ Register Src="../../../components/UserComboBox.ascx" TagName="UserComboBox" TagPrefix="uc3" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Src="../../../components/Header.ascx" TagName="Header" TagPrefix="uc1" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="../../../components/DimensionTree.ascx" TagName="DimensionTree" TagPrefix="uc2" %>


    <asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
        <br />
        <asp:Label ID="TT" runat="server" Text="Label"></asp:Label><br />
        <br />
        <asp:Label ID="TG" runat="server" Text="Label"></asp:Label>&nbsp;<igtbl:UltraWebGrid
            ID="GG" runat="server" EnableAppStyling="True" OnClick="GRID_CLIK" OnDataBinding="G_DataBinding"
            OnInitializeLayout="G_InitializeLayout" StyleSetName="Office2007Blue" SkinID="UltraWebGrid">
            <Bands>
                <igtbl:UltraGridBand>
                    <AddNewRow View="NotSet" Visible="NotSet">
                    </AddNewRow>
                </igtbl:UltraGridBand>
            </Bands>
            <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                HeaderClickActionDefault="SortMulti" Name="GG" RowHeightDefault="20px"
                RowSelectorsDefault="No" StationaryMargins="Header"
                StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy" SelectTypeCellDefault="Extended" SelectTypeColDefault="Extended">
                <GroupByBox Hidden="True">
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
                    BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
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
        <table>
            <tr>
                <td>
        <asp:Label ID="TC" runat="server" Text="Label"></asp:Label></td>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label></td>
            </tr>
            <tr>
                <td>
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server">
        <igchart:UltraChart ID="CC" runat="server" BackgroundImageFileName=""  
             EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
            Version="9.1" OnDataBinding="C_DataBinding" ChartType="AreaChart3D" OnInvalidDataReceived="C_InvalidDataReceived" Transform3D-XRotation="125" Transform3D-YRotation="0">
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" />
            <ColorModel AlphaLevel="255" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear">
            </ColorModel>
            <Effects>
                <Effects>
                    <igchartprop:GradientEffect>
                    </igchartprop:GradientEffect>
                </Effects>
            </Effects>
            <Axis>
                <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                        Orientation="Horizontal" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="Horizontal"
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
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                            VerticalAlign="Center" FormatString="">
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
                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="VerticalLeftFacing"
                            VerticalAlign="Center" FormatString="">
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
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="Horizontal"
                            VerticalAlign="Center" FormatString="">
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
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                            VerticalAlign="Center" FormatString="">
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
            <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_FK0101_2#SEQNUM(100).png" />
        </igchart:UltraChart>
        </igmisc:WebAsyncRefreshPanel>
                </td>
                <td style="vertical-align: top">
                    <igtbl:UltraWebGrid
            ID="BG" runat="server" EnableAppStyling="True" OnDataBinding="BG_DataBinding" StyleSetName="Office2007Blue" SkinID="UltraWebGrid" OnInitializeLayout="BG_InitializeLayout">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1" RowHeightDefault="20px"
                RowSelectorsDefault="No" StationaryMargins="Header"
                StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy" SelectTypeCellDefault="Extended" SelectTypeColDefault="Extended">
                            <GroupByBox Hidden="True">
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
                    BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
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
                    </igtbl:UltraWebGrid></td>
            </tr>
        </table>
    </asp:Content>