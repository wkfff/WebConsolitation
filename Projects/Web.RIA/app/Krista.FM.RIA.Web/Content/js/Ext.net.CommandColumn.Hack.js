Ext.override(Ext.net.CommandColumn, {
    getRecords: function (groupId) {
        if (groupId) {
            var groupIdFix = groupId.replace(/"/gi, '&quot;');
            var records = this.grid.store.queryBy(function (r) {
                return r._groupId == groupIdFix;
            });
            return records ? records.items : [];
        }
    }
});
