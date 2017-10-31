var getUrlForStatus = function (value) {
    var tpl = '<img src="{0}" ext:qtip="{1}"/>';
    if (value == 1) {
        return String.format(tpl, UrlIconStatusEdit.getValue(), UrlIconStatusEdit.invalidText);
    } else if (value == 2) {
        return String.format(tpl, UrlIconStatusReview.getValue(), UrlIconStatusReview.invalidText);
    } else if (value == 3) {
        return String.format(tpl, UrlIconStatusAcceptred.getValue(), UrlIconStatusAcceptred.invalidText);
    } else {
        return '';
    }
};

var getStatusFilters = function () {
    var filter = [true, true, true];
    filter[0] = btnFilterEditable.pressed;
    filter[1] = btnFilterReview.pressed;
    filter[2] = btnFilterAccepted.pressed;
    return filter;
};

var editCard = function (areaId, regNumber) {
    wAreaDetail.autoLoad.params.id = areaId;
    var title = 'Карточка: ';
    if (regNumber === null)
    { title += 'без номера'; }
    else
    { title += regNumber; }
    wAreaDetail.setTitle(title);
    wAreaDetail.show();
    wAreaDetail.center();
};

var DeleteProjectSelectedRow = function (gridPanelId) {
    Ext.Msg.confirm('Предупреждение', 'Удалить карточку?',
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

var ShowGoogleMap = function (gridPanelId) {
    var table = Ext.getCmp(gridPanelId);
    if (table === undefined || !table.hasSelection()) { return; }
    var areaId = table.getSelectionModel().selections.items[0].data.ID;
    wGoogleMap.autoLoad.params.id = areaId;
    wGoogleMap.show();
    wGoogleMap.center();
};