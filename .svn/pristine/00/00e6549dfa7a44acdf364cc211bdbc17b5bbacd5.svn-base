using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class SubjectTreeRepository : ISubjectTreeRepository
    {
        public IList<SubjectsTree> GetTree(int subjectId)
        {
            string queryString;
            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                queryString = @"
select distinct
  S.ID as ID,
  S.Name as Subject, 
  S.ShortName as SubjectShortName, 
  S.ParentID as ParentId, 
  R.NAME as Role,
  L.NAME as ReportLevel
from D_CD_Subjects S
    join D_CD_ROLES R on S.RefRole = R.ID
    join D_CD_LEVEL L on S.RefLevel = L.ID
connect by prior S.ID = S.ParentId
start with S.ID = :SubjectId";
            }
            else
            {
                queryString = @"
WITH Subjects AS 
(
	SELECT * FROM D_CD_Subjects WHERE ID = :SubjectId
    UNION ALL
    SELECT chld.*  
    FROM D_CD_Subjects AS chld, Subjects
    WHERE Subjects.ID = chld.ParentID
)
SELECT 
    Subjects.ID, 
    Subjects.Name as Subject, 
    Subjects.ShortName as SubjectShortName, 
    Subjects.ParentID as ParentId, 
    R.NAME as Role, 
    L.NAME as ReportLevel
FROM Subjects
    join D_CD_ROLES R ON (R.ID = Subjects.RefRole)
    join D_CD_LEVEL L ON (L.ID = Subjects.RefLevel)";
            }

            var list = NHibernateSession.Current.CreateSQLQuery(queryString)
                .AddEntity(typeof(SubjectsTree))
                .SetInt32("SubjectId", subjectId)
                .List<SubjectsTree>();

            list.First(x => x.ID == subjectId).ParentId = null;
            
            return list;
        }

        public IList<D_CD_Subjects> GetChildsAndSelf(D_CD_Subjects subject)
        {
            string queryString;
            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                queryString = @"
select distinct
  S.*
from D_CD_Subjects S
    join D_CD_ROLES R on S.RefRole = R.ID
    join D_CD_LEVEL L on S.RefLevel = L.ID
connect by prior S.ID = S.ParentId
start with S.ID = :SubjectId";
            }
            else
            {
                queryString = @"
WITH Subjects AS 
(
	SELECT * FROM D_CD_Subjects WHERE ID = :SubjectId
    UNION ALL
    SELECT chld.*  
    FROM D_CD_Subjects AS chld, Subjects
    WHERE Subjects.ID = chld.ParentID
)
SELECT 
    Subjects.* 
FROM Subjects
    join D_CD_ROLES R ON (R.ID = Subjects.RefRole)
    join D_CD_LEVEL L ON (L.ID = Subjects.RefLevel)";
            }

            var list = NHibernateSession.Current.CreateSQLQuery(queryString)
                .AddEntity(typeof(D_CD_Subjects))
                .SetInt32("SubjectId", subject.ID)
                .List<D_CD_Subjects>();

            return list;
        }
    }
}
