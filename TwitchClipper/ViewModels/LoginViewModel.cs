using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using TwitchClipper.Services;

namespace TwitchClipper.ViewModels;

public class LoginViewModel
{
    private const int Port = 52147;
    private readonly INavigationService _navigationService;
    private readonly ITwitchService _twitchService;

    public LoginViewModel(ITwitchService twitchService, INavigationService navigationService)
    {
        _twitchService = twitchService;
        _navigationService = navigationService;
        LoginCommand = new AsyncRelayCommand(LogInWithTwitch);
    }

    public AsyncRelayCommand LoginCommand { get; }

    private async Task LogInWithTwitch()
    {
        if (IsPortUsed(Port))
        {
            MessageBox.Show(
                $"There was an error, probably port {Port} is being used.\nClose any programs using this port and try again.");
            return;
        }

        var randomBytes = RandomNumberGenerator.GetBytes(32);
        var state = Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        var redirectUrl = $"http://localhost:{Port}/";

        var httpListener = new HttpListener();
        httpListener.Prefixes.Add(redirectUrl);
        httpListener.Start();

        var authUrl = string.Format(
            "https://id.twitch.tv/oauth2/authorize?response_type=token&client_id={0}&redirect_uri={1}&state={2}",
            _twitchService.ClientId, Uri.EscapeDataString(redirectUrl), state);

        var browserProcessInfo = new ProcessStartInfo(authUrl)
        {
            UseShellExecute = true
        };

        Process.Start(browserProcessInfo);

        var context = await httpListener.GetContextAsync();

        if (context.Request.QueryString.Count == 0)
            await SetupResponse(
                $"<script>window.location.href = \"{redirectUrl}/\" + document.location.hash.replace(\"#\", \"?\");</script>",
                context.Response);
        context = await httpListener.GetContextAsync();

        if (context.Request.QueryString.Get("access_token") == null
            || context.Request.QueryString.Get("state") == null
            || context.Request.QueryString.Get("state") != state)
            await SetupResponse("<h1>There was an error logging you in, try again later.</h1>", context.Response);
        else
            await SetupResponse("<h1>Successfully logged in! You can now return to the app.</h1>", context.Response);

        httpListener.Stop();
        _twitchService.AccessToken = context.Request.QueryString.Get("access_token")!;
        _navigationService.Navigate("SelectEntriesPage");
    }

    private static async Task SetupResponse(string content, HttpListenerResponse response)
    {
        var buffer = Encoding.UTF8.GetBytes(content);
        response.ContentLength64 = buffer.Length;
        var responseOutput = response.OutputStream;
        await responseOutput.WriteAsync(buffer);
        responseOutput.Close();
    }

    private bool IsPortUsed(int port)
    {
        try
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, port);
            tcpListener.Start();
            tcpListener.Stop();
            return false;
        }
        catch (SocketException)
        {
            return true;
        }
    }
}