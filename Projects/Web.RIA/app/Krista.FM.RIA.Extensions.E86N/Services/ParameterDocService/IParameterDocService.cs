using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.ParameterDocService
{
    public interface IParameterDocService : INewRestService
    {
        void Delete(int id);
    }
}
