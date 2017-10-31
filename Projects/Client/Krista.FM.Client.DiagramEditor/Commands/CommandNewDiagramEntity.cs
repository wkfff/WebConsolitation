using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Krista.FM.Client.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда создания нового класса на диаграмме
    /// </summary>
    public class CommandNewDiagramEntity : DiagramEditorCommand
    {
        public CommandNewDiagramEntity(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgNewEntity];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        /// <summary>
        /// Создание нового класса
        /// </summary>
        /// <param name="entity"> Объект схемы</param>
        public UMLEntityBase AddEntity(IEntity entity, Point clientPoint)
        {
            UMLEntityBase entityBase = new UMLEntityBase(entity.Key, Guid.NewGuid(), Diagram, clientPoint.X, clientPoint.Y, UMLEntityBase.GetColorByClassType(entity.ClassType));
            if (Diagram.Document.ParentPackage != (entityBase.CommonObject as IEntity).ParentPackage)
            {
                entityBase.Export = String.Format("(from_{0})", (entityBase.CommonObject as IEntity).ParentPackage.Name);
            }

            Diagram.Entities.Add(entityBase);

            entityBase.AutoSizeRec();

            Diagram.IsChanged = true;

            return entityBase;
        }

        #region Command Members

        public override void Execute()
        {
        }
        
        protected void Refresh(IEntity entity)
        {
            // Обновляем родительский объект в дереве
            Diagram.Site.SchemeEditor.RefreshPackage(entity.ParentPackage.Key);
            Diagram.Site.SchemeEditor.SelectObject(entity.Key, false);

            Diagram.Site.Invalidate();
        }

        #endregion
    }
}
