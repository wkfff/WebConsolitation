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
    class CustomColorCollectionEditor : CustomCollectionEditor
   {

      /// <summary>
      /// Конструктор
      /// </summary>
        public CustomColorCollectionEditor(Type t)
           : base(t)
      {
          Trace.WriteLine("CustomColorCollectionEditor() ctor");
      }


      /// <summary>
      /// Перекрытый метод создания формы редактора 
      /// </summary>
      protected override CustomCollectionForm CreateCollectionForm()
      {
          CustomCollectionEditorCollectionForm collform = (CustomCollectionEditorCollectionForm)base.CreateCollectionForm();
          collform.Buttons = CustomCollectionEditorCollectionForm.EditorButtons.None;
          collform.FormHeader = "Коллекция цветов";
          collform.MembersLabel = "Пользовательские цвета";
          return collform;
       }
   }
    
}
