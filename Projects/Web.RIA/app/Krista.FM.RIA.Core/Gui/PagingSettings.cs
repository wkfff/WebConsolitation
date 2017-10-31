namespace Krista.FM.RIA.Core.Gui
{
    public class PagingSettings
    {
        public PagingSettings()
        {
            Size = 25;
            Start = 0;
        }

        public int Size { get; set; }

        public int Start { get; set; }
    }
}