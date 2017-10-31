/* Extjs 3.4 не поддерживает LocalStorageProvider, в 4 он уже есть */

Ext.ns('Ext.ux.state');

/**
* @class Ext.ux.state.LocalStorageProvider
* @extends Ext.state.Provider
* A Provider implementation which saves and retrieves state via the HTML5 localStorage object.
* If the browser does not support local storage, an exception will be thrown upon instantiating
* this class.
* <br />Usage:
<pre><code>
Ext.state.Manager.setProvider(new Ext.ux.state.LocalStorageProvider({prefix: 'my-'}));
</code></pre>
* @cfg {String} prefix The application-wide prefix for the stored objects
* @constructor
* Create a new LocalStorageProvider
* @param {Object} config The configuration object
*/

Ext.ux.state.LocalStorageProvider = Ext.extend(Ext.state.Provider, {

    constructor: function (config) {
        Ext.ux.state.LocalStorageProvider.superclass.constructor.call(this);
        Ext.apply(this, config);
        
        this.prefix = this.prefix.hashCode();
        
        this.store = this.getStorageObject();
        this.state = this.readLocalStorage();
    },

    readLocalStorage: function () {
        var store = this.store,
        i = 0,
        len = store.length,
        prefix = this.prefix + this.userName,
        prefixLen = prefix.length,
        data = {},
        key;

        for (; i < len; ++i) {
            key = store.key(i);
            if (key.substring(0, prefixLen) == prefix) {
                var k = key.substr(prefixLen);
                data[k] = this.decodeValue(store.getItem(key));
            }
        }

        return data;
    },

    set: function (name, value) {
        this.clear(name);
        if (typeof value == "undefined" || value === null) {
            return;
        }
        this.store.setItem(this.prefix + this.userName + name, this.encodeValue(value));

        Ext.ux.state.LocalStorageProvider.superclass.set.call(this, name, value);
    },

    // private
    clear: function (name) {
        this.store.removeItem(this.prefix + name);

        Ext.ux.state.LocalStorageProvider.superclass.clear.call(this, name);
    },
    
    removeAll: function () {
       var i = 0,
       store = this.store,
       len = store.length,
       prefix = this.prefix + this.userName,
       prefixLen = prefix.length,
       key;
        
        for (; i < len; ++i) {
            key = store.key(i);
            if (key.indexOf(this.userName) !== -1) {
                this.store.removeItem(key);
            }
        }
    },

    getStorageObject: function () {
        try {
            var supports = 'localStorage' in window && window['localStorage'] !== null;
            if (supports) {
                return window.localStorage;
            }
        } catch (e) {
            return false;
        }
    }
    
});

//////////////////////////////////////////////////////////////////////////////////////////////////////
/////http://werxltd.com/wp/2010/05/13/javascript-implementation-of-javas-string-hashcode-method/ /////
//////////////////////////////////////////////////////////////////////////////////////////////////////

String.prototype.hashCode = function () {
    var hash = 0;
    if (this.length == 0) return hash;
    for (i = 0; i < this.length; i++) {
        char = this.charCodeAt(i);
        hash = ((hash << 5) - hash) + char;
        hash = hash & hash; // Convert to 32bit integer
    }
    
    return hash;
}