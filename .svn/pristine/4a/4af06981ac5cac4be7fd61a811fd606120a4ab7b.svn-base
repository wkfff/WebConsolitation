Ext.ns('E86n.View.Control');

E86n.View.Control.UiExtensions = {
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

    setLockupField: function (record, targetRecord, fields, setupField, toUpper) {
        if (!Ext.isEmpty(targetRecord)) {
            targetRecord.beginEdit();
            Ext.iterate(fields, function (key, values) {
                var result = "";
                var summ = [];
                Ext.each(values, function (value) {
                    if (value == setupField) {
                        result = record.get(toUpper ? value.toUpperCase() : value);
                    }
                    summ.push(record.get(toUpper ? value.toUpperCase() : value));
                });
                if (result == "") {
                    targetRecord.set(key, summ.join(";"));
                } else {
                    targetRecord.set(key, result);
                }
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

    onSelect: function (record, grid, fields, displayField, setupField, toUpper, comboBox) {
        var targetRecord = this.getGridActiveRecord(grid);
        this.setLockupField(record, targetRecord, fields, setupField, toUpper);
        grid.activeEditor.setValue(targetRecord.get(displayField));
        comboBox.setVisible(false);
    }
};

E86n.View.Control.DBComboBox = {
    onSelect: function (record, fields, display, toUpper, comboBox) {
        Ext.iterate(fields, function (key, values) {
            var summ = [];
            Ext.each(values, function (value) {
                summ.push(record.get(toUpper ? value.toUpperCase() : value));
            });
            if (key == display) {
                comboBox.setValue(summ.join(";"));
            } else {
                var element = Ext.getCmp(key);
                if (element) {
                    element.setValue(summ.join(";"));
                }
            }
        });
    }
};