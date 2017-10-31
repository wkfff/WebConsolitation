function OnSelectCriterias(combo, record, index) {
    var selectedItem = gpEstimate.getSelectionModel().getSelections()[0];
    var store = selectedItem.store;
    var row = store.getById(selectedItem.get('ID'));
    row.set("SelectedId", record.get('ID'));
};