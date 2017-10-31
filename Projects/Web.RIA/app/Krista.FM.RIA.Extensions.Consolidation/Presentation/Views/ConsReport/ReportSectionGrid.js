/// <reference path="/Content/js/ext-base-debug.js" />
/// <reference path="/Content/js/ext-all-debug.js" />
/// <reference path="/Content/js/App.js" />
Ext.ns('App.Cons');

App.Cons.ReportSectionGrid = function (viewer, config) {
    var self = this;
    Ext.apply(this, config);

    // Добавляет множимую строку
    function onNewRow(sourceRecord, insertTo) {
        var record = sourceRecord.copy();
        record.newRecord = true;
        record.phantom = true;
        record.id = Ext.data.Record.id(record);
        record.set('ID', -1);
        self.store.insert(insertTo, record);
                    
        var h = self.getView().mainBody.getHeight();
        self.setHeight(h + 55);
    };

    // Удаляет множимую строку
    function onDeleteRow(record, index)
    {    
        var metaId = record.get('RefFormRow');
        var multipliesRows = self.store.queryBy(function(r){ return r.get('RefFormRow') == metaId; });
        if (multipliesRows.getCount() > 1) {
            self.store.removeAt(index);

            var h = self.getView().mainBody.getHeight();
            self.setHeight(h + 55);
        }
    };

    // Формируем список граф
    var fields = [{ name: "ID" }, { name: "RefFormRow" }, { name: "MetaRowOrd"}];
    var columns = [];
    Ext.each(config.mapping.Columns, function (mc) {
        var c;
        Ext.each(config.formSection.Columns, function (e) {
            if (mc.Code == e.InternalName) {
                c = e;
                return false;
            }
            return true;
        });

        fields.push({ name: c.InternalName });
        columns.push({
            id: c.InternalName,
            dataIndex: c.InternalName,
            header: c.Name,
            editable: true
        });
    });

    // Настройка источника данных
    this.store = new Ext.net.Store({
        proxyId: 'store' + config.formSection.InternalName,
        proxy: new Ext.data.HttpProxy({ method: "GET", url: "/ConsReport/GetSectionData" }),
        updateProxy: new Ext.net.HttpWriteProxy({ url: "/ConsReport/SaveSectionData" }),
        reader: new Ext.data.JsonReader({
            fields: fields,
            idProperty: "ID",
            root: "data"
        }),
        sortInfo: { field: "MetaRowOrd", direction: "asc" },
        _serverParams: { params: { reportId: config.reportId, sectionCode: config.formSection.Code} }
    });

    // Настройка грида
    App.Cons.ReportSectionGrid.superclass.constructor.call(this, {
        region: 'center',
        id: 'grid' + config.formId,
        loadMask: { msg: 'Загрузка данных...' },
        colModel: new Ext.grid.ColumnModel({
            columns: columns,
            getCellEditor: function (colIndex, rowIndex) {
                return self.getCellEditor(colIndex, rowIndex);
            }
        }),
        sm: new Ext.grid.CellSelectionModel({ singleSelect: true }),
        view: new Ext.ux.ExcelGridView({ layoutMarkup: config.markup }),
        border: false,
        hideHeaders: true,
        collapsible: true
    });

    // Настраиваем обработчики добавления/удаления множимых строк
    self.getView().on('newrow', onNewRow);
    self.getView().on('deleterow', onDeleteRow);
};

Ext.extend(App.Cons.ReportSectionGrid, Ext.net.GridPanel, {
    // public
    refresh: function () {
        this.store.reload(this.store._serverParams);
    },

    save: function () {
        this.store.save(this.store._serverParams);
    },

    getCellConfig: function (metaRowId, colName) {
        var cellConfig;

        // Находим графу
        var columnId;
        Ext.each(this.formSection.Columns, function (c) {
            if (c.InternalName == colName) {
                columnId = c.ID;
                cellConfig = c;
                return false;
            }
            return true;
        });

        // Находим строку
        var row;
        Ext.each(this.formSection.Rows, function (r) {
            if (r.ID == metaRowId) {
                row = r;
                return false;
            }
            return true;
        });

        if (row) {
            Ext.apply(cellConfig, {
                Multiplicity: row.Multiplicity,
                ReadOnly: row.ReadOnly
            });
        }

        // Находим ячейку
        var cell;
        Ext.each(this.formSection.Cells, function (cl) {
            if (cl.RefColumn == columnId && cl.RefRow == metaRowId) {
                cell = cl;
                return false;
            }
            return true;
        });

        if (cell) {
            Ext.apply(cellConfig, {
                Required: cell.Required,
                ReadOnly: cell.ReadOnly,
                DefaultValue: cell.DefaultValue
            });
        }

        return cellConfig;
    },

    getCellEditor: function (colIndex, rowIndex) {
        var v = this.getView();
        var rowEl = v.getRows()[rowIndex];
        var cellEl = v.getCell(rowIndex, colIndex);
        var metaRowId = v.getRowMetaId(rowEl);
        var colName = this.getColumnModel().getColumnAt(colIndex).id;

        var cellConfig = this.getCellConfig(metaRowId, colName);
        if (cellConfig.ReadOnly) {
            return null;
        }

        var ed = this.createColumnField(cellConfig);
        return new Ext.Editor(
            Ext.apply(ed, {
                height: cellEl.clientHeight,
                width: cellEl.clientWidth + 2
            })
        );
    },

    // private
    createColumnField: function (config) {
        if (config.DataType == 'System.String') {
            return new Ext.form.TextField({
                allowBlank: !config.Required
            });
        }

        if (config.DataType == 'System.Int32') {
            return new Ext.form.NumberField({
                allowBlank: !config.Required
            });
        }

        if (config.DataType == 'System.Decimal') {
            return new Ext.form.NumberField({
                allowBlank: !config.Required,
                decimalPrecision: config.DataTypeScale | 2,
                decimalSeparator: ','
            });
        }

        return null;
    }
});