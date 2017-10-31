namespace MDXParser
{
    using System;
    using System.Drawing;

    public class Source
    {
        private Locator m_StartLocation = new Locator();

        public virtual Source Clone()
        {
            Source source = new Source();
            source.m_StartLocation = this.m_StartLocation;
            return source;
        }

        public virtual void DrawWigglyLine(int pos, int len)
        {
        }

        public virtual void SetColor(int pos, int len, Color color)
        {
        }

        public Locator StartLocation
        {
            get
            {
                return this.m_StartLocation;
            }
            set
            {
                this.m_StartLocation = value;
            }
        }
    }
}

