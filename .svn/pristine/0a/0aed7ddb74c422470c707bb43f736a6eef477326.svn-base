using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Gui
{
    public class XGridView : XControl
    {
        public XGridView(IScheme scheme, string config, IParametersService parametersService) 
            : base(typeof(GridView), scheme, config, parametersService)
        {
        }

        protected override void InitializeControl(Control control, XElement xConfig, Dictionary<string, string> parameters)
        {
            var gridView = (GridModelControl)control;

            gridView.Id = xConfig.Attribute("id").Value;
            gridView.Title = xConfig.Attribute("title").Value;

            var xReadonly = xConfig.Attribute("readonly");
            if (xReadonly != null)
            {
                gridView.Readonly = Convert.ToBoolean(xReadonly.Value);
            }

            string entityKey = xConfig.Element("Entity").Attribute("objectKey").Value;
            entityKey = new Expression(ParametersService).Eval(entityKey);
            gridView.Entity = Scheme.RootPackage.FindEntityByName(entityKey);

            if (xConfig.Element("Presentation") != null)
            {
                string presentationKey = xConfig.Element("Presentation").Attribute("objectKey").Value;
                presentationKey = new Expression(ParametersService).Eval(presentationKey);
                if (gridView.Entity.Presentations.ContainsKey(presentationKey))
                {
                    gridView.Presentation = gridView.Entity.Presentations[presentationKey];
                }
            }

            if (xConfig.Element("RowEditorFormView") != null)
            {
                RowEditorFormViewDescriptor refvd = new RowEditorFormViewDescriptor();
                refvd.Id = xConfig.Element("RowEditorFormView").Attribute("viewId").Value;
                var xCustomView = xConfig.Element("RowEditorFormView").Element("CustomView");
                if (xCustomView != null)
                {
                    refvd.Url = xCustomView.Attribute("url").Value;
                    var xCustomViewParams = xCustomView.Element("Params");
                    if (xCustomViewParams != null)
                    {
                        foreach (var xParam in xCustomViewParams.Elements("Param"))
                        {
                            RowEditorFormViewParameterDescriptor param = new RowEditorFormViewParameterDescriptor();
                            param.Name = xParam.Attribute("name").Value;
                            param.Value = xParam.Attribute("value").Value;
                            param.Mode = RowEditorFormViewParameterMode.Value;
                            var xParamMode = xParam.Attribute("mode");
                            if (xParamMode != null)
                            {
                                param.Mode = (RowEditorFormViewParameterMode)Enum.Parse(
                                        typeof(RowEditorFormViewParameterMode),
                                        xParamMode.Value);
                            }
                            
                            if (param.Mode == RowEditorFormViewParameterMode.Value)
                            {
                                param.Value = new Expression(ParametersService).Eval(param.Value);
                            }

                            refvd.Params.Add(param);
                        }
                    }
                }

                gridView.RowEditorFormView = refvd;
            }

            if (xConfig.Element("StoreService") != null)
            {
                Type storeServiceType = Type.GetType(xConfig.Element("StoreService").Attribute("type").Value);
                gridView.ViewService = Resolver.Get(storeServiceType) as IViewService;
            }

            // Параметры
            var xParams = xConfig.Element("Params");
            if (xParams != null)
            {
                foreach (var xParameter in xParams.Elements("Parameter"))
                {
                    gridView.Params.Add(
                        xParameter.Attribute("name").Value,
                        xParameter.Attribute("value").Value);
                }
            }

            // Обработчики событий Store
            var xStoreListeners = xConfig.Element("StoreListeners");
            if (xStoreListeners != null)
            {
                foreach (var xListener in xStoreListeners.Elements("Listener"))
                {
                    gridView.StoreListeners.Add(
                        xListener.Attribute("name").Value,
                        xListener.Value);
                }
            }

            // Обработчики событий GridPanel
            var xGridListeners = xConfig.Element("GridListeners");
            if (xGridListeners != null)
            {
                foreach (var xListener in xGridListeners.Elements("Listener"))
                {
                    gridView.GridListeners.Add(
                        xListener.Attribute("name").Value,
                        xListener.Value);
                }
            }
        }
    }
}
