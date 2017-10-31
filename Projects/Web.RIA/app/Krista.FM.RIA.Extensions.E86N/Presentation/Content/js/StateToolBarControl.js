Ext.ns('E86n.Control');
E86n.Control.StateToolBar =
    {
        // функция закрытия документа с параметрами readOnly, recId
        ReadOnlyDocHandler: {},

        showNoteWin: function (docId, error) {

            var myProxy = new Ext.data.HttpProxy({
                url: '/StartDoc/',
                api: {
                    read: { url: '/StartDoc/GetNote', method: 'GET' },
                    update: '/StartDoc/SetNote'
                },
                timeout: 60000
            });

            var myReader = new Ext.data.JsonReader({
                successProperty: 'success',
                idProperty: 'ID',
                root: 'data',
                fields: ["ID", "Note"]
            });

            var myWriter = new Ext.data.JsonWriter({
                encode: true,
                writeAllFields: false
            });

            var myStore = new Ext.data.Store({
                proxy: myProxy,
                reader: myReader,
                writer: myWriter,
                idProperty: 'id',
                autoSave: true,
                listeners: {
                    beforeload: function () {
                        var mask = new Ext.LoadMask(Ext.getBody(), { msg: 'Please wait...', store: myStore });
                        mask.show();
                    },
                    load: function (/*store, records, options*/) {
                        var val;
                        if (error === undefined) {
                            val = window.E86n.Control.StateToolBar.getNoteValue(myStore, docId);
                        } else {
                            val = error;
                        }
                        window.E86n.Control.StateToolBar.showWin(myStore, docId, val);
                    }
                }
            });

            myStore.setBaseParam('docId', docId);
            myStore.setBaseParam('id', docId);
            myStore.load();
        },

        showWin: function (myStore, docId, val, isGrbsOrAdmin) {
            var form = new Ext.form.FormPanel({
                border: false,
                layout: 'anchor',
                bodyStyle: { 'background-color': '#E0E6F8' },
                items: [{
                    xtype: 'textarea',
                    id: 'message',
                    border: false,
                    frame: false,
                    value: val,
                    anchor: '100% 100%',
                    editable: isGrbsOrAdmin
                }]
            });
            var feedWin = new Ext.Window({
                id: 'win',
                width: 500,
                height: 300,
                minWidth: 300,
                minHeight: 200,
                layout: 'fit',
                title: 'Примечание',
                modal: true,
                items: form,
                buttons: [{
                    text: 'Сохранить',
                    id: 'buttonOK',
                    handler: function () {
                        var note = Ext.getCmp('message').getValue();
                        var index = myStore.findExact('ID', docId);
                        var record = myStore.getAt(index);
                        record.set('Note', note);
                        feedWin.close();
                        if (Ext.getCmp('Documents')) {
                            var grid = Ext.getCmp('Documents');
                            grid.getStore().reload();
                        }
                    }
                },
                        {
                            text: 'Отмена',
                            handler: function () {
                                feedWin.close();
                            }
                        }]
            });

            feedWin.show();
        },

        getNoteValue: function (mStore, docId) {

            var index = mStore.findExact('ID', docId);
            var record = mStore.getAt(index);
            return record.get('Note');
        },

        checkDocumentClosure: function (recId, isAdmin) {
            var proxy = new Ext.data.HttpProxy({
                url: '/ParameterDoc/',
                api: {
                    read: { url: '/ParameterDoc/Read', method: 'GET' }
                },
                timeout: 60000
            });

            var reader = new Ext.data.JsonReader({
                successProperty: 'success',
                idProperty: 'ID',
                root: 'data',
                fields: ["ID", "CloseDate", "RefPartDocID"]
            });

            var writer = new Ext.data.JsonWriter({
                encode: true,
                writeAllFields: false
            });

            var store = new Ext.data.Store({
                proxy: proxy,
                reader: reader,
                writer: writer,
                idProperty: 'id',
                autoSave: true,
                listeners:
                    {
                        load: function () {
                            var record = this.getAt(0);
                            if (!!record.get('CloseDate')) {
                                window.E86n.Control.StateToolBar.SetCloseDoc(recId, isAdmin);
                            }
                            else {
                                if (Ext.getCmp('btnStateClose') != undefined) {
                                    Ext.getCmp('btnStateClose').setDisabled(!isAdmin);
                                    window.btnStateClose.toggle(false);
                                }
                        
                                if (Ext.getCmp('btnStateOpen') != undefined) {
                                    Ext.getCmp('btnStateOpen').setDisabled(true);
                                    window.btnStateOpen.toggle(true);
                                }
                            }
                        }
                    }
                });
            store.setBaseParam('itemId', recId);
            store.load();
        },

        getCloseDateValue: function (recId) {
            var proxy = new Ext.data.HttpProxy({
                url: '/ParameterDoc/',
                api: {
                    read: { url: '/ParameterDoc/Read', method: 'GET' }
                },
                async: true,
                timeout: 60000
            });

            var reader = new Ext.data.JsonReader({
                successProperty: 'success',
                idProperty: 'ID',
                root: 'data',
                fields: ["ID", "CloseDate"]
            });

            var writer = new Ext.data.JsonWriter({
                encode: true,
                writeAllFields: false
            });

            var store = new Ext.data.Store({
                proxy: proxy,
                reader: reader,
                writer: writer,
                idProperty: 'id',
                autoSave: true
            });

            store.setBaseParam('parentId', recId);
            store.load();
            var record = store.getAt(0);
            return record.get('CloseDate');

        },
        
        //закрытие\открытие формы
        // todo нужно вынести в общий функционал
        SetReadOnlyFormPanel: function (formpanel, readOnly) {
            var form = Ext.getCmp(formpanel);
            var saveBtn = Ext.getCmp(formpanel + 'SaveBtn');
            if (saveBtn) {
                saveBtn.setVisible(!readOnly);
            }
            var componentcount = form.items.getCount();
            var i;
            for (i = 0; i < componentcount; i++) {
                var cmp = form.items.itemAt(i);
                //для контейнеров рекурсивно вызываем
                if (cmp.items)
                    window.E86n.Control.StateToolBar.SetReadOnlyFormPanel(cmp.id, readOnly);
                if (cmp.setReadOnly)
                    cmp.setReadOnly(readOnly);
            }
        },

        closeDoc: function (recId) {
            var proxy = new Ext.data.HttpProxy({
                url: '/ParameterDoc/',
                api: {
                    read: { url: '/ParameterDoc/Read', method: 'GET' },
                    update: '/ParameterDoc/Update'
                },
                async: true,
                timeout: 60000
            });

            var reader = new Ext.data.JsonReader({
                successProperty: 'success',
                idProperty: 'ID',
                root: 'data',
                fields: ["ID", "RefPartDocID", "RefSostID", "RefUchrID", "RefYearFormID", "Note", "PlanThreeYear", "CloseDate"]
            });

            var writer = new Ext.data.JsonWriter({
                encode: true,
                writeAllFields: true
            });

            var store1 = new Ext.data.Store({
                proxy: proxy,
                reader: reader,
                writer: writer,
                idProperty: 'id',
                autoSave: true,
                validateRequest: false,
                listeners: {
                    load: function (store) {
                        var record = store.getAt(0);
                        record.set('CloseDate', new Date());
                    }

                }
            });
            store1.setBaseParam('itemId', recId);
            store1.load();

        },

        // todo нужно вынести в общий функционал
        //закрытие\открытие грида и его "стандартных" кнопок
        SetReadOnlyGrid: function (readOnly, gridName, doStdBtn) {
            var grid = Ext.getCmp(gridName);
            if (grid) {
                var colModel = grid.getColumnModel();
                var colcount = colModel.getColumnCount(false);
                var i;
                for (i = 0; i < colcount; i++) {
                    colModel.setEditable(i, !readOnly);
                }

                if (doStdBtn === undefined) {
                    doStdBtn = true;
                }

                if (doStdBtn) {
                    var btn = Ext.getCmp(gridName + 'SaveBtn');
                    if (btn) {
                        btn.setVisible(!readOnly);
                    }

                    btn = Ext.getCmp(gridName + 'RemoveRecordBtn');
                    if (btn) {
                        btn.setVisible(!readOnly);
                    }

                    btn = Ext.getCmp(gridName + 'NewRecordBtn');
                    if (btn) {
                        btn.setVisible(!readOnly);
                    }

                    btn = Ext.getCmp(gridName + 'SummBtn');
                    if (btn) {
                        btn.setVisible(!readOnly);
                    }
                }
            }
        },

        //переменная нужна для скрытия команды добавления документа в группировке в приложениях
        hid: false,

        //обработчик команды скрывающий кнопку команды
        prepareCommand: function (command) {
            command.hidden = window.E86n.Control.StateToolBar.hid;
        },

        // todo нужно вынести в общий функционал
        //закрытие\открытие вкладки приложений для документа
        ReadOnlyDocs: function (readOnly, recId) {
            //коряво скрываем команду добаления документа в группировке
            //пример в паспорте приходится вешать обработчик PrepareGroupCommand
            window.E86n.Control.StateToolBar.hid = readOnly;

            var grid = Ext.getCmp('gpDocs' + recId);
            if (grid) {
                //колонка в гриде с командой удалить документ
                var colIndex = grid.getColumnModel().getIndexById('ComandColumn');
                grid.getColumnModel().setHidden(colIndex, readOnly);

                //нужно обновление грида для перерисовки команды - группы
                //для этого перегружаем стор
                grid.getStore().reload();
            }

            window.adfName.setReadOnly(readOnly);

            window.adfDocDate.setReadOnly(readOnly);

            window.adfNumberNPA.setReadOnly(readOnly);

            //кнопка Сохранить документ
            window.adfSave.setVisible(!readOnly);
            //кнопка Удалить загруженный документ
            eval('adfDeleteBtn' + recId).setVisible(!readOnly);
            //кнопка Выбрать файл
            eval('adfFileName' + recId).setVisible(!readOnly);
        },

        // todo нужно вынести в общий функционал
        // закрытие документа
        SetCloseDoc: function (recId, isAdmin) {
            // ищем группу кнопок переходов
            var cmp = Ext.getCmp('StatesButtonGroup');
            if (cmp) {
                // закрываем кнопки переходов
                cmp.items.each(function (item) {
                    item.disable();
                });
            }

            // обрабатываем кнопки версионной группы 
            var btn = Ext.getCmp('btnStateClose');
            if (btn) {
                btn.disable();
                btn.toggle(true);
            }

            btn = Ext.getCmp('btnStateOpen');
            if (btn) {
                btn.setDisabled(!isAdmin);
                btn.toggle(false);
            }

            btn = Ext.getCmp('btnCopy');
            if (btn) {
                btn.disable();
            }

            // закрываем содержимое документа
            if (typeof window.E86n.Control.StateToolBar.ReadOnlyDocHandler == 'function') {
                window.E86n.Control.StateToolBar.ReadOnlyDocHandler(true, recId);
            }
        }
    };