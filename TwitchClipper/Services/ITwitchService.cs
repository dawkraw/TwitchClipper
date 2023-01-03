using System;
using System.Threading.Tasks;
using TwitchClipper.Models;

namespace TwitchClipper.Services;

public interface ITwitchService
{
    public string AccessToken { set; }
    public string ClientId { get; }
    Task<TwitchClip[]?> GetClipsAsync(ITwitchEntity entity, DateTime dateSince, int numberOfClips);

    Task<T?> GetEntityByNameAsync<T>(string query) where T : ITwitchEntity;
}