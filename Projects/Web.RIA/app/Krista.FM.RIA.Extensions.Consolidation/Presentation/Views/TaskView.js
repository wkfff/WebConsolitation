
var onTabCloseEventHandler = function (currentTab) {
    if (currentTab.forceClose != undefined && currentTab.forceClose) {
        return true;
    }
    var tabBody = currentTab.getBody();
    if (tabBody.taskForm.isDirty() || tabBody.gpTaskFiles.isDirty()) {
        Ext.Msg.show({
            title: 'Внимание',
            msg: 'При закрытии все несохраненные изменения будут потеряны. Закрыть?',
            buttons: Ext.Msg.YESNO,
            animEl: 'viewportMain',
            icon: Ext.MessageBox.WARNING,
            fn: function (buttonId) {
                if (buttonId == "yes") {
                    currentTab.forceClose = true;
                    currentTab.ownerCt.closeTab(currentTab);
                }
            }
        });

        return false;
    }
    else {
        return true;
    }
};

function successSaveHandler(response, result) {
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

    } else {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: 'Ок',
            title: 'Уведомление',
            hideDelay: 2500
        });
    }
};

function failureSaveHandler(response, result) {
    if (result.extraParams != undefined && result.extraParams.responseText != undefined) {
        Ext.Msg.alert('Ошибка', result.extraParams.responseText);
    } else {
        var responseParams = Ext.decode(result.responseText);
        if (responseParams != undefined && responseParams.extraParams != undefined && responseParams.extraParams.responseText != undefined) {
            Ext.Msg.alert('Ошибка', responseParams.extraParams.responseText);
        } else if (responseParams.errorMessage) {
            Ext.Msg.alert('Ошибка', responseParams.errorMessage);
        } else {
            Ext.Msg.alert('Ошибка', 'Server failed');
        }
    }
};

function resetDirtyAttributeOnFormItems() {
    taskForm.getForm().items.each(function (f) {
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

function saveForm(newStatus) {
    if ((newStatus != undefined) && (newStatus != null)) {
        var reportTab = parent.MdiTab.getComponent('consReport_' + taskId.getValue());
        if (reportTab) {
            if (reportTab.getBody().isDirty()) {
                Ext.Msg.alert(
                    'Предупреждение', 
                    'Необходимо сохранить внесенные в отчет изменения перед изменением состояния задачи.',
                    function () { parent.MdiTab.setActiveTab(reportTab); },
                    this);
                return;
            }
        }
        refStatusId.setValue(newStatus);
    }
    
    Ext.net.DirectMethod.request({
        url: '/ConsTask/Save',
        cleanRequest: true,
        params: {
            taskId: taskId.getValue(),
            refStatusId: refStatusId.getValue(),
            deadlineStr: Ext.util.JSON.encodeDate(deadline.getValue()),
            newComment: newComment.getValue(),
            taskFilesChangedData: gpTaskFiles.store.getChangedData()
        },
        success: successSaveHandler,
        failure: failureSaveHandler
    });
};

function doPump() {
    Ext.net.DirectMethod.request({
        url: '/ConsPumper/PumpReport',
        cleanRequest: true,
        params: { taskId: taskId.getValue() },
        eventMask: { showMask: true, msg: "Выполняется передача данных..." },
        timeout: 10 * 60 * 1000,
        success: successSaveHandler,
        failure: failureSaveHandler
    });
}