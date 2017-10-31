<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Logs.aspx.cs" Inherits="Krista.FM.Server.Dashboards.Logs" %>

<%@ Register Src="Components/CustomLogin.ascx" TagName="CustomLogin" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Логи</title>
</head>
<body bgcolor="white" style="width: 100%; height: 100%">
    <form id="form1" runat="server">
        <uc1:CustomLogin ID="CustomLogin1" runat="server"></uc1:CustomLogin>
        <asp:Panel ID="GetLogButtonsPanel" runat="server" Height="66px" Width="309px" Visible="False">
            <asp:Button ID="GetCrashLog" runat="server" Text="Лог ошибок" Width="150px" OnClick="GetCrashLog_Click" />&nbsp;<asp:Label ID="CrashLogFileSize" runat="server" Text="Label"></asp:Label><br />
            <asp:Button ID="GetUserLog" runat="server" Text="Лог подключений" Width="150px" OnClick="GetUserLog_Click" />&nbsp;<asp:Label ID="UserLogFileSize" runat="server" Text="Label"></asp:Label><br />
            <asp:Button ID="GetUserAgentsLog" runat="server" Text="Лог user-agents" Width="150px" OnClick="GetUserAgentsLog_Click" />&nbsp;<asp:Label ID="UserAgetnsLogFileSize" runat="server" Text="Label"></asp:Label><br />
            <asp:Button ID="GetQueryLog" runat="server" Text="Лог запросов" Width="150px" OnClick="GetQueryLog_Click" />&nbsp;<asp:Label ID="QueryLogFileSize" runat="server" Text="Label"></asp:Label><br />         
            <asp:Button ID="GetServerLog" runat="server" Text="Лог сервера" Width="150px" OnClick="GetServerLog_Click" />&nbsp;<asp:Label ID="ServerLogFileSize" runat="server" Text="Label"></asp:Label><br />
            <asp:Button ID="ClearLogs" runat="server" Text="Очистить логи" Width="150px" OnClick="ClearLogs_Click" /><br/>
            <asp:Button ID="CheckServer" runat="server" Text="Проверить сервер" Width="150px" OnClick="CheckServer_Click" />
            &nbsp;
            <asp:Label ID="lbScheme" runat="server" Text="lbScheme"></asp:Label></asp:Panel>
    </form>
</body>
</html>
