using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine
{
    public class FormScriptingEngine : IFormScriptingEngine
    {
        private readonly ScriptingEngineImpl impl;
        private readonly IDatabaseObjectHashNameResolver hashNameResolver;

        public FormScriptingEngine(ScriptingEngineFactory scriptingEngineFactory, IDatabaseObjectHashNameResolver hashNameResolver)
        {
            this.impl = scriptingEngineFactory.Create();
            this.hashNameResolver = hashNameResolver;
        }

        /// <summary>
        /// Создает структуры в базе данных для хранения данных формы.
        /// </summary>
        /// <param name="form">Мета-описания структуры формы.</param>
        /// <param name="version">Версия формы.</param>
        public IList<string> Create(D_CD_Templates form, int version)
        {
            var script = new List<string>();

            script.AddRange(CreateReportRequisitesTables(form, version));

            foreach (var formPart in form.Parts)
            {
                script.AddRange(CreateSectionTable(formPart, version));
            }

            return script;
        }

        /// <summary>
        /// Уданяет структуры из базы данных для для указанной формы.
        /// АХТУНГ! Этод метод только для внутреннего использования (отладки), 
        /// в реальном мире структуры и данные формы не должны удаляться.
        /// </summary>
        /// <param name="form">Мета-описания структуры формы.</param>
        public IList<string> Drop(D_CD_Templates form)
        {
            var script = new List<string>();

            foreach (var formPart in form.Parts)
            {
                script.AddRange(DropSectionTable(formPart, form.FormVersion));
            }

            script.AddRange(DropReportTable(form, form.FormVersion));

            return script;
        }

        /// <summary>
        /// Создает таблицы для хранения реквизитов отчета.
        /// </summary>
        private IEnumerable<string> CreateReportRequisitesTables(D_CD_Templates form, int version)
        {
            List<string> script = new List<string>();
            script.AddRange(
                CreateRequisiteTable(
                    form.Requisites.Where(x => x.IsHeader).ToList(),
                    RequisiteKinds.Header,
                    RequisiteClass.Report,
                    version));

            script.AddRange(
                CreateRequisiteTable(
                    form.Requisites.Where(x => !x.IsHeader).ToList(),
                    RequisiteKinds.Footer,
                    RequisiteClass.Report,
                    version));

            return script;
        }

        private IEnumerable<string> DropReportTable(D_CD_Templates form, int version)
        {
            List<string> script = new List<string>();
            script.AddRange(
                DropRequisiteTable(
                    form.Requisites.Where(x => x.IsHeader).ToList(),
                    RequisiteKinds.Header,
                    RequisiteClass.Report,
                    version));

            script.AddRange(
                DropRequisiteTable(
                    form.Requisites.Where(x => !x.IsHeader).ToList(),
                    RequisiteKinds.Footer,
                    RequisiteClass.Report,
                    version));

            return script;
        }

        /// <summary>
        /// Создает таблицы для хранения реквизитов и строк раздела отчета.
        /// </summary>
        /// <param name="formPart">Раздел формы.</param>
        /// <param name="version">Версия формы.</param>
        private IEnumerable<string> CreateSectionTable(D_Form_Part formPart, int version)
        {
            var script = new List<string>();

            script.AddRange(
                CreateRequisiteTable(
                    formPart.Requisites.Where(x => x.IsHeader).ToList(), 
                    RequisiteKinds.Header, 
                    RequisiteClass.Section, 
                    version));
            
            script.AddRange(
                CreateRequisiteTable(
                    formPart.Requisites.Where(x => !x.IsHeader).ToList(), 
                    RequisiteKinds.Footer, 
                    RequisiteClass.Section, 
                    version));

            script.AddRange(CreateRowsTable(formPart, version));

            return script;
        }

        private IEnumerable<string> DropSectionTable(D_Form_Part formPart, int version)
        {
            var script = new List<string>();

            script.AddRange(
                DropRequisiteTable(
                    formPart.Requisites.Where(x => x.IsHeader).ToList(),
                    RequisiteKinds.Header,
                    RequisiteClass.Section,
                    version));

            script.AddRange(
                DropRequisiteTable(
                    formPart.Requisites.Where(x => !x.IsHeader).ToList(),
                    RequisiteKinds.Footer,
                    RequisiteClass.Section,
                    version));

            script.AddRange(DropTable(formPart, version));

            return script;
        }

        /// <summary>
        /// Создает табицу для хранения данных реквизитов отчета.
        /// </summary>
        /// <param name="requisites">Реквизиты формы.</param>
        /// <param name="requisiteKind">Вид реквизита.</param>
        /// <param name="requisiteClass">Класс реквизита.</param>
        /// <param name="version">Версия формы.</param>
        private IEnumerable<string> CreateRequisiteTable(IList<D_Form_Requisites> requisites, RequisiteKinds requisiteKind, RequisiteClass requisiteClass, int version)
        {
            List<string> scripts = new List<string>();

            if (requisites.Count == 0)
            {
                return scripts;
            }

            var requisite = requisites[0];
            var form = requisiteClass == RequisiteClass.Section ? requisite.RefPart.RefForm : requisite.RefForm;
            var suffix = requisite.IsHeader ? "rh" : "rf";

            var tableName = requisiteClass == RequisiteClass.Section
                ? ScriptingUtils.GetSectionTableName(form.InternalName, requisite.RefPart.InternalName, version, suffix)
                : ScriptingUtils.GetReportTableName(form.InternalName, version, suffix);

            StringBuilder script = new StringBuilder()
                .Append("create table ")
                .Append(tableName)
                .Append("(");

            script.Append(impl.GetColumnScript("ID", typeof(int), 10, null, false)).Append(',');

            foreach (var req in requisites)
            {
                script.Append(impl.GetColumnScript(req.InternalName, Type.GetType(req.DataType), req.DataTypeSize, req.DataTypeScale, true))
                    .Append(',');
            }

            script.RemoveLastChar();
            
            script.Append(")");

            scripts.Add(script.ToString());

            var primaryKeyName = hashNameResolver.Create(
                "PK" + tableName,
                ObjectTypes.ForeignKeysConstraint);
            scripts.Add("alter table {0} add constraint {1} primary key (ID)".FormatWith(tableName, primaryKeyName));

            var foreignKeyName = hashNameResolver.Create(
                "FK" + tableName + "ID",
                ObjectTypes.ForeignKeysConstraint);
            scripts.Add("alter table {0} add constraint {1} foreign key (ID) references D_CD_Report (ID)".FormatWith(tableName, foreignKeyName));

            return scripts;
        }

        private IEnumerable<string> DropRequisiteTable(IList<D_Form_Requisites> requisites, RequisiteKinds requisiteKind, RequisiteClass requisiteClass, int version)
        {
            List<string> scripts = new List<string>();

            if (requisites.Count == 0)
            {
                return scripts;
            }

            var requisite = requisites[0];
            var form = requisiteClass == RequisiteClass.Section ? requisite.RefPart.RefForm : requisite.RefForm;
            var suffix = requisiteKind == RequisiteKinds.Header ? "rh" : "rf";

            var tableName = requisiteClass == RequisiteClass.Section
                ? ScriptingUtils.GetSectionTableName(form.InternalName, requisite.RefPart.InternalName, version, suffix)
                : ScriptingUtils.GetReportTableName(form.InternalName, version, suffix);

            StringBuilder script = new StringBuilder()
                .Append("drop table ")
                .Append(tableName);

            try
            {
                hashNameResolver.Delete("PK" + tableName, ObjectTypes.ForeignKeysConstraint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка удаления ключа PK{0}: {1}".FormatWith(tableName, e.Message));
            }

            try
            {
                hashNameResolver.Delete("FK" + tableName + "ID", ObjectTypes.ForeignKeysConstraint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка удаления ключа FK{0}ID: {1}".FormatWith(tableName, e.Message));
            }

            return new List<string> { script.ToString() };
        }

        /// <summary>
        /// Создает таблицу для зранения строк раздела отчета.
        /// </summary>
        /// <param name="formPart">Раздел формы.</param>
        /// <param name="version">Версия формы.</param>
        private IEnumerable<string> CreateRowsTable(D_Form_Part formPart, int version)
        {
            List<string> scripts = new List<string>();

            var tableName = ScriptingUtils.GetSectionTableName(formPart.RefForm.InternalName, formPart.InternalName, version, "rw");
            StringBuilder script = new StringBuilder()
                .Append("create table ")
                .Append(tableName)
                .Append("(");

            script.Append(impl.GetColumnScript("ID", typeof(int), 10, null, false)).Append(',');
            
            foreach (var column in formPart.Columns)
            {
                script.Append(impl.GetColumnScript(column.InternalName, Type.GetType(column.DataType), column.DataTypeSize, column.DataTypeScale, true))
                    .Append(',');
            }

            script.RemoveLastChar();

            script.Append(")");

            scripts.Add(script.ToString());

            var primaryKeyName = hashNameResolver.Create(
                "PK" + tableName, 
                ObjectTypes.ForeignKeysConstraint);
            scripts.Add("alter table {0} add constraint {1} primary key (ID)".FormatWith(tableName, primaryKeyName));

            var foreignKeyName = hashNameResolver.Create(
                "FK" + tableName + "ID",
                ObjectTypes.ForeignKeysConstraint);
            scripts.Add("alter table {0} add constraint {1} foreign key (ID) references D_Report_Row (ID)".FormatWith(tableName, foreignKeyName));

            return scripts;
        }

        private IEnumerable<string> DropTable(D_Form_Part formPart, int version)
        {
            var tableName = ScriptingUtils.GetSectionTableName(formPart.RefForm.InternalName, formPart.InternalName, version, "rw");
            StringBuilder script = new StringBuilder()
                .Append("drop table ")
                .Append(tableName);

            try
            {
                hashNameResolver.Delete("PK" + tableName, ObjectTypes.ForeignKeysConstraint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка удаления ключа PK{0}: {1}".FormatWith(tableName, e.Message));
            }

            try
            {
                hashNameResolver.Delete("FK" + tableName + "ID", ObjectTypes.ForeignKeysConstraint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка удаления ключа FK{0}ID: {1}".FormatWith(tableName, e.Message));
            }

            return new List<string> { script.ToString() };
        }
    }
}
