using System;
using System.Data.Common;
using System.Data.OracleClient;

namespace Krista.FM.Common.Exceptions.DbExceptions
{
    public class OracleDbExceptionService : IDbExceptionService
    {
        public DbException GetDbException(Exception e)
        {
            OraCode errCode = GetExceptionCode(e, 0);
            switch (errCode)
            {
                case OraCode.DuplicateKeysFound:
                    return new DbDuplicateKeysFoundException(e);
                
                case OraCode.CanNotDropNonExistentConstraint:
                    return new DbNonExistentConstraintException(e);

                case OraCode.SuchUniqueKeyAlreadyExistsInTable:
                    return new DbSuchUniqueKeyAlreadyExistsInTableException(e);

                case OraCode.NameAlreadyUsedByExistingConstraint:
                    return new DbNameAlreadyUsedByExistingConstraintException(e);

                case OraCode.InvalidColumnName:
                    return new DbInvalidColumnNameException(e);

            }
            
            return null;
        }

        /// <summary>
        /// допустимое количество вложенности элементов
        /// </summary>
        private const int MAX_ITERATION = 100;

        /// <summary>
        /// Рекурсивная функция, находящая среди вложенных исключений исключение с типом OracleException, и возвращает от него номер ошибки БД
        /// </summary>
        /// <param name="e"></param>
        /// <param name="iteration"></param>
        /// <returns></returns>
        private OraCode GetExceptionCode(Exception e, int iteration)
        {
            if (iteration > MAX_ITERATION)
                throw new OverflowException("Ошибка при рекурсивном получении вложенного сообщения");

            OraCode result;

            if (e == null)
                return OraCode.Unknown;

            if (e is OracleException)
            {
                result = (OraCode)((System.Data.OracleClient.OracleException)e).Code;
            }
            else
            {
                result = GetExceptionCode(e.InnerException, iteration + 1);
            }

            return result;
        }
        
        /// <summary>
        /// Коды ошибок Oracle
        /// </summary>
        private enum OraCode : int
        {
           Unknown = -99999,
           DuplicateKeysFound = 2299,
           InvalidColumnName = 904,
           CanNotDropNonExistentConstraint = 2443,
           SuchUniqueKeyAlreadyExistsInTable = 2261,
           NameAlreadyUsedByExistingConstraint = 2264,
           
        }

    }
}
