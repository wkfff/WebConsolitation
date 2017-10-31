using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.MobileReports.Common
{
    /// <summary>
    /// Базовый класс для Отчетов(ReportInfo) и Категорий(CategoryInfo)
    /// </summary>
    public class BaseElementInfo
    {
        #region Поля
        private int _id;
        private int _parentId;
        private string _code;
        private int _refObject;
        private string _name;
        private string _description;
        private bool _subjectDepended;
        private DateTime _lastDeployDate;
        private int _forumDiscussionID;

        private BaseElementInfo _parent;
        private List<int> _groupsPermisions;
        private List<int> _usersPermisions;
        private string _territorialTagsID;
        private MobileTemplateTypes _templateType;

        private int _selfHashCode;
        #endregion

        #region Свойства
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public int RefObject
        {
            get { return _refObject; }
            set { _refObject = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public bool SubjectDepended
        {
            get { return _subjectDepended; }
            set { _subjectDepended = value; }
        }

        public DateTime LastDeployDate
        {
            get { return _lastDeployDate; }
            set { _lastDeployDate = value; }
        }

        public BaseElementInfo Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public List<int> GroupsPermisions
        {
            get { return _groupsPermisions; }
            set { _groupsPermisions = value; }
        }

        public List<int> UsersPermisions
        {
            get { return _usersPermisions; }
            set { _usersPermisions = value; }
        }

        public string TerritorialTagsID
        {
            get { return _territorialTagsID; }
            set { _territorialTagsID = value; }
        }

        public MobileTemplateTypes TemplateType
        {
            get { return _templateType; }
            set { _templateType = value; }
        }

        public int SelfHashCode
        {
            get 
            {
                if (_selfHashCode == int.MinValue)
                    _selfHashCode = this.GetSelfHashCode();
                return _selfHashCode;
            }
        }

        public int ForumDiscussionID
        {
            get { return _forumDiscussionID; }
            set { _forumDiscussionID = value; }
        }

        #endregion

        public BaseElementInfo() 
            : this(-1, -1, -1, string.Empty, string.Empty, string.Empty, string.Empty, MobileTemplateTypes.None)
        {
        }

        public BaseElementInfo(BaseElementInfo baseElement)
            : this(baseElement.Id, baseElement.ParentId, baseElement.RefObject, 
            baseElement.Name, baseElement.Description, baseElement.Code, 
            baseElement.TerritorialTagsID, baseElement.TemplateType)
        {
        }

        public BaseElementInfo(int id, int parentId, int refObject, string name, string description,
            string code, string territorialTagsID, MobileTemplateTypes templateType)
        {
            this.Id = id;
            this.ParentId = parentId;
            this.RefObject = refObject;
            this.Name = name;
            this.Description = description;
            this.Code = code;
            this.TerritorialTagsID = territorialTagsID;
            this.TemplateType = templateType;

            this.GroupsPermisions = new List<int>();
            this.UsersPermisions = new List<int>();

            this._selfHashCode = int.MinValue;
            this.ForumDiscussionID = -1;
        }

        public new bool Equals(object obj)
        {
            BaseElementInfo comparedObject = (BaseElementInfo)obj;
            return this.Code == comparedObject.Code;
        }

        /// <summary>
        /// Возвращает тип шаблона для IPhoteTemplateDescriptor (если у родителя такой же тип, 
        /// вернет тип шаблона None, т.к. по умолчанию будет брать родительский)
        /// </summary>
        /// <returns></returns>
        private MobileTemplateTypes GetTemplateTypeForDescription()
        {
            MobileTemplateTypes result = this.TemplateType;
            if ((this.Parent != null) && (this.Parent.TemplateType == result))
                result = MobileTemplateTypes.None;
            return result;
        }

        /// <summary>
        /// Вернет интерфейсе IPhoteTemplateDescriptor
        /// </summary>
        /// <param name="isOptimizeTemplateType">если выставляем флаг, то в случае совпадения типа 
        /// с родительским, тип будет - None, если нужно фактическое значение выставляем false</param>
        /// <returns></returns>
        public virtual IPhoteTemplateDescriptor GetTemplateDescriptor(bool isOptimizeTemplateType)
        {
            IPhoteTemplateDescriptor descriptor = new IPhoteTemplateDescriptor();
            descriptor.TemplateType = isOptimizeTemplateType ? this.GetTemplateTypeForDescription() : this.TemplateType;
            descriptor.SubjectDepended = this.SubjectDepended;
            descriptor.LastDeployDate = this.LastDeployDate;
            descriptor.ForumDiscussionID = this.ForumDiscussionID;
            descriptor.TerritoryRF = this.TerritorialTagsID;
            return descriptor;
        }

        public virtual void SetTemplateDescriptor(IPhoteTemplateDescriptor value)
        {
            this.TemplateType = value.TemplateType;
            this.SubjectDepended = value.SubjectDepended;
            this.LastDeployDate = value.LastDeployDate;
            this.ForumDiscussionID = value.ForumDiscussionID;
            this.TerritorialTagsID = value.TerritoryRF != null ? value.TerritoryRF : string.Empty;
        }

        public virtual int GetSelfHashCode()
        {
            int result = this.Id.GetHashCode();
            result += this.Name.GetHashCode();
            result += this.Description.GetHashCode();
            result += Utils.GetHashCode(this.UsersPermisions);
            result += Utils.GetHashCode(this.GroupsPermisions);
            result += this.TerritorialTagsID.GetHashCode();
            result += this.Code.GetHashCode();
            result += this.SubjectDepended.GetHashCode();
            result += this.TemplateType.GetHashCode();
            result += this.LastDeployDate.GetHashCode();
            result += this.ForumDiscussionID.GetHashCode();
            
            /*if (this.Code == "fo_1035_0015_mo=353")
            {
                Trace.TraceInformation("==========================================================");
                Trace.TraceInformation("Code={0} HashCode={1}", this.Code, this.Code.GetHashCode());
                Trace.TraceInformation("Id={0} HashCode={1}", this.Id, this.Id.GetHashCode());
                Trace.TraceInformation("Name={0} HashCode={1}", this.Name, this.Name.GetHashCode());
                Trace.TraceInformation("Description={0} HashCode={1}", this.Description, this.Description.GetHashCode());
                Trace.TraceInformation("TemplateType={0} HashCode={1}", this.TemplateType, this.TemplateType.GetHashCode());
                Trace.TraceInformation("LastDeployDate={0} HashCode={1}", this.LastDeployDate, this.LastDeployDate.GetHashCode());
                Trace.TraceInformation("ForumDiscussionID={0} HashCode={1}", this.ForumDiscussionID, this.ForumDiscussionID.GetHashCode());
                Trace.TraceInformation("SubjectDepended={0} HashCode={1}", this.SubjectDepended, this.SubjectDepended.GetHashCode());
                Trace.TraceInformation("Id={0} HashCode={1}", this.Id, this.Id.GetHashCode());

                Trace.TraceInformation("GroupsPermisions={0} HashCode={1}", this.GroupsPermisions, Utils.GetHashCode(this.GroupsPermisions));
                string str = string.Empty;
                foreach (int item in this.GroupsPermisions)
                {
                    str += string.Format("{0}|", item);
                }
                Trace.TraceInformation("GroupsPermisions={0}", str);

                Trace.TraceInformation("UsersPermisions={0} HashCode={1}", this.UsersPermisions, Utils.GetHashCode(this.UsersPermisions));
                str = string.Empty;
                foreach (int item in this.UsersPermisions)
                {
                    str += string.Format("{0}|", item);
                }
                Trace.TraceInformation("UsersPermisions={0}", str);
                
                Trace.TraceInformation("Total={0} ", result);
            }*/

            return result.GetHashCode();
        }
    }
}
