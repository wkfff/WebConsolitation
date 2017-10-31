<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.FK_0001_0009.FK_0001_0009Form" Title="Картограмма: расходы по субъектам РФ" %>

<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc2" %>

<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>

<%@ Register Src="../../Components/CustomComboList.ascx" TagName="CustomComboList"
    TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <div>
            <table style="width: 655px">
                <tr>
                    <td colspan="3">
                        <asp:Label ID="LabelTitle" runat="server" SkinID="PageTitle"></asp:Label></td>
                    <td style="width: 3px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 19px; height: 24px">
                        <asp:Label ID="LabelParamYear" runat="server" Text="Год" SkinID="ParamChooseTitle"></asp:Label></td>
                    <td style="width: 168px; height: 24px">
                        <uc1:CustomComboList ID="ComboYear" runat="server" />
                    </td>
                    <td style="width: 167px; height: 24px">
                    </td>
                    <td style="width: 3px; height: 24px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 19px; height: 23px;">
                        <asp:Label ID="LabelParamMonth" runat="server" Text="Месяц" SkinID="ParamChooseTitle"></asp:Label></td>
                    <td style="width: 168px; height: 23px;">
                        <uc1:CustomComboList ID="ComboMonth" runat="server" />
                    </td>
                    <td style="width: 167px; height: 23px;">
                    </td>
                    <td style="width: 3px; height: 23px;">
                    </td>
                </tr>
                <tr>
                    <td style="width: 19px">
                        <asp:Label ID="LabelParamMap" runat="server" Text="Карта" SkinID="ParamChooseTitle"></asp:Label></td>
                    <td style="width: 168px">
                        <uc1:CustomComboList ID="ComboMap" runat="server" />
                    </td>
                    <td style="width: 167px">
                    </td>
                    <td style="width: 3px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 19px; height: 24px">
                        <asp:Label ID="LabelParamMeasure" runat="server" Text="Показатель" SkinID="ParamChooseTitle"></asp:Label></td>
                    <td style="width: 168px; height: 24px">
                        <uc1:CustomComboList ID="ComboMeasure" runat="server" />
                    </td>
                    <td style="width: 167px; height: 24px">
                    </td>
                    <td style="width: 3px; height: 24px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 19px">
                        <asp:Label ID="LabelParamClassifier" runat="server" SkinID="ParamChooseTitle" Text="Вид расходов" Width="125px"></asp:Label></td>
                    <td style="width: 168px">
                        <uc1:CustomComboList ID="ComboClassifier" runat="server" />
                    </td>
                    <td style="width: 167px">
                    </td>
                    <td style="width: 3px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 19px">
                        &nbsp;<uc2:RefreshButton ID="RefreshButton1" runat="server" />
                    </td>
                    <td style="width: 168px">
                    </td>
                    <td style="width: 167px">
                    </td>
                    <td style="width: 3px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 19px">
                    </td>
                    <td style="width: 168px">
                    </td>
                    <td style="width: 167px">
                    </td>
                    <td style="width: 3px">
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        &nbsp;<DMWC:MapControl ID="map" runat="server" BackColor="White" BackGradientType="TopBottom"
                            BackSecondaryColor="249, 249, 240" BorderLineColor="DarkOrange" BorderLineWidth="2"
                            GridUnderContent="True" Height="323px" ImageUrl="../../TemporaryImages/MapPic_#SEQ(300,3)"
                            OnClick="map_Click" RenderingImageUrl="../../TemporaryImages/"
                            ResourceKey="#MapControlResKey#MapControl1#" Width="640px">
                            <Frame FrameStyle="Emboss" />
                            <NavigationPanel BackColor="150, 255, 255, 255" BackSecondaryColor="150, 224, 238, 208"
                                BackShadowOffset="0" BorderColor="185, 206, 102" ButtonBorderColor="88, 95, 41"
                                Dock="Right" SymbolBorderColor="88, 95, 41" Visible="True">
                                <Location X="83.21413" Y="3.87477636" />
                                <Size Height="90" Width="90" />
                            </NavigationPanel>
                            <Viewport AutoSize="False" BackColor="" BorderColor="Gray">
                                <Location X="1.95434761" Y="3.87477636" />
                                <Size Height="90.17028" Width="95.19531" />
                            </Viewport>
                            <Parallels LabelColor="88, 95, 41" LineColor="Silver" Visible="False" />
                            <ShapeRules>
                                <DMWC:ShapeRule Category="2" ColorCount="1" ColoringMode="ColorRange" FromColor="RoyalBlue"
                                    MiddleColor="RoyalBlue" Name="ShapeRule1" ToColor="RoyalBlue">
                                    <CustomColors>
                                        <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                        <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                        <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                        <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                        <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                        <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                        <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                        <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                        <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                        <DMWC:CustomColor Color="RoyalBlue" VisibleInt="True" />
                                    </CustomColors>
                                </DMWC:ShapeRule>
                            </ShapeRules>
                            <ZoomPanel BackColor="150, 255, 255, 255" BackSecondaryColor="150, 224, 238, 208"
                                BackShadowOffset="0" BorderColor="185, 206, 102" ButtonBorderColor="88, 95, 41"
                                Dock="Right" SliderBarColor="88, 95, 41" SnapToTickMarks="True" SymbolBorderColor="48, 48, 48"
                                ThumbBorderColor="88, 95, 41" TickBorderColor="88, 95, 41" Visible="True">
                                <Location X="91.03886" Y="31.8250866" />
                                <Size Height="200" Width="40" />
                            </ZoomPanel>
                            <Meridians LabelColor="88, 95, 41" LineColor="Silver" Visible="False" />
                            <ColorSwatchPanel BackColor="150, 255, 255, 255" BackSecondaryColor="150, 224, 238, 208"
                                BackShadowOffset="0" BorderColor="185, 206, 102" DockedInsideViewport="False"
                                LabelColor="88, 95, 41" TitleColor="88, 95, 41">
                                <Size Height="80" Width="180" />
                                <Location X="2.52219772" Y="73.6541443" />
                            </ColorSwatchPanel>
                            <ShapeFields>
                                <DMWC:Field Name="ID" Type="System.Double" />
                            </ShapeFields>
                            <DistanceScalePanel BackColor="150, 255, 255, 255" BackSecondaryColor="150, 224, 238, 208"
                                BackShadowOffset="0" BorderColor="185, 206, 102" Dock="Right" LabelColor="88, 95, 41">
                                <Location X="76.9543457" Y="77.24434" />
                                <Size Height="55" Width="130" />
                            </DistanceScalePanel>
                            <Labels>
                                <DMWC:MapLabel Dock="Bottom" DockAlignment="Near" Name="MapLabelMeasures" Text=""
                                    TextAlignment="BottomLeft">
                                    <Location X="1.95434761" Y="64.20086" />
                                    <Size Height="30" Width="100" />
                                </DMWC:MapLabel>
                            </Labels>
                        </DMWC:MapControl>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/reports/FK_0001_0008/Default.aspx"
                            SkinID="HyperLink" Width="301px">Картограмма: доходы по субъектам РФ</asp:HyperLink></td>
                    <td style="width: 167px">
                    </td>
                    <td style="width: 3px">
                    </td>
                </tr>
            </table>
        </div>

    </div>
</asp:Content>
