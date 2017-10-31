using System;
using System.Collections;
using System.Collections.Generic;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public struct Group
    {
        public string CodeName;
        public int Code;
        public string TextName;
        public string Description;
        public MathMethods Methods;
    }

    internal struct FixedMathGroups
    {
        public const int ComplexEquation = 0;
        public const int FirstOrderRegression = 1;
        public const int SecondOrderRegression = 2;
        public const int ARMAMethod = 3;
        public const int MultiRegression = 4;
        public const int PCAForecast = 5;
    }
    
    public class MathGroups : IEnumerable<Group>
    {
        private List<Group> lstOfGroups = new List<Group>();
        
        public void AddGroup(string codeName, int code, string textName)
        {
            Group group = new Group
                {
                    CodeName = codeName,
                    Code = code,
                    TextName = textName,
                    Description = String.Empty,
                    Methods = new MathMethods()
                };

            lstOfGroups.Add(group);
        }

        public void ClearAll()
        {
            foreach (Group group in lstOfGroups)
            {
                group.Methods.ClearAll();
            }

            lstOfGroups.Clear();
        }

        public Group? GetGroupByCode(int code)
        {
            foreach (Group group in lstOfGroups)
            {
                if (group.Code == code)
                {
                    return group;
                }
            }

            return null;
        }

        public IEnumerator<Group> GetEnumerator()
        {
            foreach (Group group in lstOfGroups)
            {
                yield return group;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
