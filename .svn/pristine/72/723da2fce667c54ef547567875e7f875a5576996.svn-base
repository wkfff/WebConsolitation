using System;
using System.Linq;

using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainGenerator
{
    public class DomainTypesResolver : IDomainTypesResolver
    {
        public Type GetRecordType(D_Form_Part part)
        {
            var assemblyName = "Krista.FM.Domain.Gen.{0}.{1}".FormatWith(part.RefForm.InternalName, part.RefForm.FormVersion);
            var typeName = "Krista.FM.Domain." + ScriptingUtils.GetSectionTableName(part.RefForm.InternalName, part.InternalName, part.RefForm.FormVersion, "rw");
            var assembly = AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == assemblyName);
            var recType = assembly.GetType(typeName);
            if (recType == null)
            {
                throw new ApplicationException("Тип записи данных не найден \"{0}\"".FormatWith(typeName));
            }

            return recType;
        }

        public Type GetRequisiteType(D_CD_Templates form, D_Form_Part part, RequisiteKinds requisiteKind)
        {
            var suffix = requisiteKind == RequisiteKinds.Header ? "rh" : "rf";
            string tableName = part == null
                                   ? ScriptingUtils.GetReportTableName(form.InternalName, form.FormVersion, suffix)
                                   : ScriptingUtils.GetSectionTableName(form.InternalName, part.InternalName, form.FormVersion, suffix);

            var assemblyName = "Krista.FM.Domain.Gen.{0}.{1}".FormatWith(form.InternalName, form.FormVersion);
            var typeName = "Krista.FM.Domain." + tableName;
            var assembly = AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == assemblyName);
            var recType = assembly.GetType(typeName);
            if (recType == null)
            {
                throw new ApplicationException("Тип записи данных не найден \"{0}\"".FormatWith(typeName));
            }

            return recType;
        }
    }
}
