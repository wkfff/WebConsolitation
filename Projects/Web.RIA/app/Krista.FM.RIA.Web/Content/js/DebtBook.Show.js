var forceHideDetailsWindow = false;

var bookClose = function (btn, e) {
    if (MetaData.SelectedId) {
        var grid = Ext.getCmp('GridPanel1');
        var sel = grid.selModel.getSelectedCell();
        var fieldName = grid.getColumnModel().getDataIndex(sel[1]);
        var r = grid.store.getAt(sel[0]);
        r.beginEdit();
        r.set(fieldName, MetaData.SelectedId);
        MetaData.SelectedId = undefined;
        r.endEdit();
        BookWindow.hide();
    }
}

var gridCommandHandler = function (cmd, record, rowIndex, colIndex) {
    switch (cmd) {
        case "edit":
            DetailsWindow.clearContent();

            //DetailsWindow.autoLoad.params.id = record.id;
            DetailsWindow.autoLoad.url =
                "/BebtBook/ShowDetails?objectKey=" + this.ModelObjectKey
                + "&recordId=" + record.id
                + "&viewModelKey=" + this.ViewModelKey
                + "&userRegionType=" + this.UserRegionType;
            DetailsWindow.setTitle(this.title);
            //btnOk.disable();
            DetailsWindow.show();
            break;

        case "EditRefCell":
            var md = MetaData[this.ModelKey];
            var fieldName = this.getColumnModel().getDataIndex(colIndex);
            BookWindow.autoLoad.params.id = record.id;
            BookWindow.autoLoad.url = "/Entity/Book?objectKey=" + md[fieldName].objectKey;
            BookWindow.setTitle(md[fieldName].caption);
            btnOk.disable();
            BookWindow.show();
            break;
    }
}
