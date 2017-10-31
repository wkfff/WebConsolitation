var ResetDirtyAttributeOnFormItems = function (form) {
    form.getForm().items.each(function (f) {
        if (f.isFormField) {
            f.originalValue = f.getValue();
            if (f.xtype == 'compositefield') {
                this.eachItem(function (item) {
                    item.originalValue = item.getValue();
                });
            }
        }
    });
};

var submitForm = function (formPanel) {
    formPanel.getForm().submit({
        waitMsg: 'Сохранение...',
        success: successFormSaveHandler,
        failure: failureFormSaveHandler,
        params: { programId: ID.getValue() }
    });
};

function successFormSaveHandler(form, action) {
    if (action.result.script) {
        eval(action.result.script);
    }
    if (action.result != undefined && action.result.extraParams.msg != undefined) {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: action.result.extraParams.msg,
            title: 'Уведомление',
            hideDelay: 2500
        });
    } else {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: 'Ок',
            title: 'Уведомление',
            hideDelay: 2500
        });
    }
};

function failureFormSaveHandler(form, action) {
    switch (action.failureType) {
        case Ext.form.Action.CLIENT_INVALID:
            Ext.Msg.alert('Ошибка', 'Все поля должны иметь корректные значения');
            break;
        case Ext.form.Action.CONNECT_FAILURE:
            Ext.Msg.alert('Ошибка', 'Ajax communication failed');
            break;
        case Ext.form.Action.SERVER_INVALID:
            if (action.result != undefined) {
                if (action.result.extraParams != undefined && action.result.extraParams.responseText != undefined) {
                    Ext.Msg.alert('Ошибка', action.result.extraParams.responseText);
                } else if (action.result.msg != undefined) {
                    Ext.Msg.alert('Ошибка', action.result.msg);
                } else {
                    if (action.result.errors != undefined) {
                        Ext.Msg.alert('Ошибка', action.result.errors[0].msg);
                    }
                }
            } else {
                Ext.Msg.alert('Ошибка', 'Server failed');
            }
            break;
    }
};
