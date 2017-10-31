/**
 * Licensed under GNU LESSER GENERAL PUBLIC LICENSE Version 3
 *
 * @author Thorsten Suckow-Homberg <ts@siteartwork.de>
 * @url http://www.siteartwork.de/wizardcomponent
 */

Ext.ns('Ext.ux.wizard');

/**
 * @class Ext.ux.Wiz.Card
 * @extends Ext.FormPanel
 *
 * A specific {@link Ext.FormPanel} that can be used as a card in a
 * {@link Ext.ux.Wiz}-component. An instance of this card does only work properly
 * if used in a panel that uses a {@see Ext.layout.CardLayout}-layout.
 *
 * @constructor
 * @param {Object} config The config object
 */
Ext.ux.wizard.Card = Ext.extend(Ext.form.FormPanel, {
    cardTitle: '',
    cls: 'ux-wiz-card',

    /**
    * @cfg {Boolean} header "True" to create the header element. Defaults to
    * "false". See {@link Ext.form.FormPanel#header}
    */
    header: false,

    /**
    * @cfg {Strting} hideMode Hidemode of this component. Defaults to "offsets".
    * See {@link Ext.form.FormPanel#hideMode}
    */
    hideMode: 'display',

    initComponent: function () {

        this.addEvents(
        /**
        * @event beforecardhide
        * If you want to add additional checks to your card which cannot be easily done
        * using default validators of input-fields (or using the monitorValid-config option),
        * add your specific listeners to this event.
        * This event gets only fired if the activeItem of the ownerCt-component equals to
        * this instance of {@see Ext.ux.Wiz.Card}. This is needed since a card layout usually
        * hides it's items right after rendering them, involving the beforehide-event.
        * If those checks would be attached to the normal beforehide-event, the card-layout
        * would never be able to hide this component after rendering it, depending on the
        * listeners return value.
        *
        * @param {Ext.ux.Wiz.Card} card The card that triggered the event
        */
            'beforecardhide'
        );

        this.cardTitle = this.title;
        this.title = (this.showTitle ? '<span style="' + this.titleStyle + '" class="' + this.titleCls + '" >' + this.title + '</span>' : '');

        Ext.ux.wizard.Card.superclass.initComponent.call(this);

        this.on('show', this.onCardShow, this);
    },

    beforeStep: function (card, direction) {
        return true;
    },

    canNavigate: function (direction) {
        return direction > 0 ? this.isValid() : true;
    },

    // -------- helper
    isValid: function () {
        return this.getForm().isValid();
    },

    /**
    * Overrides parent implementation. This is needed because in case
    * this method uses "monitorValid=true", the method "startMonitoring" must
    * not be called, until the "show"-event of this card fires.
    */
    initEvents: function () {
        var old = this.monitorValid;
        this.monitorValid = false;
        
        Ext.ux.wizard.Card.superclass.initEvents.call(this);
        
        this.monitorValid = old;

        this.on('beforehide', this.bubbleBeforeHideEvent, this);

        this.on('navigate', this.onNavigate, this);
        //this.on('beforecardhide', this.isValid, this);
        this.on('hide', this.onCardHide, this);
    },

    // -------- listener

    /**
    * @private virtual
    */
    onNavigate: function (card, direction) {
        return this.isValid();
    },

    /**
    * Checks wether the beforecardhide-event may be triggered.
    */
    bubbleBeforeHideEvent: function () {
        var ly = this.ownerCt.layout;
        var activeItem = ly.activeItem;

        if (activeItem && activeItem.id === this.id) {
            return this.fireEvent('beforecardhide', this);
        }

        return true;
    },

    /**
    * Stops monitoring the form elements in this component when the
    * 'hide'-event gets fired.
    */
    onCardHide: function () {
        if (this.monitorValid) {
            this.stopMonitoring();
        }
    },

    /**
    * Starts monitoring the form elements in this component when the
    * 'show'-event gets fired.
    */
    onCardShow: function () {
        if (this.monitorValid) {
            this.startMonitoring();
        }
    }
});