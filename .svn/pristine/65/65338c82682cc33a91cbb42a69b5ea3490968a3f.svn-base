Ext.ns("EO15AIP.View.AdditObjectInfo");

EO15AIP.View.AdditObjectInfo.Grid = {
    addRecordInGrid: function (gpId, objId) {
        var gp = Ext.getCmp(gpId);
        gp.insertRecord();
        var record = gp.store.data.items[0];
        record.set('CObjectId', objId);
        record.set('PeriodId', gp.store.baseParams.PeriodId);
    }
};