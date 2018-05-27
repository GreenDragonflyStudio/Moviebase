using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Moviebase.Core.Utils
{
    public static class IOExtension
    {
        private static Lazy<MD5CryptoServiceProvider> _md5Lazy = new Lazy<MD5CryptoServiceProvider>();

        public static bool IsEmpty(this DirectoryInfo dir)
        {
            return !dir.GetFiles().Any() && !dir.GetDirectories().Any();
        }
        
        /// <summary>
        ///     True if path is a directory
        /// </summary>
        public static bool IsDirectory(string path)
        {
            return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }

        /// <summary>
        ///     True if a filesystem entry has the readonly attribute
        ///     or, if directory and recursive, contains a readonly entry
        /// </summary>
        public static bool IsReadonly(string path, bool recursive)
        {
            if (File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly))
            {
                return true;
            }

            if (IsDirectory(path) && recursive)
            {
                string[] items = Directory.GetFileSystemEntries(path, "*", SearchOption.AllDirectories);
                if (items.Any(x => IsReadonly(x, false)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     True if a filesystem entry exists (file or directory)
        /// </summary>
        public static bool Exists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        /// <summary>
        /// Removes the readonly attribute from a file system entry
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="recursive">
        /// If target is a directory removes the attribute from whole filesystem entries in the subtree
        /// </param>
        public static void RemoveReadonly(string targetPath, bool recursive = false)
        {
            var attr = File.GetAttributes(targetPath);
            if (attr.HasFlag(FileAttributes.ReadOnly))
            {
                FileAttributes modAttr = attr & ~FileAttributes.ReadOnly;
                File.SetAttributes(targetPath, modAttr);
            }

            if (!IsDirectory(targetPath) || !recursive) return;
            var items = Directory.GetFileSystemEntries(targetPath, "*", SearchOption.AllDirectories);
            foreach (string item in items)
            {
                RemoveReadonly(item);
            }
        }

        /// <summary>
        /// Deletes a filesystem entry (file or directory) if exists
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive">If target is a directory deletes the whole subtree</param>
        /// <param name="ignoreReadonly">Force deletion of readonly entries</param>
        public static void SafeDelete(string path, bool recursive = false, bool ignoreReadonly = false)
        {
            if (!Exists(path)) return;
            if (ignoreReadonly)
            {
                RemoveReadonly(path, recursive);
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

        /// <summary>
        /// Clones a file or a directory tree to a new path. All files in the target will be erased.
        /// </summary>
        public static void ClonePath(string sourcePath, string targetPath)
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
        /// Renames a file or a directory
        /// </summary>
        public static void Rename(string originalPath, string renamedPath)
        {
            if (IsDirectory(originalPath))
            {
                Directory.Move(originalPath, renamedPath);
            }
            else
            {
                File.Move(originalPath, renamedPath);
            }
        }

        /// <summary>
        /// Creates a directory if not exists
        /// </summary>
        public static void SafeCreateDirectory(string target)
        {
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }
        }

        public static string QuickHash(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var bufferSize = Math.Min(1024 * 4096, fs.Length);
                var buffer = new byte[bufferSize];

                fs.Read(buffer, 0, buffer.Length);

                var computed = _md5Lazy.Value.ComputeHash(buffer);
                return BitConverter.ToString(computed);
            }
        }
    }
}