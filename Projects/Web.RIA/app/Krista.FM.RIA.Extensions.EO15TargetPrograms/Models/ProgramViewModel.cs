namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Models
{
    public class ProgramViewModel
    {
        public int ID { get; set; }
        
        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Creator { get; set; }
        
        public string ApproveDate { get; set; }

        public int RefTypeProgId { get; set; }

        public int RefBeginDateVal { get; set; }
        
        public int RefEndDateVal { get; set; }
        
        public string NpaListCommaSeparated { get; set; }
        
        public string ParentName { get; set; }
        
        public int? ParentId { get; set; }
    }
}
