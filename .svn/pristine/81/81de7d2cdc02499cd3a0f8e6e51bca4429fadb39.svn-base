Ext.ns('E86n.View.DiverseInfoView');

E86n.View.DiverseInfoView =
    {
        RowSelect: function (record) {
            if (record.dirty) {
                //Запись изменена и не сохранена!
                window.PaymentDetailsTargets.setDisabled(true);
            } else {
                //Запись в порядке, открываем детализацию!
                window.PaymentDetailsTargets.setDisabled(false);
                eval('PaymentDetailsTargetsStore').reload();
            }
        },

        getSelectedPaymentDetailsId: function () {
            var grid = window.Ext.getCmp("PaymentDetails");
            if (grid.getSelectionModel().hasSelection()) {
                return grid.getSelectionModel().getSelected().data.ID;
            } else return -1;
        },

        reloadDetail: function () {
            var activeTab = window.DetailTabPanel.getActiveTab();
            if (activeTab.id == 'PaymentDetailsPanel') {
                eval('PaymentDetailsStore').reload();
            } else {
                eval(activeTab.id + 'Store').reload();
            }
        },

        SetReadOnlyDoc: function (readOnly) {
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'TofkList');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'PaymentDetails');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'PaymentDetailsTargets');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'LicenseDetails');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'AccreditationDetails');
        }
    };