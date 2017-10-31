using System;

namespace Krista.FM.Common.OfficeHelpers
{
    public interface IOfficeApplication : IDisposable
    {
        /// <summary>
        /// создание документа по шаблону
        /// </summary>
        OfficeDocument CreateAsTemplate(string templatePath);
        
        /// <summary>
        /// Сохранение изменений.
        /// </summary>
        void SaveChanges(object docObj, string fileName);

        object RunMacros(string macrosName);
        object RunMacros(string macrosName, object rs);
        
        void Quit();
        
        bool Visible { get; set; }
    }
}