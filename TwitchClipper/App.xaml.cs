using System;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TwitchClipper.Services;
using TwitchClipper.ViewModels;

namespace TwitchClipper;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly bool isInitialized;

    public App()
    {
        InitializeComponent();

        if (!isInitialized)
        {
            isInitialized = true;
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddTransient<LoginViewModel>()
                    .AddTransient<SelectEntriesViewModel>()
                    .AddSingleton<INavigationService, NavigationService>()
                    .AddSingleton<IVideoService, VideoService>()
                    .AddHttpClient<ITwitchService, TwitchService>(client =>
                    {
                        client.BaseAddress = new Uri("https://api.twitch.tv/helix/");
                    })
                    .Services.BuildServiceProvider());
        }
    }
}