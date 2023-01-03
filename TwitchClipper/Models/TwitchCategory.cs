using System;
using System.Text.Json.Serialization;

namespace TwitchClipper.Models;

public class TwitchCategory : ITwitchEntity
{
    [JsonPropertyName("id")] public int Id { get; init; }

    [JsonPropertyName("Name")] public string? Name { get; init; }

    [JsonPropertyName("box_art_url")] public string? ThumbnailUrl { get; init; }

    public bool Equals(ITwitchEntity? other)
    {
        return other != null && Id == other.Id && Name == other.Name && ThumbnailUrl == other.ThumbnailUrl;
    }

    protected bool Equals(TwitchCategory other)
    {
        return Id == other.Id && Name == other.Name && ThumbnailUrl == other.ThumbnailUrl;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((TwitchCategory) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, ThumbnailUrl);
    }
}