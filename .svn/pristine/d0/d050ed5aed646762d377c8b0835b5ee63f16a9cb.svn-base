/**
* @author chander, adapted by SIDGEY
*/
////////////////////////////////////////////////////////////////////////////////////////////////////////////
//------------------------------------MULTI GROUPING STORE ----------------------------------------------------------------
////////////////////////////////////////////////////////////////////////////////////////////////////////////
Ext.ux.MultiGroupingStore = Ext.extend(Ext.net.Store, {
    sortInfo: [],

    constructor: function (config) { Ext.ux.MultiGroupingStore.superclass.constructor.apply(this, arguments); },
    groupField: false,
    //groupField: [],
    //groupField: ['Year', 'TemplateGroup', 'TemplateName'],
    sort: function (field, dir) {
        if (field === undefined) {
            return true;
        }
        var f = [];
        if (Ext.isArray(field)) { for (var i = 0, len = field.length; i < len; ++i) { f.push(this.fields.get(field[i].field)); } }
        else { f.push(this.fields.get(field)); }
        if (f.length < 1) { return false; }
        if (!dir) {
            if (this.sortInfo && this.sortInfo.length > 0 && this.sortInfo[0].field == f[0].name) {
                // toggle sort direction
                dir = (this.sortToggle[f[0].name] || "ASC").toggle("ASC", "DESC");
            }
            else
                dir = f[0].sortDir;
        }
        var st = (this.sortToggle) ? this.sortToggle[f[0].name] : null;
        var si = (this.sortInfo) ? this.sortInfo : null;
        this.sortToggle[f[0].name] = dir;
        this.sortInfo = [];
        for (i = 0, len = f.length; i < len; ++i) {
            this.sortInfo.push({ field: f[i].name, direction: dir });
        }
        if (!this.remoteSort) {
            this.applySort();
            this.fireEvent("datachanged", this);
        }
        else {
            if (!this.load(this.lastOptions)) {
                if (st) { this.sortToggle[f[0].name] = st; }
                if (si) { this.sortInfo = si; }
            }
        }
        return true;
    },
    setDefaultSort: function (field, dir) {
        dir = dir ? dir.toUpperCase() : "ASC";
        this.sortInfo = [];
        if (!Ext.isArray(field)) { this.sortInfo.push({ field: field, direction: dir }); }
        else {
            for (var i = 0, len = field.length; i < len; ++i) {
                this.sortInfo.push({ field: field[i].field, direction: dir });
                this.sortToggle[field[i]] = dir;
            }
        }
    },
    groupBy: function (field, forceRegroup, replace) {
        if (!forceRegroup && this.groupField == field)
            return; // already grouped by this field
        if (this.groupField && !replace) {
            for (var z = 0; z < this.groupField.length; z++) {
                if (field == this.groupField[z])
                    return;
            }
            this.groupField.push(field);
        } else {
            if (replace) {
                this.groupField = field;
            } else {
                this.groupField = [field];
            }
        }
        if (this.remoteGroup) {
            if (!this.baseParams) { this.baseParams = {}; }
            this.baseParams['groupBy'] = field;
        }
        if (this.groupOnSort) { this.sort(field); return; }
        if (this.remoteGroup) { this.reload(); }
        else {
            var si = this.sortInfo || [];
            if (si.field != field) { this.applySort(); }
            else { this.sortData(field); }
            this.fireEvent('datachanged', this);
        }
    },
    applySort: function () {
        var si = this.sortInfo;
        if (si && si.length > 0 && !this.remoteSort) { this.sortData(si, si[0].direction); }
        if (!this.groupOnSort && !this.remoteGroup) {
            var gs = this.getGroupState();
            if (gs && gs != this.sortInfo) {
                this.sortData(this.groupField);
            }
        }
    },
    getGroupState: function () {
        return this.groupOnSort && this.groupField !== false ? (this.sortInfo ? this.sortInfo : undefined) : this.groupField;
    },
    sortData: function (flist, direction) {
        direction = direction || 'ASC';
        var st = [];
        var o;
        for (var i = 0, len = flist.length; i < len; ++i) {
            o = flist[i];
            var field = this.fields.get(o.field ? o.field : o);
            if (field) {
                st.push(field.sortType);
            }
        }
        var fn = function (r1, r2) {
            var v1 = [];
            var v2 = [];
            var len = flist.length;
            var o;
            var name;
            for (i = 0; i < len; ++i) {
                o = flist[i];
                name = o.field ? o.field : o;
                v1.push(st[i](r1.data[name]));
                v2.push(st[i](r2.data[name]));
            }
            var result;
            for (i = 0; i < len; ++i) {
                result = v1[i] > v2[i] ? 1 : (v1[i] < v2[i] ? -1 : 0);
                if (result !== 0) { return result; }
            }
            return result; // if it gets here, that means all fields are equal
        };
        this.data.sort(direction, fn);
        if (this.snapshot && this.snapshot != this.data) { this.snapshot.sort(direction, fn); }
    }
});

Ext.ux.MultiGroupingView = Ext.extend(Ext.grid.GroupingView, {
    constructor: function (config) {
        Ext.ux.MultiGroupingView.superclass.constructor.apply(this, arguments);
        // Added so we can clear cached rows each time the view is refreshed
        this.on("beforerefresh", function () {
            if (this.rowsCache)
                delete rowsCache;
        }, this);
    },
    /* updated version of the updateGroupWidths from GroupingView:
    * improve performance by keeping value of this.el.dom.offsetWidth
    * in member of this class.
    */
    offsetWidth: 0,
    updateGroupWidths: function () {
        if (!this.enableGrouping || !this.hasRows()) {
            return;
        }
        if (!this.offsetWidth)
            this.offsetWidth = this.el.dom.offsetWidth;
        var tw = Math.max(this.cm.getTotalWidth(),
	    this.offsetWidth - this.getScrollOffset()) + 'px';
        var gs = this.getGroups();
        for (var i = 0, len = gs.length; i < len; i++) {
            gs[i].firstChild.style.width = tw;
        }
    },
    /* updated version of GridView::syncHeaderScroll */
    headerScrollSynced: false,
    syncHeaderScroll: function () {
        if (this.headerScrollSynced)
            return;
        this.headerScrollSynced = true;
        var mb = this.scroller.dom;
        this.innerHd.scrollLeft = mb.scrollLeft;
        this.innerHd.scrollLeft = mb.scrollLeft; // second time for IE (1/2 time first fails, other browsers ignore)
    },

    get_column_by_id: function (id) {
        for (i in this.cm.lookup) {
            if (this.cm.lookup[i].dataIndex == id)
                return this.cm.lookup[i];
        }
        return null;
    },
    getGroups: function () {
        return Ext.DomQuery.select("div.x-grid-group", this.mainBody.dom);
    },

    displayEmptyFields: false,

    displayFieldSeperator: ', ', 
    
    renderRows: function () {
        var groupField = this.getGroupField();
        var eg = !!groupField;
        // if they turned off grouping and the last grouped field is hidden
        if (this.hideGroupedColumn) {
            var colIndexes = [];
            for (var i = 0, len = groupField.length; i < len; ++i) {
                var cidx = this.cm.findColumnIndex(groupField[i]);
                if (cidx >= 0) { colIndexes.push(cidx); }
            }
            if (!eg && this.lastGroupField !== undefined) {
                this.mainBody.update('');
                for (var i = 0, len = this.lastGroupField.length; i < len; ++i) {
                    var cidx = this.cm.findColumnIndex(this.lastGroupField[i]);
                    if (cidx >= 0) { this.cm.setHidden(cidx, false); }
                }
                delete this.lastGroupField;
                delete this.lgflen;
            }
            else if (eg && colIndexes.length > 0 && this.lastGroupField === undefined) {
                this.lastGroupField = groupField;
                this.lgflen = groupField.length;
                for (var i = 0, len = colIndexes.length; i < len; ++i)
                    this.cm.setHidden(colIndexes[i], true);
            }
            else if (eg && this.lastGroupField !== undefined &&
                (groupField !== this.lastGroupField || this.lgflen != this.lastGroupField.length)) {
                this.mainBody.update('');
                for (var i = 0, len = this.lastGroupField.length; i < len; ++i) {
                    var cidx = this.cm.findColumnIndex(this.lastGroupField[i]);
                    if (cidx >= 0) { this.cm.setHidden(cidx, false); }
                }
                this.lastGroupField = groupField;
                this.lgflen = groupField.length;
                for (var i = 0, len = colIndexes.length; i < len; ++i) {
                    this.cm.setHidden(colIndexes[i], true);
                }
            }
        }
        return Ext.ux.MultiGroupingView.superclass.renderRows.apply(this, arguments);
    },

    // this collection keeps a fast reference for group memberships for each record
    // it is not kept in the record since it would be destroyed during edit
    // it is used to do a fast update of summaries
    record_memberships: [],
    
    /** This sets up the toolbar for the grid based on what is grouped
    * It also iterates over all the rows and figures out where each group should appeaer
    * The store at this point is already stored based on the groups.
    */
    doRender: function (cs, rs, ds, startRow, colCount, stripe) {
        var ss = this.grid.getTopToolbar();
        if (rs.length < 1) { return ''; }
        var groupField = this.getGroupField();
        var gfLen = groupField.length;
        for (var i = 0; i < cs.length; i++)
            cs[i].style = this.getColumnStyle(i, false);

        // Remove all entries alreay in the toolbar
        for (var hh = 0; hh < ss.items.length; hh++)
            Ext.removeNode(Ext.getDom(ss.items.itemAt(hh).id));

        ss.removeAll();
        if (gfLen == undefined || gfLen == 0)
            ss.addItem(new Ext.Toolbar.TextItem("Перетащите сюда столбцы для группировки"));
        else {
            // Add back all entries to toolbar from GroupField[]    
            ss.addItem(new Ext.Toolbar.TextItem("Группировка:"));
            for (var gfi = 0; gfi < gfLen; gfi++) {
                var t = groupField[gfi];
                /*if (gfi>0)*/
                {
                    ss.addItem(new Ext.Toolbar.Separator());
                    var b = new Ext.Toolbar.Button({
                        text: this.get_column_by_id(t).header
                    });
                    b.fieldName = t;
                    ss.addItem(b);
                }
            }
        }
        ss.doLayout();
        this.enableGrouping = !!groupField;
        if (!this.enableGrouping || this.isUpdating)
            return Ext.grid.GroupingView.superclass.doRender.apply(this, arguments);

        var gstyle = 'width:' + this.getTotalWidth() + ';';
        var gidPrefix = this.grid.getGridEl().id;
        var groups = [], curGroup, i, len, gid;
        var lastvalues = [];
        var added = 0;
        var currGroups = [];

        // Create a specific style
        var st = Ext.get(gidPrefix + "-style");
        if (st)
            st.remove();
        var html_code =
            "div#" + gidPrefix +
            " div.x-grid3-row {padding-left:" + (gfLen * 12) + "px}" +
            "div#" + gidPrefix + " div.x-grid3-header {padding-left:" + (gfLen * 12) + "px}";
        Ext.getDoc().child("head").createChild({
            tag: 'style',
            id: gidPrefix + "-style",
            html: html_code
        });
        // traverse all rows in grid
        for (var i = 0, len = rs.length; i < len; i++) {
            added = 0;
            var rowIndex = startRow + i;
            var r = rs[i];
            var differ = 0;
            var gvalue = [];
            var fieldName;
            var fieldLabel;
            var grpFieldNames = [];
            var grpFieldLabels = [];
            var v;
            var changed = 0;
            var addGroup = [];
            var member_of_groups = [];
            this.record_memberships[r.id] = member_of_groups;
            // check group fields to see if we have a different group
            for (var j = 0; j < gfLen; j++) {
                fieldName = groupField[j];
                fieldLabel = this.get_column_by_id(fieldName).header;
                v = r.data[fieldName];
                if (v) {
                    if (i == 0) {
                        // First record always starts a new group
                        addGroup.push({ idx: j, dataIndex: fieldName, header: fieldLabel, value: v });
                        lastvalues[j] = v;
                        gvalue.push(v);
                        grpFieldNames.push(fieldName);
                        grpFieldLabels.push(fieldLabel + ': ' + v);
                    }
                    else {
                        if (lastvalues[j] != v) {
                            // This record is not in same group as previous one
                            addGroup.push({ idx: j, dataIndex: fieldName, header: fieldLabel, value: v });
                            lastvalues[j] = v;
                            changed = 1;
                            gvalue.push(v);
                            grpFieldNames.push(fieldName);
                            grpFieldLabels.push(fieldLabel + ': ' + v);
                        }
                        else {
                            if (gfLen - 1 == j && changed != 1) {
                                // This row is in all the same groups to the previous group
                                curGroup.rs.push(r);
                                member_of_groups.push(curGroup);
                            }
                            else if (changed == 1) {
                                // This group has changed because an earlier group changed.
                                addGroup.push({ idx: j, dataIndex: fieldName, header: fieldLabel, value: v });
                                gvalue.push(v);
                                grpFieldNames.push(fieldName);
                                grpFieldLabels.push(fieldLabel + ': ' + v);
                            }
                            else if (j < gfLen - 1) {
                                var parent_group = currGroups[fieldName];
                                // This is a parent group, and this record is part of this parent so add it
                                if (parent_group) {
                                    parent_group.rs.push(r);
                                    member_of_groups.push(parent_group);
                                }
                            }
                        }
                    }
                }
                else {
                    if (this.displayEmptyFields) {
                        addGroup.push({ idx: j, dataIndex: fieldName, header: fieldLabel, value: this.emptyGroupText || '(none)' });
                        grpFieldNames.push(fieldName);
                        grpFieldLabels.push(fieldLabel + ': ');
                    }
                }
            } // end of "for j"

            // build current group record
            for (var k = 0; k < addGroup.length; k++) {
                var gp = addGroup[k];
                g = gp.dataIndex;
                var glbl = addGroup[k].header;
                N = this.cm.findColumnIndex(g);
                var F = this.cm.config[N];
                var B = F.groupRenderer || F.renderer;
                var S = this.showGroupName ? (F.groupName || F.header) + ": " : "";
                V = this.getGroup(gp.value, r, B, i, N, ds);
                gid = gidPrefix + '-gp-' + gp.dataIndex + '-' +
                    Ext.util.Format.htmlEncode(gp.value);

                // if state is defined use it, however state is in terms of expanded
                // so negate it, otherwise use the default.
                var isCollapsed = typeof this.state[gid] !== 'undefined' ?
                    !this.state[gid] : this.startCollapsed;

                var gcls = isCollapsed ? 'x-grid-group-collapsed' : '';
                curGroup = {
                    group: gp.dataIndex,
                    gvalue: V,
                    key: r.data[gp.dataIndex], // current grouping key
                    text: gp.header,
                    groupId: gid,
                    group_level: gp.idx,
                    startRow: rowIndex,
                    rs: [r],
                    cls: gcls,
                    style: gstyle + 'padding-left:' + (gp.idx * 12) + 'px;'
                };
                currGroups[gp.dataIndex] = curGroup;
                groups.push(curGroup);
                r._groupId = gid; // Associate this row to a group
                member_of_groups.push(curGroup);
                if (typeof this.groups == "undefined")
                    this.groups = new Array();
                this.groups.push(curGroup);
            } // end of "for k"
        } // end of "for i"

        var buf = [];
        var parents_queued = [];
        var queue_count = 0;
        for (var ilen = 0, len = groups.length; ilen < len; ilen++) {
            var g = groups[ilen];
            var next_group = groups[ilen + 1];
            var cur_level = g.group_level;
            var next_level = next_group == null ? -1 : next_group.group_level;
            var leaf = g.group == groupField[gfLen - 1]
            this.doMultiGroupStart(buf, g, cs, ds, colCount);
            if (g.rs.length != 0 && leaf) {
                buf[buf.length] = Ext.grid.GroupingView.superclass.doRender.call(
                    this, cs, g.rs, ds, g.startRow, colCount, stripe);
            }
            if (leaf) {
                // do summaries on all grouping levels for this group
                this.doMultiGroupEnd(buf, g, cs, ds, colCount);
            }
            else
                parents_queued.push(g);
            if (next_level >= cur_level)
                continue;
            // going back from leaf - pop parents from queue
            // and call doGroupEnd for each
            while (cur_level > next_level && cur_level > 0) {
                g = parents_queued.pop();
                this.doMultiGroupEnd(buf, g, cs, ds, colCount);
                cur_level--;
            }
        }
        return buf.join('');
    },

    initTemplates: function () {
        Ext.grid.GroupingView.superclass.initTemplates.call(this);
        this.state = {};

        var sm = this.grid.getSelectionModel();
        sm.on(sm.selectRow ? 'beforerowselect' : 'beforecellselect', 
            this.onBeforeRowSelect, this);

        if (!this.startMultiGroup) {
            this.startMultiGroup = new Ext.XTemplate(
                '<div id="{groupId}" class="x-grid-group {cls}">',
                    '<div id="{groupId}-hd" class="x-grid-group-hd" style="{style}"><div class="x-grid-group-title">',
                    this.groupTextTpl,
                    '</div></div>', 
                    '<div id="{groupId}-bd" class="x-grid-group-body">');
        }
        
        this.startMultiGroup.compile();
        this.endMultiGroup = '</div></div>';
    },

    doMultiGroupStart: function (buf, g, cs, ds, colCount) {
        var groupName = g.group.toLowerCase(),
        groupFieldTemplate;

        if (ds.groupFieldTemplates && (groupFieldTemplate = ds.groupFieldTemplates[groupName])) {
            if (typeof (groupFieldTemplate) == 'string') {
                groupFieldTemplate = new Ext.XTemplate('<div id="{groupId}" class="x-grid-group {cls}">', '<div id="{groupId}-hd" class="x-grid-group-hd" style="{style}"><div>', groupFieldTemplate, '</div></div>', '<div id="{groupId}-bd" class="x-grid-group-body">');
                groupFieldTemplate.compile();
            }
            buf[buf.length] = groupFieldTemplate.apply(g);
        } else {
            buf[buf.length] = this.startMultiGroup.apply(g);
        }

    },

    doMultiGroupEnd: function (buf, g, cs, ds, colCount) {
        buf[buf.length] = this.endMultiGroup;
    },

    getGroup: function (A, D, F, G, B, E) {
        var C = F ? F(A, {}, D, G, B, E) : String(A);
        if (C === "")
            C = this.cm.config[B].emptyGroupText || this.emptyGroupText;
        return C;
    },
    
    /** Should return an array of all elements that represent a row, it should bypass
    *  all grouping sections
    */
    getRows: function () {
        if (!this.enableGrouping)
            return Ext.grid.GroupingView.superclass.getRows.call(this);

        return Ext.DomQuery.select("div.x-grid3-row", this.mainBody.dom);
    },
    
    getGroupById: function (gid) {
        var g = null;
        for (var i = 0; i < this.groups.length; i++) {
            group = this.groups[i];
            if (group.groupId == gid)
                return group;
        }
        return g;
    },
    
    /* override the processEvent function of groupingView to handle groupField
    * which is an array of fields
    */
    processEvent: function (name, e) {
        Ext.grid.GroupingView.superclass.processEvent.call(this, name, e);
        var hd = e.getTarget('.x-grid-group-hd', this.mainBody);
        if (hd) {
            // group value is at the end of the string
            var field = this.getGroupField();
            // in MultiGrouping field is an array of field names, so take just
            // the last field name
            if (typeof field == "object" && field.length)
                field = field[field.length - 1];
            var prefix = this.getPrefix(field);
            var groupValue = hd.id.substring(prefix.length);
            var emptyRe = new RegExp('gp-' + Ext.escapeRe(field) + '--hd');
            // remove trailing '-hd'
            groupValue = groupValue.substr(0, groupValue.length - 3);

            // also need to check for empty groups
            if (groupValue || emptyRe.test(hd.id)) {
                this.grid.fireEvent('group' + name, this.grid, field, groupValue, e);
            }
            if (name == 'mousedown' && e.button == 0) {
                this.toggleGroup(hd.parentNode);
            }
        }
    }
});

Ext.ux.MultiGroupingPanel = function (config) {
    config = config || {};
    config.tbar = new Ext.Toolbar({ id: 'grid-tbr' });
    Ext.ux.MultiGroupingPanel.superclass.constructor.call(this, config);
};

Ext.extend(Ext.ux.MultiGroupingPanel, Ext.grid.EditorGridPanel, {
    initComponent: function () {
        Ext.ux.MultiGroupingPanel.superclass.initComponent.call(this);
        this.on("render", this.setUpDragging, this);
    },
    setUpDragging: function () {
        this.dragZone = new Ext.dd.DragZone(this.getTopToolbar().getEl(), {
            ddGroup: "grid-body"
           , panel: this
           , scroll: false
           , onInitDrag: function (e) {
               var clone = this.dragData.ddel;
               clone.id = Ext.id('ven');
               this.proxy.update(clone);
               return true;
           }
           , getDragData: function (e) {
               var target = Ext.get(e.getTarget().id);
               if (target.hasClass('x-toolbar x-small-editor')) {
                   return false;
               }
               d = e.getTarget().cloneNode(true);
               d.id = Ext.id();
               this.dragData = {
                   repairXY: Ext.fly(target).getXY(),
                   ddel: d,
                   btn: e.getTarget()
               };
               return this.dragData;
           }
           , getRepairXY: function () { return this.dragData.repairXY; }
        });
        this.dropTarget2s = new Ext.dd.DropTarget('grid-tbr', {
            ddGroup: "gridHeader" + this.getGridEl().id
           , panel: this
           , notifyDrop: function (dd, e, data) {
               var btname = this.panel.getColumnModel().getDataIndex(this.panel.getView().getCellIndex(data.header));
               this.panel.store.groupBy(btname);
               return true;
           }
        });
        this.dropTarget22s = new Ext.dd.DropTarget(this.getView().el.dom.childNodes[0].childNodes[1], {
            ddGroup: "grid-body"
           , panel: this
           , notifyDrop: function (dd, e, data) {
               var txt = Ext.get(data.btn).dom.innerHTML;
               var tb = this.panel.getTopToolbar();
               var bidx = tb.items.findIndexBy(function (b) {
                   return b.text == txt;
               }, this);
               if (bidx < 0) return false;
               var fld = tb.items.get(bidx).fieldName;
               Ext.removeNode(Ext.getDom(tb.items.get(bidx).id));
               if (bidx > 0) Ext.removeNode(Ext.getDom(tb.items.get(bidx - 1).id)); ;
               var cidx = this.panel.view.cm.findColumnIndex(fld);
               if (cidx < 0) { }
               this.panel.view.cm.setHidden(cidx, false);
               var temp = [];
               for (var i = this.panel.store.groupField.length - 1; i >= 0; i--) {
                   if (this.panel.store.groupField[i] == fld) {
                       this.panel.store.groupField.pop();
                       break;
                   }
                   temp.push(this.panel.store.groupField[i]);
                   this.panel.store.groupField.pop();
               }
               for (var i = temp.length - 1; i >= 0; i--) { this.panel.store.groupField.push(temp[i]); }
               if (this.panel.store.groupField.length == 0) { this.panel.store.groupField = false; }

               this.panel.store.fireEvent('datachanged', this);
               return true;
           }
        });
    },
    applyState: function (state) {
        var cm = this.colModel,
        cs = state.columns,
        store = this.store,
        s,
        c,
        colIndex;
        if (cs) {
            for (var i = 0, len = cs.length; i < len; i++) {
                s = cs[i];
                c = cm.getColumnById(s.id);
                if (c) {
                    colIndex = cm.getIndexById(s.id);
                    cm.setState(colIndex, {
                        hidden: s.hidden,
                        width: s.width,
                        sortable: s.sortable
                    });
                    if (colIndex != i) {
                        cm.moveColumn(colIndex, i);
                    }
                }
            }
        }
        if (store) {
            s = state.sort;
            if (s) {
                store[store.remoteSort ? 'setDefaultSort' : 'sort'](s);
            }
            s = state.group;
            if (store.groupBy) {
                if (s) {
                    store.groupBy(s, undefined, true);
                } else {
                    store.clearGrouping();
                }
            }
        }
        var o = Ext.apply({}, state);
        delete o.columns;
        delete o.sort;
        Ext.grid.GridPanel.superclass.applyState.call(this, o);
    }
});

Ext.reg('Ext.ux.MultiGroupingStore', Ext.ux.MultiGroupingStore);
Ext.reg('Ext.ux.MultiGroupingView', Ext.ux.MultiGroupingView);
Ext.reg('Ext.ux.MultiGroupingPanel', Ext.ux.MultiGroupingPanel);
