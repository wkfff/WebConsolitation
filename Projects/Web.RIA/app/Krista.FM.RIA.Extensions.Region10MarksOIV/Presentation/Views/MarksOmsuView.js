ViewPersistence.refresh = function () {
    this.dsMarks.reload();
};

ViewPersistence.isDirty = function () {
    if (this.dsMarks.isDirty()) {
        return true;
    }
    return false;
};

function saveData() {
    dsMarks.save();
};

function processSaveQuestion(button) {
    if (button == 'yes') {
        //меняем статус у измененных строк
        var records = dsMarks.getModifiedRecords();
        for (var i = 0; i < records.length; i++) {
            var rec = records[i];
            if (rec.get('RefStatusData') == 1) {
                rec.set('RefStatusData', 2);
            }
        }
    }
    saveData();
};

function saveButtonClick() {
    Ext.Msg.show({
        title: 'Сохранить данные',
        msg: 'Отправить показатели на рассмотрение?',
        buttons: {yes:'Да, сохранить и отправить.', no:'Нет, сохранить.'},
        fn: processSaveQuestion,
        icon: Ext.MessageBox.QUESTION
    });
    Ext.MessageBox.getDialog().defaultButton = 2;
    Ext.MessageBox.getDialog().focus();
};

function toTestButtonClick() {
    var arr = [];
    var isDirty = false;
    var func = function(grid) {
        var s = grid.getSelectionModel().getSelections();
        for (i = 0; i < s.length; i++) {
            if (s[i].get('RefStatusData') == 1) {
                arr.push(s[i]);
            }
            isDirty = isDirty || s[i].dirty;
        }
    };

    func(gpMarks);

    if (isDirty) {
        // Спросить и Сохранить измененные данные
    }

    for (i = 0; i < arr.length; i++) {
        arr[i].set('RefStatusData', 2);
    }
    
    saveData();
};

function toggleFilter(button, state) {
    dsMarks.load();    
};

function getStateFilter() {
    var filter = [true, true];

    filter[0] = filterEdit.pressed;
    filter[1] = filterMagnify.pressed;
    
    return filter;
};
