using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert.Common.Converts
{
    public struct TrimmingConvertor
    {
        public static TextTrimming ToInfragisticsTrimming(StringTrimming systemTrimming)
        {
            switch (systemTrimming)
            {
                case StringTrimming.Character: return TextTrimming.Character;
                case StringTrimming.EllipsisCharacter: return TextTrimming.EllipsisCharacter;
                case StringTrimming.EllipsisPath: return TextTrimming.EllipsisPath;
                case StringTrimming.EllipsisWord: return TextTrimming.EllipsisWord;
                case StringTrimming.None: return TextTrimming.None;
                case StringTrimming.Word: return TextTrimming.Word;
            }
            return TextTrimming.Default;
        }

        public static StringTrimming ToSystemTrimming(TextTrimming infragisticsTrimming)
        {
            switch (infragisticsTrimming)
            {
                case TextTrimming.Character:
                case TextTrimming.CharacterWithLineLimit: return StringTrimming.Character;
                case TextTrimming.Default: return StringTrimming.None;
                case TextTrimming.EllipsisCharacter:
                case TextTrimming.EllipsisCharacterWithLineLimit: return StringTrimming.EllipsisCharacter;
                case TextTrimming.EllipsisPath: 
                case TextTrimming.EllipsisPathWithLineLimit: return StringTrimming.EllipsisPath;
                case TextTrimming.EllipsisWord: 
                case TextTrimming.EllipsisWordWithLineLimit: return StringTrimming.EllipsisWord;
                case TextTrimming.None:
                case TextTrimming.NoneWithLineLimit: return StringTrimming.None;
                case TextTrimming.Word:
                case TextTrimming.WordWithLineLimit: return StringTrimming.Word;
            }
            return StringTrimming.None;
        }
    }
}
