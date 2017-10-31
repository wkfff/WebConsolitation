using System;
using System.Collections.Generic;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public sealed class NonUniqueDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, IList<TValue>> container = new Dictionary<TKey, IList<TValue>>();

        public IEnumerable<TValue> this[TKey key]
        {
            get
            {
                return container.ContainsKey(key) ? container[key] : null;
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (!container.ContainsKey(key))
            {
                container.Add(key, new List<TValue>());
            }

            container[key].Add(value);
        }
        public bool IsContainKey(TKey key)
        {
            return container.ContainsKey(key) ? true : false;
        }
    }
    public static class ErrorsCollector
    {
        public static Dictionary<string, positionResultType> positionResults = new Dictionary<string, positionResultType>();
        
        public static void addViolation(string key, violationType violation)
        {
            if (!positionResults.ContainsKey(key))
            {
                positionResults.Add(key, new positionResultType
                                             {
                                                 refPositionId = key,
                                                 result = "failure",
                                             });
            }
            positionResults[key].violation.Add(violation);
        }


        public static positionResultType getPositionResult(string id)
        {
            if (positionResults.ContainsKey(id))
            {
                return positionResults[id];
            }
            else
            {
                return new positionResultType
                           {
                               refPositionId = id,
                               result = "success",
                           };
            }
        }

        public static string getResult(string id)
        {
            return positionResults.ContainsKey(id) ? "failure" : "success";
        }
    }
}
