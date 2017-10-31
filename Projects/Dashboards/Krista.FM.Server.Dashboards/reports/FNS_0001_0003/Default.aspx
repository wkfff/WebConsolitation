<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.FNS_0001_0003.Default" Title="Данные 410 по районам и поселениям"  %>

<%@ Register Src="../../components/Header.ascx" TagName="Header" TagPrefix="uc2" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../components/DimensionTree.ascx" TagName="DimensionTree" TagPrefix="uc1" %>

<asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
    <div>
    
    <table>
     <tr> 
        <td>
         <table width="100%"><tr><td>
             <asp:Label ID="Label1" runat="server" SkinID="PageTitle" Text="Информация по налоговым доходам ФНС в разрезе районов и поселений"></asp:Label></td>
                              <td align="right">
                           <a href="../FNS_0001_0002/Default_FNS.aspx" style="font-size: 10px; font-family: Verdana" runat="server" id="A1">по&nbsp;КД&nbsp;и&nbsp;уровням&nbsp;бюджета</a>
                           <br/>
                           <a href="../FNS_0001_0004/Default.aspx" style="font-size: 10px; font-family: Verdana" runat="server" id="A2">по&nbsp;показателям</a>
                           <br/>
                           <a href="../FNS_0001_0005/DefaultOKVD.aspx" style="font-size: 10px; font-family: Verdana" runat="server" id="A3">по&nbsp;ОКВЭД</a>
                           <br/>
                        </td>
                </tr>         
         </table>
        </td>
     </tr> 
     <tr>
       <td>
         <table>
           <tr>
             <td>
                <igmisc:WebPanel ID="WebPanel" runat="server" EnableAppStyling="True" StyleSetName="Office2007Blue" Expanded="False">
                <Template>
                <table>
                   <tr>
                      <td>Период.Месяц</td>
                      <td>Уровни бюджетов</td>
                      <td>ФНС 28н Показатели</td>
                      <td>КД.Сопоставимый</td>
                   </tr>
                   <tr>
                      <td>
                          <uc1:DimensionTree ID="Period" runat="server" CubeName="ФНС_28н_с расщеплением"
                                                DefaultMember="[Период].[Месяц].[Данные всех периодов].[2008].[Полугодие 1].[Квартал 2].[Май]"
                                                HierarchyName="[Период].[Месяц]" ProviderKind="Primary"
                                                MultipleChoice="false" EnableTheming="true" Height="300" Width="250"/>
                      </td>
                      <td>                         

                          <uc1:DimensionTree ID="Level" runat="server" CubeName="ФНС_28н_с расщеплением"
                                                DefaultMember="[Уровни бюджетов].[Все уровни].[Конс.бюджет субъекта].[Конс.бюджет МО]"
                                                HierarchyName="[Уровни бюджетов]" ProviderKind="Primary"
                                                MultipleChoice="false" EnableTheming="true" Height="300" Width="250"/>

                      </td>
                      <td>
                          <uc1:DimensionTree ID="Indicator" runat="server" CubeName="ФНС_28н_с расщеплением"
                                                DefaultMember="[ФНС_28н_Показатели].[Все показатели].[Начислено_Расчеты текущего периода]"
                                                HierarchyName="[ФНС_28н_Показатели]" ProviderKind="Primary"
                                                MultipleChoice="false" EnableTheming="true" Height="300" Width="250"/>

                      </td>
                       <td>
                          <uc1:DimensionTree ID="KD" runat="server" CubeName="ФНС_28н_с расщеплением"
                                                DefaultMember="[КД].[Сопоставимый].[Все коды доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ]"
                                                HierarchyName="[КД].[Сопоставимый]" ProviderKind="Primary"
                                                MultipleChoice="false" EnableTheming="true" Height="300" Width="250"/>

                      </td>
                   </tr>
                 </table>
                </Template>
                </igmisc:WebPanel>              
             </td>
             <td valign="top">
                 <igtxt:WebImageButton ID="SubmitButton" runat="server" Height="20px" ImageTextSpacing="2"
                        Text="Обновить данные" OnClick="SubmitButton_Click">
                                                <DisabledAppearance>
                            <Style BorderColor="Control"></Style>
                        </DisabledAppearance>
                        <Appearance>
                            <Style BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"></Style>
                        </Appearance>
                        <HoverAppearance>
                            <Style Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False"></Style>
                        </HoverAppearance>
                        <FocusAppearance>
                            <Style Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False"></Style>
                        </FocusAppearance>
                        <PressedAppearance>
                            <Style Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False"></Style>
                        </PressedAppearance>
                 </igtxt:WebImageButton>
            </td>      
           </tr>
         </table>       
       </td>
     </tr>
     <tr>
      <td>
        <table>
            <tr>
            <td valign="top" >
                    <igtbl:UltraWebGrid ID="uwGrid" runat="server" EnableAppStyling="True"
                        Height="233px" Width="600px" StyleSetName="Office2007Blue" OnDataBinding="uwGrid_DataBinding" OnInitializeLayout="uwGrid_InitializeLayout" OnInitializeRow="uwGrid_InitializeRow">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                            ColWidthDefault="150px" HeaderClickActionDefault="SortMulti" Name="uwGrid"
                            RowHeightDefault="20px" SelectTypeRowDefault="Extended" StationaryMargins="Header"
                            StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
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
                            <FilterOptionsDefault AllowRowFiltering="OnClient">
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
                            <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left"
                                Wrap="True">
                                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                            </HeaderStyleDefault>
                            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                            </EditCellStyleDefault>
                            <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
                                BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="233px"
                                Width="620px">
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
            </td> 
            </tr>
        </table>
      </td>
     </tr>     
    </table>
    
    </div>
    </asp:Content>
