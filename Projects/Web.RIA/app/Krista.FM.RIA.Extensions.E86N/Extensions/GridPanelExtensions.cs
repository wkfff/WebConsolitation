using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Ext.Net;

using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Extensions
{
    public static class GridPanelExtensions
    {
        /// <summary>
        /// Делаем колонку обязательной
        /// </summary>
        public static ColumnBase SetNotNullable(this ColumnBase column)
        {
            column.CustomConfig.Remove(new ConfigItem("AllowBlank", "true"));
            column.Css = null;
            column.Header = column.Header.Replace("<i>", string.Empty).Replace("</i>", string.Empty);
            if (column.Editor.Count > 0)
            {
                if (column.Editor[0] is TextFieldBase)
                {
                    ((TextFieldBase)column.Editor[0]).AllowBlank = false;
                }
            }
            
            return column;
        }

        /// <summary>
        /// Добавление клолнки в грид по метаданным дизайнера
        /// Отличается от коровского тем что там UPPER стоит на dataIndex тем самым данные не отображаются
        /// </summary>
        public static ColumnBase AddGridColumn(this ColumnModel cm, IDataAttribute attribute)
        {
            return cm.AddGridColumn(attribute.Name, attribute.Caption, attribute.Type, attribute.IsNullable ? Mandatory.Nullable : Mandatory.NotNull);
        }

        public static ColumnBase AddGridColumn(this ColumnModel cm, string columnId, string header, DataAttributeTypes attributeType, Mandatory mandatory)
        {
            ColumnBase column = cm.AddColumn(columnId, columnId, header, attributeType, mandatory);
            if (mandatory == Mandatory.Nullable)
            {
                column.SetNullable();
            }

            return column;
        }

        /// <summary>
        /// Добавление и настройка колонки по свойству модели
        /// </summary>
        public static ColumnBase AddColumn<T>(this ColumnModel cm, Expression<Func<T>> expr, bool readOnly = false)
        {
            return cm.AddColumn((PropertyInfo)((MemberExpression)expr.Body).Member, readOnly);
        }

        /// <summary>
        /// Добавляет столбец с именем и описанием из атрибутов модели
        /// </summary>
        /// <typeparam name="T">Несущественный параметр</typeparam>
        /// <param name="cm">Обьект от которого вызывается функция</param>
        /// <param name="expr">Лямбда выражение, указывающее на поле модели</param>
        /// <param name="attributeType">Тип отображаетмых данных</param>
        /// <returns>Возвращает данный ColumnBase</returns>
        public static ColumnBase AddColumn<T>(this ColumnModel cm, Expression<Func<T>> expr, DataAttributeTypes attributeType)
        {
            return cm.AddColumn(UiBuilders.NameOf(expr), UiBuilders.DescriptionOf(expr), attributeType);
        }

        public static ColumnBase AddColumn(this ColumnModel cm, PropertyInfo info, bool readOnly = false)
        {
            ColumnBase column;
            var attributes = info.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length != 0)
            {
                var descriptionAttribute = attributes[0] as DescriptionAttribute;

                if (descriptionAttribute != null)
                {
                    if (info.PropertyType == typeof(string))
                    {
                        column = cm.AddColumn(info.Name, descriptionAttribute.Description, DataAttributeTypes.dtString);
                        if (!readOnly)
                        {
                            column.SetEditableString();
                        }

                        return column;
                    }

                    if (info.PropertyType == typeof(double) || info.PropertyType == typeof(decimal))
                    {
                        column = cm.AddColumn(info.Name, descriptionAttribute.Description, DataAttributeTypes.dtDouble);
                        if (!readOnly)
                        {
                            column.SetEditableDouble(2);
                        }

                        return column;
                    }

                    if (info.PropertyType == typeof(DateTime))
                    {
                        column = cm.AddColumn(info.Name, descriptionAttribute.Description, DataAttributeTypes.dtDateTime);
                        if (!readOnly)
                        {
                            column.SetEditableDate();
                        }

                        return column;
                    }

                    if (info.PropertyType == typeof(int))
                    {
                        column = cm.AddColumn(info.Name, descriptionAttribute.Description, DataAttributeTypes.dtInteger);
                        if (!readOnly)
                        {
                            column.SetEditableInteger();
                        }

                        return column;
                    }

                    if (info.PropertyType == typeof(bool))
                    {
                        column = cm.AddColumn(info.Name, descriptionAttribute.Description, DataAttributeTypes.dtBoolean);
                        if (!readOnly)
                        {
                            column.SetEditableBoolean();
                        }

                        return column;
                    }
                }

                return null;
            }

            IDataAttribute dataAttribute = null;

            attributes = info.GetCustomAttributes(typeof(DataBaseBindingFieldAttribute), false);
            if (attributes.Any())
            {
                dataAttribute = ((DataBaseBindingFieldAttribute)attributes[0]).Info;
            }
            else
            {
                attributes = info.GetCustomAttributes(typeof(DataBaseBindingTableAttribute), false);
                if (attributes.Any())
                {
                    dataAttribute = ((DataBaseBindingTableAttribute)attributes[0]).GetInfo(info.Name);
                }
            }

            if (dataAttribute != null)
            {
                column = cm.AddColumn(info.Name, dataAttribute.Caption, dataAttribute.Type);
                ConfigColumn(column, dataAttribute, readOnly);
                return column;
            }

            return null;
        }

        private static void ConfigColumn(ColumnBase column, IDataAttribute field, bool readOnly = false)
        {
            if (field.IsNullable)
            {
                column.SetNullable();
            }

            if (!readOnly && !field.IsReadOnly)
            {
                switch (field.Type)
                {
                    case DataAttributeTypes.dtBoolean:
                        column.SetEditableBoolean();
                        break;
                    case DataAttributeTypes.dtDouble:
                        column.SetEditableDouble(field.Scale);
                        break;
                    case DataAttributeTypes.dtInteger:
                        column.SetEditableInteger();
                        break;
                    case DataAttributeTypes.dtDate:
                    case DataAttributeTypes.dtDateTime:
                        column.SetEditableDate();
                        break;
                    default:
                        column.SetEditableString();
                        break;
                }

                if (field.Size > 0)
                {
                    column.SetMaxLengthEdior(field.Size);
                }

                if (!field.IsNullable)
                {
                    column.Editor[0].SetValue(field.DefaultValue);
                }
            }
        }
    }
}
