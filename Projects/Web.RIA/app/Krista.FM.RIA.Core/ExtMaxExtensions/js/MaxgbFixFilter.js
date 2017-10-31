applyFilter = false;    

Ext.override(Ext.ux.grid.GridFilters, {
    onBeforeLoad: function (store, options) {
        options.params = options.params || {};
        this.cleanParams(options.params[this.paramPrefix] || {});
        var params = this.buildQuery(this.getFilterData());
        var filterParams = {};
        if (!Ext.isEmptyObj(params)) {
            filterParams[this.paramPrefix] = store.proxy.isMemoryProxy ? params : Ext.encode(params);
        }
        options.params[this.paramPrefix] = {};
        Ext.apply(options.params, filterParams);

        /*        store.setActiveNode(store.active_node);*/
        if (store.active_node === undefined || store.active_node === null) {
            options.params["anode"] = null;
        }

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
    listeners: {
        filterupdate: {
            fn: function (filtersPlugin, filter) { applyFilter = true }
        }
    },

    /**
    * @access private
    */
    onClick: function (e) {
        var clearSelection = false;
        var target = e.getTarget(),
            view = this.getView(),
            row = view.findRowIndex(target),
            store = this.getStore(),
            sm = this.getSelectionModel(),
            record, record_id, do_default = true;
        var an = store.getAt(row)
        if (e.browserEvent.ctrlKey) {
            store.setActiveNode(null);
            this.active_node = null;
            this.store.loadAll = true;
            loadAll = true;
            e.target = null;
            clearSelection = !(this.getSelectionModel() instanceof Ext.grid.RowSelectionModel);
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
        if (clearSelection) {
            this.getSelectionModel().clearSelections();
        }
    },

    tempId: -2,
    removed: [],
    /* обработчик при добавлении записи */
    /**
    * @access public
    */
    addHandler: function () {
        /*получаем выбранную запись r*/
        if (this.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {
            r = this.getSelectionModel().getSelected();
        }
        else {
            if (this.getSelectionModel().selection == null)
                r = null;
            else
                r = this.getSelectionModel().selection.record;
        }
        /*получаем список полей f*/
        var f = this.store.recordType.prototype.fields,
            dv = {};
        /*инициализируем поля значениями по умолчанию*/
        for (var i = 0; i < f.length; i++) {
            if (f.items[i].defaultValue == null)
                dv[f.items[i].name] = "";
            else
                dv[f.items[i].name] = f.items[i].defaultValue;
        }

        dv['SOURCEID'] = activeSource;
        /*присваиваем временный идентификатор*/
        dv['ID'] = this.tempId;
        /*отмечаем, что запись новая*/
        dv.phantom = true;
        /*формируем новую запись*/
        var myNewRecord = new this.store.recordType(dv, this.tempId);
        this.tempId--;
        /*добавляем новую запись либо в корень*/
        if (r == null) {
            myNewRecord.set(this.parent_id_field_name, null);
            this.store.add(myNewRecord);
        }
        /*добавляем новую запись либо к выбранному узлу*/
        else
            this.store.addToNode(r, myNewRecord);
        /*расхлапываем выбранный узел*/
        this.store.expandNode(r);
        /*обновляем интерфейс*/
        this.getView().refresh();

    },

    /*пометить запись и ее потомков 'на удаление'*/
    markDeleted: function (rc) {
        var i, len, children = this.store.getNodeChildren(rc);
        /*для всех потомков*/
        for (i = 0, len = children.length; i < len; i++) {
            // если это не новая запись (еще не сохранена в бд)
            if (!children[i].phantom) {
                // маркируем на удаление ее потомков
                this.markDeleted(children[i]);
            }
        }
        /*маркируем запись на удаление - добавляем в список removed*/
        this.removed.push(rc.data);
    },

    /* обработчик при удалении записи*/
    deleteHandler: function () {
        // получаем выбранную запись record
        var record;
        if (this.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {
            record = this.getSelectionModel().getSelected();
        }
        else {
            record = this.getSelectionModel().selection.record;
        }
        // если запись не новая, маркируем ее на удаление
        if (!record.phantom) {
            this.markDeleted(record);
        }
        // удаляем из store
        this.store.remove(record);
        // обновляем интерфейс
        this.getView().refresh();
    },

    /* обработчик при отмене изменений*/
    rejectHandler: function () {
        var grid = this;
        /* отменяем добавление новых записей */
        // список записей, которые надо удалить
        grid.toRemove = [];
        // пробегаем по всем записям в store
        Ext.each(this.store.data.items, function (record) {
            // если запись новая
            if (record.data.phantom) {
                // добавляем в список на удаление
                grid.toRemove.push(record);
            }
        });
        // удаляем новые записи (не сохраненные в бд)
        Ext.each(grid.toRemove, function (record) {
            this.store.remove(record);
        });

        /*отменяем удаление записей*/
        //восстанавливаем узлы с удаленными записями
        i = 0;
        cnt = grid.removed.length - 1
        for (i = cnt; i >= 0; i--) {
            rcD = grid.removed[i];
            if (!rcD.phantom) {
                row = this.store.indexOfId(rcD['PARENTID']);
                /*формируем запись*/
                var myNewRecord = new this.store.recordType(rcD, rcD['ID']);
                if (row >= 0) {
                    r = this.store.getAt(row);
                    /*добавляем запись к узлу r*/
                    this.store.addToNode(r, myNewRecord);
                    /*расхлапываем выбранный узел*/
                    this.store.expandNode(r);
                }
                else {
                    this.store.add(myNewRecord);
                }
            }
        };
        grid.removed = [];
        /*отменяем обновление записей*/
        this.store.rejectChanges();
        this.store.reload();
        /* сохраняем изменения*/
        //this.store.commitChanges();
        /*обновляем интерфейс*/
        this.getView().refresh();
    },

    // сохранение прошло успешно
    ajaxSuccess: function (response) {
        var xml = response.responseXML;
        if (xml)
            var errors = xml.getElementsByTagName('error');
        if (!errors || !errors.length)
            this.store.commitChanges();
        else {
            var l = errors.length;
            var msg = '';
            for (var f = 0; f < l; f++)
                msg += errors[f].firstChild.nodeValue + '\n';
            Ext.MessageBox.alert('Ошибка', msg, 1);
        }
    },

    /*Вызывается, когда записи были корректно удалены*/
    deleteSuccess: function (response) {
        ajaxSuccess(response);
        // опустошаем список удаленных записей
        removed = [];
    },

    /*Вызывается, когда записи были корректно добавлены*/
    addedSuccess: function (response) {
        /*Снимаем отметки 'новая запись'*/
        Ext.each(this.store.data.items, function (record) {
            if (record.data.phantom) {
                record.data.phantom = false;
            }
        });
        ajaxSuccess(response);
    },

    /*обработчик при сохранении изменений*/
    saveHandler: function (objKey) {
        /* обновлять ли данные */
        var needReload = false;
        /*список измененных записей*/
        var data = [];
        var updatedSize = 0;
        /*список добавленных записей*/
        var addedData = [];
        var addedSize = 0;
        var grid = this;
        /*формируем список измененных записей*/
        Ext.each(this.store.getModifiedRecords(), function (record) {
            if (!record.data.phantom) {
                updatedSize++;
                data.push(record.data);
            }
        });
        errorFields = "";
        errorFieldsCnt = 0;
        /*формируем список добавленных записей*/
        Ext.each(this.store.data.items, function (record) {
            if (record.data.phantom) {
                recData = record.data;
                Ext.each(grid.colModel.columns, function (column) {
                    if ((column.allowBlank == false) && ((recData[column.dataIndex] == null) || (recData[column.dataIndex] === ""))) {
                        errorFields += '"' + column.header + '", ';
                        errorFieldsCnt++;
                    }
                });
                addedData.push(record.data);
                addedSize++;
            }
        });

        if (errorFieldsCnt > 1) {
            Ext.MessageBox.alert('Предупреждение', "Поля " + errorFields.substring(0, errorFields.length - 2) + " должны иметь значение", 1);
        }
        else if (errorFieldsCnt == 1) {
            Ext.MessageBox.alert('Предупреждение', "Поле " + errorFields.substring(0, errorFields.length - 2) + " должно иметь значение", 1);
        }
        if (errorFieldsCnt < 1) {
            needReload = true;
            /*если есть добавленные записи, отправлем запрос на сервер*/
            if (addedSize)
                Ext.Ajax.request(
                            {
                                url: '/Entity/Save?objectKey=' + objKey,
                                params: {
                                    data: '{"Created":' + Ext.encode(addedData) + ', "hierarchy": true}'
                                },
                                success: function (response, options) {
                                    var errors = response.responseText;
                                    if (!errors || !errors.length) {
                                        grid.store.commitChanges();
                                        addedData = [];
                                        addedSize = 0;
                                        /*Снимаем отметки 'новая запись'*/
                                        Ext.each(grid.store.data.items, function (record) {
                                            if (record.data.phantom) {
                                                record.data.phantom = false;
                                            }
                                        });
                                    }
                                    else {
                                        var pos = errors.indexOf('success:true');
                                        if (-1 !== pos) {
                                            grid.store.commitChanges();
                                            addedData = [];
                                            addedSize = 0;
                                            /*Снимаем отметки 'новая запись'*/
                                            Ext.each(grid.store.data.items, function (record) {
                                                if (record.data.phantom) {
                                                    record.data.phantom = false;
                                                }
                                            });
                                        }
                                        else {
                                            Ext.MessageBox.alert('Cообщение', "Сохранение новых записей не выполнено", 1);
                                        }
                                    }
                                },
                                failure: function (response, options) {
                                    alert('Message' + response.statusText);
                                }
                            });
            /*если есть измененные записи, отправлем запрос на сервер*/
            if (updatedSize)
                Ext.Ajax.request({
                    url: '/Entity/Save?objectKey=' + objKey,
                    params: {
                        data: '{"Updated":' + Ext.encode(data) + '}'
                    },
                    success: function (response, options) {
                        var errors = response.responseText;
                        if (!errors || !errors.length) {
                            grid.store.commitChanges();
                        }
                        else {
                            var pos = errors.indexOf('success:true');
                            if (-1 !== pos) {
                                data = [];
                                updatedSize = 0;
                                grid.store.commitChanges();
                            }
                        }
                    },
                    failure: function (response, options) {
                        Ext.MessageBox.alert('Message', response.statusText);
                    }
                });
            /*если есть удаленные записи, отправлем запрос на сервер*/
            if (this.removed.length > 0)
                Ext.Ajax.request({
                    url: '/Entity/Save?objectKey=' + objKey,
                    params: {
                        data: '{"Deleted":' + Ext.encode(this.removed) + '}'
                    },
                    success: function (response, options) {
                        var errors = response.responseText;
                        if (!errors || !errors.length) {
                            grid.store.commitChanges();
                            grid.removed = [];
                        }
                        else {
                            var pos = errors.indexOf('success:true');
                            if (-1 !== pos) {
                                grid.store.commitChanges();
                                grid.removed = [];
                            }
                            else {
                                /*отменяем удаление записей*/
                                //восстанавливаем узлы с удаленными записями
                                i = 0;
                                cnt = grid.removed.length - 1
                                for (i = cnt; i >= 0; i--) {
                                    rcD = grid.removed[i];
                                    if (!rcD.phantom) {
                                        row = grid.store.indexOfId(rcD['PARENTID']);
                                        /*формируем запись*/
                                        var myNewRecord = new grid.store.recordType(rcD, rcD['ID']);
                                        if (row >= 0) {
                                            r = grid.store.getAt(row);
                                            /*добавляем запись к узлу r*/
                                            grid.store.addToNode(r, myNewRecord);
                                            /*расхлапываем выбранный узел*/
                                            grid.store.expandNode(r);
                                        }
                                        else {
                                            grid.store.add(myNewRecord);
                                        }
                                    }
                                };
                                grid.removed = [];
                                Ext.MessageBox.alert('Cообщение', "Удаление записей не выполнено", 1);
                            }
                        }
                    },
                    failure: function (response, options) {
                        Ext.MessageBox.alert('Message', response.statusText);
                    }
                });
        }
        if (needReload) {
            grid.store.setActiveNode(null);
            grid.active_node = null;
            grid.store.loadAll = true;
            grid.store.reload();
        }

        /*обновляем интерфейс*/
        this.getView().refresh();
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
    * Loads current active record data.
    */
    load: function (options) {
        if (options) {
            if (options.params) {
                options.params[this.paramNames.active_node] = this.active_node ? this.active_node.id : null;
            }
            else {
                options.params = {};
                options.params[this.paramNames.active_node] = this.active_node ? this.active_node.id : null;
            }
        }
        else {
            options = { params: {} };
            options.params[this.paramNames.active_node] = this.active_node ? this.active_node.id : null;
        }

        if (options.params[this.paramNames.active_node] !== null || this.loadAll) {
            options.add = true;
        }

        return Ext.ux.maximgb.tg.AbstractTreeStore.superclass.load.call(this, options);
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

Ext.override(Ext.ux.maximgb.tg.PagingToolbar, {
    updateInfo: function () {
        var count = 0, cursor = 0, total = 0, msg;
        if (this.displayItem) {
            if (this.store) {
                cursor = this.store.getActiveNodePageOffset();
                count = this.store.getActiveNodeCount();
                total = this.store.getActiveNodeTotalCount();
            }
            text = "";
            msg = count == 0 ?
               text + this.emptyMsg
                    :
                String.format(
                    text + this.displayMsg,
                    cursor + 1, cursor + count, total
                );
            this.displayItem.setText(msg);
        }
    }
});

Ext.override(Ext.ux.maximgb.tg.AdjacencyListStore, {
    addToNode: function (parent, child) {
        if ((parent === undefined) || (parent === null))
            child.set(this.parent_id_field_name, null);
        else
            child.set(this.parent_id_field_name, parent.id);
        this.addSorted(child);
    }
});