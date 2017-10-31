using System.Collections.Generic;

namespace Krista.FM.RIA.Core
{
    public class AjaxStoreSaveDomainModel<T>
    {
        public AjaxStoreSaveDomainModel()
        {
            Created = new List<T>();
            Deleted = new List<T>();
            Updated = new List<T>();
        }
        
        public List<T> Created { get; set; }

        public List<T> Deleted { get; set; }

        public List<T> Updated { get; set; }
    }
}
