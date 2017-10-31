/// <reference path="/Content/js/ext-base-debug.js" />
/// <reference path="/Content/js/ext-all-debug.js" />
/// <reference path="/Content/js/App.js" />
Ext.ns('App.Cons');

App.Cons.ReportRequisitesGrid = Ext.extend(Ext.net.GridPanel, {
    constructor: function (config) {
        Ext.apply(this, config);

        // Создаем поля и голонки
        var fields = [{ name: "ID" }, { name: "RefFormRow" }, { name: "MetaRowOrd"}];
        var columns = [];
        for (var i = 0; i < config.requsites.length; i++) {
            var c = config.requsites[i];
            fields.push({ name: c.InternalName.toLocaleUpperCase() });
            columns.push({
                id: c.InternalName,
                dataIndex: c.InternalName.toLocaleUpperCase(),
                header: c.Name,
                editor: this.createColumnField(c)
            });
        }

        // Store
        var actionName = config.type == 'section' ? 'Section' : '';
        actionName = actionName + (config.kind == 'header' ? 'HeaderRequisites' : 'FooterRequisites');
        this.store = new Ext.net.Store({
            proxyId: 'store' + config.formId,
            proxy: new Ext.data.HttpProxy({ url: "/ConsReport/Get" + actionName }),
            updateProxy: new Ext.net.HttpWriteProxy({ url: "/ConsReport/Save" + actionName }),
            reader: new Ext.data.JsonReader({
                fields: fields,
                idProperty: "ID",
                root: "data"
            }),
            sortInfo: { field: "MetaRowOrd", direction: "asc" },
            _serverParams: { params: { reportId: config.reportId, sectionCode: config.formSectionCode} }
        });

        // Грид
        App.Cons.ReportRequisitesGrid.superclass.constructor.call(this, {
            region: 'center',
            id: 'grid' + config.formId,
            loadMask: { msg: 'Загрузка данных...' },
            colModel: new Ext.grid.ColumnModel({ columns: columns }),
            sm: new Ext.ux.RequisitesSelectionModel(),
            view: new Ext.ux.ExcelRequisitesGridView({ layoutMarkup: config.markup }),
            border: false,
            hideHeaders: true,
            collapsible: true
        });
    },

    refresh: function () {
        this.store.reload(this.store._serverParams);
    },

    save: function () {
        this.store.save(this.store._serverParams);
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