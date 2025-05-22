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

        if (tabStrip is null)
            return;

        if (tabStrip.SelectedIndex >= _tabCache.Count)
            return;

        tabStripContent.Content = _tabCache[tabStrip.SelectedIndex];
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        ViewModel.TabStripItems.CollectionChanged += TabStripItems_CollectionChanged;
        _tabCache.Clear();

        var tabViews = ViewModel.TabStripItems.Select(CreateViewForViewModel);
        _tabCache.AddRange(tabViews);
    }

    private void TabStripItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems is null)
                throw new InvalidOperationException();

            var index = e.OldStartingIndex;
            var itemsRemoved = e.OldItems.Count;

            for (int i = 0; i < itemsRemoved; i++, index++)
                _tabCache.RemoveAt(index);
        }
        else if (e.Action == NotifyCollectionChangedAction.Add)
        {
            if (e.NewItems is null)
                throw new InvalidOperationException();

            var index = e.NewStartingIndex;
            var itemCount = e.NewItems.Count;
            var items = e.NewItems;

            foreach (var item in items.Cast<TabViewModel>())
            {
                var view = CreateViewForViewModel(item);
                _tabCache.Insert(index, view);
                index++;
            }
        }
    }

    private UserControl CreateViewForViewModel(TabViewModel viewModel)
    {
        var view = new PersonView() { DataContext = viewModel };
        return view;
    }
}