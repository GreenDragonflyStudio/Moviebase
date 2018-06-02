using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using log4net;
using LiteDB;
using Moviebase.Core.Utils;
using Moviebase.DAL;
using Moviebase.DAL.Entities;
using Moviebase.Services.Title;

namespace Moviebase.Core.Components
{
    /// <inheritdoc />
    public class FileAnalyzer : IFileAnalyzer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileAnalyzer));
        private readonly ITitleProvider _provider;

        /// <inheritdoc />
        public List<string> SubtitleExtensions { get; set; }

        /// <inheritdoc />
        public List<string> PosterExtensions { get; set; }

        /// <summary>
        /// Initialize new instance of <see cref="FileAnalyzer"/>.
        /// </summary>
        /// <param name="titleProvider">Movie title provider.</param>
        public FileAnalyzer(ITitleProvider titleProvider)
        {
            _provider = titleProvider;
        }

        /// <inheritdoc />
        public async Task<AnalyzedFile> Analyze(string filePath)
        {
            var file = new FileInfo(filePath);
            var item = new AnalyzedFile(filePath);
            Log.DebugFormat("Analyzing: {0}", file.Name);

            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var hashCollection = db.GetCollection<MediaFileHash>();
                var fileCollection = db.GetCollection<MediaFile>();

                // lookup for hash
                var hash = IOExtension.QuickHash(filePath);
                item.Hash = hash;
                var hashEntity = hashCollection.FindOne(x => x.Hash == hash);
                
                // the file has recognized hash on database and has matching movie information
                if (hashEntity != null && fileCollection.Exists(x => x.Hash == hashEntity.Hash))
                {
                    item.Title = hashEntity.Title;
                    item.Year = hashEntity.Year;
                    item.ImdbId = hashEntity.ImdbId;
                    item.IsKnown = true;

                    Log.DebugFormat("File found on hash table: {0}", hash);
                }
                else
                {
                    // the file only has one information on database, guessing is needed
                    var cleaned = await _provider.GuessTitle(item.FullPath);
                    item.Title = cleaned.Title;
                    item.Year = cleaned.Year;
                    item.ImdbId = cleaned.ImdbId;
                    item.IsKnown = false;

                    Log.DebugFormat("File hash recorded: {0}", hash);
                }
            }

            // find subtitle and poster
            var extraFile = FindAssociatedFiles(Path.GetDirectoryName(filePath));
            item.SubtitlePath = extraFile.Item1;
            item.PosterPath = extraFile.Item2;
            
            return item;
        }

        private Tuple<string, string> FindAssociatedFiles(string path)
        {
            string subtitle = null, poster = null;
            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly))
            {
                var currentExtension = Path.GetExtension(file);
                if (SubtitleExtensions.Contains(currentExtension))
                    subtitle = file;
                if (PosterExtensions.Contains(currentExtension))
                    poster = file;

                // early break
                if (subtitle != null && poster != null) break;
            }

            return new Tuple<string, string>(subtitle, poster);
        }
    }
}