using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Playnite.SDK;
using WishlistHub.Api.Models;
using WishlistHub.Settings;

namespace WishlistHub.Api.Services
{
    public class WishlistHubApiClient : IDisposable
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private readonly HttpClient _httpClient;
        private readonly WishlistHubSettings _settings;
        private readonly JsonSerializerSettings _jsonSettings;

        public WishlistHubApiClient(WishlistHubSettings settings)
        {
            _settings = settings;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver(),
            };
        }

        public async Task<ImportResponse> ImportGamesAsync(
            IReadOnlyCollection<GameWithLauncher> games,
            CancellationToken ct)
        {
            if (games == null || games.Count == 0)
            {
                throw new ArgumentException("Lista de jogos vazia.", nameof(games));
            }

            var endpoint = BuildEndpoint();
            ImportResponse last = null;
            foreach (var batch in Batch(games, 500))
            {
                var request = new ImportRequest
                {
                    Token = _settings.AuthenticationToken,
                    Data = JsonConvert.SerializeObject(batch, _jsonSettings),
                };
                var body = JsonConvert.SerializeObject(request, _jsonSettings);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                Logger.Info($"Wishlist Hub import: {batch.Count} jogos → {endpoint}");
                var response = await _httpClient.PostAsync(endpoint, content, ct).ConfigureAwait(false);
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException(responseString);
                }

                var parsed = JsonConvert.DeserializeObject<ImportResponse>(responseString, _jsonSettings);
                if (parsed == null || !parsed.Success)
                {
                    var msg = parsed?.Data?.Message ?? responseString;
                    throw new InvalidOperationException(msg);
                }

                last = Merge(last, parsed);
            }

            return last ?? new ImportResponse { Success = true, Data = new ImportResponseData { Result = new List<ImportResult>() } };
        }

        private string BuildEndpoint()
        {
            var baseUrl = (_settings.BaseUrl ?? "https://wishlist-hub.paz.poa.br").TrimEnd('/');
            return baseUrl + "/api/playnite/collection/import";
        }

        private static IEnumerable<List<GameWithLauncher>> Batch(IReadOnlyCollection<GameWithLauncher> games, int size)
        {
            var list = games.ToList();
            for (var i = 0; i < list.Count; i += size)
            {
                yield return list.Skip(i).Take(size).ToList();
            }
        }

        private static ImportResponse Merge(ImportResponse a, ImportResponse b)
        {
            if (a == null)
            {
                return b;
            }

            var results = new List<ImportResult>();
            if (a.Data?.Result != null)
            {
                results.AddRange(a.Data.Result);
            }

            if (b.Data?.Result != null)
            {
                results.AddRange(b.Data.Result);
            }

            return new ImportResponse
            {
                Success = a.Success && b.Success,
                Data = new ImportResponseData { Result = results },
            };
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
