using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    internal static class AssociateOperations
    {
        public static object Invoke(string methodName, object[] args)
        {
            switch (methodName)
            {
                case "Concatenate":
                    return Concatenate(args);
                /*case "SubString":
                    return SubString(args);*/
                default:
                    throw new Exception(String.Format("Неизвестный метод \"{0}\"", methodName));
            }
        }

        public static string Concatenate(params object[] args)
        {
            StringBuilder result = new StringBuilder();

            foreach (object item in args)
            {
                result.Append(Convert.ToString(item));
            }
            return result.ToString();
        }

        public static string SubString(string sourceString, int startIndex, int length)
        {
            return sourceString.Substring(startIndex, length);
        }
    }

    /// <summary>
    /// Значение используемое в правиле сопоставления
    /// </summary>
    internal class MappingValue : SMOSerializable, IMappingValue
    {
        /// <summary>
        /// Определяет входной параметр для операторов
        /// </summary>
        internal class Parameter
        {
            public DataAttribute Attribute;
            public Object Value;
            public string Bind;
            public Operator _operator;

            public Parameter(XmlNode xmlNode, IEntity classifier)
            {
                this.Attribute = DataAttributeCollection.GetAttributeByKeyName(
                    classifier.Attributes,
                    xmlNode.Attributes["name"].Value,
                    xmlNode.Attributes["name"].Value);

                if (this.Attribute == null)
                {
                    XmlNode xmlValueAttr = xmlNode.Attributes["value"];
                    if (xmlValueAttr != null)
                    {
                        this.Bind = xmlNode.Attributes["name"].Value;
                        this.Value = xmlValueAttr.Value;
                    }
                    else
                        throw new Exception(String.Format("Указан несуществующий атрибут в качестве параметра {0}.{1}", classifier.FullName, xmlNode.Attributes["name"].Value));
                }

                XmlNode xmlOperator = xmlNode.SelectSingleNode("Operator");
                if (xmlOperator != null)
                    _operator = new Operator(xmlOperator, classifier);
            }

            internal void Save2Xml(XmlNode node)
            {
                if (this.Attribute != null)
                    XmlHelper.SetAttribute(node, "name", this.Attribute.Name);
                else
                {
                    XmlHelper.SetAttribute(node, "bind", this.Bind);
                    XmlHelper.SetAttribute(node, "value", Convert.ToString(this.Value));
                }

                if (this._operator != null)
                {
                    XmlNode xmlOperator = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Operator", null);

                    this._operator.Save2Xml(xmlOperator);

                    node.AppendChild(xmlOperator);
                }
            }

            /// <summary>
            /// Преобразует значение в строку с явным указанием размерности аттрибута
            /// </summary>
            /// <param name="attributeValue">Значение атрибута</param>
            /// <param name="attributeName">Наименование атрибута</param>
            /// <param name="attributeType">Тип атрибута</param>
            /// <param name="size">Размерность значения атрибута, которое нужно получить</param>
            /// <returns>Значение преобразованное в строку</returns>
            internal static string GetStringValue(object attributeValue, string attributeName, DataAttributeTypes attributeType, bool isNumericContain, int size, StringElephanterSettings settings)
            {
                string val = Convert.ToString(attributeValue);
                if ((attributeType == DataAttributeTypes.dtInteger || 
                    isNumericContain ||
                    attributeName == "CodeStr" ||
                    attributeName == "SystemCalculatedCode" ||
                    attributeName == "LongCode") 
                    && val.Length <= size)
                {
                    // пустые значения не добиваем нулями слева, дабы не сопоставились они с нулевыми кодами
                    return string.IsNullOrEmpty(val) ? val : val.PadLeft(size, '0');
                }
                else if (attributeType == DataAttributeTypes.dtString)
                {
                    return StringElephanter.TrampDown(val, settings);
                }
                else
                    return val;
            }

            /// <summary>
            /// Преобразует значение в строку с явным указанием размерности аттрибута
            /// </summary>
            /// <param name="dataRow">Строка содержащая значение атрибута</param>
            /// <param name="attribute">Описание атрибута</param>
            /// <param name="size">Размерность значения атрибута, которое нужно получить</param>
            /// <returns>Значение преобразованное в строку</returns>
            internal static string GetStringValue(DataRow dataRow, IDataAttribute attribute, bool isNumericContain, int size, StringElephanterSettings settings)
            {
                return GetStringValue(dataRow[attribute.Name], attribute.Name, attribute.Type, isNumericContain, size, settings);
            }

            /// <summary>
            /// Преобразует значение в строку
            /// </summary>
            /// <param name="dataRow">Строка содержащая значение атрибута</param>
            /// <param name="attribute">Описание атрибута</param>
            /// <returns>Значение преобразованное в строку</returns>
            internal static string GetStringValue(DataRow dataRow, IDataAttribute attribute, bool isNumericContain, StringElephanterSettings settings)
            {
                return GetStringValue(dataRow, attribute, isNumericContain, attribute.Size, settings);
            }
        }

        /// <summary>
        /// Оператод для вычисления значения
        /// </summary>
        internal class Operator
        {
            public string Bind;
            private string name;
            private List<Parameter> parameters;
            IEntity classifier;

            private Dictionary<string, object> methodArguments;

            public Operator(XmlNode xmlNode, IEntity classifier)
            {
                this.classifier = classifier;
                this.name = xmlNode.Attributes["name"].Value;
                XmlNode xmlBindAttr = xmlNode.Attributes["bind"];
                if (xmlBindAttr == null)
                    this.Bind = "Unbinded";
                else
                    this.Bind = xmlBindAttr.Value;

                XmlNodeList xmlParams = xmlNode.SelectNodes("Param");
                parameters = new List<Parameter>(xmlParams.Count);
                foreach (XmlNode item in xmlParams)
                {
                    parameters.Add(new Parameter(item, classifier));
                }
            }

            internal void Save2Xml(XmlNode node)
            {
                XmlHelper.SetAttribute(node, "name", this.name);
                if (this.Bind != "Unbinded")
                    XmlHelper.SetAttribute(node, "bind", this.Bind);

                foreach (Parameter item in parameters)
                {
                    XmlNode xmlParameter = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Param", null);

                    item.Save2Xml(xmlParameter);

                    node.AppendChild(xmlParameter);
                }
            }

            internal void Compile()
            {
                if (methodArguments != null)
                    return;

                ParameterInfo[] parametersInfo = typeof(AssociateOperations).GetMethod(name).GetParameters();

                methodArguments = new Dictionary<string, object>(parametersInfo.GetLength(0));
                foreach (ParameterInfo item in parametersInfo)
                {
                    methodArguments.Add(item.Name, null);
                }
            }

            internal object Compute(DataRow dataRow, StringElephanterSettings settings)
            {
                Compile();

                if (name == "Concatenate")
                {
                    object[] argsValue = new object[parameters.Count];

                    int i = 0;
                    foreach (Parameter item in parameters)
                    {
                        argsValue[i++] = Parameter.GetStringValue(dataRow, item.Attribute, false, settings);
                    }

                    return AssociateOperations.Invoke(name, argsValue);
                }
                else
                {
                    Dictionary<string, object> namedArgs = new Dictionary<string, object>(methodArguments);
                    foreach (Parameter item in parameters)
                    {
                        if (item.Attribute != null)
                            namedArgs[item.Bind] = Parameter.GetStringValue(dataRow, item.Attribute, false, settings);
                        else
                            namedArgs[item.Bind] = item.Value;
                    }

                    object[] args = new object[methodArguments.Count];
                    namedArgs.Values.CopyTo(args, 0);
                    return AssociateOperations.Invoke(name, args);
                }
            }

            internal string[] SourceAttributes
            {
                get
                {
                    List<string> attributes = new List<string>();

                    foreach (Parameter item in parameters)
                    {
                        attributes.Add(item.Attribute.Name);
                    }

                    return attributes.ToArray();
                }
            }
        }

        /// <summary>
        /// Наименование вычисляемого поля
        /// </summary>
        private string calculatedAttributeName;

        /// <summary>
        /// Описание атрибута содержащего значение
        /// </summary>
        private DataAttribute attribute;

        /// <summary>
        /// Выражение для вычисления значения
        /// </summary>
        private Operator rootOperator;

        /// <summary>
        /// Оприеделяет содержит ли значение число и необходимо ли его дополнять нулями.
        /// </summary>
        private bool isNumericContain;

        /// <summary>
        /// Тип значения.
        /// </summary>
        private DataAttributeTypes valueType;


        /// <summary>
        /// Используется для инициализации маппинга формирования классификаторов
        /// </summary>
        /// <param name="attribute"></param>
        public MappingValue(ServerSideObject owner, DataAttribute attribute)
            : base(owner, owner.State)
        {
            this.Attribute = attribute;
        }

        /// <summary>
        /// Используется для инициализации маппинга сопоставления
        /// </summary>
        /// <param name="xmlNode">XML-описание</param>
        /// <param name="classifier">Классификатор в котором хранится значение</param>
        public MappingValue(ServerSideObject owner, XmlNode xmlNode, IEntity classifier)
            : base(owner)
        {
            DataAttribute dataAttribute = DataAttributeCollection.GetAttributeByKeyName(classifier.Attributes, String.Empty, xmlNode.Attributes["name"].Value);
            if (dataAttribute != null)
                this.Attribute = dataAttribute;
            else
                this.calculatedAttributeName = xmlNode.Attributes["name"].Value;

            XmlNode xmlOperator = xmlNode.SelectSingleNode("Operator");
            if (xmlOperator != null)
                rootOperator = new Operator(xmlOperator, classifier);
        }

        /// <summary>
        /// Сохранение в XML элемент
        /// </summary>
        /// <param name="node"></param>
        internal void SaveToXml(XmlNode node)
        {
            XmlHelper.SetAttribute(node, "name", Name);

            if (rootOperator != null)
            {
                XmlNode xmlOperator = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Operator", null);

                rootOperator.Save2Xml(xmlOperator);

                node.AppendChild(xmlOperator);
            }
        }

        /// <summary>
        /// Наименование поля
        /// </summary>
        public string Name
        {
            get
            {
                // Если значение хранимое, то возвращается наименование хранимого в базе аттрибута
                // иначе возвращаем наименование вычисляемого атрибута
                if (Instance.Attribute != null)
                    return Instance.Attribute.Name;
                else
                    return Instance.calculatedAttributeName;
            }
        }

        /// <summary>
        /// Определяет простое значение или сложное составное
        /// </summary>
        public bool IsSample
        {
            get { return Instance.rootOperator == null; }
        }

        /// <summary>
        /// Описание атрибута содержащего значение
        /// </summary>
        public IDataAttribute Attribute
        {
            get { return Instance.attribute; }
            set { SetInstance.attribute = (DataAttribute)value; }
        }

        /// <summary>
        /// Тип значения.
        /// </summary>
        public DataAttributeTypes ValueType
        {
            get { return Instance.valueType; }
            internal set { SetInstance.valueType = value; }
        }

        /// <summary>
        /// Оприеделяет содержит ли значение число и необходимо ли его дополнять нулями.
        /// </summary>
        public bool IsNumericContain
        {
            get { return isNumericContain; }
            set { isNumericContain = value; }
        }

        /// <summary>
        /// Преобразует значение в строку. Если значение сложное, то дополнительно вычисляет его
        /// </summary>
        /// <param name="dataRow">Запись с исходными данными</param>
        internal string GetValue(DataRow dataRow, int size, StringElephanterSettings settings)
        {
            if (IsSample)
                return Parameter.GetStringValue(dataRow, Attribute, isNumericContain, size, settings);
            else
            {
                return Parameter.GetStringValue(
                    rootOperator.Compute(dataRow, settings),
                    "SystemCalculatedCode", DataAttributeTypes.dtString, isNumericContain, size, settings);
            }
        }

        /// <summary>
        /// Преобразует значение в строку. Если значение сложное, то дополнительно вычисляет его
        /// </summary>
        /// <param name="dataRow">Запись с исходными данными</param>
        internal string GetValue(DataRow dataRow, StringElephanterSettings settings)
        {
            return GetValue(dataRow, Attribute.Size, settings);
        }

        /// <summary>
        /// Наименования хранимых в базе аттрибутов на основе которых получается значение
        /// </summary>
        public string[] SourceAttributes
        {
            get
            {
                if (IsSample)
                    return new string[1] { Attribute.Name };
                else
                    return Instance.rootOperator.SourceAttributes;
            }
        }

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected MappingValue Instance
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return (MappingValue)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected MappingValue SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (MappingValue)CloneObject;
                else
                    return this;
            }
        }

        /*public override IServerSideObject Lock()
        {
            AssociateMapping ownerClone = (AssociateMapping)Owner.Lock();
            this.
            return ownerClone.;
        }*/

        /*public override object Clone()
        {
            return base.Clone();
        }*/

        #endregion ServerSideObject

        public override string ToString()
        {
            return String.Format("{0} : {1}", Instance.Name, base.ToString());
        }
    }
}
