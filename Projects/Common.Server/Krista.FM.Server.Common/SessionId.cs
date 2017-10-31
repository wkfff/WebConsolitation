using System;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;

using Krista.FM.Common;

namespace Krista.FM.Server.Common
{
    internal class SessionId
    {
        #region Поля

        internal const int ENCODING_BITS_PER_CHAR = 5;
        internal const int ID_LENGTH_BITS = 120;
        internal const int ID_LENGTH_BYTES = 15;
        internal const int ID_LENGTH_CHARS = 0x18;
        internal const int NUM_CHARS_IN_ENCODING = 0x20;
        private static char[] s_encoding;
        private static bool[] s_legalchars;

        #endregion Поля

        #region Методы

        static SessionId()
        {
            s_encoding = new char[0x20] { 
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 
                'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5'
            };
            
            s_legalchars = new bool[0x80];
            for (int i = s_encoding.Length - 1; i >= 0; i--)
            {
                char ch = s_encoding[i];
                s_legalchars[ch] = true;
            }
        }

        private SessionId()
        {
        }

        internal static string Create(ref RandomNumberGenerator randgen)
        {
            if (randgen == null)
            {
                randgen = new RNGCryptoServiceProvider();
            }
            byte[] buffer = new byte[15];
            randgen.GetBytes(buffer);
            return Encode(buffer);
        }

        private static string Encode(byte[] buffer)
        {
            char[] chArray = new char[ID_LENGTH_CHARS];
            int num2 = 0;
            for (int pos = 0; pos < 15; pos += 5)
            {
                int num4 = ((buffer[pos] | (buffer[pos + 1] << 8)) | (buffer[pos + 2] << 0x10)) | (buffer[pos + 3] << 0x18);
                int num3 = num4 & 0x1f;
                chArray[num2++] = s_encoding[num3];
                num3 = (num4 >> 5) & 0x1f;
                chArray[num2++] = s_encoding[num3];
                num3 = (num4 >> 10) & 0x1f;
                chArray[num2++] = s_encoding[num3];
                num3 = (num4 >> 15) & 0x1f;
                chArray[num2++] = s_encoding[num3];
                num3 = (num4 >> 20) & 0x1f;
                chArray[num2++] = s_encoding[num3];
                num3 = (num4 >> 0x19) & 0x1f;
                chArray[num2++] = s_encoding[num3];
                num4 = (num4 >> 30) | (buffer[pos + 4] << 2);
                num3 = num4 & 0x1f;
                chArray[num2++] = s_encoding[num3];
                num3 = (num4 >> 5) & 0x1f;
                chArray[num2++] = s_encoding[num3];
            }
            return new string(chArray);
        }

        internal static bool IsLegit(string s)
        {
            bool flag;
            if ((s == null) || (s.Length != ID_LENGTH_CHARS))
            {
                return false;
            }
            try
            {
                int i = ID_LENGTH_CHARS;
                while (--i >= 0)
                {
                    char ch = s[i];
                    if (!s_legalchars[ch])
                    {
                        return false;
                    }
                }
                flag = true;
            }
            catch (IndexOutOfRangeException)
            {
                flag = false;
            }
            return flag;
        }
 
        #endregion Методы

    }

    internal class SessionIDManager
    {
        private RandomNumberGenerator _randgen;


        public void Initialize()
        {
        }

        public bool InitializeRequest(LogicalCallContextData context, string schemeName)
        {
            if (context["SessionIDManagerInitializeRequestCalled"] != null)
            {
                return false;
            }
            context["SchemeName"] = schemeName;
            context["SessionID"] = SessionId.Create(ref _randgen);
            context["SessionIDManagerInitializeRequestCalled"] = true;
            return true;
        }

        public void ClearRequest(LogicalCallContextData context)
        {
            if (context["SessionIDManagerInitializeRequestCalled"] != null)
            {
                context["SchemeName"] = null;
                context["SessionID"] = null;
                context["SessionIDManagerInitializeRequestCalled"] = null;
            }
        }

        internal static bool CheckIdLength(string id, bool throwOnFail)
        {
            bool flag = true;
            if (id.Length <= 80)
            {
                return flag;
            }
            if (throwOnFail)
            {
                throw new Exception("Session_id_too_long");
            }
            return false;
        }

        private static void CheckInitializeRequestCalled(LogicalCallContextData context)
        {
            if (context["SessionIDManagerInitializeRequestCalled"] == null)
            {
                throw new Exception("SessionIDManager_InitializeRequest_not_called");
            }
        }

        public virtual string CreateSessionID(CallContext сontext)
        {
            return SessionId.Create(ref this._randgen);
        }

        public string GetSessionID(LogicalCallContextData context)
        {
            string text1;

            CheckInitializeRequestCalled(context);
            text1 = this.Decode(Convert.ToString(context["SessionID"]));
            if ((text1 != null) && !this.Validate(text1))
            {
                text1 = null;
            }
            return text1;
        }

        public virtual string Decode(string id)
        {
            return id.ToLower();
        }

        public virtual bool Validate(string id)
        {
            if (CheckIdLength(id, false))
            {
                return SessionId.IsLegit(id);
            }
            return false;
        }

    }
}