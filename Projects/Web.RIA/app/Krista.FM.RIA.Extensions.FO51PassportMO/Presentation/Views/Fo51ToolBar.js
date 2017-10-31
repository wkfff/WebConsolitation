Ext.override(Ext.net.GridPanel, {
    reconfigure: function (store, colModel) {
        Ext.net.GridPanel.superclass.reconfigure.call(this, store, colModel);
        if (this.saveMask) {
            if (this.saveMask.destroy != undefined) {
                this.saveMask.destroy();
            }
            this.saveMask = new Ext.net.SaveMask(this.bwrap,
                Ext.apply({ writeStore: store }, this.initialConfig.saveMask));
        }
    }
});

Ext.override(Ext.net.SaveMask, {
    onBeforeSave: function () {
        if (!this.disabled && this.el != null && this.el != undefined) {
            this.el.mask(this.msg, this.msgCls);
        }
    },
    onSave: function () {
        if (this.el != null && this.el != undefined) {
            this.el.unmask(this.removeMask);
        }
    }
});

var copyPrevEstimate = function (storeFrom, gp) {
    var cnt = storeFrom.data.getCount();
    for (var i = 0; i < cnt; i++) {
        if (gp.store.getAt(i).data.IsLeaf) {
            gp.store.getAt(i).data.ScoreMO1 = storeFrom.getAt(i).data.ScoreMO1;
            gp.store.getAt(i).data.ScoreMO2 = storeFrom.getAt(i).data.ScoreMO2;
            gp.store.getAt(i).data.ScoreMO3 = storeFrom.getAt(i).data.ScoreMO3;
            gp.store.getAt(i).data.ScoreMO4 = storeFrom.getAt(i).data.ScoreMO4;
            gp.store.getAt(i).data.ScoreMO5 = storeFrom.getAt(i).data.ScoreMO5;
            gp.store.getAt(i).data.ScoreMO6 = storeFrom.getAt(i).data.ScoreMO6;
            gp.store.getAt(i).data.ScoreMO7 = storeFrom.getAt(i).data.ScoreMO7;
            gp.store.getAt(i).data.ScoreMO8 = storeFrom.getAt(i).data.ScoreMO8;
            gp.store.getAt(i).data.ScoreMO9 = storeFrom.getAt(i).data.ScoreMO9;
            gp.store.getAt(i).data.ScoreMO10 = storeFrom.getAt(i).data.ScoreMO10;
            gp.store.getAt(i).data.ScoreMO11 = storeFrom.getAt(i).data.ScoreMO11;
            gp.store.getAt(i).data.ScoreMO12 = storeFrom.getAt(i).data.ScoreMO12;
            gp.store.getAt(i).dirty = true;
            gp.store.getAt(i).modified = [];
            gp.store.getAt(i).modified['ScoreMO1'] = true;
            gp.store.getAt(i).modified['ScoreMO2'] = true;
            gp.store.getAt(i).modified['ScoreMO3'] = true;
            gp.store.getAt(i).modified['ScoreMO4'] = true;
            gp.store.getAt(i).modified['ScoreMO5'] = true;
            gp.store.getAt(i).modified['ScoreMO6'] = true;
            gp.store.getAt(i).modified['ScoreMO7'] = true;
            gp.store.getAt(i).modified['ScoreMO8'] = true;
            gp.store.getAt(i).modified['ScoreMO9'] = true;
            gp.store.getAt(i).modified['ScoreMO10'] = true;
            gp.store.getAt(i).modified['ScoreMO11'] = true;
            gp.store.getAt(i).modified['ScoreMO12'] = true;

            gp.store.modified.push(gp.store.getAt(i));
        }
    }
    gp.getView().refresh();
}

var saveMesOtchHandler = function (regionId, waitTitle, waitText, grid, markId) {
    Ext.net.DirectMethod.request({
        url: '/FO51FormSbor/SaveMesOtch?periodId=' + cbPeriodMonth.value + '&regionId=' + regionId + '&parentMarkId=' + markId,
        timeout: 600000,
        params: {
            fxProgressConfig: { message: 'Получение данных месячной отчетности...' }
        },
        success: function (response, options) {
            if (response.success) {
                grid.reload();
                
                Ext.net.Notification.show({
                    iconCls: 'icon-information',
                    html: response.message,
                    title: 'Внимание',
                    hideDelay: 10000
                });
            }
            else {
                Ext.net.Notification.show({
                    iconCls: 'icon-information',
                    html: response.message,
                    title: 'Внимание',
                    hideDelay: 10000
                });
            }
        },
        failure: function (response, options) {
            var fi = options.responseText.indexOf('message:') + 9;
            var li = options.responseText.lastIndexOf('"')
            var msg = options.responseText.substring(li, fi);
            Ext.net.Notification.show({
                iconCls: 'icon-information',
                html: msg,
                title: 'Внимание',
                hideDelay: 10000
            });
        }
    });
};

var changeStateHandler = function (grid, state, waitTitle, successText, stateName, isOGV) {
    Ext.net.DirectMethod.request({
        url: '/FO51FormSbor/ChangeState?periodId=' + cbPeriodMonth.getValue() +
            '&state=' + state + '&regionId=' + grid.selModel.selection.record.data.ID,
        params: {
            fxProgressConfig: { message: 'Смена состояния данных...' }
        },
        success: function (response, options) {
            if (response.success) {
                grid.selModel.selection.record.set('StatusID', state);
                grid.selModel.selection.record.set('StatusName', stateName);
                if (isOGV)
                    updateButtonsOGV(state);
                else
                    updateButtonsMO(state);

                Ext.net.Notification.show({
                    iconCls: 'icon-information',
                    html: response.message,
                    title: 'Внимание',
                    hideDelay: 10000
                });
            }
            else {
                Ext.net.Notification.show({
                    iconCls: 'icon-information',
                    html: 'Действие не выполнено',
                    title: 'Внимание',
                    hideDelay: 10000
                });
            }
        },
        failure: function (response, options) {
            var fi = options.responseText.indexOf('message:') + 9;
            var li = options.responseText.lastIndexOf('"')
            var msg = options.responseText.substring(li, fi);
            Ext.net.Notification.show({
                iconCls: 'icon-information',
                html: msg,
                title: 'Внимание',
                hideDelay: 10000
            });
        }
    });
};

var updateButtonsMO = function (state) {
    if (state == 1) {
        SendEditBtn.hide();
        AcceptBtn.hide();
        SendDFBtn.show();
    }
    else {
        SendEditBtn.hide();
        AcceptBtn.hide();
        SendDFBtn.hide();
    }
};

var updateButtonsOGV = function (state) {
    if (state == 2) {
        SendEditBtn.show();
        AcceptBtn.show();
        SendDFBtn.hide();
    }
    else if (state == 3) {
        SendEditBtn.hide();
        AcceptBtn.hide();
        SendDFBtn.show();
    }
    else {
        SendEditBtn.hide();
        AcceptBtn.hide();
        SendDFBtn.hide();
    }
};

var changeStateReloadHandler = function (regionId, state, waitTitle, waitText, isOGV) {
    /*saveHandler();*/
    Ext.net.DirectMethod.request({
        url: '/FO51FormSbor/ChangeState?periodId=' + cbPeriodMonth.value + '&state=' + state + '&regionId=' + regionId,
        timeout: 600000,
        params: {
            fxProgressConfig: { message: 'Смена состояния данных...' }
        },
        success: function (response, options) {
            if (response.success) {
                reloadHandler();
                if (isOGV)
                    updateButtonsOGV(state);
                else
                    updateButtonsMO(state);

                Ext.net.Notification.show({
                    iconCls: 'icon-information',
                    html: response.message,
                    title: 'Внимание',
                    hideDelay: 10000
                });
            }
            else {
                Ext.net.Notification.show({
                    iconCls: 'icon-information',
                    html: response.message,
                    title: 'Внимание',
                    hideDelay: 10000
                });
            }
        },
        failure: function (response, options) {
            var fi = options.responseText.indexOf('message:') + 9;
            var li = options.responseText.lastIndexOf('"')
            var msg = options.responseText.substring(li, fi);
            Ext.net.Notification.show({
                iconCls: 'icon-information',
                html: msg,
                title: 'Внимание',
                hideDelay: 10000
            });
        }
    });
};

var reloadHandler = function() 
{
};

var saveHandler = function() 
{
};