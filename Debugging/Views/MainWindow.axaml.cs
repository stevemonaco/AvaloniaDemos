using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Input;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Debugging.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AddHandler(PointerPressedEvent, Window_PointerPressed, RoutingStrategies.Tunnel);
    }

    private void Search_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(controlHashInput.Text))
            return;

        if (!int.TryParse(controlHashInput.Text, out var searchHash))
        {
            searchResult.Text = $"Could not parse '{controlHashInput.Text}' as integer";
            return;
        }

        var visual = this.GetSelfAndVisualDescendants().FirstOrDefault(x => x.GetHashCode() == searchHash);

        if (visual is null)
        {
            searchResult.Text = $"No control found for #{searchHash}";
        }
        else
        {
            var tree = FormatVisualTree(visual);
            searchResult.Text = $"Type: {visual.GetType()}\nName: {visual.Name}\nVisual Tree: {tree}";
        }
    }

    private void Window_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(this);
        searchResult.Text = "";

        if (point.Properties.IsLeftButtonPressed && e.KeyModifiers == KeyModifiers.Control)
        {
            var tl = GetTopLevel(this);
            var inputElement = GetControlAtPoint(tl!, point.Position);

            if (inputElement is not Control control)
            {
                controlHashInput.Text = "";
                return;
            }

            var controlVisitor = control;

            while (controlVisitor.TemplatedParent is not null)
                controlVisitor = (Control)controlVisitor.TemplatedParent;

            controlHashInput.Text = controlVisitor.GetHashCode().ToString();

            e.Handled = true;
        }
    }

    private Control? GetControlAtPoint(TopLevel topLevel, Point p)
    {
        // Code adapted from DevTools (Avalonia.Diagnostics)
        return (Control?)topLevel.GetVisualsAt(p, x =>
        {
            if (x is AdornerLayer || !x.IsVisible)
            {
                return false;
            }

            return !(x is IInputElement ie) || ie.IsHitTestVisible;
        })
        .FirstOrDefault();
    }

    private static string FormatVisualTree(Visual visual)
    {
        return string.Join(" -> ", visual.GetSelfAndVisualAncestors().Reverse().Select(x => x.GetType().Name));
    }
}