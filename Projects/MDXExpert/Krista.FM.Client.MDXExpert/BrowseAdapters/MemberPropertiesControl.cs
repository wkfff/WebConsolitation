using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;
using System.Windows.Forms.Design;
using Krista.FM.Expert.PivotData;

namespace Krista.FM.Client.MDXExpert
{
    public partial class MemberPropertiesControl : UserControl
    {
        private MemberProperties memberProperties = new MemberProperties();

        //private List<string> visibleMemberProperties;
        
        public MemberProperties MemberProperties
        {
            get { return memberProperties; }
            set { memberProperties = value; }
        }


        public MemberPropertiesControl()
        {
            InitializeComponent();
        }

        public MemberPropertiesControl(MemberProperties memberProperties)
        {
            InitializeComponent();

            this.memberProperties = memberProperties;

            if (memberProperties.AllProperties == null)
            {
                return;
            }

            UltraListViewItem lvItem;

            foreach(string prop in memberProperties.AllProperties)
            {
                lvItem = lvMemberProperties.Items.Add(prop);
                lvItem.Value = prop;
                if ((memberProperties.VisibleProperties != null) && memberProperties.VisibleProperties.Contains(prop))
                {
                    lvItem.CheckState = CheckState.Checked;
                }
                else
                {
                    lvItem.CheckState = CheckState.Unchecked;
                }
            }
        }

        //функция сравнения двух стринглистов. не знаю может уже есть такая, но я не нашел
        private bool StrListEquals(List<string> list1, List<string> list2)
        {
            if (list1.Count == list2.Count)
            {
                foreach (string item in list1)
                {
                    if (!list2.Contains(item))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private void btOK_Click(object sender, EventArgs e)
        {

            List<string> tmpList = new List<string>();
            

            foreach (UltraListViewItem lvItem in lvMemberProperties.Items)
            {
                if (lvItem.CheckState == CheckState.Checked)
                {
                    tmpList.Add(lvItem.Text);
                }
            }

            if (!StrListEquals(tmpList, memberProperties.VisibleProperties))
            {
                memberProperties.VisibleProperties. Clear();
                memberProperties.VisibleProperties = tmpList;
            }

            if(Tag != null)
            {
                ((IWindowsFormsEditorService)Tag).CloseDropDown();
            }
        }

    }
}
