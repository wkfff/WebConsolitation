using System;

namespace Krista.FM.Client.iMonitoringWM.Common.Cryptography
{
    public class SHA512Managed
    {
        private byte[] _hash = null;
        public byte[] Hash
        {
            get { return _hash; }
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            _hash = SHA512.MessageSHA512(buffer);
            return _hash;
        }
    }
}
