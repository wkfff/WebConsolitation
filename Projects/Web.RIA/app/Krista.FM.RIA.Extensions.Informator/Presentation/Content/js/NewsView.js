Ext.ns('Informator.View.News');

Informator.View.News.Grid =
{
    showNewsWin: function (gridID, ID, type) {
        var myProxy = new Ext.data.HttpProxy
        ({
            url: '/News/',
            api:
            {
                read: { url: '/News/ReadMessage', method: 'GET' },
                update: '/News/Update'
            },
            timeout: 60000
        });

        var myReader = new Ext.data.JsonReader
        ({
            successProperty: 'success',
            idProperty: 'ID',
            root: 'data',
            fields: ["ID", "ReceivedDate", "MessageStatus", "MessageImportance", "Subject", "Body", "RefUserSender", "WriteMessageButton"]
        });

        var myWriter = new Ext.data.JsonWriter
        ({
            encode: true,
            writeAllFields: false
        });

        var myStore = new Ext.data.Store
        ({
            id: 'newsStore',
            proxy: myProxy,
            reader: myReader,
            writer: myWriter,
            idProperty: 'id',
            autoSave: true,
            listeners:
            {
                load: function (store, records, options) {
                    Informator.View.News.Grid.showWin(gridID, myStore, ID, type);

                }
            }
        });
        myStore.setBaseParam('messageId', ID);

        myStore.load();
    },

    showWin: function (gridID, myStore, ID) {
        panel = new Ext.Panel
        ({
            width: 500,
            html: "<p>Тема: " + Informator.View.News.Grid.getSubject(ID, myStore) + "</p>\r\n" +
            "<p>Новость: " + Informator.View.News.Grid.getMessage(ID, myStore) + "</p>"
        });
        form = new Ext.form.FormPanel
        ({
            border: false,
            layout: 'anchor',
            bodyStyle: { 'background-color': '#E0E6F8' },
            items:
            [{
                xtype: 'textarea',
                id: 'Тема',
                border: true,
                frame: true,
                value: "Тема: " + Informator.View.News.Grid.getSubject(ID, myStore),
                anchor: '100% 20%'
            },
            {
                xtype: 'textarea',
                id: 'Новость',
                border: true,
                frame: true,
                value: "Новость: " + Informator.View.News.Grid.getMessage(ID, myStore),
                anchor: '100% 80%'
            }]
        });

        feedWin = new Ext.Window
        ({
            id: 'win',
            width: 500,
            height: 300,
            minWidth: 300,
            minHeight: 200,
            layout: 'fit',
            title: 'Новость',
            modal: true,
            items: panel,
            buttons:
            [{
                text: 'ОК',
                id: 'buttonOK',
                handler: function () {
                    index = myStore.findExact('ID', ID);
                    record = myStore.getAt(index);
                    if (gridID != "outboxNews") {
                        record.set('MessageStatus', 'Read');
                    }
                    feedWin.close();
                    if (Ext.getCmp('News')) {
                        var grid = Ext.getCmp('News');
                        grid.getStore().reload();
                    }
                }
            }]
        });

        feedWin.show();
    },

    getSubject: function (ID, myStore) {
        var index = myStore.findExact('ID', ID);
        var record = myStore.getAt(index);
        var subject = record.get('Subject');
        return subject;

    },

    getMessage: function (ID, myStore) {
        var index = myStore.findExact('ID', ID);
        var record = myStore.getAt(index);
        var subject = record.get('Body');
        return subject;

    },

    showAttach: function (Attachment) {
        window.document.location.href = "/News/Download?Attachment=" + Attachment;
    },

    getRowClass: function (record, index, rowParams, store) {
        var rowBody = "";
        var subject = record.get("Subject");
        var index = subject.search("/separator/");
        subject = subject.substr(0, index);
        rowBody += "<font size='2'><b>Тема новости: " + "<p><dl>" + subject + "</dl></p><b></font>";

        rowParams.body = rowBody;
        //        if (record.get('MessageStatus') === 'Новая')
        //            return 'new_message-row';
        //        if (record.get('MessageStatus') === 'Прочитана')
        //            return 'read_message-row';
        //        if (record.get('MessageStatus') === 'Удалена')
        //            return 'delete_message-row';

    },

    toggleFilter: function (button, state) {
        var grid = Ext.getCmp('News');

        var filters = grid.filters;
        var new_message = filters.filters.get('new_message');
        var read_message = filters.filters.get('read_message');
        var exclamation_message = filters.filters.get('exclamation_message');
        var regular_message = filters.filters.get('regular_message');

        if (button.id === "filterNew" || button.id === "filterRead" || button.id === "filterExclamation"
        || button.id === "filterRegular") {
            var filterNew = Ext.getCmp('filterNew');
            var filterRead = Ext.getCmp('filterRead');
            var filterExclamation = Ext.getCmp('filterExclamation');
            var filterRegular = Ext.getCmp('filterRegular');

            new_message.setValue(filterNew.pressed);
            new_message.setActive(filterNew.pressed);

            read_message.setValue(filterRead.pressed);
            read_message.setActive(filterRead.pressed);

            exclamation_message.setValue(filterExclamation.pressed);
            exclamation_message.setActive(filterExclamation.pressed);

            regular_message.setValue(filterRegular.pressed);
            regular_message.setActive(filterRegular.pressed);
        }

        filters.reload();
    },

    sendtoggleFilter: function (button, state) {
        var grid = Ext.getCmp('outboxNews');

        var filters = grid.filters;
        var exclamation_message = filters.filters.get('exclamation_message');
        var regular_message = filters.filters.get('regular_message');

        if (button.id === "filterExclamation" || button.id === "filterRegular") {
            var filterExclamation = Ext.getCmp('filterExclamation');
            var filterRegular = Ext.getCmp('filterRegular');

            exclamation_message.setValue(filterExclamation.pressed);
            exclamation_message.setActive(filterExclamation.pressed);

            regular_message.setValue(filterRegular.pressed);
            regular_message.setActive(filterRegular.pressed);
        }

        filters.reload();
    },

    actionHandler: function (grid, command, record, rowIndex, colIndex) {
        switch (command.command) {
            case "OpenNews":
                Informator.View.News.Grid.showNewsWin(grid.ID, record.data.ID, grid.id);
                break;
            case "OpenAttach":
                Informator.View.News.Grid.showAttach(record.data.RefMessageAttachment);
                break;
            default:
        }

    },

    actionHandler1: function (item, rowIndex, columnIndex, e) {
        if (item.getColumnModel().getDataIndex(columnIndex) == 'Action') {
            var record = item.getStore().getAt(rowIndex);
            var command = {
                command: e.getTarget().attributes.cmd.value

            };
            var grid;
            if (Ext.getCmp('News')) {
                grid = Ext.getCmp('News');
            }
            else {
                grid = Ext.getCmp('outboxNews');
            }
            switch (command.command) {
                case "OpenNews":
                    Informator.View.News.Grid.showNewsWin(grid.id, record.data.ID, grid.id);
                    break;
                case "OpenAttach":
                    Informator.View.News.Grid.showAttach(record.data.RefMessageAttachment);
                    break;
                default:
            }
        }

    },

    prepareCommand: function (grid, command, record, row) {
        if ((command.command == 'OpenAttach' && record.data.MessageAttachment == false) ||
           (command.command == 'ImpNews' && record.data.MessageImportance == 'Обычная')) {
            command.hidden = true;
            command.hideMode = 'visibility';
        };

    },

    getIds: function (gridName) {
        var grid = Ext.getCmp(gridName);
        if (grid) {
            var selection = grid.getSelectionModel().getSelections();
            var data = '';
            for (var i = 0; i < selection.length; i++) {
                data += selection[i].data.ID + ',';
            }
            return data;
        }
        else {
            return '';
        }
    }
};



