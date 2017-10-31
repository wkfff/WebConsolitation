ViewPersistence.isDirty = function () {
    return false;
}

ViewPersistence.refresh = function () {
    targetMarkStore.reload()
}

ShowDifferenceWarning = function () {
    var warningsFound = false;
    for (i = 0; i < targetFactsStore.getCount(); i++) {
        warningsFound = warningsFound || (targetFactsStore.getAt(i).get('HasWarning') != '');
    }
    if (warningsFound) {
        Ext.Msg.show({
            title: 'Внимание!',
            msg: 'В некоторых МО есть устаревшие значения показателя!',
            buttons: Ext.Msg.OK,
            animEl: 'targetFactsGrid',
            icon: Ext.MessageBox.EXCLAMATION
        });
    }
}

ToggleSourceDataPanel = function () {
    if (sourceDataGrid.collapsed) {
        sourceDataGrid.expand(true);
    } else {
        sourceDataGrid.collapse(true);
    }
}

FactsBackToEditHandler = function () {
    var s = targetFactsGrid.getSelectionModel().getSelections();
    for (i = 0; i < s.length; i++) {
        s[i].set('RefStatusData', 1);
    }
    targetFactsGrid.save();
}

FactsApproveHandler = function () {
    var s = targetFactsGrid.getSelectionModel().getSelections();
    for (i = 0; i < s.length; i++) {
        if (s[i].get('RefStatusData') < 3) {
            s[i].set('RefStatusData', 3);
        }
    }
    targetFactsStore.save();
}

TargetMarkStoreLoadHandler = function () {
    targetMarkForm.getForm().loadRecord(targetMarkStore.getAt(0));
    sourceFactsGrid.setDisabled(true);
    sourceFactsGrid.collapse(true);
}