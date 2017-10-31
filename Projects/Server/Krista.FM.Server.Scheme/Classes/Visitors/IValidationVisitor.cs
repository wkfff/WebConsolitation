using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes.Visitors
{
    /// <summary>
    /// Посетитель проверяющий корректность состояния  объекта.
    /// </summary>
    internal interface IValidationVisitor
    {
        void Visit(IPackage node);
        void Visit(IEntity node);
        void Visit(IEntityAssociation node);
        void Visit(IDocument node);
        void Visit(IDataAttribute node);
    }
}
