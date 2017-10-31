<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Krista.FM.Extensions" %>
<%@ Register assembly="Ext.Net" namespace="Ext.Net" tagprefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <!--<meta http-equiv="Content-Type" content="text/html; windows-1251" />-->
    <script runat="server">
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

    </script>
</head>
<body>
<form id="Form1" runat="server">
<!--ResourcePlaceHolder-->
    <ext:ResourcePlaceHolder ID="ResourcePlaceHolder1" runat="server"/>

<!--ResourceManager-->
    <ext:ResourceManager ID="ResourceManager1" runat="server"/>

    <ext:Store ID="dsTest" runat="server" ShowWarningOnFailure="true" AutoLoad="true">
        <Proxy>
            <ext:HttpProxy Url="/Home/Data" Method="GET"/>
        </Proxy>
        <Reader>
            <ext:JsonReader IDProperty="ID" Root="data" TotalProperty="total">
                <Fields>
                    <ext:RecordField Name="ID" />
                    <ext:RecordField Name="TEXT" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:GridPanel ID="gpTest" runat="server"
        StoreID="dsTest">
        <ColumnModel>
            <Columns>
                <ext:Column ColumnID="ID" DataIndex="ID" Header="ID"></ext:Column>
                <ext:Column ColumnID="Text" DataIndex="Text" Header="Text"></ext:Column>
            </Columns>
        </ColumnModel>
    </ext:GridPanel>
</form>
</body>
</html>
