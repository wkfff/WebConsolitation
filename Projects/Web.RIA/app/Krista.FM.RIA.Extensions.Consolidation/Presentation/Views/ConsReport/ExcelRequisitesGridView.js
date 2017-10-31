Ext.ux.ExcelRequisitesGridView = Ext.extend(Ext.ux.ExcelGridView, {

    constructor: function () {
        Ext.ux.ExcelRequisitesGridView.superclass.constructor.apply(this, arguments);
    },

    /** @override
    * @private Возвращает массив строк относящихся только к таблице данных, 
    * не влючая заголовок таблицы и ее нижнюю часть.
    * @return {Array}
    */
    getRows: function () {
        if (this.hasRows()) {
            var childs = this.mainBody.dom.childNodes,
                rows = [];

            for (var i = 0; i < childs.length; i++) {
                if (i >= this.layoutMarkup.FirstRow && i <= this.layoutMarkup.LastRow) {
                    rows.push(childs[i]);
                }
            }

            return rows;
        }

        return [];
    },

    /**
    * Возвращает имя реквизита находяшегося в указанной ячейке.
    * @param {Number} row The row index in which to find the cell.
    * @param {Number} col The column index of the cell.
    * @return {String} Имя реквизита или null, если ячейка не является реквизитом.
    */
    getCellName: function (row, col) {
        var cell = Ext.fly(this.getRow(row)).query(this.cellSelector)[col];
        var match = cell.className.match(this.colRe);
        if (match && match[1]) {
            return match[1];
        }

        return null;
    },

    /**
    * Returns the grid's <tt>&lt;td></tt> HtmlElement at the specified coordinates.
    * @param {Number} row The row index in which to find the cell.
    * @param {Number} col The column index of the cell.
    * @return {HtmlElement} The td at the specified coordinates.
    */
    getCell: function (row, col) {
        return Ext.fly(this.getRow(row)).query(this.cellSelector)[col];
    },

    /**
    * @private
    * Заполняет ячейки реквизитов значениями.
    * @param {Array} columns The column data acquired from getColumnData.
    * @param {Array} records The array of records to render
    * @return {String} Возвращает пустую строку.
    */
    doRender: function (columns, records) {
        var len = columns.length,
            record, j;

        if (records.length < 1) {
            return '';
        }

        record = records[0];

        // устанавливаем значение для каждого реквизита
        for (j = 0; j < len; j++) {
            var column = columns[j];
            var value = record.get(column.id.toUpperCase());

            var el = this.mainBody.query('td.x-grid3-td-' + column.id + ' > div');
            if (el) {
                el[0].textContent = value;
            }
        }

        return '';
    },

    /**
    * @private
    * Обновляет данные в строке.
    */
    refreshRowData: function (record) {
        this.doRender(this.getColumnData(), [record]);
        return -1;
    },

    processRows: function () {
        if (!this.ds || this.ds.getCount() < 1) {
            return;
        }

        var rows = this.getUserRows(),
            length = rows.length,
            row, i;

        for (i = 0; i < length; i++) {
            row = rows[i];
            if (row) {
                row.childNodes[0].childNodes[0].className = 'ux-excelgrid-rowaction-new';
                row.childNodes[row.childNodes.length - 1].childNodes[0].className = 'ux-excelgrid-rowaction-delete';
            }
        }
    },

    /**
    * @private
    */
    afterRender: function () {
        if (!this.ds || !this.cm) {
            return;
        }

        this.processRows(0, true);

        this.grid.fireEvent('viewready', this.grid);
    },

    /**
    * Refreshs the grid UI
    */
    refresh: function () {
        this.fireEvent('beforerefresh', this);
        this.grid.stopEditing(true);

        this.renderBody();
        this.processRows(0, true);
        this.layout();
        this.fireEvent('refresh', this);
    },

    processEvent: function (name, e) {
        Ext.ux.ExcelRequisitesGridView.superclass.processEvent.apply(this, arguments);

        var target = e.getTarget(),
            grid = this.grid;

        if (name == 'mousedown' || name == 'dblclick') {
            var match = target.parentNode.className.match(this.colRe);
            if (match && match[1]) {
                var row = target.parentNode.parentNode.rowIndex;
                var cell = target.parentNode.cellIndex;
                grid.fireEvent('cell' + name, grid, row, cell, e);
            }
        }
    },

    // private
    initUI: function (grid) {
        // переопредеяем методы Ext.grid.EditorGridPanel
        grid.startEditing = this.startEditing;
        grid.onEditComplete = this.onEditComplete;
        //grid.walkCells = this.walkCells;
    },

    /*onUpdate : function (store, record) {
    this.fireEvent("beforerowupdate", this, store.indexOf(record), record);
    this.refreshRow(record);
    },*/

    /* ------ Переопределение методов Ext.grid.EditorGridPanel ------ */

    walkCells: function () {
        return null;
    },

    onEditComplete: function (ed, value, startValue) {
        this.editing = false;
        this.lastActiveEditor = this.activeEditor;
        this.activeEditor = null;

        var r = ed.record,
            reqName = this.view.getCellName(ed.row, ed.col),
            reqCol = this.colModel.getIndexById(reqName),
            field = this.colModel.getDataIndex(reqCol).toUpperCase();
        value = this.postEditValue(value, startValue, r, field);
        if (this.forceValidation === true || String(value) !== String(startValue)) {
            var e = {
                grid: this,
                record: r,
                field: field,
                originalValue: startValue,
                value: value,
                row: ed.row,
                column: ed.col,
                cancel: false
            };
            if (this.fireEvent("validateedit", e) !== false && !e.cancel && String(value) !== String(startValue)) {
                r.set(field, e.value);
                delete e.cancel;
                this.fireEvent("afteredit", e);
            }
        }
        this.view.focusCell(ed.row, ed.col);
    },

    startEditing: function (row, col) {
        this.stopEditing();

        var reqName = this.view.getCellName(row, col);
        if (!reqName) {
            return;
        }

        // !!!if(this.colModel.isCellEditable(col, row)){
        this.view.ensureVisible(row, col, true);
        var r = this.store.getAt(0),
                reqCol = this.colModel.getIndexById(reqName);
        var e;
        field = this.colModel.getDataIndex(reqCol).toUpperCase(),
                e = {
                    grid: this,
                    record: r,
                    field: field,
                    value: r.data[field],
                    row: row,
                    column: col,
                    cancel: false
                };
        if (/*this.fireEvent("beforeedit", e) !== false &&*/!e.cancel) {
            this.editing = true;
            var ed = this.colModel.getCellEditor(reqCol, 0);
            if (!ed) {
                return;
            }
            if (!ed.rendered) {
                ed.parentEl = this.view.getEditorParent(ed);
                ed.on({
                    scope: this,
                    render: {
                        fn: function (c) {
                            c.field.focus(false, true);
                        },
                        single: true,
                        scope: this
                    },
                    specialkey: function (field, event) {
                        this.getSelectionModel().onEditorKey(field, event);
                    },
                    complete: this.onEditComplete,
                    canceledit: this.stopEditing.createDelegate(this, [true])
                });
            }
            Ext.apply(ed, {
                row: row,
                col: col,
                record: r
            });
            this.lastEdit = {
                row: row,
                col: col
            };
            this.activeEditor = ed;
            // Set the selectSameEditor flag if we are reusing the same editor again and
            // need to prevent the editor from firing onBlur on itself.
            ed.selectSameEditor = (this.activeEditor == this.lastActiveEditor);
            var v = this.preEditValue(r, field);
            ed.startEdit(this.view.getCell(row, col).firstChild, Ext.isDefined(v) ? v : '');

            // Clear the selectSameEditor flag
            (function () {
                delete ed.selectSameEditor;
            }).defer(50);
        }
        // !!!}
    }
});

Ext.reg('Ext.ux.ExcelRequisitesGridView', Ext.ux.ExcelRequisitesGridView);
