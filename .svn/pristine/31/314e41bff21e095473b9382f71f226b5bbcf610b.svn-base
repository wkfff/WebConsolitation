/// <reference path="vswd-ext_2.2.js" />
var taskLoaded = function(store, records) {
    if (records.length > 0) {
        GeneralForm.form.loadRecord(records[0]);
    }
    else {
        GeneralForm.form.reset();
    }
    initUI(records[0].data.Editable, records[0].data.Actions, records[0].data.CanDoAction);
    TaskPanel.el.unmask();
    taskToolbar.doLayout();
}

var initUI = function(editable, actions, canDoAction) {
    btnApplay.setDisabled(!editable);
    btnCancel.setDisabled(!editable);

    // скрываем фантомную кнопку
    btnPhantom.hide();

    // Добавляем кнопки действий
    if (!editable) {
        // удаляем прежние кнопки
        for (var i = 0; i < 6; i++) {
            var btn = Ext.get('btnChangeState' + i);
            if (btn) {
                btn.remove();
            }
        }
        if (canDoAction) {
            // добавляем новые
            for (var i = 0; i < actions.length; i++) {
                var btn = new Ext.Button({
                    id: "btnChangeState" + i,
                    text: actions[i],
                    xtype: "tbbutton",
                    iconCls: "icon-coggo",
                    listeners: { click: { fn: onChangeStateClick} }
                });
                taskToolbar.addButton(btn);
            }
        }
    } else {
        // удаляем кнопки управления состоянием
        for (var i = 0; i < 6; i++) {
            var btn = Ext.get('btnChangeState' + i);
            if (btn) {
                btn.remove();
            }
        }
    }

    TaskID.setDisabled(true);
    disableTaskForm(!editable);
}

function disableTaskForm(disable){
    fHeadline.setDisabled(disable);
    fTask.setDisabled(disable);
    fState.setDisabled(disable);
    fFromDate.setDisabled(disable);
    fToDate.setDisabled(disable);
    fOwner.setDisabled(disable);
    fDoer.setDisabled(disable);
    fCurator.setDisabled(disable);
    fDescription.setDisabled(disable);
    fCashedAction.setDisabled(disable);
}

function onApplayClick(el,e){
    GeneralForm.form.submit({
        waitMsg:'Сохранение...', 
        params:{id: TaskID.getValue()}, 
        success: successApplayHandler, 
        failure: failureApplayHandler});
}

function successApplayHandler(form, action){
    ViewPort1.getEl().unmask();
    dsTask.reload();
}

function failureApplayHandler(form, action){
    ViewPort1.getEl().unmask();
    switch (action.failureType) {
        case Ext.form.Action.CLIENT_INVALID:
            Ext.Msg.alert('Failure', 'Form fields may not be submitted with invalid values');
            break;
        case Ext.form.Action.CONNECT_FAILURE:
            Ext.Msg.alert('Ошибка', 'Ajax communication failed');
            break;
        case Ext.form.Action.SERVER_INVALID:
            if (action.result != undefined) {
                if (action.result.msg != undefined) {
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
}

function onCancelClick(el,e){
    //ViewPort1.getEl().mask('Выполнение операции...');
    Ext.Ajax.request({
        url: '/Task/Cancel',
        waitMsg:'Сохранение...', 
        params:{id: TaskID.getValue()},
        success: successApplayHandler,
        failure: failureApplayHandler
    });
}

function onChangeStateClick(el,e){
    if (el.text == 'Удалить'){
        Ext.Msg.show({
            title:'Предупреждение',
            msg: 'Вы действительно хотите удалить задачу?',
            buttons: Ext.Msg.YESNO,
            fn: processResultDeleteQuestion,
            animEl: el,
            icon: Ext.MessageBox.WARNING,
            state: el.text
        });    
    }else{
        processResultDeleteQuestion('yes', '', {state: el.text});
    }
}

function processResultDeleteQuestion(buttonId, text, opt){
    if (buttonId == 'yes'){
        ViewPort1.getEl().mask('Выполнение операции...');
        Ext.Ajax.request({
            url: '/Task/SetState',
            params:{id: TaskID.getValue(), state: opt.state},
            success: successChangeStateHandler,
            failure: failureChangeStateHandler
        });
    }
}

function successChangeStateHandler(response, options){
    ViewPort1.getEl().unmask();
    if (options.params.state == 'Удалить'){
        parent.tpMain.remove(parent.tpMain.activeTab, true)
        return;
    }
    dsTask.reload();
}

function failureChangeStateHandler(response, options){
    ViewPort1.getEl().unmask();
    Ext.Msg.alert('Failure', response);
}

var grdDocumentsCommand = function(cmd, record, rowIndex, colIndex) {
    switch (cmd) {
        case "attachDocument":
            attachDocument(record);
            break;
        case "openDocument":
            location.href = "/Task/GetDocument?taskId=" + TaskID.getValue() + '&documentId=' + record.data.ID;
            break;
    }
}

function attachDocument(record) {
    if (btnApplay.disabled){
        Ext.Msg.show({
            title:'Действие запрещено',
            msg:'Для прикрепления файлов необходимо взять задачу на редактирование.',
            buttons: Ext.Msg.OK,
            animEl: 'grdDocuments',
            icon: Ext.MessageBox.WARNING
        });
        return;
    }
    UploadWindow.Document = record.data; 
    UploadWindow.show('grdDocuments');
}
