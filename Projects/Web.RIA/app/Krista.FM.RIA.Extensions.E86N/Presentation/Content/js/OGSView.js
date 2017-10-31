//получение значений для фильтра
var getFilter = function (fld) {
    var val;
    switch (fld) {
        case 'PPO':
            val = RefOrgPPOFlt.getValue();
            if (val != '')
             return val; 
            else
             return -1;
        case 'GRBS':
            val = RefOrgGRBSFlt.getValue();
            if (val != '')
             return val; 
            else
             return -1; 
        default:
            return -1;
    }
};

Ext.ns('E86n.View.OGSView');
E86n.View.OGSView = {
    Update: function () {
        if (OGSGrid.getSelectionModel().hasSelection()) {
            OGSGridRemoveRecordBtn.enable();
            if (window.OGSGrid.getSelectionModel().getSelected().data.Status) {
                btnClose.enable();
                btnOpen.disable();
            } else {
                btnClose.disable();
                btnOpen.enable();
            }
        } else {
            OGSGridRemoveRecordBtn.disable();
            btnClose.disable();
            btnOpen.disable();
        }
    },
    
    UpdateFlt: function () {
        if (RefOrgPPOFltName.getValue() != '') {
            var t = RefOrgPPOFltName.getValue();
            OGSGrid.filters.getFilter(8).setValue(t.substring(0,t.length-2));
            OGSGrid.filters.getFilter(8).setActive(true);
        }
        if (RefOrgGRBSFltName.getValue() != '') {
            var t = RefOrgGRBSFltName.getValue();
            OGSGrid.filters.getFilter(7).setValue(t.substring(0, t.length - 2));
            OGSGrid.filters.getFilter(7).setActive(true);
        }
        OGSGrid.reload();
    },

    commandClick: function (command, record) {
        if (command == "EditTypeHistory") {
            E86n.View.OGSView.editTypeHistory(record);
        }
    },

    editTypeHistory: function (record) {
        var recId = record.data.ID;

        if (recId == -1) {
            Ext.Msg.alert('Ошибка', 'Сначала нужно сохранить учреждение');
        }

        var store = new Ext.data.Store({
            proxy: new Ext.data.HttpProxy({
                url: '/OGS/',
                api: {
                    read: { url: '/OGS/GetTypes', method: 'GET' }
                },
                timeout: 60000
            }),
            reader: new Ext.data.JsonReader({
                successProperty: 'success',
                idProperty: 'ID',
                root: 'data',
                fields: ["ID", "Name"]
            }),
            idProperty: 'id',
            listeners: {
                load: function () {
                    Ext.Ajax.request({
                        url: '/OGS/GetLastTypeCloseDate',
                        params: { recId: recId },
                        success: function (response) {
                            var jo = Ext.util.JSON.decode(response.responseText);
                            var dateOpen = jo.data;

                            E86n.View.OGSView.editTypeHistoryWnd(store, recId, dateOpen);
                        }
                    });
                }
            }
        });

        store.load();
    },


    editTypeHistoryWnd: function (store, recId, dateOpen) {
        var form = new Ext.form.FormPanel({
            border: false,
            monitorValid: true,
            padding: 20,
            autoHeight: true,
            width: 400,
            items: [
                {
                    xtype: 'fieldset',
                    title: 'Текущий тип',
                    defaults: {
                        anchor: '100%'
                    },
                    items: [
                        {
                            xtype: 'textarea',
                            id: 'noteTypeHistory',
                            fieldLabel: 'Примечание',
                            emptyText: 'Введите сообщение...'
                        }, {
                            xtype: 'datefield',
                            id: 'dateStartTypeHistory',
                            fieldLabel: 'Дата открытия',
                            value: dateOpen != null ? new Date(dateOpen) : null,
                            disabled: !!dateOpen,
                            allowBlank: false
                        }, {
                            xtype: 'datefield',
                            id: 'dateEndTypeHistory',
                            fieldLabel: 'Дата закрытия',
                            value: new Date(),
                            allowBlank: false
                        }
                    ]
                },
                {
                    xtype: 'combo',
                    id: 'typeTypeHistory',
                    triggerAction: 'all',
                    mode: 'local',
                    store: store,
                    valueField: 'ID',
                    displayField: 'Name',
                    fieldLabel: 'Новый тип',
                    allowBlank: false,
                    anchor: '100%',
                    emptyText: 'Выберите из списка'
                }
            ],
            buttons: [
                {
                    text: 'ОК',
                    disabled: true,
                    formBind: true,
                    handler: function () {
                        var newType = Ext.getCmp('typeTypeHistory').getValue();
                        var note = Ext.getCmp('noteTypeHistory').getValue();
                        var dateStart = Ext.getCmp('dateStartTypeHistory').getValue();
                        var dateEnd = Ext.getCmp('dateEndTypeHistory').getValue();

                        Ext.Ajax.request({
                            url: '/OGS/SetType',
                            params: { recId: recId, typeId: newType, note: note, dateStart: dateStart, dateEnd: dateEnd },
                            success: function (response) {
                                var jo = Ext.util.JSON.decode(response.responseText);
                                if (!jo.success) {
                                    Ext.Msg.alert('Ошибка', jo.message);
                                    return;
                                }
                                Ext.getCmp('EditHistoryWnd').close();
                                Ext.getCmp('OGSGrid').getStore().reload();
                            },
                            failure: function (response) {
                                var jo2 = Ext.util.JSON.decode(response.responseText);
                                Ext.Msg.alert('Ошибка', jo2.message);
                            }
                        });
                    }
                },
                {
                    text: 'Отмена',
                    handler: function () {
                        Ext.getCmp('EditHistoryWnd').close();
                    }
                }
            ]
        });

        var wnd = new Ext.Window({
            id: 'EditHistoryWnd',
            layout: 'fit',
            title: 'Изменение типа учреждения',
            modal: true,
            closable: false,
            items: form
        });

        wnd.show();
    }
};