<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Oil_0008_0002.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.Oil_0008_0002" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <div style="position: absolute; width: 768px; height: 950px; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: hidden;">
        <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black;
            top: 0px; left: 0px">
            <tr>
                <td align="left" valign="top">
                    <uc1:iPadElementHeader ID="IncomesHeader" runat="server" Text="��������� ���� �� �������������"
                        Width="100%" />
                    <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Height="200px" Width="509px"
                        SkinID="UltraWebGrid">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                            StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                            StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid"
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
                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                </BorderDetails>
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
                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                </BorderDetails>
                            </HeaderStyleDefault>
                            <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                            </EditCellStyleDefault>
                            <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                Font-Names="Microsoft Sans Serif" BackColor="Window" Width="509px" Height="200px">
                            </FrameStyle>
                            <Pager MinimumPagesForDisplay="2">
                                <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                    </BorderDetails>
                                </PagerStyle>
                            </Pager>
                            <AddNewBox Hidden="False">
                                <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                    </BorderDetails>
                                </BoxStyle>
                            </AddNewBox>
                        </DisplayLayout>
                    </igtbl:UltraWebGrid>
                </td>
            </tr>
            <tr>
                <td align="left" valign="top">
                    <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="��������� ��� � ������� ��"
                        Width="100%" />
                    <div style="margin-top: -10px">
                        <igchart:UltraChart ID="UltraChart" runat="server" SkinID="UltraWebColumnChart">
                            <DeploymentScenario FilePath="../../../TemporaryImages/" ImageURL="../../../TemporaryImages/Chart_mfrf01_05_#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </div>
                </td>
            </tr>
        </table>
        <div style="margin-top: 100px">
            <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
        </div>
    </div>
    </form>
</body>
</html>