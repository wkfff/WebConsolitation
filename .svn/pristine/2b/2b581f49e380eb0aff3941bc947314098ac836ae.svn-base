namespace Krista.FM.RIA.Extensions.E86N.Extensions
{
    public static class ObjectExtensions
    {
        // todo метод не рабочий! надо либо свой пилить либо искать другое решение

        /// <summary>
        /// Клонирование объекта
        /// </summary>
       public static T Clone<T>(this T obj)
        {
            var inst = obj.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            return (T)(inst == null ? inst.Invoke(obj, null) : null);
        }
    }
}
