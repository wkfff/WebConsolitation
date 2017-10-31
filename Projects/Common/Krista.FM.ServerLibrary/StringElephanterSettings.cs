using System;

namespace Krista.FM.ServerLibrary
{
    [Serializable]
    public class StringElephanterSettings
    {
        /// <summary>
        /// ��������� ������� ��� ������������� (�� ��������� false)
        /// </summary>
        private bool matchCase = false;

        /// <summary>
        /// ��������� ������������� �������, ������� � ������ � ����� ������ (�� ��������� false)
        /// </summary>
        private bool duplicateSpaces = false;

        /// <summary>
        /// ��������� ������� (�� ��������� false)
        /// </summary>
        private bool allowSpaces = false;

        /// <summary>
        /// ��������� ����� ���������� (�� ��������� false)
        /// </summary>
        private bool punctuationChars = false;

        /// <summary>
        /// ��������� �������� ������� (�� ��������� true)
        /// </summary>
        private bool allowDigits = true;

        /// <summary>
        /// ��������� ��������� ������� (�� ��������� true)
        /// </summary>
        private bool allowSingleChars = true;


        /// <summary>
        /// ��������� ������� ��� ������������� (�� ��������� false)
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
        /// ��������� ������������� �������, ������� � ������ � ����� ������ (�� ��������� false)
        /// </summary>
        /// <remarks>
        /// ���� ���������� � true, �� �������� AllowSpaces ������������� ��������������� � true
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
        /// ��������� ������� (�� ��������� false)
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
        /// ��������� ����� ���������� (�� ��������� false)
        /// </summary>
        public bool AllowPunctuationChars
        {
            get { return punctuationChars; }
            set { punctuationChars = value; }
        }

        /// <summary>
        /// ��������� �������� ������� (�� ��������� true)
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
        /// ��������� ��������� ������� (�� ��������� true)
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
