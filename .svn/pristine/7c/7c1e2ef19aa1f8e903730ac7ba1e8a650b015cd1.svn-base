using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Krista.FM.Client.MDXExpert.Data
{
    public delegate void MemberPropertiesEventHandler();


    public class MemberProperties
    {
        #region Поля

        private List<string> allProperties;
        private List<string> visibleProperties;
//        private MemberPropertiesDisplayType displayType;

        #endregion

        #region Свойства

        public List<string> AllProperties
        {
            get
            {
                return allProperties;
            }
            set
            {
                allProperties = value;
            }
        }

        public List<string> VisibleProperties
        {
            get
            {
                return visibleProperties;
            }
            set
            {
                visibleProperties = value;
                this.DoChanged();
            }
        }
        /*
        public MemberPropertiesDisplayType DisplayType
        {
            get 
            { 
                return displayType; 
            }
            set 
            { 
                displayType = value;
                changed();
            }
        }*/

        #endregion

        #region События

        private MemberPropertiesEventHandler changed = null;


        public event MemberPropertiesEventHandler Changed
        {
            add
            {
                changed += value;
            }
            remove
            {
                changed -= value;
            }
        }

        private void DoChanged()
        {
            if (this.changed != null)
                this.changed();
        }

        #endregion


        public MemberProperties()
        {
            this.allProperties = new List<string>();
            this.visibleProperties = new List<string>();
            //this.displayType = MemberPropertiesDisplayType.None;
        }

        public override string ToString()
        {
            string result = "";

            if (visibleProperties != null)
            {
                foreach (string prop in visibleProperties)
                {
                    result += prop + ";";
                }
            }
            return result;
        }

    }
    
    public enum MemberPropertiesDisplayType
    {
        [Description("Нет")]
        None,
        [Description("В отчете")]
        DisplayInReport,
        [Description("В комментариях")]
        DisplayInHint,
        [Description("Везде")]
        DisplayOverall
    }

}
