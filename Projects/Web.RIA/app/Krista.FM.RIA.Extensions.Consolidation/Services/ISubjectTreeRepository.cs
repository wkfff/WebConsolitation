using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public interface ISubjectTreeRepository
    {
        IList<SubjectsTree> GetTree(int subjectId);

        IList<D_CD_Subjects> GetChildsAndSelf(D_CD_Subjects subject);
    }
}