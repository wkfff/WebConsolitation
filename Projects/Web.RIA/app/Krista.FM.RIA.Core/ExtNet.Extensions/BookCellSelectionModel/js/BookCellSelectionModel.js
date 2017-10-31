Ext.ux.BookCellSelectionModel = Ext.extend(Ext.grid.CellSelectionModel, {

    /** @ignore */
    handleKeyDown: function (e) {
        if (!e.isNavKeyPress()) {
            return;
        }

        var k = e.getKey(),
            g = this.grid,
            s = this.selection,
            sm = this,
            walk = function (row, col, step) {
                return g.walkCells(
                    row,
                    col,
                    step,
                    g.isEditor && g.editing ? sm.acceptsNav : sm.isSelectable, // *** handle tabbing while editorgrid is in edit mode
                    sm
                );
            },
            cell, newCell, r, c, ae;

        if (!s) {
            cell = walk(0, 0, 1); // *** use private walk() function defined above
            if (cell) {
                this.select(cell[0], cell[1]);
            }
            return;
        }

        cell = s.cell;  // currently selected cell
        r = cell[0];    // current row
        c = cell[1];    // current column

        switch (k) {
            case e.ENTER:
                Extension.entityBook.bookEnterHandler();
                return;
            case e.ESC:
                Extension.entityBook.bookEscHandler();
                return;
            case e.PAGE_UP:
            case e.PAGE_DOWN:
                // do nothing
                break;
            default:
                // *** call e.stopEvent() only for non ESC, PAGE UP/DOWN KEYS
                e.stopEvent();
                break;
        }

        switch (k) {
            case e.TAB:
                if (e.shiftKey) {
                    newCell = walk(r, c - 1, -1);
                } else {
                    newCell = walk(r, c + 1, 1);
                }
                break;
            case e.DOWN:
                newCell = walk(r + 1, c, 1);
                break;
            case e.UP:
                newCell = walk(r - 1, c, -1);
                break;
            case e.RIGHT:
                newCell = walk(r, c + 1, 1);
                break;
            case e.LEFT:
                newCell = walk(r, c - 1, -1);
                break;
        }

        if (newCell) {
            // *** reassign r & c variables to newly-selected cell's row and column
            r = newCell[0];
            c = newCell[1];

            this.select(r, c); // *** highlight newly-selected cell and update selection

            if (g.isEditor && g.editing) { // *** handle tabbing while editorgrid is in edit mode
                ae = g.activeEditor;
                if (ae && ae.field.triggerBlur) {
                    // *** if activeEditor is a TriggerField, explicitly call its triggerBlur() method
                    ae.field.triggerBlur();
                }
                g.startEditing(r, c);
            }
        }
    }

});