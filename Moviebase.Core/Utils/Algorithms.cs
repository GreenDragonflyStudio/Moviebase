using System;
using System.IO;
using System.Text;

namespace Moviebase.Core.Utils.Algorithms
{
    /// <summary>
    ///     Represents an hashing algorythm
    /// </summary>
    public enum HashAlgorythm
    {
        /// <summary>
        ///     Used for definint custom Hashing algorythms
        /// </summary>
        Custom = 0,

        /// <summary>
        ///     MD5 encoding created by Ronald Rivest in 1991 and defined in RFC 1321.
        /// </summary>
        MD5,

        /// <summary>
        ///     SHA-512 (512 bit) is part of SHA-2 set of cryptographic hash functions, designed by the U.S. National Security
        ///     Agency (NSA) and published in 2001
        /// </summary>
        SHA512
    }

    public interface IHashProvider

    {
        /// <summary>

        ///     Gests the hashing algorythm used by this provider to compute hashes

        /// </summary>

        #region Properties

        HashAlgorythm Algorythm { get; }

        #endregion Properties

        /// <summary>

        ///     Computes the hash for a file

        /// </summary>

        /// <param name="file">File to be hashed</param>

        /// <returns></returns>

        #region Methods

        byte[] Hash(FileInfo file);

        /// <summary>

        ///     Computes the hash for a string

        /// </summary>

        /// <param name="text"></param>

        /// <returns></returns>

        byte[] Hash(string text);

        /// <summary>

        ///     Computes the hash for a byte array

        /// </summary>

        /// <param name="bytes"></param>

        /// <returns></returns>

        byte[] Hash(byte[] bytes);

        #endregion Methods
    }

    public static class Distance
    {
        #region Methods

        public static int DamerauLevenshteinDistance(string s, string t)
        {
            var bounds = new { Height = s.Length + 1, Width = t.Length + 1 };

            int[,] matrix = new int[bounds.Height, bounds.Width];

            for (int height = 0; height < bounds.Height; height++) { matrix[height, 0] = height; }
            for (int width = 0; width < bounds.Width; width++) { matrix[0, width] = width; }

            for (int height = 1; height < bounds.Height; height++)
            {
                for (int width = 1; width < bounds.Width; width++)
                {
                    int cost = (s[height - 1] == t[width - 1]) ? 0 : 1;
                    int insertion = matrix[height, width - 1] + 1;
                    int deletion = matrix[height - 1, width] + 1;
                    int substitution = matrix[height - 1, width - 1] + cost;

                    int distance = Math.Min(insertion, Math.Min(deletion, substitution));

                    if (height > 1 && width > 1 && s[height - 1] == t[width - 2] && s[height - 2] == t[width - 1])
                    {
                        distance = Math.Min(distance, matrix[height - 2, width - 2] + cost);
                    }

                    matrix[height, width] = distance;
                }
            }

            return matrix[bounds.Height - 1, bounds.Width - 1];
        }

        public static int LevenshteinDistance(string source1, string source2)
        {
            var source1Length = source1.Length;

            var source2Length = source2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length

            if (source1Length == 0)

                return source2Length;

            if (source2Length == 0)

                return source1Length;

            // Initialization of matrix with row size source1Length and columns size source2Length

            for (var i = 0; i <= source1Length; matrix[i, 0] = i++)
            {
                // Init Matrix
            }
            for (var j = 0; j <= source2Length; matrix[0, j] = j++)
            {
                // Init Matrix
            }

            // Calculate rows and collumns distances

            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                                   Math.Min(matrix[i - 1, j] + 1,
                                            matrix[i, j - 1] + 1),
                                            matrix[i - 1, j - 1] + cost);
                }
            }

            // return result
            return matrix[source1Length, source2Length];
        }

        #endregion Methods
    }

    /// <summary>

    /// Defines a component for compute hash of data

    /// </summary> <summary>

    /// Defines a component for computing hashes based on MD5 Algorythm

    /// </summary>

    public class MD5 : IHashProvider

    {
        /// <summary>

        ///     Gests the hashing algorythm used by this provider to compute hashes

        /// </summary>

        #region Properties

        public HashAlgorythm Algorythm => HashAlgorythm.MD5;

        #endregion Properties

        /// <summary>

        ///     Computes the hash for a file using the MD5 Algorythm

        /// </summary>

        /// <param name="file">File to be hashed</param>

        /// <returns></returns>

        #region Methods

        public static byte[] HashBytes(byte[] bytesToHash)

        {
            return GetInstance().Hash(bytesToHash);
        }

        public static byte[] HashFile(FileInfo fileToHash)

        {
            return GetInstance().Hash(fileToHash);
        }

        public static byte[] HashString(string textToHash)

        {
            return GetInstance().Hash(textToHash);
        }

        public byte[] Hash(FileInfo file)

        {
            return MD5File(file);
        }

        /// <summary>

        ///     Computes the hash for a string using the MD5 Algorythm

        /// </summary>

        /// <param name="text"></param>

        /// <returns></returns>

        public byte[] Hash(string text)

        {
            return MD5Bytes(Encoding.Default.GetBytes(text));
        }

        /// <summary>

        ///     Computes the hash for a byte array using the MD5 Algorythm

        /// </summary>

        /// <param name="bytes"></param>

        /// <returns></returns>

        public byte[] Hash(byte[] bytes)

        {
            return MD5Bytes(bytes);
        }

        private static MD5 GetInstance()

        {
            return new MD5();
        }

        private static byte[] MD5Bytes(byte[] bytes)

        {
            // Create a new instance of the MD5CryptoServiceProvider object.

            byte[] data;

            using (var md5Hasher = System.Security.Cryptography.MD5.Create())

            {
                data = md5Hasher.ComputeHash(bytes);
            }

            return data;
        }

        private static byte[] MD5File(FileInfo file)

        {
            byte[] mHash;

            using (var fs = file.OpenRead())

            {
                // definizione del nostro tipo

                using (var sscMd5 = System.Security.Cryptography.MD5.Create())

                {
                    mHash = sscMd5.ComputeHash(fs);
                }
            }

            return mHash;
        }

        #endregion Methods

        /// <summary>

        /// Provides a static interface to hash a string

        /// </summary>

        /// <param name="textToHash">string to hash</param>

        /// <returns></returns>
        /// <summary>

        /// Provides a static interface to hash a string

        /// </summary>

        /// <param name="fileToHash">file to hash</param>

        /// <returns></returns>
        /// <summary>

        /// Provides a static interface to hash an array of bytes

        /// </summary>

        /// <param name="bytesToHash">data to hash</param>

        /// <returns></returns>
    }
}