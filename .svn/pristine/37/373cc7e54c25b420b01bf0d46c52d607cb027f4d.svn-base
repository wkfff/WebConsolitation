Extension.DebtBookReportParams = {
    activeRefField: {},
    activeLookupField: {},
    selectedBookRecord: {},
    onBookRowSelect: function (record) {
        btnOk.enable();
        Extension.DebtBookReportParams.selectedBookRecord = record;
    }
};

function triggerClick(field, bookObjectKey) {
    Extension.DebtBookReportParams.activeRefField = Ext.getCmp(field.id.substring(3, field.id.length));
    Extension.DebtBookReportParams.activeLookupField = field;

    BookWindow.autoLoad.params.filter = '';
    BookWindow.autoLoad.params.sourceId = '';
    var sourceId = -1;
    BookWindow.autoLoad.params.sortField = 'ID';
    if (bookObjectKey == '383f887a-3ebb-4dba-8abb-560b5777436f') {
        // Классификатор "Районы.Анализ"
        sourceId = Extension.View.getWorkbench().extensions.DebtBook.currentAnalysisSourceId
        // сортируем по наименованию территории
        BookWindow.autoLoad.params.sortField = 'Name';
    } else if (bookObjectKey == '4d192956-aced-4718-a87c-b2e5519c022a') {
        // Классификатор "Должности отчетов"
        BookWindow.autoLoad.params.filter = 'REFREGION = ' + Extension.View.getWorkbench().extensions.DebtBook.userRegionId;
    } else {
        //sourceId = Extension.View.getWorkbench().extensions.DebtBook.currentSourceId
    }

    BookWindow.autoLoad.params.id = Extension.DebtBookReportParams.activeRefField.getValue();
    if (sourceId != -1) {
        BookWindow.autoLoad.params.sourceId = sourceId;
    }
    BookWindow.autoLoad.url = "/Entity/Book?objectKey=" + bookObjectKey;
    BookWindow.setTitle(field.fieldLabel);
    btnOk.disable();
    BookWindow.show();
}

function bookClose(btn, e) {
    if (Extension.DebtBookReportParams.selectedBookRecord) {
        Extension.DebtBookReportParams.activeRefField.setValue(BookWindow.getBody().Extension.entityBook.selectedRecord.id);
        Extension.DebtBookReportParams.activeLookupField.setValue(BookWindow.getBody().Extension.entityBook.getLookupValue());
        BookWindow.hide();
    }
}

