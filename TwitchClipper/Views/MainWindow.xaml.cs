using System;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using TwitchClipper.Services;

namespace TwitchClipper;

public partial class MainWindow : Window
{
    private readonly INavigationService _navigationService;

    public MainWindow()
    {
        _navigationService = Ioc.Default.GetRequiredService<INavigationService>();
        _navigationService.CurrentPageChanged += NavigationServiceOnCurrentPageChanged;
        _navigationService.Navigate("LogInPage");
        InitializeComponent();
    }

    private void NavigationServiceOnCurrentPageChanged()
    {
        Content = _navigationService.CurrentPage;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
    }
}