using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using LiteDB;
using Moviebase.Core.Components;
using Moviebase.DAL;
using Moviebase.DAL.Entities;
using Moviebase.Services.Title;
using TMDbLib.Client;

namespace Moviebase.Core.App
{
    /// <inheritdoc />
    public sealed partial class MoviebaseApp : IMoviebaseApp
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MoviebaseApp));
        private static readonly Lazy<IMoviebaseApp> AppLazy = new Lazy<IMoviebaseApp>(() => new MoviebaseApp());

        private readonly TMDbClient _apiClient;
        private readonly IMoviebaseDAL _dal;
        private readonly IPathTransformer _pathTransformer;
        private readonly IFileScanner _fileScanner;
        private readonly IFileAnalyzer _fileAnalyzer;
        private readonly IFileOrganizer _fileOrganizer;

        /// <inheritdoc />
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Gets current instance of <see cref="MoviebaseApp"/>.
        /// </summary>
        public static IMoviebaseApp Instance => AppLazy.Value;

        /// <summary>
        /// Initialize new instance of <see cref="MoviebaseApp"/>.
        /// </summary>
        private MoviebaseApp()
        {
            _dal = new MoviebaseDAL();
            _pathTransformer = new PathTransformer();

            _fileScanner = new FileScanner();
            _fileAnalyzer = new FileAnalyzer(new CompositeTitleProvider());
            _fileOrganizer = new FileOrganizer(new FolderCleaner(), _pathTransformer);

            _apiClient = new TMDbClient(GlobalSettings.Default.ApiKey);

            ReloadSettings();
        }

        /// <inheritdoc />
        public void ReloadSettings()
        {
            var settings = GlobalSettings.Default;

            _pathTransformer.TargetPath = settings.TargetPath;
            _pathTransformer.TokenTemplate = settings.RenameTemplate;
            _pathTransformer.SwapThe = true;
            _fileAnalyzer.PosterExtensions = new List<string> { "jpg", "png" };
            _fileAnalyzer.SubtitleExtensions = new List<string> { "srt", "ass", "ssa" };
            _fileScanner.MovieExtensions = new List<string> { "mkv", "mp4", "avi" };
            _fileOrganizer.DeleteEmptyDirectories = true;

            _apiClient.DefaultCountry = "ID";
            _apiClient.DefaultLanguage = "id";
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
        }

        /// <inheritdoc />
        public async Task ScanFileAsync(string filePath)
        {
            var analyzedFile = await _fileAnalyzer.Analyze(filePath);
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

        #endregion Event Invocator
    }
}