﻿using Rhino.Mocks.Interfaces;

namespace Krista.FM.RIA.Extensions.DebtBook.Tests.Helpers
{
    public static class Ext
    {
        public static IMethodOptions<T> Ret<T>(this IMethodOptions<T> m, T objToReturn)
        {
            return m.Return(objToReturn).Repeat.Any();
        }
    }
}
