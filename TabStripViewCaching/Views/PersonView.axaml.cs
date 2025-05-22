using Avalonia.Controls;
using System;

namespace TabStripViewCaching.Views;
public partial class PersonView : UserControl
{
    public PersonView()
    {
        InitializeComponent();
    }

    private void RandomizeBackground_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var random = Random.Shared;
        var color = Avalonia.Media.Color.FromArgb(255, (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        Background = new Avalonia.Media.SolidColorBrush(color);
    }
}