﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.239
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Resources {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Krista.FM.RIA.Extensions.Region10MarksOIV.Resources.Resource", typeof(Resource).Assembly);
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
        ///   Ищет локализованную строку, похожую на ViewPersistence.refresh = function () {
        ///    var comboBox = this.marksCombo;
        ///    comboBox.clear();
        ///    comboBox.store.addListener(
        ///        &apos;load&apos;,
        ///        function () {
        ///            if (comboBox.store.data.length &gt; 0) {
        ///                comboBox.selectByIndex(0);
        ///            } else {
        ///                comboBox.clear();
        ///            }
        ///            comboBox.fireEvent(&apos;select&apos;);
        ///        },
        ///        this,
        ///        { single: true }
        ///    );
        ///    comboBox.store.reload();
        ///    comboBox.lastQuery = &apos;&apos;;
        ///};
        ///
        ///V [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string MarksCompareView {
            get {
                return ResourceManager.GetString("MarksCompareView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на ViewPersistence.refresh = function () {
        ///    this.dsMarks.reload();
        ///};
        ///
        ///ViewPersistence.isDirty = function () {
        ///    if (this.dsMarks.isDirty()) {
        ///        return true;
        ///    }
        ///    return false;
        ///};
        ///
        ///function saveData() {
        ///    dsMarks.save();
        ///};
        ///
        ///function processSaveQuestion(button) {
        ///    if (button == &apos;yes&apos;) {
        ///        //меняем статус у измененных строк
        ///        var records = dsMarks.getModifiedRecords();
        ///        for (var i = 0; i &lt; records.length; i++) {
        ///            var rec = records[i];
        ///      [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string MarksOivView {
            get {
                return ResourceManager.GetString("MarksOivView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на ViewPersistence.refresh = function () {
        ///    this.dsMarks.reload();
        ///};
        ///
        ///ViewPersistence.isDirty = function () {
        ///    if (this.dsMarks.isDirty()) {
        ///        return true;
        ///    }
        ///    return false;
        ///};
        ///
        ///function saveData() {
        ///    dsMarks.save();
        ///};
        ///
        ///function processSaveQuestion(button) {
        ///    if (button == &apos;yes&apos;) {
        ///        //меняем статус у измененных строк
        ///        var records = dsMarks.getModifiedRecords();
        ///        for (var i = 0; i &lt; records.length; i++) {
        ///            var rec = records[i];
        ///      [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string MarksOmsuView {
            get {
                return ResourceManager.GetString("MarksOmsuView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на function showCellTip() {
        ///    var rowIndex = gpMarks.view.findRowIndex(this.triggerElement),
        ///        cellIndex = gpMarks.view.findCellIndex(this.triggerElement),
        ///        record = dsMarks.getAt(rowIndex),
        ///        fieldName = gpMarks.getColumnModel().getDataIndex(cellIndex),
        ///        data = record.get(fieldName);
        ///
        ///    if (cellIndex == 2) {
        ///        if (data == 1) {
        ///            data = &apos;На редактировании&apos;;
        ///        } else if (data == 2) {
        ///            data = &apos;На рассмотрении&apos;;
        ///        } else if (data ==  [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string MarksTableControl {
            get {
                return ResourceManager.GetString("MarksTableControl", resourceCulture);
            }
        }
        
        internal static byte[] TemplateExportForOiv {
            get {
                object obj = ResourceManager.GetObject("TemplateExportForOiv", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] TemplateExportForOmsuCompare {
            get {
                object obj = ResourceManager.GetObject("TemplateExportForOmsuCompare", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
