<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FO0201Gadget.ascx.cs" Inherits="Krista.FM.Server.Dashboards.reports.Dashboard.FO0201Gadget" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v9.1, Version=9.1.20091.2013, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<div class="GadgetTopDate">на 1 июля 2008 года</div>
<asp:Panel ID="MainPanel" runat="server" CssClass="GadgetMainPanel" Height="190px" Width="99%">
<igtbl:UltraWebGrid ID="Grid" runat="server" EnableAppStyling="True" Height="100%" StyleSetName="Office2007Blue" Width="100%" SkinID="GadgetGrid">
    <Bands>
        <igtbl:UltraGridBand AllowAdd="No">
            <HeaderStyle Wrap="True" />
            <AddNewRow View="NotSet" Visible="NotSet">
            </AddNewRow>
            <RowStyle Wrap="True" />
            <Columns>
                <igtbl:UltraGridColumn Width="130px">
                    <Header Caption="Показатели">
                    </Header>
                    <CellStyle BackColor="#EFF3FB">
                    </CellStyle>
                </igtbl:UltraGridColumn>
                <igtbl:UltraGridColumn Width="70px">
                    <Header Caption="Среднее по районам и ГО">
                        <RowLayoutColumnInfo OriginX="1" />
                    </Header>
                    <Footer>
                        <RowLayoutColumnInfo OriginX="1" />
                    </Footer>
                    <CellStyle HorizontalAlign="Right">
                        <Padding Right="10px" />
                    </CellStyle>
                </igtbl:UltraGridColumn>
                <igtbl:UltraGridColumn Width="80px">
                    <Header Caption="Минимум">
                        <RowLayoutColumnInfo OriginX="2" />
                    </Header>
                    <Footer>
                        <RowLayoutColumnInfo OriginX="2" />
                    </Footer>
                    <CellStyle BackColor="White" BackgroundImage="~\images\CornerRed.gif" CustomRules="background-repeat: no-repeat; background-position: top right;">
                    </CellStyle>
                </igtbl:UltraGridColumn>
                <igtbl:UltraGridColumn Width="85px">
                    <Header Caption="Максимум">
                        <RowLayoutColumnInfo OriginX="3" />
                    </Header>
                    <Footer>
                        <RowLayoutColumnInfo OriginX="3" />
                    </Footer>
                    <CellStyle BackColor="White" BackgroundImage="~\images\CornerGreen.gif" CustomRules="background-repeat: no-repeat; background-position: top right;">
                    </CellStyle>
                </igtbl:UltraGridColumn>
            </Columns>
        </igtbl:UltraGridBand>
    </Bands>
    <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
        AllowSortingDefault="OnClient" BorderCollapseDefault="Separate"
        HeaderClickActionDefault="SortMulti" Name="ctl00xGrid" RowHeightDefault="23px"
        RowSelectorsDefault="No" SelectTypeRowDefault="Extended" StationaryMargins="Header"
        StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00"
        ReadOnly="LevelZero">
        <FrameStyle BorderColor="InactiveCaption" BorderStyle="Solid"
            BorderWidth="0px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="100%"
            Width="100%">
        </FrameStyle>
        <Pager MinimumPagesForDisplay="2">
            <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
            </PagerStyle>
        </Pager>
        <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
        </EditCellStyleDefault>
        <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
        </FooterStyleDefault>
        <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
        </HeaderStyleDefault>
        <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
            Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
            <Padding Left="3px" />
            <BorderDetails ColorLeft="Window" ColorTop="Window" />
        </RowStyleDefault>
        <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
        </GroupByRowStyleDefault>
        <GroupByBox Hidden="True">
            <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
            </BoxStyle>
        </GroupByBox>
        <AddNewBox>
            <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
            </BoxStyle>
        </AddNewBox>
        <ActivationObject BorderColor="" BorderWidth="">
        </ActivationObject>
        <FilterOptionsDefault>
            <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                Font-Size="11px" Height="300px" Width="200px">
                <Padding Left="2px" />
            </FilterDropDownStyle>
            <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
            </FilterHighlightRowStyle>
            <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid"
                BorderWidth="1px" CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                Font-Size="11px">
                <Padding Left="2px" />
            </FilterOperandDropDownStyle>
        </FilterOptionsDefault>
    </DisplayLayout>
    <Rows>
        <igtbl:UltraGridRow Height="">
            <Cells>
                <igtbl:UltraGridCell Text="Исполнение бюджета по доходам">
                </igtbl:UltraGridCell>
                <igtbl:UltraGridCell Text="abc">
                </igtbl:UltraGridCell>
                <igtbl:UltraGridCell Text="abc">
                </igtbl:UltraGridCell>
                <igtbl:UltraGridCell Text="abc">
                </igtbl:UltraGridCell>
            </Cells>
        </igtbl:UltraGridRow>
        <igtbl:UltraGridRow Height="">
            <Cells>
                <igtbl:UltraGridCell Text="Исполнение бюджета по расходам">
                </igtbl:UltraGridCell>
                <igtbl:UltraGridCell Text="abc">
                </igtbl:UltraGridCell>
                <igtbl:UltraGridCell Text="abc">
                </igtbl:UltraGridCell>
                <igtbl:UltraGridCell Text="abc">
                </igtbl:UltraGridCell>
            </Cells>
        </igtbl:UltraGridRow>
        <igtbl:UltraGridRow Height="">
            <Cells>
                <igtbl:UltraGridCell Text="Коэффициент бюджетной обеспеченности населения, тыс.руб. / чел">
                </igtbl:UltraGridCell>
                <igtbl:UltraGridCell Text="abc">
                </igtbl:UltraGridCell>
                <igtbl:UltraGridCell Text="abc">
                </igtbl:UltraGridCell>
                <igtbl:UltraGridCell Text="abc">
                </igtbl:UltraGridCell>
            </Cells>
        </igtbl:UltraGridRow>
        <igtbl:UltraGridRow Height="">
            <Cells>
                <igtbl:UltraGridCell Text="Среднедушевые доходы, тыс.руб. / чел">
                </igtbl:UltraGridCell>
                <igtbl:UltraGridCell Text="abc">
                </igtbl:UltraGridCell>
                <igtbl:UltraGridCell Text="abc">
                </igtbl:UltraGridCell>
                <igtbl:UltraGridCell Text="abc">
                </igtbl:UltraGridCell>
            </Cells>
        </igtbl:UltraGridRow>
    </Rows>
</igtbl:UltraWebGrid></asp:Panel>
