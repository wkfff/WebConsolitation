using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Реализует поведение зависимых (агрегатов) объектов
    /// </summary>
    internal abstract class MinorObject : KeyIdentifiedObject, IMinorObject
    {
        public MinorObject(string key, ServerSideObject owner, ServerSideObjectStates state)
            : base(key, owner, state)
        {
        }
    }
}
