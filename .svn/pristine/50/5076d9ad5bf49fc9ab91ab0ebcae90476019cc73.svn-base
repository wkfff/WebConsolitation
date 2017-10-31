using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class UserFormsControls
    {
        private Dictionary<string, object> controls = new Dictionary<string, object>();

        public UserFormsControls(int id)
        {
            VarID = id;
            DataService = Core.Resolver.Get<IDataService>();
        }

        public UserFormsControls()
        {
        }

        public IDataService DataService { get; set; }

        public int VarID { get; private set; }

        public int ParamId { get; set; }

        public object GetObject(string key)
        {
            if (controls.ContainsKey(key))
            {
                return controls[key];
            }

            return null;
        }

        public void DeleteObject(string key)
        {
            if (controls.ContainsKey(key))
            {
                controls.Remove(key);
            }
        }

        public void AddObject(string key, object o)
        {
            controls.Add(key, o);
        }

        public void UpdateObject(string key, object o)
        {
            controls.Remove(key);
            controls.Add(key, o);
        }

        public bool Contains(string key)
        {
            return controls.ContainsKey(key);
        }
    }
}
