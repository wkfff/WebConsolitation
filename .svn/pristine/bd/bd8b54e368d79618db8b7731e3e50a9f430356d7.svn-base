Ext.ns("EO15AIP.View.Contract");

EO15AIP.View.Contract.Grid = {
    acceptContractHandler: function (winId, gpId) {
        var win = Ext.getCmp(winId);
        var gp = Ext.getCmp(gpId);
        var rec = win.getBody().Extension.entityBook.selectedRecord;
        var recordToUpdate = gp.selModel.selection.record;
        recordToUpdate.beginEdit();
        recordToUpdate.set('ContractId', rec.data.ID);
        recordToUpdate.set('ContractName', rec.data.PROPERTY);
        recordToUpdate.set('PartnerId', rec.data.REFPARTNERS);
        recordToUpdate.set('PartnerName', rec.data.LP_REFPARTNERS);
        recordToUpdate.endEdit();
        win.hide();
    },
    acceptStateHandler: function (winId, gpId) {
        var win = Ext.getCmp(winId);
        var gp = Ext.getCmp(gpId);
        var rec = win.getBody().Extension.entityBook.selectedRecord;
        var recordToUpdate = gp.selModel.selection.record;
        recordToUpdate.beginEdit();
        recordToUpdate.set('StateId', rec.data.ID);
        recordToUpdate.set('StateName', rec.data.NAME);
        recordToUpdate.endEdit();
        win.hide();
    },
    addRecordInGrid: function (gpId, objId, objName, stateId, stateName) {
        var gp = Ext.getCmp(gpId);
        var rec = gp.insertRecord();
        var length = gp.store.data.items.length - 1;
        var record = gp.store.data.items[length];
        record.beginEdit();
        record.set('CObjectId', objId);
        record.set('CObjectName', objName);
        record.set('StatusDId', stateId);
        record.set('StatusDName', stateName);
        record.endEdit();
    },
    selectNewValue: function (gp, record, idColumn, nameColumn) {
        var srec;
        if (gp.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {
           srec = gp.getSelectionModel().getSelected();
        }
        else {
            srec = gp.getSelectionModel().selection.record;
        }

        if (srec != null) 
        {
            srec.beginEdit();
            srec.set(idColumn, record.get('ID'));
            srec.set(nameColumn, record.get('NAME'));
            srec.endEdit();
        }
    }
};