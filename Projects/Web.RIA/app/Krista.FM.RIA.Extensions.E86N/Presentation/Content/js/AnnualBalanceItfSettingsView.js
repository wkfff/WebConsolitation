Ext.ns('E86n.View.AnnualBalanceItfSettingsView');

E86n.View.AnnualBalanceItfSettingsView =
    {
        getPartDoc: function () {
            var val = window.Settings.getSelectionModel().getSelected().data.RefPartDoc;
            if (!val) val = -1;
            return val;
        },
        
        reloadLP: function () {
            window.DetailsLP.reload();
        },
        
        RowSelect: function (grid) {
            var btn = Ext.getCmp(grid + 'RemoveRecordBtn');
            if (btn) btn.enable();
        },
    };