ViewPersistence.isDirty = function () {
    if (this.dsMarks.isDirty()) {
        return true;
    }
    for (p in this) {
        if (p.substring(0, 9) == 'StoreRow_') {
            var store = this[p];
            if (store.isDirty()) {
                return true;
            }
        }
    }
    return false;
};

ViewPersistence.refresh = function () {
    this.regionsCombo.store.reload();
};

function beforeEditCellReadonly(e) {
    return false;
}

function saveData() {
    var func = function (item) {
        try {
            var store = eval('StoreRow_' + item.id);
            store.data.each(func);
            store.save();
        } catch (e) {
        }
    };
    dsMarks.data.each(func);
    dsMarks.save();
};

function acceptButtonClick() {
    var arr = [];
    var isDirty = false;
    var addSelectedRowsFromGrid = function (grid) {
        var s = grid.getSelectionModel().getSelections();
        for (i = 0; i < s.length; i++) {
            if (s[i].get('RefStatusData') == 2) {
                arr.push(s[i]);
            }
            isDirty = isDirty || s[i].dirty;
        }
    };

    addSelectedRowsFromGrid(gpMarks);

    var recursiveFunc = function (item) {
        try {
            var childGrid = eval('GridPanelRow_' + item.id);
            addSelectedRowsFromGrid(childGrid);
            childGrid.store.data.each(recursiveFunc);
        } catch (e) {
        }
    };

    gpMarks.store.data.each(recursiveFunc);

    if (isDirty) {
        // Спросить и Сохранить измененные данные
    }

    for (i = 0; i < arr.length; i++) {
        arr[i].set('RefStatusData', 3);
    }
    saveData();
}

function rejectButtonClick() {
    var arr = [];
    var isDirty = false;
    var addSelectedRowsFromGrid = function (grid) {
        var s = grid.getSelectionModel().getSelections();
        for (i = 0; i < s.length; i++) {
            if (s[i].get('RefStatusData') == 2 || s[i].get('RefStatusData') == 3) {
                arr.push(s[i]);
            }
            isDirty = isDirty || s[i].dirty;
        }
    }

    addSelectedRowsFromGrid(gpMarks);
    
    var recursiveFunc = function (item) {
        try {
            var childGrid = eval('GridPanelRow_' + item.id);
            addSelectedRowsFromGrid(childGrid);
            childGrid.store.data.each(recursiveFunc);
        } catch (e) {
        }
    };

    gpMarks.store.data.each(recursiveFunc);

    if (isDirty) {
        // Спросить и Сохранить измененные данные
    }

    for (i = 0; i < arr.length; i++) {
        arr[i].set('RefStatusData', 1);
    }
    saveData();
}

function toggleFilter(button, state) {
    if (button.getId() == 'calculatePreviousResultFilter') {
        if (state) {
            gpMarks.getColumnModel().setHidden(7, false);
            dsMarks.load();
        } else {
            var func = function (item) {
                try {
                    var childGrid = eval('GridPanelRow_' + item.id);
                    childGrid.store.data.each(func);
                    childGrid.getColumnModel().setHidden(7, true);
                } catch (e) {
                }
            };
            gpMarks.store.data.each(func);
            gpMarks.getColumnModel().setHidden(7, true);
        }
    } else {
        if (button.getId() == 'showHierarhy') {
            if (state) {
                dsMarks.addListener('load', function () { gpMarks.view.collapseAllGroups(); }, this, { single: true });
            } else {
                dsMarks.addListener('load', function () { gpMarks.view.expandAllGroups(); }, this, { single: true });
            }
        }        
        dsMarks.load();
    }
}

function getStateFilter() {
    var filter = [true, true, true, true];

    filter[0] = filterEdit.pressed;
    filter[1] = filterMagnify.pressed;
    filter[2] = filterTick.pressed;
    filter[3] = filterAccept.pressed;

    return filter;
}

function showCellTip() {
    var rowIndex = gpMarks.view.findRowIndex(this.triggerElement),
        cellIndex = gpMarks.view.findCellIndex(this.triggerElement),
        record = dsMarks.getAt(rowIndex);

    if (!record) {
        this.hide();
        return false;
    }

    var fieldName = gpMarks.getColumnModel().getDataIndex(cellIndex),
        data = record.get(fieldName);

    if (cellIndex == 2) {
        if (data == 1) {
            data = 'На редактировании';
        } else if (data == 2) {
            data = 'На рассмотрении';
        } else if (data == 3) {
            data = 'Утвержден';
        } else if (data == 4) {
            data = 'Принят';
        }

        this.body.dom.innerHTML = data;
    }
    else {
        this.hide();
        return false;
    }
}