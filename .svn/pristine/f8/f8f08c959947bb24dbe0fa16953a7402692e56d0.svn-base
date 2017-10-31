Ext.ns('E86n.View');

function reloadDetail() {
    var activeTab = window.DetailTabPanel.getActiveTab();

    if (activeTab.id == 'Consumer') {
        window.ConsumerRemoveRecordBtn.setDisabled(true);
    }

    if (activeTab.id == 'Characteristic') {
        window.CharacteristicRemoveRecordBtn.setDisabled(true);
    }

    if (activeTab.id == 'Provider') {
        window.ProviderRemoveRecordBtn.setDisabled(true);
    }

    //Обновляем детализацию и сбрасываем выделение 
    activeTab.reload();
    activeTab.getSelectionModel().clearSelections();
};

E86n.View.Services =
    {
        Copy: function () {
            Ext.Ajax.request({
                url: '/Services/Copy',
                params: { id: Services.getSelectionModel().getSelected().id },
                failure: function () {
                    Ext.Msg.show({ title: 'Ошибка', msg: 'Не удалось скопировать', buttons: Ext.MessageBox.OK });
                    return;
                },
                success: function (result) {
                    Ext.Msg.show({ title: 'Успех', msg: 'Копирование прошло успешно', buttons: Ext.MessageBox.OK });
                    window.Services.filters.clearFilters();
                    window.Services.filters.getFilter(7).setValue(result.responseText);
                    window.Services.filters.getFilter(7).setActive(true);
                }
            });
        },

        UpdateView: function () {
            if (window.Services.getSelectionModel().hasSelection()) {
                if (Services.getSelectionModel().getSelected().id > 0) {
                    window.CopyBtn.setDisabled(false);
                    window.ServicesRemoveRecordBtn.setDisabled(false);
                    window.DetailTabPanel.setDisabled(false);
                    reloadDetail();
                } else {
                    window.DetailTabPanel.setDisabled(true);
                }
            } else {
                //Если нет выделеных элементов - детализация и кнопка уделания не активны
                window.ServicesRemoveRecordBtn.setDisabled(true);
                window.DetailTabPanel.setDisabled(true);
                window.CopyBtn.setDisabled(true);
            }
        },

        ChangePage: function () {
            //При переходе на дгурую станицу в центральном гриде - сбрасываем выделение и обновляем.
            window.Services.getSelectionModel().clearSelections();
            window.E86n.View.Services.UpdateView();
        },

        getSelectedServiceId: function () {
            var selection = window.Services.getSelectionModel();
            if (selection.hasSelection()) {
                return selection.getSelected().id;
            } else {
                return -1;
            }
        }
    };