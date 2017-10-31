var getUrlForStatus = function (value) {
    var tpl = '<img src="{0}" ext:qtip="{1}"/>';
    if (value == 1) {
        return String.format(tpl, UrlIconStatusUnapproved.getValue(), UrlIconStatusUnapproved.invalidText);
    } else if (value == 2) {
        return String.format(tpl, UrlIconStatusApproved.getValue(), UrlIconStatusApproved.invalidText);
    } else if (value == 3) {
        return String.format(tpl, UrlIconStatusRunning.getValue(), UrlIconStatusRunning.invalidText);
    } else if (value == 4) {
        return String.format(tpl, UrlIconStatusFinished.getValue(), UrlIconStatusFinished.invalidText);
    } else {
        return '';
    }
};

var getFilters = function () {
    var filter = [true, true, true];
    filter[0] = btnFilterTypeDCP.pressed;
    filter[1] = btnFilterTypeVCP.pressed;
    filter[2] = btnFilterUnapproved.pressed;
    filter[3] = btnFilterApproved.pressed;
    filter[4] = btnFilterRunning.pressed;
    filter[5] = btnFilterFinished.pressed;
    return filter;
};

var openProgram = function (programId, programName) {
    wProgram.autoLoad.params.id = programId;
    var title = 'Программа: ';
    if (programName === null)
    { title += 'без имени'; }
    else
    { title += programName; }
    wProgram.setTitle(title);
    wProgram.show();
    wProgram.center();
};

var deleteProgramSelectedRow = function (gridPanelId) {
    Ext.Msg.confirm('Предупреждение', 'Удалить программу?',
          function (btn) {
              if (btn == 'yes') {
                  var table = Ext.getCmp(gridPanelId);
                  table.deleteSelected();
                  table.store.save();
                  RowSelectionModel1.fireEvent('RowDeselect');
              }
          }
    );
};

var openReport = function (window, programId, title, programName) {
    
    window.setTitle(title + ' (' + programName.slice(0,100) + ')' );
    window.autoLoad.params.programId = programId;
    window.show();
    window.center();
};