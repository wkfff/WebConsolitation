namespace Krista.FM.RIA.Core.Gui
{
    public enum RowEditorFormViewParameterMode
    {
        /// <summary>
        /// Значение параметра передается в клиентский скрипт в виде сериализованного объекта.
        /// </summary>
        Value,

        /// <summary>
        /// Значение параметра передается в клиентский скрипт как есть.
        /// </summary>
        Raw
    }

    public class RowEditorFormViewParameterDescriptor
    {
        public string Name { get; set; }

        public string Value { get; set; }
        
        public RowEditorFormViewParameterMode Mode { get; set; }
    }
}