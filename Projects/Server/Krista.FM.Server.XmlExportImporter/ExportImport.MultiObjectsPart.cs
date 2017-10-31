using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.XmlExportImporter
{
    public class MultiObjectsExportImporter
    {
        // сохраняем все объекты схемы в один файл. Сперва идут классификаторы, исключая фиксированные и сопоставимые, на котрые ссылаются данные
        // потом идут данные того классификатора или таблицы фактов, который собираемся загружать
        // все классификаторы - ссылки проверяем на наличие по параметрам код-наименование, если есть такие атрибуты, с соответствующим делением по источникам.
        // источник используем тот, который выбран у каждого из объектов схемы


    }
}
