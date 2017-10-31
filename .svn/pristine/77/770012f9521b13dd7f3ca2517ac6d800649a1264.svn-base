Ext.ns('E86n.View.Control');

E86n.View.Control.SearchComboboxControl = {
    getGridActiveRecord: function (grid) {
        if (grid.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {
            return grid.getSelectionModel().getSelected();
        } else {
            return grid.getSelectionModel().selection.record;
        }
    },

    getClearTrigger: function (triggers) {
        return this.getTriggerByTag(triggers, "TriggerClear");
    },

    getTriggerByTag: function (triggers, tag) {
        for (var i = 0, len = triggers.length; i < len; ++i) {
            if (triggers[i].getAttributeNS("ext", "tid") === tag)
                return triggers[i];
        }
        return null;
    },

    setLockupField: function (record, targetRecord, fields) {
        if (!Ext.isEmpty(targetRecord)) {
            targetRecord.beginEdit();
            Ext.iterate(fields, function (key, values) {
                var result = [];
                Ext.each(values, function (value)  {
                    result.push(record.get(value));
                });
                targetRecord.set(key, result.join(";"));
            });
            targetRecord.endEdit();
        }
    },

    clearLockupField: function (targetRecord, fields) {
        if (!Ext.isEmpty(targetRecord)) {
            targetRecord.beginEdit();
            Ext.each(fields, function (field) {
                targetRecord.set(field, null);
            });
            targetRecord.endEdit();
        }
    },
    
    isEmptyLockupField: function (targetRecord, fields) {
        var result = true;

        if (!Ext.isEmpty(targetRecord)) {
            Ext.each(fields, function (field) {
                result &= Ext.isEmpty(targetRecord.get(field));
            });
        }
        
        return result;
    },

    onTriggerClear: function () {
    },
    
    onSelect: function (record, grid, fields, displayField) {
        var targetRecord = this.getGridActiveRecord(grid);
        this.setLockupField(record, targetRecord, fields);
        grid.activeEditor.setValue(targetRecord.get(displayField));
    },
};
