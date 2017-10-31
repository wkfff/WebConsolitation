using System;

namespace Krista.FM.Client.iMonitoringWM.Common.Cryptography
{
    public static class CryptUtils
    {
        /// <summary>
        /// Вычислить хэш (SHA512Managed) пароля
        /// </summary>
        /// <param name="password">Пароль</param>
        /// <returns>хэш (Base64String)</returns>
        public static string GetPasswordHash(string password)
        {
            byte[] pwdByteBuff = null;
            if (String.IsNullOrEmpty(password))
            {
                pwdByteBuff = new byte[1] { 0 };
            }
            else
            {
                pwdByteBuff = new byte[password.Length];
                for (int i = 0; i < pwdByteBuff.Length; i++)
                {
                    pwdByteBuff[i] = (byte)password[i];
                }
            }
            SHA512Managed crypter = new SHA512Managed();
            byte[] pwdHashBuff = crypter.ComputeHash(pwdByteBuff);
            string str = string.Empty;
            foreach (byte b in pwdHashBuff)
            {
                str += string.Format("{0:X}", b);
            }
            return Convert.ToBase64String(pwdHashBuff);
        }
    }
}
