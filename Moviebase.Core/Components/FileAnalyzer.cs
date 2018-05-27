using System.Diagnostics;
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
                var hashCollection = db.GetCollection<MovieHash>();
                var movieCollection = db.GetCollection<Movie>();
                var hash = IOExtension.QuickHash(filePath);

                //var hashEntity = hashCollection.FindOne(x => x.Hash == hash);
                //if (hashEntity != null)
                //{
                //    var movieEntity = movieCollection.FindById(hashEntity.MovieId);

                //    item.Title = movieEntity.Title;
                //    item.ImdbId = movieEntity.ImdbId;
                //    item.Year = movieEntity.Year;
                //    item.MovieId = movieEntity.Id;
                //    item.IsKnown = true;

                //    Log.DebugFormat("Hash Found: {0}", hash);
                //}
                //else
                {
                    var cleaned = await _provider.GuessTitle(item.FullPath);
                    item.Title = cleaned.Title;
                    item.Year = cleaned.Year;
                    item.IsKnown = false;

                    // ADD ID HERE
                    hashCollection.Insert(new MovieHash {Hash = hash});
                    hashCollection.EnsureIndex(x => x.Id);

                    Log.DebugFormat("Hash Computed: {0}", hash);
                    Debug.Print(file.Name + " - " + hash);
                }
            }

            return item;
        }
    }
}