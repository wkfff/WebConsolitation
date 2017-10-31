using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Common
{
    /// <summary>
    /// Преобразователь строк.
    /// </summary>
    public static class StringElephanter
    {
        /// <summary>
        /// Возвращает преобразованную строку в соответствии с настройками
        /// </summary>
        /// <param name="source">Исходная строка</param>
        /// <param name="settings">Настройки преобразования</param>
        /// <returns>Преобразованная строка</returns>
        public static string TrampDown(string source, StringElephanterSettings settings)
        {
            if (String.IsNullOrEmpty(source))
                return source;

            if (!settings.MatchCase)
            {
                source = source.ToUpper();
            }

            char[] sourceArray = source.ToCharArray();

            if (!settings.AllowSingleChars)
                sourceArray = TrampDownSingleChars(sourceArray);

            return new string(TrampDown(sourceArray, settings.AllowDuplicateSpaces, settings.AllowSpaces, settings.AllowPunctuationChars, settings.AllowDigits));
        }

        //[DllImport("Krista.FM.Server.Elephanter.dll")]
        //private static extern string TrampDown(string lpString, bool duplicateSpaces, bool allowSpaces, bool punctuationChars);
        private static char[] TrampDown(char[] source, bool duplicateSpaces, bool allowSpaces, bool punctuationChars, bool allowDigits)
        {
            char[] dest = new char[source.Length];
            int length = 0;
            int sourceLength = source.Length;
            char prevChar = ' ';

            if (allowSpaces)
                prevChar = '\0';

            char currChar = source[0];
            for (int i = 0; i < sourceLength; i++, prevChar = currChar)
            {
                currChar = source[i];

                if (!allowSpaces && currChar == ' ')
                    continue;

                if (!punctuationChars && (
                    currChar == ',' ||
                    currChar == '.' ||
                    currChar == '"' ||
                    currChar == '-' ||
                    currChar == ':' ||
                    currChar == '?' ||
                    currChar == '(' ||
                    currChar == ')' ||
                    currChar == '%' ||
                    currChar == ';' ||
                    currChar == '\'' ||
                    currChar == '`' ||
                    currChar == '!' ||
                    currChar == '|' ||
                    currChar == '\\' ||
                    currChar == '/' ||
                    currChar == '<' ||
                    currChar == '[' ||
                    currChar == ']' ||
                    currChar == '{' ||
                    currChar == '}' ||
                    currChar == '>' ||
                    currChar == '=' ||
                    currChar == '+' ||
                    currChar == '_' ||
                    currChar == '~' ||
                    currChar == '@' ||
                    currChar == '#' ||
                    currChar == '$' ||
                    currChar == '^' ||
                    currChar == '&' ||
                    currChar == '*' ||
                    currChar == '\n' ||
                    currChar == '\r' ||
                    currChar == '\t'))
                    continue;

                if (!allowDigits && (currChar >= '0' && currChar <= '9'))
                    continue;

                if (!duplicateSpaces && currChar == ' ' && prevChar == ' ')
                    continue;

                dest[length++] = currChar;
            }
            char[] result = new char[length];
            Array.Copy(dest, result, length);
            return result;
        }

        private static char[] TrampDownSingleChars(char[] source)
        {
            char[] dest = new char[source.Length];
            int length = 0;
            int sourceLength = source.Length;
            char prevChar = ' ';
            char prev2Char = ' ';
            char currChar = source[0];
            int magicNamber = 0;
            for (int i = 0; i < sourceLength; i++, prev2Char = prevChar, prevChar = currChar)
            {
                currChar = source[i];


                if (prev2Char == ' ' && Char.IsLetter(prevChar) && currChar == ' ')
                {
                    length -= 2 - magicNamber;
                    magicNamber = 2;
                    continue;
                }

                dest[length++] = currChar;

                if (magicNamber > 0)
                    magicNamber--;
            }
            char[] result = new char[length];
            Array.Copy(dest, result, length);
            return result;
        }
    }
}
