using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    public interface IMarksCalculator
    {
        void Calc(IList<F_OIV_REG10Qual> facts, int territoryId, bool calculatePrognoz);
    }
}
