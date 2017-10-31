using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.StateTaskService
{
    public interface IStateTaskService : INewRestService
    {
        void DeleteDoc(int id);

        void DeleteDetails(int id);

        /// <summary>
        /// Удаление данных документа при проставлениее признака "Не доводить ГЗ"
        /// </summary>
        void DeleteDocForNotBring(int id);
    }
}