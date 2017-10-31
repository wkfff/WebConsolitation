_grid: null;
Controller = function () {

    function createGrid() {

        // create the data store
        var record = Ext.data.Record.create([
                    { name: 'ID', type: 'int' },
                    { name: 'NAME' },
                    { name: 'REFENTITIES', type: 'auto' }
                 ]);
    
        var store = new Ext.ux.maximgb.tg.AdjacencyListStore({
            autoLoad: true,
            parent_id_field_name: 'REFENTITIES',
            url: '/Entity/Data?objectkey=3d6959a5-3ce6-439d-a5f1-b17aeeedfa37&limit=30&start=0&dir=ASC&sort=ID&serverfilter=""&filters=0',
            reader: new Ext.data.JsonReader(
                        { id: 'ID',
                            root: 'data.Rows',
                            totalProperty: 'totalCount',
                            successProperty: 'success'
                        },
                        record
                    )
        });

        /*Data?objectkey=3d6959a5-3ce6-439d-a5f1-b17aeeedfa37&limit=30&start=0&dir=ASC&sort=ID&serverfilter=""&filters=0',*/

        // create the Grid
        var grid = new Ext.ux.maximgb.tg.GridPanel({
            store: store,
            master_column_id: 'NAME',
            columns: [
                        { header: "ID", width: 50, sortable: true, dataIndex: 'ID', hidden: true },
                        { id: 'NAME', header: "Наименование", width: 100, sortable: true, hideable: false, dataIndex: 'NAME' }
                    ],
            stripeRows: true,
            autoExpandColumn: 'NAME',
            root_title: 'Все классификаторы',
            border: false,
            tbar: new Ext.ux.maximgb.tg.PagingToolbar({
                store: store,
                displayInfo: true,
                pageSize: 10,
                header: false,
                border: false
            })
        });
        var vp = new Ext.Viewport({
            layout: 'fit',
            border: false,
            items: [grid]
        });
        grid.getSelectionModel().selectFirstRow();
        _grid = grid;
        grid.on({
            cellmousedown: mousedownHandler
        })
    }

    var mousedownHandler = function (g, ri, ci, e) {
        var r = g.getStore().getAt(ri);
        var fName = g.getColumnModel().getDataIndex(ci);
        if (fName == "LockedUserName") {
            var t = e.getTarget();
            if (t.className == "controlBtn") {
                parent.MdiTab.addTab({ title: r.data.HEADLINE + ' (' + r.data.ID + ')', url: "/Entity/Show/" + r.data.ID, icon: "icon-report" });
            }
        }
    }

    var commandHandler = function (grid, rowIndex, columnIndex, e) {
        var record = grid.getStore().getAt(rowIndex);
        var fieldName = grid.getColumnModel().getDataIndex(columnIndex);
        var target = e.getTarget();
        switch (cmd) {
            case "Go":
                parent.MdiTab.addTab({ title: record.data.Semantic + "." + record.data.Caption, url: "/Entity/Show?objectKey=" + record.data.ObjectKey, icon: "icon-report" });
                break;
        }
    }

    return {
        init: function () {
            Ext.BLANK_IMAGE_URL = '';
            createGrid();
        }
    }
} ();

Ext.onReady(Controller.init);