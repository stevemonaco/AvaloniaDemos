using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace TextOutlineAnimation.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    public string SampleText => "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut " +
        "labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea " +
        "commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
        "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

    private bool _useTransparentForeground;
    public bool UseTransparentForeground
    {
        get => _useTransparentForeground;
        set => this.RaiseAndSetIfChanged(ref _useTransparentForeground, value);
    }

    private bool _useStrokeAnimation = true;
    public bool UseStrokeAnimation
    {
        get => _useStrokeAnimation;
        set => this.RaiseAndSetIfChanged(ref _useStrokeAnimation, value);
    }
}
