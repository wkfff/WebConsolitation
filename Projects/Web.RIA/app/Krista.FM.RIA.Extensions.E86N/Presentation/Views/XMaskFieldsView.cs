using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class XMaskFieldsView : XView
    {
        public XMaskFieldsView(IScheme scheme, string config, IParametersService parametersService)
            : base(typeof(MaskFieldsView), scheme, config, parametersService)
        {
        }

        protected override void InitializeControl(
            Control control,
            XElement xConfig,
            Dictionary<string, string> parameters)
        {
            base.InitializeControl(control, xConfig, parameters);

            var formView = (MaskFieldsView)control;

            var entity = xConfig.Element("Entity");
            if (entity != null)
            {
                var objectKey = entity.Attribute("objectKey");
                if (objectKey != null)
                {
                    string entityKey = objectKey.Value;
                    entityKey = new Expression(ParametersService).Eval(entityKey);
                    formView.Entity = Scheme.RootPackage.FindEntityByName(entityKey);
                }

                var readOnly = entity.Attribute("readonly");
                if (readOnly != null)
                {
                    formView.Readonly = Convert.ToBoolean(readOnly.Value);
                }
                
                var useHierarchy = entity.Attribute("useHierarchy");
                if (useHierarchy != null)
                {
                    formView.UseHierarchy = Convert.ToBoolean(useHierarchy.Value);
                }
            }

            // обработка полей
            var fields = xConfig.Element("Fields");
            if (fields != null)
            {
                if (new Condition(ParametersService).Test(fields))
                {
                    foreach (var field in fields.Elements("Field"))
                    {
                        if (new Condition(ParametersService).Test(field))
                        {
                            var name = field.Attribute("name");
                            if (name != null)
                            {
                                var key = name.Value;
                                var attr = field.Attribute("width");
                                if (attr != null)
                                {
                                    formView.FieldsWidth.Add(key, Convert.ToInt32(attr.Value));
                                }

                                attr = field.Attribute("hide");
                                if (attr != null)
                                {
                                    var hide = Convert.ToBoolean(attr.Value);
                                    if (hide)
                                    {
                                        formView.HideFields.Add(key);
                                    }
                                }
                                
                                // маска ввода для полей
                                attr = field.Attribute("mask");
                                if (attr != null)
                                {
                                    formView.MaskRe.Add(key, attr.Value);
                                }
                            }
                        }
                    }
                }
            }

            var imports = xConfig.Element("ImpExps");
            if (imports != null)
            {
                if (new Condition(ParametersService).Test(imports))
                {
                    foreach (var import in imports.Elements())
                    {
                        if (new Condition(ParametersService).Test(import))
                        {
                            var action = import.Attribute("action");
                            var caption = import.Attribute("caption");
                            var ico = import.Attribute("ico");

                            if ((action != null) && (caption != null) && (ico != null))
                            {
                                formView.ImpExps.Add(
                                    new BtnMenuItems
                                        {
                                            Action = action.Value,
                                            Caption = caption.Value,
                                            Ico = ico.Value,
                                            Import = import.Name == "Import"
                                        });
                            }
                        }
                    }
                }
            }

            if (xConfig.Element("StoreService") != null)
            {
                var element = xConfig.Element("StoreService");
                if (element != null)
                {
                    var attribute = element.Attribute("type");
                    if (attribute != null)
                    {
                        Type storeServiceType = Type.GetType(attribute.Value);
                        formView.ViewService = Resolver.Get(storeServiceType) as IViewService;
                    }
                }
            }

            // Обработчики событий Store
            var storeListeners = xConfig.Element("StoreListeners");
            if (storeListeners != null)
            {
                foreach (var listener in storeListeners.Elements("Listener"))
                {
                    var attribute = listener.Attribute("name");
                    if (attribute != null)
                    {
                        formView.StoreListeners.Add(attribute.Value, listener.Value);
                    }
                }
            }
        }
    }
}
