﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.1
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Krista.FM.Client.TreeLogger.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Krista.FM.Client.TreeLogger.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Ищет локализованную строку, похожую на Восстановление многомерной базы.
        /// </summary>
        internal static string defaultLogFileName {
            get {
                return ResourceManager.GetString("defaultLogFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Время выполнения: .
        /// </summary>
        internal static string durationNodeCaption {
            get {
                return ResourceManager.GetString("durationNodeCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Завершение: .
        /// </summary>
        internal static string endNodeCaption {
            get {
                return ResourceManager.GetString("endNodeCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Всего ошибок: .
        /// </summary>
        internal static string errorNodeCaption {
            get {
                return ResourceManager.GetString("errorNodeCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Выполнена успешно.
        /// </summary>
        internal static string opStatusFinishedOK {
            get {
                return ResourceManager.GetString("opStatusFinishedOK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Выполнена с ошибками.
        /// </summary>
        internal static string opStatusFinishedWithErrors {
            get {
                return ResourceManager.GetString("opStatusFinishedWithErrors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Выполнена с предупреждениями.
        /// </summary>
        internal static string opStatusFinishedWithWarnings {
            get {
                return ResourceManager.GetString("opStatusFinishedWithWarnings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Информация.
        /// </summary>
        internal static string opStatusInfo {
            get {
                return ResourceManager.GetString("opStatusInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Выполняется.
        /// </summary>
        internal static string opStatusRunning {
            get {
                return ResourceManager.GetString("opStatusRunning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ожидание.
        /// </summary>
        internal static string opStatusWaiting {
            get {
                return ResourceManager.GetString("opStatusWaiting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Начало: .
        /// </summary>
        internal static string startNodeCaption {
            get {
                return ResourceManager.GetString("startNodeCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Всего предупреждений: .
        /// </summary>
        internal static string warningNodeCaption {
            get {
                return ResourceManager.GetString("warningNodeCaption", resourceCulture);
            }
        }
    }
}
