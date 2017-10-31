Ext.ns("MdiTab");

MdiTab = {
    hashCode: function (str) {
        var hash = 1315423911;

        for (var i = 0; i < str.length; i++) {
            hash ^= ((hash << 5) + str.charCodeAt(i) + (hash >> 2));
        }

        return (hash & 0x7FFFFFFF);
    },

    addTabTo: function (tabPanel, config) {
        if (Ext.isEmpty(config.url, false)) {
            return;
        }

        var id;

        if ((typeof config.id) !== 'undefined') {
            id = config.id
        }
        else {
            id = this.hashCode(config.url);
        }
        var tab = tabPanel.getComponent(id);

        if (!tab) {
            tab = tabPanel.addTab({
                id: id.toString(),
                title: config.title,
                iconCls: config.icon || 'icon-applicationdouble',
                closable: true,
                autoLoad: {
                    showMask: true,
                    url: config.url,
                    mode: 'iframe',
                    noCache: true,
                    maskMsg: "Загрузка '" + config.title + "'...",
                    scripts: true,
                    passParentSize: config.passParentSize
                }
            });
        } else {
            tabPanel.setActiveTab(tab);
            Ext.get(tab.tabEl).frame();
        }
    },

    addTab: function (config) {
        var tabPanel = Ext.getCmp('tpMain');
        this.addTabTo(tabPanel, config);
    },

    getComponent: function (id) {
        var tp = Ext.getCmp('tpMain');
        return tp.getComponent(id);
    },

    setActiveTab: function (tab) {
        var tp = Ext.getCmp('tpMain');
        tp.setActiveTab(tab);
    },

    getActiveTab: function () {
        var tp = Ext.getCmp('tpMain');
        var t = tp.getActiveTab();
        return t;
    },

    getTabs: function () {
        var tp = Ext.getCmp('tpMain');
        return tp.items.items;
    }

};
