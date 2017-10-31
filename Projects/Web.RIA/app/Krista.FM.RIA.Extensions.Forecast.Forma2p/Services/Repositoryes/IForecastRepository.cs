using System.Collections.Generic;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public interface IForecastParamsRepository : ILinqRepository<D_Forecast_PParams>
    {
        IList<D_Forecast_PParams> GetAllParams();
    }

    public interface IForecastVariantsRepository : ILinqRepository<D_Forecast_PVars>
    {
        IList<D_Forecast_PVars> GetAllVariants();
    }

    public interface IForecastValuesRepository : ILinqRepository<D_Forecast_PValues>
    {
        IList<D_Forecast_PValues> GetAllValues();
    }
    
    public interface IForecastForma2pRepository : ILinqRepository<D_Forecast_Forma2p>
    {
        IList<D_Forecast_Forma2p> GetAllParams();
    }

    public interface IForecastForma2pVarRepository : ILinqRepository<F_Forecast_VarForm2P>
    {
        IList<F_Forecast_VarForm2P> GetAllVariants();
    }

    public interface IForecastForma2pValueRepository : ILinqRepository<T_Forecast_ParamValues>
    {
        IList<T_Forecast_ParamValues> GetAllValues();
    }

    public interface IForecastRegulatorsRepository : ILinqRepository<D_Forecast_Regs>
    {
        IList<D_Forecast_Regs> GetAllRegulators();
    }

    public interface IForecastRegulatorsValueRepository : ILinqRepository<D_Forecast_RegValues>
    {
        IList<D_Forecast_RegValues> GetAllValues();
    }
    
    public interface IForecastScenarioVarsRepository : ILinqRepository<F_Forecast_Scenario>
    {
        IList<F_Forecast_Scenario> GetAllVars();
    }

    public interface IForecastSvodMOParamsMORepository : ILinqRepository<D_Forecast_ParamsMO>
    {
        IList<D_Forecast_ParamsMO> GetAllParams();
    }

    public interface IForecastSvodMOVarsMORepository : ILinqRepository<F_Forecast_VariantsMO>
    {
        IList<F_Forecast_VariantsMO> GetAllVars();
    }

    public interface IForecastSvodMOValuesMORepository : ILinqRepository<T_Forecast_ValuesMO>
    {
        IList<T_Forecast_ValuesMO> GetAllValues();
    }

    public interface IForecastSvodMOVarsMERRepository : ILinqRepository<F_Forecast_VariantsMER>
    {
        IList<F_Forecast_VariantsMER> GetAllVars();
    }

    public interface IForecastSvodMOValuesMERRepository : ILinqRepository<T_Forecast_ValuesMER>
    {
        IList<T_Forecast_ValuesMER> GetAllValues();
    }

    public interface IForecastSvodMOVarsOIVRepository : ILinqRepository<F_Forecast_VariantsOIV>
    {
        IList<F_Forecast_VariantsOIV> GetAllVars();
    }

    public interface IForecastSvodMOValuesOIVRepository : ILinqRepository<T_Forecast_ValuesOIV>
    {
        IList<T_Forecast_ValuesOIV> GetAllValues();
    }
}
