using System;

namespace Krista.FM.Update.Framework.Utils
{
    /// <summary>
    /// Служит для защиты пользовательских секретов, хранимых в .config-файлах 
    /// и других уязвимых источниках 
    /// Для компиляции понадобится подкючить System.Security
    /// </summary>

    public class SecureConfigStringProvider
    {
        static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("Salt Is Not A Password");

        /// <summary>
        /// Шифрует строку уникальным для пользователя ключом        
        /// </summary>        
        /// <returns>Зашифрованная строка в base64</returns>
        public static string EncryptString(string input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(input),
                entropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Расшифрует строку, зашифрованную EncryptString
        /// </summary>
        /// <param name="encryptedData">Зашифрованная строка в base64</param>
        /// <returns>Расшифровання строка</returns>
        public static string DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return System.Text.Encoding.Unicode.GetString(decryptedData);
            }
            catch
            {
                return "";
            }
        }
    }
}
