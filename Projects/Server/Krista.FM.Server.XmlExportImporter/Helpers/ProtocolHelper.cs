using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.XmlExportImporter.Helpers
{
    internal class ProtocolHelper
    {
        /// <summary>
        /// запись в протокол о каких то несовпадениях в атрибутах
        /// </summary>
        internal static void SendMessageToProtocol(IEntity schemeObject, IClassifiersProtocol protocol, string message,
            int sourceId, ClassifiersEventKind classifiersEventKind)
        {
            //if (schemeObject is IClassifier)
            protocol.WriteEventIntoClassifierProtocol(classifiersEventKind, schemeObject.OlapName,
                -1, sourceId, (int)schemeObject.ClassType, message);
        }
    }
}
