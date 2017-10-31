using System;
using System.Data.Common;

namespace Krista.FM.Common.Exceptions.DbExceptions
{
    public interface IDbExceptionService
    {
        /// <summary>
        /// Анализирует текст исключения(включая вложенные), определяя тип ошибки, возникший в БД
        /// </summary>
        /// <param name="e">Исключение, содержащее сообщение об ошибке из БД</param>
        /// <returns>Исключение БД, одно из наследников DbException, либо NULL</returns>
        DbException GetDbException(Exception e);
    } 

}
