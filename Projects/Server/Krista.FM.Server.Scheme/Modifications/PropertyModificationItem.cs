using System;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    internal sealed class PropertyModificationItem : ModificationItem
    {
        /// <summary>
        /// Тип модифицируемого свойства
        /// </summary>
        //private Type propertyType;

        /// <summary>
        /// Наименование модифицируемого свойства
        /// </summary>
        private readonly string propertyName;


        public PropertyModificationItem(string name, object fromObject, object toObject, ModificationItem parent)
            : base(ModificationTypes.Modify, name, fromObject, toObject, parent)
        {
            propertyName = name;
        }

        protected override void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            object properyValue = Parent.FromObject.GetType().InvokeMember(propertyName, System.Reflection.BindingFlags.GetProperty, null, Parent.FromObject, null);
            if (propertyName != "DeveloperDescription")
            {
                Trace.Write(String.Format("Изменение свойства {0} c \"{1}\" на \"{2}\" ", propertyName, properyValue, ToObject));
            }
            Parent.FromObject.GetType().InvokeMember(propertyName, System.Reflection.BindingFlags.SetProperty, null, Parent.FromObject, new object[] { ToObject });
            isAppliedPartially = false;
        }
    }
}
