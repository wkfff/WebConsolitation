Ext.ns("EO15AIP.View.AIP");

var beforeCloseAIP = function (currentTab) {
    if (currentTab.forceClose != undefined && currentTab.forceClose) {
        return true;
    }

    currentTab.ownerCt.setActiveTab(currentTab);
    if (AIPGrid.isDirty() || CObjectGrid.isDirty()) {
        Ext.Msg.show({
            title: 'Внимание',
            msg: 'Все несохраненные изменения будут потеряны. Сохранить данные?',
            buttons: { yes: 'Сохранить', no: 'Закрыть', cancel: 'Отмена' },
            animEl: 'MdiTab',
            icon: Ext.MessageBox.WARNING,
            fn: function (buttonId) {
                if (buttonId == 'yes') {
                    AIPGrid.save();
                    CObjectGrid.save();
                }
                else if (buttonId == 'no') {
                    currentTab.forceClose = true;
                    currentTab.ownerCt.closeTab(currentTab);
                }
            }
        });
        return false;
    }
    else {
        return true;
    }
};

EO15AIP.View.AIP = {
    updateObjectsList: function (aipStore, objectsStore, rowIndex) {
        objectsStore.baseParams.aipId = aipStore.getAt(rowIndex).data.AIPId;
        objectsStore.reload();
    },

    objListDropFn: function (ddSource, e, data) {
        // Loop through the selections
        Ext.each(ddSource.dragData.selections, function (record) {
            if (CObjectsStore.baseParams.aipId > 0) {
                // Search for duplicates
                var foundItem = CObjectsStore.findExact('CObjectId', record.data.CObjectId);

                //Remove Record from the source
                ddSource.grid.store.remove(record);

                // if not found
                if (foundItem < 0) {
                    record.data.AIPId = CObjectsStore.baseParams.aipId;
                    CObjectsStore.add(record);

                    // Call a sort dynamically
                    CObjectsStore.sort('CObjectName', 'ASC');
                }
            }
        });

        return true;
    },

    objAllListDropFn: function (ddSource, e, data) {
        // Loop through the selections
        Ext.each(ddSource.dragData.selections, function (record) {
            // Search for duplicates
            var foundItem = CObjectsStoreAll.findExact('CObjectId', record.data.CObjectId);

            //Remove Record from the source
            ddSource.grid.store.remove(record);

            // if not found
            if (foundItem < 0) {
                CObjectsStoreAll.add(record);

                // Call a sort dynamically
                CObjectsStoreAll.sort('CObjectName', 'ASC');
            }
        });

        return true;
    }
};