namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine
{
    public interface IDatabaseObjectHashNameResolver
    {
        /// <summary>
        /// Возвращает сужествующее хеш-имя, для имен длиннее 30 символов, или null, если хеш-имя не существует.
        /// Формат имени: [первые 20 символов от оригинального мени]$[хеш (7 символов base64)]$[количество коллизий]
        /// </summary>
        string Get(string longName, ObjectTypes objectType);

        /// <summary>
        /// Создает хеш-имя, для имен длиннее 30 символов. 
        /// Если такое имя уже зарегистрировано, то кидает исключение InvalidOperationException.
        /// Формат имени: [первые 20 символов от оригинального мени]$[хеш (7 символов base64)]$[количество коллизий]
        /// </summary>
        string Create(string longName, ObjectTypes objectType);

        /// <summary>
        /// Удаляет хеш-имя, для имен длиннее 30 символов.
        /// </summary>
        void Delete(string longName, ObjectTypes objectType);
    }
}