Ext.ns("EO15AIP.Control.StatusD");

EO15AIP.Control.StatusD = {
    updateButtonsClient: function (state, gp) {
        Ext.getCmp("toEdit_" + gp.id).hide();
        Ext.getCmp("toAccept_" + gp.id).hide();
        if (state == 1) {
            Ext.getCmp("toReview_" + gp.id).show();
        }
        else {
            Ext.getCmp("toReview_" + gp.id).hide();
        }
    },

    updateButtonsCoord: function (state, gp) {
        if (state == 2) {
            Ext.getCmp("toEdit_" + gp.id).show();
            Ext.getCmp("toReview_" + gp.id).hide();
            Ext.getCmp("toAccept_" + gp.id).show();
        }
        else if (state == 3) {
            Ext.getCmp("toEdit_" + gp.id).show();
            Ext.getCmp("toReview_" + gp.id).hide();
            Ext.getCmp("toAccept_" + gp.id).hide();
        }
        else {
            Ext.getCmp("toEdit_" + gp.id).hide();
            Ext.getCmp("toReview_" + gp.id).hide();
            Ext.getCmp("toAccept_" + gp.id).hide();
        }
    },

    rowClick: function (gp, rowIndex, isCoord, isClient) {
        var stateId = gp.store.getAt(rowIndex).get('StatusDId');
        if (isCoord == true) {
            EO15AIP.Control.StatusD.updateButtonsCoord(stateId, gp);
        }
        if (isClient == true) {
            EO15AIP.Control.StatusD.updateButtonsClient(stateId, gp);
        }
    },

    rowKeySelect: function (gp, e, isCoord, isClient) {
        var rowIndex = gp.selModel.selection.cell[0];
        EO15AIP.Control.StatusD.rowClick(gp, rowIndex, isCoord, isClient);
    },

    toEdit: function (gp, isCoord, isClient) {
        var s = gp.getSelectionModel().getSelections();
        if (s.length > 0) {
            for (i = 0; i < s.length; i++) {
                if ((s[i].get('StatusDId') == 2 || s[i].get('StatusDId') == 3) && isCoord) {
                    var recordToUpdate = gp.selModel.selection.record;
                    recordToUpdate.beginEdit();
                    recordToUpdate.set('StatusDId', 1);
                    recordToUpdate.set('StatusDName', 'На редактировании');
                    recordToUpdate.modified['StatusDId'] = true;
                    recordToUpdate.modified['StatusDName'] = true;
                    recordToUpdate.endEdit();
                }
            }
            if (isCoord == true) {
                EO15AIP.Control.StatusD.updateButtonsCoord(1, gp);
            }
            if (isClient == true) {
                EO15AIP.Control.StatusD.updateButtonsClient(1, gp);
            }
        }
    },

    toReview: function (gp, isCoord, isClient) {
        var s = gp.getSelectionModel().getSelections();
        if (s.length > 0) {
            for (i = 0; i < s.length; i++) {
                if (s[i].get('StatusDId') == 1 && isClient) {
                    var record = gp.selModel.selection.record;
                    record.beginEdit();
                    record.set('StatusDId', 2);
                    record.set('StatusDName', 'На рассмотрении');
                    record.modified['StatusDId'] = true;
                    record.modified['StatusDName'] = true;
                    record.endEdit();
                }
            }
            if (isCoord == true) {
                EO15AIP.Control.StatusD.updateButtonsCoord(2, gp);
            }
            if (isClient == true) {
                EO15AIP.Control.StatusD.updateButtonsClient(2, gp);
            }
        }
    },

    toAccept: function (gp, isCoord, isClient) {
        var s = gp.getSelectionModel().getSelections();
        if (s.length > 0) {
            for (i = 0; i < s.length; i++) {
                if (s[i].get('StatusDId') == 2 && isCoord) {
                    var record = gp.selModel.selection.record;
                    record.beginEdit();
                    record.set('StatusDId', 3);
                    record.set('StatusDName', 'Утверждено');
                    record.modified['StatusDId'] = true;
                    record.modified['StatusDName'] = true;
                    record.endEdit();
                }
            }
            if (isCoord == true) {
                EO15AIP.Control.StatusD.updateButtonsCoord(3, gp);
            }
            if (isClient == true) {
                EO15AIP.Control.StatusD.updateButtonsClient(3, gp);
            }
        }
    },

    rendererFn: function (r, gp) {
        if (r.data.StatusDId > 0) {
            var tpl = '<img title=\'{1}\' src=\'{0}\' width=\'16\' height=\'16\' />';
            return String.format(
                        tpl,
                        Ext.getCmp(String.format('UrlIconStatusD{0}_{1}', r.data.StatusDId, gp.id)).getValue(), r.data.StatusDName);
        }
    },

    toggleFilter: function (button, state) {
        this.ownerCt.ownerCt.reload();
    },

    getStateFilter: function (gp) {
        var filter = [true, true, true];
        var statusDFilter1 = Ext.getCmp("statusDFilter1_" + gp.id);
        var statusDFilter2 = Ext.getCmp("statusDFilter2_" + gp.id);
        var statusDFilter3 = Ext.getCmp("statusDFilter3_" + gp.id);
        filter[0] = statusDFilter1.pressed;
        filter[1] = statusDFilter2.pressed;
        filter[2] = statusDFilter3.pressed;
        return filter;
    },

    checkEdit: function (e, isCoord, isClient) {
        return EO15AIP.Control.StatusD.checkEditRecord(e.record, isCoord, isClient);
    },

    checkEditRecord: function (record, isCoord, isClient) {
        var status = record.get('StatusDId');
        if (isCoord && (status == 2 || status == 3))
            return true;

        if (isClient && status == 1)
            return true;

        return false;
    }
};