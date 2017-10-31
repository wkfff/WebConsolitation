using System;
using System.Data.Common;
using System.Runtime.Serialization;

namespace Krista.FM.Common.Exceptions.DbExceptions
{
    /// <summary>
    /// Нарушение уникальности записей в таблице
    /// </summary>
    [Serializable]
    public class DbDuplicateKeysFoundException : DbException
    {
        public DbDuplicateKeysFoundException(Exception parent)
            : base("Обнаружены неуникальные записи", parent)
        {
        }

        protected DbDuplicateKeysFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Отсутствует констрейнт с таким именем
    /// </summary>
    [Serializable]
    public class DbNonExistentConstraintException : DbException
    {
        public DbNonExistentConstraintException(Exception parent)
            : base("Не существует ограничение целостности с таким именем", parent)
        {
        }

        protected DbNonExistentConstraintException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Уникальный ключ с таким набором столбцов уже существует
    /// </summary>
    [Serializable]
    public class DbSuchUniqueKeyAlreadyExistsInTableException : DbException
    {
        public DbSuchUniqueKeyAlreadyExistsInTableException(Exception parent)
            : base("Уникальный ключ с таким набором столбцов уже существует", parent)
        {
        }

        protected DbSuchUniqueKeyAlreadyExistsInTableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Ограничение целостности с таким именем уже существует
    /// </summary>
    [Serializable]
    public class DbNameAlreadyUsedByExistingConstraintException : DbException
    {
        public DbNameAlreadyUsedByExistingConstraintException(Exception parent)
            : base("Ограничение целостности с таким именем уже существует", parent)
        {
        }

        protected DbNameAlreadyUsedByExistingConstraintException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// У таблицы отсутствует столбец с таким именем
    /// </summary>
    [Serializable]
    public class DbInvalidColumnNameException : DbException
    {
        public DbInvalidColumnNameException(Exception parent)
            : base("У таблицы отсутствует столбец с таким именем", parent)
        {
        }
        protected DbInvalidColumnNameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

}
