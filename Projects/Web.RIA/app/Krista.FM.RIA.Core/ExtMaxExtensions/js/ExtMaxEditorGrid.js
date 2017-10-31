OnLoadGridH = function () {
    initGridData = function () {
        /*список удаленных записей*/
        removed = [];

        /*обработчик при сохранении изменений*/
        saveHandler = function () {
            var gridShowHierarchy = Ext.getCmp(GridId);
            gridShowHierarchy.store.save();
            /*список измененных записей*/
            var data = [];
            var updatedSize = 0;
            /*список добавленных записей*/
            var addedData = [];
            var addedSize = 0;
            /*формируем список измененных записей*/
            Ext.each(gridShowHierarchy.store.getModifiedRecords(), function (record) {
                updatedSize++;
                data.push(record.data);
            });
            /*формируем список добавленных записей*/
            Ext.each(gridShowHierarchy.store.data.items, function (record) {
                if (record.data.phantom) {
                    addedData.push(record.data);
                    addedSize++;
                }
            });
            /*если есть добавленные записи, отправлем запрос на сервер*/
            if (addedSize)
                Ext.Ajax.request(
                        {
                            url: '/Entity/Save?objectKey="' + objKey + '"',
                            params: {
                                data: '{"Created:":"' + Ext.encode(addedData) + '", hierarchy: true}'
                            },
                            success: function (response, options) {
                                addedSuccess(response);
                            },
                            failure: function (response, options) {
                                Ext.MessageBox.alert('Message', response.statusText);
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
                        ajaxSuccess(response);
                    },
                    failure: function (response, options) {
                        Ext.MessageBox.alert('Message', response.statusText);
                    }
                });
            /*если есть удаленные записи, отправлем запрос на сервер*/
            if (removed.length > 0)
                Ext.Ajax.request({
                    url: '/Entity/Save?objectKey="' + objKey + '"',
                    params: {
                        data: '{"Deleted":"' + Ext.encode(removed) + '"}'
                    },
                    success: function (response, options) {
                        deleteSuccess(response);
                    },
                    failure: function (response, options) {
                        Ext.MessageBox.alert('Message', response.statusText);
                    }
                });
            /*обновляем интерфейс*/
            gridShowHierarchy.getView().refresh();
        }

        /* обработчик при добавлении записи */
        addHandler = function () {
            var gridShowHierarchy = Ext.getCmp(GridId);
            /*получаем выбранную запись r*/
            r = gridShowHierarchy.getSelectionModel().getSelected();
            /*получаем список полей f*/
            var f = gridShowHierarchy.store.recordType.prototype.fields,
                dv = {};
            /*инициализируем поля значениями по умолчанию*/
            for (var i = 0; i < f.length; i++) {
                dv[f.items[i].name] = f.items[i].defaultValue;
            }

            // !!!!!!!!!!!!!!   
            /*        for (var iAttr = 0; iAttr < AttributesDefaultValues; iAttr++) {
            dv[AttributesDefaultValues.key] = AttributesDefaultValues.value;
            }*/

            /*присваиваем временный идентификатор*/
            dv['ID'] = ID;
            /*отмечаем, что запись новая*/
            dv.phantom = true;
            /*формируем новую запись*/
            var myNewRecord = new gridShowHierarchy.store.recordType(dv, ID);
            ID--;
            /*добавляем новую запись к выбранному узлу*/
            gridShowHierarchy.store.addToNode(r, myNewRecord);
            /*расхлапываем выбранный узел*/
            gridShowHierarchy.store.expandNode(r);
            /*обновляем интерфейс*/
            gridShowHierarchy.getView().refresh();
        }

        // сохранение прошло успешно
        function ajaxSuccess(response) {
            var grid = Ext.getCmp(GridId);
            var ds = grid.getStore();
            var xml = response.responseXML;
            if (xml)
                var errors = xml.getElementsByTagName('error');
            if (!errors || !errors.length)
                ds.commitChanges();
            else {
                var l = errors.length;
                var msg = '';
                for (var f = 0; f < l; f++)
                    msg += errors[f].firstChild.nodeValue + '\n';
                Ext.MessageBox.alert('Ошибка', msg, 1);
            }
        }

        /*Вызывается, когда записи были корректно удалены*/
        function deleteSuccess(response) {
            ajaxSuccess(response);
            // опустошаем список удаленных записей
            removed = [];
        }

        /*Вызывается, когда записи были корректно добавлены*/
        function addedSuccess(response) {
            var gridShowHierarchy = Ext.getCmp(GridId);
            /*Снимаем отметки 'новая запись'*/
            Ext.each(gridShowHierarchy.store.data.items, function (record) {
                if (record.data.phantom) {
                    record.data.phantom = false;
                }
            });
            ajaxSuccess(response);
        }

        // обработчик выбора записи в таблице
        mousedownHandler = function (g, ri, ci, e) {
            var t = e.getTarget();
            // открытие справочника
            if (t.className == 'EditRefCell') {
                var r = g.getStore().getAt(ri);
                var fieldName = g.getColumnModel().getDataIndex(ci);

                createBookWindow();

                curFieldName = fieldName;
                Extension.EntityView.activeRefField = fieldName.substring(3, fieldName.length);
                Extension.EntityView.activeLookupField = fieldName;
                var bookWindow = Ext.getCmp(BookWindowId);
                bookWindow.autoLoad.params.id = record.id;
                bookWindow.autoLoad.url = '/Entity/Book?objectKey=' + MetaData[Extension.EntityView.activeRefField].objectKey;
                bookWindow.setTitle(MetaData[Extension.EntityView.activeRefField].caption);
                btnOk.disable();
                bookWindow.show();
            }
        }

        Ext.ns('MetaData');

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
            gridShowHierarchy.master_column_id = "NAME";
            /*отменяем обновление записей*/
            gridShowHierarchy.store.rejectChanges();
            /* сохраняем изменения*/
            gridShowHierarchy.store.commitChanges();
            /*обновляем интерфейс*/
            gridShowHierarchy.getView().refresh();
        }

        var gridShowHierarchy = Ext.getCmp(GridId);

        gridShowHierarchy.store.writer = new Ext.data.JsonWriter({
        encode: true,
        writeAllFields: false
        });
        gridShowHierarchy.store.writer.meta = grid.store.reader.meta;

    }
    return {
        init: function () {
            Ext.BLANK_IMAGE_URL = '';
            initGridData();
        }
    }

} ();
