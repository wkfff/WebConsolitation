<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logon.aspx.cs" Inherits="Krista.FM.Server.Dashboards.Autenticate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body bgcolor="white" style="width: 100%; height: 100%">
    <div class="logonBackground"></div>
    <form id="form1" runat="server">            
        <div class="main">
        <div class="shadowTop"></div> 
        <div class="shadowBottom"></div>
        <div class="shadowLeft"></div>
        <div class="shadowRight"></div>
        <div class="title"></div>
        <div runat="server" id="loginDiv" class="loginDiv">
            <asp:Login ID="Login" runat="server" OnAuthenticate="Login_Authenticate" BorderColor="LightGray" BorderStyle="Solid" Font-Names="Verdana" Font-Size="Small" LoginButtonText="Вход" PasswordLabelText="Пароль:" RememberMeText="Запомнить учетные данные" TitleText="" UserNameLabelText="Имя пользователя:" Height="140px" LoginButtonImageUrl="~/images/button.gif" LoginButtonType="Image" Width="300px" BorderWidth="4px" FailureText="">
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
    </form>
</body>
</html>