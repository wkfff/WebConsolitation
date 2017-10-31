using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.Pkcs;

namespace Krista.FM.DigitalSignature
{
    public static class DSign
    {
        public static ICollection<string> Verify(byte[] attachedSign)
        {
            // Объект, в котором будут происходить декодирование и проверка.
            SignedCms signedCms = new SignedCms();

            // Декодируем сообщение.
            signedCms.Decode(attachedSign);

            if (signedCms.SignerInfos.Count == 0)
            {
                return new Collection<string> { "Документ не подписан." };
            }

            SignerInfoEnumerator enumerator = signedCms.SignerInfos.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SignerInfo current = enumerator.Current;

                try
                {
                    // Используем проверку подписи и стандартную 
                    // процедуру проверки сертификата: построение цепочки, 
                    // проверку цепочки, и необходимых расширений для данного 
                    // сертификата.
                    current.CheckSignature(false);
                }
                catch (System.Security.Cryptography.CryptographicException e)
                {
                    return new Collection<string> { e.Message };
                }
            }

            return new Collection<string>();
        }

        public static ICollection<string> Verify(byte[] detachedSign, byte[] sourceDoc)
        {
            // Создаем объект ContentInfo по сообщению.
            // Это необходимо для создания объекта SignedCms.
            ContentInfo contentInfo = new ContentInfo(sourceDoc);

            // Объект, в котором будут происходить декодирование и проверка.
            // Свойство Detached устанавливаем явно в true, таким 
            // образом сообщение будет отделено от подписи.
            SignedCms signedCms = new SignedCms(contentInfo, true);

            // Декодируем сообщение.
            signedCms.Decode(detachedSign);

            // Проверяем число основных и дополнительных подписей.
            if (signedCms.SignerInfos.Count == 0)
            {
                return new Collection<string> { "Документ не подписан." };
            }

            var enumerator = signedCms.SignerInfos.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SignerInfo current = enumerator.Current;

                try
                {
                    // Используем проверку подписи и стандартную 
                    // процедуру проверки сертификата: построение цепочки, 
                    // проверку цепочки, и необходимых расширений для данного 
                    // сертификата.
                    current.CheckSignature(false);
                }
                catch (System.Security.Cryptography.CryptographicException e)
                {
                    return new Collection<string> { e.Message };
                }

                // При наличии соподписей проверяем соподписи.
                if (current.CounterSignerInfos.Count > 0)
                {
                    SignerInfoEnumerator coenumerator = current.CounterSignerInfos.GetEnumerator();
                    while (coenumerator.MoveNext())
                    {
                        SignerInfo cosigner = coenumerator.Current;

                        try
                        {
                            // Используем проверку подписи и стандартную 
                            // процедуру проверки сертификата: построение цепочки, 
                            // проверку цепочки, и необходимых расширений для данного 
                            // сертификата.
                            cosigner.CheckSignature(false);
                        }
                        catch (System.Security.Cryptography.CryptographicException e)
                        {
                            return new Collection<string> { e.Message };
                        }
                    }
                }
            }

            return new Collection<string>();
        }
    }
}