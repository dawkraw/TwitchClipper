using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchClipper.Services;

public class VideoService : IVideoService
{
    private Process? _ffmpegProcess;

    public async Task ConcatVideosAsync(string[] videos, string outputPath)
    {
        if (IsProcessStillRunning()) return;

        var tempFile = Path.GetTempFileName().Replace("tmp", "txt");
        await File.WriteAllLinesAsync(tempFile, videos.Select(v => $"file '{v}'"));
        var processInfo = CreateProcessInfo($"-f concat -safe 0 -i {tempFile} -c copy {outputPath}");

        using (_ffmpegProcess = new Process {StartInfo = processInfo})
        {
            _ffmpegProcess.Start();
            await _ffmpegProcess.WaitForExitAsync();
            File.Delete(tempFile);
        }
    }

    public async Task<FileInfo?> DownloadClip(string? url, string? textInfo = null)
    {
        if (IsProcessStillRunning()) return null;

        var tempFile = Path.GetTempFileName().Replace("tmp", "mp4");
        var arguments = string.Format("-i \"{0}\" {1}-c:v libx264 -preset slow -crf 22 \"{2}\"", url,
            textInfo is not null
                ? $"-vf \"drawtext=text='{textInfo}': x=0: y=10: fontcolor=white: fontsize=24: box=1: boxcolor=black@0.6: boxborderw=4\" "
                : "",
            tempFile);
        var processInfo = CreateProcessInfo(arguments);

        using (_ffmpegProcess = new Process {StartInfo = processInfo})
        {
            _ffmpegProcess.Start();
            await _ffmpegProcess.WaitForExitAsync();
            return _ffmpegProcess.ExitCode == 0 ? new FileInfo(tempFile) : null;
        }
    }

    private ProcessStartInfo CreateProcessInfo(string arguments)
    {
        return new ProcessStartInfo
        {
            FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe"),
            Arguments = arguments,
            WorkingDirectory = Directory.GetCurrentDirectory(),
            CreateNoWindow = true,
            UseShellExecute = false
        };
    }

    private bool IsProcessStillRunning()
    {
        if (_ffmpegProcess is null) return false;

        try
        {
            if (_ffmpegProcess.HasExited) return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
    }
}