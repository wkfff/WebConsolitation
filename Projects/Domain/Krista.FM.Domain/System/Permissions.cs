namespace Krista.FM.Domain
{
    public class Permissions : DomainObject
    {
        public virtual Objects RefObjects { get; set; }
        public virtual Groups RefGroups { get; set; }
        public virtual Users RefUsers { get; set; }
        public virtual int AllowedAction { get; set; }
    }
}
