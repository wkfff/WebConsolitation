using System;
using System.Data;
using System.Linq;
using System.Text;
using Ext.Net;
using Krista.FM.Common;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine;
using Krista.FM.ServerLibrary;
using Newtonsoft.Json.Linq;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL
{
    public class SectionRequisiteDataService : ISectionRequisiteDataService
    {
        private readonly IScheme scheme;

        public SectionRequisiteDataService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        /// <summary>
        /// Выбирает строку реквизитов отчета.
        /// </summary>
        /// <param name="report">Конкретный отчет.</param>
        /// <param name="section">Раздел отчета.</param>
        /// <param name="requisiteKind">Вид реквизитов.</param>
        public DataTable Get(D_CD_Report report, D_Report_Section section, RequisiteKinds requisiteKind)
        {
            var query = new StringBuilder()
                .Append("select x.ID");

            var requisites = section.RefFormSection.Requisites
                .Where(x => x.IsHeader == (requisiteKind == RequisiteKinds.Header))
                .ToList();

            foreach (var requisite in requisites)
            {
                query.Append(", x.").Append(requisite.InternalName.ToUpper());
            }

            var suffix = requisiteKind == RequisiteKinds.Header ? "rh" : "rf";
            query
                .Append(" from ")
                .Append(ScriptingUtils.GetSectionTableName(report.RefForm.InternalName, section.RefFormSection.InternalName, report.RefForm.FormVersion, suffix))
                .Append(" x where x.ID = ?");

            DataTable dt;
            using (var db = scheme.SchemeDWH.DB)
            {
                dt = (DataTable)db.ExecQuery(
                    query.ToString(),
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("ID", report.ID));
            }

            return dt;
        }

        public void Save(D_CD_Report report, D_Report_Section section, RequisiteKinds requisiteKind, JsonObject json)
        {
            var requisites = section.RefFormSection.Requisites
                .Where(x => x.IsHeader == (requisiteKind == RequisiteKinds.Header))
                .ToList();

            var suffix = requisiteKind == RequisiteKinds.Header ? "rh" : "rf";
            var query = new StringBuilder();
            query.Append("update ")
                .Append(ScriptingUtils.GetSectionTableName(report.RefForm.InternalName, section.RefFormSection.InternalName, report.RefForm.FormVersion, suffix))
                .Append(" set ");
            IDbDataParameter[] prms = new IDbDataParameter[requisites.Count() + 1];
            var colIndex = 0;
            foreach (var requisite in requisites)
            {
                query.Append(requisite.InternalName).Append(" = ?,");
                prms[colIndex] = new DbParameterDescriptor(requisite.InternalName, CreateTypeInstance(requisite));
                colIndex++;
            }

            prms[requisites.Count()] = new DbParameterDescriptor("ID", 0);

            query.RemoveLastChar();
            query.Append(" where ID = ?");

            using (var db = scheme.SchemeDWH.DB)
            {
                JArray updated = (JArray)json["Updated"];
                foreach (var row in updated)
                {
                    for (int i = 0; i < requisites.Count(); i++)
                    {
                        prms[i].Value = ((JValue)row[requisites[i].InternalName.ToUpper()]).Value ?? DBNull.Value;
                    }

                    prms[requisites.Count()].Value = report.ID;

                    db.ExecQuery(query.ToString(), QueryResultTypes.NonQuery, prms);
                }
            }
        }

        public void Save(D_CD_Report report, D_Report_Section section, RequisiteKinds requisiteKind, DataRow row)
        {
            var requisites = section.RefFormSection.Requisites
                .Where(x => x.IsHeader == (requisiteKind == RequisiteKinds.Header))
                .ToList();

            var suffix = requisiteKind == RequisiteKinds.Header ? "rh" : "rf";
            var query = new StringBuilder();
            query.Append("update ")
                .Append(ScriptingUtils.GetSectionTableName(report.RefForm.InternalName, section.RefFormSection.InternalName, report.RefForm.FormVersion, suffix))
                .Append(" set ");
            IDbDataParameter[] prms = new IDbDataParameter[requisites.Count() + 1];
            var colIndex = 0;
            foreach (var requisite in requisites)
            {
                query.Append(requisite.InternalName).Append(" = ?,");
                prms[colIndex] = new DbParameterDescriptor(requisite.InternalName, CreateTypeInstance(requisite));
                colIndex++;
            }

            prms[requisites.Count()] = new DbParameterDescriptor("ID", 0);

            query.RemoveLastChar();
            query.Append(" where ID = ?");

            using (var db = scheme.SchemeDWH.DB)
            {
                int i = 0;
                foreach (var requisite in requisites)
                {
                    prms[i].Value = row[requisite.InternalName];
                    i++;
                }

                prms[requisites.Count()].Value = report.ID;

                db.ExecQuery(query.ToString(), QueryResultTypes.NonQuery, prms);
            }
        }

        private object CreateTypeInstance(D_Form_Requisites requisite)
        {
            if (requisite.DataType == "System.String")
            {
                return String.Empty;
            }

            return Activator.CreateInstance(Type.GetType(requisite.DataType));
        }
    }
}
