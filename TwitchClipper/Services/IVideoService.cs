using System.IO;
using System.Threading.Tasks;

namespace TwitchClipper.Services;

public interface IVideoService
{
    Task ConcatVideosAsync(string[] videos, string outputPath);

    Task<FileInfo?> DownloadClip(string? url, string? textInfo = null);
}