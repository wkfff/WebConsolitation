Ext.override(Ext.ux.maximgb.tg.AdjacencyListStore, {
    createNewRow: function() {
        var newRow = new this.recordType();
        newRow.set(this.leaf_field_name, true);
        this.setNodeLoaded(newRow, true);
        return newRow;
    },

    addToNode: function (parent, child) {
        if (child) {
            child.set(this.leaf_field_name, true);
            if ((parent === undefined) || (parent === null)) {
                child.set(this.parent_id_field_name, null);
                this.addSorted(child);
            } else {
                child.set(this.parent_id_field_name, parent.id);
                if (this.isLeafNode(parent)){
                    parent.set(this.leaf_field_name, false);    
                }
                this.addSorted(child);
                this.expandNode(parent);
            }
        }
    },
    
    isDirty: function () {
        return this.removed.length > 0 || this.getModifiedRecords().length > 0;
    }
});

Ext.override(Ext.ux.maximgb.tg.EditorGridPanel, {
    getSelectedRecord : function() {
      var r;
      if (this.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {
          r = this.getSelectionModel().getSelected();
      }
      else if (this.getSelectionModel().selection == null) {
          r = null;
      } else {
          r = this.getSelectionModel().selection.record;
      }
      return r;
    }      
});

Ext.override(Ext.ux.maximgb.tg.EditorGridPanel, {
    /**
    * @access private
    */
    onClick: function (e) {
        var clearSelection = false;
        var target = e.getTarget();
        var view = this.getView();
        var row = view.findRowIndex(target);
        var store = this.getStore();
        var record;
        var do_default = true;
        var an = store.getAt(row);
        if (e.browserEvent.ctrlKey) {
            store.setActiveNode(null);
            this.active_node = null;
            this.store.loadAll = true;
            this.loadAll = true;
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
    addHandler: function (parent, model) {
        // получаем список полей f
        var fields = this.store.recordType.prototype.fields,
            defValues = {};
        // инициализируем поля значениями по умолчанию
        for (var i = 0; i < fields.length; i++) {
            if (model[fields.items[i].name]) {
                defValues[fields.items[i].name] = model[fields.items[i].name];
            } else if (fields.items[i].defaultValue)
                defValues[fields.items[i].name] = fields.items[i].defaultValue;
            else
                defValues[fields.items[i].name] = "";
                
        }

        // присваиваем временный идентификатор
        defValues['ID'] = this.tempId;
        // отмечаем, что запись новая
        defValues.phantom = true;
        // формирование новой записи
        var record = new this.store.recordType(defValues, this.tempId);
        record.set(this.store.leaf_field_name, true);
        this.store.setNodeLoaded(record, true);
        this.tempId--;
        // добавление записи в дерево
        this.store.addToNode(parent, record);
        // инициализация флага раскрытия узла дерева
        this.store.setNodeExpanded(record, true);
        // обновление интерфейса
        this.getView().refresh();
        // прокурутка к строке
        this.getView().ensureVisible(this.store.indexOfId(record.id), 0, false);
        // на всякий случай возвращаем добавленную запись
        return record;
    },

    /*пометить запись и ее потомков 'на удаление'*/
    markDeleted: function (rc) {
        var i, len, children = this.store.getNodeChildren(rc);
        // для всех потомков
        for (i = 0, len = children.length; i < len; i++) {
            // если это не новая запись (еще не сохранена в бд)
            if (!children[i].phantom) {
                // маркируем на удаление ее потомков
                this.markDeleted(children[i]);
            }
        }
        // маркируем запись на удаление - добавляем в список removed
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
        // обновление флагов
        var parentId = record.get(this.store.parent_id_field_name);
        if (parentId) {
            var parent = this.store.getById(parentId);
            if (this.store.getNodeChildren(parent).length == 0) {
                parent.set(this.store.leaf_field_name, true);
            }
        }
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
        var cnt = grid.removed.length - 1;
        for (var i = cnt; i >= 0; i--) {
            var rcD = grid.removed[i];
            if (!rcD.phantom) {
                var row = this.store.indexOfId(rcD['PARENTID']);
                /*формируем запись*/
                var myNewRecord = new this.store.recordType(rcD, rcD['ID']);
                if (row >= 0) {
                    var r = this.store.getAt(row);
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
        var errors = '';
        if (xml)
            errors = xml.getElementsByTagName('error');
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
    saveHandler: function () {
        // ссылки для использования в функциях
        var refGrid = this;
        var refStore = this.store;
        
        // флаг наличия ошибок
        var isValid = true;

        // проверка записей перед сохранением
        Ext.each(refStore.data.items, function (record) {
            var message = 'Не заполнены значения столбцов:';
            if (!record.isValid()) {
                Ext.each(refGrid.colModel.columns, function (column) {
                    if (!Ext.isEmpty(column.dataIndex)) {
                        var dataIndex = column.dataIndex;
                        // поиск полей не допускающих пустые значения
                        if (!record.fields.item(dataIndex).allowBlank && Ext.isEmpty(record.data[dataIndex])) {
                            message += '<br> - ' + column.header;
                        }
                    }
                });
                refGrid.getSelectionModel().selectRow(refStore.indexOfId(record.id));
                Ext.MessageBox.alert('Ошибка', message);
                // сброс флага для выхода из процедуры сохранения
                isValid = false;
                return false;
            } else {
                return true;
            }
        });

        if (!isValid) {
            return false;
        }

        // список измененных записей
        var updatedRecords = [];
        var updatedRecordsCount = 0;


        // формируем список измененных записей
        Ext.each(refStore.getModifiedRecords(), function (record) {
            if (!record.data.phantom) {
                updatedRecordsCount++;
                updatedRecords.push(record.data);
            }
        });
        
        // список добавленных записей
        var createdRecords = [];
        var createdRecordsCount = 0;

        // формируем список добавленных записей
        Ext.each(refStore.data.items, function (record) {
            if (record.data.phantom) {
                createdRecords.push(record.data);
                createdRecordsCount++;
            }
        });
       
        if (!refStore.updateProxy || !refStore.updateProxy.conn || !refStore.updateProxy.conn.url) {
            Ext.MessageBox.alert('Ошибка', "Не назначен метод контроллера для сохранения данных");
            return false;
        }
        
        var countOfRecords = createdRecordsCount + updatedRecordsCount + this.removed.length;
        if (countOfRecords == 0) {
            Ext.net.Notification.show(
            {
                iconCls: 'icon-information',
                html: 'Изменений нет',
                title: 'Уведомление',
                hideDelay: 2500
            });
            return false;
        }

        // формирование словаря параметров, сначала копия общих
        var requestOptions = this.getRequestOptions();
        // затем дописываются заданные для метода сохранения данных
        if (refStore.beforeSaveParams) {
            refStore.beforeSaveParams(this.store, requestOptions);
        }
        var recordsAll = {
            Created: createdRecords,
            Updated: updatedRecords,
            Deleted: this.removed
        };
        requestOptions.params['data'] = recordsAll;
        
        Ext.net.DirectMethod.request({
            cleanRequest: true,
            url: refStore.updateProxy.conn.url,
            eventMask: {
                showMask: true
            },
            timeout: 1000 * 60 * 2,
            params: requestOptions.params,
            success: function () {
                // сброс флагов 'новая запись'
                Ext.each(refStore.data.items, function (record) {
                    if (record.data.phantom) {
                        record.data.phantom = false;
                    }
                });
                // очистка списка удаленных записей
                refGrid.removed = [];
                // сброс флагов измененной записи
                refStore.commitChanges();
                // вызов события
                refStore.fireEvent('save', refStore, countOfRecords, recordsAll);
                // перегрузка содержимого
                refGrid.reloadHandler();
                // оповещение о сохранении
                Ext.net.Notification.show(
                {
                    iconCls: 'icon-information',
                    html: 'Изменения сохранены',
                    title: 'Уведомление',
                    hideDelay: 2500
                });
            },
            failure: function (response, options) {
                // оповещение об ошибке
                Ext.Msg.alert('Ошибка при сохранении', response);
                // вызов события
                refStore.fireEvent('saveexception', refGrid.store, options, response);
            }
        });

        // обновление интерфейса
        this.getView().refresh();

        return true;
    },
    
    // перегрузка содержимого таблицы
    reloadHandler: function () {
        // сброс активного элемента
        this.store.setActiveNode(null);
        // сворачивание всех уровней
        this.store.collapseAll();
        // отмена изменений
        this.store.rejectChanges();
        // формирование словаря параметров, сначала копия общих
        var requestOptions = this.getRequestOptions();
        // потом дописываются параметры для метода получения данных
        if (this.store.beforeLoadParams) {
            this.store.beforeLoadParams(this.store, requestOptions);
        }
        // перегрузка содержимого (на всякий случай, может на сервере изменилось)        
        this.store.load(requestOptions);
        // обновление интерфейса
        this.getView().refresh();
    },
    
    // копия параметров для запроса
    getRequestOptions: function() {
        var requestParams = {};
        if (this.store.baseParams) {
            for (var baseParamName in this.store.baseParams) {
                requestParams[baseParamName] = this.store.baseParams[baseParamName];
            }
        }
        return { params: requestParams };
    }
});


