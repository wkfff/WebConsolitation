using System;

namespace Krista.FM.ServerLibrary
{
    [Serializable]
    public class StringElephanterSettings
    {
        /// <summary>
        /// Учитывать регистр при сопоставлении (по умолчанию false)
        /// </summary>
        private bool matchCase = false;

        /// <summary>
        /// Учитывать повторяющиеся пробелы, пробелы в начале и конце строки (по умолчанию false)
        /// </summary>
        private bool duplicateSpaces = false;

        /// <summary>
        /// Учитывать пробелы (по умолчанию false)
        /// </summary>
        private bool allowSpaces = false;

        /// <summary>
        /// Учитывать знаки пунктуации (по умолчанию false)
        /// </summary>
        private bool punctuationChars = false;

        /// <summary>
        /// Учитывать цифровые символы (по умолчанию true)
        /// </summary>
        private bool allowDigits = true;

        /// <summary>
        /// Учитывать одиночные символы (по умолчанию true)
        /// </summary>
        private bool allowSingleChars = true;


        /// <summary>
        /// Учитывать регистр при сопоставлении (по умолчанию false)
        /// </summary>
        public bool MatchCase
        {
            get { return matchCase; }
            set
            {
                matchCase = value;
            }
        }

        /// <summary>
        /// Учитывать повторяющиеся пробелы, пробелы в начале и конце строки (по умолчанию false)
        /// </summary>
        /// <remarks>
        /// Если установить в true, то свойство AllowSpaces автоматически устанавливается а true
        /// </remarks>
        public bool AllowDuplicateSpaces
        {
            get { return duplicateSpaces; }
            set
            {
                if (value)
                    allowSpaces = value;
                duplicateSpaces = value;
            }
        }

        /// <summary>
        /// Учитывать пробелы (по умолчанию false)
        /// </summary>
        public bool AllowSpaces
        {
            get { return allowSpaces; }
            set
            {
                allowSpaces = value;
            }
        }

        /// <summary>
        /// Учитывать знаки пунктуации (по умолчанию false)
        /// </summary>
        public bool AllowPunctuationChars
        {
            get { return punctuationChars; }
            set { punctuationChars = value; }
        }

        /// <summary>
        /// Учитывать цифровые символы (по умолчанию true)
        /// </summary>
        public bool AllowDigits
        {
            get { return allowDigits; }
            set
            {
                allowDigits = value;
            }
        }

        /// <summary>
        /// Учитывать одиночные символы (по умолчанию true)
        /// </summary>
        public bool AllowSingleChars
        {
            get { return allowSingleChars; }
            set
            {
                allowSingleChars = value;
            }
        }
    }
}
