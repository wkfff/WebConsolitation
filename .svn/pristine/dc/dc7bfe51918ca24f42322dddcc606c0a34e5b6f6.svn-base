using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.MobileReports.Common
{
    /// <summary>
    /// Информация о категории
    /// </summary>
    public class CategoryInfo : BaseElementInfo
    {
        /// <summary>
        /// Массив байт с изображением
        /// </summary>
        private byte[] _iconByte;
        private List<CategoryInfo> _childrenCategory;
        private List<ReportInfo> _childrenReports;

        public byte[] IconByte
        {
            get { return _iconByte; }
            set { _iconByte = value; }
        }

        public List<CategoryInfo> ChildrenCategory
        {
            get { return _childrenCategory; }
            set { _childrenCategory = value; }
        }

        public List<ReportInfo> ChildrenReports
        {
            get { return _childrenReports; }
            set { _childrenReports = value; }
        }

        public CategoryInfo()
            : this(new BaseElementInfo())
        {
        }

        public CategoryInfo(BaseElementInfo baseElement)
            : base(baseElement)
        {
            this.ChildrenCategory = new List<CategoryInfo>();
            this.ChildrenReports = new List<ReportInfo>();
        }

        public override IPhoteTemplateDescriptor GetTemplateDescriptor(bool isOptimizeTemplateType)
        {
            IPhoteTemplateDescriptor result = base.GetTemplateDescriptor(isOptimizeTemplateType);
            result.IconByte = IconByte;
            return result;
        }

        public override void SetTemplateDescriptor(IPhoteTemplateDescriptor value)
        {
            base.SetTemplateDescriptor(value);
            this.IconByte = value.IconByte;
        }

        public int GetDescendantsHashCode()
        {
            int result = 0;
            foreach (CategoryInfo category in this.ChildrenCategory)
            {
                result += category.SelfHashCode;
                result += category.GetDescendantsHashCode();
            }

            foreach (ReportInfo report in this.ChildrenReports)
            {
                result += report.SelfHashCode;
            }

            return result;
        }
    }
}
