Ext.ns('E86n.View');
E86n.View.StateSystem =
{
    getMasterID: function (gridName) {
        var grid = Ext.getCmp(gridName);
        if (grid.getSelectionModel().hasSelection()) {
            var row = grid.getSelectionModel().getSelected();
            return row.data.ID;
        } else return -1;
    },

    RowSelect: function (part, record) {
        if (record.dirty) {
            //Запись изменена и не сохранена!
            switch (part) {
                case 'MasterGrid':
                    window.DetailsTabPanel.setDisabled(true);
                    break;

                case 'OptionsTransition':
                    window.RightsTransition.setDisabled(true);
                    break;
            }
        }
        else {
            //Запись в порядке, открываем детализацию!
            switch (part) {
                case 'MasterGrid':
                    window.DetailsTabPanel.setDisabled(false);
                    window.E86n.View.StateSystem.reloadDetail();
                    break;

                case 'OptionsTransition':
                    window.RightsTransition.setDisabled(false);
                    window.RightsTransitionStore.reload();
                    break;
            }
        };
    },

    reloadDetail: function () {
        var activeTab = window.DetailsTabPanel.getActiveTab();

        if (activeTab.id == 'States') {
            window.StatesStore.reload();
        };

        if (activeTab.id == 'Transitions') {
            window.TransitionsStore.reload();
            window.StatesLPStore.reload();
        };
    },

    prepareCellCommand: function (grid, command, record) {
        var clsIco = "icon-" + record.get("Ico");
        command.iconCls = clsIco.toLowerCase();
    },

    cmdHandler: function (cmd, record) {
        switch (cmd) {
            case "EditTransitions":
                {
                    window.OptionsTransitionStore.baseParams.MasterId = record.data.ID;
                    window.TransitionLPStore.reload();
                    window.OptionsTransitionStore.reload();
                    window.RightsTransitionStore.reload();

                    window.wndOptionsTransition.show();
                }
                break;
        }
    },

    wndOptionsTransitionBtnOk: function () {
        window.OptionsTransition.getSelectionModel().clearSelections();
        window.RightsTransition.setDisabled(true);
        window.RightsTransitionStore.reload();
        window.OptionsTransitionStore.reload();
        window.wndOptionsTransition.hide();
    },

    prepareToolbar: function (grid, toolbar, rowIndex, record) {
        if (record.dirty) {
            toolbar.setDisabled(true);
        } else {
            toolbar.setDisabled(false);
        }
    }
};