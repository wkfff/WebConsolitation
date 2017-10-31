//  key для активного источника
activeSource = -1;
// список источников еще не был загружен
var notLoaded = true;

/* выбран источник, обновление данных */
var sourceHandler = function (comboSource) {
    if (comboSource.selectedIndex == -1) {
        if (activeSource == -1) {
            comboSource.value = comboSource.store.reader.jsonData.extraParams;
            activeSource = comboSource.value;
        } else {
            comboSource.value = activeSource;
        }
    }
    sourceID.setText('(ID Источника = ' + comboSource.value + ')');
    sourceID.show();
    comboSource.initValue();
    var gridShowHierarchy = Ext.getCmp(GridId);
    gridShowHierarchy.getSelectionModel().clearSelections(false);
    gridShowHierarchy.store.baseParams = gridShowHierarchy.store.baseParams | { source: comboSource.value };
    gridShowHierarchy.store.load();
    gridShowHierarchy.getView().refresh();
}

// перед загрузкой списка источников устанавливаем значения передаваемых серверу параметров
var onBeforeLoadSource = function () {
    sourceStore.baseParams = { objectkey: objKey };
};

// загружаем данные в грид только при первой загрузке списка источников
var tempOnLoad = function () {
    if (notLoaded) {
        sourceHandler();
        notLoaded = false;
    }
}