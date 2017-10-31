Ext.ns("EO15AIP.View.Plan");

EO15AIP.View.Plan.Grid = {
    addRecordInGrid: function (gp, objId, stateId, stateName) {
        gp.insertRecord();
        var record = gp.store.data.items[0];
        record.beginEdit();
        record.set('CObjectId', objId);
        record.set('StatusDId', stateId);
        record.set('StatusDName', stateName);
        record.endEdit();
    },
    acceptSourceFinanceHandler: function (winId, gpId) {
        var win = Ext.getCmp(winId);
        var gp = Ext.getCmp(gpId);
        var rec = win.getBody().Extension.entityBook.selectedRecord;
        var recordToUpdate = gp.selModel.selection.record;
        recordToUpdate.beginEdit();
        recordToUpdate.set('SourceFinanceId', rec.data.ID);
        recordToUpdate.set('SourceFinance', rec.data.NAME);
        recordToUpdate.endEdit();
        win.hide();
    }
};