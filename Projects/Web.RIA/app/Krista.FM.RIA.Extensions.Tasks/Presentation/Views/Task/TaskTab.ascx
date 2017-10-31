<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register assembly="Ext.Net" namespace="Ext.Net" tagprefix="ext" %>
<ext:FitLayout ID="gdFitLayout1" runat="server">
    <Items>
        <ext:FormPanel ID="GeneralForm" runat="server" Border="false" Url="/Task/Save/">
            <Content>
                <ext:FormLayout ID="layGeneral" runat="server" LabelWidth="130">
                    <Anchors>
                        <ext:Anchor>
                            <ext:TextField ID="TaskID" DataIndex="TaskID" runat="server" FieldLabel="ID задачи" AllowBlank="false"></ext:TextField>
                        </ext:Anchor>
                        <ext:Anchor>
                            <ext:Panel runat="server" Border="false">
                                <Items>
                                    <ext:Container runat="server" Layout="Column" Height="196">
                                        <Items>
                                             <ext:Container runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                                <Items>
                                                    <ext:TextArea ID="fHeadline" DataIndex="Headline" runat="server" FieldLabel="Наименование" AllowBlank="false" Height="50" AnchorHorizontal="95%"></ext:TextArea>
                                                    <ext:TextArea ID="fTask" DataIndex="Job" runat="server" FieldLabel="Задание" AllowBlank="true" Height="100" AnchorHorizontal="95%"></ext:TextArea>
                                                </Items>
                                             </ext:Container>
                                             <ext:Container runat="server" LabelAlign="Left" Layout="Form" ColumnWidth=".5">
                                                <Items>
                                                    <ext:TextField ID="fCashedAction" DataIndex="CashedAction" runat="server" FieldLabel="Действие" ReadOnly="true"></ext:TextField>
                                                    <ext:TextField ID="fState" DataIndex="State" runat="server" FieldLabel="Состояние" AllowBlank="false" ReadOnly="true"></ext:TextField>
                                                    <ext:TextField ID="fFromDate" DataIndex="FromDate" runat="server" FieldLabel="Дата начала" AllowBlank="false" ReadOnly="true"/>
                                                    <ext:TextField ID="fToDate" DataIndex="ToDate" runat="server" FieldLabel="Дата завершения" AllowBlank="false" ReadOnly="true"/>
                                                    <ext:ComboBox 
                                                            ID="fOwner" runat="server" 
                                                            StoreID="dsUsers" 
                                                            DisplayField="NAME" ValueField="NAME" 
                                                            DataIndex="OwnerName" FieldLabel="Владелец"
                                                            TypeAhead="false" LoadingText="Поиск..." 
                                                            PageSize="10" HideTrigger="false" 
                                                            ItemSelector="div.search-item" MinChars="1"
                                                            Width="250">
                                                        <Template runat="server">
                                                            <Html>
                                                                <tpl for=".">
                                                                    <div class="search-item">
                                                                        <div><h3>{NAME}<span>{JOBTITLE}</span></h3></div>
                                                                        <div><span>{LASTNAME}</span><span>{FIRSTNAME}</span><span>{PATRONYMIC}</span></div>
                                                                    </div>
                                                                </tpl>
                                                            </Html>
                                                        </Template>
                                                    </ext:ComboBox>
                                                    <ext:ComboBox 
                                                            ID="fDoer" runat="server" 
                                                            StoreID="dsUsers" 
                                                            DisplayField="NAME" ValueField="NAME" 
                                                            DataIndex="DoerName" FieldLabel="Исполнитель"
                                                            TypeAhead="false" LoadingText="Поиск..." 
                                                            PageSize="10" HideTrigger="false" 
                                                            ItemSelector="div.search-item" MinChars="1"
                                                            Width="250">
                                                        <Template ID="Template2" runat="server">
                                                            <Html>
                                                                <tpl for=".">
                                                                    <div class="search-item">
                                                                        <div><h3>{NAME}<span>{JOBTITLE}</span></h3></div>
                                                                        <div><span>{LASTNAME}</span><span>{FIRSTNAME}</span><span>{PATRONYMIC}</span></div>
                                                                    </div>
                                                                </tpl>
                                                            </Html>
                                                        </Template>
                                                    </ext:ComboBox>
                                                    <ext:ComboBox 
                                                            ID="fCurator" runat="server" 
                                                            StoreID="dsUsers" 
                                                            DisplayField="NAME" ValueField="NAME" 
                                                            DataIndex="CuratorName" FieldLabel="Куаратор"
                                                            TypeAhead="false" LoadingText="Поиск..." 
                                                            PageSize="10" HideTrigger="false" 
                                                            ItemSelector="div.search-item" MinChars="1"
                                                            Width="250">
                                                        <Template ID="Template3" runat="server">
                                                            <Html>
                                                                <tpl for=".">
                                                                    <div class="search-item">
                                                                        <div><h3>{NAME}<span>{JOBTITLE}</span></h3></div>
                                                                        <div><span>{LASTNAME}</span><span>{FIRSTNAME}</span><span>{PATRONYMIC}</span></div>
                                                                    </div>
                                                                </tpl>
                                                            </Html>
                                                        </Template>
                                                    </ext:ComboBox>
                                                </Items>
                                             </ext:Container>
                                        </Items>
                                    </ext:Container>
                                </Items>
                            </ext:Panel>
                        </ext:Anchor>
                        <ext:Anchor>
                            <ext:Panel ID="pnlDescription" runat="server" Border="false">
                                <Items>
                                    <ext:FormLayout ID="gdFormLayout4" runat="server" LabelAlign="Top">
                                        <Anchors>
                                            <ext:Anchor Horizontal="100%">
                                                <ext:TextArea ID="fDescription" runat="server" DataIndex="Description" FieldLabel="Коментарии" Height="200"></ext:TextArea>
                                            </ext:Anchor>
                                        </Anchors>
                                    </ext:FormLayout>
                                </Items>
                            </ext:Panel>
                        </ext:Anchor>
                    </Anchors>
                </ext:FormLayout>
            </Content>
        </ext:FormPanel>
    </Items>
</ext:FitLayout>

