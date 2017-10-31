using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainGenerator;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainStore
{
    /// <summary>
    /// Создает, Хранит и Пердоставляет доступ к доменным сборкам.
    /// Хранятся доменные сборки в памяти.
    /// </summary>
    public class DomainFormsAssembliesStore : IDomainFormsAssembliesStore
    {
        private static readonly Dictionary<string, Assembly> Store = new Dictionary<string, Assembly>();

        /// <summary>
        /// Пытается найти динамическую сборку в хранилие доменных динамических сборок.
        /// Это необходимо для того, чтобы NHibernate смог разрешить ссылки на сгенерированные доменные типы.
        /// </summary>
        public static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("Krista.FM.Domain.Gen"))
            {
                return Store.Values.FirstOrDefault(assembly => assembly.FullName == args.Name);
            }

            return null;
        }

        /// <summary>
        /// Создает и добавляет в хранилище новую доменную сборку для указанной формы.
        /// </summary>
        /// <param name="form">Форма для которой нужно создать доменную сборку.</param>
        public void Register(D_CD_Templates form)
        {
            string assemblyKey = GetAssemblyKey(form);
            if (Store.ContainsKey(assemblyKey))
            {
                throw new InvalidOperationException("Для формы {0} доменная сборка уже зарегистрирована.".FormatWith(assemblyKey));
            }

            var classes = GetClassDescriptorsFromForm(form);

            Store.Add(assemblyKey, FormsDomainGenerator.Assembly(assemblyKey, classes));
        }

        /// <summary>
        /// Возвращает все зарегистрированные доменные сборки.
        /// </summary>
        public Assembly[] GetAllAssemblies()
        {
            return Store.Values.ToArray();
        }

        private static List<ClassDescriptor> GetClassDescriptorsFromForm(D_CD_Templates form)
        {
            var classes = new List<ClassDescriptor>();

            // Реквизиты заголовочной части
            if (form.Requisites.Any(x => x.IsHeader))
            {
                var classDescriptor = new RequisiteClassDescriptor
                {
                    Name = ScriptingUtils.GetReportTableName(form.InternalName, form.FormVersion, "rh"),
                    Properties = { new PropertyDescriptor { Name = "ID", DataType = typeof(int), Required = true } }
                };
                foreach (var req in form.Requisites.Where(x => x.IsHeader))
                {
                    classDescriptor.Properties.Add(
                        new PropertyDescriptor
                        {
                            Name = req.InternalName,
                            DataType = Type.GetType(req.DataType),
                            Required = req.Required
                        });
                }

                classes.Add(classDescriptor);
            }

            // Разделы
            foreach (var part in form.Parts)
            {
                // Реквизиты заголовочной части рздела
                if (part.Requisites.Any(x => x.IsHeader))
                {
                    var classDescriptor = new RequisiteClassDescriptor
                    {
                        Name = ScriptingUtils.GetSectionTableName(form.InternalName, part.InternalName, form.FormVersion, "rh"),
                        Properties = { new PropertyDescriptor { Name = "ID", DataType = typeof(int), Required = true } }
                    };
                    foreach (var req in part.Requisites.Where(x => x.IsHeader))
                    {
                        classDescriptor.Properties.Add(
                            new PropertyDescriptor
                            {
                                Name = req.InternalName,
                                DataType = Type.GetType(req.DataType),
                                Required = req.Required
                            });
                    }

                    classes.Add(classDescriptor);
                }

                // Таблица раздела
                var sectionClsDescr = new RowClassDescriptor
                {
                    Name = ScriptingUtils.GetSectionTableName(form.InternalName, part.InternalName, form.FormVersion, "rw"),
                    Properties = { new PropertyDescriptor { Name = "ID", DataType = typeof(int), Required = true } }
                };
                foreach (var column in part.Columns)
                {
                    sectionClsDescr.Properties.Add(
                        new PropertyDescriptor
                        {
                            Name = column.InternalName,
                            DataType = Type.GetType(column.DataType),
                            Required = column.Required
                        });
                }

                classes.Add(sectionClsDescr);

                // Реквизиты заключительной части раздела
                if (part.Requisites.Any(x => !x.IsHeader))
                {
                    var classDescriptor = new RequisiteClassDescriptor
                    {
                        Name = ScriptingUtils.GetSectionTableName(form.InternalName, part.InternalName, form.FormVersion, "rf"),
                        Properties = { new PropertyDescriptor { Name = "ID", DataType = typeof(int), Required = true } }
                    };
                    foreach (var req in part.Requisites.Where(x => !x.IsHeader))
                    {
                        classDescriptor.Properties.Add(
                            new PropertyDescriptor
                            {
                                Name = req.InternalName,
                                DataType = Type.GetType(req.DataType),
                                Required = req.Required
                            });
                    }

                    classes.Add(classDescriptor);
                }
            }

            // Реквизиты заключительной части
            if (form.Requisites.Any(x => !x.IsHeader))
            {
                var classDescriptor = new RequisiteClassDescriptor
                {
                    Name = ScriptingUtils.GetReportTableName(form.InternalName, form.FormVersion, "rf"),
                    Properties = { new PropertyDescriptor { Name = "ID", DataType = typeof(int), Required = true } }
                };
                foreach (var req in form.Requisites.Where(x => !x.IsHeader))
                {
                    classDescriptor.Properties.Add(
                        new PropertyDescriptor
                        {
                            Name = req.InternalName,
                            DataType = Type.GetType(req.DataType),
                            Required = req.Required
                        });
                }

                classes.Add(classDescriptor);
            }

            return classes;
        }

        private string GetAssemblyKey(D_CD_Templates form)
        {
            return form.InternalName + '.' + form.FormVersion;
        }
    }
}
