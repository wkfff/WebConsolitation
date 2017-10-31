Ext.ns('E86n.View.Documents');

E86n.View.Documents.Grid =
    {
        getRowClass: function (record) {
            switch (record.data.State) {
            case "Создан":
            case "'Значение не указано'":
                return 'not_active-row';
            case "На рассмотрении":
                return 'under_advisement-row';
            case "Экспортирован":
                return 'is_exported-row';
            case "Завершено":
                return 'is_processed-row';
            default:
                return '';
            }
        },

        actionHandler: function (command, record) {
            switch (command.command) {
            case "OpenDocument":
                window.parent.MdiTab.addTab({ title: record.data.Type, url: record.data.Url });
                break;
            case "ExportDocument":
                window.wndUploadGMU.docs = record.data.ID;
                window.wndUploadGMU.show();
                break;
            case "EditNote":
                window.E86n.Control.StateToolBar.showNoteWin(record.data.ID);
                break;
            case "DeleteDoc":
                Ext.Msg.show({
                    title: 'Подтверждение',
                    msg: 'Удалить документ?',
                    width: 300,
                    buttons: Ext.Msg.YESNO,
                    fn: function (btn) {
                        if (btn == 'yes') {
                            window.dsDocuments.remove(record);
                            window.dsDocuments.save();
                            window.dsDocuments.commitChanges();

                        }
                    },
                    modal: true,
                    icon: Ext.Msg.QUESTION,
                    closable: false
                });
                break;
            case "OpenDoc":
                Ext.Msg.show({
                    title: 'Подтверждение',
                    msg: 'Выполнение этой операции может привести к появлению нескольких открытых документов. Вы подтверждаете открытие?',
                    width: 300,
                    buttons: Ext.Msg.YESNO,
                    fn: function (btn) {
                        if (btn == 'yes') {
                            Ext.Ajax.request({
                                url: '/Documents/OpenDocument',
                                params: { recId: record.data.ID }
                            });
                            window.dsDocuments.reload();
                        }
                    },
                    modal: true,
                    icon: Ext.Msg.QUESTION,
                    closable: false
                });
                break;
                case "CloseDoc":
                    Ext.Msg.show({
                        title: 'Подтверждение',
                        msg: 'Вы подтверждаете закрытие??',
                        width: 300,
                        buttons: Ext.Msg.YESNO,
                        fn: function (btn) {
                            if (btn == 'yes') {
                                Ext.Ajax.request({
                                    url: '/Documents/CloseDocument',
                                    params: { recId: record.data.ID }
                                });
                                window.dsDocuments.reload();
                            }
                        },
                        modal: true,
                        icon: Ext.Msg.QUESTION,
                        closable: false
                    });
                    break;
            default:
            }
        },

        actionHandler1: function (item, rowIndex, columnIndex, e) {
            if (item.getColumnModel().getDataIndex(columnIndex) == 'Action') {
                var record = item.getStore().getAt(rowIndex);

                var cmd = e.getTarget().attributes.cmd;

                if (cmd) {
                    var command = {
                        command: cmd.value,
                    };
                    window.E86n.View.Documents.Grid.actionHandler(command, record, rowIndex, columnIndex);
                }
            }
        },

        prepareCommand: function (grid, command, record) {
            if (command.command == 'ExportDocument' && (record.data.State != 'Завершено' || record.data.Closed)) {
                command.hidden = true;
                command.hideMode = 'visibility';
            }
            if (command.command == 'ClosedDocument' && !record.data.Closed) {
                command.hidden = true;
                command.hideMode = 'visibility';
            }
            if (command.command == 'ClosedOrg' && !record.data.ClosedOrg) {
                command.hidden = true;
                command.hideMode = 'visibility';
            }
            if (command.command == 'OpenDoc' && !record.data.Closed) {
                command.hidden = true;
            }
            if (command.command == 'CloseDoc' && record.data.Closed) {
                command.hidden = true;
            }
        },

        toggleFilter: function (button) {
            var filters = Ext.getCmp('Documents').filters;
            var closed = filters.filters.get('Closed');
            var closedOrg = filters.filters.get('ClosedOrg');
            
            if (button.id == "filterNotClosed") {
                Ext.getCmp('filterClosed').toggle(false);
                closed.setValue(!button.pressed);
                closed.setActive(button.pressed);
            }
            if (button.id == "filterClosed") {
                Ext.getCmp('filterNotClosed').toggle(false);
                closed.setValue(button.pressed);
                closed.setActive(button.pressed);
            }
            if (button.id == "filterNotClosedOrg") {
                Ext.getCmp('filterClosedOrg').toggle(false);
                closedOrg.setValue(!button.pressed);
                closedOrg.setActive(button.pressed);
            }
            if (button.id == "filterClosedOrg") {
                Ext.getCmp('filterNotClosedOrg').toggle(false);
                closedOrg.setValue(button.pressed);
                closedOrg.setActive(button.pressed);
            }
        
            filters.reload();
        },
        
        InitView: function () {
            var button = Ext.getCmp('filterNotClosedOrg');
            button.toggle(true);
            this.toggleFilter(button);
        },

        BatchExport: function () {
            window.wndUploadGMU.docs = window.dsDocumentsStore.data.items.filter(item => { return item.data.Check }).map(item => { return item.id });
            window.wndUploadGMU.show();
        },

        SelectAll: function () {
            window.E86n.View.Documents.Grid.SelectDeselect(true);
        },

        DeselectAll: function () {
            window.E86n.View.Documents.Grid.SelectDeselect(false);
        },

        SelectDeselect: function (ckeck) {
            window.dsDocumentsStore.data.items.forEach(item => { item.data.Check = ckeck });
            window.Documents.view.refresh(true);
        }
    };