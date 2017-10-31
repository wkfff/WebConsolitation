using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Servises;
using View = Krista.FM.RIA.Core.Gui.View;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas.Presentation.Views
{
    public class MapView : View
    {
        private IAreaService areaService;

        public MapView(IAreaService areaService)
        {
            this.areaService = areaService;
        }

        public int? AreaId
        {
            get { return String.IsNullOrEmpty(Params["id"]) ? null : (int?)Convert.ToInt32(Params["id"]); }
        }

        public override List<Component> Build(ViewPage page)
        {
            var view = new Viewport
            {
                ID = "viewportMain",
                Items = { new FitLayout { Items = { GetBody() } } }
            };

            return new List<Component> { view };
        }

        private List<Component> GetBody()
        {
            var errorMessage = String.Empty;
            decimal coordinatesLat = 0;
            decimal coordinatesLng = 0;

            try
            {
                if (AreaId == null)
                {
                    throw new KeyNotFoundException("Не указана карточка.");
                }

                var areaModel = areaService.GetAreaModel((int)AreaId);

                if (areaModel.CoordinatesLat == null || areaModel.CoordinatesLng == null)
                {
                    throw new KeyNotFoundException("У данной карточки не указаны координаты территории");
                }

                coordinatesLat = (decimal)areaModel.CoordinatesLat;
                coordinatesLng = (decimal)areaModel.CoordinatesLng;
            }
            catch (Exception e)
            {
                return new List<Component> { new DisplayField { Text = e.Message } };
            }

            var googleMapsPanel = new Panel
                                      {
                                          ID = "googleMapPanel",
                                          Border = false
                                      };
            googleMapsPanel.AutoLoad.Url = "http://maps.google.com";
            googleMapsPanel.AutoLoad.Mode = LoadMode.IFrame;
            ////googleMapsPanel.AutoLoad.TriggerEvent = "show";
            googleMapsPanel.AutoLoad.NoCache = true;
            googleMapsPanel.AutoLoad.Params.Add(new Parameter("t", "h", ParameterMode.Value)); // тип карты - 
            googleMapsPanel.AutoLoad.Params.Add(new Parameter("z", "16", ParameterMode.Value));
            googleMapsPanel.AutoLoad.Params.Add(new Parameter("output", "embed", ParameterMode.Value));
                
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            googleMapsPanel.AutoLoad.Params.Add(new Parameter("q", "loc:{0},{1}".FormatWith(coordinatesLat.ToString(nfi), coordinatesLng.ToString(nfi)), ParameterMode.Value));
                                                         
            googleMapsPanel.AutoLoad.ShowMask = true;
            googleMapsPanel.AutoLoad.MaskMsg = "Загрузка...";

            return new List<Component> { googleMapsPanel };
        }
    }
}
