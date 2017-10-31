<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GridSearch.ascx.cs" Inherits="Krista.FM.Server.Dashboards.Components.GridSearch" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<table>
<tr><td>
<input id="FindVal" onkeydown="javascript:resetFind(event);" type="text" name="FindVal"
                                        style="FONT-SIZE: 8pt; FONT-FAMILY: Verdana"/></td><td>
                        <igtxt:WebImageButton ID="Find"
                runat="server" AutoSubmit="False" Width="16px" Height="16px" ImageDirectory="">
                <Appearance>
                    <ButtonStyle CssClass="SearchButton">
                    </ButtonStyle>
                </Appearance>
                <PressedAppearance>
                    <ButtonStyle CssClass="SearchButton">
                    </ButtonStyle>
                </PressedAppearance>
                            <ClientSideEvents Click="FindValue" />
            </igtxt:WebImageButton>   
            </td></tr>
 </table>  
<input id="FindIndicator" type="Hidden" value="Find"/>
<input id="SelectedRowId" type="Hidden" value=""/>
                        
                        
                        
                   