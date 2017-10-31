using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Models;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Services
{
    public interface IControlService
    {
        List<FO51ControlModel> GetDefects(int periodId, D_Regions_Analysis region);
    }
}
