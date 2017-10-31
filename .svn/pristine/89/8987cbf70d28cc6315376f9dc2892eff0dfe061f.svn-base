<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomLogin.ascx.cs" Inherits="Krista.FM.Server.Dashboards.Components.Components.CustomLogin" %>
<div class="logonBackground"></div>
        <div class="main">
        <div class="shadowTop"></div> 
        <div class="shadowBottom"></div>
        <div class="shadowLeft"></div>
        <div class="shadowRight"></div>
        <div class="title"></div>
        <div runat="server" id="loginDiv" class="loginDiv">
            <asp:Login ID="aspLogin" runat="server" BorderColor="LightGray" BorderStyle="Solid" Font-Names="Verdana" Font-Size="Small" LoginButtonText="Вход" PasswordLabelText="Пароль:" RememberMeText="Запомнить учетные данные" TitleText="" UserNameLabelText="Имя пользователя:" Height="140px" LoginButtonImageUrl="~/images/button.gif" LoginButtonType="Image" Width="300px" BorderWidth="4px" FailureText="">
                <TitleTextStyle BorderColor="Black" CssClass="AutenticateTitle" />
                <LoginButtonStyle BackColor="Transparent" BorderColor="Transparent" Height="22px"
                    Width="82px" />
            </asp:Login>
        </div>
        <div class="errorTable">
            <asp:Table ID="errorTable" runat="server"  CellPadding="0" CellSpacing="0" Width="310px" Visible="False">
                <asp:TableRow ID="TableRow2" runat="server" Height="80px" Width="304px">
                    <asp:TableCell ID="TableCell2" runat="server" CssClass="ErrorTableCell"></asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            </div>
            <div class="errorTableHeader">
            <asp:Table ID="errorTableHeader" runat="server" CellPadding="0" CellSpacing="0" Width="310px" Visible="False">
                <asp:TableRow ID="TableRow1" runat="server" Height="4px" Width="304px">
                    <asp:TableCell ID="TableCell1" runat="server"><asp:Image ID="shadowLeft" runat="server" Height="4px" Width="310px" ImageUrl="~/images/errorHeader.gif" /></asp:TableCell>
                </asp:TableRow>
            </asp:Table> 
            </div>                 
      </div>   