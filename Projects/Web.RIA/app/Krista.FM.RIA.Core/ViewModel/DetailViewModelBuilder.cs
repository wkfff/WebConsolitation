using System.Collections.Generic;

namespace Krista.FM.RIA.Core.ViewModel
{
    public class DetailViewModelBuilder
    {
        public DetailViewModelBuilder()
        {
            // Инициализируем значения default-значения
            ColumnState = new Dictionary<string, ColumnState>();
            FieldsPerColumn = 16;
            ViewService = new DefaultViewService();
        }

        private Dictionary<string, ColumnState> ColumnState { get; set; }

        private int FieldsPerColumn { get; set; }
        
        private IViewService ViewService { get; set; }
        
        private int TabRegionType { get; set; }

        // Для неявного приведения типа
        public static implicit operator DetailViewModel(DetailViewModelBuilder builder)
        {
            return new DetailViewModel
            {
                ColumnState = builder.ColumnState,
                FieldsPerColumn = builder.FieldsPerColumn,
                ViewService = builder.ViewService,
                TabRegionType = builder.TabRegionType
            };
        }

        public DetailViewModelBuilder WithFieldsPerColumn(int value)
        {
            FieldsPerColumn = value;
            return this;
        }

        public DetailViewModelBuilder AddColumnState(string key, ColumnState value)
        {
            ColumnState.Add(key, value);
            return this;
        }

        public DetailViewModelBuilder WithViewService(IViewService value)
        {
            ViewService = value;
            return this;
        }

        public DetailViewModelBuilder WithTabRegionType(int value)
        {
            TabRegionType = value;
            return this;
        }
    }
}
