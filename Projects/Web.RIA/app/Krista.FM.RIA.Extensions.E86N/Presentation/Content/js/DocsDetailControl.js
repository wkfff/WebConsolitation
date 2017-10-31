Ext.ns('E86n.Control');
E86n.Control.DocsDetailControl =
    {
        ValidateFile: function (el, totalDocsSize) {
            var file = el.files[0];

            if (file.size > 38535168) {
                return 'Размер файла не может быть больше 36,5 мегабайт';
            }

            if (file.size + totalDocsSize > 38535168) {
                return 'Размер прикрепленных к документу файлов превысит 50Мб';
            }
            
            return '';
        },

        BeforeFileSelected: function (el, extraParams, totalDocsSize, urlField) {
            var validate = window.E86n.Control.DocsDetailControl.ValidateFile(el.fileInput.dom, totalDocsSize.getValue());

            if (validate.length > 0) {
                Ext.Msg.show({ title: 'Ошибка валидации', msg: validate, minWidth: 200, modal: true, icon: Ext.Msg.ERROR, buttons: Ext.Msg.OK });
                el.reset();
                return false;
            }

            var knownFile = urlField.getValue();
            if (!(knownFile === 'НетФайла'
                    || knownFile === 'НеУказан'
                    || knownFile === 'НеЗадан'
                    || knownFile === ''
                    || knownFile == null))
            {
                Ext.Msg.show( { title: 'Предупреждение', msg: 'Файл для этого документа уже был загружен. Сначала удалите существующий файл.', minWidth: 200, modal: true, icon: Ext.Msg.WARNING, buttons: Ext.Msg.OK } );
                el.reset();
                return false;
            }

            var fileName = el.getValue();
            extraParams.fileName = fileName;
            extraParams.size = el.fileInput.dom.files[0].size;
            if ( ! confirm('Вы подтверждаете загрузку файла ' + fileName + '?'))
                return false;
            
            Ext.Msg.wait('Передача файла...', 'Загрузка');
            return true;
        },

        GroupCommand: function (grid, command, groupId, records) {
            var record = grid.insertRecord({ commit: 'false' });
            record._groupId = records[0]._groupId;
            record.data = records[0].data;
            record.data.ID = record.id;
            record.data.Name = '(' + record.data.RefTypeDocName + ')';
            record.data.DocDate = new Date();
            record.data.Url = 'НетФайла';

            grid.view.refresh(true);
            grid.store.save();
            grid.store.commitChanges();
            grid.store.reload();
            grid.getSelectionModel().selectFirstRow();
        },

        Command: function (grid, record) {
            grid.deleteRecord(record);
            grid.store.save();
        },

        RowSelect: function (record, form, urlField, downloadBtn, deleteBtn, formFileField) {
            window.Ext.getCmp('adfNumberNPA').setVisible(record.data.RefTypeDoc === 5);
            form.getForm().loadRecord(record);
            form.setDisabled(false);
            var knownFile = urlField.getValue();
            var filePresent = (knownFile === 'НетФайла' ||  knownFile === 'НеУказан' || knownFile === 'НеЗадан' || knownFile === '' || knownFile == null);
            downloadBtn.setDisabled( filePresent );
            deleteBtn.setDisabled( filePresent );
            window.E86n.View.UIBuilders.appliedDocumentGridSelectRow(record, form);
            formFileField.reset();
        },

        BeforeDownload: function (extraParams, idField, formFileField, urlField) {
            var knownFile = urlField.getValue();
            if ( knownFile === 'НетФайла' ||  knownFile === 'НеУказан' || knownFile === 'НеЗадан' || knownFile === '' || knownFile == null )
            {
                Ext.Msg.alert('Ошибка', 'Нет данных для просмотра. Очевидно, документ еще не загружали.', idField);
                return false;
            }
            extraParams.id = idField.getValue();
            extraParams.fileName = formFileField.getValue();
            return true;
        },

        BeforeDelete: function (extraParams, idField, urlField) {
            var knownFile = urlField.getValue();
            if ( knownFile === 'НетФайла' ||  knownFile === 'НеУказан' || knownFile === 'НеЗадан' || knownFile === '' || knownFile == null )
            {
                Ext.Msg.alert('Ошибка', 'Нет файла для удаления. Очевидно, документ еще не загружали.', idField);
                return false;
            }
           
            if ( ! confirm('Вы подтверждаете удаление загруженного ранее файла?'))
                return false;

            extraParams.id = idField.getValue();
            return true;
        }
    };