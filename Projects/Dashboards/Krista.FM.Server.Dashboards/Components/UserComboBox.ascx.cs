using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Components
{
    // обычный выпадающий список просто заполняется данными из файла
    public partial class UserComboBox : UserControl
    {
        // --------------------------------------------------------------------

        // имя файла с содержанием параметров
        private String file_name = "core\\UserComboBoxStuff.xml";
        public String FileName
        {
            get { return file_name; }
            set { file_name = value; }
        }

        // имя блока со списком параметров
        private String stuff_id = "default";
        public String StuffID
        {
            get { return stuff_id; }
            set { stuff_id = value; }
        }

        // высота элемента
        public Unit Height
        {
            get { return scombo.Height; }
            set { scombo.Height = value; }
        }

        // ширина элемента
        public Unit Width
        {
            get { return scombo.Width; }
            set { scombo.Width = value; }
        }

        // текущий элемент
        public ListItem SelectedItem
        {
            get { return scombo.SelectedItem; }
        }

        private String dir
        {
            get { return Server.MapPath("~") + "\\"; }
        }

        // --------------------------------------------------------------------

        // загрузка страницы
        protected void Page_Load(object sender, EventArgs e)
        {                        
        }

        // --------------------------------------------------------------------

        // инициализация страницы
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                UserInit();
            }
        }

        // --------------------------------------------------------------------

        // выполнение загрузки данных для выпадающего списка
        private Boolean UserInit() // nothrow
        {
            try
            {
                loadFromXML();
                return true;
            }
            catch (Exception e)
            {
                CRHelper.SaveToUserLog("Krista.FM.Server.Dashboards.core.UserComboBox: " + e.Message);                
                return false;
            }            
        }

        // --------------------------------------------------------------------

        // загрузка из текстого файла
        private void loadFromFile()
        {
            StreamReader reader = new StreamReader(dir + file_name);
            while (!reader.EndOfStream)
            {
                String region_str = reader.ReadLine().Trim();
                if (region_str == String.Empty) continue;
                scombo.Items.Add(new ListItem(getLastBlock(region_str), region_str));
            }
            reader.Close();
        }

        // --------------------------------------------------------------------

        // загрузка из XML файла
        private void loadFromXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(dir + file_name);
            XmlNode node = doc.SelectSingleNode(string.Format("//stuff[@id='{0}']", stuff_id));

            if (node != null)
            {
                String region_str = null;
                String[] list = node.InnerText.Split('\n');
                foreach (String str in list)
                {
                    region_str = str.Trim();
                    if (region_str == String.Empty) continue;
                    scombo.Items.Add(new ListItem(getLastBlock(region_str), region_str));
                }
            }
            else
            {
                throw new Exception("Can't find StuffID: '" + stuff_id + "' in file" + file_name);
            }
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Возвращает последний блок для MDX Member
         *  например: getLastBlock("[Территории].[РФ].[Все территории]") возвращает 'Все территории'
         *  </summary>
         */
        public static String getLastBlock(String mdx_member)
        {
            if (mdx_member == null) return null;
            String[] list = mdx_member.Split('.');
            Int32 index = list.Length - 1;
            String total = list[index];
            total = total.Replace("[", "");
            total = total.Replace("]", "");
            return total;
        }

        // --------------------------------------------------------------------
    }
}