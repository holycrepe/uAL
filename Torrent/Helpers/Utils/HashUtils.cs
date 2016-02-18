using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Torrent.Extensions;

namespace Torrent.Helpers.Utils
{
    public static class HashUtils
    {
        public static string EmptySHA1Hash = new string('0', 20);
        static SHA1CryptoServiceProvider hasher;
        public static string SHA1()
            => EmptySHA1Hash;
        public static string SHA1(Stream stream)
            => (hasher ?? (hasher = new SHA1CryptoServiceProvider())).ComputeHash(stream).ToHex();
        public static string SHA1(byte[] bytes)
            => (hasher ?? (hasher = new SHA1CryptoServiceProvider())).ComputeHash(bytes).ToHex();
        public static string SHA1(IEnumerable<byte> bytes)
            => SHA1(bytes.ToArray());
    }
}
