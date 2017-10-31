using System;
using System.Collections.Generic;
using Ext.Net;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.MinSport.MinSportUtils
{
    public class SportUtils
    {        
        public static Store CreateStoreForComboBox(string controller, string action, string storeId)
        {
            var mainComboBoxStore = new Store { ID = storeId }
                .SetJsonReader()
                .SetHttpProxy(String.Format("/{0}/{1}", controller, action))
                .AddField("Value")
                .AddField("Text");
            return mainComboBoxStore;
        }

        public static Store CreateStoreForComboBox(string controller, string action, string storeId, bool autoLoad, Dictionary<string, string> parametersList)
        {
            var mainComboBoxStore = CreateStoreForComboBox(controller, action, storeId);
            mainComboBoxStore.AutoLoad = autoLoad;
            foreach (KeyValuePair<string, string> kvp in parametersList)
            {
                mainComboBoxStore.SetBaseParams(kvp.Key, kvp.Value, ParameterMode.Raw);
            }

            return mainComboBoxStore;
        }
    }
}
