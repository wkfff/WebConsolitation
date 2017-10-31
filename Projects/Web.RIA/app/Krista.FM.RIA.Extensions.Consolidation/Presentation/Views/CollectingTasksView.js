function newTaskWindowSubmit() {
    newTaskForm.getForm().submit({
        success: function (form, action) {
            newTaskWindow.hide();
            Ext.Msg.alert('Уведомление', "Задачи созданы успешно.");
        },
        failure: function (form, action) {
            switch (action.failureType) {
                case Ext.form.Action.CLIENT_INVALID:
                    Ext.Msg.alert('Ошибка', 'Form fields may not be submitted with invalid values');
                    break;
                case Ext.form.Action.CONNECT_FAILURE:
                    Ext.Msg.alert('Ошибка', 'Ajax communication failed');
                    break;
                case Ext.form.Action.SERVER_INVALID:
                    Ext.Msg.alert('Ошибка', action.result.errors[0].msg);
            }
        }
    });
};

function doPump() {

    var selCell = gpTasks.getSelectionModel().getSelectedCell();
    if (selCell == null) {
        Ext.Msg.alert('Уведомление', 'Необходимо выбрать задачу.');
        return;
    }

    var recId = dsTasks.getAt(selCell[0]).get('Id');

    Ext.net.DirectMethod.request({
        url: '/ConsPumper/PumpCollectTask',
        cleanRequest: true,
        params: {
             taskId: recId,
             fxProgressConfig: { message: 'Выполняется передача данных...' }
        },
        timeout: 30 * 60 * 1000,
        success: handlerSuccess,
        failure: handlerFailure
    });
};

function deleteTask() {
    var sm = gpTasks.getSelectionModel();
    if (sm.hasSelection()) {
        var rowId = sm.selection.record.get("Id");
        Ext.net.DirectMethod.request({
            url: '/ConsCollectingTasks/Delete',
            cleanRequest: true,
            timeout: 60 * 1000 * 60,
            params: { taskId: rowId },
            success: handlerSuccess,
            failure: handlerFailure
        });
    }
};

function handlerSuccess(response, result) {
    if (result.extraParams != undefined && result.extraParams.msg != undefined) {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: result.extraParams.msg,
            title: 'Уведомление',
            hideDelay: 2500
        });
    } else if (response.extraParams != undefined && response.extraParams.msg != undefined) {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: response.extraParams.msg,
            title: 'Уведомление',
            hideDelay: 2500
        });

    } else if (response) {
        Ext.Msg.alert('Сообщение', response);
    } else {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: 'Ок',
            title: 'Уведомление',
            hideDelay: 2500
        });
    }
};

function handlerFailure(response, result) {
    if (result.extraParams != undefined && result.extraParams.responseText != undefined) {
        Ext.Msg.alert('Ошибка', result.extraParams.responseText);
    } else {
        var responseParams = Ext.decode(result.responseText);
        if (responseParams != undefined && responseParams.extraParams != undefined && responseParams.extraParams.responseText != undefined) {
            Ext.Msg.alert('Ошибка', responseParams.extraParams.responseText);
        } else {
            Ext.Msg.alert('Ошибка', 'Server failed');
        }
    }
};