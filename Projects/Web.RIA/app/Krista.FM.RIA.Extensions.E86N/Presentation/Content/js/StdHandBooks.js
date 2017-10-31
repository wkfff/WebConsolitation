HB = {
    // Возвращает объект рабочей области главного окна
    getWorkbench: function () {
        return top.Workbench;
    },
    // поле(колонка грида) куда нужно записать значения
    Field: {},
    // грид вызвавший справочник
    Grid: {}
};

//обработчик ввыбора строки для поля-триггера
function SelectRowFT(record, lookupVal) {
    var idFld = Ext.getCmp(HB.Field.substring(0, HB.Field.length - 4));
    var nameFld = Ext.getCmp(HB.Field);
    nameFld.setValue(lookupVal);
    idFld.setValue(record.data.ID);
};

//обработчик ввыбора строки для грида
function SelectRowGrid(record, lookupVal) {
    var grid = Ext.getCmp(HB.Grid);
    var fld = Ext.getCmp(HB.Field);
    var srec = grid.getSelectionModel().getSelected(); 
    if (srec != null)
     {
         srec.set(HB.Field.substring(0, HB.Field.length - 4), record.data.ID);
         srec.set(HB.Field, lookupVal);
         fld.setValue(lookupVal);
     }
};

//вызов справочника для тригер поля
function showHandBook(field, entity, filter) {
    var fld = Ext.getCmp(field);
    HB.Field = field;
    HB.getWorkbench().extensions.HandBooks.showHandBookWhd(entity, fld.fieldLabel, SelectRowFT, filter);
};

//вызов справочника для грида
function showHandBookGrid(grid, colId, entity, title, fn, filter) {
    HB.Field = colId;
    HB.Grid = grid;
    HB.getWorkbench().extensions.HandBooks.showHandBookWhd(entity, title, fn, filter);
};