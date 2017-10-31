using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert.Grid
{
    class GridConsts
    {
        //�������� ��������� ��� ��������� �������
        static public Color gridRowsBackColorStart = Color.FromArgb(228, 236, 247);
        static public Color gridRowsBackColorEnd = Color.FromArgb(228, 236, 247);

        static public Color gridColumnsBackColorStart = gridRowsBackColorStart;
        static public Color gridColumnsBackColorEnd = gridRowsBackColorEnd;

        static public Color gridCaptionsBackColorStart = Color.FromArgb(228, 236, 247);
        static public Color gridCaptionsBackColorEnd = Color.FromArgb(228, 236, 247);

        static public Color gridCommonBorderColor = Color.FromArgb(158, 182, 206);
        static public Color gridAxisForeColor = Color.FromArgb(0, 0, 0);
        static public Color gridMesuresForeColor = Color.FromArgb(0, 0, 0);

        //���������� �� �������� ��������� ������, ������� � �������� �������� �� �������������� 
        //(��������� ������� - ��������)
        public const int boundsDeflection = 5;

        /// <summary>
        /// ������������ ������ �����
        /// </summary>
        public const string totalCaption = "�����";

        /// <summary>
        /// ���� �������� ���������� �� ������� ������, ����� ������ � ��� empty
        /// </summary>
        public const string empty = "(�����)";

        /// <summary>
        /// ��������� ��� �������� "������� ��������"
        /// </summary>
        public const string average = "������� ��������";

        /// <summary>
        /// ��������� ��� �������� "�������"
        /// </summary>
        public const string median = "�������";

        /// <summary>
        /// ��������� ��� �������� "����������� ����������"
        /// </summary>
        public const string stdev = "����������� ����������";


        /// <summary>
        /// �� ���������� �������� � ������
        /// </summary>
        public const string errorValue = "#����!";
        
        /// <summary>
        /// �������� � ������ - ��������� ������� �� 0
        /// </summary>
        public const string errorDivZero = "#���/0!";

        /// <summary>
        /// ����� ����
        /// </summary>
        public const string grandTotal = "grandTotal";
        /// <summary>
        /// ����
        /// </summary>
        public const string total = "total";
        /// <summary>
        /// ������� ���������
        /// </summary>
        public const string member = "member";
        /// <summary>
        /// ���������� ������� ���������
        /// </summary>
        public const string collapsedMember = "collapsed_member";
        /// <summary>
        /// ��������
        /// </summary>
        public const string duplicate = "duplicate";

        //
        //���������� ����� � Xml
        //

        /// <summary>
        /// �������� �������
        /// </summary>
        public const string gridPropertys = "gridPropertys";
        /// <summary>
        /// �����������
        /// </summary>
        public const string painter = "painter";
        /// <summary>
        /// ��������� �������
        /// </summary>
        public const string gridCaption = "gridCaption";
        /// <summary>
        /// ��������� ��������
        /// </summary>
        public const string filtersCaptions = "filtersCaptions";
        /// <summary>
        /// ��������� �������
        /// </summary>
        public const string columnsCaptions = "columnsCaptions";
        /// <summary>
        /// ��������� �����
        /// </summary>
        public const string rowsCaptions = "rowsCaptions";
        /// <summary>
        /// ������
        /// </summary>
        public const string rows = "rows";
        /// <summary>
        /// �������
        /// </summary>
        public const string columns = "columns";
        /// <summary>
        /// ��������� ���
        /// </summary>
        public const string measuresCaptions = "measuresCaptions";
        /// <summary>
        /// ������� ������
        /// </summary>
        public const string measuresData = "measuresData";
        /// <summary>
        /// �����������
        /// </summary>
        public const string comments = "comments";
        /// <summary>
        /// �������������� ��������
        /// </summary>
        public const string addinPropertys = "addinPropertys";
        /// <summary>
        /// �����
        /// </summary>
        public const string style = "style";
        /// <summary>
        /// ����� ���� MemberProperties
        /// </summary>
        public const string memPropNameStyle = "memPropNameStyle";
        /// <summary>
        /// ����� �������� MemberProperties
        /// </summary>
        public const string memPropValueStyle = "memPropValueStyle";
        /// <summary>
        /// ��������� ����������� ��������� ���
        /// </summary>
        public const string stateMembersExpand = "stateMembersExpand";
        /// <summary>
        /// ����� ������
        /// </summary>
        public const string totalsStyle = "totalsStyle";
        /// <summary>
        /// ����� ��������� ����� ���
        /// </summary>
        public const string dummyStyle = "dummyStyle";
        /// <summary>
        /// ����� ����������� ������ ���������
        /// </summary>
        public const string optionalCellStyle = "optionalCellStyle";
        /// <summary>
        /// �����
        /// </summary>
        public const string colors = "colors";
        /// <summary>
        /// �����
        /// </summary>
        public const string font = "font";
        /// <summary>
        /// �������� ���������� ��������
        /// </summary>
        public const string propertys = "propertys";
        /// <summary>
        /// ������ ������
        /// </summary>
        public const string stringFormat = "stringFormat";

        //
        //���������� ��������� � Xml
        //

        /// <summary>
        /// ��������� ���� ����
        /// </summary>
        public const string backColorStart = "backColorStart";
        /// <summary>
        /// �������� ���� ����
        /// </summary>
        public const string backColorEnd = "backColorEnd";
        /// <summary>
        /// ���� �������
        /// </summary>
        public const string borderColor = "borderColor";
        /// <summary>
        /// ���� ������
        /// </summary>
        public const string foreColor = "foreColor";
        /// <summary>
        /// ���� ������
        /// </summary>
        public const string gradient = "gradient";
        /// <summary>
        /// ��������� ���������� ������
        /// </summary>
        public const string highlightColor = "highlightColor";
        /// <summary>
        /// ����� ���������������� � ������
        /// </summary>
        public const string sfont = "sfont";
        /// <summary>
        /// ������� ���������
        /// </summary>
        public const string sizes = "size";
        /// <summary>
        /// ������ ��������
        /// </summary>
        public const string widths = "widths";
        /// <summary>
        /// ������ ���������
        /// </summary>
        public const string heights = "heights";
        /// <summary>
        /// �������� �������
        /// </summary>
        public const string timerInterval = "timerInterval";
        /// <summary>
        /// ������������ ������ ������������
        /// </summary>
        public const string maxWidth = "maxWidth";
        /// <summary>
        /// ����������
        /// </summary>
        public const string isDisplay = "isDisplay";
        /// <summary>
        /// ����������
        /// </summary>
        public const string visible = "visible";
        /// <summary>
        /// ���������� �� ����� ��������
        /// </summary>
        public const string displayUntilConrolChange = "displayUntilConrolChange";
        /// <summary>
        /// ������������ (�� ���������)
        /// </summary>
        public const string vAligment = "vAligment";
        /// <summary>
        /// ������������ (�� �����������)
        /// </summary>
        public const string hAligment = "hAligment";
        /// <summary>
        /// ��������� ������
        /// </summary>
        public const string trimming = "trimming";
        /// <summary>
        /// �������������� ����� �������������� ������
        /// </summary>
        public const string formatFlags = "formatFlags";
        /// <summary>
        /// �������� ���� ����
        /// </summary>
        public const string value = "value";
        /// <summary>
        /// ������������� ������ ������ ����� � �����
        /// </summary>
        public const string autoSizeRows = "autoSizeRows";
        /// <summary>
        /// ������ ����������� ����� ��������� �������
        /// </summary>
        public const string separatorHeight = "separatorHeight";
        /// <summary>
        /// �����
        /// </summary>
        public const string text = "text";
        /// <summary>
        /// ���������
        /// </summary>
        public const string proportion = "proportion";
        /// <summary>
        /// ����� ������������
        /// </summary>
        public const string place = "place";
        /// <summary>
        /// ����������� ���������� ��������� ������������ � ������� � ��������
        /// </summary>
        public const string tipDisplayMaxMemberCount = "tipDisplayMaxMemberCount";
        /// <summary>
        /// ���������� ������������ ��������� � �������� �������
        /// </summary>
        public const string displayMemberCount = "displayMemberCount";
        /// <summary>
        /// ���������� ������� � ���������� ��������� �������
        /// </summary>
        public const string isCaptionIncludeParents = "isCaptionIncludeParents";
        /// <summary>
        /// ��� ����
        /// </summary>
        public const string measureName = "measureName";
        /// <summary>
        /// ������� ��� ������� ���������
        /// </summary>
        public const string colorRuleCondition = "condition";
        /// <summary>
        /// �������� 1 ��� ������� ���������
        /// </summary>
        public const string colorRuleValue1 = "value1";
        /// <summary>
        /// �������� 2 ��� ������� ���������
        /// </summary>
        public const string colorRuleValue2 = "value2";
        /// <summary>
        /// ������� ���������� ������� ���������
        /// </summary>
        public const string colorRuleArea = "area";
        /// <summary>
        /// ������� ���������
        /// </summary>
        public const string colorRule = "colorRule";
        /// <summary>
        /// ��������� ������ ���������
        /// </summary>
        public const string colorRules = "colorRules";
        /// <summary>
        /// ������� �������
        /// </summary>
        public const string gridScale = "scale";
        /// <summary>
        /// �������� ��������
        /// </summary>
        public const string scaleValue = "value";
        /// <summary>
        /// ���������� ������
        /// </summary>
        public const string selectedCells = "selectedCells";
        /// <summary>
        /// ������������� ���������� ������
        /// </summary>
        public const string selectedCellItem = "item";

    }
}
