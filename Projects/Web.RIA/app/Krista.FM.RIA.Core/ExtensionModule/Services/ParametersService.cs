using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Extensions;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Core.ExtensionModule.Services
{
    public class ParametersService : IParametersService
    {
        private readonly Dictionary<string, object> parameters;
        private readonly IUnityContainer container;

        public ParametersService(IUnityContainer container)
        {
            this.container = container;
            parameters = new Dictionary<string, object>();
        }

        public void RegisterExtensionConfigParameters(Stream stream)
        {
            XDocument doc = XDocument.Load(new XmlTextReader(stream));
            XElement paramsElement = doc.Root.Element("Params");
            if (paramsElement != null)
            {
                foreach (XElement xParameter in paramsElement.Elements("Param"))
                {
                    var provider = xParameter.Element("ValueProvider");
                    if (provider != null)
                    {
                        string parameterName = xParameter.Attribute("name").Value;
                        string typeName = provider.Attribute("type").Value;
                        Type type = Type.GetType(typeName);
                        
                        this.RegisterExtensionConfigParameter(parameterName, type);
                    }
                }
            }
        }

        public void RegisterExtensionConfigParameter(string name, Type type)
        {
            if (parameters.ContainsKey(name))
            {
                throw new Exception("Конфигурационный параметр \"{0}\" уже определен."
                    .FormatWith(name));
            }

            if (type != null)
            {
                parameters.Add(name, type);
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
            {
                throw new Exception("Конфигурационный параметр '{0}' не найден."
                                        .FormatWith(parameterName));
            }

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

        public void Clear()
        {
            parameters.Clear();
        }
    }
}
