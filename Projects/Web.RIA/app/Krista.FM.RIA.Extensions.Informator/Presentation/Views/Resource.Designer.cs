﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.269
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Krista.FM.RIA.Extensions.Informator.Presentation.Views {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Krista.FM.RIA.Extensions.Informator.Presentation.Views.Resource", typeof(Resource).Assembly);
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
        ///   Ищет локализованную строку, похожую на Ext.ns(&apos;Informator.View.News&apos;);
        ///
        ///Informator.View.News.Grid =
        ///{
        ///
        ///    showSendWin: function() 
        ///    {
        ///
        ///        var myStore = Informator.View.News.Grid.getStore(&apos;getAdressBook&apos;);
        ///
        ///        form = new Ext.form.FormPanel
        ///        ({
        ///            border: false,
        ///            layout: &apos;anchor&apos;,
        ///            id: &apos;panel&apos;,
        ///            //bodyStyle: { &apos;background-color&apos;: &apos;#E0E6F8&apos; },
        ///            items: 
        ///            [
        ///                {
        ///                    xtype: &apos;combo&apos;,
        ///                    id: &apos;recipients&apos;, [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string NewsView {
            get {
                return ResourceManager.GetString("NewsView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ext.onReady(function () {
        ///    debugger;
        ///
        ///    var extension = document.getElementById(&quot;acE86NExtension&quot;);
        ///    extension.addEventListener(&quot;afterLoad&quot;, function () {
        ///
        ///        var myProxy = new Ext.data.HttpProxy
        ///    ({
        ///        url: &apos;/News/&apos;,
        ///        api:
        ///        {
        ///            read: { url: &apos;/News/ReadNewMessages&apos;, method: &apos;GET&apos; },
        ///            create: { url: &apos;/News/SendMessage&apos;, method: &apos;POST&apos; }
        ///        },
        ///        timeout: 60000
        ///    });
        ///
        ///        var myReader = new Ext.data.JsonReader
        ///    ({
        ///   [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string NewsWindow {
            get {
                return ResourceManager.GetString("NewsWindow", resourceCulture);
            }
        }
    }
}