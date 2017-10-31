Ext.ns('E86n.View.StartDoc');

//получение значений для фильтра
var getFilter = function (fld) {
    var val;
    switch (fld) {
    case 'PPO':
        val = window.RefOrgPPOFlt.getValue();
        if (val != '') {
            return val;
        } else {
            return -1;
        }
    case 'GRBS':
        val = window.RefOrgGRBSFlt.getValue();
        if (val != '') {
            return val;
        } else {
            return -1;
        }
    case 'YF':
        val = window.cbYearFormation.getValue();
        if (val != '') {
            return val;
        } else {
            return -1;
        }
    default:
        return -1;
    }
};

var getMasterID = function (gridName) {
    var grid = window.Ext.getCmp(gridName);
    if (grid != null && grid.getSelectionModel().hasSelection()) {
        var row = grid.getSelectionModel().getSelected();
        return row.data.ID;
    } else {
        return -1;
    }
};

//возвращает картинку для состояния по коду
var getUrlForStatus = function (value, record) {
    var tpl;
    if (!!record.data.CloseDate) {
        tpl = '<img src="{0}" ext:qtip="{1}"/><img src="{2}" ext:qtip="{3}"/>';
        switch (value) {
        case 1:
            return String.format(tpl, window.UrlIconStatusFix.getValue(), window.UrlIconStatusFix.invalidText, window.UrlIconStatusClosed.getValue(), window.UrlIconStatusClosed.invalidText);
        case 2:
            return String.format(tpl, window.UrlIconStatusAdd.getValue(), window.UrlIconStatusAdd.invalidText, window.UrlIconStatusClosed.getValue(), window.UrlIconStatusClosed.invalidText);
        case 3:
            return String.format(tpl, window.UrlIconStatusEdit.getValue(), window.UrlIconStatusEdit.invalidText, window.UrlIconStatusClosed.getValue(), window.UrlIconStatusClosed.invalidText);
        case 4:
            return String.format(tpl, window.UrlIconStatusRasm.getValue(), window.UrlIconStatusRasm.invalidText, window.UrlIconStatusClosed.getValue(), window.UrlIconStatusClosed.invalidText);
        case 5:
            return String.format(tpl, window.UrlIconStatusReW.getValue(), window.UrlIconStatusReW.invalidText, window.UrlIconStatusClosed.getValue(), window.UrlIconStatusClosed.invalidText);
        case 6:
            return String.format(tpl, window.UrlIconStatusUse.getValue(), window.UrlIconStatusUse.invalidText, window.UrlIconStatusClosed.getValue(), window.UrlIconStatusClosed.invalidText);
        case 7:
            return String.format(tpl, window.UrlIconStatusEnd.getValue(), window.UrlIconStatusEnd.invalidText, window.UrlIconStatusClosed.getValue(), window.UrlIconStatusClosed.invalidText);
        case 8:
            return String.format(tpl, window.UrlIconStatusExported.getValue(), window.UrlIconStatusExported.invalidText, window.UrlIconStatusClosed.getValue(), window.UrlIconStatusClosed.invalidText);
        default:
            return String.format(tpl, window.UrlIconStatusNot.getValue(), window.UrlIconStatusNot.invalidText, window.UrlIconStatusClosed.getValue(), window.UrlIconStatusClosed.invalidText);
        }
    } else {
        tpl = '<img src="{0}" ext:qtip="{1}"/>';
        switch (value) {
        case 1:
            return String.format(tpl, window.UrlIconStatusFix.getValue(), window.UrlIconStatusFix.invalidText);
        case 2:
            return String.format(tpl, window.UrlIconStatusAdd.getValue(), window.UrlIconStatusAdd.invalidText);
        case 3:
            return String.format(tpl, window.UrlIconStatusEdit.getValue(), window.UrlIconStatusEdit.invalidText);
        case 4:
            return String.format(tpl, window.UrlIconStatusRasm.getValue(), window.UrlIconStatusRasm.invalidText);
        case 5:
            return String.format(tpl, window.UrlIconStatusReW.getValue(), window.UrlIconStatusReW.invalidText);
        case 6:
            return String.format(tpl, window.UrlIconStatusUse.getValue(), window.UrlIconStatusUse.invalidText);
        case 7:
            return String.format(tpl, window.UrlIconStatusEnd.getValue(), window.UrlIconStatusEnd.invalidText);
        case 8:
            return String.format(tpl, window.UrlIconStatusExported.getValue(), window.UrlIconStatusExported.invalidText);
        default:
            return String.format(tpl, window.UrlIconStatusNot.getValue(), window.UrlIconStatusNot.invalidText);
        }
    }
};

//обработка команд
var cmdHandler = function (cmd, record) {
    switch (cmd) {
    case "EditDoc":
        {
            parent.MdiTab.addTab({
                title: record.data.RefPartDocName,
                url: record.data.Url + "?docId=" + record.data.ID,
                icon: "icon-report"
            });
        }
        break;
    }
};

E86n.View.StartDoc.Grid =
    {
        ChangeFilter: function () {
            if (window.StartDocInstitutionsGrid != undefined) {
                window.StartDocInstitutionsGrid.getSelectionModel().clearSelections();
                window.StartDocInstitutionsStore.reload();
            }
            window.E86n.View.StartDoc.Grid.Update();
        },

        ChangePage: function () {
            window.StartDocInstitutionsGrid.getSelectionModel().clearSelections();
            window.E86n.View.StartDoc.Grid.Update();
        },

        // Вызывается при выделении учреждения или изменении фильра по году формирования
        Update: function () {
            if (window.StartDocInstitutionsGrid != undefined) {
                if (window.StartDocInstitutionsGrid.getSelectionModel().hasSelection()) {
                    window.StartDocDocumentGrid.setDisabled(false);
                    window.StartDocDocumentGrid.getSelectionModel().clearSelections();
                    window.StartDocDocumentStore.reload();
                } else {
                    window.StartDocDocumentGrid.setDisabled(true);
                }
            } else {
                window.StartDocDocumentGrid.getSelectionModel().clearSelections();
                window.StartDocDocumentStore.reload();
            }
        },

        SetEditbleNote: function (isGrbs) {
            if (window.DetailDocs.getSelectionModel().getSelected().data.RefSost == 4 && isGrbs) {
                return true;
            }
            return false;
        },

        prepareToolbar: function (grid, toolbar, rowIndex, record) {
            if (record.dirty) {
                toolbar.setDisabled(true);
            } else {
                toolbar.setDisabled(false);
            }
        },

        prepareCommand: function (grid, toolbar, rowIndex, record) {
            if (toolbar.items.items[0].command == 'CloseDoc' && record.data.CloseDate != null) {
                toolbar.setDisabled(true);
            } else {
                toolbar.setDisabled(false);
            }
        },

        RowSelectDocs: function () {
            if (Ext.getCmp('StartDocDocumentGridRemoveRecordBtn') != undefined) {
                window.StartDocDocumentGridRemoveRecordBtn.setDisabled(false);
            }
        },

        RowDeselectDocs: function () {
            if (!window.StartDocDocumentGrid.getSelectionModel().hasSelection() && Ext.getCmp('StartDocDocumentGridRemoveRecordBtn') != undefined) {
                window.StartDocDocumentGridRemoveRecordBtn.setDisabled(true);
            }
        },

        getSelectedFormationDate: function () {
            return window.StartDocDocumentGrid.getSelectionModel().getSelected().get('FormationDate');
        }
    };