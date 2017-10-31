<%@ Control Language="C#" AutoEventWireup="true" Codebehind="CustomMultiCombo.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.Components.CustomMultiCombo" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>

            <igmisc:WebPanel ID="webPanel" runat="server" EnableAppStyling="True" StyleSetName="Office2007Blue"
                Height="15px" Width="100px" ExpandEffect="None" ExpandOnClick="true">
                <Template>
                <div style="width: 100%; height: 100%; overflow: auto">
                    <ignav:UltraWebTree ID="treeView" runat="server" EnableAppStyling="True" StyleSetName="Office2007Blue"
                        OnDataBinding="treeView_DataBinding">
                        <ClientSideEvents />
                    </ignav:UltraWebTree></div>
                </Template>
            </igmisc:WebPanel>        