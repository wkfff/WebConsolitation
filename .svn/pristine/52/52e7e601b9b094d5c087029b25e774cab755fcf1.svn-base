using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class UniqueKeyListControl : ModifiableListControl<SmoDictionaryBaseDesign<string, IUniqueKey>, IUniqueKey>
    {
        public UniqueKeyListControl(IUniqueKeyCollection controlObject, CustomTreeNodeControl parent)
            : base("IUniqueKeyCollection", "Уникальные ключи", new SmoUniqueKeyCollectionDesign(controlObject), parent, (int)Images.UniqueKeys)
        {
        }

        public override CustomTreeNodeControl Create(IUniqueKey uniqueKey)
        {
            SmoUniqueKeyDesign uniqueKeyDesign = new SmoUniqueKeyDesign(uniqueKey);
            return new UniqueKeyControl(uniqueKey.ObjectKey, uniqueKey.Caption, uniqueKeyDesign, this);
        }

        /// <summary>
        /// Добавляет дочерние узлы для коллекции уникальных ключей
        /// </summary>
        protected override void ExpandNode()
        {
            try
            {
                StartExpand();

                foreach (KeyValuePair<string, IUniqueKey> kvp in (SmoDictionaryBaseDesign<string, IUniqueKey>)ControlObject)
                {

                    CustomTreeNodeControl customTreeNodeControl = Create(kvp.Value);
                    customTreeNodeControl.Expanded = true;
                    Nodes.Add(customTreeNodeControl);
                }
            }
            finally
            {
                EndExpand();
            }
        }

        public override void OnDragOver(DragEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {
            //Get the node the mouse is over.
            Point point = Control.PointToClient(new Point(e.X, e.Y));
            CustomTreeNodeControl targetNode = Control.GetNodeFromPoint(point) as CustomTreeNodeControl;

            if (targetNode != null
                && SelectedNodesFromSuchParent((SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection)), targetNode.Parent)
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

        /// <summary>
        /// Проверяет все ли выделенные атрибуты из той же таблицы, что и уникальный ключ
        /// </summary>
        internal static bool SelectedNodesFromSuchParent(SelectedNodesCollection selectedNodes, UltraTreeNode parent)
        {
            // TODO: проверка являются ли все элементы из списка детьми этого родителя (parent)
            return true;
        }


        public override void QueryStateAllowedForNode(ObjectsTreeViewDropHightLightDrawFilter.QueryStateAllowedForNodeEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {
            e.StatesAllowed = DropLinePositionEnum.OnNode;
        }

        /// <summary>
        /// Создание нового уникального ключа
        /// </summary>
        public override void OnDragDrop(DragEventArgs e, ObjectsTreeViewDropHightLightDrawFilter dropHightLightDrawFilter)
        {
            SelectedNodesCollection selectedNodes = (SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection));
        
            try
            {
                if (MessageBox.Show("Создать новый уникальный ключ?"
                                , "Создание"
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

                    SmoUniqueKeyCollectionDesign smoUniqueKeyCollectionDesign =
                        (SmoUniqueKeyCollectionDesign) this.ControlObject;
                    IUniqueKey uniqueKey = smoUniqueKeyCollectionDesign.CreateItem("Уникальный ключ", fields, false);

                    //Добавляем его в дерево
                    CustomTreeNodeControl node = Create(uniqueKey);
                    node.Expanded = true;
                    Nodes.Add(node);
                    OnChange(this, new EventArgs());
                    SelectNewObject(node);
                }

            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show(exception.Message, "Пакет не взят на редактирование", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Ошибка при создании уникального ключа", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            finally
            {
                dropHightLightDrawFilter.ClearDropHighlight();
            }
        }

        [MenuAction("Создать", CheckMenuItemEnabling = "false")]
        public override void AddNew() {}


    }
}
