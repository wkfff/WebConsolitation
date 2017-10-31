Ext.form.ComboBox.override({

    checkTab: function (e, me) {
        if (!e.getKey) {
            var t = e;
            e = me;
            me = t;
        }

        var key = e.getKey();

        if (key == e.TAB) {
            if (this.isExpanded()) {
                this.onViewClick(false);
            }

            if (!this.inEditor) {
                this.triggerBlur();
            }
        }

        if (key == e.RIGHT) {
            if (e.ctrlKey) {
                if (this.pageTb != null) {
                    var d = this.pageTb.getPageData();
                    if (d.activePage < d.pages)
                        this.pageTb.moveNext();
                }
            }
        }
        else
            if (key == e.LEFT) {
                if (e.ctrlKey) {
                    if (this.pageTb != null) {
                        var d = this.pageTb.getPageData();
                        if (d.activePage > 1)
                            this.pageTb.movePrevious();
                    }
                }
            }
    }
});