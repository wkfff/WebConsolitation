using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Models;

namespace Krista.FM.RIA.Extensions.E86N.Services.SmetaService
{
    public interface ISmetaService : IRestService<F_Fin_Smeta, SmetaViewModel>
    {        
        //note: empty interface is needed to make IoC work correctly
    }
}
