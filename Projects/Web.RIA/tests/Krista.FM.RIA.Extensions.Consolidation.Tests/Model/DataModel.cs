using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    public class DataModel
    {
        public DataModel()
        {
            Initialize();
        }

        public List<FX_Date_Year> Years { get; set; }

        public List<FX_FX_FormStatus> FormStatus { get; set; }
        
        public List<D_CD_Roles> Roles { get; set; }

        public List<D_CD_Subjects> Subjects { get; set; }
        
        public List<FX_FX_Periodicity> Periodicity { get; set; }

        public List<D_CD_ReportKind> Forms { get; set; }
        
        public List<D_CD_Templates> Templates { get; set; }
        
        public List<D_CD_Reglaments> Reglaments { get; set; }

        private void Initialize()
        {
            FormStatus = new List<FX_FX_FormStatus>
            {
                new FX_FX_FormStatus { ID = 1, Name = "На редактировании" },
                new FX_FX_FormStatus { ID = 2, Name = "На рассмотрении" },
                new FX_FX_FormStatus { ID = 3, Name = "Утвержден" }
            };

            Years = new List<FX_Date_Year>
            {
                new FX_Date_Year { ID = 2010 },
                new FX_Date_Year { ID = 2011 },
                new FX_Date_Year { ID = 2012 }
            };

            Roles = new List<D_CD_Roles>
            {
                new D_CD_Roles { ID = 1, Name = "ОИВ" },
                new D_CD_Roles { ID = 2, Name = "ОМСУ" }
            };

            Subjects = new List<D_CD_Subjects>
            {
                new D_CD_Subjects { ID = 1, Name = "Департамент образования ЯО", ShortName = "ДОЯО", RefRole = Roles[0], UserId = 1, },
                new D_CD_Subjects { ID = 2, Name = "МО г.Рыбинск", ShortName = "ДОМОРыбинск", RefRole = Roles[1], UserId = 2 },
                new D_CD_Subjects { ID = 3, Name = "МО г.Ярославль", ShortName = "ДОМОЯрославль", RefRole = Roles[1], UserId = 3 },
                new D_CD_Subjects { ID = 4, Name = "МО г.Тутаев", ShortName = "ДОМОТутаев", RefRole = Roles[1], UserId = 3 }
            };

            Periodicity = new List<FX_FX_Periodicity>
            {
                new FX_FX_Periodicity { ID = 1, Name = "Годовая" },
                new FX_FX_Periodicity { ID = 2, Name = "Квартальная" },
                new FX_FX_Periodicity { ID = 3, Name = "Месячная" },
                new FX_FX_Periodicity { ID = 4, Name = "Декадная" },
                new FX_FX_Periodicity { ID = 5, Name = "Недельная" },
                new FX_FX_Periodicity { ID = 6, Name = "Разовая" }
            };

            Forms = new List<D_CD_ReportKind>
            {
                new D_CD_ReportKind { ID = 1, Name = "Ежегодная форма сбора" }
            };

            Templates = new List<D_CD_Templates>
            {
                new D_CD_Templates { ID = 1, Name = "Сбор данных для оценки деятельности ОМСУ", Class = "DataRIAForm1" }
            };

            Reglaments = new List<D_CD_Reglaments>
            {
                new D_CD_Reglaments { ID = 1, RefRepKind = Forms[0], RefTemplate = Templates[0], RefRole = Roles[0], Laboriousness = 11, Workdays = true },
                new D_CD_Reglaments { ID = 2, RefRepKind = Forms[0], RefTemplate = Templates[0], RefRole = Roles[1], Laboriousness = 10, Workdays = true }
            };
        }
    }
}
