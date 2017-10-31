using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using bus.gov.ru.Properties;
using bus.gov.ru.types.Item1;
using Microsoft.Practices.Unity;
using Xml.Schema.Linq;

namespace bus.gov.ru
{
    public static class XTypedElementExtention
    {
        /// <summary> Валидация элемента. </summary>
        /// <remarks> Krutin, 30.05.2013. </remarks>
        /// <param name="globalTypedElement"> элемент, поддерживаются только SchemaOrigin.Fragment и
        ///                                   SchemaOrigin.Element - контейнеры. </param>
        /// <param name="schemaSet"> подразумевается контейнер в рамках сессии, например <see cref="UnityContainer"/>
        ///                          <code>Resolver.Get&lt;XmlSchemaSet&gt;()</code> </param>
        /// <param name="detail"> [out] сериализованные в строку сообщения валидатора. </param>
        /// <returns> true если в процессе не получено сообщений от валидатора. </returns>
        /// <exception cref="NotSupportedException"> если globalTypedElement недопустимого типа. </exception>
        public static bool Validate(this XTypedElement globalTypedElement, XmlSchemaSet schemaSet, out string detail)
        {
            // todo: [refactor] need verify contract
            PrepareSchemaSet(schemaSet);

            var errorCollector = new StringBuilder();
            
            globalTypedElement.Untyped.Validate(
                PartialValidationType(globalTypedElement, schemaSet),
                schemaSet,
                (sender, args) => errorCollector.Append(string.Format("{0}: {1}\n", args.Severity, args.Message)));

            return string.IsNullOrEmpty(detail = errorCollector.ToString());
        }

        /// <summary> Валидация элемента. </summary>
        /// <remarks> Krutin, 30.05.2013. </remarks>
        /// <param name="globalTypedElement"> Элемент, поддерживаются только SchemaOrigin.Fragment и
        ///                                   SchemaOrigin.Element - контейнеры. </param>
        /// <param name="schemaSet"> Подразумевается контейнер в рамках сессии, например <see cref="UnityContainer"/>
        ///                          <code>Resolver.Get&lt;XmlSchemaSet>()></code> </param>
        /// <param name="violation"> [out] The violation. </param>
        /// <returns> true если в процессе не получено сообщений от валидатора. </returns>
        public static bool Validate(this XTypedElement globalTypedElement, XmlSchemaSet schemaSet, out IEnumerable<violationType> violation)
        {
            // todo: [refactor] need verify contract
            PrepareSchemaSet(schemaSet);

            var violationCollector = new List<violationType>();

            globalTypedElement.Untyped.Validate(
                PartialValidationType(globalTypedElement, schemaSet),
                schemaSet,
                (sender, args) => violationCollector.Add(
                    new violationType
                        {
                            code = "XVE",
                            level = args.Severity.ToString().ToLowerInvariant(),
                            name = args.Message,
                            description = args.Exception.Message,
                        }));

            return !(violation = violationCollector).Any();
        }

        private static void PrepareSchemaSet(XmlSchemaSet schemaSet)
        {
            if (!schemaSet.Contains("http://bus.gov.ru/fk/1")
                && !schemaSet.Contains("http://bus.gov.ru/external/1")
                && !schemaSet.Contains("http://bus.gov.ru/types/1"))
            {
                schemaSet.Add("http://bus.gov.ru/fk/1", XmlReader.Create(new StringReader(Resources.FK)));
                schemaSet.Add("http://bus.gov.ru/external/1", XmlReader.Create(new StringReader(Resources.External)));
                schemaSet.Add("http://bus.gov.ru/types/1", XmlReader.Create(new StringReader(Resources.Types)));
            }

            if (!schemaSet.IsCompiled)
            {
                schemaSet.Compile();
            }
        }

        private static XmlSchemaObject PartialValidationType(XTypedElement globalTypedElement, XmlSchemaSet schemaSet)
        {
            var xmlQualifiedName = new XmlQualifiedName(
                ((IXMetaData)globalTypedElement).SchemaName.LocalName,
                ((IXMetaData)globalTypedElement).SchemaName.NamespaceName);

            switch (((IXMetaData)globalTypedElement).TypeOrigin)
            {
                case SchemaOrigin.Fragment:
                    return schemaSet.GlobalTypes[xmlQualifiedName];
                case SchemaOrigin.Element:
                    return schemaSet.GlobalElements[xmlQualifiedName];
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
