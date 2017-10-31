using System;
using System.Collections.Generic;
using System.ComponentModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoUniqueKeyDesign : SmoCommonObjectDesign, IUniqueKey
    {
        
        public SmoUniqueKeyDesign(IUniqueKey serverControl) 
            : base(serverControl)
        {
        }

        [Browsable(false)]
        public IEntity Parent
        {
            get { return ((IUniqueKey) serverControl).Parent; }
        }

        [Browsable(false)]
        public List<string> Fields
        {
            get { return ((IUniqueKey) serverControl).Fields; }
            set { ((IUniqueKey) serverControl).Fields = value; }
        }

        [Browsable(false)]
        [DisplayName(@"Вычислять хэш (Hashable)")]
        [Description("Признак необходимости создать доп.поле с хешем по полям данного уникального ключа")]
        [RefreshProperties(RefreshProperties.All)]
        public bool Hashable
        {
            get { return ((IUniqueKey)serverControl).Hashable; }
            ////set { ((IUniqueKey)serverControl).Hashable = value; }
            set { }
        }


        [DisplayName(@"Наименование уникального ключа (Caption)")]
        [Description("Наименование уникального ключа, выводимое в интерфейсе")]
        public override string Caption
        {
            get { return base.Caption; }
            set { base.Caption = value; }
        }


        [Browsable(false)]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value;}
        }

        [Browsable(false)]
        public string Description
        {
            get { return base.Description; }
            set { base.Description = value; }
        }

    }


}
