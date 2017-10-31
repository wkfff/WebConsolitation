using System;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Models
{
    public class ProjectDetailViewModel
    {
        public int ID { get; set; }

        public DateTime IncomingDate { get; set; }
        
        public DateTime? RosterDate { get; set; }
        
        public string Name { get; set; }
        
        public string InvestorName { get; set; }
        
        public string LegalAddress { get; set; }
        
        public string MailingAddress { get; set; }
        
        public string Email { get; set; }
        
        public string Phone { get; set; }
        
        public string Goal { get; set; }
        
        public string ExpectedOutput { get; set; }
        
        public string Production { get; set; }
        
        public string PaybackPeriod { get; set; }

        public string DocBase { get; set; }
        
        public string InvestAgreement { get; set; }
        
        public string AddMech { get; set; }
        
        public string ExpertOpinion { get; set; }
        
        public string Study { get; set; }
        
        public string Effect { get; set; }
        
        public string Exception { get; set; }
        
        public string Contact { get; set; }
        
        public string Code { get; set; }
        
        public int? RefBeginDateVal { get; set; }
        
        public int? RefEndDateVal { get; set; }
        
        public int RefTerritoryId { get; set; }
        
        public string RefTerritoryName { get; set; }
        
        public int RefStatusId { get; set; }
        
        public int RefPartId { get; set; }
        
        public int RefOKVEDId { get; set; }
        
        public string RefOKVEDName { get; set; }
    }
}
