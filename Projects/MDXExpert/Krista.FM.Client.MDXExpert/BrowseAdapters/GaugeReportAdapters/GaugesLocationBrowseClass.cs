using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Controls;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class  GaugesLocationBrowseClass : FilterablePropertyBase
    {
        #region ����

        private GaugesLocation _location;

        #endregion

        #region ��������

        [DisplayName("�������������� ����������")]
        [Description("�������������� ���������� �����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool IsAutoLocation
        {
            get { return this._location.IsAutoLocation; }
            set { this._location.IsAutoLocation = value; }
        }


        [Description("�������")]
        [DisplayName("���������� �������")]
        [DynamicPropertyFilter("IsAutoLocation", "False")]
        [Browsable(true)]
        public int Columns
        {
            get { return this._location.Columns; }
            set { this._location.Columns = value; }
        }

        [Description("����")]
        [DisplayName("���������� �����")]
        [DynamicPropertyFilter("IsAutoLocation", "False")]
        [Browsable(true)]
        public int Rows
        {
            get { return this._location.Rows; }
            set { this._location.Rows = value; }
        }

        [Description("������������ ���������� �����������")]
        [DisplayName("������������ ���������� �����������")]
        [Browsable(true)]
        public int MaxCount
        {
            get { return this._location.MaxCount; }
            set { this._location.MaxCount = value; }
        }



        #endregion

        public GaugesLocationBrowseClass(GaugesLocation location)
        {
            this._location = location;
        }

        public override string ToString()
        {
            return String.Empty;
        }

    }
       
}