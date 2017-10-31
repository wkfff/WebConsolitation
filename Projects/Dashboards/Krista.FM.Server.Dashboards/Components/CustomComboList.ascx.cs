using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.Components
{
    /// <summary>
    /// Элемент выпадающий список
    /// </summary>
    public partial class CustomComboList : UserControl
    {
//        public delegate void SelectedIndexChanged(object sender, EventArgs e);
//        private SelectedIndexChanged _OnSelectIndexChanged = null;
//
//        public event SelectedIndexChanged SelectIndexChanged
//        {
//            add { _OnSelectIndexChanged += value; }
//            remove { _OnSelectIndexChanged -= value; }
//        }
//      
        /// <summary>
        /// Имя параметра
        /// </summary>
//        public string ParamText
//        {
//            get { return ParamLabel.Text; }
//            set
//            {
//                ParamLabel.Text = value;
//            }
//        }

        /// <summary>
        /// Номер выбранного элемента списка
        /// </summary>
        public int SelectedIndex
        {
            get { return DropDownList.SelectedIndex; }
            set { DropDownList.SelectedIndex = value; }
        }

        /// <summary>
        /// Значение выбранного элемента списка
        /// </summary>
        public string SelectedValue
        {
            get { return DropDownList.SelectedValue; }
        }

        /// <summary>
        /// Выбрать элемент списка по значению
        /// </summary>
        /// <param name="value"></param>
        public void SelectValue(string value)
        {
            for(int i = 0; i < DropDownList.Items.Count; i++)
            {
                if (DropDownList.Items[i].Value == value)
                {
                    DropDownList.SelectedIndex = i;
                    return;
                }
            }
        }

        /// <summary>
        /// Заполнить список годами
        /// </summary>
        /// <param name="lowValue">нижний предел</param>
        /// <param name="highValue">верхний предел</param>
        public void FillYearValues(int lowValue, int highValue)
        {
            DropDownList.Items.Clear();
            for (int i = lowValue; i <= highValue; i++)
            {
                DropDownList.Items.Add(i.ToString());
            }
        }

        /// <summary>
        /// Заполнить список месяцами
        /// </summary>
        public void FillMonthValues()
        {
            DropDownList.Items.Clear();
            for (int i = 1; i <= 12; i++)
            {
                DropDownList.Items.Add(ToUpperFirstSymbol(CRHelper.RusMonth(i)));
            }
        }

        /// <summary>
        /// Заполнить список федеральными округами
        /// </summary>
        public void FillFONames(Collection<string> foCollection)
        {
            DropDownList.Items.Clear();
            DropDownList.Items.Add("Все федеральные округа");
            for (int i = 0; i < foCollection.Count; i++)
            {
                DropDownList.Items.Add(foCollection[i]);
            }
        }

        /// <summary>
        /// Заполнить список субъектами
        /// </summary>
        public void FillSubjectNames(Collection<string> subjectCollection)
        {
            DropDownList.Items.Clear();
            for (int i = 0; i < subjectCollection.Count; i++)
            {
                DropDownList.Items.Add(subjectCollection[i]);
            }
        }

        /// <summary>
        /// Заполнить список видами доходов
        /// </summary>
        public void FillKDNames(Dictionary<string, string> kdDictionary)
        {
            DropDownList.Items.Clear();

            foreach (string key in kdDictionary.Keys)
            {
                DropDownList.Items.Add(key.TrimEnd('_'));
            }
        }

        /// <summary>
        /// Заполнить список значениями из коллекции
        /// </summary>
        public void FillValues(Collection<string> nameCollection)
        {
            DropDownList.Items.Clear();
            for (int i = 0; i < nameCollection.Count; i++)
            {
                DropDownList.Items.Add(nameCollection[i]);
            }
        }

        /// <summary>
        /// Сделать строку с заглавной буквы
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string ToUpperFirstSymbol(string source)
        {
            return source == string.Empty ? 
                             string.Empty :
                             source[0].ToString().ToUpper() + source.Remove(0, 1);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

//        protected void DropDownList_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            _OnSelectIndexChanged.Invoke(sender, e);
//        }
    }
}