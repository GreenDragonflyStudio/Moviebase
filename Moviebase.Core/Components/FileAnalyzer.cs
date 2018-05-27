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
    public class FileAnalyzer : IFileAnalyzer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileAnalyzer));

        private readonly ITitleProvider _provider;

        public FileAnalyzer(ITitleProvider titleProvider)
        {
            _provider = titleProvider;
        }

        public async Task<AnalyzedFile> Analyze(string filePath)
        {
            var file = new FileInfo(filePath);
            var item = new AnalyzedFile(filePath);
            Log.DebugFormat("Analyzing: {0}", file.Name);

            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var hashCollection = db.GetCollection<MediaFileHash>();
                var movieCollection = db.GetCollection<Movie>();
                var hash = IOExtension.QuickHash(filePath);
                item.Hash = hash;

                Log.DebugFormat("Hash found: {0}", hash);
                var hashEntity = hashCollection.FindOne(x => x.Hash == hash);
                if (hashEntity != null && movieCollection.FindOne(x => x.ImdbId == hashEntity.ImdbId) != null)
                {
                    item.Title = hashEntity.Title;
                    item.Year = hashEntity.Year;
                    item.ImdbId = hashEntity.ImdbId;
                    item.IsKnown = true;

                    Log.DebugFormat("File found on hash table: {0}", hash);
                }
                else
                {
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