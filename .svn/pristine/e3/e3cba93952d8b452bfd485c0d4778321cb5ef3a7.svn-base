Ext.ns('E86n.View');

E86n.View.InfControlMeasures =
    {
        //требуется ли вызывать контроллер
        update: true,

        DataChanged: function (checkBox, store) {
            window.E86n.View.InfControlMeasures.update = false;
            var cb = Ext.getCmp(checkBox);
            var record = store.getAt(0);
            if (record) {
                cb.setValue(record.get(checkBox));
            } else {
                cb.setValue(false);
            }
            window.E86n.View.InfControlMeasures.update = true;
        },
        
        Check: function (item, checked, url, docId) {
            if (window.E86n.View.InfControlMeasures.update) {
                if (checked) {
                    Ext.Msg.show({
                        title: 'Подтверждение',
                        msg: 'Данное действие удалит все данные документа. Продолжить?',
                        width: 300,
                        buttons: Ext.Msg.YESNO,
                        fn: function (btn) {
                            if (btn == 'yes') {
                                Ext.Ajax.request({
                                    url: url + '&value=' + checked,
                                    success: function (response) {
                                        var jo = Ext.util.JSON.decode(response.responseText);
                                        if (jo.success) {
                                            Ext.net.Notification.show({
                                                iconCls: 'icon-information',
                                                html: 'Все действия выполнены',
                                                title: 'Выполнено',
                                                hideDelay: 2000
                                            });
                                            window.E86n.View.InfControlMeasures.update = false;
                                            window.E86n.View.InfControlMeasures.SetReadOnly(true, docId);
                                            window.E86n.View.InfControlMeasures.update = true;
                                            window.InfControlMeasuresStore.reload();
                                        } else {
                                            Ext.Msg.alert('Ошибка выполнения', jo.message);
                                            window.E86n.View.InfControlMeasures.update = false;
                                            window.E86n.View.InfControlMeasures.SetReadOnly(false, docId);
                                            item.setValue(false);
                                            window.E86n.View.InfControlMeasures.update = true;
                                        }
                                    },
                                    failure: function (response) {
                                        var jo = Ext.util.JSON.decode(response.responseText);
                                        Ext.Msg.alert('Ошибка выполнения', jo.message);
                                        window.E86n.View.InfControlMeasures.update = false;
                                        window.E86n.View.InfControlMeasures.SetReadOnly(false, docId);
                                        item.setValue(false);
                                        window.E86n.View.InfControlMeasures.update = true;
                                    }
                                });
                            }
                            else {
                                window.E86n.View.InfControlMeasures.update = false;
                                window.E86n.View.InfControlMeasures.SetReadOnly(false, docId);
                                item.setValue(false);
                                window.E86n.View.InfControlMeasures.update = true;
                            }
                        },
                        modal: true,
                        icon: Ext.Msg.QUESTION,
                        closable: false
                    });

                } else {
                    Ext.Ajax.request({
                        url: url + '&value=' + checked,
                        success: function (response) {
                            var jo = Ext.util.JSON.decode(response.responseText);
                            if (jo.success) {
                                Ext.net.Notification.show({
                                    iconCls: 'icon-information',
                                    html: 'Все действия выполнены',
                                    title: 'Выполнено',
                                    hideDelay: 2000
                                });
                                window.E86n.View.InfControlMeasures.update = false;
                                window.E86n.View.InfControlMeasures.SetReadOnly(false, docId);
                                window.E86n.View.InfControlMeasures.update = true;
                                window.InfControlMeasuresStore.reload();
                            } else {
                                Ext.Msg.alert('Ошибка выполнения', jo.message);
                                window.E86n.View.InfControlMeasures.update = false;
                                window.E86n.View.InfControlMeasures.SetReadOnly(true, docId);
                                item.setValue(true);
                                window.E86n.View.InfControlMeasures.update = true;
                            }
                        },
                        failure: function (response) {
                            var jo = Ext.util.JSON.decode(response.responseText);
                            Ext.Msg.alert('Ошибка выполнения', jo.message);
                            window.E86n.View.InfControlMeasures.update = false;
                            window.E86n.View.InfControlMeasures.SetReadOnly(true, docId);
                            item.setValue(true);
                            window.E86n.View.InfControlMeasures.update = true;
                        }
                    });
                }
            }
        },
        
        SetReadOnlyInfControlMeasures: function (readOnly, recId) {
            
            window.NotInspectionActivity.setDisabled(readOnly);
            window.E86n.View.InfControlMeasures.SetReadOnly(readOnly, recId);
        },
        
        SetReadOnly: function (readOnly, recId) {
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'InfControlMeasures');

            window.E86n.Control.StateToolBar.ReadOnlyDocs(readOnly, recId);
        }
    };