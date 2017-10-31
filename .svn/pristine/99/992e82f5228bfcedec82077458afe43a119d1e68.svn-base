Ext.override(Ext.ux.grid.GridFilters, {
     applyFilter = false;

    onBeforeLoad: function (store, options) {
        options.params = options.params || {};
        this.cleanParams(options.params[this.paramPrefix] || {});
        var params = this.buildQuery(this.getFilterData());
        var filterParams = {};
        if (!Ext.isEmptyObj(params)) {
            filterParams[this.paramPrefix] = store.proxy.isMemoryProxy ? params : Ext.encode(params);
        }
        store.setActiveNode(store.active_node);
        if (store.active_node === undefined || store.active_node === null) {
            options.params["anode"] = null;
        }
        options.params[this.paramPrefix] = {};
        Ext.apply(options.params, filterParams);

        // если применяется новый фильтр отображаем с 1 страницы
        if (applyFilter) {
            this.cleanParams(options.params['start'] || {});
            Ext.apply(options.params, {start: 0});
        }

        applyFilter = false;
    }
});

Ext.override(Ext.ux.maximgb.tg.EditorGridPanel, {
    /**
    * @access private
    */
    onClick: function (e) {
        var target = e.getTarget(),
            view = this.getView(),
            row = view.findRowIndex(target),
            store = this.getStore(),
            sm = this.getSelectionModel(),
            record, record_id, do_default = true;
        var an = store.getAt(row)
        if (e.browserEvent.ctrlKey) {
            this.store.loadAll = true;
            e.target = null;
        }
        else {
            store.setActiveNode(an);
            this.active_node = an;
            // Row click
            if (row !== false) {
                if (Ext.fly(target).hasClass('ux-maximgb-tg-elbow-active')) {
                    record = store.getAt(row);
                    if (store.isExpandedNode(record)) {
                        store.collapseNode(record);
                        e.target = null;
                    }
                    else {
                        store.expandNode(record);
                    }
                    do_default = false;
                }
            }
        }
        if (do_default) {
            Ext.ux.maximgb.tg.EditorGridPanel.superclass.onClick.call(this, e);
        }
    }
});

Ext.override(Ext.ux.maximgb.tg.AbstractTreeStore, {

    loadAll: false,

    collapseNode: function (rc) {
        if (
            this.isExpandedNode(rc) &&
            this.fireEvent('beforecollapsenode', this, rc) !== false
        ) {
            this.setNodeExpanded(rc, false);
            this.fireEvent('collapsenode', this, rc);
        }
    },
    /**
    * Sets active node.
    * 
    * @access public
    * @param {Record} rc Record to set active. 
    */
    setActiveNode: function (rc) {
        this.loadAll = false;
        if (this.active_node !== rc) {
            if (rc) {
                if (this.data.indexOf(rc) != -1) {
                    if (this.fireEvent('beforeactivenodechange', this, this.active_node, rc) !== false) {
                        this.active_node = rc;
                        this.fireEvent('activenodechange', this, this.active_node, rc);
                    }
                }
                else {
                    throw "Нет такой записи."; // Given record is not from the store.
                }
            }
            else {
                if (this.fireEvent('beforeactivenodechange', this, this.active_node, rc) !== false) {
                    this.active_node = rc;
                    this.fireEvent('activenodechange', this, this.active_node, rc);
                }
            }
        }
    }
});

Ext.override(Ext.ux.maximgb.tg.AbstractTreeStore, {

});