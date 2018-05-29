using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Moviebase.Core.Utils
{
    /// <summary>
    /// Provides quick IO extension.
    /// </summary>
    public static class IOExtension
    {
        private static readonly Lazy<MD5CryptoServiceProvider> Md5Lazy = new Lazy<MD5CryptoServiceProvider>();
        
        /// <summary>
        /// Check if specified path is directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectory(string path)
        {
            return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }

        /// <summary>
        /// Check if speified path is readonly.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static bool IsReadOnly(string path, bool recursive)
        {
            if (File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly))
            {
                return true;
            }

            if (!IsDirectory(path) || !recursive) return false;
            var items = Directory.EnumerateFileSystemEntries(path, "*", SearchOption.AllDirectories);
            return items.Any(x => IsReadOnly(x, false));
        }

        /// <summary>
        /// Check if specified path is exist.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        /// <summary>
        /// Removes read-only attribute from path.
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="recursive"></param>
        public static void RemoveReadOnly(string targetPath, bool recursive = false)
        {
            var attr = File.GetAttributes(targetPath);
            if (attr.HasFlag(FileAttributes.ReadOnly))
            {
                var modAttr = attr & ~FileAttributes.ReadOnly;
                File.SetAttributes(targetPath, modAttr);
            }

            if (!IsDirectory(targetPath) || !recursive) return;
            var items = Directory.GetFileSystemEntries(targetPath, "*", SearchOption.AllDirectories);
            foreach (string item in items)
            {
                RemoveReadOnly(item);
            }
        }

        /// <summary>
        /// Safely delete specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        /// <param name="ignoreReadonly"></param>
        public static void SafeDelete(string path, bool recursive = false, bool ignoreReadonly = false)
        {
            if (!Exists(path)) return;
            if (ignoreReadonly)
            {
                RemoveReadOnly(path, recursive);
            }

            if (IsDirectory(path))
            {
                Directory.Delete(path, recursive);
            }
            else
            {
                File.Delete(path);
            }
        }

        public static void CloneDirectoryStructure(string sourcePath, string targetPath)
        {
            if (IsDirectory(sourcePath))
            {
                if (Exists(targetPath))
                {
                    SafeDelete(targetPath, true, true);
                }
                Directory.CreateDirectory(targetPath);

                // Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*",
                    SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
                }

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",
                    SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
                }
            }
            else
            {
                File.Copy(sourcePath, targetPath, true);
            }
        }

        /// <summary>
        /// Hash the specified file using MD5.
        /// </summary>
        /// <param name="filePath">Full path to file.</param>
        /// <returns></returns>
        public static string QuickHash(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var bufferSize = Math.Min(1024 * 4096, fs.Length);
                var buffer = new byte[bufferSize];

                fs.Read(buffer, 0, buffer.Length);

                var computed = Md5Lazy.Value.ComputeHash(buffer);
                return BitConverter.ToString(computed);
            }
        }

        /// <summary>
        /// Clean filename from invalid characters.
        /// </summary>
        /// <param name="fileName">Full path to file.</param>
        /// <returns>Path friendly string.</returns>
        public static string CleanFileName(string fileName)
        {
            var pathTokens = fileName.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < pathTokens.Length; i++)
            {
                var token = pathTokens[i];
                pathTokens[i] = Path.GetInvalidFileNameChars().Aggregate(token, (current, c) => current.Replace(c.ToString(), string.Empty));
            }
            return string.Join("\\", pathTokens);
        }

        /// <summary>
        /// Ensure the specified path is unique and has no file duplicate.
        /// </summary>
        /// <param name="fullPath">Full path to file.</param>
        /// <param name="duplicateCount">Duplicate count.</param>
        /// <returns>Non duplicated file path.</returns>
        public static string EnsureNonDuplicateName(string fullPath, out int duplicateCount)
        {
            var fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            var extension = Path.GetExtension(fullPath);
            var path = Path.GetDirectoryName(fullPath);
            Trace.Assert(path != null);

            duplicateCount = 0;
            var newFullPath = fullPath;
            while (File.Exists(newFullPath))
            {
                duplicateCount++;
                var tempFileName = $"{fileNameOnly} ({duplicateCount++})";
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            return newFullPath;
        }
    }
}