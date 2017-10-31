namespace Krista.FM.Domain
{
    public class Templates : DomainObject
    {
        public virtual int? ParentID { get; set; }
        public virtual int Type { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string DocumentFileName { get; set; }
        public virtual int? Editor { get; set; }
        public virtual string Code { get; set; }
        public virtual int? SortIndex { get; set; }
        public virtual int? Flags { get; set; }
        public virtual TemplatesTypes RefTemplatesTypes { get; set; }
        public virtual byte[] Document { get; set; }

        private bool isVisible;

        public virtual bool GetIsVisible()
        {
            return isVisible;
        }

        public virtual void SetIsVisible(bool value)
        {
            isVisible = value;
        }
    }
}
