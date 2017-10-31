Ext.ns("Google.Map");

Google.Map = {
    // Выделенный объект.
    selectedShape: null,

    // Актуальный объект.
    upToDateShape: null,
    
    map: null,

    // Получение координат актуального объекта.
    GetCoords: function(isMarker) {
        var resultPaths = '[';
        if (window.Google.Map.upToDateShape && window.Google.Map.upToDateShape.map) {
            if (isMarker) {
                var coords = window.Google.Map.upToDateShape.getPosition();
                resultPaths += '{\"Lat\":' + coords.lat() + ', \"Lng\":' + coords.lng() + '}';
            } else {
                var paths = window.Google.Map.upToDateShape.getPath();
                for (var j = 0; j < paths.length; j++) {
                    resultPaths += '{\"Lat\":' + paths.getAt(j).lat() + ', \"Lng\":' + paths.getAt(j).lng() + '},';
                }
                resultPaths[resultPaths.length - 1] = '';
            }
        }
        resultPaths += ']';
        return resultPaths;
    },
    
    // Удаление выделенного объекта.
    deleteSelectedShape: function() {
        if (window.Google.Map.selectedShape) {
            window.Google.Map.selectedShape.setMap(null);
            window.Google.Map.selectedShape = null;
        }
    },
    
    moveMarker: function (lat, lng) {
        Google.Map.moveMarkerNotCenter(lat, lng);
        if (lat != null && lng != null) {
            map.setCenter(new google.maps.LatLng(lat, lng));
        }
    },
    
    moveMarkerNotCenter: function(lat, lng) {
        if (lat == null || lng == null) {
            window.Google.Map.deleteSelectedShape();
            if (window.Google.Map.upToDateShape) {
                window.Google.Map.upToDateShape.setMap(null);
                window.Google.Map.upToDateShape = null;
            }
        } else {
            window.Google.Map.deleteSelectedShape();
            if (window.Google.Map.upToDateShape) {
                window.Google.Map.upToDateShape.setMap(null);
                window.Google.Map.upToDateShape = null;
            }

            var point = new google.maps.LatLng(lat, lng);
            if (window.Google.Map.upToDateShape == null) {
                window.Google.Map.upToDateShape = new google.maps.Marker({
                    position: point,
                    map: map
                });
            } else {
                window.Google.Map.upToDateShape.setPosition(point);
            }

            window.Google.Map.selectedShape = window.Google.Map.upToDateShape;
        }    
    },

    // Создание карты и отрисовка полигона (если он есть).
    // Параметры:
    // lat - широта, 
    // lng - долгота, 
    // zoom - приближение, 
    // coords - cписок координат объекта,
    // canEdit - редактируема ли карта (расположение объекта).
    AddMap: function (lat, lng, zoom, coords, canEdit, canUsePolygon) {
        // Преобразуем к координатам карт.
        var polygonCoords = [];
        window.Ext.each(coords, function(coord) {
            polygonCoords.push(new google.maps.LatLng(coord.Lat, coord.Lng));
        });
        
        // Центр карты.
        var latlng = new google.maps.LatLng(lat, lng);

        // Свойства карты.
        var myOptions = {
            zoom: zoom, // приближение
            center: latlng, // центр
            mapTypeId: google.maps.MapTypeId.HYBRID, // тип карты
            scrollwheel: false, // скрол колесиком мышки
            draggableCursor: 'crosshair',
            draggingCursor: 'move'
        };

        // Создаем карту.
        map = new google.maps.Map(document.getElementById('googleMapPanel'), myOptions);

        // Менеджер работы с картой (набор инструменов).
        var drawingManager;
        
        /*var colors = ['#1E90FF', '#FF1493', '#32CD32', '#FF8C00', '#4B0082'];*/
        // Цвет объекта.
        var selectedColor = "#1E90FF";

        // Если расположение объекта задано
        if (polygonCoords.length > 0) {
            if (polygonCoords.length == 1) {
                // Отрисовываем маркер.
                window.Google.Map.selectedShape = new google.maps.Marker({
                    map: map,
                    position: new google.maps.LatLng(polygonCoords[0].lat(), polygonCoords[0].lng())
                });
                window.Google.Map.upToDateShape = window.Google.Map.selectedShape;
            }
            else
            {
                // Отрисовываем полигон.
                window.Google.Map.selectedShape = new google.maps.Polygon({
                    paths: polygonCoords,
                    strokeColor: selectedColor,
                    fillColor: selectedColor,
                    strokeOpacity: 0.8,
                    strokeWeight: 3,
                    fillOpacity: 0.35,
                    editable: true
                });
            }
            
            window.Google.Map.selectedShape.setMap(map);
        }
        
        // Изначально выделенный и актуальный полигон совпадают.
        window.Google.Map.upToDateShape = window.Google.Map.selectedShape;
        if (window.Google.Map.upToDateShape) {
            // Добавляем событие на выделение объекта, когда пользователь щелкает по нему.
            google.maps.event.addListener(window.Google.Map.upToDateShape, 'click', function() {
                setSelection(window.Google.Map.upToDateShape);
            });
        }

        // Если карта доступна для редактирования:
        if (canEdit) {
            if (canUsePolygon) {
                // Обработчик на завершение выделения объекта (полигона).
                google.maps.event.addListener(map, 'polygoncomplete', function(polygon) {
                    window.Google.Map.selectedShape = polygon;
                    window.Google.Map.selectedShape.setMap(null);
                });
                /*// Обработчик на завершение выделения объекта (ломаной).
                google.maps.event.addListener(map, 'polylinecomplete', function(polyline) {
                    window.Google.Map.selectedShape = polyline;
                    window.Google.Map.selectedShape.setMap(null);
                });*/

                var polyOptions = {
                    strokeWeight: 0,
                    fillOpacity: 0.45,
                    editable: true
                };
            }

            // Создаем набор инструменов на карте, которые позволят пользователю рисовать объекты (полигоны и м.б. ломаные)
            drawingManager = new google.maps.drawing.DrawingManager({
                drawingMode: null,
                drawingControlOptions: {
                    position: google.maps.ControlPosition.TOP_LEFT,
                    drawingModes: []
                },
                polylineOptions: {
                    editable: true
                },
                rectangleOptions: polyOptions,
                circleOptions: polyOptions,
                polygonOptions: polyOptions,
                map: map
            });

            if (canUsePolygon) {
                var len = drawingManager.drawingControlOptions.drawingModes.length;
                drawingManager.drawingControlOptions.drawingModes[len] = google.maps.drawing.OverlayType.POLYGON;
            }
            else {
                var len = drawingManager.drawingControlOptions.drawingModes.length;
                drawingManager.drawingControlOptions.drawingModes[len] = google.maps.drawing.OverlayType.MARKER;

                google.maps.event.addListener(map, 'click', function (event) {
                    window.Google.Map.moveMarkerNotCenter(event.latLng.lat(), event.latLng.lng());
                });
            }

            /*google.maps.drawing.OverlayType.MARKER,
            google.maps.drawing.OverlayType.CIRCLE,
            google.maps.drawing.OverlayType.POLYLINE
            google.maps.drawing.OverlayType.RECTANGLE*/

            // Устанавливаем цвет объектов.
            if (canUsePolygon) {
                selectColor(selectedColor);
            }

            // Добавляем обработчик на завершение создания объекта.
            google.maps.event.addListener(drawingManager, 'overlaycomplete', function (e) {
                if (e.type != google.maps.drawing.OverlayType.MARKER) {
                    // Выбираем режим "Не рисуем" после завершения рисования объекта.
                    drawingManager.setDrawingMode(null);

                    // Добавляем событие на выделение нового объекта, когда пользователь щелкает по нему.
                    var newShape = e.overlay;
                    newShape.type = e.type;
                    google.maps.event.addListener(newShape, 'click', function() {
                        setSelection(newShape);
                    });

                    // Проверяем, был ли ранее уже был нарисован объект:
                    if (!window.Google.Map.upToDateShape) {
                        // Если нет - настраиваем новый объект как актуальный и выделенный.
                        deleteUpToDateShape();
                        setSelection(newShape);
                    } else {
                        // Иначе спрашиваем у пользователя - какое расположение объекта использовать - старое или новое (вдруг случайно нарисовал).
                        window.Ext.Msg.show({
                            title: 'Анализ и планирование',
                            width: 350,
                            msg: 'Изменить расположение объекта? (Да - использовать новое расположение, Нет - использовать прежнее расположение)',
                            buttons: window.Ext.MessageBox.YESNO,
                            fn: function(btn) {
                                if (btn == 'yes') {
                                    // Настраиваем новый объект как актуальный и выделенный.
                                    deleteUpToDateShape();
                                    setSelection(newShape);
                                } else {
                                    // Удаляем новый объект
                                    if (newShape) {
                                        newShape.setMap(null);
                                    }
                                    // Делаем актуальным прежний.
                                    setSelection(window.Google.Map.upToDateShape);
                                }
                            }
                        });
                    }
                }
            });

            // Снимаем текущее выделение, когда изменяется способ рисования или происходит клик по карте.
            if (canUsePolygon) {
                google.maps.event.addListener(drawingManager, 'drawingmode_changed', clearSelection);
                google.maps.event.addListener(map, 'click', clearSelection);
            }
        }

        // Снятие выделения с объекта.
        function clearSelection() {
            if (window.Google.Map.selectedShape) {
                if (canUsePolygon) {
                    window.Google.Map.selectedShape.setEditable(false);
                }
                window.Google.Map.selectedShape = null;
            }
        }

        // Устанавка выделения у объекта.
        function setSelection(shape) {
            // Снимаем прежнее выделение
            clearSelection();
            // Делаем объект выбранным
            window.Google.Map.selectedShape = shape;
            // И актуальным
            window.Google.Map.upToDateShape = shape;
            // Делаем редактируемым и устанавливаем цвет.
            if (shape) {
                if (canUsePolygon) {
                    shape.setEditable(true);
                }
                selectColor(selectedColor);
            }
        }

        // Удаление актуального объекта
        function deleteUpToDateShape() {
            if (window.Google.Map.upToDateShape) {
                window.Google.Map.upToDateShape.setMap(null);
                window.Google.Map.upToDateShape = null;
            }
        }

        // Установка цвета для объектов
        function selectColor(color) {
            /*// Для ломаных.
            var polylineOptions = drawingManager.get('polylineOptions');
            polylineOptions.strokeColor = color;
            polylineOptions.fillColor = color;
            drawingManager.set('polylineOptions', polylineOptions);
    
            // Для четырехугольников.
            var rectangleOptions = drawingManager.get('rectangleOptions');
            rectangleOptions.strokeColor = color;
            rectangleOptions.fillColor = color;
            drawingManager.set('rectangleOptions', rectangleOptions);
    
            // Для кругов.
            var circleOptions = drawingManager.get('circleOptions');
            circleOptions.strokeColor = color;
            circleOptions.fillColor = color;
            drawingManager.set('circleOptions', circleOptions);*/

            // Для полигонов.
            var polygonOptions = drawingManager.get('polygonOptions');
            polygonOptions.strokeColor = color;
            polygonOptions.fillColor = color;
            drawingManager.set('polygonOptions', polygonOptions);
        }
    }
};