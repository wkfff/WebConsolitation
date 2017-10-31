﻿using System;
using System.IO;
using System.Security.Cryptography;

namespace Krista.FM.Update.Framework.Utils
{
    public static class FileChecksum
    {
        public static string GetSHA256Checksum(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                SHA256Managed sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }

        public static string GetSHA256Checksum(byte[] fileData)
        {
            SHA256Managed sha = new SHA256Managed();
            byte[] checksum = sha.ComputeHash(fileData);
            return BitConverter.ToString(checksum).Replace("-", String.Empty);
        }
    }
}
