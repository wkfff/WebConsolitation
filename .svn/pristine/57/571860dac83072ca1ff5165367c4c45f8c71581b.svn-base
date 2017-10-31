var SaveAreaAttributes = function (statusForSet) {
   areaDetailForm.form.submit({
        waitMsg: 'Сохранение...',
        success: successFormSaveHandler,
        failure: failureFormSaveHandler,
        params: { areaId: ID.getValue(), statusForSet: statusForSet }
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

var SetFieldsEditableOption = function (status) {
    var maneFieldEnabled = status == 1;
    var cmp;
    cmp = Ext.getCmp('lfRefTerritoryName'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Location'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('CadNumber'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Area'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Category'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Owner'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Head'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Contact'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Email'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Phone'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('PermittedUse'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('ActualUse'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Documentation'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Limitation'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('PermConstr'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Relief'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Road'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Station'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Pier'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Airport'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Plumbing'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Sewage'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Gas'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Electricity'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Heating'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Landfill'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Telephone'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Connectivity'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Fee'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('DistanceZones'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Buildings'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Resources'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Settlement'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('ObjectEducation'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('ObjectHealth'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('ObjectSocSphere'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('ObjectServices'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('Hotels'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('CoordinatesLat'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }
    cmp = Ext.getCmp('CoordinatesLng'); if (cmp != undefined) { cmp.setReadOnly(!maneFieldEnabled); }

    cmp = Ext.getCmp('RegNumber'); if (cmp != undefined) { cmp.setReadOnly(status != 2); }
    cmp = Ext.getCmp('Note'); if (cmp != undefined) { cmp.setReadOnly(status == 1); }
};

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

var EnableChangeStatusButtons = function (status) {
    var btnSaveEnabled;
    var btn1Enabled;
    var btn2Enabled;
    var btn3Enabled;
    if (status == 1) {
        btnSaveEnabled = true;
        btn1Enabled = false;
        btn2Enabled = true;
        btn3Enabled = false;
    }
    else if (status == 2) {
        btnSaveEnabled = false;
        btn1Enabled = true;
        btn2Enabled = false;
        btn3Enabled = true;
    }
    else if (status == 3) {
        btnSaveEnabled = false;
        btn1Enabled = true;
        btn2Enabled = false;
        btn3Enabled = false;
    }
    else {
        alert('Ошибка. Неверное значение статуса!');
    }
    if (btnToEdit != undefined) {
        btnToEdit.setDisabled(!btn1Enabled);
    }
    if (btnToReview != undefined) {
        btnToReview.setDisabled(!btn2Enabled);
    }
    if (btnToAccepted != undefined) {
        btnToAccepted.setDisabled(!btn3Enabled);
    }
    if (btnSave != undefined) {
        btnSave.setDisabled(!btnSaveEnabled);
    }
    if (btnSaveFiles != undefined) {
        btnSaveFiles.setDisabled(!btnSaveEnabled);
    }
};

var onBeforeWindowHideEventHandler = function () {
    var window = this;
    if (window.forceHide != undefined && window.forceHide) {
        window.forceHide = false;
        return true;
    }
    var winBody = window.getBody();
    if (winBody.areaDetailForm.isDirty()) {
        Ext.Msg.show({
            title: 'Внимание',
            msg: 'При закрытии все несохраненные изменения будут потеряны. Закрыть?',
            buttons: Ext.Msg.YESNO,
            animEl: 'viewportMain',
            icon: Ext.MessageBox.WARNING,
            fn: function (buttonId) {
                if (buttonId == "yes") {
                    window.forceHide = true;
                    window.hide();
                }
            }
        });
        return false;
    }
    else {
        return true;
    }
};