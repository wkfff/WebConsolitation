﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.1
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Krista.FM.RIA.Extensions.FO41.Resources {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Krista.FM.RIA.Extensions.FO41.Resources.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на // добавляем сообщение при закрытии вкладки с заявкой
        ///var beforeCloseTab = function (currentTab) {
        ///    if (currentTab.forceClose != undefined &amp;&amp; currentTab.forceClose) {
        ///        return true;
        ///    }
        ///    if (isDirty()) {
        ///        Ext.Msg.show({
        ///            title: &apos;Внимание&apos;,
        ///            msg: &apos;Все несохраненные изменения будут потеряны. Сохранить заявку?&apos;,
        ///            buttons: { yes: &apos;Сохранить&apos;, no: &apos;Закрыть&apos;, cancel: &apos;Отмена&apos;},
        ///            animEl: &apos;viewportmain&apos;,
        ///            icon: Ext.MessageBox.WAR [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string RequestViewJs {
            get {
                return ResourceManager.GetString("RequestViewJs", resourceCulture);
            }
        }
        
        internal static byte[] TemplateExportEstimateCategoryHMAO {
            get {
                object obj = ResourceManager.GetObject("TemplateExportEstimateCategoryHMAO", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] TemplateExportResultCategotyHMAO {
            get {
                object obj = ResourceManager.GetObject("TemplateExportResultCategotyHMAO", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] TemplateExportResultData {
            get {
                object obj = ResourceManager.GetObject("TemplateExportResultData", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] TemplateExportResultTaxHMAO {
            get {
                object obj = ResourceManager.GetObject("TemplateExportResultTaxHMAO", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] TemplateExportToOGV {
            get {
                object obj = ResourceManager.GetObject("TemplateExportToOGV", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] TemplateExportToTaxpayer {
            get {
                object obj = ResourceManager.GetObject("TemplateExportToTaxpayer", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] TemplateExportToTaxpayerHMAO {
            get {
                object obj = ResourceManager.GetObject("TemplateExportToTaxpayerHMAO", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
