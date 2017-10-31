Ext.ns("EO15AIP.View.Finance");

EO15AIP.View.Finance.Grid = {
    rendererFn: function (v, p, r, rowIndex, colIndex, ds) {
        if (!r.data.Editable) {
            p.css = 'gray-cell';
        }
        var f = Ext.util.Format.numberRenderer(',00/i');
        return f(v);
    }
};