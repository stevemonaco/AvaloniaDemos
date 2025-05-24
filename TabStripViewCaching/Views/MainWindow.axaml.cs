using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Specialized;
using System.Linq;
using TabStripViewCaching.ViewModels;

namespace TabStripViewCaching.Views;
public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;
    private AvaloniaList<UserControl> _tabCache = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void TabStrip_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems is not { Count: 1 })
            throw new InvalidOperationException();

        if (tabStrip?.SelectedIndex is null || tabStrip.SelectedIndex >= _tabCache.Count)
            return;

        tabStripContent.Content = _tabCache[tabStrip.SelectedIndex];
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        _tabCache.Clear();
        ViewModel.TabStripItems.CollectionChanged += TabStripItems_CollectionChanged;

        var tabViews = ViewModel.TabStripItems.Select(CreateViewForViewModel);
        _tabCache.AddRange(tabViews);

        var selectedView = _tabCache.FirstOrDefault(x => x.DataContext == ViewModel.SelectedTabStripItem);
        if (selectedView is not null)
            tabStripContent.Content = selectedView;
    }

    /// <summary>
    /// The hard part -- responding to INCC and keeping the TabStrip's view cache correct
    /// There's more work here to be done regarding managing the selected item if it's removed
    /// </summary>
    private void TabStripItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems is null)
                    throw new InvalidOperationException();

                var removeIndex = e.OldStartingIndex;
                var itemsRemoved = e.OldItems.Count;

                for (int i = itemsRemoved - 1; i >= 0; i--)
                    _tabCache.RemoveAt(removeIndex + i);
                break;

            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is null)
                    throw new InvalidOperationException();

                var addIndex = e.NewStartingIndex;
                var items = e.NewItems;

                foreach (var item in items.Cast<TabViewModel>())
                {
                    var view = CreateViewForViewModel(item);
                    _tabCache.Insert(addIndex, view);
                    addIndex++;
                }
                break;

            case NotifyCollectionChangedAction.Replace:
                if (e.NewItems is null || e.OldItems is null)
                    throw new InvalidOperationException();

                var replaceIndex = e.NewStartingIndex;

                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    var newViewModel = (TabViewModel)e.NewItems[i]!;
                    var newView = CreateViewForViewModel(newViewModel);
                    _tabCache[replaceIndex + i] = newView;
                }
                break;

            case NotifyCollectionChangedAction.Reset:
                _tabCache.Clear();
                var tabViews = ViewModel.TabStripItems.Select(CreateViewForViewModel);
                _tabCache.AddRange(tabViews);
                break;

            case NotifyCollectionChangedAction.Move:
                throw new NotSupportedException($"Collection change action '{e.Action}' is not supported.");

            default:
                throw new NotSupportedException($"Collection change action '{e.Action}' is not supported.");
        }
    }

    private UserControl CreateViewForViewModel(TabViewModel viewModel)
    {
        return viewModel switch
        {
            PersonViewModel => new PersonView() { DataContext = viewModel },
            _ => throw new NotSupportedException($"ViewModel type '{viewModel.GetType()}' is not supported.")
        };
    }
}