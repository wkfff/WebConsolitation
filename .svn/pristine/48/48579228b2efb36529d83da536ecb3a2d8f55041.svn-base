Ext.ns("Krista.FM.DSign");

Krista.FM.DSign.Window = Ext.extend(Ext.Window, {
    title: 'Подпись дукумента',
    width: 600,
    height: 500,
    layout: 'fit',
    plain: true,

    constructor: function (config) {
        Krista.FM.DSign.Window.superclass.constructor.call(this, config);
    },

    initComponent: function () {
        var self = this;

        function loadDocument() {
            self.getEl().mask('Загрузка документа...');
            signForm.reset();
            signForm.load({
                success: function (form, action) {
                    self.getEl().unmask();
                },
                failure: function (form, action) {
                    var msg;
                    if (action.result) {
                        msg = action.result.message;
                    } else {
                        msg = action.response.responseText;
                    }

                    Ext.Msg.show({ title: 'Ошибка', msg: msg, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.ERROR });
                    self.getEl().unmask();
                    self.hide();
                }
            });
        }

        function signDocument() {
            self.getEl().mask('Выполняется подпись документа...');

            var data = Ext.getCmp('FileBase64').getValue();
            var cert = Ext.getCmp('CertThumbprint').getValue();
            var dsign = Krista.FM.DSign.sign(data, cert);

            if (dsign != null && Krista.FM.DSign.verify(data, dsign)) {
                // В случае удачной проверки подписи отправляем документ и подпись на сервер
                saveSignAndDocument(dsign, data, self.callback);
            }
            else {
                self.getEl().unmask();
            }
        }

        function saveSignAndDocument(sign, document, callback) {
            self.getEl().mask('Передача данных на сервер...');
            Ext.Ajax.request({
                url: '/SignedDocument/Save',
                method: 'POST',
                params: { dsign: sign, document: document },
                scope: self,
                success: function (result) {
                    self.getEl().unmask();
                    self.hide();
                    
                    var jsonData = Ext.util.JSON.decode(result.responseText);
                    var guid = null;
                    if (jsonData.success) {
                        guid = jsonData.guid;
                        Ext.Msg.show({
                            title: 'ЭЦП',
                            msg: 'Документ подписан.',
                            buttons: Ext.MessageBox.OK,
                            icon: Ext.MessageBox.INFO
                        });
                    }
                    else {
                        Ext.Msg.show({
                            title: 'ЭЦП',
                            msg: 'Ошибка проверки подписи на сервере: ' + jsonData.message,
                            buttons: Ext.MessageBox.OK,
                            icon: Ext.MessageBox.ERROR
                        });
                    }

                    if (callback) {
                        callback(guid);
                    }
                },
                failure: function (result, request) {
                    self.getEl().unmask();

                    if (callback) {
                        callback(guid);
                    }

                    this.hide();
                    Ext.Msg.show({
                        title: result.statusText,
                        msg: result.responseText,
                        buttons: Ext.MessageBox.OK
                    });
                }
            });
        }

        var signForm = new Ext.FormPanel({
            url: this.dsignReadUrl, // Url используемый для получения данных
            layout: {
                type: 'vbox',
                align: 'stretch'  // Child items are stretched to full width
            },
            items: [
	            {
	                id: 'CertThumbprint',
	                xtype: 'textfield',
	                name: 'CertThumbprint',
	                hidden: true
	            },
	            {
	                id: 'File',
	                xtype: 'textarea',
	                name: 'File',
	                flex: 1
	            },
	            {
	                id: 'FileBase64',
	                xtype: 'textfield',
	                name: 'FileBase64',
	                flex: 1,
	                hidden: true
	            }
	        ]
        });

        var signAction = new Ext.Action({
            text: 'Подписать',
            handler: signDocument
        });

        var config = {
            items: signForm,
            buttons: [signAction]
        };

        Ext.apply(this, Ext.apply(this.initialConfig, config));
        Krista.FM.DSign.Window.superclass.initComponent.apply(this, arguments);

        this.on({
            afterrender: loadDocument,
            scope: this
        });

    }
});
