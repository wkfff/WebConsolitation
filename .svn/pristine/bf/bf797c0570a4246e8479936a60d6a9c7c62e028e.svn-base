using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Ext.Net;

using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainGenerator;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine;

using Newtonsoft.Json.Linq;

using NHibernate.Criterion;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL
{
    public class ReportSectionDataService : IReportSectionDataService
    {
        private readonly IRepository<D_Report_Row> reportRowRepository;
        private readonly IRepository<D_Form_TableRow> formRowRepository;
        private readonly IDomainTypesResolver typeResolver;
        private readonly IReportDataRepository repository;

        public ReportSectionDataService(IRepository<D_Report_Row> reportRowRepository, IRepository<D_Form_TableRow> formRowRepository, IDomainTypesResolver typeResolver, IReportDataRepository repository)
        {
            this.reportRowRepository = reportRowRepository;
            this.formRowRepository = formRowRepository;
            this.typeResolver = typeResolver;
            this.repository = repository;
        }

        public IList<IRecord> GetAll(D_CD_Report report, string sectionCode)
        {
            var formSection = report.RefForm.Parts.FirstOrDefault(x => x.Code.ToUpper() == sectionCode.ToUpper());
            if (formSection == null)
            {
                throw new ReportDataAccessException("Внутреннее имя раздела \"{0}\" не найдено.".FormatWith(sectionCode));
            }

            var reportSection = report.Sections.FirstOrDefault(x => x.RefReport == report && x.RefFormSection == formSection);
            if (reportSection == null)
            {
                throw new ReportDataAccessException("Раздел данных отчета \"{0}\" не найден.".FormatWith(sectionCode));
            }

            return repository.FindAll(
                typeResolver.GetRecordType(formSection),
                new EqPropertyExpression("RefSection.ID", new ConstantProjection(reportSection.ID)));
        }

        public IRecord CreateRecord(D_CD_Report report, string sectionCode)
        {
            var formSection = report.RefForm.Parts.FirstOrDefault(x => x.Code.ToUpper() == sectionCode.ToUpper());
            if (formSection == null)
            {
                throw new ReportDataAccessException("Внутреннее имя раздела \"{0}\" не найдено.".FormatWith(sectionCode));
            }

            var reportSection = report.Sections.FirstOrDefault(x => x.RefReport == report);
            if (reportSection == null)
            {
                throw new ReportDataAccessException("Раздел данных отчета \"{0}\" не найден.".FormatWith(sectionCode));
            }

            var obj = Activator.CreateInstance(typeResolver.GetRecordType(formSection));

            return new ReportDataRecord(obj, true);
        }

        public void Save(D_CD_Report report, D_Form_Part formSection, IEnumerable<IRecord> records)
        {
            foreach (var record in records)
            {
                switch (record.State)
                {
                    case ReportDataRecordState.Added:
                    case ReportDataRecordState.Modified:
                        repository.Save(record);
                        break;

                    case ReportDataRecordState.Deleted:
                        repository.Delete(record);
                        break;
                }
            }

            repository.CommitChanges();
        }

        public void Save(D_CD_Report report, D_Form_Part formSection, JsonObject json)
        {
            if (json.ContainsKey("Updated") && ((JArray)json["Updated"]).Count > 0)
            {
                JArray updated = (JArray)json["Updated"];
                foreach (var row in updated)
                {
                    object idVal = ((JValue)row["ID"]).Value;
                    var rec = NHibernateSession.Current.Get("Krista.FM.Domain." + ScriptingUtils.GetSectionTableName(report.RefForm.InternalName, formSection.InternalName, report.RefForm.FormVersion, "rw"), Convert.ToInt32(idVal));

                    // Устанавливаем значения полей записи данных
                    SetRecordValues(formSection, row, rec);

                    NHibernateSession.Current.Save(rec);
                }
            }

            if (json.ContainsKey("Created") && ((JArray)json["Created"]).Count > 0)
            {
                JArray created = (JArray)json["Created"];
                
                // Инициализируем тип записи данных
                var assemblyName = "Krista.FM.Domain.Gen.{0}.{1}".FormatWith(formSection.RefForm.InternalName, formSection.RefForm.FormVersion);
                var typeName = "Krista.FM.Domain." + ScriptingUtils.GetSectionTableName(report.RefForm.InternalName, formSection.InternalName, report.RefForm.FormVersion, "rw");
                var assembly = AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == assemblyName);
                var recType = assembly.GetType(typeName);
                if (recType == null)
                {
                    throw new ApplicationException("Тип записи данных не найден \"{0}\"".FormatWith(typeName));
                }

                var reportSection = report.Sections.First(x => x.RefFormSection == formSection);
                foreach (var row in created)
                {
                    // Создаем родительскую запись
                    var primaryRow = new D_Report_Row();
                    primaryRow.RefSection = reportSection;
                    primaryRow.RefFormRow = formRowRepository.Get(Convert.ToInt32(((JValue)row["METAROWID"]).Value));
                    reportRowRepository.Save(primaryRow);

                    // Создаем дочернюю запись данных
                    var rec = assembly.CreateInstance(typeName);
                    if (rec == null)
                    {
                        throw new ApplicationException("Запись данных с типом \"{0}\" не может быть создана.".FormatWith(typeName));
                    }

                    // Устанавливаем ID родительской записи (связь один к одному)
                    recType.GetProperty("ID")
                        .SetValue(rec, primaryRow.ID, BindingFlags.SetProperty, null, null, CultureInfo.CurrentCulture);

                    // Устанавливаем значения полей записи данных
                    SetRecordValues(formSection, row, rec);

                    NHibernateSession.Current.Save(rec);
                }
            }

            if (json.ContainsKey("Deleted") && ((JArray)json["Deleted"]).Count > 0)
            {
                JArray deleted = (JArray)json["Deleted"];
                foreach (var row in deleted)
                {
                    object idVal = ((JValue)row["ID"]).Value;
                    var rec = NHibernateSession.Current.Get("Krista.FM.Domain." + ScriptingUtils.GetSectionTableName(report.RefForm.InternalName, formSection.InternalName, report.RefForm.FormVersion, "rw"), Convert.ToInt32(idVal));

                    NHibernateSession.Current.Delete(rec);
                }
            }
        }

        public IList<IRecord> GetMultipliesRowsTemplates(D_Report_Section section)
        {
            var part = section.RefFormSection;
            var rows = part.Rows.Where(x => x.Multiplicity);

            Type recType = typeResolver.GetRecordType(part);
            List<IRecord> records = new List<IRecord>();
            foreach (var row in rows)
            {
                var rec = repository.Create(recType);

                var cells = part.Cells.Where(x => x.RefRow == row && x.DefaultValue != null).ToList();
                foreach (var cell in cells)
                {
                    rec.Set(cell.RefColumn.InternalName, cell.DefaultValue);
                }

                ((D_Report_Row)rec.Value).RefSection = section;
                ((D_Report_Row)rec.Value).RefFormRow = row;
                records.Add(rec);
            }

            return records;
        }

        private static void SetRecordValues(D_Form_Part formSection, JToken row, object rec)
        {
            foreach (D_Form_TableColumn t in formSection.Columns)
            {
                var prop = rec.GetType().GetProperty(t.InternalName);

                var cellVal = row[t.InternalName];
                if (cellVal == null)
                {
                    // Если графы нет в исходном наборе, то пропускаем ее
                    continue;
                }

                object val = ((JValue)cellVal).Value;
                if (val is DBNull)
                {
                    val = null;
                }

                object convertedValue;
                
                Type nullableType = Nullable.GetUnderlyingType(prop.PropertyType);
                if (nullableType != null)
                {
                    convertedValue = val == null ? null : Convert.ChangeType(val, nullableType);
                }
                else
                {
                    convertedValue = Convert.ChangeType(val, prop.PropertyType);
                }

                val = convertedValue;

                prop.SetValue(rec, val, BindingFlags.SetProperty, null, null, CultureInfo.CurrentCulture);
            }
        }
    }
}
