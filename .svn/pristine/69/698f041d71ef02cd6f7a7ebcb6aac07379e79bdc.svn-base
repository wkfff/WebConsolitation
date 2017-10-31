Extension.entityBook = {
    selectedRecord: {},
    onRowSelect: function (record) {
        window.parent.btnOk.enable();
    },
    lookup: {
        primary: [],
        secondary: []
    },
    getLookupValue: function () {
        var result = '';
        for (var i = 0; i < this.lookup.primary.length; i++) {
            var curValue = this.selectedRecord.data[this.lookup.primary[i]];
            if (curValue != null)
                result += curValue + '; ';
        }
        return result;
    }

};

var onGridRowSelect = function (selModel, rowIndex, x, r) {
    Extension.entityBook.selectedRecord = r;
    Extension.entityBook.onRowSelect(r);
}