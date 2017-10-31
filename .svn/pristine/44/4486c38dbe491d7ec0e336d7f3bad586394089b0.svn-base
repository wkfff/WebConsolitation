using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.SchemeEditor;
using Krista.FM.Client.SchemeEditor.DiargamEditor;
using Krista.FM.ServerLibrary;
using Krista.FM.Common.Exceptions;
using System.Xml.XPath;

namespace Krista.FM.Client.Help
{
    /// <summary>
    /// Вариант генерации справки
    /// </summary>
    public enum  HelpVariant : int
    {
        // добавление диаграмм
        diagramAdd = 0,

        // создание диаграмм заново
        diagramFull = 1
    }

    /// <summary>
    /// Режим генерации справки
    /// </summary>
    public enum HelpMode : int
    {
        // режим пользователя
        userMode = 0,

        // режим разработчика
        developerMode = 1,

        // lite mode
        liteMode = 2
    }

    /// <summary>
    /// Класс для управления процессом создания справки по семантической структуре
    /// </summary>
    public class HelpManager
    {
        /// <summary>
        /// Схема
        /// </summary>
        private SchemeEditor.SchemeEditor schemeEditor;

        /// <summary>
        /// Событие, символизируещее о завершении этапа создания XML-описания схемы
        /// </summary>
        private event EventHandler OnFinishXMLCreate;

        /// <summary>
        /// вариант генерации справки
        /// </summary>
        private HelpVariant variant;
                
        /// <summary>
        /// Режим генерации справки
        /// </summary>
        private HelpMode mode;
                

        #region Const

        /// <summary>
        /// Корневой каталог к исходным файлам для генерации справки по семантической структуре
        /// </summary>
        public const string profile = "HelpGeneratorProfileUML";
        /// <summary>
        /// Корневой каталог к файлам-ресурсам для генерации справки по семантической структуре
        /// </summary>
        public const string result = "HelpGeneratorResultUML";

        #endregion Const

        #region Конструктор

        public HelpManager(HelpVariant variant, HelpMode mode)
        {
            this.variant = variant;
            this.mode = mode;
            this.schemeEditor = SchemeEditor.SchemeEditor.Instance;
        }

        #endregion Конструктор

        #region Properties

        public HelpVariant Variant
        {
            get { return variant; }
        }

        public HelpMode Mode
        {
            get { return mode; }
        }

        #endregion Properties

        public void OnFinishXml(EventArgs args)
        {
            if (OnFinishXMLCreate != null)
                OnFinishXMLCreate(this, args);
        }

        private void CallFinishXml()
        {
            EventArgs args = new EventArgs();
            OnFinishXml(args);
        }

        #region Основная функция генерации справки

        public void HelpGenerator()
        {
            Operation operation = new Operation();
            try
            {
                operation.Text = "Начало генерации справки";
                operation.StartOperation();
                // Создание каталогов выгрузки справки
                operation.Text = "Создание каталогов выгрузки справки";
                CreateWorkFolder();
                // Создание XML-описания схемы
                operation.Text = "Создание XML-описания схемы";
                CreateXML();
                // Сохранение диаграмм
                operation.Text = "Сохранение диаграмм";
                schemeEditor.SaveAllDigrams(result, (variant == HelpVariant.diagramAdd)? true:false);
                // Преобразование из XML в HTML
                operation.Text = "Преобразование из XML в HTML";
                XSLTTransform();
                // Создание метаданных справки и индексов
                operation.Text = "Создание метаданных справки и индексов";
                CreateIndex();

                operation.StopOperation();

               MessageBox.Show("Генерация справки успешно завершена", "Генерация справки", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch(HelpException e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                operation.ReleaseThread();
            }
        }

        private static void CreateIndex()
        {
            XslCompiledTransform xslt = new XslCompiledTransform();

            // Индексы
            // Выбор шаблона для преобразования должен зависеть от режима генерации справки
            xslt.Load(String.Format(@"{0}\HHK.xsl", profile));
            xslt.Transform(String.Format(@"{0}\output.xml", result), String.Format(@"{0}\Index.hhk", result));
            // Структура
            xslt.Load(String.Format(@"{0}\HHC.xsl", profile));
            xslt.Transform(String.Format(@"{0}\output.xml", result), String.Format(@"{0}\TOC.hhc", result));
            // Метаданные справки
            xslt.Load(String.Format(@"{0}\HHP.xsl", profile));
            xslt.Transform(String.Format(@"{0}\output.xml", result), String.Format(@"{0}\scheme.hhp", result));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        private void XSLTTransform()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format(@"{0}\output.xml", result));

                HeadSave head = new HeadSave(this);
                head.SaveToHtml(null);

                // сохранение пакетов
                XmlNodeList list = doc.SelectNodes("//ServerConfiguration//Package");

                if (list.Count != 0)
                {
                    foreach (XmlNode node in list)
                    {
                        PackageSave package = new PackageSave(this);
                        package.SaveToHtml(node);
                    }
                }

                // источники информации
                SuppliersSave suppl = new SuppliersSave(this);
                suppl.SaveToHtml(null);

                // список семантик
                SemanticsSave sem = new SemanticsSave(this);
                sem.SaveToHtml(null);
            }
            catch (XmlException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (XPathException ex)
            {
                throw new HelpException(ex.ToString());
            }
        }
                

        #endregion Основная функция генерации справки

        /// <summary>
        /// Создание каталогов выгрузки справки
        /// </summary>
        private void CreateWorkFolder()
        {
            //корневой каталог
            DirectoryInfo dirInfo = new DirectoryInfo(".");
            if (!Directory.Exists(result))
                dirInfo.CreateSubdirectory(result);

            //диаграммы
            if (Directory.Exists(String.Format(@"{0}\diagrams", result)))
                if(variant == HelpVariant.diagramFull)
                    Directory.Delete(String.Format(@"{0}\diagrams", result), true);
            if (!Directory.Exists(String.Format(@"{0}\diagrams", result)))
                dirInfo.CreateSubdirectory(String.Format(@"{0}\diagrams", result));

            // классы
            if (Directory.Exists(String.Format(@"{0}\classes", result)))
                Directory.Delete(String.Format(@"{0}\classes", result), true);

            dirInfo.CreateSubdirectory(String.Format(@"{0}\classes", result));

            // пакеты
            if (Directory.Exists(String.Format(@"{0}\packages", result)))
                Directory.Delete(String.Format(@"{0}\packages", result), true);

            dirInfo.CreateSubdirectory(String.Format(@"{0}\packages", result));

            // ресурсы
            if (Directory.Exists(String.Format(@"{0}\Resources", result)))
                Directory.Delete(String.Format(@"{0}\Resources", result), true);

            dirInfo.CreateSubdirectory(String.Format(@"{0}\Resources", result));

            // !временный вариант сохранения рисунков(можно перенести в build event - post-build)!
            Resource1.all.Save(string.Format(@"{0}\Resources\all.gif", result));
            Resource1.association.Save(string.Format(@"{0}\Resources\association.gif", result));
            Resource1.ASSOCIATIONBRIDGE.Save(string.Format(@"{0}\Resources\ASSOCIATIONBRIDGE.gif", result));
            Resource1.attribute.Save(string.Format(@"{0}\Resources\attribute.gif", result));
            Resource1.bridgeCls.Save(string.Format(@"{0}\Resources\bridgeCls.gif", result));
            Resource1.CLASSES.Save(string.Format(@"{0}\Resources\CLASSES.gif", result));
            Resource1.documents.Save(string.Format(@"{0}\Resources\documents.gif", result));
            Resource1.factCls.Save(string.Format(@"{0}\Resources\factCls.gif", result));
            Resource1.fixedCls.Save(string.Format(@"{0}\Resources\fixedCls.gif", result));
            Resource1.HIERARCHY.Save(string.Format(@"{0}\Resources\Hierarchy.gif", result));
            Resource1.kd.Save(string.Format(@"{0}\Resources\kd.gif", result));
            Resource1.logo_Krista2.Save(string.Format(@"{0}\Resources\logo_Krista2.jpg", result));
            Resource1.masterdetail.Save(string.Format(@"{0}\Resources\masterdetail.gif", result));
            Resource1.memberKey.Save(string.Format(@"{0}\Resources\memberKey.gif", result));
            Resource1.memberName.Save(string.Format(@"{0}\Resources\memberName.gif", result));
            Resource1.package.Save(string.Format(@"{0}\Resources\package.gif", result));
            Resource1.parentKey.Save(string.Format(@"{0}\Resources\parentKey.gif", result));
            Resource1.tableCls.Save(string.Format(@"{0}\Resources\tableCls.gif", result));
            Resource1.lev0.Save(string.Format(@"{0}\Resources\lev0.gif", result));
            Resource1.lev1.Save(string.Format(@"{0}\Resources\lev1.gif", result));
            Resource1.lev2.Save(string.Format(@"{0}\Resources\lev2.gif", result));
            Resource1.lev3.Save(string.Format(@"{0}\Resources\lev3.gif", result));
            Resource1.lev4.Save(string.Format(@"{0}\Resources\lev4.gif", result));
            Resource1.lev5.Save(string.Format(@"{0}\Resources\lev5.gif", result));
            Resource1.lev6.Save(string.Format(@"{0}\Resources\lev6.gif", result));
            Resource1.lev7.Save(string.Format(@"{0}\Resources\lev7.gif", result));
            Resource1.lev8.Save(string.Format(@"{0}\Resources\lev8.gif", result));
            Resource1.lev9.Save(string.Format(@"{0}\Resources\lev9.gif", result));
            Resource1.lev10.Save(string.Format(@"{0}\Resources\lev10.gif", result));
            Resource1.lev11.Save(string.Format(@"{0}\Resources\lev11.gif", result));
            Resource1.lev12.Save(string.Format(@"{0}\Resources\lev12.gif", result));
            Resource1.lev13.Save(string.Format(@"{0}\Resources\lev13.gif", result));
            Resource1.lev14.Save(string.Format(@"{0}\Resources\lev14.gif", result));
            Resource1.lev15.Save(string.Format(@"{0}\Resources\lev15.gif", result));
            Resource1.lev16.Save(string.Format(@"{0}\Resources\lev16.gif", result));
            Resource1.AttributeKey.Save(string.Format(@"{0}\Resources\AttributeKey.gif", result));
            Resource1.AttributeServ.Save(string.Format(@"{0}\Resources\AttributeServ.gif", result));
            Resource1.AttributeLock.Save(string.Format(@"{0}\Resources\AttributeLock.gif", result));
            Resource1.AttributeLink.Save(string.Format(@"{0}\Resources\AttributeLink.gif", result));

            File.Copy(String.Format(@"{0}\main.css", profile), String.Format(@"{0}\Resources\main.css", result), true);
        }

        private void CreateXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(schemeEditor.Scheme.ConfigurationXMLDocumentation);

            Krista.FM.Common.Xml.XmlHelper.Save(doc, String.Format(@"{0}\output.xml", result));
        }
         
        /// <summary>
        /// Заголовок справки по структуре
        /// </summary>
        /// <param name="mode"> Режим генерации справки</param>
        /// <returns></returns>
        public static string HelpHeaderName(HelpMode mode)
        {
            switch (mode)
            {
                case HelpMode.developerMode:
                    {
                        return "Справочник по семантической структуре (для разработчиков)";
                    }
                case HelpMode.userMode:
                    {
                        return "Справочник по семантической структуре";
                    }
                case HelpMode.liteMode:
                    {
                        return "Справочник по семантической структуре (упрощенный)";
                    }
                default:
                    throw new Exception(String.Format("Необработанный режим генерации справки: {0}", mode.ToString()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CheckDocumentName(string name)
        {
            char[] illegalCharacters = new char[] { ':', '/', '\\', '|', '*', '<', '>', '?', '"' };
            for (int i = 0; i < illegalCharacters.Length; i++)
            {
                if (name.IndexOf(illegalCharacters[i]) > -1)
                {
                    name = name.Replace(illegalCharacters[i], '_');
                }
            }

            return name;
        }
    }
}
