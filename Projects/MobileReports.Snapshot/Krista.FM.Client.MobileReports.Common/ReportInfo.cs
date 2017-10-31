using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.MobileReports.Common
{
    /// <summary>
    /// ���������� �� ������
    /// </summary>
    public class ReportInfo : BaseElementInfo
    {
        private bool _isNotScrollable;
        private bool _isNeedUpdating;

        /// <summary>
        /// �� ������������ �����
        /// </summary>
        public bool IsNotScrollable
        {
            get { return _isNotScrollable; }
            set { _isNotScrollable = value; }
        }

        /// <summary>
        /// ����� ������� ����������
        /// </summary>
        public bool IsNeedUpdating
        {
            get { return _isNeedUpdating; }
            set { _isNeedUpdating = value; }
        }


        /// <summary>
        /// ���������� ����� � ������, � iPad �� 2 � ��������� 3
        /// </summary>
        public int ViewCount
        {
            get 
            {
                return (this.TemplateType == MobileTemplateTypes.IPad) ? 2 : 3;
            }
        }

        public ReportInfo()
            : this(new BaseElementInfo())
        {
        }

        public ReportInfo(BaseElementInfo baseElement)
            : base(baseElement)
        {
        }

        /*public ReportInfo(int id, int parentId, int refObject, string name, string description, 
            string code, bool subjectDepended, MobileTemplateTypes templateType)
            : base(id, parentId, refObject, name, description, code, templateType)
        {
            this.SubjectDepended = subjectDepended;
        }*/

        /// <summary>
        /// �.�. � ������ ��� �������� �������������, � ������� ���� ID, �������� ������
        /// </summary>
        /// <param name="viewMode">�������� �������������</param>
        /// <returns></returns>
        public string GetReportCode(ReportViewMode viewMode)
        {
            switch (viewMode)
            {
                case ReportViewMode.Original: return this.Code;
                case ReportViewMode.Vertical: return this.Code + "_V";
                case ReportViewMode.Horizontal: return this.Code + "_H";
            }
            return this.Code;
        }

        public override void SetTemplateDescriptor(IPhoteTemplateDescriptor value)
        {
            base.SetTemplateDescriptor(value);
            this.IsNotScrollable = value.IsNotScrollable;
        }

        public override int GetSelfHashCode()
        {
            int result = base.GetSelfHashCode();
            result += this.IsNotScrollable.GetHashCode();

            return result;
        }

    }

    /// <summary>
    /// ���� ����������� �������
    /// </summary>
    public enum ReportViewMode
    {
        Original,
        Vertical,
        Horizontal
    }
}
