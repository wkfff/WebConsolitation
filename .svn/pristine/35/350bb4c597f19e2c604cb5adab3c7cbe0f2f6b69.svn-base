using System.Drawing;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandRefresh : DiagramEditorCommand 
    {
        public CommandRefresh(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgRefresh];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        #region Command Members

        /// <summary>
        /// Команда обновления диаграммы
        /// </summary>
        public override void Execute()
        {
            try
            {
                Diagram.Site.SchemeEditor.Operation.Text = "Обновление...";
                Diagram.Site.SchemeEditor.Operation.StartOperation();

                foreach (DiagramEntity entity in Diagram.Entities)
                {
                    if (entity is UMLEntityBase)
                    {
                        // обновление имени объекта
                        entity.RefreshKey();

                        // обновление аттрибутов
                        ((UMLEntityBase)entity).RefreshAttrCollection();
                    }
                }
            }
            finally
            {
                Diagram.Site.SchemeEditor.Operation.StopOperation();
            }
        }

        #endregion
    }
}
