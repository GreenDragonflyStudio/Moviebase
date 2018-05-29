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

            return item;
        }
    }
}