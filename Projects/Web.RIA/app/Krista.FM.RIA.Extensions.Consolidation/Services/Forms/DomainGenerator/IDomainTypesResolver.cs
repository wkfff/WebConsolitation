using System;

using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainGenerator
{
    public interface IDomainTypesResolver
    {
        Type GetRecordType(D_Form_Part part);

        Type GetRequisiteType(D_CD_Templates form, D_Form_Part part, RequisiteKinds requisiteKind);
    }
}