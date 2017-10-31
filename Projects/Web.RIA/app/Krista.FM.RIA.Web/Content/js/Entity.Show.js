Extension.EntityView = {
    activeRefField: {},
    activeLookupField: {}
};

var commandHandler = function (cmd, record, rowIndex, colIndex) {
    switch (cmd) {
        case "Go":
            parent.MdiTab.addTab({ title: record.data.Semantic + "." + record.data.Caption, url: "/Entity/Index?objectKye=" + record.data.ObjectKey, icon: "icon-report" });
            break;
        case "EditRefCell":
            var fieldName = this.getColumnModel().getDataIndex(colIndex);
            Extension.EntityView.activeRefField = fieldName.substring(3, fieldName.length);
            Extension.EntityView.activeLookupField = fieldName;
            BookWindow.autoLoad.params.id = record.id;
            BookWindow.autoLoad.url = "/Entity/Book?objectKey=" + MetaData[fieldName].objectKey;
            BookWindow.setTitle(MetaData[fieldName].caption);
            btnOk.disable();
            BookWindow.show();
            break;
    }
}

var getSelectedVariantId = function () {
    var grid = Ext.getCmp('GridPanel1');
    var record = grid.getSelectionModel().getSelections()[0];
    if (record) {
        return record.id;
    }
    return -1;
}

