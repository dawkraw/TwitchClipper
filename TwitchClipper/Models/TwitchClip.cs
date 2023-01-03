using System.Text.Json.Serialization;

namespace TwitchClipper.Models;

public class TwitchClip
{
    [JsonPropertyName("id")] public string? Id { get; init; }

    [JsonPropertyName("title")] public string? Title { get; init; }

    [JsonPropertyName("broadcaster_name")] public string? ChannelName { get; init; }

    [JsonPropertyName("thumbnail_url")] public string? ThumbnailUrl { get; init; }

    public string? VideoUrl => ThumbnailUrl?.Replace("-preview-480x272.jpg", ".mp4");
}