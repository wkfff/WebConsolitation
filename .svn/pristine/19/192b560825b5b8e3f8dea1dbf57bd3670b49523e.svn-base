using System.Text;

namespace Krista.FM.RIA.Core.Progress
{
    /// <summary>
    /// Параметр индикатора запроса.
    /// </summary>
    public class ProgressConfig : Ext.Net.Parameter
    {
        private const string ParameterName = "fxProgressConfig";

        public ProgressConfig(string message)
            : base(ParameterName, new ParameterConfig { Message = message, Interval = 500 } .ToString(), Ext.Net.ParameterMode.Raw)
        {
        }

        public ProgressConfig(string message, bool canHide)
            : base(ParameterName, new ParameterConfig { Message = message, CanHide = canHide, Interval = 500 } .ToString(), Ext.Net.ParameterMode.Raw)
        {
        }

        public ProgressConfig(string message, int interval)
            : base(ParameterName, new ParameterConfig { Message = message, Interval = interval } .ToString(), Ext.Net.ParameterMode.Raw)
        {
        }

        public ProgressConfig(string message, bool canHide, int interval)
            : base(ParameterName, new ParameterConfig { Message = message, CanHide = canHide, Interval = interval } .ToString(), Ext.Net.ParameterMode.Raw)
        {
        }

        private class ParameterConfig
        {
            public string Message { private get; set; }
            
            public bool CanHide { private get; set; }
            
            public int Interval { private get; set; }

            public override string ToString()
            {
                return new StringBuilder()
                    .Append('{')
                    .Append("message:'").Append(Message)
                    .Append("',canHide:").Append(CanHide ? "true" : "false")
                    .Append(",interval:").Append(Interval)
                    .Append('}')
                    .ToString();
            }
        }
    }
}
