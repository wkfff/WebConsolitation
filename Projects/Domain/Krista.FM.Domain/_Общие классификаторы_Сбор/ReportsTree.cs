using System;

namespace Krista.FM.Domain
{
    /// <summary>
    /// Дерево сбора отчетности.
    /// </summary>
    public class ReportsTree : DomainObject
    {
        public virtual string TemplateName { get; set; }
        
        public virtual string TemplateShortName { get; set; }
        
        public virtual string TemplateClass { get; set; }
        
        public virtual string TemplateGroup { get; set; }

        public virtual int  PeriodId { get; set; }
        
        public virtual string Period { get; set; }

        public virtual string FormName { get; set; }

        public virtual string TaskName { get; set; }

        public virtual DateTime BeginDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual DateTime Deadline { get; set; }

        public virtual int OwnerUserId { get; set; }

        public virtual int Year { get; set; }

        public virtual int StatusId { get; set; }
        
        public virtual string Status { get; set; }

        public virtual int SubjectId { get; set; }
        
        public virtual string Subject { get; set; }
        
        public virtual string SubjectShortName { get; set; }

        public virtual string Role { get; set; }

        public virtual string ReportLevel { get; set; }

        public virtual DateTime? LastChangeDate { get; set; }

        public virtual string LastChangeUser { get; set; }
    }
}