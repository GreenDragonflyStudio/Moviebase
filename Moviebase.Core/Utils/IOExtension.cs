using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

// ReSharper disable once InconsistentNaming

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
        /// <param name="path">Full path to directory being checked.</param>
        /// <returns><c>True</c> if the specified path is a directory, otherwise <c>Flase</c>.</returns>
        public static bool IsDirectory(string path)
        {
            return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }

        /// <summary>
        /// Check if specified path is exist.
        /// </summary>
        /// <param name="path">Full path to directory or file.</param>
        /// <returns><c>True</c> if the specified path is exist, otherwise <c>Flase</c>.</returns>
        public static bool Exists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        /// <summary>
        /// Check if speified path is readonly.
        /// </summary>
        /// <param name="path">Full path to directory or file.</param>
        /// <param name="recursive">If the specified path is a directory, check all file system for read-only attribute.</param>
        /// <returns><c>True</c> if the specified path contains read-only attribute, otherwise <c>Flase</c>.</returns>
        public static bool IsReadOnly(string path, bool recursive)
        {
            if (File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly))
            {
                return true;
            }

            if (!IsDirectory(path) || !recursive) return false;
            return Directory.EnumerateFileSystemEntries(path, "*", SearchOption.AllDirectories)
                .Any(x => IsReadOnly(x, false));
        }

        /// <summary>
        /// Removes read-only attribute from path.
        /// </summary>
        /// <param name="targetPath">Full path to directory or file.</param>
        /// <param name="recursive">If the specified path is a directory, remove all file system from read-only attribute.</param>
        public static void RemoveReadOnly(string targetPath, bool recursive = false)
        {
            var attr = File.GetAttributes(targetPath);
            if (attr.HasFlag(FileAttributes.ReadOnly))
            {
                var modAttr = attr & ~FileAttributes.ReadOnly;
                File.SetAttributes(targetPath, modAttr);
            }

            if (!IsDirectory(targetPath) || !recursive) return;
            foreach (var item in Directory.EnumerateFileSystemEntries(targetPath, "*", SearchOption.AllDirectories))
            {
                RemoveReadOnly(item);
            }
        }

        /// <summary>
        /// Safely delete specified path.
        /// </summary>
        /// <param name="path">Full path to delete.</param>
        /// <param name="recursive">Recrusively delete all folders from parent path.</param>
        /// <param name="ignoreReadonly">Ignore read-only attribute.</param>
        public static void SafeDelete(string path, bool recursive = false, bool ignoreReadonly = false)
        {
            if (!Exists(path)) return;
            if (ignoreReadonly) RemoveReadOnly(path, recursive);

            if (IsDirectory(path))
            {
                Directory.Delete(path, recursive);
            }
            else
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Clone <paramref name="sourcePath"/> into <paramref name="targetPath"/> preserving directory structure.
        /// </summary>
        /// <param name="sourcePath">Source path to clone.</param>
        /// <param name="targetPath">Destination path to place cloned path.</param>
        /// <remarks>All files on <paramref name="targetPath"/> will be overwritten.</remarks>
        public static void CloneDirectoryStructure(string sourcePath, string targetPath)
        {
            if (IsDirectory(sourcePath))
            {
                if (Exists(targetPath)) SafeDelete(targetPath, true, true);
                Directory.CreateDirectory(targetPath);
                
                //Copy all the files & Replaces any files with the same name
                foreach (var newPath in Directory.EnumerateFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    var currentPath = Path.GetDirectoryName(newPath);
                    Debug.Assert(currentPath != null, "currentPath != null");
                    if (!Directory.Exists(currentPath)) Directory.CreateDirectory(currentPath);

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
        /// <returns><see cref="string"/> representation of current file hash.</returns>
        /// <remarks>
        /// The file is hashed using a maximum buffer of 1024 * 4096 bytes. The hashing algorithm used is MD5.
        /// The resulting hash bytes is converted using <see cref="BitConverter"/>.
        /// </remarks>
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
        /// <param name="filePath">Full path to file.</param>
        /// <returns>Path friendly string with stripped invalid path characters.</returns>
        public static string CleanFilePath(string filePath)
        {
            var pathTokens = filePath.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
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