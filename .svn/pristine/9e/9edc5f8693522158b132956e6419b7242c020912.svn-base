using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Presentation.Views;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    /// <summary>
    /// Поле вычисления суммы по столбцу в гриде(сторе)
    /// </summary>
    public class SummFieldControl : Control
    {
        public SummFieldControl(string id, Store store, string summField, string labelField)
        {
            Id = id;
            Store = store;
            SummField = summField;
            LabelField = labelField;
            Width = 300;
            SummmHandler = "CalculateSumm('{0}', '{1}', '{2}');".FormatWith(store.ID, summField, Id + "DisplayField");
        }

        /// <summary>
        ///  Стор поля
        /// </summary>
        public Store Store { get; set; }

        /// <summary>
        /// Поле по которому вычислять сумму
        /// </summary>
        public string SummField { get; set; }
        
        /// <summary>
        /// Лэйбл поля
        /// </summary>
        public string LabelField { get; set; }

        /// <summary>
        /// Ширина поля
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Процедура вычисления суммы или другого действия
        /// </summary>
        public string SummmHandler { get; set; }
        
        public override List<Component> Build(ViewPage page)
        {
            const string Style = @".total-field{
                        background-color : #fff;
                        font-weight      : bold !important;                       
                        color            : #000;
                        border           : solid 1px silver;
                        padding          : 2px;
                        margin-right     : 5px;
                    } 
                ";

            ResourceManager.GetInstance(page).RegisterClientStyleBlock("SummFldStyle", Style);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("SummFn", Resource.SummFn);

            var cf = new CompositeField
                         {
                             ID = Id,
                             Width = Width
                         };

            cf.Items.Add(new Label
                                {
                                    ID = Id + "Label",
                                    Text = LabelField,
                                    AutoWidth = true 
                                });

            cf.Items.Add(new DisplayField
                         {
                             ID = Id + "DisplayField",
                             Cls = "total-field",
                             Text = @"-",
                             AutoWidth = true
                         });

            Store.Listeners.Update.AddAfter(SummmHandler);
            Store.Listeners.DataChanged.AddAfter(SummmHandler);

            return new List<Component> { cf };
        }

        public virtual CompositeField BuildComponent(ViewPage page)
        {
            return Build(page)[0] as CompositeField;
        }
    }
}
