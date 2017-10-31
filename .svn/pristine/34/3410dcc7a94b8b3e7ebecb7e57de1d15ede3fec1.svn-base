using System;
using System.Collections.Generic;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoUniqueKey : SmoCommonObject, IUniqueKey
    {
        public SmoUniqueKey(IUniqueKey serverObject) 
            : base(serverObject)
        {
        }

        
        public SmoUniqueKey(SMOSerializationInfo cache) 
            : base(cache)
        {
        }


        public IEntity Parent
        {       
            get
            {   
                return ((IUniqueKey) serverControl).Parent;
            }
        }

       
        public List<string> Fields
        {
            get
            {
                // return ((IUniqueKey) serverControl).Fields;
                if (cached)
                {
                    return (List<string>)GetCachedValue("Fields");
                }
                else
                {
                    return ((IUniqueKey)serverControl).Fields;
                }
            }
            set
            {
                ((IUniqueKey)serverControl).Fields = value;
            }
        }

        public bool Hashable
        {
            get { return ((IUniqueKey) serverControl).Hashable; }
            set { ((IUniqueKey) serverControl).Hashable = value; }
        }
    }

}
