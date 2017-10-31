namespace MDXParser
{
    using System;

    public class FormatOptions
    {
        private bool m_ColorFunctionNames = true;
        private bool m_CommaAfterNewLine = true;
        private int m_Indent = 2;
        private int m_LineWidth = 80;
        private OutputFormat m_Output = OutputFormat.Text;

        public bool ColorFunctionNames
        {
            get
            {
                return this.m_ColorFunctionNames;
            }
            set
            {
                this.m_ColorFunctionNames = value;
            }
        }

        public bool CommaBeforeNewLine
        {
            get
            {
                return !this.m_CommaAfterNewLine;
            }
            set
            {
                this.m_CommaAfterNewLine = !value;
            }
        }

        public int Indent
        {
            get
            {
                return this.m_Indent;
            }
            set
            {
                this.m_Indent = value;
            }
        }

        public int LineWidth
        {
            get
            {
                return this.m_LineWidth;
            }
            set
            {
                this.m_LineWidth = value;
            }
        }

        public OutputFormat Output
        {
            get
            {
                return this.m_Output;
            }
            set
            {
                this.m_Output = value;
            }
        }
    }
}

