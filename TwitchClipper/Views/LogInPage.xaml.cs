using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using TwitchClipper.ViewModels;

namespace TwitchClipper.Views;

public partial class LogInPage : Page
{
    public LogInPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<LoginViewModel>();
    }
}