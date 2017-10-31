var SaveProjectAttributes = function () {
    ProjectAttrForm.form.submit({
        waitMsg: 'Сохранение...',
        success: successFormSaveHandler,
        failure: failureFormSaveHandler,
        params: { projId: ID.getValue() }  
        });
};


var onTabCloseEventHandler = function (currentTab) {
    if (currentTab.forceClose != undefined && currentTab.forceClose) {
        return true;
    }
    var tabBody = currentTab.getBody();
    if (tabBody.ProjectAttrForm.isDirty()) {
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

var onTabChangeEventHandler = function (tabPanel, newTab, currentTab) {
    if (currentTab.forceChange != undefined && currentTab.forceChange) {
        currentTab.forceChange = false;
        return true;
    }
    switch (currentTab.id) {
        case 'InvestTab':
        case 'GosTab':
            var store = currentTab.getBody().dsInvests;
            break;
        case 'TargetRatingsTab':
            var store = currentTab.getBody().dsRatings;
            break;
        case 'VisualizationTab':
            var store = currentTab.getBody().dsFiles;
            break;
        default:
            var store = undefined;
            break;
    }
    if (store != undefined && store.isDirty()) {
        Ext.Msg.show({
            title: 'Внимание',
            msg: 'На вкладке имеются несохраненные изменения. Уйти?',
            buttons: Ext.Msg.YESNO,
            animEl: 'viewportMain',
            icon: Ext.MessageBox.WARNING,
            fn: function (buttonId) {
                if (buttonId == 'yes') {
                    currentTab.forceChange = true;
                    currentTab.ownerCt.setActiveTab(newTab);
                }
            }
        });
        return false;
    }
};

var SetFieldsEditableOption = function (editable) {
    var cmp = Ext.getCmp('Name'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('Code'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('comboRefPart'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('lfRefTerritoryName'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('Goal'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('InvestorName'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('LegalAddress'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('MailingAddress'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('Email'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('Phone'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('Contact'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('Study'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('Effect'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('RefBeginDateVal'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('RefEndDateVal'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('ExpectedOutput'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('Production'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('PaybackPeriod'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('SumInvestPlan'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('DocBase'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('InvestAgreement'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('AddMech'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('ExpertOpinion'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('lfRefOKVEDName'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('IncomingDate'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
    cmp = Ext.getCmp('RosterDate'); if (cmp != undefined) { cmp.setReadOnly(!editable); }
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
