using System;

namespace Krista.FM.RIA.Extensions.Consolidation.Models
{
    public class TaskFileViewModel
    {
        public int ID { get; set; }

        public string FileName { get; set; }

        public string FileDescription { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? ChangeDate { get; set; }

        public string ChangeUser { get; set; }
    }
}
