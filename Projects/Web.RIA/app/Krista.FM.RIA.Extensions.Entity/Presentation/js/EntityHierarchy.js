Controller = function () {
    function createGrid() {
        // значение ID для новой записи (классификатора)
        ID = -2;
        // имя выбранной ячейки 
        curFieldName = '';
        // reader сущности
        entityReader = new Ext.data.JsonReader(
            { id: 'ID',
                root: 'data',
                totalProperty: 'total',
                successProperty: 'success',
                idProperty: 'ID'
            },
             record
        );
        store = new Ext.ux.maximgb.tg.AdjacencyListStore({
            id: 'store',
            proxyId: 'store',
            autoLoad: false,
            autosave: false,
            remoteSort: true,
            batch: false,
            totalProperty: 'total',
            parent_id_field_name: 'PARENTID',
            reader: entityReader,
            proxy: new Ext.data.HttpProxy({
                method: 'POST',
                url: '/Entity/DataH'
            })
        });

        store.on('beforeload', onbeforeLoad);
        var pagingToolBar = new Ext.ux.maximgb.tg.PagingToolbar({
            id: 'pagingToolBar',
            store: store,
            displayInfo: true,
            pageSize: 10,
            header: false,
            border: false,
            displayMsg: 'Всего записей: {2}. Отображаются с {0} по {1}.',
            emptyMsg: 'Нет данных'
        });

        entityFilters = new Ext.ux.grid.GridFilters({
            id: 'EntityFilters',
            local: false,
            filters: efilters
        });
        // если не редактируемый
        if (!IsEditable)
        // не отображаем кнопки
            buttons = [];
        else
        // иначе позволяем сохранять, отменять изменения, добавлять, удалять записи
            buttons = [
                { text: 'Сохранить',
                    iconCls: 'icon-disk',
                    listeners: {
                        click: {
                            fn: function (el, e) {
                                saveHandler();
                            }
                        }
                    }
                },
                { text: 'Добавить',
                    iconCls: 'icon-add',
                    listeners: {
                        click: {
                            fn: function (el, e) {
                                addHandler();
                            }
                        }
                    }
                },
                { text: 'Отменить изменения',
                    iconCls: 'icon-arrowundo',
                    listeners: {
                        click: {
                            fn: function (el, e) {
                                rejectHandler();
                            }
                        }
                    }
                },
                { text: 'Удалить',
                    iconCls: 'icon-delete',
                    handler: function () {
                        deleteHandler();
                    }
                }
            ];
        // если делится по источникам
        if (isDivided)
        // отображаем кнопки и выпадающий список источников
            gridExts = [
                    buttons,
                    comboSource,
                    sourceID
            ];
        else
        // отображаем только кнопки (если они доступны)
            gridExts = [buttons];
        selModel = null;
        if (!IsEditable)
            selModel = new Ext.grid.RowSelectionModel({
                id: 'sm',
                proxyId: '',
                listeners: {
                    beforerowselect: onGridRowSelect
                }
            });
        else
            selModel = new Ext.grid.RowSelectionModel({
                proxyId: ''
            });
        // Таблица отображения
        grid = new Ext.ux.maximgb.tg.EditorGridPanel({
            id: GridId,
            store: store,
            trackMouseOver: true,
            clicksToEdit: 2,
            memoryIDField: 'ID',
            master_column_id: masterColumnId,
            sm: selModel,
            columns: Columns,
            stripeRows: true,
            autoExpandColumn: 'NAME',
            autoExpandMin: 150,
            root_title: 'Все классификаторы',
            border: false,
            loadMask: { showMask: true },
            saveMask: { showMask: true },
            plugins: [entityFilters],
            bbar: pagingToolBar,
            layout: 'fit',
            tbar: new Ext.Toolbar(
            {
                items: gridExts
            })
        });

        // добавляем обработчик на изменение фильтра
        grid.on("filterupdate", filterUpdated)

        grid.store.writer = new Ext.data.JsonWriter({
            encode: true,
            writeAllFields: false
        });
        grid.store.writer.meta = grid.store.reader.meta;
        // если не делится по источникам, загружаем данные
        if (!isDivided) {
            grid.store.load();
        }
        var vp = new Ext.Viewport({
            layout: 'fit',
            border: false,
            renderTo: Ext.getBody(),
            items: grid
        });
    }

    filterUpdated = function (p, filter) {
        applyFilter = true;
    }

    /*пометить запись и ее потомков 'на удаление'*/
    var markDeleted = function (rc) {
        var gridShowHierarchy = Ext.getCmp(GridId);
        var i, len, children = gridShowHierarchy.store.getNodeChildren(rc);
        /*для всех потомков*/
        for (i = 0, len = children.length; i < len; i++) {
            // если это не новая запись (еще не сохранена в бд)
            if (!children[i].phantom) {
                // маркируем на удаление ее потомков
                markDeleted(children[i]);
            }
        }
        /*маркируем запись на удаление - добавляем в список removed*/
        removed.push(rc.data);
    }

    /* обработчик при удалении записи*/
    var deleteHandler = function () {
        var gridShowHierarchy = Ext.getCmp(GridId);
        // получаем выбранную запись record
        var record = gridShowHierarchy.getSelectionModel().getSelected();
        // если запись не новая, маркируем ее на удаление
        if (!record.phantom) {
            markDeleted(record);
        }
        // удаляем из store
        gridShowHierarchy.store.remove(record);
        // обновляем интерфейс
        gridShowHierarchy.getView().refresh();
    }


    /* обработчик при отмене изменений*/
    var rejectHandler = function () {
        var gridShowHierarchy = Ext.getCmp(GridId);
        /* отменяем добавление новых записей */
        // список записей, которые надо удалить
        toRemove = [];
        // пробегаем по всем записям в store
        Ext.each(gridShowHierarchy.store.data.items, function (record) {
            // если запись новая
            if (record.data.phantom) {
                // добавляем в список на удаление
                toRemove.push(record);
            }
        });
        // удаляем новые записи (не сохраненные в бд)
        Ext.each(toRemove, function (record) {
            gridShowHierarchy.store.remove(record);
        });

        /*отменяем удаление записей*/
        //восстанавливаем узлы с удаленными записями
        i = 0;
        cnt = removed.length - 1
        for (i = cnt; i >= 0; i--) {
            rcD = removed[i];
            if (!rcD.phantom) {
                row = gridShowHierarchy.store.indexOfId(rcD['PARENTID']);
                if (row >= 0) {
                    r = gridShowHierarchy.store.getAt(row);
                    /*формируем запись*/
                    var myNewRecord = new gridShowHierarchy.store.recordType(rcD, rcD['ID']);
                    /*добавляем запись к узлу r*/
                    gridShowHierarchy.store.addToNode(r, myNewRecord);
                    /*расхлапываем выбранный узел*/
                    gridShowHierarchy.store.expandNode(r);
                }
            }
        };
        /*отменяем обновление записей*/
        gridShowHierarchy.store.rejectChanges();
        /* сохраняем изменения*/
        gridShowHierarchy.store.commitChanges();
        /*обновляем интерфейс*/
        gridShowHierarchy.getView().refresh();
    }

    return {
        init: function () {
            Ext.BLANK_IMAGE_URL = '';
            createGrid();
        }
    }

} ();
