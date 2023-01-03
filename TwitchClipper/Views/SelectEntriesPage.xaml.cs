using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using TwitchClipper.ViewModels;

namespace TwitchClipper.Views;

public partial class SelectEntriesPage : Page
{
    public SelectEntriesPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<SelectEntriesViewModel>();
    }
}