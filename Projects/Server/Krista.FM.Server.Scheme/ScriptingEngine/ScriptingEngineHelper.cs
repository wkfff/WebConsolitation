using System;

namespace Krista.FM.Server.Scheme.ScriptingEngine
{
	public static class ScriptingEngineHelper
	{
		/// <summary>
		/// Проверка на русские символы
		/// </summary>
		/// <param name="value"></param>
		public static void CheckDBName(string value)
		{
			if (value.Length > 30 || value.Length < 1)
				throw new Exception("Недопустимое количество символов в наименовании. Допустимые размеры : 1 - 30 символов.");

			if (char.IsDigit(value[0]))
				throw new Exception(String.Format("Наименование не может начинаться с цифры {0}.", value[0]));

			foreach (char ch in value)
			{
				if (!((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || char.IsDigit(ch)))
				{
					throw new Exception(String.Format("В наименовании присутствует недопустимый символ {0}. Допустимые символы : латинские буквы и цифры.", ch));
				}
			}
		}

		/// <summary>
		/// Проверка на недопустимые символы
		/// </summary>
		/// <param name="value">Анализируемая строка</param>
		/// <param name="invalidCharacters">Массив недопустимых символов</param>
		public static void CheckOlapName(string value, char[] invalidCharacters)
		{
			if (value == null)
				return;

			int index = value.LastIndexOfAny(invalidCharacters);
			if (index != -1)
				throw new Exception(String.Format("В наименовании встретился недопустимый символ {0}", value[index]));
		}

		/// <summary>
		/// Проверка на недопустимые символы(, ; ' ` : / \ * | ? " & % $ ! - + = () [] {})
		/// </summary>
		/// <param name="value">Анализируемая строка</param>
		public static void CheckOlapName(string value)
		{
			CheckOlapName(value, ".,;'`:/\\*|?\"&%$!-+=()[]{}".ToCharArray());
		}
	}
}
