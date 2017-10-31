using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ReportTreeService : IReportTreeService
    {
        public IList<ReportsTree> GetReportsTree(int subjectId, bool showChilds)
        {
            string queryString;
            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                if (showChilds)
                {
                    queryString = @"
with SubjectsTree as (
   select S.ID as SubjectId, S.Name as SubjectName, S.ShortName as SubjectShortName, S.REFROLE, S.REFLEVEL
           from d_cd_subjects S
           connect by prior S.id = s.parentid
           start with s.id = :Subject_Id
),";
                }
                else
                {
                    queryString = @"
with SubjectsTree as (
    select S.ID as SubjectId, S.Name as SubjectName, S.ShortName as SubjectShortName, S.REFROLE, S.REFLEVEL
    from d_cd_subjects S
    where S.id = :Subject_Id
),";
                }
            }
            else
            {
                if (showChilds)
                {
                    queryString = @"
with SubjectsTree as (
	select ID as SubjectId, Name as SubjectName, ShortName as SubjectShortName, REFROLE, REFLEVEL
	from d_cd_subjects
	where id = :Subject_Id
	union all
	select S.ID as SubjectId, S.Name as SubjectName, S.ShortName as SubjectShortName, S.REFROLE, S.REFLEVEL
	from d_cd_subjects S, SubjectsTree
	where SubjectsTree.SubjectId = s.parentid
),";
                }
                else
                {
                    queryString = @"
with SubjectsTree as (
	select ID as SubjectId, Name as SubjectName, ShortName as SubjectShortName, REFROLE, REFLEVEL
	from d_cd_subjects
	where id = :Subject_Id
),";
                }
            }

            queryString += @"Tree_with_TASK AS (
  select 
         T.ID, 
         SubjectId, 
         SubjectName, 
         SubjectShortName, 
         T.REFTEMPLATE, 
         S.REFROLE, 
         S.REFLEVEL, 
         Sts.ID as TaskStatusId, 
         Sts.Name as TaskStatus, 
         Y.ID as TaskYear,
         T.OWNERUSERID,
         T.Begindate,
         T.Enddate,
         T.Deadline,
         T.Name,
         T.LastChangeDate,
         T.LastChangeUser,
         T.RefCollectTask
    from SubjectsTree S
    left outer join D_CD_Task T on (S.SubjectId = T.REFSUBJECT)
    join fx_fx_formstatus Sts on (T.RefStatus = Sts.ID)
    join Fx_Date_Year Y on (T.REFYEAR = Y.ID)
),
Reglament as (
  select 
         CT.ID as CollectTaskId,
         R.ID as RoleId, 
         R.Name as RoleName, 
         L.ID as LevelId, 
         L.Name as LevelName, 
         Tmpl.Id as TemplateId, 
         Tmpl.Name as TemplateName, 
         Tmpl.ShortName as TemplateShortName, 
         Tmpl.Class as TemplateClass, 
         Tmpl.NameCD as TemplateGroup, 
         RK.Name as FormName, 
         P.Id as PeriodicyId,
         P.Name as PeriodicyName
    from D_CD_REGLAMENTS Rgl,
         D_CD_TEMPLATES Tmpl,
         D_CD_CollectTask CT,
         d_Cd_Reportkind RK,
         D_CD_PERIOD P,
         D_CD_ROLES R,
         D_CD_LEVEL L
   where Rgl.RefTemplate = Tmpl.ID
     and Rgl.Refrole = R.ID
     and Rgl.RefLevel = L.ID
     and Rgl.RefRepKind = RK.ID
     and CT.REFPERIOD = P.ID
     and P.Refrepkind = RK.ID
)
select 
       TT.ID as ID,
       TemplateName,
       TemplateShortName,
       TemplateClass,
       TemplateGroup,
       PeriodicyId as PeriodId,
       PeriodicyName as Period,
       FormName as FormName,
       TT.SubjectId as SubjectId, 
       TT.Subjectname as Subject, 
       TT.SubjectShortName as SubjectShortName, 
       TT.name as TaskName,
       TT.begindate as BeginDate,
       TT.enddate as EndDate,
       TT.deadline as Deadline,
       TT.owneruserid as OwnerUserId,
       TT.TaskYear as Year,
       TT.TaskStatusId as StatusId,
       TT.TaskStatus as Status,
       TT.LastChangeDate as LastChangeDate,
       TT.LastChangeUser as LastChangeUser,
       RoleName as Role,
       LevelName as ReportLevel
  from Tree_with_TASK TT, Reglament Regl
 where TT.REFROLE = Regl.RoleId 
   and TT.REFLEVEL = Regl.LevelId
   and TT.REFTEMPLATE = Regl.TemplateId
   and TT.RefCollectTask = Regl.CollectTaskId";

            var list = NHibernateSession.Current.CreateSQLQuery(queryString)
                .AddEntity(typeof(ReportsTree))
                .SetInt32("Subject_Id", subjectId)
                .List<ReportsTree>();
            
            return list;
        }
    }
}
