using System;

namespace TwitchClipper.Models;

public interface ITwitchEntity : IEquatable<ITwitchEntity>
{
    public int Id { get; init; }

    public string? Name { get; init; }

    public string? ThumbnailUrl { get; init; }
}