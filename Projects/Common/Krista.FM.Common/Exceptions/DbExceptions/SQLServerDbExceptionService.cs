using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Krista.FM.Common.Exceptions.DbExceptions
{
    public class SQLServerDbExceptionService : IDbExceptionService
    {
        public DbException GetDbException(Exception e)
        {
            MsSqlCode errCode = GetExceptionCode(e, 0);
            switch (errCode)
            {
                case MsSqlCode.DuplicateKeysFound:
                    return new DbDuplicateKeysFoundException(e);

                case MsSqlCode.CanNotDropNonExistentConstraint:
                    return new DbNonExistentConstraintException(e);

                /*case MsSqlCode.SuchUniqueKeyAlreadyExistsInTable:   //в SqlServer можно создавать индексы с разными именами и одинаковым набором столбцов - УЖОС!
                    return new DbSuchUniqueKeyAlreadyExistsInTableException(e);
                 */

                case MsSqlCode.NameAlreadyUsedByExistingConstraint:
                    return new DbNameAlreadyUsedByExistingConstraintException(e);

                case MsSqlCode.InvalidColumnName:
                    return new DbInvalidColumnNameException(e);

            }

            return null;
        }

        /// <summary>
        /// допустимое количество вложенности элементов
        /// </summary>
        private const int MAX_ITERATION = 100;


        /// <summary>
        /// Рекурсивная функция, находящая среди вложенных исключений исключение с типом SqlException, и возвращает от него номер ошибки БД
        /// </summary>
        /// <param name="e"></param>
        /// <param name="iteration"></param>
        /// <returns></returns>
        private MsSqlCode GetExceptionCode(Exception e, int iteration)
        {
            if (iteration > MAX_ITERATION)
                throw new OverflowException("Ошибка при рекурсивном получении вложенного сообщения");

            MsSqlCode result;

            if (e == null)
                return MsSqlCode.Unknown;

            if (e is SqlException)
            {
                result = (MsSqlCode) ((SqlException)e).Number;
            }
            else
            {
                result = GetExceptionCode(e.InnerException, iteration + 1);
            }

            return result;

        }


        /// <summary>
        /// Коды ошибок MS SqlServer
        /// </summary>
        private enum MsSqlCode : int
        {
            Unknown = -99999,
            DuplicateKeysFound = 1505,
            InvalidColumnName = 1911,
            CanNotDropNonExistentConstraint = 3728,
            NameAlreadyUsedByExistingConstraint = 2714
        }

    }
}
