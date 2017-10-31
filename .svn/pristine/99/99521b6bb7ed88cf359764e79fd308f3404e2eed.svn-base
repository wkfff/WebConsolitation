using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// TypeConverter для списка
    /// </summary>
    class GaugePresetsConverter : StringConverter
    {

        /// <summary>
        /// Получаем список настроек индикатора
        /// </summary>
        /// <returns></returns>
        private StringCollection GetPresetList()
        {
            StringCollection presets = new StringCollection();
            if (Directory.Exists(Application.StartupPath + "\\GaugePresets"))
            {
                string[] files = Directory.GetFiles(Application.StartupPath + "\\GaugePresets\\", "*.xml");
                foreach (string fileName in files)
                {
                    presets.Add(Path.GetFileNameWithoutExtension(fileName));
                }

            }
            return presets;
        }

        /// <summary>
        /// Будем предоставлять выбор из списка
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            // false - можно вводить вручную
            // true - только выбор из списка
            return true;
        }

        /// <summary>
        /// А вот и список
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetPresetList());
        }

      
    }

}