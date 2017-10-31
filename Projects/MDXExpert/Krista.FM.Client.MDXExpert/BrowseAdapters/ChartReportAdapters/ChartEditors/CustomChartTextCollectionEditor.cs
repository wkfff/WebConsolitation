using System;
using Infragistics.UltraChart.Resources.Editor;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// �������� �������� ���������
    /// </summary>
    public class CustomChartTextCollectionEditor : ChartCollectionEditorBase
    {
        protected override Type CollectionEditorFormType
        {
            get
            {
                return typeof(CustomChartTextCollectionEditorForm);
            }
        }

        protected override string CollectionPropertyName
        {
            get
            {
                return "ChartText";
            }
        }
    }
}
