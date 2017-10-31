<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PopupInformer.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.Components.PopupInformer" %>

<div id="overlay" class="overlay" style="display: none"></div>
<span class="help_sp" onclick="help(this);" title="Справка">
    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/getHelp.gif" />
    <div id="help" style="top: 70px; display: none; float: left;"></div></span>	