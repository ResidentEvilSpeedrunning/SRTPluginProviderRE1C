using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SRTPluginProviderRE1C
{
    /// <summary>
    /// SHA256 hashes for the RE1/BIO1 game executables.
    /// </summary>
    public static class GameHashes
    {
        private static readonly byte[] BiohazardWW_19981106_1 = new byte[32] { 0x25, 0xBE, 0x34, 0xD4, 0xF4, 0x58, 0x6E, 0x9E, 0xC8, 0x4E, 0x39, 0x41, 0x29, 0x15, 0xAE, 0x1D, 0xE3, 0x8A, 0xF1, 0xFC, 0x59, 0x24, 0xC0, 0x28, 0x24, 0xD3, 0x8E, 0x15, 0xC8, 0x40, 0x64, 0x58 };

        public static string DetectVersion(string filePath)
        {
            byte[] checksum;
            using (SHA256 hashFunc = SHA256.Create())
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                checksum = hashFunc.ComputeHash(fs);

            if (checksum.SequenceEqual(BiohazardWW_19981106_1))
            {
                Console.WriteLine("ddraw");
                return "ddraw";
            }

            Console.WriteLine("Unknown Version");
            return "Unknown Version";
        }
    }
}