using System;
using System.Collections.Generic;
using Krista.FM.Update.Framework;

namespace Krista.FM.Update.PatchMakerLibrary 
{
    public interface IUpdetableSubProgram
    {
        /// <summary>
        /// Получает задачи обновления
        /// </summary>
        IUpdatePatch GetPatch(string patchName, string patchDescription, string patchDetailDescription, Use use, string baseUrl, string version, string displayName, string displayVersion, string installerVersion);
        /// <summary>
        /// Зависимости, создаваемые данной частью системы
        /// </summary>
        List<IUpdateCondition> DependentConditions { get; }
        /// <summary>
        /// Список зависимых подпрограмм
        /// </summary>
        List<Type> SubProgramDependentTypes { get; }
        /// <summary>
        /// Признак обработки данной подпрограммы
        /// </summary>
        bool IsHandle { get; }

        /// <summary>
        /// Имя канала обновления
        /// </summary>
        string GetFeedName();

        string Name { get; set; }
    }
}
