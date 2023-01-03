using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TwitchClipper.Models;

namespace TwitchClipper.Services;

public class TwitchService : ITwitchService
{
    private static string? _accessToken;
    private readonly HttpClient _httpClient;

    public TwitchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("Client-Id", ClientId);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    public string AccessToken
    {
        set => _accessToken = value;
    }

    public string ClientId { get; } = "xz9ung5k8umh726zxiareo7m3ovk4h";

    public async Task<TwitchClip[]?> GetClipsAsync(ITwitchEntity entity, DateTime dateSince, int numberOfClips)
    {
        var dateSinceInRfc3339 = dateSince.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);

        var requestUri = string.Format("clips?{0}={1}&started_at={2}&first={3}",
            entity is TwitchCategory ? "game_id" : "broadcaster_id",
            entity.Id, Uri.EscapeDataString(dateSinceInRfc3339), numberOfClips);
        return (await _httpClient.GetFromJsonAsync<TwitchSearchResponse<TwitchClip>>(requestUri))?.Entries;
    }

    public async Task<T?> GetEntityByNameAsync<T>(string query) where T : ITwitchEntity
    {
        T? result;
        try
        {
            var requestUrl =
                $"search/{(typeof(T) == typeof(TwitchCategory) ? "categories" : "channels")}?query={query}&first=1";

            var response = await _httpClient.GetFromJsonAsync<TwitchSearchResponse<T>>(requestUrl);
            result = response?.Entries is not null ? response.Entries.First() : default;
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException)
        {
            result = default;
        }

        return result;
    }
}