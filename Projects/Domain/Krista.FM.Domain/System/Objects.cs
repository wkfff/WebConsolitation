namespace Krista.FM.Domain
{
    public class Objects : DomainObject
    {
        public virtual string ObjectKey { get; set; }				/* Уникальный идентификатор объекта схемы */
        public virtual string Name { get; set; }	                /* Английское имя */
        public virtual string Caption { get; set; }              	/* Русское наименование */
        public virtual string Description { get; set; }			    /* Описание */
        public virtual int ObjectType { get; set; }	
    }
}
