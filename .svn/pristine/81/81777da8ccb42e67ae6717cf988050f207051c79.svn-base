<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register assembly="Ext.Net" namespace="Ext.Net" tagprefix="ext" %>
<ext:Window 
    ID="UploadWindow" 
    runat="server" 
    Title="Форма загрузки документа"
    Hidden="true" 
    Modal="true" 
    Width="500" 
    AutoHeight="true"
    Resizable="false"
    Border="false">
    <Content>
        <ext:FormPanel 
            ID="uploadDocumentForm" 
            runat="server" 
            Frame="true"
            MonitorValid="true"
            Border="false" 
            BodyBorder="false"
            BodyStyle="padding: 10px 10px 0 10px;background:none repeat scroll 0 0 #DFE8F6;">
            <Defaults>
                <ext:Parameter Name="anchor" Value="95%" Mode="Value" />
                <ext:Parameter Name="allowBlank" Value="false" Mode="Raw" />
                <ext:Parameter Name="msgTarget" Value="side" Mode="Value" />
            </Defaults>
            <Content>
                <ext:FormLayout runat="server" LabelWidth="50">
                    <Anchors>
                        <ext:Anchor>
                            <ext:FileUploadField 
                                ID="uploadField" 
                                runat="server" 
                                EmptyText="Выберите файл"
                                FieldLabel="Файл"
                                ButtonText=""
                                Icon="ImageAdd">
                            </ext:FileUploadField>
                        </ext:Anchor>
                    </Anchors>
                </ext:FormLayout>
            </Content>
            <Listeners>
                <ClientValidation Handler="#{uploadButton}.setDisabled(!valid);" />
            </Listeners>
            <Buttons>
                <ext:Button ID="uploadButton" runat="server" Text="Передать" Icon="DiskUpload">
                    <DirectEvents>
                        <Click 
                            Url="/Task/SaveDocument"
                            IsUpload="true" 
                            CleanRequest="true"
                            Method="POST"
                            FormID="uploadDocumentForm"
                            Before="if(!#{uploadDocumentForm}.getForm().isValid()) { return false; } 
                                Ext.Msg.wait('Передача данных на сервер...', 'Загрузка');"
                            Failure="Ext.Msg.show({ 
                                title   : 'Ошибка', 
                                msg     : result.msg || response.responseText, 
                                minWidth: 200, 
                                modal   : true, 
                                icon    : Ext.Msg.ERROR, 
                                buttons : Ext.Msg.OK,
                                animEl: 'grdDocuments'
                            });"
                            Success="
                            UploadWindow.hide();
                            Ext.Msg.show({ 
                                title   : 'Уведомление', 
                                msg     : result.msg, 
                                minWidth: 200, 
                                modal   : true, 
                                icon    : Ext.Msg.INFO, 
                                buttons : Ext.Msg.OK,
                                animEl: 'grdDocuments'
                            });">
                            <ExtraParams>
                                <ext:Parameter Name="taskId" Value="#{txtFilter}.getValue()" Mode="Raw"/>
                                <ext:Parameter Name="documentId" Value="UploadWindow.Document.ID" Mode="Raw"/>
                            </ExtraParams>
                        </Click>
                    </DirectEvents>
                </ext:Button>
                <ext:Button ID="resetButton" runat="server" Text="Сброс">
                    <Listeners>
                        <Click Handler="#{uploadDocumentForm}.getForm().reset();" />
                    </Listeners>
                </ext:Button>
            </Buttons>
        </ext:FormPanel>
    </Content>
</ext:Window>
