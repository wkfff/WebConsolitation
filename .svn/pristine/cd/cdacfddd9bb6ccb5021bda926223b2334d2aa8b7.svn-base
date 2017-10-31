Ext.ns("EO15AIP.View.Review");

EO15AIP.View.Review.Grid = {
    addRecordInGrid: function (gpId, objId, stateId, stateName) {
        var gp = Ext.getCmp(gpId);
        gp.insertRecord();
        var record = gp.store.data.items[0];
        record.beginEdit();
        record.set('CObjectId', objId);
        record.set('StatusDId', stateId);
        record.set('StatusDName', stateName); record.endEdit();
    }
};