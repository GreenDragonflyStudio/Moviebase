using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using LiteDB;
using Moviebase.Core.Components;
using Moviebase.DAL;
using Moviebase.DAL.Entities;
using TMDbLib.Client;

namespace Moviebase.Core.App
{
    /// <inheritdoc />
    public sealed partial class MoviebaseApp : IMoviebaseApp
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MoviebaseApp));
        private readonly TMDbClient _apiClient;
        private readonly IMoviebaseDAL _dal;
        private readonly IFileScanner _fileScanner;
        private readonly IDirectoryScanner _directoryScanner;
        private readonly IFileAnalyzer _analyzer;
        private readonly IFileOrganizer _fileOrganizer;

        /// <inheritdoc />
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Initialize new instance of <see cref="MoviebaseApp"/>.
        /// </summary>
        /// <param name="fileScanner">File scanner.</param>
        /// <param name="analyzer">File analyzer.</param>
        /// <param name="organizer">File organizer.</param>
        /// <param name="directoryScanner">Directory scanner.</param>
        /// <param name="dal">Moviebase database access.</param>
        public MoviebaseApp(IFileScanner fileScanner, IFileAnalyzer analyzer, IFileOrganizer organizer,
            IDirectoryScanner directoryScanner, IMoviebaseDAL dal)
        {
            _fileScanner = fileScanner;
            _analyzer = analyzer;
            _fileOrganizer = organizer;
            _directoryScanner = directoryScanner;
            _dal = dal;

            _apiClient = new TMDbClient(GlobalSettings.Default.ApiKey)
            {
                DefaultLanguage = "id",
                DefaultCountry = "ID"
            };
        }

        /// <inheritdoc />
        public async Task ScanDirectoryAsync(string scanPath, CancellationToken? cancellationToken = null)
        {
            var files = _fileScanner.ScanMovie(scanPath).ToList();
            var totalItems = files.Count;

            for (var i = 0; i < files.Count; i++)
            {
                if (cancellationToken?.IsCancellationRequested == true) break;

                await ScanFileAsync(files[i]);
                OnProgressChanged(i, totalItems);
            }

            _dal.RecordScanFolder(scanPath);
        }

        /// <inheritdoc />
        public async Task ScanFileAsync(string filePath)
        {
            var analyzedFile = await _analyzer.Analyze(filePath);
            Movie movie;

            if (analyzedFile.IsKnown)
            {
                movie = _dal.GetMovieByImdbId(analyzedFile.ImdbId);
                Log.Debug("Use local movie information");
            }
            else
            {
                movie = await IdentifyFile(analyzedFile);
                Log.Debug("Use remote movie information");
            }

            // sync database
            _dal.RecordScanFile(analyzedFile, movie);
        }

        /// <inheritdoc />
        public async Task AssociateAsync(CancellationToken? cancellationToken = null)
        {
            var dirs = _directoryScanner.EnumerateDirectories().ToList();
            var totalItems = dirs.Count;

            for (int i = 0; i < dirs.Count; i++)
            {
                // cancellation
                var dir = dirs[i];
                if (cancellationToken?.IsCancellationRequested == true) break;

                // scan movie
                var files = _fileScanner.ScanMovie(dir).ToList();
                if (!files.Any()) continue;

                // record
                var analyzedFile = await _analyzer.Analyze(files.First());
                _dal.RecordExtraFile(analyzedFile, _fileScanner.ScanSubtitle(dir).FirstOrDefault(),
                    _fileScanner.ScanPoster(dir).FirstOrDefault());

                OnProgressChanged(i, totalItems);
            }
        }

        /// <inheritdoc />
        public Task OrganizeFileAsync(CancellationToken? cancellationToken = null)
        {
            return Task.Run(() =>
            {
                using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
                {
                    var mediaCollection = db.GetCollection<MediaFile>();
                    var mediaList = mediaCollection.FindAll().ToList();
                    var totalItems = mediaList.Count;

                    for (var i = 0; i < mediaList.Count; i++)
                    {
                        // cancellation
                        var mediaFile = mediaList[i];
                        if (cancellationToken?.IsCancellationRequested == true) break;

                        // organize
                        _fileOrganizer.Organize(mediaFile);

                        OnProgressChanged(i, totalItems);
                    }
                }
            });
        }

        #region Event Invocator

        private void OnProgressChanged(int current, int total)
        {
            if (total <= 0) return;

            var p = 100 * current / total;
            var e = new ProgressChangedEventArgs(p, null);
            ProgressChanged?.Invoke(this, e);
        }

        #endregion
    }
}