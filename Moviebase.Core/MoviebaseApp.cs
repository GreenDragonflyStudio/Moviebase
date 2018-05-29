using log4net;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using Moviebase.Core.Components;
using Moviebase.DAL;
using Moviebase.DAL.Entities;
using TMDbLib.Client;

namespace Moviebase.Core
{
    /// <inheritdoc />
    public sealed partial class MoviebaseApp : IMoviebaseApp
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MoviebaseApp));
        private readonly TMDbClient _apiClient;
        private readonly IFileScanner _scanner;
        private readonly IFileAnalyzer _analyzer;
        private readonly IFileOrganizer _fileOrganizer;

        /// <inheritdoc />
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Initialize new instance of <see cref="MoviebaseApp"/>.
        /// </summary>
        /// <param name="scanner">File scanner.</param>
        /// <param name="analyzer">File analyzer.</param>
        /// <param name="organizer">File organizer.</param>
        public MoviebaseApp(IFileScanner scanner, IFileAnalyzer analyzer, IFileOrganizer organizer)
        {
            _scanner = scanner;
            _analyzer = analyzer;
            _fileOrganizer = organizer;

            _apiClient = new TMDbClient(GlobalSettings.Default.ApiKey)
            {
                DefaultLanguage = "id",
                DefaultCountry = "ID"
            };
        }

        /// <inheritdoc />
        public async Task ScanDirectoryAsync(string scanPath, CancellationToken? cancellationToken = null)
        {
            var files = _scanner.Scan(scanPath).ToList();
            var totalItems = files.Count;

            for (var index = 0; index < files.Count; index++)
            {
                if (cancellationToken?.IsCancellationRequested == true) break;

                await ScanFileAsync(files[index]);
                OnProgressChanged(index, totalItems);
            }

            RecordScanFolder(scanPath);
        }

        /// <inheritdoc />
        public async Task ScanFileAsync(string filePath)
        {
            var analyzedFile = await _analyzer.Analyze(filePath);
            Movie movie;

            if (analyzedFile.IsKnown)
            {
                movie = GetMovieFromDatabase(analyzedFile.ImdbId);
                Log.Debug("Use local movie information");
            }
            else
            {
                movie = await IdentifyFile(analyzedFile);
                Log.Debug("Use remote movie information");
            }

            // sync database
            RecordScanFile(movie, analyzedFile);
        }

        /// <inheritdoc />
        public Task OrganizeFileAsync(CancellationToken? cancellationToken = null)
        {
            return Task.Run(() =>
            {
                using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
                {
                    var mediaCollection = db.GetCollection<MediaFile>();
                    foreach (var mediaFile in mediaCollection.FindAll())
                    {
                        _fileOrganizer.Organize(mediaFile);
                    }
                }
            });
        }
       
    }
}