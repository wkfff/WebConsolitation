using System;
using Infragistics.UltraChart.Resources.Editor;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// �������� �������� ���������
    /// </summary>
    public class ChartAreaCollectionEditor : ChartCollectionEditorBase
    {
        protected override Type CollectionEditorFormType
        {
            get
            {
                return typeof(ChartAreaCollectionEditorForm);
            }
        }

        protected override string CollectionPropertyName
        {
            get
            {
                return "ChartAreas";
            }
        }
    }
}
