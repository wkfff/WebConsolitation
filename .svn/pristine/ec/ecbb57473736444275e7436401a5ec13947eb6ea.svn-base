<%@ Page Language="C#"%>
<%@ Import Namespace="Krista.FM.RIA.Core" %>

<%@ Register assembly="Ext.Net" namespace="Ext.Net" tagprefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Вход в систему</title>
    <link href="<%# ResourceRegister.Style("/Content/css/LogOn.css") %>" rel="stylesheet" type="text/css" />
</head>

<script runat="server">
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        Page.Header.DataBind();
    }
</script>

<body>
    <noscript>
        <div style="margin: 0; padding: 0; width: 400px; height: 60px; 
            position: absolute; top: 50%; left: 50%; margin-top: -30px; margin-left: -200px; background: red;
            text-align: center; text-decoration: blink; color: black;">
            <br>
            Для работы приложения необходима поддержка JavaScript. Проверьте настройки браузера.
        </div>
    </noscript>

    <ext:ResourceManager runat="server" EnableTheming="true" DisableViewState="true"/>
    
    <% 
    if(!this.ViewData.ModelState.IsValid){
        if (this.ViewData.ModelState.Values.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ModelError error in this.ViewData.ModelState["_FORM"].Errors)
            {
                sb.AppendLine(error.ErrorMessage);
            }
            logonError.Text = sb.ToString();
            errorPanel.Hidden = false;
        }
        logonWindow.Show();
        logonWindow.DoLayout(true);
        logonWindow.AddScript("Ext.net.Mask.hide();");
    } %>
    
    <ext:Window 
        ID="logonWindow" 
        runat="server" 
        Closable="false"
        Resizable="false"
        AutoHeight="true"
        Icon="Lock" 
        Title="Вход в систему"
        Draggable="false"
        Width="300"
        Modal="true"
        BodyStyle="padding:5px;">
        <Content>
            <ext:FitLayout runat="server">
                <Items>
                    <ext:FormPanel
                        FormID="logonForm"
                        runat="server"
                        Border="false"
                        AutoHeight="true"
                        BodyBorder="false"
                        MonitorValid="true"
                        BodyStyle="background:transparent;">
                        <Defaults>
                            <ext:Parameter Name="anchor" Value="93%" Mode="Value" />
                            <ext:Parameter Name="allowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="msgTarget" Value="side" Mode="Value" />
                        </Defaults>
                        <Content>
                            <ext:FormLayout ID="FormLayout1" runat="server" LabelWidth="50">
                                <Anchors>
                                    <ext:Anchor>
                                        <ext:TextField 
                                            ID="userName" 
                                            runat="server" 
                                            FieldLabel="Логин" 
                                            BlankText="Необходимо ввести Ваш логин для входа в систему."
                                            />
                                    </ext:Anchor>
                                    <ext:Anchor>
                                        <ext:TextField 
                                            ID="password" 
                                            runat="server" 
                                            InputType="Password" 
                                            FieldLabel="Пароль" 
                                            BlankText="Необходимо ввести Ваш пароль для входа в систему."
                                            />
                                    </ext:Anchor>
                                    <ext:Anchor>
                                        <ext:Checkbox 
                                            ID="rememberMe" 
                                            runat="server"
                                            FieldLabel="Запомнить меня"
                                            AllowBlank="false"
                                            Checked="false"
                                            Hidden="true">
                                        </ext:Checkbox>
                                    </ext:Anchor>
                                    <ext:Anchor>
                                        <ext:Panel 
                                            ID="errorPanel" runat="server" 
                                            AutoHeight="true" Hidden="true" Border="false" BodyBorder="false" 
                                            BodyStyle="background: transparent;color:Red" >
                                            <Content>
                                                <ext:Label ID="logonError" runat="server"></ext:Label>
                                            </Content>
                                        </ext:Panel>
                                    </ext:Anchor>
                                </Anchors>
                            </ext:FormLayout>
                        </Content>
                        <Listeners>
                            <ClientValidation Handler="#{logonButton}.setDisabled(!valid);" />
                        </Listeners>
                    </ext:FormPanel>
                </Items>
            </ext:FitLayout>
        </Content>
        <Buttons>
            <ext:Button ID="logonButton" runat="server" Text="Войти" Icon="Accept">
                <DirectEvents>
                    <Click 
                        Url="/Account/Logon/" 
                        Timeout="240000"
                        FormID="logonForm"
                        CleanRequest="true" 
                        Method="POST"
                        Failure="
                        Ext.Msg.show({
                           title:   'Login Error',
                           msg:     result.errorMessage,
                           buttons: Ext.Msg.OK,
                           icon:    Ext.MessageBox.ERROR
                        });"
                        Success="">
                        <EventMask ShowMask="true" Msg="Подключение..."/>
                        <ExtraParams>
                            <ext:Parameter Name="returnUrl" Value="Ext.urlDecode(String(document.location).split('?')[1]).r || '/'" Mode="Raw" />
                            <ext:Parameter Name="__EVENTARGUMENT" Value="logonButton|event|Click" Mode="Value" />
                        </ExtraParams>
                    </Click>
                </DirectEvents>
                <Listeners>
                    <Click Handler="Ext.net.Mask.show({ msg: 'Подключение...' });" Delay="100"/>
                </Listeners>
            </ext:Button>
        </Buttons>
        <KeyMap>
            <ext:KeyBinding>
                <Keys>
                    <ext:Key Code="ENTER" />
                </Keys>
                <Listeners>
                    <Event Handler="if(!logonButton.disabled){ logonButton.fireEvent('click');}" />
                </Listeners>
            </ext:KeyBinding>
        </KeyMap>
    </ext:Window>
</body>
</html>
