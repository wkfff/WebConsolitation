using System.Drawing;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Командя для команды создания команды создания комментария
    /// </summary>
    public class CreateNewCommentCommand : DiagramEditorCommand
    {
        public CreateNewCommentCommand(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgNewComment];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            CommandNewComment cmdNewComment = new CommandNewComment(Diagram);
            cmdNewComment.Location = Diagram.Site.LastClickPoint;

            cmdNewComment.Execute();
            Diagram.Site.UndoredoManager.Do(cmdNewComment);
        }
    }
}
