_grid : null;
Controller = function () {

    function createGrid() {

        var expander = new Ext.grid.RowExpander({
            tpl: new Ext.Template(
              '<p><b>Задание:</b> {JOB}</p><br>',
              '<p><b>Описание:</b> {DESCRIPTION}</p><br>'
          )
        });

        // create the data store
        var record = Ext.data.Record.create([
     	    { name: 'ID', type: 'int' },
   		    { name: 'HEADLINE' },
     	    { name: 'STATE' },
     	    { name: 'FROMDATE', type: 'date', dateFormat: 'd.m.Y h:m:s' },
     	    { name: 'TODATE', type: 'date', dateFormat: 'd.m.Y h:m:s' },
     	    { name: 'OWNERNAME' },
     	    { name: 'DOERNAME' },
     	    { name: 'CURATORNAME' },
     	    { name: 'LOCKEDUSERNAME' },
   		    { name: 'JOB' },
   		    { name: 'DESCRIPTION' },
     	    { name: 'REFTASKS', type: 'auto' },
     	    { name: '_is_leaf', type: 'bool' }
   	    ]);
        var store = new Ext.ux.maximgb.tg.AdjacencyListStore({
            autoLoad: true,
            parent_id_field_name: 'REFTASKS',
            url: '/TaskNav/Data',
            reader: new Ext.data.JsonReader(
				    {
				        id: 'ID',
				        root: 'data.Rows',
				        totalProperty: 'totalCount',
				        successProperty: 'success'
				    },
				    record
			    )
        });
        // create the Grid
        var grid = new Ext.ux.maximgb.tg.GridPanel({
            store: store,
            master_column_id: 'HEADLINE',

            columns: [
                expander,
                { header: "", width: 44, sortable: false, dataIndex: 'LOCKEDUSERNAME', hideable: false,
                    renderer: function (v, p, r, rowIndex, colIndex, ds) {
                        var s = '<img title="Открыть задачу" src="/icons/play_green-png/ext.axd" width="16" height="16" class="controlBtn">';
                        if (v != null) {
                            if (_grid.store.reader.jsonData.extraParams.toUpperCase() != v.toUpperCase()) {
                                s += '<img title="Задача заблокирована пользователем \'' + v + '\'" src="/extjs/resources/images/default/grid/hmenu-lock-png/ext.axd" width="16" height="16">';
                            } else {
                                s += '<img title="Задача заблокирована Вами" src="/icons/pencil-png/ext.axd" width="16" height="16">';
                            }
                        }
                        return s;
                    }
                },
                { header: "ID", width: 50, sortable: true, dataIndex: 'ID', hidden: true },
                { id: 'HEADLINE', header: "Наименование", width: 500, sortable: true, hideable: false, dataIndex: 'HEADLINE' },
                { header: "Состояние", width: 85, sortable: true, dataIndex: 'STATE' },
                { header: "Дата начала", width: 85, sortable: true, dataIndex: 'FROMDATE',
                    renderer: Ext.util.Format.dateRenderer('d.m.Y')
                },
                { header: "Дата окончания", width: 90, sortable: true, dataIndex: 'TODATE',
                    renderer: Ext.util.Format.dateRenderer('d.m.Y')
                },
                { header: "Владелец", width: 120, sortable: true, dataIndex: 'OWNERNAME' },
                { header: "Исполнитель", width: 120, sortable: true, dataIndex: 'DOERNAME' },
                { header: "Куратор", width: 120, sortable: true, dataIndex: 'CURATORNAME' }
            ],
            stripeRows: true,
            autoExpandColumn: 'HEADLINE',
            root_title: 'Все задачи',
            border: false,
            plugins: expander,
            tbar: new Ext.ux.maximgb.tg.PagingToolbar({
                store: store,
                displayInfo: true,
                pageSize: 20,
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
            cellmousedown: mousedownHandler,
            resize: resizeHandler
        })

    }

    var mousedownHandler = function (g, ri, ci, e) {
        var r = g.getStore().getAt(ri);
        var fName = g.getColumnModel().getDataIndex(ci);
        if (fName == "LOCKEDUSERNAME") {
            var t = e.getTarget();
            if (t.className == "controlBtn") {
                parent.MdiTab.addTab({ title: r.data.HEADLINE + ' (' + r.data.ID + ')', url: "/Task/Show/" + r.data.ID, icon: "icon-report" });
            }
        }
    }

    // Вычисление количества строк на странице в зависимости от размера грида
    var resizeHandler = function applyPageSize(grid, adjWidth, adjHeight, rawWidth, rawHeight) {
        var headerHeight = grid.el.child(".x-grid3-header").getHeight(); //header height;
        var rowHeight = Ext.fly(grid.getView().getRow(0)).getHeight(); //row height;
        var contentHeight = Math.max(rowHeight + headerHeight,
                    grid.getInnerHeight()) - headerHeight;

        var maxRowsPerGrid = Math.floor(contentHeight / rowHeight);
        var tbar = grid.getTopToolbar();
        if (tbar.pageSize != maxRowsPerGrid) {
            tbar.pageSize = maxRowsPerGrid;

            var newPage = 1;

            if (grid.getSelectionModel().hasSelection()) {
                var record = grid.getSelectionModel().getSelected(),
                        indexOnPage = grid.getStore().indexOf(record),
                        index = bbar.cursor + indexOnPage;
                newPage = Math.ceil((index + 1) / bbar.pageSize);
            }

            tbar.changePage(newPage);
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