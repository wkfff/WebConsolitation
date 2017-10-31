using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Dundas.Maps.WinControl;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using Infragistics.UltraChart.Resources.Editor;


namespace Krista.FM.Client.MDXExpert
{
    class MapLegendCollectionEditor : CustomCollectionEditor
   {

      /// <summary>
      /// Конструктор
      /// </summary>
        public MapLegendCollectionEditor(Type t)
           : base(t)
      {
          Trace.WriteLine("MapLegendCollectionEditor() ctor");
      }


      /// <summary>
      /// Перекрытый метод создания формы редактора 
      /// </summary>
      protected override CustomCollectionForm CreateCollectionForm()
      {
          CustomCollectionEditorCollectionForm collform = (CustomCollectionEditorCollectionForm)base.CreateCollectionForm();
          collform.CollectionEditable = false;
          collform.Buttons = CustomCollectionEditorCollectionForm.EditorButtons.None;
          collform.FormHeader = "Коллекция легенд";
          collform.MembersLabel = "Легенды";
          collform.MainForm = MainForm;
          return collform;
       }
   }
    
}
