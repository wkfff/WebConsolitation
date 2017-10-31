Ext.ns('E86n.View');

E86n.View.UIBuilders = {
    appliedDocumentGridSelectRow: function (record, documentForm) {
        var isExternalFiles = !(record.data.UrlExternal === '' || record.data.UrlExternal === null || record.data.UrlExternal === 'undefined');
        documentForm.items.map.externalFiles.items.map.externalFileDownload.setText(record.data.UrlExternal);
        documentForm.items.map.externalFiles.items.map.externalFileDownload.setUrl(record.data.UrlExternal);

        documentForm.items.map.adfName.setDisabled(isExternalFiles);
        documentForm.items.map.adfRefTypeDoc_Name.setDisabled(isExternalFiles);
        documentForm.items.map.adfDocDate.setDisabled(isExternalFiles);

        documentForm.items.map.externalFiles.setVisible(isExternalFiles);
        documentForm.items.map.internalFiles.setVisible(!isExternalFiles);
    }
};