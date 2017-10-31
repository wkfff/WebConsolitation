/*!
 * Ext JS Library 3.4.0
 * Copyright(c) 2006-2011 Sencha Inc.
 * licensing@sencha.com
 * http://www.sencha.com/license
 */
/**
 * @class Ext.grid.CellSelectionModel
 * @extends Ext.grid.AbstractSelectionModel
 * This class provides the basic implementation for <i>single</i> <b>cell</b> selection in a grid.
 * The object stored as the selection contains the following properties:
 * <div class="mdetail-params"><ul>
 * <li><b>cell</b> : see {@link #getSelectedCell} 
 * <li><b>record</b> : Ext.data.record The {@link Ext.data.Record Record}
 * which provides the data for the row containing the selection</li>
 * </ul></div>
 * @constructor
 * @param {Object} config The object containing the configuration of this model.
 */
Ext.ux.RequisitesSelectionModel = Ext.extend(Ext.grid.CellSelectionModel,  {
    
    constructor : function(config){
        Ext.apply(this, config);

	    Ext.ux.RequisitesSelectionModel.superclass.constructor.call(this);
    },

    /**
     * Selects a cell.  Before selecting a cell, fires the
     * {@link #beforecellselect} event.  If this check is satisfied the cell
     * will be selected and followed up by  firing the {@link #cellselect} and
     * {@link #selectionchange} events.
     * @param {Number} rowIndex The index of the row to select
     * @param {Number} colIndex The index of the column to select
     * @param {Boolean} preventViewNotify (optional) Specify <tt>true</tt> to
     * prevent notifying the view (disables updating the selected appearance)
     * @param {Boolean} preventFocus (optional) Whether to prevent the cell at
     * the specified rowIndex / colIndex from being focused.
     * @param {Ext.data.Record} r (optional) The record to select
     */
    select : function(rowIndex, colIndex, preventViewNotify, preventFocus, /*internal*/ r){
        if(this.fireEvent("beforecellselect", this, rowIndex, colIndex) !== false){
            this.clearSelections();
            r = r || this.grid.store.getAt(0);
            this.selection = {
                record : r,
                cell : [rowIndex, colIndex]
            };

            var v = this.grid.getView();
            if(!preventViewNotify){
                v.onCellSelect(rowIndex, colIndex);
                if(preventFocus !== true){
                    v.focusCell(rowIndex, colIndex);
                }
            }
            
            var cellName = v.getCellName(rowIndex, colIndex);
            if (cellName) {
                colIndex = this.grid.getColumnModel().getIndexById(cellName);
                this.fireEvent("cellselect", this, 0, colIndex);
                this.fireEvent("selectionchange", this, this.selection);
            }
        }
    },
    
    onRowUpdated : function(v, index, r){
        if(this.selection && this.selection.record == r){
            v.onCellSelect(this.selection.cell[0], this.selection.cell[1]);
        }
    }
});