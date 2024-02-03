using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;

namespace ImageHotspot;

public enum ControllerButton { Up, Left, Right, Down, LeftAnalog, RightAnalog, Start, B, A, Y, X, LeftShoulder, RightShoulder, Z }

public partial class MainWindow : Window
{
    private DispatcherTimer _timer;

    public MainWindow()
    {
        InitializeComponent();
        _timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Default, Timer_Tick);
        _timer.Stop();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        lastPress.Text = "";
        _timer.Stop();
    }

    public void Controller_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button uiButton)
            return;

        var buttonName = uiButton.Name switch
        {
            nameof(buttonUp) => ControllerButton.Up,
            nameof(buttonLeft) => ControllerButton.Left,
            nameof(buttonRight) => ControllerButton.Right,
            nameof(buttonDown) => ControllerButton.Down,
            nameof(buttonLeftAnalog) => ControllerButton.LeftAnalog,
            nameof(buttonRightAnalog) => ControllerButton.RightAnalog,
            nameof(buttonStart) => ControllerButton.Start,
            nameof(buttonB) => ControllerButton.B,
            nameof(buttonA) => ControllerButton.A,
            nameof(buttonY) => ControllerButton.Y,
            nameof(buttonX) => ControllerButton.X,
            nameof(buttonL) => ControllerButton.LeftShoulder,
            nameof(buttonR) => ControllerButton.RightShoulder,
            nameof(buttonZ) => ControllerButton.Z,
            _ => throw new ArgumentException($"Button '{uiButton.Name}' could not be handled")
        };

        lastPress.Text = buttonName.ToString();
        _timer.Stop();
        _timer.Start();
    }
}