using Avalonia.Controls;

namespace BlockPatternAnimation.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        window.PropertyChanged += Window_PropertyChanged;
    }

    protected override void OnLoaded()
    {
        base.OnLoaded();
        ResizeMainGrid();
    }

    private void Window_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == "ClientSize")
        {
            ResizeMainGrid();
        }
    }

    private void ResizeMainGrid()
    {
        // This is a workaround to ensure the pattern extends one full repeat outside of the window size
        // so that content is filled during the translation tranform animation of the grid layers
        mainGrid.Width = window.Width + 400;
        mainGrid.Height = window.Height + 400;
    }
}
