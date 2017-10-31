using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Dundas.Maps.WinControl;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;



namespace Krista.FM.Client.MDXExpert
{
    class MapLayerCollectionEditor2 : CustomCollectionEditor
   {

      /// <summary>
      /// Конструктор
      /// </summary>
        public MapLayerCollectionEditor2(Type t)
           : base(t)
      {
          Trace.WriteLine("MapLayerCollectionEditor2() ctor");
      }


      /// <summary>
      /// Перекрытый метод создания формы редактора 
      /// </summary>
      protected override CustomCollectionForm CreateCollectionForm()
      {
          CustomCollectionEditorCollectionForm collform = (CustomCollectionEditorCollectionForm)base.CreateCollectionForm();
          collform.Buttons = CustomCollectionEditorCollectionForm.EditorButtons.None;
          collform.FormHeader = "Коллекция слоев";
          collform.MembersLabel = "Слои";
          return collform;
       }
   }
    
}
