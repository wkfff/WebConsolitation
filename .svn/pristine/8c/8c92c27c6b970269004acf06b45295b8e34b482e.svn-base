using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Krista.FM.Client.MDXExpert.Data
{
    /// <summary>
    /// Набор иерархий.
    /// </summary>
    public class FieldSetCollection : PivotObject, IEnumerable
    {
        private List<FieldSet> items;

        public List<FieldSet> Items
        {
            get { return items; }
        }

        public FieldSet this[int index]
        {
            get
            {
                return items[index];
            }
        }

        public FieldSet this[string fieldSetName]
        {
            get
            {
                foreach (FieldSet fieldSet in items)
                {
                    if (fieldSet.UniqueName == fieldSetName)
                    {
                        return fieldSet;
                    }
                }
                return null;
            }

            
        }

        /// <summary>
        /// Содержит ли в себе меры
        /// </summary>
        public bool IsContainMeasures
        {
            get
            {
                return (this.GetFieldSetByName("[Measures]") != null);
            }
        }

        public FieldSetCollection(Client.MDXExpert.Data.PivotData pivotData)
        {
            items = new List<FieldSet>();
            this._parentPivotData = pivotData; 
        }
        
        public FieldSet GetLastItem()
        {
            return this.Count > 0 ? this[this.Count - 1] : null;
        }

        public int Count
        {
            get { return items.Count; }
        }

        public int IndexOf(FieldSet fs)
        {
            if (fs == null)
                return -1;
            return this.items.IndexOf(fs);
        }

        public FieldSet Add(string fsName, string fsCaption)
        {
            FieldSet fs = new FieldSet(this.ParentPivotData, fsName, fsCaption);
            fs.ParentCollection = this;
            items.Add(fs);
            return fs;
        }

        public FieldSet Add(FieldSet fs)
        {
            items.Add(fs);
            fs.ParentCollection = this;

            return fs;
        }

        public FieldSet CopyFieldSet(FieldSet fs, AxisType axisType, bool copyTotalsVisible)
        {
            FieldSet newFS = new FieldSet(this.ParentPivotData, fs.UniqueName, fs.Caption);
            
            if (copyTotalsVisible)
                newFS.IsVisibleTotals = fs.IsVisibleTotals;

            newFS.AxisType = axisType;

            foreach (PivotField field in fs.Fields)
            {
                newFS.Fields.Add(new PivotField(newFS, this.ParentPivotData, field.UniqueName, field.Caption));
                if (copyTotalsVisible) 
                    newFS.IsVisibleTotals = field.IsVisibleTotal;
            }

            newFS.MemberNames = fs.MemberNames;
            newFS.ParentCollection = this;
            items.Add(newFS);
            return newFS;
        }

        public FieldSet CopyFieldSet(FieldSet fs, AxisType axisType)
        {
            return CopyFieldSet(fs, axisType, true);
        }

        public void Remove(string uniqueName)
        {
            for (int i = 0; i > this.items.Count; i++)
            {
                if (items[i].UniqueName == uniqueName)
                    this.Remove(i);
            }
        }

        public void Remove(int index)
        {
            this.items.RemoveAt(index);
        }

        public void Remove(FieldSet fieldSet)
        {
            this.items.Remove(fieldSet);
        }

        /// <summary>
        /// Существует ли хотя бы у одного измерения сортировка
        /// </summary>
        public bool IsSorted
        {
            get
            {
                foreach (FieldSet fieldSet in items)
                {
                    if (fieldSet.IsSorted)
                        return true;
                }
                return false;
            }
        }

        #region
        /// <summary>
        /// Получение набора полей по имени
        /// </summary>
        /// <param name="uniqueName">юникнейм набора полей</param>
        /// <returns>набор полей, null - если набор полей не найден</returns>
        public FieldSet GetFieldSetByName(string uniqueName)
        {
            foreach (FieldSet fieldSet in items)
            {
                if (uniqueName == fieldSet.UniqueName)
                {
                    return fieldSet;
                }
            }
            return null;
        }

        public PivotField GetFieldByName(string uniqueName)
        {
            foreach (FieldSet fieldSet in items)
            {
                foreach (PivotField field in fieldSet.Fields)
                {
                    if (uniqueName == field.UniqueName)
                    {
                        return field;
                    }
                }
            }
            return null;

        }

        /// <summary>
        /// Проверка существует ли такой набор полей
        /// </summary>
        /// <param name="fieldsetName">юникнейм набора</param>
        /// <returns>true - если существует</returns>
        public bool FieldSetIsPreset(string fieldsetName)
        {
            foreach (FieldSet fieldSet in items)
            {
                if (fieldSet.UniqueName == fieldsetName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверка существует ли такой набор полей
        /// </summary>
        /// <param name="pivotObject">pivotObject</param>
        /// <returns>true - если существует</returns>
        public bool FieldSetIsPreset(PivotObject pivotObject)
        {
            foreach (FieldSet fieldSet in items)
            {
                if (fieldSet.Equals(pivotObject))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверка, есть ли такое поле
        /// </summary>
        /// <param name="fieldName">юникнейм поля</param>
        /// <returns>true - если есть</returns>
        public bool FieldIsPreset(PivotObject pivotObject)
        {
            return this.FieldIsPreset(((PivotField)pivotObject).UniqueName);
        }

        /// <summary>
        /// Проверка, есть ли такое поле
        /// </summary>
        /// <param name="fieldName">юникнейм поля</param>
        /// <returns>true - если есть</returns>
        public bool FieldIsPreset(string fieldName)
        {
            foreach (FieldSet fieldSet in items)
            {
                if (fieldSet.FieldIsPresent(fieldName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверка, есть ли такой мембер 
        /// </summary>
        /// <returns>true - если есть</returns>
        public bool MemberIsPreset(string memberName)
        {
            foreach (FieldSet fs in items)
            {
                if ((fs.MemberNames != null) && fs.MemberNames.OuterXml.Contains(memberName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверка, есть такой элемент
        /// </summary>
        /// <param name="objectName">юникнейм элемента</param>
        /// <returns>true - если есть</returns>
        public bool ObjectIsPreset(string objectName)
        {
            return ((FieldSetIsPreset(objectName)) || (FieldIsPreset(objectName)) || (MemberIsPreset(objectName)));
        }

        /// <summary>
        /// Обновить сортировку у всез измерений оси, влкючая уровни
        /// </summary>
        public void RefreshSortType(PivotObject sender)
        {
            foreach (FieldSet fieldSet in items)
            {
                fieldSet.RefreshFieldSortType(sender);
                if (fieldSet != sender)
                    fieldSet.SetSortTypeWithoutRefresh(SortType.None);
            }
        }

        #endregion
        
        public PivotField AddField(string fieldSetName, string fieldSetCaption, string fieldName, string fieldCaption)
        {
            PivotField f = new PivotField(this.ParentPivotData, fieldName, fieldCaption);

            if (FieldSetIsPreset(fieldSetName))
            {
                if (!FieldIsPreset(fieldName))
                {
                    this[fieldSetName].Fields.Add(f);
                }
            }
            else
            {
                Add(fieldSetName, fieldSetCaption).Fields.Add(f);
            }

            f.ParentFieldSet = this[fieldSetName];

            return f;
        }

        public void Clear()
        {
            items.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
