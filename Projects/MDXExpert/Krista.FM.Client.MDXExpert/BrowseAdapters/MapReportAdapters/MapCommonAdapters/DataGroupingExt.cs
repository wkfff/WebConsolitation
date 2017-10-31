using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Krista.FM.Client.MDXExpert
{
    public enum DataGroupingExt
    {
        [Description("Равномерное распределение")]
        EqualDistribution,
        [Description("Равные интервалы")]
        EqualInterval,
        [Description("Оптимальное")]
        Optimal,
        [Description("Пользовательское")]
        Custom
    }

}
