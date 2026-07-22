using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Playnite.SDK;

namespace WishlistHub.Queue
{
    /// <summary>Fila com debounce para sync automática (evita N POSTs em import grande).</summary>
    public class ImportQueue
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private readonly string _filePath;
        private readonly Func<IReadOnlyCollection<Guid>, Task> _processor;
        private readonly object _lock = new object();
        private readonly HashSet<Guid> _pending = new HashSet<Guid>();
        private CancellationTokenSource _debounceCts;
        private readonly SemaphoreSlim _runLock = new SemaphoreSlim(1, 1);
        private readonly TimeSpan _debounce;

        public ImportQueue(string filePath, Func<IReadOnlyCollection<Guid>, Task> processor, TimeSpan? debounce = null)
        {
            _filePath = filePath;
            _processor = processor;
            _debounce = debounce ?? TimeSpan.FromSeconds(8);
            Load();
        }

        public void Enqueue(IEnumerable<Guid> gameIds)
        {
            lock (_lock)
            {
                foreach (var id in gameIds)
                {
                    _pending.Add(id);
                }

                Save();
                _debounceCts?.Cancel();
                _debounceCts = new CancellationTokenSource();
                var token = _debounceCts.Token;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(_debounce, token).ConfigureAwait(false);
                        await ProcessAsync().ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                        // novo enqueue reiniciou o debounce
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "WishlistHub queue debounce failed");
                    }
                });
            }
        }

        public void ProcessInBackground()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await ProcessAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "WishlistHub queue process failed");
                }
            });
        }

        public async Task ProcessAsync()
        {
            if (!await _runLock.WaitAsync(0).ConfigureAwait(false))
            {
                return;
            }

            try
            {
                List<Guid> batch;
                lock (_lock)
                {
                    if (_pending.Count == 0)
                    {
                        return;
                    }

                    batch = _pending.ToList();
                    _pending.Clear();
                    Save();
                }

                await _processor(batch).ConfigureAwait(false);
            }
            finally
            {
                _runLock.Release();
            }
        }

        private void Load()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return;
                }

                var json = File.ReadAllText(_filePath);
                var ids = JsonConvert.DeserializeObject<List<Guid>>(json) ?? new List<Guid>();
                lock (_lock)
                {
                    foreach (var id in ids)
                    {
                        _pending.Add(id);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load WishlistHub queue");
            }
        }

        private void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(_filePath, JsonConvert.SerializeObject(_pending.ToList()));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to save WishlistHub queue");
            }
        }
    }
}
