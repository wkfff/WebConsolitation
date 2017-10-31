using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Krista.FM.Domain;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainGenerator
{
    public class FormsDomainGenerator
    {
        /// <summary>
        /// Создает сборку с именем name сожержащую доменные описанные в classes.
        /// </summary>
        /// <param name="name">Имя доменной сборки.</param>
        /// <param name="classes">Коллекция дескрипторов классов.</param>
        public static Assembly Assembly(string name, IEnumerable<ClassDescriptor> classes)
        {
            AssemblyName assemblyName = new AssemblyName
            {
                Name = "Krista.FM.Domain.Gen." + name,
                Version = new Version(1, 0, 0, 0)
            };

            // Создаем новую сборку
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder module = assembly.DefineDynamicModule(assemblyName.Name, true);

            // Создаем классы
            foreach (var classDescriptor in classes)
            {
                TypeBuilder classBuilder = module.DefineType("Krista.FM.Domain." + classDescriptor.Name, TypeAttributes.Public, classDescriptor.BaseType);
                foreach (var property in classDescriptor.Properties)
                {
                    AddProperty(classBuilder, property);
                }

                classBuilder.CreateType();
            }

            return assembly;
        }

        private static void AddProperty(TypeBuilder classBuilder, PropertyDescriptor propDescr)
        {
            var type = propDescr.DataType;

            if (!propDescr.Required)
            {
                // System.Nullable могут быть только типы-значения, объектные типы не могут быть System.Nullable
                if (type.IsValueType)
                {
                    type = Type.GetType("System.Nullable`1[{0}]".FormatWith(type.FullName));
                    if (type == null)
                    {
                        throw new ApplicationException("Тип поля {0} {1} не найден либо не может принимать пустые значения.".FormatWith(propDescr.DataType.Name, propDescr.Name));
                    }
                }
            }

            FieldBuilder backFieldBuilder = classBuilder.DefineField("_" + propDescr.Name, type, FieldAttributes.Private);

            PropertyBuilder prop = classBuilder.DefineProperty(propDescr.Name, PropertyAttributes.HasDefault, type, null);
            MethodBuilder getPropertyBuilder = classBuilder.DefineMethod(
                "get_" + propDescr.Name,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                CallingConventions.HasThis,
                type,
                Type.EmptyTypes);

            ILGenerator getPropGenerator = getPropertyBuilder.GetILGenerator();

            getPropGenerator.Emit(OpCodes.Ldarg_0);
            getPropGenerator.Emit(OpCodes.Ldfld, backFieldBuilder);
            getPropGenerator.Emit(OpCodes.Ret);

            prop.SetGetMethod(getPropertyBuilder);

            MethodBuilder setPropertyBuulder = classBuilder.DefineMethod(
                "set_" + propDescr.Name,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                CallingConventions.HasThis,
                null,
                new[] { type });

            ILGenerator setPropGenerator = setPropertyBuulder.GetILGenerator();

            setPropGenerator.Emit(OpCodes.Ldarg_0);
            setPropGenerator.Emit(OpCodes.Ldarg_1);
            setPropGenerator.Emit(OpCodes.Stfld, backFieldBuilder);
            setPropGenerator.Emit(OpCodes.Ret);

            prop.SetSetMethod(setPropertyBuulder);
        }
    }
}
