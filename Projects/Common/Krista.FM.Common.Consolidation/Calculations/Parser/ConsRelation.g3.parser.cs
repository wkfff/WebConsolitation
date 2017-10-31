using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace Krista.FM.Common.Consolidation.Calculations.Parser
{
    /// <summary>
    /// Расширение парсера.
    /// </summary>
    public partial class ConsRelationParser : Antlr.Runtime.Parser
    {
        public List<string> Errors { get; private set; }

        public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
        {
            base.DisplayRecognitionError(tokenNames, e);

            if (Errors == null)
            {
                Errors = new List<string>();
            }

            string hdr = GetErrorHeader(e);
            string msg = GetErrorMessage(e, tokenNames);
            Errors.Add(msg + " at " + hdr);
        }

        partial void CreateTreeAdaptor(ref ITreeAdaptor adaptor)
        {
            adaptor = new ConsRelationTreeAdaptor();
        }
    }
}
