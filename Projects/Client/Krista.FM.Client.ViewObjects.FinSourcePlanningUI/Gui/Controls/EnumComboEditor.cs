using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Design;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public class EnumComboEditor : UltraComboEditor
    {
        private Type enumType;

        public EnumComboEditor()
        {
        }

        private void OnEnumTypeChange()
        {
            if (EnumType == null)
                return;

            if (!enumType.IsEnum)
                throw new ArgumentException(String.Format("Тип {0} не является перечислением.", enumType.FullName));

            this.Items.Clear();

            foreach (FieldInfo fi in enumType.GetFields())
            {
                if (fi.IsLiteral)
                {
                    DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(
                        fi, typeof(DescriptionAttribute));

                    Infragistics.Win.ValueListItem valueListItem = new Infragistics.Win.ValueListItem();
                    valueListItem.DataValue = fi.GetRawConstantValue();
                    valueListItem.DisplayText = da != null ? da.Description : fi.Name;
                    this.Items.Add(valueListItem);
                }
            }
            this.SelectedIndex = 0;
        }

        [Editor(typeof(EnumTypeEditor), typeof(UITypeEditor))]
        public Type EnumType
        {
            get
            {
                return enumType;
            }
            set
            {
                enumType = value;
                OnEnumTypeChange();
            }
        }
    }
}
