using System;
using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.Consolidation.Models;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class TaskViewModel
    {
        /// <summary>
        /// Виды статусов утверждения задачи
        /// </summary>
        public enum TaskStatus : int
        {
            /// <summary>
            /// Значение не указано
            /// </summary>
            Undefined = 0,

            /// <summary>
            /// Редактируется ответственным
            /// </summary>
            Edit = 1,

            /// <summary>
            /// На согласовании
            /// </summary>
            OnTest = 2,

            /// <summary>
            /// Утвержден ответственным
            /// </summary>
            Accepted = 3
        }

        public int ID { get; set; }
        
        public DateTime BeginDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public DateTime Deadline { get; set; }
        
        public string TemplateName { get; set; }
        
        public string TemplateShortName { get; set; }

        public string TemplateClass { get; set; }
        
        public string Status { get; set; }
        
        public int RefStatus { get; set; }

        public int SubjectId { get; set; }

        public string SubjectShortName { get; set; }
        
        public int? ParentSubjectId { get; set; }

        public D_Regions_Analysis Region { get; set; }
    }    
}