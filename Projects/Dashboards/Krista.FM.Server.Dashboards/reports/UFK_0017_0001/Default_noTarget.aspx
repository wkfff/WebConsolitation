<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default_noTarget.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.UFK_0017_0001.Default_noTarget" Title="Остатки на счете"  %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

    <asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
    <div>
    <table>
        <tr><td><table><tr>
        <td style="font-family: Verdana; font-size:medium">
        <b><nobr><asp:Label id="lbTitle" runat="server" Text="Label" CssClass="PageTitle"></asp:Label></nobr></b></td>
        <td colspan="1" valign="middle" style="width: 60px" align="center">
                                                <nobr><a href="Default_target.aspx" style="font-size: 10px; font-family: Verdana">Остаток целевых средств</a></nobr></td>
                             </tr></table></td>
                                         
            
                             </tr>
    <tr> 
    <td>
        <table>
            <tr><td valign="top">
                <asp:Label ID="Label1" runat="server" CssClass="ParamChooseTitle" Text="Месяц"></asp:Label><br />
                <igcmbo:WebCombo ID="comboMonth" runat="server" BackColor="White" BorderColor="Silver"
                    BorderStyle="Solid" BorderWidth="1px" EnableAppStyling="False" Font-Names="verdana"
                    Font-Size="8pt" ForeColor="Black" Height="20px" SelBackColor="White" SelectedIndex="7"
                    SelForeColor="Black" StyleSetName="Office2007Blue" TabIndex="1" Version="4.00"
                    Width="130px">
                    <Columns>
                        <igtbl:UltraGridColumn Width="80px">
                            <selectedcellstyle bordercolor="Silver" borderstyle="Solid" borderwidth="1px"></selectedcellstyle>
                            <header caption="Уровень детализации" key="mode"></header>
                            <cellstyle backcolor="White" bordercolor="White" borderstyle="Solid" borderwidth="1px"
                                font-names="verdana" font-size="X-Small" width="125px"></cellstyle>
                        </igtbl:UltraGridColumn>
                    </Columns>
                    <ExpandEffects ShadowColor="LightGray" />
                    <DropDownLayout ColHeadersVisible="No" DropdownHeight="250px" DropdownWidth="128px"
                        GridLines="None" RowHeightDefault="17px" RowSelectors="NotSet" Version="4.00"
                        XmlLoadOnDemandType="Synchronous">
                        <RowAlternateStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px">
                        </RowAlternateStyle>
                        <RowSelectorStyle BackColor="#E4ECF7" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                            Font-Bold="False" Font-Names="verdana" Font-Size="X-Small">
                            <BorderDetails StyleLeft="None" />
                        </RowSelectorStyle>
                        <FrameStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                            Font-Size="X-Small" Height="250px" Width="128px">
                        </FrameStyle>
                        <RowStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px"
                            Font-Names="verdana" Font-Size="X-Small" Height="20px" Width="130px">
                            <BorderDetails StyleBottom="None" StyleLeft="None" StyleRight="None" StyleTop="None" />
                        </RowStyle>
                        <SelectedRowStyle BackColor="LightSteelBlue" BorderColor="White" BorderStyle="Solid"
                            BorderWidth="1px" />
                    </DropDownLayout>
                    <Rows>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="Январь"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="Февраль"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="Март"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="Апрель"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="Май"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="Июнь"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="Июль"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="Август"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="Сентябрь"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="Октябрь"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="Ноябрь"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                        <igtbl:UltraGridRow Height="">
                            <cells>
<igtbl:UltraGridCell Text="Декабрь"></igtbl:UltraGridCell>
</cells>
                        </igtbl:UltraGridRow>
                    </Rows>
                </igcmbo:WebCombo>
            </td>
                <td valign="top">
                    <asp:Label ID="Label2" runat="server" CssClass="ParamChooseTitle" Text="Год"></asp:Label><br />
                    <igcmbo:WebCombo ID="comboYear" runat="server" BackColor="White" BorderColor="Silver"
                        BorderStyle="Solid" BorderWidth="1px" EnableAppStyling="False" Font-Names="verdana"
                        Font-Size="8pt" ForeColor="Black" Height="20px" SelBackColor="White" SelForeColor="Black"
                        StyleSetName="Office2007Blue" TabIndex="1" Version="4.00" Width="97px">
                        <Columns>
                            <igtbl:UltraGridColumn Width="30px">
                                <selectedcellstyle bordercolor="Silver" borderstyle="Solid" borderwidth="1px"></selectedcellstyle>
                                <header caption="Уровень детализации" key="mode"></header>
                                <cellstyle backcolor="White" bordercolor="White" borderstyle="Solid" borderwidth="1px"
                                    font-names="verdana" font-size="X-Small" width="125px"></cellstyle>
                            </igtbl:UltraGridColumn>
                        </Columns>
                        <ExpandEffects ShadowColor="LightGray" />
                        <DropDownLayout ColHeadersVisible="No" DropdownHeight="260px" DropdownWidth="80px"
                            GridLines="None" RowHeightDefault="17px" RowSelectors="NotSet" Version="4.00"
                            XmlLoadOnDemandType="Synchronous">
                            <RowAlternateStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px">
                            </RowAlternateStyle>
                            <RowSelectorStyle BackColor="#E4ECF7" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                Font-Bold="False" Font-Names="verdana" Font-Size="X-Small">
                                <BorderDetails StyleLeft="None" />
                            </RowSelectorStyle>
                            <FrameStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                                Font-Size="X-Small" Height="260px" Width="80px">
                            </FrameStyle>
                            <RowStyle BackColor="White" BorderColor="White" BorderStyle="Solid" BorderWidth="1px"
                                Font-Names="verdana" Font-Size="X-Small" Height="20px" Width="130px">
                                <BorderDetails StyleBottom="None" StyleLeft="None" StyleRight="None" StyleTop="None" />
                            </RowStyle>
                            <SelectedRowStyle BackColor="LightSteelBlue" BorderColor="White" BorderStyle="Solid"
                                BorderWidth="1px" />
                        </DropDownLayout>
                        <Rows>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="1998"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="1999"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="2000"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="2001"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="2002"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="2003"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="2004"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="2005"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="2006"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="2007"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="2008"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="2009"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                            <igtbl:UltraGridRow Height="">
                                <cells>
<igtbl:UltraGridCell Text="2010"></igtbl:UltraGridCell>
</cells>
                            </igtbl:UltraGridRow>
                        </Rows>
                    </igcmbo:WebCombo>
                </td>
                <td valign="top">
                    <br />
                    <igtxt:WebImageButton ID="SubmitButton" runat="server" Height="20px" ImageTextSpacing="2" Text="Обновить данные">
                        <Appearance>
                            <Style BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"></Style>
                        </Appearance>
                        <PressedAppearance>
                            <Style Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False"></Style>
                        </PressedAppearance>
                        <DisabledAppearance>
                            <Style BorderColor="Control"></Style>
                        </DisabledAppearance>
                        <HoverAppearance>
                            <Style Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False"></Style>
                        </HoverAppearance>
                        <FocusAppearance>
                            <Style Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False"></Style>
                        </FocusAppearance>
                    </igtxt:WebImageButton>
                </td>
                 <td>
                    <nobr><asp:Label id="lbAvg" runat="server" Text="Label" CssClass="ParamChooseTitle"></asp:Label></nobr><br/>
                    <nobr><asp:Label id="lbMax" runat="server" Text="Label" CssClass="ParamChooseTitle"></asp:Label></nobr><br/>
                    <nobr><asp:Label id="lbMin" runat="server" Text="Label" CssClass="ParamChooseTitle"></asp:Label></nobr>
                    </td>
            </tr>
        </table>             
        <table>            
            <tr>
                <td>
                    <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" EnableAppStyling="True" Height="200px"
                        OnDataBinding="UltraWebGrid_DataBinding" OnInitializeLayout="UltraWebGrid_InitializeLayout"
                        StyleSetName="Office2007Blue" Width="400px" SkinID="UltraWebGrid">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                            HeaderClickActionDefault="SortMulti" Name="UltraWebGrid" RowHeightDefault="20px"
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
                        </DisplayLayout>
                    </igtbl:UltraWebGrid></td>
            </tr>
            <tr>
                <td>
                    <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName=""  
                         EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                        Height="365px" OnDataBinding="UltraChart_DataBinding" Version="7.3" ChartType="LineChart" OnFillSceneGraph="UltraChart1_FillSceneGraph">
                        <Tooltips Font-Names="Verdana" FormatString="&lt;ITEM_LABEL&gt;: &lt;DATA_VALUE:N2&gt; тыс. руб."
                            HotTrackingFillColor="Yellow" HotTrackingOutlineColor="Black" />
                        <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo32_01_#SEQNUM(100).png" />
                        <ColorModel AlphaLevel="150">
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
                                    Orientation="Horizontal" VerticalAlign="Center">
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
                                    Orientation="Horizontal" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </Y2>
                            <X Extent="30" LineThickness="1" TickmarkInterval="1" TickmarkStyle="Smart" Visible="True">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                    Orientation="Horizontal" VerticalAlign="Center">
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
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                        Orientation="Horizontal" VerticalAlign="Center">
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
                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                                    Orientation="Horizontal" VerticalAlign="Center">
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
                        <Legend Font="Verdana, 7.8pt" Location="Bottom" SpanPercentage="22" Visible="True"></Legend>
                    </igchart:UltraChart>
                </td>
            </tr>
            </table>
      </td>
      </tr>
    </table>
    </div>
    </asp:Content>
