<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage"%>
<%@ Import Namespace="Krista.FM.RIA.Core" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Index</title>
    <meta http-equiv="Content-Type" content="text/html; windows-1251" />
    <ext:ResourcePlaceHolder runat="server"/>
    <link href="<%# ResourceRegister.Style("/Krista.FM.RIA.Extensions.Entity/Presentation/Content/css/Entity.Index.css/extention.axd") %>" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var commandHandler = function (cmd, record) {
            switch (cmd) {
                case "Go":
                    parent.MdiTab.addTab({ title: record.data.Semantic + "." + record.data.Caption, url: "/Entity/Show?objectKey=" + record.data.ObjectKey, icon: "icon-report"});
                    break;
            }
        }
    </script>
    <script runat="server">
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            Page.Header.DataBind();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ScriptManager1" runat="server" />

<%
    ((HttpProxy) dsCustomers.Proxy[0]).Method = HttpMethod.POST;
     ((HttpProxy) dsCustomers.Proxy[0]).Url = "/EntityNav/Data?classType=" + Model; 
%>
    <ext:Store ID="dsCustomers" runat="server" RemoteSort="true">
        <Proxy>
            <ext:HttpProxy Url="/EntityNav/Data/" />
        </Proxy>
        <Reader>
            <ext:JsonReader IDProperty="reader1" Root="data" TotalProperty="total" AutoDataBind="True">
                <Fields>
                    <ext:RecordField Name="ClassType" />
                    <ext:RecordField Name="Semantic" />
                    <ext:RecordField Name="Caption" />
                    <ext:RecordField Name="Description" />
                    <ext:RecordField Name="ObjectKey" />
                </Fields>
            </ext:JsonReader>
        </Reader>
        <BaseParams>
            <ext:Parameter Name="limit" Value="10" Mode="Raw" />
            <ext:Parameter Name="start" Value="0" Mode="Raw" />
            <ext:Parameter Name="sort" Value="Semantic" Mode="Value"/>
            <ext:Parameter Name="dir" Value="ASC"  />
        </BaseParams>
    </ext:Store>

    <ext:Viewport ID="ViewPort1" runat="server">
        <Content>
            <ext:FitLayout runat="server">
                <Items>
                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="dsCustomers" TrackMouseOver="true">
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:CommandColumn Width="60">
                                    <Commands>
                                        <ext:GridCommand Icon="PlayGreen" CommandName="Go">
                                            <ToolTip Text="Открыть в новом окне" />
                                        </ext:GridCommand>
                                    </Commands>
                                </ext:CommandColumn>
                                <ext:Column ColumnID="ClassType" DataIndex="ClassType" Header="ClassType" Hidden="true">
                                </ext:Column>
                                <ext:Column ColumnID="Semantic" DataIndex="Semantic" Header="Семантика" Width="200">
                                </ext:Column>
                                <ext:Column ColumnID="Caption" DataIndex="Caption" Header="Наименование" Width="400">
                                </ext:Column>
                                <ext:Column ColumnID="ObjectKey" DataIndex="ObjectKey" Header="Ключ объекта" Hidden="true">
                                </ext:Column>
                            </Columns>
                        </ColumnModel>
                        <Plugins>
                            <ext:GridFilters runat="server" ID="GridFilters1" Local="false">
                                <Filters>
                                    <ext:NumericFilter DataIndex="ClassType" />
                                    <ext:StringFilter DataIndex="Semantic" />
                                    <ext:StringFilter DataIndex="Caption" />
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                         <BottomBar>
                            <ext:PagingToolbar ID="PagingToolbar1" runat="server" StoreID="dsCustomers" PageSize="10">
                            </ext:PagingToolbar>
                        </BottomBar>

                        <View>
                            <ext:GridView ID="GridView1" runat="server" EnableRowBody="true">
                                <GetRowClass Handler="rowParams.body = '<p>' + record.data.Description + '</p>'; return 'x-grid3-row-expanded';" />                    
                            </ext:GridView>        
                        </View>    
                        <Listeners>
                            <Command Fn="commandHandler" />
                        </Listeners>
                        <LoadMask ShowMask="true" />
                        <SaveMask ShowMask="true" />
                    </ext:GridPanel>
                </Items>
            </ext:FitLayout>
        </Content>
    </ext:Viewport>

    </form>
    
</body>
</html>
