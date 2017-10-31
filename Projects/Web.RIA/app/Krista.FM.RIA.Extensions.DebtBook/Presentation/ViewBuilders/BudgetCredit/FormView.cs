using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Common.Constants;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.DebtBook.Params;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders.BudgetCredit
{
    public class FormView : BebtBookFormView
    {
        public FormView(IVariantProtocolService protocolService, IDebtBookExtension bebtBookExtension, IParametersService parametersService) 
            : base(protocolService, bebtBookExtension, parametersService)
        {
        }

        public override List<Component> Build(ViewPage page)
        {
            var components = base.Build(page);

            if (new OKTMOValueProvider(SchemeAccessor.GetScheme()).GetValue() == OKTMO.Stavropol)
            {
                // Обрабатываем напильником для Ставрополя
                BuildStavropol(components);
            }

            return components;
        }

        private void BuildStavropol(List<Component> components)
        {
            Panel panel = (Panel)components[0];
            FormPanel formPanel = (FormPanel)panel.Items[0];
            var fieldSet = formPanel.Items.OfType<FieldSet>().FirstOrDefault(x => x.Title.Equals("Информация по договору"));
            if (fieldSet == null)
            {
                throw new Exception("Не найдена группа с наименованием Информация по договору");
            }

            var oldField = fieldSet.Items.OfType<TextField>().FirstOrDefault(x => x.ID == "COLLATERAL");
            if (oldField == null)
            {
                throw new Exception("Не найдено текстовое поле с id=COLLATERAL");
            }

            var newField = new ComboBox
            {
                ID = oldField.ID,
                FieldLabel = oldField.FieldLabel,
                AllowBlank = oldField.AllowBlank,
                Width = oldField.Width,
                DataIndex = oldField.DataIndex,
                Text = oldField.Text,
                Mode = DataLoadMode.Local,
                Items =
                                       {
                                           new ListItem("бюджет", "бюджет"),
                                           new ListItem("имущество", "имущество"),
                                           new ListItem("прочее", "прочее")
                                       },
                Editable = false,
                ReadOnly = oldField.ReadOnly
            };
            var itemPosition = fieldSet.Items.IndexOf(oldField);
            fieldSet.Items.Remove(oldField);
            fieldSet.Items.Insert(itemPosition, newField);
        }
    }
}
