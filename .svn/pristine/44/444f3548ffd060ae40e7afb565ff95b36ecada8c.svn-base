using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class UniqueKeyControl : MinorObjectControl<SmoUniqueKeyDesign>
    {
       public UniqueKeyControl(string name, string text, SmoUniqueKeyDesign controlObject, CustomTreeNodeControl parent)
            : base(name, (text ?? "Уникальный ключ"), controlObject, parent, (int)Images.UniqueKey)
        {
           ExpandNode();
        }

       /// <summary>
       /// Добавляет дочерние узлы для коллекции уникальных ключей
       /// </summary>
       protected override void ExpandNode()
       {
           foreach (string field in ((SmoUniqueKeyDesign)ControlObject).Fields)
            {
                UniqueKeyFieldControl customTreeNodeControl = new UniqueKeyFieldControl(field, field, this, (int)Images.AttributeLink);
                Nodes.Add(customTreeNodeControl);
            }    
           
       }


        [MenuAction("Удалить", Images.Remove, CheckMenuItemEnabling = "CanDelete")]
        public void Delete()
        {
            if (MessageBox.Show(String.Format("Вы действительно хотите удалить уникальный ключ \r {0} ?", this.Caption)
                                , "Удаление"
                                , MessageBoxButtons.YesNo
                                , MessageBoxIcon.Question
                                , MessageBoxDefaultButton.Button1
                                ) == DialogResult.Yes
                )
            {
                UniqueKeyListControl uniqueKeyListControl = (UniqueKeyListControl)this.Parent;
                SmoUniqueKeyCollectionDesign smoUniqueKeyCollectionDesign = (SmoUniqueKeyCollectionDesign)uniqueKeyListControl.ControlObject;
                bool SuccessfullyDeleted = smoUniqueKeyCollectionDesign.Remove(((IUniqueKey)this.ControlObject).ObjectKey);
                
                if (SuccessfullyDeleted)
                {
                    OnChange(this, new EventArgs());
                    Remove();
                }
            }
        }

        /// <summary>
        /// Проверка возможности удаления уникального ключа 
        /// </summary>
        public bool CanDelete()
        {
            return this.IsEditable();
        }

        
        public override void OnDragOver(DragEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {
            //Get the node the mouse is over.
            Point point = Control.PointToClient(new Point(e.X, e.Y));
            CustomTreeNodeControl targetNode = Control.GetNodeFromPoint(point) as CustomTreeNodeControl;

            if (targetNode != null
                && UniqueKeyListControl.SelectedNodesFromSuchParent((SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection)), targetNode.Parent.Parent)
                )
            {
                e.Effect = DragDropEffects.Move;
                dropHightLightDrawFilter.SetDropHighlightNode(targetNode, point);
                return;
            }
            else
            {
                //Запрет
                base.OnDragOver(e, dropHightLightDrawFilter);
            }

        }

        public override void QueryStateAllowedForNode(ObjectsTreeViewDropHightLightDrawFilter.QueryStateAllowedForNodeEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {
            e.StatesAllowed = DropLinePositionEnum.OnNode;
        }

        /// <summary>
        /// Добавление атрибутов к уникальному ключу
        /// </summary>
        public override void OnDragDrop(DragEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {
            SelectedNodesCollection selectedNodes = (SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection));

            try
            {
                if (MessageBox.Show("Добавить поля в уникальный ключ?"
                                , "Изменение"
                                , MessageBoxButtons.YesNo
                                , MessageBoxIcon.Question
                                , MessageBoxDefaultButton.Button2
                                ) == DialogResult.Yes
                   )
                {
                    //Формируем перечень полей
                    List<string> fields = new List<string>(selectedNodes.Count);
                    foreach (UltraTreeNode sourceNode in selectedNodes)
                    {
                        if (sourceNode is AttributeControl)
                        {
                            fields.Add(((SmoAttributeDesign) ((AttributeControl) sourceNode).ControlObject).Name);
                        }
                        else if (sourceNode is AssociationControl)
                        {
                            fields.Add(
                                ((SmoAssociationDesign) ((AssociationControl) sourceNode).ControlObject).FullDBName);
                        }

                    }

                    //Добавляем эти поля в список полей уникального ключа
                    SmoUniqueKeyDesign smoUniqueKeyDesign = (SmoUniqueKeyDesign) this.ControlObject;
                    List<string> oldFields = smoUniqueKeyDesign.Fields;
                    oldFields.AddRange(fields);
                    smoUniqueKeyDesign.Fields = oldFields;

                    //Добавляем список полей в дерево
                    foreach (string field in fields)
                    {
                        Nodes.Add(new UniqueKeyFieldControl(field, field, this, (int) Images.AttributeServ));
                    }

                    OnChange(this, new EventArgs());

                    ((UniqueKeyListControl) this.Parent).SelectNewObject(this);
                }

            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show(exception.Message, "Пакет не взят на редактирование", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Ошибка добавления полей в уникальный ключ", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            finally
            {
                dropHightLightDrawFilter.ClearDropHighlight();
            }
  
        }
    }






    public class UniqueKeyFieldControl : CustomTreeNodeControl
    {
        private string _field;

        public UniqueKeyFieldControl(string name, string text, CustomTreeNodeControl parent, int imageIndex) 
            : base(name, text, parent, imageIndex)
        {
            this._field = name;
        }


        [MenuAction("Удалить", Images.Remove, CheckMenuItemEnabling = "CanDelete")]
        public void Delete()
        {
            if (MessageBox.Show(String.Format("Вы действительно хотите удалить поле из уникального ключа \r {0} ?", this._field)
                                , "Удаление"
                                , MessageBoxButtons.YesNo
                                , MessageBoxIcon.Question
                                , MessageBoxDefaultButton.Button1
                                ) == DialogResult.Yes
                )
            {
                UniqueKeyControl uniqueKeyControl = (UniqueKeyControl) this.Parent;
                SmoUniqueKeyDesign smoUniqueKeyDesign = (SmoUniqueKeyDesign) uniqueKeyControl.ControlObject;
                List<string> fields = smoUniqueKeyDesign.Fields;
                if (fields.Remove(this._field))
                {
                    smoUniqueKeyDesign.Fields = fields;
                    OnChange(this, new EventArgs());
                    Remove();
                }
            }

        }


        /// <summary>
        /// Проверка возможности удаления поля из внешнего ключа
        /// </summary>
        public bool CanDelete()
        {
            UniqueKeyControl uniqueKeyControl = (UniqueKeyControl)this.Parent;
            return uniqueKeyControl.IsEditable() && ( ((SmoUniqueKeyDesign)uniqueKeyControl.ControlObject).Fields.Count > 1 );
        }


        //незачем вводить пользователя в заблуждение возможностью сортировки полей в уникальном ключе (это вопрос пока открытый)
        //пусть кидают поля прямо на ключ, а не в список полей
        //public override void OnDragOver(DragEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        //{
        //    //Get the node the mouse is over.
        //    Point point = Control.PointToClient(new Point(e.X, e.Y));
        //    CustomTreeNodeControl targetNode = Control.GetNodeFromPoint(point) as CustomTreeNodeControl;

        //    e.Effect = DragDropEffects.Move;
        //    dropHightLightDrawFilter.SetDropHighlightNode(targetNode, point);
        //    return;

        //}

        //public override void QueryStateAllowedForNode(ObjectsTreeViewDropHightLightDrawFilter.QueryStateAllowedForNodeEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        //{
        //    e.StatesAllowed = DropLinePositionEnum.All;
        //}

        ///// <summary>
        ///// Добавление атрибутов к уникальному ключу
        ///// </summary>
        //public override void OnDragDrop(DragEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        //{
        //    ((UniqueKeyControl)this.Parent).OnDragDrop(e, dropHightLightDrawFilter);

        //}
    }

}
