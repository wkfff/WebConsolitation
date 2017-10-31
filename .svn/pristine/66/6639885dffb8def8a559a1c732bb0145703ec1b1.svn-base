Ext.override(Ext.ux.maximgb.tg.EditorGridPanel, {

    tempId: -2,
    removed: [],
    /* обработчик при добавлении записи */
    /**
    * @access public
    */
    addHandler: function () {
        /*получаем выбранную запись r*/
        r = this.getSelectionModel().getSelected();
        /*получаем список полей f*/
        var f = this.store.recordType.prototype.fields,
            dv = {};
        /*инициализируем поля значениями по умолчанию*/
        for (var i = 0; i < f.length; i++) {
            dv[f.items[i].name] = f.items[i].defaultValue;
        }

        /*присваиваем временный идентификатор*/
        dv['ID'] = this.tempId;
        /*отмечаем, что запись новая*/
        dv.phantom = true;
        /*формируем новую запись*/
        var myNewRecord = new this.store.recordType(dv, this.tempId);
        this.tempId--;
        /*добавляем новую запись к выбранному узлу*/
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
        var record = this.getSelectionModel().getSelected();
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
        /* отменяем добавление новых записей */
        // список записей, которые надо удалить
        toRemove = [];
        // пробегаем по всем записям в store
        Ext.each(this.store.data.items, function (record) {
            // если запись новая
            if (record.data.phantom) {
                // добавляем в список на удаление
                toRemove.push(record);
            }
        });
        // удаляем новые записи (не сохраненные в бд)
        Ext.each(toRemove, function (record) {
            this.store.remove(record);
        });

        /*отменяем удаление записей*/
        //восстанавливаем узлы с удаленными записями
        i = 0;
        cnt = this.removed.length - 1
        for (i = cnt; i >= 0; i--) {
            rcD = this.removed[i];
            if (!rcD.phantom) {
                row = this.store.indexOfId(rcD['PARENTID']);
                if (row >= 0) {
                    r = this.store.getAt(row);
                    /*формируем запись*/
                    var myNewRecord = new this.store.recordType(rcD, rcD['ID']);
                    /*добавляем запись к узлу r*/
                    this.store.addToNode(r, myNewRecord);
                    /*расхлапываем выбранный узел*/
                    this.store.expandNode(r);
                }
            }
        };
        /*отменяем обновление записей*/
        this.store.rejectChanges();
        /* сохраняем изменения*/
        this.store.commitChanges();
        /*обновляем интерфейс*/
        this.getView().refresh();
    }
});

