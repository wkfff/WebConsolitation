Workbench.extensions.HandBooks = {
    //функция обработки выбора строки(значения)
    onSelectRow: function () { Ext.MessageBox.alert('Ахтунг!', 'Функция не определена', 1); },
    //функция вызова справочников с произвольной обработкой выбора
    showHandBookWhd: function (bookObjectKey, title, fn, filter) {
        window.Workbench.extensions.HandBooks.onSelectRow = fn;
        var wnd = Ext.getCmp('HBWnd');
        if (!wnd)
            Ext.MessageBox.alert('Ошибка', 'Не найдено окно: HBWnd', 1);
        wnd.autoLoad.url = "/EntityExt/ShowBook?objectKey=" + bookObjectKey;
        if (!!filter) wnd.autoLoad.url += "&filter=" + filter;
        wnd.setTitle(title);
        wnd.show();
    },

    //функция выбора значения и закрытия окна с вызовом пользовательской обработки
    closeHandBookWhd: function closeHandBook() {
        Ext.getCmp('btHBSelectVal').setDisabled(true);
        var wnd = Ext.getCmp('HBWnd');
        if (!wnd)
            Ext.MessageBox.alert('Ерор!', 'Не найдено окно: HBWnd', 1);
        var selectedBookRecord = wnd.getBody().Extension.entityBook.selectedRecord;
        window.Workbench.extensions.HandBooks.onSelectRow(selectedBookRecord, wnd.getBody().Extension.entityBook.getLookupValue());
        wnd.hide();
    }
};