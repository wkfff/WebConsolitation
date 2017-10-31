<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs"Inherits="Krista.FM.Server.Dashboards.reports.CTAT.CTAT_0013._0001._default" %>


<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>


<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>


<asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
<script language="javascript" type="text/javascript">
// <!CDATA[

function Button1_onclick() {

}

// ]]>
</script>

    <table>
        <tr>
            <td>
                <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Страница 1" /></td>
            <td>
                <asp:Button ID="Button2" runat="server" Enabled="False" Text="Страница 2" /></td>
        </tr>
    </table>

    <br />

    <table>
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" Text="c"></asp:Label>&nbsp;
                <asp:DropDownList id ="D1" runat =server></asp:DropDownList>&nbsp;
            </td>
            <td>
                &nbsp;<asp:Label ID="Label2" runat="server" Text="по"></asp:Label>&nbsp;
                <asp:DropDownList ID="D2" runat="server">
                </asp:DropDownList>
                &nbsp;
            </td>
            <td>
                <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Обновить" /></td>
        </tr>
    </table>
    <br />
    <igmisc:WebAsyncRefreshPanel ID ="WebAsyncRefreshPanel1" runat =server>
        <asp:Label ID="Label3" runat="server" Text="Задолженность организаций"></asp:Label><br />
    <igtbl:UltraWebGrid ID="G" runat =server OnDataBinding="G_DataBinding" StyleSetName="Office2007Blue" OnInitializeLayout="G_InitializeLayout" SkinID="UltraWebGrid" Height="200px" Width="325px">
        <Bands>
            <igtbl:UltraGridBand>
                <AddNewRow View="NotSet" Visible="NotSet">
                </AddNewRow>
            </igtbl:UltraGridBand>
        </Bands>
        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
            AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
            HeaderClickActionDefault="SortMulti" Name="G" RowHeightDefault="20px" RowSelectorsDefault="No" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
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
    </igtbl:UltraWebGrid><br />
    </igmisc:WebAsyncRefreshPanel>
    <asp:Label ID="Label4" runat="server" Text="Динамика темпа роста задолженности организаций в % к аналогичному периоду предыдущего года"></asp:Label><br />
    <igmisc:WebAsyncRefreshPanel ID ="PG" runat =server LinkedRefreshControlID="WebAsyncRefreshPanel1">
    <igchart:UltraChart ID = "C" runat ="server" OnDataBinding="C_DataBinding" BackgroundImageFileName=""   EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" Version="9.1" OnInvalidDataReceived="C_InvalidDataReceived">
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
                    Orientation="Horizontal" VerticalAlign="Center">
                    <Layout Behavior="Auto">
                    </Layout>
                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
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
                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="VerticalLeftFacing"
                        VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                    </SeriesLabels>
                </Labels>
            </Y2>
            <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="150">
                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                    Visible="False" />
                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                    Visible="True" />
                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="VerticalLeftFacing"
                        VerticalAlign="Center">
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
                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                        VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                    </SeriesLabels>
                </Labels>
            </Z2>
        </Axis>
        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
    </igchart:UltraChart>

 </igmisc:WebAsyncRefreshPanel>

 </asp:Content>
