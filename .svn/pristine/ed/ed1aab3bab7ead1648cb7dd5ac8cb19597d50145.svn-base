namespace Krista.FM.RIA.Extensions.E86N.Services.Params
{
    public interface IParamsMap
    {
        /// <summary>
        /// Добавить параметр
        /// </summary>
        /// <param name="key"> Ключ параметра</param>
        /// <param name="value"> Заначение параметра </param>
        /// <returns> Коллекция параметров </returns>
        ParamsMap SetParam(string key, object value);

        /// <summary>
        /// Получить параметр по ключу
        /// </summary>
        /// <typeparam name="T"> тип параметра </typeparam>
        /// <param name="key"> ключ параметра</param>
        /// <returns> параметр заданого типа</returns>
        T GetParam<T>(string key);

        /// <summary>
        /// Проверка на наличие параметра в коллекции
        /// </summary>
        /// <param name="key"> ключ значения</param>
        /// <returns> true - ключ присутствует в коллекции, иначе - false</returns>
        bool HasParam(string key);
    }
}
