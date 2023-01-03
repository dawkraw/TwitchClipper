using System;
using System.Windows.Controls;

namespace TwitchClipper.Services;

public interface INavigationService
{
    public Page? CurrentPage { get; set; }

    public void Navigate(string destination);

    public event Action CurrentPageChanged;
}