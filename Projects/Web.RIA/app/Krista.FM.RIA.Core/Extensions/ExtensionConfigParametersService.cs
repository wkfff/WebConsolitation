using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Krista.FM.Extensions;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Core.Extensions
{
    public class ExtensionConfigParametersService : IParametersService
    {
        private readonly IUnityContainer container;
        private readonly Dictionary<string, object> parameters;

        public ExtensionConfigParametersService(Stream stream, IUnityContainer container)
        {
            this.container = container;
            parameters = new Dictionary<string, object>();

            XDocument doc = XDocument.Load(new XmlTextReader(stream));
            XElement paramsElement = doc.Root.Element("Params");
            if (paramsElement != null)
            {
                foreach (XElement xParameter in paramsElement.Elements("Param"))
                {
                    var provider = xParameter.Element("ValueProvider");
                    if (provider != null)
                    {
                        string typeName = provider.Attribute("type").Value;
                        Type type = Type.GetType(typeName);
                        if (type != null)
                        {
                            parameters.Add(xParameter.Attribute("name").Value, type);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает вычесленное значение параметра.
        /// </summary>
        /// <param name="parameterName">Имя параметра.</param>
        /// <returns>Значение параметра.</returns>
        public string GetParameterValue(string parameterName)
        {
            if (!parameters.ContainsKey(parameterName))
                throw new Exception("Конфигурационный параметр '{0}' не найден."
                    .FormatWith(parameterName));

            object obj = parameters[parameterName];
            if (obj is IParameterValueProvider)
            {
                return ((IParameterValueProvider)parameters[parameterName]).GetValue();
            }
            
            var valueProvider = container.Resolve((Type)obj)
                as IParameterValueProvider;
            if (valueProvider == null)
            {
                valueProvider = (IParameterValueProvider)Activator
                    .CreateInstance((Type)obj);
            }

            parameters[parameterName] = valueProvider;
            return valueProvider.GetValue();
        }
    }
}
