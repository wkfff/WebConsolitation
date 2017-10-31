Ext.ns('Extensions.Messages');

Extensions.Messages.Resive = {
    applyFilter: function(field) {
        var store = Ext.getCmp('messagesGridPanel').getStore();
        store.suspendEvents();
        store.filterBy(Extensions.Messages.Resive.getRecordFilter());
        store.resumeEvents();
        Ext.getCmp('messagesGridPanel').getView().refresh(false);
    },

    getRecordFilter: function() {
        var f = [];
        f.push({
            filter: function(record) {
                return Extensions.Messages.Resive.filterNumber(Ext.getCmp('ComboBoxStatus').getValue(), "Status", record);
            }
        });

        f.push({
            filter: function(record) {
                return Extensions.Messages.Resive.filterNumber(Ext.getCmp('ComboBoxType').getValue(), "MessageType", record);
            }
        });

        f.push({
            filter: function(record) {
                return Extensions.Messages.Resive.filterNumber(Ext.getCmp('ComboBoxSender').getValue(), "Sender", record);
            }
        });

        f.push({
            filter: function(record) {
                return Extensions.Messages.Resive.filterDate(Ext.getCmp('DateFilter').getValue(), "Date", record);
            }
        });

        f.push({
            filter: function(record) {
                return Extensions.Messages.Resive.filterString(Ext.getCmp('SubjectTextField').getValue(), "Subject", record);
            }
        });

        f.push({
            filter: function(record) {
                return Extensions.Messages.Resive.filterNumber(Ext.getCmp('ComboBoxImportance').getValue(), "Importance", record);
            }
        });

        var len = f.length;

        return function(record) {
            for (var i = 0; i < len; i++) {
                if (!f[i].filter(record)) {
                    return false;
                }
            }
            return true;
        };
    },

    filterString: function(value, dataIndex, record) {
        var val = record.get(dataIndex);

        if (typeof val != "string") {
            return value.length == 0;
        }

        return val.toLowerCase().indexOf(value.toLowerCase()) > -1;
    },

    filterDate: function(value, dataIndex, record) {
        var val = record.get(dataIndex).clearTime(true).getTime();

        if (!Ext.isEmpty(value, false) && val != value.clearTime(true).getTime()) {
            return false;
        }

        return true;
    },

    clearFilter: function() {
        Ext.getCmp('ComboBoxStatus').reset();
        Ext.getCmp('ComboBoxType').reset();
        Ext.getCmp('ComboBoxSender').reset();
        Ext.getCmp('DateFilter').reset();
        Ext.getCmp('SubjectTextField').reset();
        Ext.getCmp('ComboBoxImportance').reset();
        Ext.getCmp('messagesGridPanel').getStore().clearFilter();
    },

    filterNumber: function(value, dataIndex, record) {
        var val = record.get(dataIndex);

        if (!Ext.isEmpty(value, false) && val != value) {
            return false;
        }

        return true;
    },

    template: '<span style="color:{0};">{1}</span>',

    change: function(value) {
        return String.format(template, (value > 0) ? "green" : "red", value);
    },

    refresh: function () {
        var store = Ext.getCmp('messagesGridPanel').getStore();
        store.suspendEvents();
        store.reload();
        store.filter('Status', 1);
        Extensions.Messages.Resive.refreshMessageCountCaption(store.getCount());
        store.clearFilter();
        store.filterBy(Extensions.Messages.Resive.getRecordFilter());
        store.resumeEvents();
        Ext.getCmp('messagesGridPanel').getView().refresh(false);
    },

    cloneStore: function(source, target, uclmn) {
        var records = source.getAllRange();
        target.suspendEvents(false);
        target.removeAll();
        target.clearFilter(true);
        var flt = [];
        for (var i = 0; i < records.length; i++) {
            if (!this.contains(flt, records[i].data[uclmn], uclmn))
                flt.push(records[i].copy());
        }
        target.add(flt);
        target.resumeEvents();
    },

    contains: function(a, obj, uclmn) {
        for (var i = 0; i < a.length; i++) {
            if (a[i].data[uclmn] === obj) {
                return true;
            }
        }
        return false;
    },

    isNumber: function(n) {
        return !isNaN(parseFloat(n)) && isFinite(n);
    },
    
    refreshMessageCountCaption: function (n) {
        if (parent.acMessages) {
            if (n != 0) {
                parent.acMessages.setTitle('Сообщения (' + n + ')');
                parent.acMessages.setIconClass('icon-emailstar');
            } else {
                parent.acMessages.setTitle('Сообщения');
                parent.acMessages.setIconClass('icon-email');
            }
        }
}
};
