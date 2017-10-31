var openProjectTab = function (projId, refPart, name) {
    var tabPanel = parent.ProjectsTabPanel;
    var id = 'ProjectDetail_' + projId;
    var tab = tabPanel.getComponent(id);
    if (!tab) {
        tab = parent.ProjectsTabPanel.addTab({ id: id,
            title: 'Карта инвестиционного проекта - ' + name,
            closable: true,
            autoLoad: {
                showMask: true,
                url: '/View/InvProjProjectDetail' + '?projId=' + projId + '&refPart=' + refPart,
                mode: 'iframe',
                maskMsg: 'Загрузка формы...',
                noCache: true,
                scripts: true,
                passParentSize: true
            }
        });
    }
    else {
        tabPanel.setActiveTab(tab);
    }
};

var getUrlForStatus = function (value) {
    var tpl = '<img src="{0}" ext:qtip="{1}"/>';
    if (value == 1) {
        return String.format(tpl, UrlIconStatusEdit.getValue(), UrlIconStatusEdit.invalidText);
    } else if (value == 2) {
        return String.format(tpl, UrlIconStatusExecut.getValue(), UrlIconStatusExecut.invalidText);
    } else if (value == 3) {
        return String.format(tpl, UrlIconStatusExclude.getValue(), UrlIconStatusExclude.invalidText);
    } else {
        return '';
    }
};

var getStatusFilters = function () {
    var filter = [true, true, true];
    filter[0] = btnFilterEditable.pressed;
    filter[1] = btnFilterExecutable.pressed;
    filter[2] = btnFilterExcluded.pressed;
    return filter;
};

var DeleteProjectSelectedRow = function (gridPanelId) {
    Ext.Msg.confirm('Предупреждение', 'Удалить проект?',
      function (btn) {
          if (btn == 'yes') {
              var table = Ext.getCmp(gridPanelId);
              table.deleteSelected();
              table.store.save();
              if (!table.hasSelection()) {
                  btnDelete.disable();
                  if (Ext.getCmp('btnMoveToProposedProjects') != undefined) {
                      btnMoveToProposedProjects.disable();
                  }
              }
          }
      }
    );
};

var ChangeProjectPartSelectedRow = function (gridPanelId) {
    Ext.Msg.confirm('Предупреждение', 'Перенести в другой раздел?',
      function (btn) {
          if (btn == 'yes'){
              var table = Ext.getCmp(gridPanelId);
              Ext.net.DirectMethod.request({
                  url: '/ProjectsList/ChangePart',
                  cleanRequest: true,
                  params: { projId: table.getSelectionModel().getSelected().data.ID },
                  success: successSaveHandler,
                  failure: failureSaveHandler
              });
              if (!table.hasSelection()) {
                  btnDelete.disable();
                  btnMoveToProposedProjects.disable();
              }
          }
      }
    );
};

var successSaveHandler = function (response, result) {
    if (result.extraParams != undefined && result.extraParams.msg != undefined) {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: result.extraParams.msg,
            title: 'Уведомление',
            hideDelay: 2500
        });
    } else if (response.extraParams != undefined && response.extraParams.msg != undefined) {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: response.extraParams.msg,
            title: 'Уведомление',
            hideDelay: 2500
        });

    } else {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: 'Ок',
            title: 'Уведомление',
            hideDelay: 2500
        });
    }
};

var failureSaveHandler = function (response, result) {
    if (result.extraParams != undefined && result.extraParams.responseText != undefined) {
        Ext.Msg.alert('Ошибка', result.extraParams.responseText);
    } else {
        var responseParams = Ext.decode(result.responseText);
        if (responseParams != undefined && responseParams.extraParams != undefined && responseParams.extraParams.responseText != undefined) {
            Ext.Msg.alert('Ошибка', responseParams.extraParams.responseText);
        } else {
            Ext.Msg.alert('Ошибка', 'Server failed');
        }
    }
};