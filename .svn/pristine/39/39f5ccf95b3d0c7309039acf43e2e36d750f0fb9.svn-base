Ext.ns("EO15AIP.Control.CommonWin");

EO15AIP.Control.CommonWin = {
    resizeFn: function (winId) {
        var win = Ext.getCmp(winId);
        var size = Ext.getBody().getSize();
        win.setSize({ width: size.width * 0.95, height: size.height * 0.95 });
        if (win.iframe != undefined) {
            win.reload();
        }
    }
};

