using System;
using System.Reflection;

namespace Krista.FM.Common
{
	/// <summary>
	/// Управляет единственным экземпляром класса.
	/// </summary>
	/// <typeparam name="T">Тип класса-одиночки.</typeparam>
	[System.Diagnostics.DebuggerStepThrough]
	public static class Singleton<T> where T : class
	{
		#region Поля

		/// <summary>
		/// Единственный экземпляр целевого класса.
		/// </summary>
		private static volatile T instance;

		/// <summary>
		/// Служебный объект, используемый для блокировки.
		/// </summary>
		private static readonly object lockObject = new object();

		#endregion Поля

		#region Свойства

		/// <summary>
		/// Возвращает единственный экземпляр класса. 
		/// </summary>
		public static T Instance
		{
			get
			{
				try
				{
					if (instance == null)
						lock (lockObject)
						{
							if (instance == null)
								instance = typeof(T).InvokeMember(typeof(T).Name, 
									BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic, 
									null, null, null) as T;
						}

					return instance;
				}
				catch (Exception exception)
				{
					if (exception.InnerException != null)
						throw new SingletonException(exception.InnerException);
					throw new SingletonException(exception);
				}
			}
		}

		#endregion Свойства
	}
}
