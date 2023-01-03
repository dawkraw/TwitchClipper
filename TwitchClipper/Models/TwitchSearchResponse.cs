using System.Text.Json.Serialization;

namespace TwitchClipper.Models;

public class TwitchSearchResponse<T>
{
    [JsonPropertyName("data")] public T[]? Entries { get; set; }
}