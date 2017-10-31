Ext.ns('E86n.View');

function reloadDetail() {
    var activeTab = window.DetailTabPanel.getActiveTab();

    //Обновляем детализацию и сбрасываем выделение 
    activeTab.reload();
    activeTab.getSelectionModel().clearSelections();
}

E86n.View.Service2016 =
    {
        UpdateView: function () {
            if (window.Service2016.getSelectionModel().hasSelection()) {
                if (typeof window.Service2016RemoveRecordBtn !== "undefined")
                    window.Service2016RemoveRecordBtn.setDisabled(false);

                var row = window.Service2016.getSelectionModel().getSelected();
                if (row.id > 0) {
                    window.DetailTabPanel.setDisabled(false);
                    reloadDetail();
                } else {
                    window.DetailTabPanel.setDisabled(true);
                }
            } else {
                //Если нет выделеных элементов - детализация и кнопка уделания не активны
                window.DetailTabPanel.setDisabled(true);
            }
        },

        ChangePage: function () {
            //При переходе на дгурую станицу в центральном гриде - сбрасываем выделение и обновляем.
            window.Service2016.getSelectionModel().clearSelections();
            window.E86n.View.Service2016.UpdateView();
        },

        getSelectedServiceId: function () {
            var selection = window.Service2016.getSelectionModel();
            if (selection.hasSelection()) {
                return selection.getSelected().id;
            } else return -1;
        },

        Copy: function () {
            Ext.Ajax.request({
                url: '/Service2016/Copy',
                params: { id: Service2016.getSelectionModel().getSelected().id },
                failure: function () {
                    Ext.Msg.show({ title: 'Ошибка', msg: 'Не удалось скопировать', buttons: Ext.MessageBox.OK });
                },
                success: function () {
                    Ext.Msg.show({
                        title: 'Успех', msg: 'Копирование прошло успешно. ' +
                            '<br>Скопированная услуга помещена в Ведомственный перечень из иных источников.', buttons: Ext.MessageBox.OK
                    });
                }
            });
        },

        commandClick: function (command, record) {
            if (command === "SetActual") {
                window.E86n.View.Service2016.setActual(record);
            } else if (command === "SetNotActual") {
                window.E86n.View.Service2016.setNotActual(record);
            }
        },

        prepareCommand: function (command, record) {
            if (record.data.BusinessStatus === true) {
                if (command.command === "SetActual") {
                    command.hidden = true;
                    command.hideMode = 'visibility';
                }
            } else {
                if (command.command === "SetNotActual") {
                    command.hidden = true;
                    command.hideMode = 'visibility';
                }

                if (command.command === "SetActual" && record.data.IsEditable && record.data.FromPlaning === false) {
                    command.hidden = true;
                    command.hideMode = 'visibility';
                }

            }
        },

        setNotActual: function (service) {
            service.set('BusinessStatus', false);
            window.Service2016Store.save();
        },

        setActual: function (service) {
            Ext.Ajax.request({
                url: '/Service2016/CheckActuality',
                params: { regrnumber: service.data.Regrnumber },

                failure: function (response) {
                    var jo = Ext.util.JSON.decode(response.responseText);
                    Ext.Msg.show({
                        title: 'Ошибка',
                        msg: jo.message
                    });
                },

                success: function (response) {
                    var jo = Ext.util.JSON.decode(response.responseText);

                    // Если услуга неактуальна
                    if (!jo.success) {
                        Ext.Msg.show({ title: 'Ошибка', msg: jo.message });
                        return;
                    }

                    // Если не нужно исключать другую услугу
                    if (!jo.data) {
                        if (jo.message) {
                            Ext.Msg.show({
                                title: 'Информация',
                                msg: jo.message,
                                buttons: Ext.Msg.OK,
                                fn: function () {
                                    service.set('BusinessStatus', true);
                                    window.Service2016Store.save();
                                }
                            });
                        } else {
                            service.set('BusinessStatus', true);
                            window.Service2016Store.save();
                        }

                        return;
                    }

                    // Запрашиваем подтверждение на исключение другой услуги
                    Ext.Msg.show({
                        title: 'Подтверждение',
                        msg: jo.message,
                        width: 500,
                        buttons: Ext.Msg.YESNO,
                        fn: function (btn) {
                            if (btn === 'yes') {
                                Ext.Ajax.request({
                                    url: '/Service2016/ExcludeService',
                                    params: { id: jo.data },
                                    success: function (response) {
                                        var jo2 = Ext.util.JSON.decode(response.responseText);
                                        Ext.Msg.show({
                                            title: 'Информация',
                                            msg: jo2.message,
                                            buttons: Ext.Msg.OK,
                                            fn: function () {
                                                service.set('BusinessStatus', true);
                                                window.Service2016Store.save();
                                            }
                                        });
                                    },
                                    failure: function (response) {
                                        var jo2 = Ext.util.JSON.decode(response.responseText);
                                        Ext.Msg.show({
                                            title: 'Ошибка',
                                            msg: jo2.message
                                        });
                                    }
                                });
                            } else {
                                Ext.Msg.show({
                                    title: 'Информация',
                                    msg: 'Действия отменены'
                                });
                            }
                        },
                        modal: true,
                        icon: Ext.Msg.QUESTION,
                        closable: false
                    });
                }
            });
        }
    };