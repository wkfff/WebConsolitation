﻿OnLoadGridH = function () {
    initGridData = function () {
        record = Ext.data.Record.create(RecordFields);

        /*список удаленных записей*/
        removed = [];

        applyFilter = false;

        // перед загрузкой данных сущности устанавливаем значения передаваемых серверу параметров
        onbeforeLoad = function (store) {
            store.baseParams = { objectkey: objKey, limit: 10, start: 0, dir: 'ASC', sort: 'ID' };
            if (isDivided)
                store.baseParams.source = comboSource.value;
            if (paramServerFilter != null)
                store.baseParams.serverFilter = paramServerFilter;
            if (paramShowMode != null)
                store.baseParams.showMode = paramShowMode;
        };

        // фильтры для каждого из полей
        efilters = Filters;

        efilters.id = 'eF';

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

        if (isDivided)
            createSources();

        Controller.init();
        Ext.getCmp(GridId).renderTo = PanelId;

        // обработчик выбора записи в таблице
        var mousedownHandler = function (g, ri, ci, e) {
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

        if (IsEditable) {
            // добавляем обработчик на выбор записи
            Ext.getCmp(GridId).on({ cellmousedown: mousedownHandler });
        }

        Ext.ns('MetaData');
        MetaData = AssociationsMetaData;

    }

    return {
        init: function () {
            Ext.BLANK_IMAGE_URL = '';
            initGridData();
        }
    }

} ();