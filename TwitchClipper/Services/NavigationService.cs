using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using TwitchClipper.Views;

namespace TwitchClipper.Services;

public class NavigationService : INavigationService
{
    private readonly Dictionary<string, Type> _pages = new()
    {
        {"LogInPage", typeof(LogInPage)},
        {"SelectEntriesPage", typeof(SelectEntriesPage)}
    };

    private Page? _currentPage;

    public Page? CurrentPage
    {
        get => _currentPage;
        set => SetField(ref _currentPage, value);
    }

    public event Action? CurrentPageChanged;


    public void Navigate(string destination)
    {
        if (!_pages.ContainsKey(destination)) return;
        var pageType = _pages[destination];
        CurrentPage = (Page) Activator.CreateInstance(pageType)! ?? throw new InvalidOperationException();
        CurrentPageChanged?.Invoke();
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        CurrentPageChanged?.Invoke();
        return true;
    }
}