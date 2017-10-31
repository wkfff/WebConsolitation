/**
 * Licensed under GNU LESSER GENERAL PUBLIC LICENSE Version 3
 *
 * @author Thorsten Suckow-Homberg <ts@siteartwork.de>
 * @url http://www.siteartwork.de/cardlayout
 */

Ext.ns('Ext.ux.wizard');

/**
 * @class Ext.ux.layout.CardLayout
 * @extends Ext.layout.CardLayout
 *
 * A specific {@link Ext.layout.CardLayout} that only sets the active item
 * if the 'beforehide'-method of the card to hide did not return false (in this case,
 * components usually won't be hidden).
 * The original implementation of {@link Ext.layout.CardLayout} does not take
 * the return value of the 'beforehide'-method into account.
 *
 * @constructor
 * @param {Object} config The config object
 */
Ext.ux.wizard.CardLayout = Ext.extend(Ext.layout.CardLayout, {
    getNext: function (step) {
         var items = this.container.items,
            index = items.indexOf(this.activeItem);
        
        return items.get(index + (step || 1)) || false;
    },

    next: function (step, anim) {
        return this.setActiveItem(this.getNext(step), anim);
    },

    getPrev: function (step) {
        var items = this.container.items,
            index = items.indexOf(this.activeItem);
            
        return items.get(index - (step || 1)) || false;
    },

    prev: function (step, anim) {
        return this.setActiveItem(this.getPrev(step), anim);
    }
});