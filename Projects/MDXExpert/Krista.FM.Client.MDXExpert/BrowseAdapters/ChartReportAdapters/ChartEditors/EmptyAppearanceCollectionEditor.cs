using System;
using Infragistics.UltraChart.Resources.Editor;

namespace Krista.FM.Client.MDXExpert
{
    public class EmptyAppearanceCollectionEditor : ChartCollectionEditorBase
    {
        protected override Type CollectionEditorFormType
        {
            get
            {
                return typeof(EmptyAppearanceCollectionEditorForm);
            }
        }

        protected override string CollectionPropertyName
        {
            get
            {
                return "EmptyStyles";
            }
        }
    }
}