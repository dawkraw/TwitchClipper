using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using TwitchClipper.Models;
using TwitchClipper.Services;

namespace TwitchClipper.ViewModels;

public class SelectEntriesViewModel : INotifyPropertyChanged
{
    private readonly ITwitchService _twitchService;
    private readonly IVideoService _videoService;

    private bool _isChoosingCategory;
    private bool _isProcessingFree = true;

    private string? _nameValue;

    private bool _showClipInfo;

    public SelectEntriesViewModel(ITwitchService twitchService, IVideoService videoService)
    {
        _twitchService = twitchService;
        _videoService = videoService;
        Entities = new ObservableCollection<ITwitchEntity>();
        Clips = new ObservableCollection<TwitchClip>();

        AddCommand = new AsyncRelayCommand(AddToList, () => _isProcessingFree);
        ProceedCommand = new AsyncRelayCommand(MakeVideo);

        ClearEntitiesCommand = new RelayCommand(ClearEntities, () => _isProcessingFree);
        ClearClipsCommand = new RelayCommand(ClearClips, () => _isProcessingFree);
        DeleteSelectedCommand = new RelayCommand<ObservableCollection<object>>(DeleteSelected, (_) => _isProcessingFree);

        SelectedDaysNumber = DaysNumber.First();
        SelectedClipsNumber = ClipsNumber.First();
    }

    public AsyncRelayCommand AddCommand { get; init; }
    public AsyncRelayCommand ProceedCommand { get; init; }
    public ObservableCollection<TwitchClip> Clips { get; set; }
    public ObservableCollection<ITwitchEntity> Entities { get; set; }
    public RelayCommand ClearEntitiesCommand { get; init; }
    public RelayCommand ClearClipsCommand { get; init; }
    public RelayCommand<ObservableCollection<object>> DeleteSelectedCommand { get; init; }

    public List<int> DaysNumber { get; set; } = Enumerable.Range(1, 31).ToList();
    public int SelectedDaysNumber { get; set; }

    public List<int> ClipsNumber { get; set; } = Enumerable.Range(1, 100).ToList();
    public int SelectedClipsNumber { get; set; }

    public string? NameValue
    {
        get => _nameValue;
        set => SetField(ref _nameValue, value);
    }

    public bool IsChoosingCategory
    {
        get => _isChoosingCategory;
        set => SetField(ref _isChoosingCategory, value);
    }

    public bool ShowClipInfo
    {
        get => _showClipInfo;
        set => SetField(ref _showClipInfo, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private async Task MakeVideo()
    {
        _isProcessingFree = false;
        var videoPaths = new List<string>();
        var dateToBringClipsFrom = DateTime.Today - TimeSpan.FromDays(SelectedDaysNumber);
        foreach (var entity in Entities)
        {
            var clips = await _twitchService.GetClipsAsync(entity, dateToBringClipsFrom, SelectedClipsNumber);
            if (clips is null)
            {
                _isProcessingFree = true;
                return;
            }
            foreach (var clip in clips)
            {
                Clips.Add(clip);
                var downloadedClip = await _videoService.DownloadClip(clip.VideoUrl,
                    ShowClipInfo ? $"{clip.Title} | {clip.ChannelName}" : null);
                if (downloadedClip is null) continue;
                videoPaths.Add(downloadedClip.FullName);
            }

            await Task.Delay(200);
        }

        var finalVideoPath = Directory.GetCurrentDirectory() + $"\\{DateTime.Now:yyyy-dd-M--HH-mm-ss}.mp4";
        await _videoService.ConcatVideosAsync(videoPaths.ToArray(), finalVideoPath);
        foreach (var path in videoPaths)
            if (File.Exists(path))
                File.Delete(path);

        MessageBox.Show($"Compilation created, placed at {finalVideoPath}");
        _isProcessingFree = true;
    }

    private void DeleteSelected(ObservableCollection<object>? selectedCollection)
    {
        if (selectedCollection == null) return;
        var selectedEntities = selectedCollection.Cast<ITwitchEntity>().ToArray();
        foreach (var entity in selectedEntities) Entities.Remove(entity);
    }

    private async Task AddToList()
    {
        if (_nameValue == null) return;
        ITwitchEntity? entity = IsChoosingCategory
            ? await _twitchService.GetEntityByNameAsync<TwitchCategory>(_nameValue)
            : await _twitchService.GetEntityByNameAsync<TwitchChannel>(_nameValue);

        if (entity is null)
        {
            MessageBox.Show("Entry not found.");
            return;
        }

        if (Entities.Contains(entity))
        {
            MessageBox.Show("You already have it in the list!");
            return;
        }

        Entities.Add(entity);
    }

    private void ClearEntities()
    {
        Entities.Clear();
    }

    private void ClearClips()
    {
        Clips.Clear();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}