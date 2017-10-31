<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register assembly="Ext.Net" namespace="Ext.Net" tagprefix="ext" %>
<%@ Register src="~/App_Resource/Krista.FM.RIA.Extensions.Tasks.dll/Krista.FM.RIA.Extensions.Tasks/Presentation/Views/Task/Dummy.ascx" tagname="Dummy" tagprefix="uc" %>

<ext:BorderLayout ID="BorderLayout1" runat="server">
    <Center>
        <ext:GridPanel
            ID="grdDocuments" 
            runat="server" 
            StoreID="dsDocuments"
            Border="false" 
            TrackMouseOver="true">
            <ColumnModel>
                <Columns>
                    <ext:CommandColumn Width="70">
                        <Commands>
                            <ext:GridCommand Icon="PageGo" CommandName="openDocument">
                                <ToolTip Text="Открыть для просмотра"/>
                            </ext:GridCommand>
                            <ext:GridCommand Icon="PageAttach" CommandName="attachDocument">
                                <ToolTip Text="Прикрепить"/>
                            </ext:GridCommand>
                        </Commands>
                    </ext:CommandColumn>
                    <ext:Column ColumnID="ID" DataIndex="ID" Header="ID" Width="40" Hidden="true" />
                    <ext:Column ColumnID="NAME" DataIndex="NAME" Header="Наименование" Width="300" />
                    <ext:Column ColumnID="DESCRIPTION" DataIndex="DESCRIPTION" Header="Комментарий" Width="200" />
                    <ext:Column ColumnID="DocumentTypeName" DataIndex="DocumentTypeName" Header="Тип документа" Width="170px" />
                    <ext:Column ColumnID="SOURCEFILENAME" DataIndex="SOURCEFILENAME" Header="Тип файла" Width="80" />
                    <ext:Column ColumnID="VERSION" DataIndex="VERSION" Header="Версия" Width="50" />
                </Columns>
            </ColumnModel>
            <View>
                <ext:GridView ID="GridView1" runat="server" AutoFill="true" />
            </View>
            <Listeners>
                <Command Fn="grdDocumentsCommand" />
            </Listeners>
            <LoadMask ShowMask="true" />
        </ext:GridPanel>
    </Center>
    <South Split="true">
        <ext:Panel ID="SouthPanel" runat="server" Border="true" Collapsible="true" Collapsed="true" Height="0" Hidden="true">
            <Content>
                <uc:Dummy ID="dummyControl" runat="server" />
            </Content>
        </ext:Panel>
    </South>
</ext:BorderLayout>

