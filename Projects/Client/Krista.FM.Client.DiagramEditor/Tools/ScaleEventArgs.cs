using System;

namespace Krista.FM.Client.DiagramEditor.Tools
{
    /// <summary>
    ///  Свои аргументы...
    /// </summary>
    public class ScaleEventArgs : EventArgs
    {
        public readonly int ScaleFactor;

        public ScaleEventArgs(int scaleFactor)
        {
            this.ScaleFactor = scaleFactor;
        }
    }
}
