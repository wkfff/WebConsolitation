using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    /// <summary>
    /// Исключение закачки данных
    /// </summary>
    public class PumpDataFailedException : ApplicationException
    {
        public PumpDataFailedException() : base() { }
        public PumpDataFailedException(string str) : base(str) { }

        public override string ToString()
        {
            return this.Message;
        }
    }

    /// <summary>
    /// Исключение закачки данных, возникающее из-за ошибок при проверке источника
    /// </summary>
    public class DataSourceIsCorruptException : ApplicationException
    {
        public DataSourceIsCorruptException() : base() { }
        public DataSourceIsCorruptException(string str) : base(str) { }

        public override string ToString()
        {
            return this.Message;
        }
    }

    /// <summary>
    /// Исключение закачки текстовых отчетов
    /// </summary>
    public class TextRepAnalysisFailedException : ApplicationException
    {
        public TextRepAnalysisFailedException() : base() { }
        public TextRepAnalysisFailedException(string str) : base(str) { }

        public override string ToString()
        {
            return this.Message;
        }
    }

    /// <summary>
    /// Исключение закачки текстовых отчетов
    /// </summary>
    public class FilesNotFoundException : ApplicationException
    {
        public FilesNotFoundException() : base() { }
        public FilesNotFoundException(string str) : base(str) { }

        public override string ToString()
        {
            return this.Message;
        }
    }
}