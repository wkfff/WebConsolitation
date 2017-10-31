using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using Ext.Net;
using Ext.Net.Utilities;

namespace Krista.FM.RIA.Core.Gui.Map
{
    public static class MapControl
    {
        /// <summary>
        /// Добавляет скрипты для отображения карты
        /// (если карта редактируема, необходима кнопка с id="deletebuttonControl" для удаления объектов).
        /// </summary>
        /// <param name="page">Страница с картой.</param>
        /// <param name="resourceManager">Менеджер ресурсов.</param>
        /// <param name="coords">Список координат объекта.</param>
        /// <param name="canEdit">Можно ли редактировать расположение объекта.</param>
        /// <param name="regionCoords">Координаты региона.</param>
        /// <param name="canUsePolygon">Доступно ли рисование полигонов.</param>
        public static void AddMapScripts(Page page, Ext.Net.ResourceManager resourceManager, List<MapPoint> coords, bool canEdit, MapPoint regionCoords, bool canUsePolygon = true)
        {
            resourceManager.RegisterClientScriptInclude("GoogleMap", "http://maps.google.com/maps/api/js?sensor=false&libraries=drawing");

            Ext.Net.ResourceManager.GetInstance(page).RegisterScript("GoogleMapMethods", "/Krista.FM.RIA.Core/Gui/Map/Map.js/extention.axd");
            
            var nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
            //// Координаты центра карты. По умолчанию - долгота и широта региона.
            var lat = regionCoords.Lat.ToString(nfi);
            var lng = regionCoords.Lng.ToString(nfi);
            //// Если у объекта задано расположени, берем центр 
            //// Определяем как центр прямоугольника, описывающего объект - так как он весь должен быть виден на карте.
            if (coords.Any())
            {
                lat = (coords.Sum(x => x.Lat) / coords.Count()).ToString(nfi);
                lng = (coords.Sum(x => x.Lng) / coords.Count()).ToString(nfi);
            }

            //// Определяем приближение карты по координатам объекта.
            var zoom = GetZoom(coords);

            var coordsForMap = new JavaScriptSerializer().Serialize(coords);

            //// Добавляем скрипт для работы с картой.
            resourceManager.AddScript("Google.Map.AddMap({0}, {1}, {2}, {3}, {4}, {5});".FormatWith(lat, lng, zoom.ToString(), coordsForMap, canEdit.ToString().ToLower(), canUsePolygon.ToString().ToLower()));
        }

        /// <summary>
        /// Формирование компонентов карты.
        /// </summary>
        /// <param name="coords">Список координат объекта.</param>
        /// <param name="mapTitle">Заголовок карты.</param>
        /// <param name="canEdit">Редактируема ли карта (расположение объектов).</param>
        public static IEnumerable<Component> CreateMapElems(string coords, string mapTitle, bool canEdit)
        {
            var mapElems = new List<Component>();
            if (canEdit)
            {
                mapElems.Add(
                    new Panel
                        {
                            ID = "deletebuttonControl",
                            Hidden = false,
                            FieldLabel = String.Empty,
                            AutoHeight = true,
                            Border = false,
                            Buttons =
                                {
                                    new Button
                                        {
                                            ID = "deleteButton",
                                            AutoWidth = true,
                                            Text = @"Удалить объект на карте",
                                            Listeners = { Click = { Handler = "Google.Map.deleteSelectedShape();" } }
                                        }
                                }
                        });
            }

            //// Сюда сохраняются координаты объекта в формате json.
            var field = new TextField
            {
                ID = "Coordinates",
                ReadOnly = true,
                FieldLabel = @"Координаты",
                AllowBlank = true,
                Hidden = true,
                Value = coords
            };

            mapElems.Add(field);
            var googleMapsPanel = new Panel
            {
                ID = "googleMapPanel",
                Hidden = false,
                Width = 800,
                Height = 500,
                FieldLabel = mapTitle,
                StyleSpec = "height: 500, width: 300",
                AutoHeight = false,
                Border = true
            };

            googleMapsPanel.Style.Add("height", "400px");

            googleMapsPanel.AutoLoad.TriggerEvent = "show";

            googleMapsPanel.AutoLoad.ShowMask = true;
            googleMapsPanel.AutoLoad.MaskMsg = @"Загрузка...";

            mapElems.Add(googleMapsPanel);
            return mapElems;
        }

        /// <summary>
        /// Определние приближения карты по координатам объекта.
        /// </summary>
        /// <param name="coords">Список координат объекта.</param>
        private static int GetZoom(List<MapPoint> coords)
        {
            // Приближение по умолчанию.
            var zoom = 6;
            var coordsCnt = coords.Count;
            //// Для точки (маркера).
            if (coordsCnt == 1)
            {
                return 14;
            }
            
            if (coordsCnt > 1)
            {
                var maxLatCoord = coords.Max(x => x.Lat);
                var minLatCoord = coords.Min(x => x.Lat);
                var maxLngCoord = coords.Max(x => x.Lng);
                var minLngCoord = coords.Min(x => x.Lng);
                var latSize = maxLatCoord - minLatCoord;
                var lngSize = maxLngCoord - minLngCoord;
                if (latSize > 0 || lngSize > 0)
                {
                    // Определяем приближение, которое необходимо по долготе и по широте отдельно, берем наименьшее.
                    var latZoom = (int)Math.Log(180.0 / (double)latSize, 2);
                    var lngZoom = (int)Math.Log(360.0 / (double)lngSize, 2);
                    zoom = Math.Min(latZoom, lngZoom);
                }
            }

            return zoom;
        }
    }
}
