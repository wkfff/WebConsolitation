using System.Reflection;

using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainStore
{
    public interface IDomainFormsAssembliesStore
    {
        /// <summary>
        /// Создает и добавляет в хранилище новую доменную сборку для указанной формы.
        /// </summary>
        /// <param name="form">Форма для которой нужно создать доменную сборку.</param>
        void Register(D_CD_Templates form);

        /// <summary>
        /// Возвращает все зарегистрированные доменные сборки.
        /// </summary>
        Assembly[] GetAllAssemblies();
    }
}