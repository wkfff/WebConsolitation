using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Grid.Painters;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Common.Xml;
using System.Xml;
using Infragistics.Win.UltraWinToolbars;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    public struct ToolbarUtils
    {
        #region Константы панели инструментов таблицы
        //Имена узлов в XML
        /// <summary>
        /// Узел в котором содержаться группы со стилями заголовка элемента отчета
        /// </summary>
        public const string elementCaptionStylesNodeName = "elementCaptionStyles";
        /// <summary>
        /// Узел в котором содержаться группы со стилями комментария элемента отчета
        /// </summary>
        public const string elementCommentStylesNodeName = "elementCommentStyles";
        /// <summary>
        /// Узел в котором содержаться группы со стилями ячеек таблицы
        /// </summary>
        public const string cellStylesNodeName = "cellStyles";
        /// <summary>
        /// Группа в которой содержаться стили
        /// </summary>
        public const string styleGroupNodeName = "styleGroup";
        /// <summary>
        /// Узел со стилем ячейки
        /// </summary>
        public const string cellStyleNodeName = "style";
        /// <summary>
        /// Узел со свойствами вкладок
        /// </summary>
        public const string tabsNodeName = "tabs";
        /// <summary>
        /// Вкладка со свойствами 
        /// </summary>
        public const string propertiesTabNodeName = "propertiesTab";

        //Атрибуты узлов
        /// <summary>
        /// Ширина ячейки стиля 
        /// </summary>
        public const string cellStyleWidth = "width";
        /// <summary>
        /// Высота ячейки стиля 
        /// </summary>
        public const string cellStyleHeight = "height";
        /// <summary>
        /// Текст отображающийся в ячейке
        /// </summary>
        public const string cellStyleText = "text";
        /// <summary>
        /// Количество колонок в галереи
        /// </summary>
        public const string galleryColumnCount = "columnCount";
        /// <summary>
        /// Атрибут в котором содержится текст (пояснения к чему либо)
        /// </summary>
        public const string groupName = "name";
        #endregion

        /// <summary>
        /// Иницилизирует галерею со стилями. Стили берет из указанного узла.  
        /// </summary>
        /// <param name="gallery">контролл</param>
        /// <param name="stylesNode">узел со стилями</param>
        public static void LoadStyleGallery(PopupGalleryTool gallery, XmlNode stylesNode)
        {
            if (stylesNode == null)
                return;
            gallery.Groups.Clear();
            gallery.Items.Clear();

            //количество колонок в галереи
            int columnCount = XmlHelper.GetIntAttrValue(stylesNode, galleryColumnCount, -1);
            if (columnCount != -1)
            {
                gallery.PreferredDropDownColumns = columnCount;
                gallery.MinDropDownColumns = columnCount;
                gallery.MaxDropDownColumns = columnCount;
            }

            int itemKey = 0;
            int groupKey = 0;
            //класс, который используется для рисования ячеек в таблице
            ProfessionalPainter painter = new ProfessionalPainter();

            //получим список групп со стилями
            XmlNodeList groupsNodes = stylesNode.SelectNodes(styleGroupNodeName);
            foreach (XmlNode groupNode in groupsNodes)
            {
                GalleryToolItemGroup galleryGroup = gallery.Groups.Add(groupKey.ToString());
                galleryGroup.Text = XmlHelper.GetStringAttrValue(groupNode, groupName, string.Empty);

                //получим список узлов со стилями
                XmlNodeList styleNodes = groupNode.SelectNodes(cellStyleNodeName);
                foreach (XmlNode styleNode in styleNodes)
                {
                    CellStyle cellStyle = new CellStyle(null, Color.White, Color.White, Color.Black, Color.Black);
                    cellStyle.Load(styleNode, false);
                    int width = XmlHelper.GetIntAttrValue(styleNode, cellStyleWidth, 100);
                    int height = XmlHelper.GetIntAttrValue(styleNode, cellStyleHeight, 20);
                    string text = XmlHelper.GetStringAttrValue(styleNode, cellStyleText, "Текст");
                    GalleryToolItem galleryItem = gallery.Items.Add(itemKey.ToString());
                    galleryItem.ToolTipText = text;
                    //получаем изображени ячейки с данным стилем
                    galleryItem.Settings.Appearance.Image = painter.GetImageCell(new Size(width, height), cellStyle,
                        text);
                    galleryItem.Tag = cellStyle;
                    //добовляем элемент в группу
                    galleryGroup.Items.Add(itemKey.ToString());
                    itemKey++;
                }
                groupKey++;
            }
        }

        /// <summary>
        /// Возвращает цвет вкладки, считывает его из XML
        /// </summary>
        /// <param name="tabsNode">узел с вкладками</param>
        /// <param name="tabName">имя вкладки, у которой будем брать цвет</param>
        /// <returns></returns>
        static public Color GetTabColor(XmlNode tabsNode, string tabName)
        {
            Color result = Color.Empty;
            if (tabsNode == null)
                return result;

            ColorConverter colorConvertor = new ColorConverter();
            //Получим узел c контролом
            XmlNode tabNode = tabsNode.SelectSingleNode(tabName);
            if (tabNode != null)
            {
                string sColor = XmlHelper.GetStringAttrValue(tabNode, Consts.backColor, string.Empty);
                if (sColor != string.Empty)
                {
                    result = (Color)colorConvertor.ConvertFromString(sColor);
                }
            }
            return result;
        }
    }
}
